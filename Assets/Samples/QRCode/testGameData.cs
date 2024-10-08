using System;
using Deepglint.XR;
using Deepglint.XR.Toolkit.Game;
using UnityEngine;

namespace  Samples.QRCode
{
    public class TestGameData : MonoBehaviour, RankConsumer
    {
        public GameObject QRObj;

        private void Start()
        {
            ShareInfo info = new ShareInfo()
            {
                AvatarId = 1,
                GameMode = GameMode.Single,
                Score = new int[] { 200, 100 },
                Time = DateTime.Now,
                SpaceId = "1111111",
                // QRImageColor = Color.black
            };

            GameObject instance = Instantiate(QRObj);
            var qr = instance.GetComponent<QR>();
            qr.SetQRInfo(info, DGXR.Space.Front, new Vector2(100, 100), new Vector2(500, 500), 1);

            GameDataManager.Instance.Subscribe(this);
        }


        private void OnApplicationQuit()
        {
            GameDataManager.Instance.Unsubscribe(this);
        }

        public RankInfoReq GetRankInfoReq()
        {
            return new RankInfoReq("5f3c73f3", GameMode.Single, 20);
        }

        public void OnDataReceived(RankInfo info)
        {
            DGXR.Logger.Log($"id:{info.Id}");
            if (info.Data == null)
            {
                return;
            }

            foreach (var sc in info.Data)
            {
                DGXR.Logger.Log($"score:{sc.Score},time:{sc.Time},mode:{sc.Mode}");
                foreach (var player in sc.Player)
                {
                    DGXR.Logger.Log($"{player.Id},{player.Url},{player.Name}");
                }
            }
        }
    }
}