using System;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Game
{
    public class testGameData : MonoBehaviour
    {
        public RawImage img;
        private void Start()
        {
            GameDataManager.OnRankDataReceived += Rank;
            ShareInfo info = new ShareInfo()
            {
                AvatarId = 1,
                GameMode = GameMode.Single,
                Score = new float[]{1,1000},
                Time = DateTime.Now,
                SpaceId = "1111111",
                // QRImageColor = Color.black
                
            };

            img.texture = GameDataManager.GenerateShareImage(info);
            var req = new RankInfoReq[]
            {
                new RankInfoReq { GameId = "5f3c73f3",GameMode = GameMode.Single,Count = 20,Interval = 5f},
            };
            GameDataManager.Instance.SetId(req,this);
        }

        private void OnApplicationQuit()
        {
            GameDataManager.OnRankDataReceived -= Rank; 
        }

        public void Rank(RankInfo info)
        {
            Debug.Log($"id:{info.Id}");
            foreach (var sc in info.Data)
            {
                Debug.Log($"score:{sc.Score},time:{sc.Time},mode:{sc.Mode}");
                foreach (var player in sc.Player)
                {
                    Debug.Log($"{player.Id},{player.Url},{player.Name}");
                }
            }
        }
    }
}