using UnityEngine;
using UnityEngine.UI;

namespace Moat
{
    public class DebugButton : MonoBehaviour
    {
        public DebugPanel DebugPanelInstance;
        private DebugConfirmPanel _confirmPanelInstance;
        public bool IsReady = false;
        
        public string UserId = "";
        
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
                    transform.SetParent(DebugPanelInstance.LeftGroup.transform);
                    SortLeftGroup();
                    if (IsReady) DevicePlayerManager.Instance.DebugRemove(UserId);
                    IsReady = !IsReady;
                    gameObject.GetComponent<Image>().color = Color.white;
                };
                _confirmPanelInstance.OnControl = () =>
                {
                    DebugPanelInstance.BeControlledUserID = UserId;
                    ResetRightGroupColor();
                    gameObject.GetComponent<Image>().color = Color.cyan;
                };
            }
            else
            {
                transform.SetParent(DebugPanelInstance.RightGroup.transform);
                DevicePlayerManager.Instance.DebugAdd(UserId);
                IsReady = !IsReady;
            }
        }

        private void SortLeftGroup()
        {
            // int childCount = DebugPanelInstance.LeftGroup.transform.childCount;
            // for (int i = 0; i < childCount; i++)
            // {
            // }    
        }

        private void ResetRightGroupColor()
        {
            int childCount = DebugPanelInstance.RightGroup.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = DebugPanelInstance.RightGroup.transform.GetChild(i);
                child.GetComponent<Image>().color = Color.white;
            }
        }

        public void ResetButton()
        {
            transform.SetParent(DebugPanelInstance.LeftGroup.transform);
            SortLeftGroup();
            IsReady = false;
            gameObject.GetComponent<Image>().color = Color.white;
            _confirmPanelInstance.gameObject.SetActive(false);
        }
    }
}

