using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BodySource;

namespace CGC 
{
    public class WsError : MonoBehaviour
    {
        public static WsError Instance;

        public GameObject messageObj;

        public bool wsStatus;
        
        private void Awake()
        {
            Instance = this;
            Hide();
        }

        private void Update()
        {
            GameObject source = GameObject.Find("Source");
            if (source == null) return;
            Source sourceConnect = source.GetComponent<Source>();

            if (sourceConnect != null && wsStatus != sourceConnect.HasConnectSuccess)
            {
                wsStatus = sourceConnect.HasConnectSuccess;
                if (sourceConnect.HasConnectSuccess)
                {
                    Hide(); 
                }
                else
                {
                    Show();
                }
            }
        }

        public void Show()
        {
            messageObj.SetActive(true);
        }

        public void Hide()
        {
            messageObj.SetActive(false);
        }
    }
}
