using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Deepglint.XR.Ros;
using Newtonsoft.Json;
using UnityEngine;

namespace Deepglint.XR
{
    public class WsPoseAdapter
    {
        private Ros2PoseAdapter _poseAdapter;
        private ClientWebSocket _ws;
        private CancellationTokenSource _cancellationTokenSource;

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
            [DataMember(Name = "data")] public string data;
        }

        public async void Start()
        {
            _poseAdapter = new Ros2PoseAdapter();
            Debug.Log($"ws://{Global.Config.Space.EngineHost}:{Global.Config.Space.WsPort}");
            Uri serverUri = new Uri($"ws://{Global.Config.Space.EngineHost}:{Global.Config.Space.WsPort}");
            _ws = new ClientWebSocket();
            try
            {
                await _ws.ConnectAsync(serverUri, CancellationToken.None);

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
            await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }

        async Task ReceiveLoop()
        {
            List<byte> buffer = new List<byte>();
            while (_ws.State == WebSocketState.Open)
            {
                byte[] receiveBuffer = new byte[20480]; // 每次接收的缓冲区大小

                WebSocketReceiveResult result =
                    await _ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    buffer.AddRange(receiveBuffer.Take(result.Count)); // 将接收到的数据添加到缓冲区
                    if (result.EndOfMessage)
                    {
                        string message = Encoding.UTF8.GetString(buffer.ToArray());
                        MetaWsPoseData info = JsonConvert.DeserializeObject<MetaWsPoseData>(message);
                        _poseAdapter.DealMsgData(info.Msg.data);
                        buffer.Clear();
                    }
                }
            }
        }

 
        public void OnDestroy()
        {
            if (_ws is { State: WebSocketState.Open })
            {
                _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed by client",
                    CancellationToken.None).Wait();
            }
        }
    }
}