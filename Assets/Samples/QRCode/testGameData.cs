using System;
using Scene.Common;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Game
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
            Debug.Log($"id:{info.Id}");
            if (info.Data == null)
            {
                return;
            }

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