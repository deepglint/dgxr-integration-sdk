using System.Collections.Generic;
using UnityEngine;

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
        
        private void OnDeviceLost(int deviceId)
        {
            if (_leftFoots.ContainsKey(deviceId))
            {
                Destroy(_leftFoots[deviceId].gameObject);
                _leftFoots.Remove(deviceId);
            }

            if (_rightFoots.ContainsKey(deviceId))
            {
                Destroy(_rightFoots[deviceId].gameObject);
                _rightFoots.Remove(deviceId);
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

        private Vector2 GetBottomUIPosition(Vector3 position)
        {
            return new Vector2(position.x * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.x, 
                position.z * Global.Space.Bottom.Resolution.width / Global.Space.Bottom.Size.y);
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
                leftFootRectTransform.anchoredPosition = GetBottomUIPosition(device.HumanBody.LeftFoot.position.value);
                
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
                rightFootRectTransform.anchoredPosition = GetBottomUIPosition(device.HumanBody.RightFoot.position.value);
                
                Vector3 eulerRotation = device.HumanPose.Rotation.value.eulerAngles;
                Vector3 localEulerAngles = new Vector3(0, 0, -eulerRotation.y);
                leftFootRectTransform.localEulerAngles = localEulerAngles;
                rightFootRectTransform.localEulerAngles = localEulerAngles;
            }
        }
    }
}