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
            ShareInfo info = new ShareInfo()
            {
                GameId = "eeadfa",
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
                new RankInfoReq { GameId = "eeadfa",GameMode = GameMode.Single,Count = 20,Interval = 5f},
            };
            GameDataManager.Instance.SetId(req,this);
        }

        private void FixedUpdate()
        {
            if (GameDataManager.Instance.TryGetRank("eeadfa",GameMode.Single, out var data))
            {
                foreach (var ran in data)
                {
                    Debug.Log(ran.Data.Length);
                    // Debug.Log($"id:{ran.Id}");
                    // foreach (var sc in ran.Data)
                    // {
                    //     Debug.Log($"score:{sc.Score},time:{sc.Time},mode:{sc.Mode}");
                    // }
                }
            }
          
        }
    }
}