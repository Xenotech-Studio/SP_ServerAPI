using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        int group = 0;
        string playerCounterChannelName => $"player_counter_{(group == 1 ? "80" : "81")}";

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
                group = int.Parse(info.Message);
            });
            ServerAPI.Send("toComputer", $"get_group|{ip}");

            while (group == 0)
            {
                yield return null;
            }

            ServerAPI.AddListener(playerCounterChannelName, (MessageInfo info) =>
            {
                string action = info.Message.Split(":")[0];
                string senderUuid = info.Message.Split(":")[1];

                if (action == "request")
                {
                    if(senderUuid == _uuid) { return; }
                    if(_host_uuid == "unknown") { _host_uuid = _uuid; }
                    ServerAPI.Send(playerCounterChannelName, "response:" + _uuid + ":" + senderUuid + ":" + _host_uuid);
                    newPlayerToGenerate.Add(senderUuid);
                }
                else if (action == "response")
                {
                    if (senderUuid == _uuid) { return; }
                    
                    // Debug.Log("response from " + senderUuid);

                    string requesterUuid = info.Message.Split(":")[2];
                    string hostUuid = info.Message.Split(":")[3];
                    
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
            GameObject otherPlayer = GameObject.Instantiate(OtherPlayerPrefab, PlayerParent, true);
            otherPlayer.name = "Player(" + uuid + ")";
            otherPlayer.transform.localPosition = new Vector3(0, 0, 0);
            otherPlayer.gameObject.SetActive(true);
            OtherPlayers.Add(uuid, otherPlayer);
            
            RegisterPoseListeners(uuid, otherPlayer);
        }
        
        private void DestroyOtherPlayer(string uuid)
        {
            if (!OtherPlayers.ContainsKey(uuid)) { return; }
            GameObject.Destroy(OtherPlayers[uuid]);
            OtherPlayers.Remove(uuid);
        }
    }
}