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

    public struct RankInfoReq
    {
        public string GameId;
        public GameMode GameMode;
        public int Count;
    }

    public class GameDataManager
    {
        private List<Coroutine> _coroutine = new List<Coroutine>();
        private static GameDataManager _instance;
        private Dictionary<string, string> _rankHash = new Dictionary<string, string>();
        private MonoBehaviour _coroutineHolder;
        private static readonly object _lock = new object();

        public delegate void RankDataEventHandler(RankInfo data);

        public static event RankDataEventHandler OnRankDataReceived;

        public void SetId(RankInfoReq[] req, MonoBehaviour holder)
        {
            foreach (var cor in _coroutine)
            {
                _coroutineHolder.StopCoroutine(cor);
            }

            _rankHash = new Dictionary<string, string>();
            _coroutineHolder = holder;
            foreach (var val in req)
            {
                string url =
                    $"{DGXR.Config.Space.ServerEndpoint}/meta/rank?id={val.GameId}&mode={(int)val.GameMode}&count={val.Count}";
                Debug.Log(url);
                var coroutine = _coroutineHolder.StartCoroutine(FetchDataRoutine(url, 3f));
                _coroutine.Add(coroutine);
            }
        }

        public static Texture GenerateShareImage(ShareInfo info)
        {
            long unixTimestamp = ((DateTimeOffset)info.Time).ToUnixTimeSeconds();
            var score = "";
            for (int i = 0; i < info.Score.Length; i++)
            {
                score += $"{info.Score[i]}";
                if (i != info.Score.Length - 1)
                {
                    score += ",";
                }
            }

            var content =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/auth?i={DGXR.ApplicationSettings.id}&s={info.SpaceId}&t={unixTimestamp}&sc={score}&a={info.AvatarId}&m={(int)info.GameMode}";
            return GenerateQRCode.GenerateQRImage(content, 256, 256, info.QRImageColor);
        }

        private GameDataManager()
        {
        }

        public static GameDataManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameDataManager();
                    }

                    return _instance;
                }
            }
        }

        private IEnumerator FetchDataRoutine(string url, float interval)
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
                    string receiveContent = request.downloadHandler.text;
                    var rank = JsonConvert.DeserializeObject<RankInfo>(receiveContent);

                    Uri uri = new Uri(url);
                    NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                    string id = queryParams["id"];
                    string mode = queryParams["mode"];
                    if (id != null && mode != null)
                    {
                        var rankId = $"{id}-{mode}";
                        var rankHash = MD5.Hash(receiveContent);
                        if (_rankHash.TryGetValue(rankId, out var data))
                        {
                            if (OnRankDataReceived == null || data == rankHash)
                            {
                                yield return new WaitForSeconds(interval);
                            }
                        }
                        _rankHash[rankId] = rankHash;
                        OnRankDataReceived(rank);
                    }
                }

                yield return new WaitForSeconds(interval);
            }
        }
    }
}