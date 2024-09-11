using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Deepglint.XR.Inputs
{
    public class FootprintTracker : MonoBehaviour
    {
        [SerializeField] 
        private GameObject leftFootPrefab;
        [SerializeField] 
        private GameObject rightFootPrefab;

        private RectTransform _rectTransform;
        private Dictionary<int, RectTransform> _leftFoots;
        private Dictionary<int, RectTransform> _rightFoots;
        private Dictionary<int, Text> _leftText;
        private Dictionary<int, Text> _rightText;
        
        private void OnDeviceLost(InputDevice device)
        {
            if (DGXR.Config.Debug)
            {
                if (_leftFoots.ContainsKey(device.deviceId))
                {
                    Destroy(_leftFoots[device.deviceId].gameObject);
                    _leftFoots.Remove(device.deviceId);
                    _leftText.Remove(device.deviceId);
                }

                if (_rightFoots.ContainsKey(device.deviceId))
                {
                    Destroy(_rightFoots[device.deviceId].gameObject);
                    _rightFoots.Remove(device.deviceId);
                    _rightText.Remove(device.deviceId);
                }
            }
        }

        private void OnEnable()
        {
            DeviceManager.OnDeviceLost += OnDeviceLost;
        }

        private void OnDisable()
        {
            DeviceManager.OnDeviceLost -= OnDeviceLost;
        }

        public void Awake()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _leftFoots = new Dictionary<int, RectTransform>();
            _rightFoots = new Dictionary<int, RectTransform>();
            _leftText = new Dictionary<int, Text>();
            _rightText = new Dictionary<int, Text>();
        }

        public void Update()
        {
            if (DGXR.Config.Debug)
            {
                foreach (var device in DeviceManager.AllActiveXRHumanDevices)
                {
                    Text leftText;
                    RectTransform leftFootRectTransform;
                    if (!_leftFoots.ContainsKey(device.deviceId))
                    {
                        // init foot;
                        var leftFoot = Instantiate(leftFootPrefab, _rectTransform, false);
                        leftFoot.SetActive(true);
                        leftFootRectTransform = leftFoot.GetComponent<RectTransform>();
                        _leftFoots[device.deviceId] = leftFootRectTransform;
                        leftText = leftFoot.GetComponentInChildren<Text>();
                        _leftText[device.deviceId] = leftText;
                    }
                    else
                    {
                        leftFootRectTransform = _leftFoots[device.deviceId];
                        leftText = _leftText[device.deviceId];
                    }

                    var leftFootPosition = device.HumanBody.LeftFoot.position.value;
                    leftFootRectTransform.anchoredPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(leftFootPosition);
                    leftText.text = $"{leftFootPosition}";

                    Text rightText;
                    RectTransform rightFootRectTransform;
                    if (!_rightFoots.ContainsKey(device.deviceId))
                    {
                        // init foot;
                        var rightFoot = Instantiate(rightFootPrefab, _rectTransform, false);
                        rightFoot.SetActive(true);
                        rightFootRectTransform = rightFoot.GetComponent<RectTransform>();
                        _rightFoots[device.deviceId] = rightFootRectTransform;
                        rightText = rightFoot.GetComponentInChildren<Text>();
                        _rightText[device.deviceId] = rightText;
                    }
                    else
                    {
                        rightFootRectTransform = _rightFoots[device.deviceId];
                        rightText = _rightText[device.deviceId];
                    }

                    var rightFootPosition = device.HumanBody.RightFoot.position.value;
                    rightFootRectTransform.anchoredPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(rightFootPosition);
                    rightText.text = $"{rightFootPosition}"; 
                    
                    Vector3 eulerRotation = device.HumanPose.Rotation.value.eulerAngles;
                    Vector3 localEulerAngles = new Vector3(0, 0, -eulerRotation.y);
                    leftFootRectTransform.localEulerAngles = localEulerAngles;
                    rightFootRectTransform.localEulerAngles = localEulerAngles;
                }
            }
        }
    }
}