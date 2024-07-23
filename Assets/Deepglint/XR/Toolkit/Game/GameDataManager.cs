using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

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
        public float[] Score;
        public int AvatarId;
        public GameMode GameMode;
        public Color QRImageColor;
    }

    public enum GameMode
    {
        Single,
        Multi
    }

    public class RankInfoReq
    {
        public string GameId;
        public GameMode GameMode;
        public int Count;
        public delegate void DataEventHandler(RankInfo data);
        public event DataEventHandler OnRankDataReceived;

        public RankInfoReq (string gameId,GameMode gameMode,int count)
        {
            GameId = gameId;
            GameMode = gameMode;
            Count = count;
           GameDataManager.Instance.Subscribe(this); 
        }
        
        public void OnDataReceived(RankInfo data)
        {
            OnRankDataReceived?.Invoke(data);
        }

        public void Close()
        {
            GameDataManager.Instance.Unsubscribe(this);
        }
    }

    public class GameDataManager: MonoBehaviour
    {
        private Dictionary<RankInfoReq,Coroutine> _coroutine = new ();
        private static GameDataManager _instance;
        private Dictionary<RankInfoReq, string> _rankHash = new Dictionary<RankInfoReq, string>();
        private static readonly object _lock = new object();

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

        public void Subscribe(RankInfoReq req)
        {
            string url =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/rank?id={req.GameId}&mode={(int)req.GameMode}&count={req.Count}";
            Debug.Log(url);

            var coroutine = StartCoroutine(FetchDataRoutine(url,req));
            _coroutine[req] = coroutine; 
        }
        
        public void Unsubscribe(RankInfoReq req)
        {
            if (_coroutine.TryGetValue(req, out var coroutine))
            {
                StopCoroutine(coroutine);
                _coroutine.Remove(req);
            }
        }

        public static Texture GenerateShareImage(ShareInfo info)
        {
            long unixTimestamp = ((DateTimeOffset)info.Time).ToUnixTimeSeconds();
            var score = string.Join(",",info.Score);
            var content =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/auth?i={DGXR.ApplicationSettings.id}&s={info.SpaceId}&t={unixTimestamp}&sc={score}&a={info.AvatarId}&m={(int)info.GameMode}";
            return GenerateQRCode.GenerateQRImage(content, 256, 256, info.QRImageColor);
        }

        private GameDataManager()
        {
        }

        public static GameDataManager Instance => _instance;

        private IEnumerator FetchDataRoutine(string url, RankInfoReq req)
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

                    Uri uri = new Uri(url);
                    NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                    string id = queryParams["id"];
                    string mode = queryParams["mode"];
                    if (id != null && mode != null)
                    {
                        var rankId = $"{id}-{mode}";
                        var rankHash = MD5.Hash(receiveContent);
                        if (_rankHash.TryGetValue(req, out var data))
                        {
                            if (data == rankHash)
                            {
                                yield return new WaitForSeconds(3f);
                                continue;
                            }
                        }
                        _rankHash[req] = rankHash;
                        req.OnDataReceived(rank);
                    }
                }

                yield return new WaitForSeconds(3f);
            }
        }
    }
}