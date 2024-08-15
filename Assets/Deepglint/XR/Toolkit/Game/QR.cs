using System;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Game
{
    public class QR : MonoBehaviour
    {
        public Texture2D ScanQR;
        public RawImage img;
        public Canvas canvas;
        private string _id;
        private int _scanCount;

        public void SetQRInfo(ShareInfo info, ScreenInfo screen, Vector2 position = new Vector2(), Vector2 size = new Vector2(), int scanCount = 1)
        {
            _scanCount = scanCount;
            string uuid = Guid.NewGuid().ToString();
            _id = uuid.Substring(0, 8);
            canvas.targetDisplay = (int)screen.TargetScreen;
            RectTransform rectTransform = img.GetComponent<RectTransform>();
            if (position != default)
            {
                rectTransform.anchoredPosition = position;
            }

            if (size != default)
            {
                rectTransform.sizeDelta = size;
            }
            img.texture = GameDataManager.GenerateShareImage(info, _id);
        }

        public void Update()
        {
            if (_scanCount <= 0)
            {
                img.texture = ScanQR;
            }
        }

        private void OnEnable()
        {
            GameDataManager.OnQREvent += HandleQREvent;
        }

        private void OnDisable()
        {
            GameDataManager.OnQREvent -= HandleQREvent;
        }

        private void HandleQREvent(string message)
        {
            if (message == _id)
            {
                _scanCount--;
            }
        }
    }
}