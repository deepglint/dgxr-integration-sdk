using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using WebSocket = NativeWebSocket.WebSocket;
using WebSocketState = NativeWebSocket.WebSocketState;

namespace Deepglint.XR.Toolkit.Game
{
    public struct RankInfo
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("data")] public RankData[] Data;
    }

    public struct RankData
    {
        [JsonProperty("time")] public DateTime Time;
        [JsonProperty("mode")] public GameMode Mode;
        [JsonProperty("score")] public float[] Score;
        [JsonProperty("player")] public Player[] Player;
    }

    public struct Player
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("url")] public string Url;
        [JsonProperty("name")] public string Name;
    }

    public struct ShareInfo
    {
        public string SpaceId;
        public DateTime Time;
        public int[] Score;
        public int AvatarId;
        public GameMode GameMode;
        public Color QRImageColor;
    }

    public enum GameMode
    {
        Single,
        Multi
    }
    
    public enum RankOrder
    {
        Desc,
        Asc
    }
    
    public class RankInfoReq
    {
        public string GameId;
        public GameMode GameMode;
        public int Count;
        public RankOrder Order;

        public RankInfoReq(string id, GameMode mode, int count,RankOrder order = RankOrder.Desc)
        {
            GameId = id;
            GameMode = mode;
            Count = count;
            Order = order;
        }
    }

    public interface RankConsumer
    {
        public void OnDataReceived(RankInfo data)
        {
        }

        public RankInfoReq GetRankInfoReq()
        {
            return null;
        }
    }

    public class GameDataManager : MonoBehaviour
    {
        private Dictionary<int, Coroutine> _coroutine = new();
        private static GameDataManager _instance;
        private Dictionary<int, string> _rankHash = new Dictionary<int, string>();
        private static readonly object _lock = new object();
        
        public delegate void QRMsg(string message);
        public static event QRMsg OnQREvent;
        public static void TriggerMyEvent(string message)
        {
            OnQREvent?.Invoke(message);
        }
       
        // ws服务
        private WebSocket _webSocket;
        private bool _success;
        private string _wsUrl;
        private CancellationTokenSource _cancellationTokenSource;

        public struct RecMsg
        {
            [JsonProperty("type")]
            public int Type;
            [JsonProperty("content")]
            public string Content; 
        }
        
        private void Start()
        {
            var host =DGXR.Config.Space.ServerEndpoint.Split("://");
            _wsUrl = $"wss://{host[1]}/meta/ws";
            _cancellationTokenSource = new CancellationTokenSource();
            ConnectAsync(_cancellationTokenSource.Token);
        }

        private void Update()
        {
            if (_webSocket != null)
            {
                _webSocket.DispatchMessageQueue();
            }
        }
        
        private void OnApplicationQuit()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _webSocket.Close();
            }
        }

        private void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            if (_webSocket != null)
            {
                _webSocket.Close();
            }
        }
        
        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            _webSocket = new WebSocket(_wsUrl);

            _webSocket.OnOpen += () =>
            {
                Debug.Log($"Connection {_wsUrl} open!");
                _success = true;
            };
            _webSocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
                _success = false;
                StartReconnect();
            };
            _webSocket.OnClose += (_) =>
            {
                Debug.Log($"Connection {_wsUrl} close!");
                _success = false;
                StartReconnect();
            };
            _webSocket.OnMessage += (bytes) =>
            {
                var message = Encoding.UTF8.GetString(bytes);
                RecMsg info = JsonConvert.DeserializeObject<RecMsg>(message);
                TriggerMyEvent(info.Content);
            };

            try
            {
                await _webSocket.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket connection failed: {ex.Message}");
                StartReconnect();
            }
        }

        private void StartReconnect()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                ReconnectAsync(_cancellationTokenSource.Token);
            }
        }

        private async void ReconnectAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Attempting to reconnect...");
                await Task.Delay(5000, cancellationToken);

                if (_webSocket != null)
                {
                    await _webSocket.Close();
                }

                await ConnectAsync(cancellationToken);
                await Task.Delay(1000, cancellationToken);

                if (_success)
                {
                    break;
                }
            }
        }
        
        private void Awake()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
                else if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Subscribe(RankConsumer rank)
        {
            var req = rank.GetRankInfoReq();
            if (req is null)
            {
                Debug.LogError("Subscribe functions can only be used if the GetRankInfoReq method is implemented");
                return;
            }

            string url =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/rank?id={req.GameId}&mode={(int)req.GameMode}&count={req.Count}&order={(int)req.Order}";
            Debug.Log($"subscribe url: {url}");
            if (_coroutine.TryGetValue(req.GetHashCode(), out var cor))
            {
                StopCoroutine(cor);
            }

            var coroutine = StartCoroutine(FetchDataRoutine(url, rank));
            _coroutine[rank.GetHashCode()] = coroutine;
        }

        public void Unsubscribe(RankConsumer rank)
        {
            if (_coroutine.TryGetValue(rank.GetHashCode(), out var coroutine))
            {
                StopCoroutine(coroutine);
                _coroutine.Remove(rank.GetHashCode());
            }

            _rankHash[rank.GetHashCode()] = "";
        }

        public static Texture GenerateShareImage(ShareInfo info,string id)
        {
            long unixTimestamp = ((DateTimeOffset)info.Time).ToUnixTimeSeconds();
            var score = string.Join(",", info.Score);
            var content =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/auth?i={DGXR.ApplicationSettings.id}&s={info.SpaceId}&t={unixTimestamp}&sc={score}&a={info.AvatarId}&m={(int)info.GameMode}&u={id}";
            return GenerateQRCode.GenerateQRImage(content, 256, 256, info.QRImageColor);
        }

        private GameDataManager()
        {
        }

        public static GameDataManager Instance => _instance;

        private IEnumerator FetchDataRoutine(string url, RankConsumer req)
        {
            while (true)
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    var rank = new RankInfo();
                    string receiveContent = request.downloadHandler.text;
                    try
                    {
                        rank = JsonConvert.DeserializeObject<RankInfo>(receiveContent);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"JSON deserialization error: {ex.Message}");
                        continue;
                    }
                    var rankHash = MD5.Hash(receiveContent);
                    if (_rankHash.TryGetValue(req.GetHashCode(), out var data))
                    {
                        if (data == rankHash)
                        {
                            yield return new WaitForSeconds(3f);
                            continue;
                        }
                    }
                    _rankHash[req.GetHashCode()] = rankHash;
                    req.OnDataReceived(rank);
                }

                yield return new WaitForSeconds(3f);
            }
        }
    }
}