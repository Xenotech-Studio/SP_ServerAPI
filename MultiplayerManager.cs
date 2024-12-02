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

        public string _uuid;

        public string _host_uuid = "unknown";
        
        private bool isInRoom = false;
        
        public Transform PlayerParent;

        private Dictionary<string, GameObject> OtherPlayers = new Dictionary<string, GameObject>();
        private List<string> newPlayerToGenerate = new List<string>();
        private List<string> playersToDestroy = new List<string>();
        

        public bool IsConnected() => ServerAPI.IsConnected;
        
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
            Leave();
        }

        public void Update()
        {
            if (IsInRoom())
            {
                CheckForNewPlayerToGenerate();
                ReportMyPose();
                UpdateOtherPlayersPose();
                CheckForPlayersToDestroy();
                
                if(poseReportingCoroutine == null) poseReportingCoroutine = StartCoroutine(PoseReportingCoroutine());
            }
        }
    }
}