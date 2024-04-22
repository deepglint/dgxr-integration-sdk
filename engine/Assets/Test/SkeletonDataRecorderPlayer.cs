using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BestHTTP.WebSocket;
using Newtonsoft.Json;
using Stardust.MultiPlayer.Scripts;
using Stardust.MultiPlayer.Scripts.MatchPlayer;
using Stardust.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Timer = System.Threading.Timer;

namespace Test
{
    public static class Utils
    {
        // TODO 对接新的stardust的，值设置为 0
        public static readonly int DiffId = 0;

        public static void Log(string message, int level)
        {
            if (level == 1)
            {
                // UnityEngine.Debug.LogWarning(message);
            }
            else
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static T ReadJsonFile<T>(string filePath)
        {
            string readData = File.ReadAllText(filePath);
            // 支持数组嵌套的情况
            return JsonConvert.DeserializeObject<T>(readData);
        }
    }

    [Serializable]
    public class CaseConfigData
    {
        public bool Debug { get; set; }
        public string TestRecordUrl { get; set; }
        public string TestRecord { get; set; }
    }

    public static class CaseConfig
    {
        public static CaseConfigData Data = new CaseConfigData()
        {
            Debug = true,
            TestRecordUrl = "",
            TestRecord = "0"
        };

        public static void ReadConfig()
        {
            Data = Utils.ReadJsonFile<CaseConfigData>(Application.streamingAssetsPath + "/json/config.json");
        }
    }

    [Serializable]
    public class SendMessageData
    {
        public string id;
        public string role;
        public string type;
        public string data;
    }

    [Serializable]
    public struct CaseData
    {
        public string type;
        public long ts;
        public Dictionary<string, float[,]> pose;
        public Dictionary<string, string> action;
        public string other;
    }

    [Serializable]
    public struct ConfigData
    {
        public bool isRecord;
        public bool isPlayback;
        public string appName;
        public string appVersion;
        public List<Config.Action> actions;
    }

    public class WebsocketOptions
    {
        private CaseData _data;

        private void ParsePoseData(CaseData info)
        {
            _data.ts = info.ts;
            _data.pose = info.pose;
            if (info.pose.Count != XrdgBodySource.Instance.Data.Count)
            {
                Utils.Log("1. WS 连接 - 1.3 骨骼人数：" + info.pose.Count + _data, 0);
            }

            foreach (var person in XrdgBodySource.Instance.Data)
            {
                if (!info.pose.ContainsKey(person.Key))
                {
                    bool removed = XrdgBodySource.Instance.Data.TryRemove(person.Key, out BodyDataSource removedValue);
                    Utils.Log($"{removed}-{removedValue}", 0);
                }
            }

            foreach (var person in info.pose)
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
                    // 新增
                    BodyDataSource body = new BodyDataSource()
                    {
                        IsTracked = true,
                        BodyID = person.Key,
                        Joints = new Dictionary<JointType, JointData>(),
                        LeftRay = new Ray(),
                        RightRay = new Ray(),
                        LeftHit = new RaycastHit(),
                        RightHit = new RaycastHit(),
                    };
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

            Utils.Log(
                "BodySource.Instance.Data: " + XrdgBodySource.Instance.Data.Count + " " +
                XrdgBodySource.Instance.GetData(), 0);
            DevicePlayerManager.Instance.PersonBodySource = XrdgBodySource.Instance.GetData();
        }

        private static void ParsePoseAction(CaseData info)
        {
            var actions = info.action
                .Where(personAction => !string.IsNullOrEmpty(personAction.Value))
                .Select(personAction => new
                {
                    PlayerId = int.Parse(personAction.Key) + Utils.DiffId,
                    ActionValue = personAction.Value
                })
                .ToList();

            foreach (var action in actions)
            {
                Utils.Log("玩家" + action.PlayerId.ToString() + " " + action.ActionValue, 1);
                EventManager.Send(action.ActionValue, new object[] { action.PlayerId.ToString() });
            }
        }

        public void OnMessage(CaseData info)
        {
            ParsePoseData(info);
            ParsePoseAction(info);
        }

        public void OnOpened()
        {
            Utils.Log("数据源接入～～～", 0);
        }

        public void OnError()
        {
            Utils.Log("数据源接入失败～～～", 0);
        }
    }

    public class SkeletonDataRecorderPlayer : MonoBehaviour
    {
        public static SkeletonDataRecorderPlayer Instance;
        [HideInInspector] public GameObject caseTestObj;
        [FormerlySerializedAs("WsUri")] public string wsUri = "";
        public bool isRecord;
        public bool isPlayback = true;
        public string wsId;

        private bool _autoReconnect = true;

        [FormerlySerializedAs("HasConnectSuccess")]
        public bool hasConnectSuccess;

        private WebSocket _webSocket;
        private int _reconnectCount;
        private readonly int _reconnectMaxCount = -1;
        private long _lastConnect;
        private WebsocketOptions _websocketOptions;
        private Type _optionType;
        private Timer _timer;
        private readonly VirtualPlayer[] _mockPlayers = new VirtualPlayer[10];
        private readonly MatchRule[] _mockMatchRule = new MatchRule[10];
        // private readonly ApertureCircle[] _apertureCircles = new ApertureCircle[10];
        private GameObject _actionConfigObject;
        private Config _actionConfig;
        private Source _sourceConnect;
        private readonly Dictionary<string, string> _action = new Dictionary<string, string>();
        private Coroutine _delayCoroutine;

        private class TimerState
        {
            public int Counter;
        }

        private void Awake()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                // Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
                caseTestObj = GameObject.Find("SkeletonDataRecorderPlayer");
                if (caseTestObj != null)
                {
                    DontDestroyOnLoad(caseTestObj);
                }
            }
        }

        private void Start()
        {
            if (!isRecord && !isPlayback)
            {
                caseTestObj = GameObject.Find("SkeletonDataRecorderPlayer");
                if (caseTestObj != null)
                {
                    caseTestObj.SetActive(false);
                }

                transform.gameObject.SetActive(false);
                return;
            }

            GameObject debugPanel = GameObject.Find("DebugPanel");
            if (debugPanel != null)
            {
                debugPanel.SetActive(false);
            }

            if (wsUri == "")
            {
                wsUri = "ws://127.0.0.1:8005";
            }

            hasConnectSuccess = false;
            _autoReconnect = true;
            _reconnectCount = 0;

            _actionConfigObject = GameObject.Find("ActionConfig");
            if (_actionConfigObject)
            {
                _actionConfig = _actionConfigObject.GetComponent<Config>();
            }

            GameObject source = GameObject.Find("Source");
            if (source)
            {
                _sourceConnect = source.GetComponent<Source>();
            }

            Init(new WebsocketOptions());
            if (isPlayback)
            {
                CreatePlayer();
            }

            if (!isRecord) return;
            EventManager.RegisterListener(ActionEvent.OnRightHandDrawCircle, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnLeftHandDrawCircle, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandBevelCut, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandParry, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandStraightCut, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandTransversal, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnStraightPunch, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnReadyStraightPunch, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnUppercut, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnKick, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnThrowOneHandInFists, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnReadyThrowOneHandInFists, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnReadyThrowBothHandInFists, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnReadyHandObliqueCut, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnWavingOneHand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnReadyWavingOneHand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnCombineHandsStraight, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnThrowBoulder, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSlowRun, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnFastRun, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnButterfly, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnFreestyle, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnKeepRaisingHand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnApplaud, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnJump, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnDeepSquat, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnRaiseBothHand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmFlat, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmFlatIsL, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmVerticalIsL, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSlideLeft, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSlideRight, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSlideUp, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSlideDown, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandsAway, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandsClose, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnWaving, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmToForward, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmToBack, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmToLeft, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnArmToRight, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnBendBothElbows, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnHandsCross, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnPoseA, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnPoseB, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnPoseC, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnPoseD, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnLeanToLeft, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnLeanToRight, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnStand, ActionLog);
            EventManager.RegisterListener(ActionEvent.OnSmallSquat, ActionLog);
        }

        private void ActionLog(EventCallBack evt)
        {
            if (!isRecord) return;
            string originalString = evt.ToString();
            string[] parts = originalString.Split(' ');
            string modifiedString = parts[0];

            if (evt.Params[0] != null && int.TryParse(evt.Params[0].ToString(), out int value))
            {
                string id = (value - Utils.DiffId).ToString();
                if (id == "") return;
                _action[id] = modifiedString;
                SetDelayedAction(0.1f, ResetAction);
            }
        }

        private void SetDelayedAction(float delay, Action action)
        {
            // 清除之前的延迟
            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
                _delayCoroutine = null;
            }

            // 开始新的延迟
            _delayCoroutine = StartCoroutine(DelayedExecution(delay, action));
        }

        private IEnumerator DelayedExecution(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        private void ResetAction()
        {
            foreach (string id in _action.Keys)
            {
                _action[id] = "";
            }
        }

        // private void CreatePlayerCircle(int i, VirtualPlayer virtualPlayer)
        // {
        //     ApertureCircle apertureCircle = ComponentManager.Instance.OpenAndCreate<ApertureCircle>(
        //         new ApertureCircle(ApertureCircle.Path),
        //         TargetDisplay.Bottom);
        //     apertureCircle.BindPlayerId(virtualPlayer.ID);
        //     apertureCircle.component.name = "poseCircle" + virtualPlayer.ID;
        //     _apertureCircles[i] = apertureCircle;
        // }

        private void CreateMatchRule(int i, VirtualPlayer virtualPlayer)
        {
            try
            {
                if (DevicePlayerManager.Instance != null)
                {
                    Type type = Type.GetType(DevicePlayerManager.Instance.matchType);
                    if (type != null)
                    {
                        try
                        {
                            if (Activator.CreateInstance(type, new object[] { virtualPlayer }) is MatchRule matchRule)
                            {
                                Utils.Log("type: " + type.FullName, 0);
                                _mockMatchRule[i] = matchRule;
                            }
                            else
                            {
                                Utils.Log("无法创建 IMatchRule 实例", 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            // 处理构造函数抛出的异常
                            Debug.LogError("在创建 IMatchRule 实例时发生异常: " + ex.Message);
                        }
                    }
                    else if (DevicePlayerManager.Instance != null && DevicePlayerManager.Instance.matchType != null)
                    {
                        Debug.LogError("无法加载类型 " + DevicePlayerManager.Instance.matchType);
                    }
                }
                else
                {
                    Debug.LogError("DevicePlayerManager 实例为空");
                }
            }
            catch (Exception ex)
            {
                // 处理其他可能发生的异常
                Debug.LogError("发生异常: " + ex.Message);
            }
        }

        private void CreatePlayer()
        {
            for (int i = 0; i < 10; i++)
            {
                string userId = $"{i + Utils.DiffId}";
                VirtualPlayer virtualPlayer = new VirtualPlayer(userId)
                {
                    State = PlayerInteractionState.NoEnter,
                    MovementInput = new Vector2(1.5f, 1.5f),
                    LeftFootInput = new Vector2(1.5f, 1.5f),
                    RightFootInput = new Vector2(1.5f, 1.5f)
                };
                _mockPlayers[i] = virtualPlayer;
                // CreatePlayerCircle(i, virtualPlayer);
                if (DevicePlayerManager.Instance != null && DevicePlayerManager.Instance.matchType != "")
                {
                    CreateMatchRule(i, virtualPlayer);
                }
                else
                {
                    // OnRaiseRightMatch matchRule = new OnRaiseRightMatch(virtualPlayer);
                    // _mockMatchRule[i] = matchRule;
                }
            }
        }

        private void SendConfig()
        {
            if (_actionConfig.actions.Count <= 2) return;
            ConfigData config = new ConfigData()
            {
                isRecord = isRecord,
                isPlayback = isPlayback,
                appName = Application.productName,
                appVersion = Application.version,
                actions = _actionConfig.actions,
            };
            string jsonString = JsonConvert.SerializeObject(config);
            Send("config", jsonString);
        }

        private void Update()
        {
            if (isRecord)
            {
                Send("pose", new CaseData()
                {
                    type = "pose-message",
                    action = _action,
                    other = _sourceConnect.optionMessage,
                });
            }

            foreach (MatchRule matchRule in _mockMatchRule)
            {
                matchRule?.Update();
            }

            // for (int i = 0; i < _apertureCircles.Length; i++)
            // {
            //     if (UnityObjectUtility.IsDestroyed(_apertureCircles[i].component))
            //     {
            //         CreatePlayerCircle(i, _mockPlayers[i]);
            //     }
            //
            //     _apertureCircles[i]?.Move(_mockPlayers[i].MovementInput);
            // }
        }

        private void Init(WebsocketOptions arg)
        {
            _websocketOptions = arg;
            _optionType = _websocketOptions.GetType();
            Utils.Log("1. WS 连接 - 1.1 地址：: " + wsUri, 0);
            Connect(wsUri);
            var timerState = new TimerState { Counter = 0 };
            _timer = new Timer(
                callback: SmartReconnect,
                state: timerState,
                dueTime: 1000,
                period: 1000
            );
        }

        private long GetNowTime()
        {
            TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            long time = (long)mTimeSpan.TotalSeconds;
            return time;
        }

        private void Connect(string url)
        {
            _lastConnect = GetNowTime();

            _webSocket = new WebSocket(new Uri(url));
            _webSocket.OnOpen += OnWebSocketOpen;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.OnClosed += OnWebSocketClosed;
            _webSocket.OnError += OnError;
            _webSocket.Open();
        }

        private void SmartReconnect(object timerState)
        {
            if (timerState is TimerState timerStateInstance)
            {
                Interlocked.Increment(ref timerStateInstance.Counter);
            }

            int maxWait = 15000;

            if (!_autoReconnect || hasConnectSuccess) return;
            if (_webSocket.State.ToString() != "Connecting")
            {
                maxWait = 3000;
            }

            if ((GetNowTime() - _lastConnect) * 1000 <= maxWait) return;
            if (_reconnectCount < _reconnectMaxCount || _reconnectMaxCount == -1)
            {
                _reconnectCount += 1;
                if (_webSocket.State.ToString() == "Open" || _webSocket.State.ToString() == "Connecting")
                {
                    // AutoReconnect = false;
                    _webSocket.Close();
                }

                Utils.Log("重连次数:" + _reconnectCount, 0);
                Connect(wsUri);
            }
            else
            {
                _timer.Dispose();
                _autoReconnect = false;
                Utils.Log("re-reconnect: " + _reconnectCount + "次，重连次数过多，不再继续重连，请联系后端服务人员处理", 1);
            }
        }

        private void OnWebSocketOpen(WebSocket webSocket)
        {
            Utils.Log("1. WS 连接 - 1.2.1 连接成功 server：web socket open!", 0);
            _lastConnect = GetNowTime();
            if (hasConnectSuccess) return;
            hasConnectSuccess = true;
            if (_optionType.GetMethod("OnOpened") != null)
            {
                _websocketOptions.OnOpened();
            }

            long timestampMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            wsId = timestampMillis.ToString();
            _reconnectCount = 0;
            Send("connect", new CaseData());
        }

        private void Send(string type, CaseData message)
        {
            Send(type, JsonConvert.SerializeObject(message));
        }

        private void Send(string type, string message)
        {
            SendMessageData data = new SendMessageData
            {
                id = wsId,
                role = isRecord ? "record" : "client",
                type = type,
                data = message
            };
            if (!(type == "connect" || type == "config" || isRecord)) return;
            string jsonString = JsonConvert.SerializeObject(data);
            _webSocket.Send(jsonString);
            if (type == "connect")
            {
                SendConfig();
            }
        }

        private void OnMessageReceived(WebSocket webSocket, string message)
        {
            if (_optionType.GetMethod("OnMessage") == null || message == null) return;
            CaseData info = JsonConvert.DeserializeObject<CaseData>(message);
            if (info.type == "syncConfig")
            {
                SendConfig();
            }
            else if (isPlayback)
            {
                _websocketOptions.OnMessage(info);
            }
        }

        private void OnError(WebSocket ws, string error)
        {
            Utils.Log("1. WS 连接 - 1.2.2 连接失败 " + error, 1);
            hasConnectSuccess = false;
            if (_optionType.GetMethod("OnError") != null)
            {
                _websocketOptions.OnError();
            }
        }

        private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
        {
            Utils.Log("server warning: web socket closed!", 1);
        }

        void OnDestroy()
        {
            if (_webSocket == null) return;
            _webSocket.Close();
            _timer.Dispose();
            _autoReconnect = false;
            // options.activeTimer.Dispose();

            EventManager.RemoveListener(ActionEvent.OnRightHandDrawCircle, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnLeftHandDrawCircle, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandBevelCut, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandParry, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandStraightCut, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandTransversal, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnStraightPunch, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnReadyStraightPunch, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnUppercut, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnKick, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnThrowOneHandInFists, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnReadyThrowOneHandInFists, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnReadyThrowBothHandInFists, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnReadyHandObliqueCut, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnWavingOneHand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnReadyWavingOneHand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnCombineHandsStraight, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnThrowBoulder, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSlowRun, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnFastRun, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnButterfly, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnFreestyle, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnKeepRaisingHand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnApplaud, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnJump, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnDeepSquat, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnRaiseOnHand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnRaiseBothHand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmFlat, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmFlatIsL, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmVerticalIsL, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSlideLeft, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSlideRight, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSlideUp, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSlideDown, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandsAway, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandsClose, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnWaving, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmToForward, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmToBack, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmToLeft, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnArmToRight, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnBendBothElbows, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnHandsCross, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnPoseA, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnPoseB, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnPoseC, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnPoseD, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnLeanToLeft, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnLeanToRight, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnStand, ActionLog);
            EventManager.RemoveListener(ActionEvent.OnSmallSquat, ActionLog);
        }
    }
}
