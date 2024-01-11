using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moat 
{
    public class AppClose : MonoBehaviour
    {
        public static AppClose Instance;
        [HideInInspector]public bool IsOpen = false;
        public Action OnClose;
        
        public GameObject CloseGameObject;
        public Text CloseGameTime;
        
        private int _timeMax = 5;
        private int _currentTime;
        
        private float _closeCount = 0;
        private float _startTime = 0;
        private float _preTime = 0;

        private void Awake()
        {
            _currentTime = _timeMax;
            Instance = this;
        }

        private void Start()
        {
            CloseGameObject?.SetActive(false);
        }

        public void CloseApp()
        {
            if (IsOpen)
            {
                // MDebug.LogFlow("====== 关闭当前应用 =====");
                GameAppManager.Instance.CloseApp();
            }
        }

        private void UpdateTime()
        {
            if (CloseGameTime != null)
            {
                CloseGameTime.text = _currentTime.ToString();
                _currentTime--;
            }
        }

        public void OpenThrottle(int interval = 5)
        {
            if (IsOpen)
            {
                _closeCount = 0;
                return;
            }

            if (_closeCount == 0)
            {
                _startTime = Time.time;
                Invoke("OpenAppClose", (float)(interval));
            }

            _closeCount += 1;
        }

        private void OpenAppClose()
        {
            _preTime = Time.time; 
            if (_closeCount > 5 && (_preTime - _startTime) > 4)
            {
                Open();
                _closeCount = 0;
            }
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            _currentTime = _timeMax;
            Invoke("Close", _timeMax);
            // MDebug.LogFlow("------ 打开[关闭弹窗] ------" + _currentTime);
            CloseGameObject?.SetActive(true);
            InvokeRepeating("UpdateTime", 0f, 1f);
        }

        private void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            CancelInvoke("Close");
            CancelInvoke("UpdateTime");
            // MDebug.Log("close AppClose dialog" + _currentTime);
            CloseGameObject?.SetActive(false);
            CancelInvoke("UpdateTime");
            OnClose?.Invoke();
        }
    }
}