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

namespace Deepglint.XR.Source
{
    public class WsPoseAdapter
    {
        private Ros2PoseAdapter _poseAdapter;
        private ClientWebSocket _ws;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

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
            Debug.Log($"ws://{Global.Config.Space.EngineHost}:{Global.Config.Space.WsPort}");
            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            await ConnectWebSocket();
        }

        private async Task ConnectWebSocket()
        {
            _ws = new ClientWebSocket();
            Uri serverUri = new Uri($"ws://{Global.Config.Space.EngineHost}:{Global.Config.Space.WsPort}");

            try
            {
                await _ws.ConnectAsync(serverUri, _cancellationTokenSource.Token);
                await SubscribeToTopic("/metapose/pose3d", "std_msgs/String");
                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket connection error: {ex.Message}");
                if (_isRunning)
                {
                    await Reconnect();
                }
            }
        }

        private async Task Reconnect()
        {
            if (_ws != null)
            {
                _ws.Dispose();
                _ws = null;
            }

            if (_isRunning)
            {
                Debug.Log("Attempting to reconnect in 5 seconds...");
                await Task.Delay(5000);
                if (_isRunning)
                {
                    await ConnectWebSocket();
                }
            }
        }

        private async Task SubscribeToTopic(string topic, string messageType)
        {
            string subscribeMsg = JsonConvert.SerializeObject(new
            {
                op = "subscribe",
                topic = topic,
                type = messageType
            });

            await SendMessageAsync(subscribeMsg);
        }

        private async Task SendMessageAsync(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            try
            {
                await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
            }
        }

        private async Task ReceiveLoop()
        {
            List<byte> buffer = new List<byte>();
            byte[] receiveBuffer = new byte[20480]; // 每次接收的缓冲区大小

            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await _ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer),
                        _cancellationTokenSource.Token);

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
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException wse) when (wse.WebSocketErrorCode ==
                                                     WebSocketError.ConnectionClosedPrematurely)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error receiving message: {ex}");
                    if (_isRunning)
                    {
                        await Reconnect();
                    }

                    break;
                }
            }

            if (_ws.State != WebSocketState.Open && _isRunning)
            {
                await Reconnect();
            }
        }

        public void OnDestroy()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            try
            {
                if (_ws is { State: WebSocketState.Open })
                {
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed by client",
                        CancellationToken.None).Wait();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error when closing WebSocket: " + ex.Message);
            }
            finally
            {
                _ws?.Dispose();
            }
        }
    }
}