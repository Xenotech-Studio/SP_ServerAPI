using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        string playerPoseChannelName => $"player_pose_{(group == 1 ? "80" : "81")}";

        private Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
        private Dictionary<string, Quaternion> rotations = new Dictionary<string, Quaternion>();
        
        public void RegisterPoseListeners(string targetUuid, GameObject player)
        {
            positions.Add(targetUuid, new Vector3());
            rotations.Add(targetUuid, new Quaternion());
            ServerAPI.AddListener(playerPoseChannelName, (MessageInfo info) =>
            {
                //return;
                string senderUuid = info.Message.Split(":")[0];
                if (senderUuid == _uuid) { return; }

                string[] positionStr = info.Message.Split(":")[1].Split(",");
                string[] rotationStr = info.Message.Split(":")[2].Split(",");

                //Debug.Log("received user position update" + info.Message + "(self is "+ targetUuid + ")");
                positions[targetUuid] = new Vector3(float.Parse(positionStr[0]), float.Parse(positionStr[1]), float.Parse(positionStr[2]));
                rotations[targetUuid] = new Quaternion(float.Parse(rotationStr[0]), float.Parse(rotationStr[1]), float.Parse(rotationStr[2]), float.Parse(rotationStr[3]));
            });
        }

        public void UpdateOtherPlayersPose()
        {
            foreach (string uuid in OtherPlayers.Keys)
            {
                OtherPlayers[uuid].transform.localPosition = positions[uuid];
                OtherPlayers[uuid].transform.localRotation = rotations[uuid];
            }
        }

        private string poseReportingMessage;
        
        public void ReportMyPose()
        {
            Vector3 position = SelfPlayer.localPosition;
            //Debug.Log(position);
            Quaternion rotation = SelfPlayer.localRotation;
            poseReportingMessage = _uuid + ":" + 
                                   position.x + "," + position.y + "," + position.z + ":"  +
                                   rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;
        }
        
        private IEnumerator PoseReportingCoroutine()
        {
            while (true)
            {
                ServerAPI.Send(playerPoseChannelName, poseReportingMessage);
                yield return null;
            }
        }
        
        Coroutine poseReportingCoroutine;
        
    }
}