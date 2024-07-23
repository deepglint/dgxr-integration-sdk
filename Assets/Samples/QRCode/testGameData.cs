using System;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Game
{
    public class TestGameData : MonoBehaviour
    {
        public RawImage img;
        private RankInfoReq line;
        private void Start()
        {
            line = new RankInfoReq("5f3c73f3", GameMode.Single, 20);
            line.OnRankDataReceived += Rank; 
            ShareInfo info = new ShareInfo()
            {
                AvatarId = 1,
                GameMode = GameMode.Single,
                Score = new float[]{3,100},
                Time = DateTime.Now,
                SpaceId = "1111111",
                // QRImageColor = Color.black
            };

            img.texture = GameDataManager.GenerateShareImage(info);
        }

        private void OnApplicationQuit()
        {
            line.OnRankDataReceived -= Rank; 
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