using System;
using UnityEngine;
using UnityEngine.UI;

namespace Moat
{
    public class DebugButton : MonoBehaviour
    {
        public static DebugButton Instance;
        public DebugPanel debugPanel;
        public DebugConfirmPanel _confirmPanelInstance;
        public bool IsReady = false;
        
        public string UserId = "";

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ChangeButtonState);
        }

        public void Initialize(string userId)
        {
            UserId = userId;
            GetComponentInChildren<Text>().text = "id:" + userId;
        }

        public void ChangeButtonState()
        {
            if (!IsReady && PlayerGroup.Instance.IsMaxPlayerCount())
            {
                return;
            }

            if (IsReady)
            {
                _confirmPanelInstance.transform.localPosition = transform.localPosition;
                _confirmPanelInstance.gameObject.SetActive(true);
                _confirmPanelInstance.OnLeave = () =>
                {
                    LeaveCall(UserId);
                };
                _confirmPanelInstance.OnControl = () =>
                {
                    debugPanel.BeControlledUserID = UserId;
                    ResetRightGroupColor();
                    transform.gameObject.GetComponent<Image>().color = Color.cyan;
                };
            }
            else
            {
                transform.SetParent(debugPanel.RightGroup.transform);
                DevicePlayerManager.Instance.DebugAdd(UserId);
                IsReady = !IsReady;
            }
        }

        public void LeaveCall(string id)
        {
            transform.SetParent(debugPanel.LeftGroup.transform);
            SortLeftGroup();
            if (IsReady) DevicePlayerManager.Instance.DebugRemove(id);
            IsReady = !IsReady;
            gameObject.GetComponent<Image>().color = Color.white;
        }
        
        private void SortLeftGroup()
        {
            // int childCount = Instance.LeftGroup.transform.childCount;
            // for (int i = 0; i < childCount; i++)
            // {
            // }    
        }

        private void ResetRightGroupColor()
        {
            int childCount = debugPanel.RightGroup.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = debugPanel.RightGroup.transform.GetChild(i);
                child.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetButton()
        {
            transform.SetParent(debugPanel.LeftGroup.transform);
            SortLeftGroup();
            IsReady = false;
            gameObject.GetComponent<Image>().color = Color.white;
            _confirmPanelInstance.gameObject.SetActive(false);
        }
    }
}

