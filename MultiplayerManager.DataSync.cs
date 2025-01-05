using System;
using System.Collections;
using System.Collections.Generic;
using Union.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using XingHan;

namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        private List<string> PlayerUUid = new List<string>();
        private bool         is_Play;

        public void RegisterDataListeners(string targetUuid)
        {
            var instance = FindObjectOfType<HardwareStatusManager>();
            instance.OnHardwareStatus();
            ServerAPI.AddListener("gameprogress", (MessageInfo info) =>
                                                  {
                                                      string action = info.Message;
                                                      if (action == "start")
                                                      {
                                                          //游戏开始
                                                          DataManager.Instance.isStartGame = true;
                                                      }
                                                      else if (action == "skip")
                                                      {
                                                          DataManager.Instance.isJumpWait = true;
                                                      }
                                                  });
            
            ServerAPI.AddListener("PlayAnimation", (MessageInfo info) =>
                                                   {
                                                       string action = info.Message.Split(":")[0];
                                                       bool   IsPlay = Boolean.Parse(info.Message.Split(":")[1]);
                                                       if (action == "play")
                                                       {
                                                           DataManager.Instance.isOpenDoor = IsPlay;
                                                       }
                                                   });
            ServerAPI.AddListener("EnterVehicle1", (MessageInfo info) =>
                                                   {
                                                       string playerUUid = info.Message;

                                                       if (!DataManager.Instance.Vehicle1PlayerUUids.Contains(playerUUid))
                                                       {
                                                           DataManager.Instance.Vehicle1PlayerUUids.Add(playerUUid);
                                                       }
                                                   });
            ServerAPI.AddListener("PlayVehicle1Video", (MessageInfo info) =>
                                                       {
                                                           string action = info.Message;
                                                           Debug.Log(action);
                                                           if (action == "IsPlay")
                                                           {
                                                               Debug.Log("载具一等待结束");
                                                               DataManager.Instance.isJumpWait = true;
                                                           }
                                                       });
            ServerAPI.AddListener("EnterVehicle2", (MessageInfo info) =>
                                                   {
                                                       string playerUUid = info.Message;

                                                       if (!DataManager.Instance.Vehicle2PlayerUUids.Contains(playerUUid))
                                                       {
                                                           DataManager.Instance.Vehicle2PlayerUUids.Add(playerUUid);
                                                       }
                                                   });
            ServerAPI.AddListener("PlayVehicle2Video", (MessageInfo info) =>
                                                       {
                                                           string action = info.Message;
                                                           if (action == "IsPlay")
                                                           {
                                                               Debug.Log("载具二等待结束");
                                                               DataManager.Instance.isJumpWait = true;
                                                           }
                                                       });
        }

        public void UpdateOtherPlayersData()
        {
            
          
        }
    }
}
