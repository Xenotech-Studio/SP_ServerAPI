using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        

        public IEnumerator JoinCoroutine()
        {
            _uuid = System.Guid.NewGuid().ToString();
            if (!ServerAPI.IsConnected)
            {
                ServerAPI.Connect();
                yield return new WaitForSeconds(1);
            }
            
            ServerAPI.AddListener("player_counter", (MessageInfo info) =>
            {
                string action = info.Message.Split(":")[0];
                string senderUuid = info.Message.Split(":")[1];
                
                //Debug.Log("player_counter: " + action + " from " + senderUuid + $"({_uuid})");

                if (action == "request")
                {
                    if(senderUuid == _uuid) { return; }
                    if(_host_uuid == "unknown") { _host_uuid = _uuid; }
                    ServerAPI.Send("player_counter", "response:" + _uuid + ":" + senderUuid + ":" + _host_uuid);
                }
                else if (action == "response")
                {
                    if (senderUuid == _uuid) { return; }
                    
                    Debug.Log("response from " + senderUuid);

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
                    OtherPlayers.Remove(senderUuid);
                    GameObject.Destroy(GameObject.Find(senderUuid));
                }
            });
            
            ServerAPI.Send("player_counter","request:" + _uuid);
            yield return new WaitForSeconds(1); // wait for any possible responses
            
            isInRoom = true;
        }
        
        public void Leave()
        {
            ServerAPI.Send("player_counter","leave:" + _uuid);
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
        
        private void GenerateOtherPlayer(string uuid)
        {
            GameObject otherPlayer = GameObject.Instantiate(OtherPlayerPrefab, PlayerParent, true);
            otherPlayer.name = "Player(" + uuid + ")";
            otherPlayer.transform.localPosition = new Vector3(0, 0, 0);
            otherPlayer.gameObject.SetActive(true);
            OtherPlayers.Add(uuid, otherPlayer);
            
            RegisterPoseListeners(uuid, otherPlayer);
        }
    }
}