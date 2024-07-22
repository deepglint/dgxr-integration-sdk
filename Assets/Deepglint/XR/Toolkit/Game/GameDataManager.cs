using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.PlayerLoop;

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
        public string GameId; //sdk自己获取
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
        public float Interval; //获取间隔时间
    }

    public class GameDataManager
    {
        private List<Coroutine> _coroutine = new List<Coroutine>();
        private static GameDataManager _instance;
        private Dictionary<string, RankInfo[]> _rankData = new Dictionary<string, RankInfo[]>();
        private MonoBehaviour _coroutineHolder;
        private static readonly object _lock = new object();

        public void SetId(RankInfoReq[] req, MonoBehaviour holder)
        {
            foreach (var cor in _coroutine)
            {
                _coroutineHolder.StopCoroutine(cor);
            }

            _coroutineHolder = holder;
            foreach (var val in req)
            {
                string url =
                    $"{DGXR.Config.Space.ServerEndpoint}/meta/rank?id={val.GameId}&mode={(int)val.GameMode}&count={val.Count}";
                Debug.Log(url);
                var coroutine = _coroutineHolder.StartCoroutine(FetchDataRoutine(url, val.Interval));
                _coroutine.Add(coroutine);
            }
        }

        public bool TryGetRank(string id,GameMode mode, out RankInfo[] rank)
        {
            var rankId = $"{id}-{(int)mode}";
            return _rankData.TryGetValue(rankId, out rank);
        }

        public static Texture GenerateShareImage(ShareInfo info)
        {
            long unixTimestamp = ((DateTimeOffset)info.Time).ToUnixTimeSeconds();
            var score = "";
            for (int i = 0; i < info.Score.Length; i++)
            {
                score += i.ToString();
                if (i != info.Score.Length - 1)
                {
                    score += ",";
                }
            }

            var content =
                $"{DGXR.Config.Space.ServerEndpoint}/meta/auth?i={info.GameId}&s={info.SpaceId}&t={unixTimestamp}&sc={score}&a={info.AvatarId}&m={(int)info.GameMode}";
            Debug.LogError(content);
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
                    var rank = JsonConvert.DeserializeObject<RankInfo[]>(receiveContent);

                    Uri uri = new Uri(url);
                    NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                    string id = queryParams["id"];
                    string mode = queryParams["mode"];
                    if (id != null&& mode!=null)
                    {
                        var rankId = $"{id}-{mode}";
                        _rankData[rankId] = rank;
                    }
                }

                yield return new WaitForSeconds(interval);
            }
        }
    }
}