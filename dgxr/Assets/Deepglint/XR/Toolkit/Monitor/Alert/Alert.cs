using System;
using Deepglint.XR.Ros;
using Deepglint.XR.Toolkit.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Monitor.Alert
{
    public class Alert : MonoBehaviour
    {
        [FormerlySerializedAs("Ros2UnityManager")]
        public ROS2UnityManager ros2UnityManager;

        private GameObject _frontCanvas;
        private GameObject _alertPrefab;

        private GameObject _playerOutNumber;
        private TextMeshProUGUI _playerOutNumberText;
        private GameObject _serviceInterrupt;
        private GameObject _loseFocus;

        private void Start()
        {
            _frontCanvas = GameObject.Find("UIRoot/Front");
            _alertPrefab = Instantiate(Resources.Load<GameObject>("Alert"), _frontCanvas.transform, false);
            _alertPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _playerOutNumber = _alertPrefab.FindChildGameObject("PlayerOutNumber");
            _playerOutNumberText =
                _alertPrefab.FindChildGameObject("PlayerOutNumberText").GetComponent<TextMeshProUGUI>();
            _serviceInterrupt = _alertPrefab.FindChildGameObject("ServiceInterrupt");
            _loseFocus = _alertPrefab.FindChildGameObject("LoseFocus");

            _loseFocus.SetActive(false);
            _playerOutNumber.SetActive(false);
            _serviceInterrupt.SetActive(false);
        }

        private void Update()
        {
            // TODO: 对接文件中的最大人数
            if (false)
            {
                var count = 2;
                _playerOutNumberText.text = $"人数超出{count}人上限";
                _playerOutNumber.SetActive(true);
            }
            else if (false)
            {
                _serviceInterrupt.SetActive(false);
            }


            if (ros2UnityManager.gameObject.activeSelf)
            {
                if (ros2UnityManager.Ok())
                {
                    _playerOutNumber.SetActive(false);
                }
                else
                {
                    _serviceInterrupt.SetActive(true);
                }
            }

#if !UNITY_EDITOR
            if (Application.isFocused)
            {
                _loseFocus.SetActive(false);
            }
            else
            {
                _loseFocus.SetActive(true);
            }
#endif
        }
    }
}