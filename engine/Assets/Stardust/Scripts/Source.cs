using System;
using System.Collections.Generic;
using System.Threading;
using BestHTTP.WebSocket;
using Newtonsoft.Json;
using Stardust.Model;
using UnityEngine;
using UnityEngine.Serialization;

// yq: ws://192.168.12.1:8000/ws
// sl: ws://192.168.8.7:8000/ws
// local: ws://127.0.0.1:8000/ws

namespace Stardust.Scripts
{
    [Serializable]
    public struct SourceData
    {
        public long Ts { get; set; }
        public Dictionary<string, float[,]> Pose { get; set; }
    }

    public class Options
    {
        public Timer ActiveTimer;

        public class TimerObject
        {
            public int Counter;
        }

        public long GetNowTime()
        {
            TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            long time = (long)mTimeSpan.TotalMilliseconds;
            return time;
        }

        public void OnMessage(string res)
        {
            MDebug.LogTest("返回的message " + res);
            if (res != null)
            {
                SourceData info = JsonConvert.DeserializeObject<SourceData>(res);
                if (info.Pose.Count != XrdgBodySource.Instance.Data.Count)
                {
                    MDebug.LogFlow("1. WS 连接 - 1.3 骨骼人数：" + info.Pose.Count);
                }

                foreach (var person in XrdgBodySource.Instance.Data)
                {
                    if (!info.Pose.ContainsKey(person.Key))
                    {
                        XrdgBodySource.Instance.Data.TryRemove(person.Key, out _);
                    }
                }

                foreach (var person in info.Pose)
                {
                    if (XrdgBodySource.Instance.Data.ContainsKey(person.Key))
                    {
                        //存在则更新
                        BodyDataSource body = XrdgBodySource.Instance.Data[person.Key];
                        int rows = person.Value.GetLength(0); // 获取行数
                        for (int i = 0; i < rows; i++)
                        {
                            JointData joint = new JointData(person.Value[i, 0], person.Value[i, 1], person.Value[i, 2]);
                            JointType jointType = (JointType)i;
                            body.Joints[jointType] = joint; 
                        }
                        XrdgBodySource.Instance.Data[person.Key] = body;
                    }
                    else
                    {
                        //新增
                        BodyDataSource body = new BodyDataSource();
                        body.IsTracked = true;
                        body.BodyID = person.Key;
                        body.Joints = new Dictionary<JointType, JointData>();
                        body.LeftRay = new Ray();
                        body.RightRay = new Ray();
                        body.LeftHit = new RaycastHit();
                        body.RightHit = new RaycastHit();
                        int rows = person.Value.GetLength(0); // 获取行数
                        for (int i = 0; i < rows; i++)
                        {
                            JointData joint = new JointData(person.Value[i, 0], person.Value[i, 1], person.Value[i, 2]);
                            JointType jointType = (JointType)i;
                            body.Joints.Add(jointType, joint);
                        }
                        XrdgBodySource.Instance.Data[person.Key] = body;
                    }
                }
                XREventListener.Instance.OnFrame();
                // 20s活体检测
                if (ActiveTimer != null)
                {
                    ActiveTimer.Dispose();
                    ActiveTimer = null;
                }

                ActiveTimer = new Timer(
                    callback: CountDown,
                    state: new TimerObject { Counter = 0 },
                    dueTime: 0,
                    period: 1000
                );
            }
        }

        public void CountDown(object timerState)
        {
            var state = timerState as TimerObject;
            if (state != null)
            {
                Interlocked.Increment(ref state.Counter);
                // MDebug.LogTest("倒计时" + state.Counter);
                if (state.Counter == 20)
                {
                    ActiveTimer.Dispose();
                    ActiveTimer = null;
                }
            }
        }

        public void OnOpened()
        {
            MDebug.Log("数据源接入～～～");
        }

        public void OnError()
        {
            MDebug.Log("数据源接入失败～～～");
        }
    }

    public class Source : MonoBehaviour
    {
        [FormerlySerializedAs("WsUri")] public string wsUri = "";
        
        private bool _autoReconnect = true;
        [FormerlySerializedAs("HasConnectSuccess")] public bool hasConnectSuccess;
        [HideInInspector] public WebSocket WebSocket;
        private int _reconnectCount;
        private int _reconnectMaxCount = -1;
        private long _lastConnect;
        private Options _options;
        private Type _optionType;
        private Timer _timer;
        public string optionMessage;

        class TimerState
        {
            public int Counter;
        }

        void Start()
        {
            if (wsUri == "")
            {
                wsUri = "ws://127.0.0.1:8000/ws";
            }

            hasConnectSuccess = false;
            _autoReconnect = true;
            _reconnectCount = 0;
            
            DisplayData.ReadConfig();
            MDebug.LogFlow("1. WS 连接 - 1.0 连接权限" + DisplayData.WsConnect + " " + DisplayData.ConfigDisplay.WsConnect);
            if (DisplayData.WsConnect)
            {
                Init(new Options());
            }
        }

        public void Init(Options arg)
        {
            _options = arg;
            _optionType = _options.GetType();
            MDebug.LogFlow("1. WS 连接 - 1.1 地址：: " + wsUri);
            Connect(wsUri);
            // keep alive heartbeat
            var timerState = new TimerState { Counter = 0 };
            _timer = new Timer(
                callback: SmartReconnect,
                state: timerState,
                dueTime: 1000,
                period: 1000
            );
        }

        public long GetNowTime()
        {
            TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            long time = (long)mTimeSpan.TotalSeconds;
            return time;
        }


        public void Connect(string url)
        {
            _lastConnect = GetNowTime();

            WebSocket = new WebSocket(new Uri(url));
            WebSocket.OnOpen += OnWebSocketOpen;
            WebSocket.OnMessage += OnMessageReceived;
            WebSocket.OnClosed += OnWebSocketClosed;
            WebSocket.OnError += OnError;
            WebSocket.Open();
        }

        private void SmartReconnect(object timerState)
        {
            if (!DisplayData.WsConnect)
            {
                _timer.Dispose();
                return;
            }

            var state = timerState as TimerState;
            if (state != null) Interlocked.Increment(ref state.Counter);

            int maxWait = 15000;
            // MDebug.LogTest("当前状态:"+ webSocket.State);


            if (_autoReconnect && !this.hasConnectSuccess)
            {
                if (WebSocket.State.ToString() != "Connecting")
                {
                    maxWait = 3000;
                }

                // MDebug.LogTest("重连等待时间:" + (getNowTime() - LastConnect) * 1000);
                if ((GetNowTime() - _lastConnect) * 1000 > maxWait)
                {
                    if (_reconnectCount < _reconnectMaxCount || _reconnectMaxCount == -1)
                    {
                        _reconnectCount += 1;
                        if (WebSocket.State.ToString() == "Open" || WebSocket.State.ToString() == "Connecting")
                        {
                            // AutoReconnect = false;
                            WebSocket.Close();
                        }

                        MDebug.LogTest("重连次数:" + _reconnectCount);
                        Connect(wsUri);
                    }
                    else
                    {
                        _timer.Dispose();
                        _autoReconnect = false;
                        MDebug.LogWarning("re-reconnect: " + _reconnectCount + "次，重连次数过多，不再继续重连，请联系后端服务人员处理");
                    }
                }
            }
        }

        private void OnWebSocketOpen(WebSocket webSocket)
        {
            MDebug.LogFlow("1. WS 连接 - 1.2.1 连接成功 server：web socket open!");
            _lastConnect = GetNowTime();
            if (!hasConnectSuccess)
            {
                hasConnectSuccess = true;
                EventManager.Send(MoatGameEvent.WsConnectSuccess);
                if (_optionType.GetMethod("OnOpened") != null)
                {
                    _options.OnOpened();
                }

                _reconnectCount = 0;
            }
        }

        private void OnMessageReceived(WebSocket webSocket, string message)
        {
            if (_optionType.GetMethod("OnMessage") != null)
            {
                if (message != null)
                {
                    SourceData info = JsonConvert.DeserializeObject<SourceData>(message);
                    if (info.Pose.Count > 0)
                    {
                        optionMessage = message;
                    }
                    else
                    {
                        optionMessage = null;
                    }
                }

                _options.OnMessage(message);
            }
        }

        private void OnError(WebSocket ws, string error)
        {
            MDebug.LogError("1. WS 连接 - 1.2.2 连接失败 " + error);
            hasConnectSuccess = false;
            EventManager.Send(MoatGameEvent.WsConnectError);
            if (_optionType.GetMethod("OnError") != null)
            {
                _options.OnError();
            }
        }

        private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
        {
            MDebug.LogWarning("server warning: web socket closed!");
        }

        void OnDestroy()
        {
            if (WebSocket == null) return;
            WebSocket.Close();
            _timer.Dispose();
            _autoReconnect = false;
            // options.activeTimer.Dispose();        
        }

        // Update is called once per frame
    }
}