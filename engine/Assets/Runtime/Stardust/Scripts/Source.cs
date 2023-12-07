using System.Collections.Generic;
using UnityEngine;
using BestHTTP.WebSocket;
using System;
using System.Threading;
using Newtonsoft.Json;
using Moat;
using Moat.Model;

// yq: ws://192.168.12.1:8000/ws
// sl: ws://192.168.7.8:8000/ws
namespace BodySource
{
    public class Options
    {
        [System.Serializable]
        public struct SourceData
        {
            public long ts { get; set; }
            public Dictionary<string, float[,]> pose { get; set; }
        }

        public Timer activeTimer = null;
        public class TimerObject
        {
            public int Counter;
        }

        public long getNowTime()
        {
            TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            long time = (long)mTimeSpan.TotalMilliseconds;
            return time;
        }


        public void onMessage(string res)
        {
            MDebug.LogTest("返回的message " + res);
            if (res != null)
            {
                // VRDGBodySource.Instance.Floor = ;
               
                SourceData info = JsonConvert.DeserializeObject<SourceData>(res);
                
                if (info.pose.Count != VRDGBodySource.Instance.Data.Count) {
                    foreach (var person in VRDGBodySource.Instance.Data)
                    {
                        if (!info.pose.ContainsKey(person.Key)) {
                            bool removed = VRDGBodySource.Instance.Data.TryRemove(person.Key, out BodyDataSource removedValue);
                        }
                    }
                 }

                foreach (var person in info.pose) {
                    BodyDataSource body = new BodyDataSource { };
                    body.IsTracked = true;
                    body.BodyID = person.Key;
                    body.Joints = new Dictionary<JointType, JointData> { };
                    int rows = person.Value.GetLength(0); // 获取行数
                    for (int i = 0; i < rows; i++)
                    {
                        JointData joint = new JointData(person.Value[i, 0], person.Value[i, 1], person.Value[i, 2]);
                        JointType jointType = (JointType)i;
                        body.Joints.Add(jointType, joint);
                    }
                    VRDGBodySource.Instance.Data[person.Key] = body;
                }

                //BodyDataSource data;
                

                // 20s活体检测
                if (activeTimer != null)
                {
                    activeTimer.Dispose();
                    activeTimer = null;
                }

                activeTimer = new Timer(
                    callback: new TimerCallback(countDown),
                    state: new TimerObject { Counter = 0 },
                    dueTime: 0,
                    period: 1000
                );
            }
        }

        public void countDown(object timerState)
        {
            var state = timerState as TimerObject;
            Interlocked.Increment(ref state.Counter);
            // MDebug.Log("倒计时" + state.Counter);
            if (state.Counter == 20)
            {
                //GameManager.instance.GameOver();
                activeTimer.Dispose();
                activeTimer = null;
            }
        }

        public void onOpened()
        {
            MDebug.Log("数据源接入～～～");
        }
        public void onError()
        {
             MDebug.Log("数据源接入失败～～～");

        }
    }

    public class Source : MonoBehaviour
    {
        public string WsUri = "";
        [HideInInspector] public bool allowConnect;
        private bool AutoReconnect = true;
        [HideInInspector] public bool HasConnectSuccess;
        [HideInInspector] public WebSocket webSocket;
        private int ReconnectCount;
        private int ReconnectMaxCount = -1;
        private long LastConnect;
        private Options options;
        private Type OptionType;
        private Timer timer;

        class TimerState
        {
            public int Counter;
        }

        void Start()
        {
            if(WsUri == ""){
                WsUri = "ws://127.0.0.1:8000/ws";
            }
            HasConnectSuccess = false;
            AutoReconnect = true;
            ReconnectCount = 0;
            DisplayData.ReadConfig();
            allowConnect = DisplayData.configDisplay.wsConnect;
            ReconnectMaxCount = DisplayData.configDisplay.ReconnectMaxCount;

            if (allowConnect)
            {
                MDebug.Log("allowConnect: 允许连接");
                init(new Options()); 
            }
        }

        public void init(Options arg)
        {
            options = arg;
            OptionType = options.GetType();
            MDebug.Log("WsUri 连接地址: " + WsUri);
            Connect(WsUri);
            // keep alive heartbeat
            var timerState = new TimerState { Counter = 0 };
            timer = new Timer(
                callback: new TimerCallback(smartReconnect),
                state: timerState,
                dueTime: 1000,
                period: 1000
            );
        }

        public long getNowTime()
        {
            TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            long time = (long)mTimeSpan.TotalSeconds;
            return time;
        }

        private bool UrlExistsUsingSockets(string url)
        {
            if (url.StartsWith("https://")) url = url.Remove(0, "https://".Length);
            try
            {
                System.Net.IPHostEntry ipHost = System.Net.Dns.GetHostEntry(url);// System.Net.Dns.Resolve(url);
                return true;
            }
            catch (System.Net.Sockets.SocketException se)
            {
                MDebug.Log("未联网error" + se.Message);
                System.Diagnostics.Trace.Write(se.Message);
                return false;
            }
        }


        public void Connect(string url)
        {
            LastConnect = getNowTime();

            webSocket = new WebSocket(new Uri(url));
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnMessageReceived;
            webSocket.OnClosed += OnWebSocketClosed;
            webSocket.OnError += OnError;
            webSocket.Open();
        }

        private void smartReconnect(object timerState)
        {
            if (!allowConnect)
            {
                timer.Dispose(); 
                return;
            }

            var state = timerState as TimerState;
            Interlocked.Increment(ref state.Counter);

            int maxWait = 15000;
            // MDebug.Log("当前状态:"+ webSocket.State);


            if (AutoReconnect && !this.HasConnectSuccess)
            {
                if (webSocket.State.ToString() != "Connecting")
                {
                    maxWait = 3000;
                }
                // MDebug.Log("重连等待时间:" + (getNowTime() - LastConnect) * 1000);
                if ((getNowTime() - LastConnect) * 1000 > maxWait)
                {
                    if (ReconnectCount < ReconnectMaxCount || ReconnectMaxCount == -1)
                    {
                        ReconnectCount += 1;
                        if (webSocket.State.ToString() == "Open" || webSocket.State.ToString() == "Connecting")
                        {
                            // AutoReconnect = false;
                            webSocket.Close();
                        }
                        MDebug.Log("重连次数:" + ReconnectCount);
                        Connect(WsUri);
                    }
                    else
                    {
                        timer.Dispose();
                        AutoReconnect = false;
                        MDebug.Log("重连" + ReconnectCount + "次，重连次数过多，不再继续重连，请联系后端服务人员处理");
                    }
                }
            }
        }

        private void OnWebSocketOpen(WebSocket webSocket)
        {
            MDebug.Log("WS：连接成功");
            LastConnect = getNowTime();
            if (!HasConnectSuccess)
            {
                HasConnectSuccess = true;
                EventManager.Send(MoatGameEvent.WsConnectSuccess);
                if (OptionType.GetMethod("onOpened") != null)
                {
                    options.onOpened();
                }
                ReconnectCount = 0;
            }
        }
        private void OnMessageReceived(WebSocket webSocket, string message)
        {
            if (OptionType.GetMethod("onMessage") != null)
            {
                options.onMessage(message);
            }
        }

        private void OnError(WebSocket ws, string error)
        {
            MDebug.LogError("失败Error: " + error);
            HasConnectSuccess = false;
            EventManager.Send(MoatGameEvent.WsConnectError);
            if (OptionType.GetMethod("onError") != null)
            {
                options.onError();
            }
        }

        private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
        {
            MDebug.Log("WebSocket 关闭!");
        }

        void OnDestroy()
        {
            if (webSocket == null) return; 
            webSocket.Close();
            timer.Dispose();
            AutoReconnect = false;
            // options.activeTimer.Dispose();        
        }


        // Update is called once per frame
    }
}