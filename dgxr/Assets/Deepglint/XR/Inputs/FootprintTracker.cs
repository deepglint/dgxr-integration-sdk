using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        private void OnDeviceLost(InputDevice device)
        {
            if (_leftFoots.ContainsKey(device.deviceId))
            {
                Destroy(_leftFoots[device.deviceId].gameObject);
                _leftFoots.Remove(device.deviceId);
            }

            if (_rightFoots.ContainsKey(device.deviceId))
            {
                Destroy(_rightFoots[device.deviceId].gameObject);
                _rightFoots.Remove(device.deviceId);
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
        }

        public void Update()
        {
            foreach (var device in DeviceManager.AllActiveXRHumanDevices)
            {
                RectTransform leftFootRectTransform;
                if (!_leftFoots.ContainsKey(device.deviceId))
                {
                    // init foot;
                    var leftFoot = Instantiate(leftFootPrefab, _rectTransform, false);
                    leftFoot.SetActive(true);
                    leftFootRectTransform = leftFoot.GetComponent<RectTransform>();
                    _leftFoots[device.deviceId] = leftFootRectTransform;
                }
                else
                {
                    leftFootRectTransform = _leftFoots[device.deviceId];
                }

                var leftFootPosition = device.HumanBody.LeftFoot.position.value;
                leftFootRectTransform.anchoredPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(leftFootPosition);
                
                RectTransform rightFootRectTransform;
                if (!_rightFoots.ContainsKey(device.deviceId))
                {
                    // init foot;
                    var rightFoot = Instantiate(rightFootPrefab, _rectTransform, false);
                    rightFoot.SetActive(true);
                    rightFootRectTransform = rightFoot.GetComponent<RectTransform>();
                    _rightFoots[device.deviceId] = rightFootRectTransform;
                }
                else
                {
                    rightFootRectTransform = _rightFoots[device.deviceId];
                }

                var rightFootPosition = device.HumanBody.RightFoot.position.value;
                rightFootRectTransform.anchoredPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(rightFootPosition);
                
                Vector3 eulerRotation = device.HumanPose.Rotation.value.eulerAngles;
                Vector3 localEulerAngles = new Vector3(0, 0, -eulerRotation.y);
                leftFootRectTransform.localEulerAngles = localEulerAngles;
                rightFootRectTransform.localEulerAngles = localEulerAngles;
            }
        }
    }
}