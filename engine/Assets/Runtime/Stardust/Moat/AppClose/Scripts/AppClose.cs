using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CGC 
{
    public class AppClose : MonoBehaviour
    {
        public static AppClose Instance;

        public GameObject closeGameObject;

        public Text closeGameTime;

        // Start is called before the first frame update
        private int timeMax = 5;
        private int currentTime;

        public Action OnCloseCallback;
        public bool isOpen = false;

        private void Awake()
        {
            currentTime = timeMax;
            Instance = this;
        }

        void Start()
        {
            closeGameObject?.SetActive(false);
        }

        public void CloseApp()
        {
            if (isOpen)
            {
                Debug.Log("====== 关闭当前应用 =====");
                GameAppManager.Instance.CloseApp();
            }
        }

        void UpdateTime()
        {
            if (closeGameTime != null)
            {
                closeGameTime.text = currentTime.ToString();
                currentTime--;
            }
        }

        public void Open()
        {
            if (isOpen) return;
            isOpen = true;
            currentTime = timeMax;
            Invoke("Close", timeMax);
            Debug.Log("------ 打开[关闭弹窗] ------" + currentTime);
            closeGameObject?.SetActive(true);
            InvokeRepeating("UpdateTime", 0f, 1f);
        }

        public void Close()
        {
            if (!isOpen) return;
            isOpen = false;
            CancelInvoke("Close");
            CancelInvoke("UpdateTime");
            Debug.Log("------ 关闭[关闭弹窗] ------" + currentTime);
            closeGameObject?.SetActive(false);
            CancelInvoke("UpdateTime");
            OnCloseCallback?.Invoke();
        }
    }
}