using System;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Scene.Common
{
    public class QR : MonoBehaviour
    {
        public Texture2D ScanQR;
        public RawImage img;
        public Canvas canvas;
        private string _id;
        private int _scanCount;

        public void SetQRInfo(ShareInfo info, ScreenInfo screen, Vector2 position, Vector2 size, int scanCount = 1)
        {
            _scanCount = scanCount;
            string uuid = Guid.NewGuid().ToString();
            _id = uuid.Substring(0, 8);
            canvas.targetDisplay = (int)screen.TargetScreen;
            RectTransform rectTransform = img.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;
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
            GameDataManager.OnQREvent += HandleMyEvent;
        }

        private void OnDisable()
        {
            GameDataManager.OnQREvent -= HandleMyEvent;
        }

        private void HandleMyEvent(string message)
        {
            if (message == _id)
            {
                _scanCount--;
            }
        }
    }
}