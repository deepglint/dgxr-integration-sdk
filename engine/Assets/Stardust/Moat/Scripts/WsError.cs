using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BodySource;

namespace Moat 
{
    public class WsError : MonoBehaviour
    {
        public static WsError Instance;

        public GameObject MessageObj;

        [HideInInspector]public bool WsStatus;
        
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

            if (sourceConnect != null && WsStatus != sourceConnect.HasConnectSuccess)
            {
                WsStatus = sourceConnect.HasConnectSuccess;
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
            MessageObj.SetActive(true);
        }

        public void Hide()
        {
            MessageObj.SetActive(false);
        }
    }
}
