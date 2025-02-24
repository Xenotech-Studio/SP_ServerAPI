using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace DataSystem.Http
{
   
    public partial class MultiplayerManager
    {
        private string[] StateNames = new[] { "Idle", "Walk", "Sit"};
        

        string playerPoseChannelName => $"player_pose_{(group == 0 ? "" : group == 1 ? "80" : "81")}";

        public SerializedDictionary<string, Vector3> positions  = new SerializedDictionary<string, Vector3>();
        private Dictionary<string, Quaternion>        rotations  = new Dictionary<string, Quaternion>();
        private Dictionary<string, string>            animStates = new Dictionary<string, string>();

        private Dictionary<string, Animator> _animatorsCache = new Dictionary<string, Animator>();
        private Dictionary<string, string> _animatorStateCache = new Dictionary<string, string>();
        
        private SerializedDictionary<string, bool> AniSetWalk = new SerializedDictionary<string, bool>();
        private SerializedDictionary<string, bool> AniSetSit = new SerializedDictionary<string, bool>();
        
        //private bool _receiveFlag = false;
        
        private long _otherTimestamp = 0;
        private long _selfTimestamp = 0;
        private bool _reportAnyway = false;
        const long MAX_TIMESTAMP = 99999999999;
        
        public void RegisterPoseListeners(string targetUuid, GameObject player)
        {
            positions.Add(targetUuid, new Vector3());
            rotations.Add(targetUuid, new Quaternion());
            animStates.Add(targetUuid, "Idle");
            AniSetWalk.Add(targetUuid, false);
            AniSetSit.Add(targetUuid, false);
            _reportAnyway = true;
           
            ServerAPI.AddListener(playerPoseChannelName, (MessageInfo info) =>
            {
                string[] splitMessage = info.Message.Split(":");
                
                long senderTimestamp = long.Parse(splitMessage[0]);
	            string senderUuid = splitMessage[1];
                
	            if (senderUuid == _uuid) { return; }
                
                if (senderTimestamp > _otherTimestamp || (_otherTimestamp >= MAX_TIMESTAMP - 2 && senderTimestamp<2) ) _otherTimestamp = senderTimestamp;

                string[] positionStr  = splitMessage[2].Split(",");
                string[] rotationStr  = splitMessage[3].Split(",");
                string[] aniSetBool = splitMessage[4].Split(",");

                //Debug.Log("received user position update" + info.Message + "(self is "+ targetUuid + ")");
                positions[senderUuid] = new Vector3(float.Parse(positionStr[0]), float.Parse(positionStr[1]), float.Parse(positionStr[2]));
                rotations[senderUuid] = new Quaternion(float.Parse(rotationStr[0]), float.Parse(rotationStr[1]), float.Parse(rotationStr[2]), float.Parse(rotationStr[3]));
                
                //animStates[senderUuid]      = animStateStr;
                AniSetWalk[senderUuid] = Boolean.Parse(aniSetBool[0]);
                AniSetSit[senderUuid] = Boolean.Parse(aniSetBool[1]);

            });
            
        }

        public void UpdateOtherPlayersPose()
        {
            foreach (string uuid in OtherPlayers.Keys)
            {
                OtherPlayers[uuid].transform.localPosition = positions[uuid];
                OtherPlayers[uuid].transform.localRotation = rotations[uuid];

                //if (GetAnimatorState(uuid) != animStates[uuid])
                //{
                //    GetAnimator(uuid).CrossFade(animStates[uuid], 0.1f);
                //    _animatorStateCache[uuid] = animStates[uuid];
                //}
                //OtherPlayers[uuid].GetComponentInChildren<Animator>().Play(animStates[uuid]);
                OtherPlayers[uuid].GetComponentInChildren<Animator>().SetBool("IsWalking",AniSetWalk[uuid]);
                OtherPlayers[uuid].GetComponentInChildren<Animator>().SetBool("IsSitting",AniSetSit[uuid]);
                
            }
        }

        private string poseReportingMessage;
        
        public void ReportMyPose()
        {
            Vector3 position = SelfPlayer.localPosition;
            //Debug.Log(position);
            Quaternion rotation = SelfPlayer.localRotation;

            bool isWalk = SelfAnimator.GetBool("IsWalking");
            bool isSit = SelfAnimator.GetBool("IsSitting");

            /*string animStateStr = "Unknown";
            foreach (var stateName in StateNames)
            {
                if (SelfAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                {
                    animStateStr = stateName;
                }
            }*/
            
            //poseReportingMessage = _uuid + ":" + 
            //                       position.x + "," + position.y + "," + position.z + ":"  +
            //                       rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w;
            //同步玩家的位置x z 旋转y
            poseReportingMessage = _uuid + ":" + position.x + "," + 0 + "," + position.z + ":" + 
                                   0 + "," + rotation.y + "," + 0 + "," + rotation.w + ":" + 
                                   /*animStateStr*/isWalk+","+isSit;
        }
        
        private IEnumerator PoseReportingCoroutine()
        {
            while (true)
            {
                if (_otherTimestamp >= _selfTimestamp || (_otherTimestamp<2 && _selfTimestamp > MAX_TIMESTAMP-2) || _reportAnyway)
                {
                    _selfTimestamp = _otherTimestamp + 1;
                    if (_selfTimestamp > MAX_TIMESTAMP) _selfTimestamp = 0;
                    ServerAPI.Send(playerPoseChannelName, _selfTimestamp + ":" + poseReportingMessage);
                    _reportAnyway = false;
                }
                
                yield return null;
            }
        }
        
        Coroutine poseReportingCoroutine;

        private Animator GetAnimator(string uuid)
        {
            if (_animatorsCache.ContainsKey(uuid) && _animatorsCache[uuid]!=null)
            {
                return _animatorsCache[uuid];
            }
            else
            {
                _animatorsCache[uuid] = OtherPlayers[uuid].GetComponentInChildren<Animator>();
                return _animatorsCache[uuid];
            }
        }
        
        private string GetAnimatorState(string uuid)
        {
            if (_animatorStateCache.ContainsKey(uuid))
            {
                return _animatorStateCache[uuid];
            }
            else
            {
                //_animatorStateCache[uuid] = animStates[uuid];
                //return _animatorStateCache[uuid];
                return null;
            }
        }
    }
}