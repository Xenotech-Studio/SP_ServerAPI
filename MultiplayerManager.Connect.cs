using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !SERVERAPI_NOT_PRODUCT
using Union.Framework;
using XingHan;
#endif


namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        int group = -1;
        string playerCounterChannelName => $"player_counter_{(group == 0 ? "" : group == 1 ? "80" : "81")}";

        public IEnumerator JoinCoroutine()
        {
            _uuid = System.Guid.NewGuid().ToString();
            if (!ServerAPI.IsConnected)
            {
                ServerAPI.Connect();
                yield return new WaitForSeconds(1);
            }

            var ip = ServerAPI.GetLocalIp();
            ServerAPI.AddListener(ip, info => {
                Debug.Log("Received group number: " + info.Message);
                group                       = int.Parse(info.Message);
                #if !SERVERAPI_NOT_PRODUCT
                DataManager.Instance.@group = @group;
                #endif
            });
            Debug.Log("Listening to " + ip);
            ServerAPI.Send("toComputer", $"get_group|{ip}");

            float timer = 0;
            while (/*group == -1 &&*/ timer < 5)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            if (group == -1)
            {
                Debug.LogError("Failed to get group number.");
                group = 0;
            }
            Debug.Log("Group: " + group);
            
            RegisterDataListeners(_uuid);

            Debug.Log("Data Listeners Registered, Adding Listener to PlayerCounterChannel...");
            ServerAPI.AddListener(playerCounterChannelName, (MessageInfo info) =>
            {
                string action = info.Message.Split(":")[0];
                string senderUuid = info.Message.Split(":")[1];
                #if !SERVERAPI_NOT_PRODUCT
                if (!DataManager.Instance.AllPlayers.Contains(_uuid))
                {
                    DataManager.Instance.AllPlayers.Add(_uuid); 
                }
                #endif
                if (action == "request")
                {
                    if(senderUuid == _uuid) { return; };
                    Debug.Log("received REQUEST from " + senderUuid + ", content:\n"+info.Message);
                    if(_host_uuid == "unknown") { _host_uuid = _uuid; }
                    ServerAPI.Send(playerCounterChannelName, "response:" + _uuid + ":" + senderUuid + ":" + _host_uuid);
                    newPlayerToGenerate.Add(senderUuid);
                }
                else if (action == "response")
                {
                    if (senderUuid == _uuid) { return; }
                    
                    // Debug.Log("response from " + senderUuid);
                    Debug.Log("received RESPONSE from " + senderUuid + ", content:\n" +info.Message);

                    string requesterUuid = info.Message.Split(":")[2];
                    string hostUuid      = info.Message.Split(":")[3];
                    
                    if (requesterUuid == _uuid)
                    {
                        newPlayerToGenerate.Add(senderUuid);
                    }
                    
                    _host_uuid = hostUuid;
                }
                else if (action == "leave")
                {
                    if (senderUuid == _host_uuid)
                    {
                        _host_uuid = "unknown";
                    }
                    playersToDestroy.Add(senderUuid);
                }
                
            });
            Debug.Log("Sending REQUEST...");
            ServerAPI.Send(playerCounterChannelName,"request:" + _uuid);
            yield return new WaitForSeconds(1); // wait for any possible responses
            
            isInRoom = true;
        }
        
        public void Leave()
        {
            ServerAPI.Send(playerCounterChannelName,"leave:" + _uuid);
        }

        public void CheckForNewPlayerToGenerate()
        {
            if (newPlayerToGenerate.Count>0){
                foreach (string uuid in newPlayerToGenerate)
                {
                    GenerateOtherPlayer(uuid);
                }
                newPlayerToGenerate.Clear();
            }
        }
        
        public void CheckForPlayersToDestroy()
        {
            if (playersToDestroy.Count>0){
                foreach (string uuid in playersToDestroy)
                {
                    DestroyOtherPlayer(uuid);
                }
                playersToDestroy.Clear();
            }
        }
        
        private void GenerateOtherPlayer(string uuid)
        {
            if (OtherPlayers.ContainsKey(uuid)) { return; }
            GameObject otherPlayer = GameObject.Instantiate(OtherPlayerPrefab, PlayerParent ,true);
            otherPlayer.name = "Player(" + uuid + ")";
            Debug.Log(otherPlayer.name);
            otherPlayer.transform.localPosition = new Vector3(0, 0, 0);
            otherPlayer.gameObject.SetActive(true);
            OtherPlayers.Add(uuid, otherPlayer);
            Debug.Log(OtherPlayers.Count);
            #if !SERVERAPI_NOT_PRODUCT
            if (!DataManager.Instance.AllPlayers.Contains(uuid))
            {
                DataManager.Instance.AllPlayers.Add(uuid); 
            }
            #endif
            RegisterPoseListeners(uuid, otherPlayer);
            //RegisterDataListeners(uuid, otherPlayer);
           
        }
        
        private void DestroyOtherPlayer(string uuid)
        {
            if (!OtherPlayers.ContainsKey(uuid)) { return; }
            GameObject.Destroy(OtherPlayers[uuid]);
            OtherPlayers.Remove(uuid);
            #if !SERVERAPI_NOT_PRODUCT
            if (DataManager.Instance.AllPlayers.Contains(uuid))
            {
                DataManager.Instance.AllPlayers.Remove(uuid); 
            }
            #endif
        }
    }
}