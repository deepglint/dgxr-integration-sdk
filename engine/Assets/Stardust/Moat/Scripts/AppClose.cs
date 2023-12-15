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
                MDebug.LogFlow("====== 关闭当前应用 =====");
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
                _startTime = 0;
                return;
            }

            if (_startTime == 0)
            {
                _startTime = Time.time;
                Invoke("CheckCloseCount2", (float)(interval + 2));
            }
            CheckCloseCount1(interval); 
        }

        private void CheckCloseCount1(int interval = 5)
        {
            if (_startTime != 0 && (Time.time - _startTime) > 1)
            {
                _closeCount += (Time.time - _startTime);
                _startTime = Time.time;
            }

            Debug.Log("_closeCount >= interval: " + _closeCount + " " + interval);
            if (_closeCount >= interval)
            {
                Open();
                _closeCount = 0;
                _startTime = 0;
            }
        }

        private void CheckCloseCount2()
        {
            CancelInvoke("CheckCloseCount2"); 
            _closeCount = 0;
            _startTime = 0;
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            _currentTime = _timeMax;
            Invoke("Close", _timeMax);
            MDebug.LogFlow("------ 打开[关闭弹窗] ------" + _currentTime);
            CloseGameObject?.SetActive(true);
            InvokeRepeating("UpdateTime", 0f, 1f);
        }

        private void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            CancelInvoke("Close");
            CancelInvoke("UpdateTime");
            MDebug.Log("close AppClose dialog" + _currentTime);
            CloseGameObject?.SetActive(false);
            CancelInvoke("UpdateTime");
            OnClose?.Invoke();
        }
    }
}