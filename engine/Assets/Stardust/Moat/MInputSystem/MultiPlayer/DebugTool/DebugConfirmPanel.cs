using System;
using UnityEngine;
using UnityEngine.UI;

namespace Moat
{
    public class DebugConfirmPanel : MonoBehaviour
    {
        public Action OnLeave;
        public Action OnControl;

        public Button LeaveButton;
        public Button ControlButton;

        private void Start()
        {
            LeaveButton.onClick.AddListener(() =>
            {
                OnLeave?.Invoke();
                gameObject.SetActive(false);
            });
            ControlButton.onClick.AddListener(() =>
            {
                OnControl?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}