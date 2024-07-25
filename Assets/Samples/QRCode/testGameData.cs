using System;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Game
{
    public class TestGameData :MonoBehaviour,RankConsumer
    {
        public RawImage img;
        private void Start()
        {
            ShareInfo info = new ShareInfo()
            {
                AvatarId = 1,
                GameMode = GameMode.Single,
                Score = new int[]{200,100},
                Time = DateTime.Now,
                SpaceId = "1111111",
                // QRImageColor = Color.black
            };

            img.texture = GameDataManager.GenerateShareImage(info);
            GameDataManager.Instance.Subscribe(this);
        }


        private void OnApplicationQuit()
        {
            GameDataManager.Instance.Unsubscribe(this);
        }
        
        public RankInfoReq GetRankInfoReq()
        {
            // return null;
            return new RankInfoReq("5f3c73f3", GameMode.Single, 20); 
        }
        
        public void OnDataReceived(RankInfo info)
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