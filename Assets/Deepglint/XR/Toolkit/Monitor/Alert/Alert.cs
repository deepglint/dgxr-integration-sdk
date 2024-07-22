using Deepglint.XR.Ros;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Monitor.Alert
{
    internal class Alert : MonoBehaviour
    {
        
        private ROS2UnityManager _ros2UnityManager;
        private GameObject _frontCanvas;
        private GameObject _alertPrefab;
        private GameObject _playerOutNumber;
        private Text _playerOutNumberText;
        private GameObject _serviceInterrupt;
        private GameObject _loseFocus;

        private void Start()
        {
            _ros2UnityManager = GameObject.Find("XRManager/Connect/RosConnect").GetComponent<ROS2UnityManager>();
            _frontCanvas = GameObject.Find("ToolkitCanvas/Front");
            _alertPrefab = Instantiate(Resources.Load<GameObject>("Alert"), _frontCanvas.transform, false);
            _alertPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _playerOutNumber = _alertPrefab.FindChildGameObject("PlayerOutNumber");
            _playerOutNumberText =
                _alertPrefab.FindChildGameObject("PlayerOutNumberText").GetComponent<Text>();
            _serviceInterrupt = _alertPrefab.FindChildGameObject("ServiceInterrupt");
            _loseFocus = _alertPrefab.FindChildGameObject("LoseFocus");

            _loseFocus.SetActive(false);
            _playerOutNumber.SetActive(false);
            _serviceInterrupt.SetActive(false);
        }

        private void Update()
        {
            if (Source.Source.Data.Count > DGXR.ApplicationSettings.playerSetting.maxPlayerCount)
            {
                _playerOutNumberText.text = $"人数超出{DGXR.ApplicationSettings.playerSetting.maxPlayerCount}人上限";
                _playerOutNumber.SetActive(true);
            }
            else
            {
                _playerOutNumber.SetActive(false);
            }

            
            if (_ros2UnityManager is not null && _ros2UnityManager.gameObject.activeSelf)
            {
                if (_ros2UnityManager.Ok())
                {
                    _serviceInterrupt.SetActive(false);
                }
                else
                {
                    _serviceInterrupt.SetActive(true);
                }
            }

#if !UNITY_EDITOR
            if (DGXR.ApplicationSettings.toolkit.enableLoseFocusTip)
            {
                if (Application.isFocused)
                {
                    _loseFocus.SetActive(false);
                }
                else
                {
                    _loseFocus.SetActive(true);
                }
            }
            else
            {
                _loseFocus.SetActive(false);
            }
#endif
        }
    }
}