using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEditor;
using UnityEngine;

namespace DataSystem.Http
{
    public partial class ServerAPI
    {
        
        private static WebSocket ws;
        
        private Action<string> onMessageReceived;
        
        public static bool IsConnected => ws != null && ws.IsAlive;

        public static void Connect()
        {
            if (IsConnected) return;

            // ws = new WebSocket("ws://fashion.xenotech.studio/api/channels");
            ws = new WebSocket(url: $"ws://{Hostname}/channels");

            ws.OnMessage += (sender, e) =>
            {
                // Debug.Log("Raw Message received: " + e.Data);

                // 解析消息格式
                // 假设消息格式为：`Message from Address(host='127.0.0.1', port=61227): toA: Hello from B`
                string rawMessage = e.Data;

                try
                {
                    int senderStart = rawMessage.IndexOf("Address(");
                    int senderEnd = rawMessage.IndexOf("):");
                    if (senderStart != -1 && senderEnd != -1)
                    {
                        string senderInfo = rawMessage.Substring(senderStart, senderEnd - senderStart) + ")";
                        string remainingMessage = rawMessage.Substring(senderEnd + 2).Trim();

                        // 从消息中提取 Channel 和 Message
                        int channelSeparator = remainingMessage.IndexOf(":");
                        if (channelSeparator != -1)
                        {
                            string channel = remainingMessage.Substring(0, channelSeparator).Trim();
                            string message = remainingMessage.Substring(channelSeparator + 1).Trim();

                            var messageInfo = new MessageInfo
                            {
                                Sender = senderInfo,
                                Channel = channel,
                                Message = message
                            };

                            //Debug.Log($"Parsed Message: {messageInfo}");

                            // 分发给监听器
                            if (channelListeners.ContainsKey(channel))
                            {
                                try
                                {
                                    channelListeners[channel]?.Invoke(messageInfo);
                                    //Debug.Log($"Channel {channel} listeners invoked.");
                                }
                                catch (Exception ex)
                                {
                                    //Debug.LogError("Error invoking channel listener: " + ex.Message + $"\n{ex.StackTrace}");
                                }
                            }
                            //else Debug.Log($"Channel {channel} does not have listeners.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Debug.LogError("Failed to parse message: " + ex.Message + "\nRaw Message: " + rawMessage);
                }
            };


            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Connected to WebSocket server.");
            };

            ws.OnClose += (sender, e) =>
            {
                Debug.Log("Disconnected from WebSocket server." + e.Reason);
            };
            
            ws.OnError += (sender, e) =>
            {
                Debug.LogError("WebSocket error: " + e.Message);
            };

            ws.Connect();
        }
        
        public static void Disconnect()
        {
            if (ws != null && ws.IsAlive)
            {
                ws.Close();
            }
        }

        public static void Send(string channel, string message)
        {
            //if (ws == null || !ws.IsAlive) return;

            string payload = $"{channel}: {message}";
            ws.Send(payload);
        }

        private static Dictionary<string, Action<MessageInfo>> channelListeners = new Dictionary<string, Action<MessageInfo>>();

        public static void AddListener(string channel, Action<MessageInfo> callback)
        {
            if (channelListeners == null) channelListeners = new Dictionary<string, Action<MessageInfo>>();

            if (!channelListeners.ContainsKey(channel))
            {
                channelListeners[channel] = callback;
            }
            else
            {
                channelListeners[channel] += callback; // 支持多个监听器
            }
            
            Debug.Log($"Added listener to channel {channel}");
        }
        
        public static void RemoveListener(string channel, Action<MessageInfo> callback)
        {
            try
            {
                if (channelListeners.ContainsKey(channel))
                {
                    channelListeners[channel] -= callback;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error removing channel listener: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
            }
        }
        
        // Tests
        #if UNITY_EDITOR
        [MenuItem("XenoSDK/Test Socket Channel")]
        public static void TestChannel()
        {
            // create a ServerTest GameObject and run the Test coroutine
            new GameObject("ServerTest").AddComponent<ServerAPITest>().StartCoroutine("TestChannelAsDeviceA");
            new GameObject("ServerTest").AddComponent<ServerAPITest>().StartCoroutine("TestChannelAsDeviceB");
            new GameObject("ServerTest").AddComponent<ServerAPITest>().StartCoroutine("TestChannelAsDeviceC");
        }
        #endif
    }
    
    public struct MessageInfo
    {
        public string Sender;  // 消息发送者
        public string Channel; // 消息通道
        public string Message; // 消息内容

        public override string ToString()
        {
            return $"Sender: {Sender}, Channel: {Channel}, Message: {Message}";
        }
    }

    
    public partial class ServerAPITest : MonoBehaviour
    {
        // these code simulates communications between three devices
        IEnumerator TestChannelAsDeviceA()
        {
            ServerAPI.Connect();
            yield return new WaitForSeconds(1); // must wait until connected
            ServerAPI.AddListener("toA", (MessageInfo info) =>
            {
                Debug.Log($"Device A received from {info.Sender}: {info.Message}");
            });
            ServerAPI.AddListener("broadcast", (MessageInfo info) =>
            {
                Debug.Log($"Device A received broadcast from {info.Sender}: {info.Message}");
            });
            
            yield return new WaitForSeconds(0.5f);
        }
        
        IEnumerator TestChannelAsDeviceB()
        {
            ServerAPI.Connect();
            yield return new WaitForSeconds(1); // must wait until connected
            ServerAPI.AddListener("broadcast", (MessageInfo info) =>
            {
                Debug.Log($"Device B received broadcast from {info.Sender}: {info.Message}");
            });
            
            yield return new WaitForSeconds(1);
            ServerAPI.Send("toA", "Hello from B");
            
            yield return new WaitForSeconds(1);
            ServerAPI.Send("broadcast", "Hello everybody, my position is (0, 1, 0)");
        }
        
        IEnumerator TestChannelAsDeviceC()
        {
            ServerAPI.Connect();
            yield return new WaitForSeconds(1); // must wait until connected
            ServerAPI.AddListener("broadcast", (MessageInfo info) =>
            {
                Debug.Log($"Device B received broadcast from {info.Sender}: {info.Message}");
            });
            
            yield return new WaitForSeconds(1);
            
        }
    }
}


            