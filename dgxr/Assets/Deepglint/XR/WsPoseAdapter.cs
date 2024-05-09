using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Runtime.Scripts.Ros;

namespace Runtime.Scripts
{
    public class WsPoseAdapter
    {
        private Ros2PoseAdapter _poseAdapter;
        private ClientWebSocket ws;
        private CancellationTokenSource cancellationTokenSource;

        [DataContract]
        public class MetaWsPoseData
        {
            [DataMember(Name = "op")] public string Op { get; set; }
            [DataMember(Name = "topic")] public string Topic { get; set; }
            [DataMember(Name = "msg")] public MessageData Msg { get; set; }
        }

        [Serializable]
        public class MessageData
        {
            [DataMember(Name = "data")] public string Data;
        }

        public async void Start()
        {
            _poseAdapter = new Ros2PoseAdapter();
            Uri serverUri = new Uri("ws://192.168.103.61:9090");
            ws = new ClientWebSocket();
            try
            {
                await ws.ConnectAsync(serverUri, CancellationToken.None);

                await SubscribeToTopic("/metapose/pose3d", "std_msgs/String");

                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket connection error: {ex.Message}");
            }
        }

        async Task SubscribeToTopic(string topic, string messageType)
        {
            // 发送订阅命令
            string subscribeMsg = "{\"op\":\"subscribe\",\"topic\":\"" + topic + "\",\"type\":\"" + messageType + "\"}";
            await SendMessageAsync(subscribeMsg);
        }

        async Task SendMessageAsync(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        async Task ReceiveLoop()
        {
            List<byte> buffer = new List<byte>();
            while (ws.State == WebSocketState.Open)
            {
                byte[] receiveBuffer = new byte[1024]; // 每次接收的缓冲区大小
                WebSocketReceiveResult result =
                    await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    buffer.AddRange(receiveBuffer.Take(result.Count)); // 将接收到的数据添加到缓冲区
                    if (result.EndOfMessage)
                    {
                        string message = Encoding.UTF8.GetString(buffer.ToArray());
                        MetaWsPoseData info = JsonConvert.DeserializeObject<MetaWsPoseData>(message);
                        _poseAdapter.DealMsgData(info.Msg.Data);
                        buffer.Clear();
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (ws != null && ws.State == WebSocketState.Open)
            {
                ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed by client",
                    CancellationToken.None).Wait();
            }
        }
    }
}
