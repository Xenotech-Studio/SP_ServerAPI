using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DataSystem.Http
{
    public class HardwareStatusManager : MonoBehaviour
    {
        private Queue<string> hardwareStatusQueue = new Queue<string>();
        private Coroutine processHardwareStatusCoroutine;
        public Vector2 doorAngle; // 存储 DoorAngle 值

        public Vector2 DoorAngle 
        { 
            get { return doorAngle; }
            private set
            {
                doorAngle = value;
                DebugOnDoorAngleChanged.Invoke($"{doorAngle.x}, {doorAngle.y}");
            } 
        }
        
        public UnityEvent<string> DebugOnDoorAngleChanged;

        // 启用时注册监听器
        private void OnEnable()
        {
           //if (!ServerAPI.IsConnected)
           //{
           //    ServerAPI.Connect();
           //}
           //
            // 注册监听 hardware_status 频道
            //ServerAPI.AddListener("hardware_status", OnHardwareStatusReceived);
            //processHardwareStatusCoroutine = StartCoroutine(ProcessHardwareStatusCoroutine());
        }

        // 禁用时清理监听器
        private void OnDisable()
        {
            if (processHardwareStatusCoroutine != null)
            {
                StopCoroutine(processHardwareStatusCoroutine);
            }

            Debug.Log("HardwareStatusManager: Disabled and no longer listening.");
        }
        
        public void OnHardwareStatus()
        {
            // 注册监听 hardware_status 频道
            ServerAPI.AddListener("hardware_status", OnHardwareStatusReceived);
            processHardwareStatusCoroutine = StartCoroutine(ProcessHardwareStatusCoroutine());
        }

        // 回调方法，处理收到的 hardware_status 消息
        private void OnHardwareStatusReceived(MessageInfo info)
        {
            lock (hardwareStatusQueue)
            {
                hardwareStatusQueue.Enqueue(info.Message);
            }
        }

        // 协程处理消息队列中的内容
        private IEnumerator ProcessHardwareStatusCoroutine()
        {
            Debug.Log("HardwareStatusManager: Started processing hardware_status messages.");
            
            while (true)
            {
                if (hardwareStatusQueue.Count > 0)
                {
                    string message;
                    lock (hardwareStatusQueue)
                    {
                        message = hardwareStatusQueue.Dequeue();
                    }

                    try
                    {
                        // Debug.Log($"Hardware Status Update: {message}");

                        string[] parts = message.Split(":");
                        if (parts.Length == 2 && parts[0].Trim() == "DoorAngle")
                        {
                            string[] valueParts = parts[1].Trim().Split(",");
                            if (valueParts.Length == 2 && 
                                float.TryParse(valueParts[0].Trim(), out float angle1) &&
                                float.TryParse(valueParts[1].Trim(), out float angle2))
                            {
                                DoorAngle                      = new Vector2(angle1, angle2);
                                DataManager.Instance.DoorAngle = angle2;
                                Debug.Log($"Updated DoorAngle: {DoorAngle}");
                            }
                            else
                            {
                                Debug.LogWarning("Invalid DoorAngle values received.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error processing hardware_status message: {ex.Message}");
                    }
                }

                yield return null; // 等待下一帧继续处理
            }
        }

# if UNITY_EDITOR
        // 模拟 door 消息的发送
        // 注意，我并不知道硬件消息的具体含义，这个需要联系一下硬件负责人。
        // 以下模拟消息来自一次实际测试的时候的打印结果
        //[MenuItem("XenoSDK/Simulate Door Message")]
        public static void SimulateDoorMessage()
        {
            var instance = FindObjectOfType<HardwareStatusManager>();
            if (instance == null)
            {
                Debug.LogError("HardwareStatusManager not found in the scene.");
                return;
            }

            instance.StartCoroutine(instance.SimulateDoorMessageCoroutine());
        }
# endif
        
        public IEnumerator SimulateDoorMessageCoroutine()
        {
            /* 注意，我并不知道硬件消息的具体含义，这个需要联系一下硬件负责人。
               以下模拟消息来自一次实际测试的时候的打印结果。 */

            float elapsedTime = 0f;
            while (elapsedTime < 3f)
            {
                string message = (elapsedTime % 1f < 0.5f) ? "DoorAngle:1601, 0.0" : "DoorAngle:1602, 1.0";
                ServerAPI.Send("hardware_status", message);
                Debug.Log($"Simulated message sent: {message}");
                elapsedTime += Time.deltaTime;
                yield return null; // 每帧发送
            }
        }
    }
}
