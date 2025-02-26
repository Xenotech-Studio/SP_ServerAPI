using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DataSystem.Http
{
    
    public partial class MultiplayerManager : MonoBehaviour
    {
        
        public GameObject OtherPlayerPrefab;

        public Transform SelfPlayer;

        private Animator selfAnimator;

        public Animator SelfAnimator
        {
            get
            {
                if (selfAnimator == null && SelfPlayer!=null)
                {
                    selfAnimator = SelfPlayer.GetComponentInChildren<Animator>();
                }
                return selfAnimator;
            }
        }

        public string _uuid;

        public string _host_uuid = "unknown";
        
        private bool isInRoom = false;
        
        public Transform PlayerParent;

        public  Dictionary<string, GameObject> OtherPlayers        = new Dictionary<string, GameObject>();
        private List<string>                             newPlayerToGenerate = new List<string>();
        private List<string>                             playersToDestroy    = new List<string>();
        
        public bool  isPlay;
        public bool   IsConnected() => ServerAPI.IsConnected;
        
        public bool IsInRoom() => isInRoom;
        
        public bool IsHost() => _uuid == _host_uuid || _host_uuid == "unknown";
        
        public void OnEnable()
        {
            if (PlayerParent == null) PlayerParent = this.transform;
            StartCoroutine(JoinCoroutine());
        }

        public void OnDisable()
        {
            StopCoroutine(poseReportingCoroutine);
            //Leave();
        }

        public void Update()
        {
            if (IsInRoom())
            {
                CheckForNewPlayerToGenerate();
                ReportMyPose();
                UpdateOtherPlayersPose();
                CheckForPlayersToDestroy();
                CheckForPlayersToTimeout();
                ReestablishHostUpdate();
                
                if(poseReportingCoroutine == null) poseReportingCoroutine = StartCoroutine(PoseReportingCoroutine());
              
                UpdateOtherPlayersData();
            }
        }
    }
}