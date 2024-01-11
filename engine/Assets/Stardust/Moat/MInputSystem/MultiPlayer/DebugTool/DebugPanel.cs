using System;
using System.Collections.Generic;
using UnityEngine;
using Moat;

namespace Moat
{
    public class DebugPanel : MonoBehaviour
    {
        public static DebugPanel Instance;
        public GameObject debugButton;
        public GameObject LeftGroup;
        public GameObject RightGroup;

        private string _beControlledUserID;

        private List<DebugButton> _buttonList = new List<DebugButton>();
        public string  BeControlledUserID
        {
            get { return _beControlledUserID;}
            set
            {
                _beControlledUserID = value; 
                DevicePlayerManager.Instance.BeControlledUserID = _beControlledUserID;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = GameObject.Instantiate(debugButton, LeftGroup.transform);
                obj.name = i + obj.name;
                obj.SetActive(true);
                obj.GetComponent<DebugButton>().debugPanel = this;
                obj.GetComponent<DebugButton>().Initialize(i + 1 + "");
                _buttonList.Add(obj.GetComponent<DebugButton>());
            }
        }

        private void OnEnable()
        {
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
        }

        private void OnRaiseOnHand(EventCallBack evt)
        {
            MDebug.LogFlow("3. 玩家进入 - 3.0.4 举右手添加按钮");
            string id = (string)evt.Params[0];
            DebugButton debugButton = LeftGroup.transform.GetChild(0).GetComponent<DebugButton>();
            if(debugButton != null && !debugButton.IsReady)debugButton.ChangeButtonState();
        }

        private DebugButton GetDebugButtonById(string id)
        {
            foreach (var button in _buttonList)
            {
                if (button.UserId == id)
                {
                    return button;
                }
            }

            return null;
        }

        public void ResetPanel()
        {
            foreach (var button in _buttonList)
            {
                button.ResetButton();
            }
        }
    }
}
