using System.Collections.Generic;
using Deepglint.XR;
using Deepglint.XR.Inputs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Samples.HumanControlInputModule
{
    public class Foot : MonoBehaviour
    {
        [SerializeField] 
        private GameObject leftFootPrefab;
        [SerializeField] 
        private GameObject rightFootPrefab;
        private Dictionary<int, GameObject> _leftFoots;
        private Dictionary<int, GameObject> _rightFoots;
        
        private void OnDeviceLost(int deviceId)
        {
            if (_leftFoots.ContainsKey(deviceId))
            {
                Destroy(_leftFoots[deviceId]);
                _leftFoots.Remove(deviceId);
            }

            if (_rightFoots.ContainsKey(deviceId))
            {
                Destroy(_rightFoots[deviceId]);
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
            return new Vector2(position.x * 1920 / Global.Config.Space.Screens[4].Size.x,
                position.z * 1920 / Global.Config.Space.Screens[4].Size.y);
        }

        public void Awake()
        {
            _leftFoots = new Dictionary<int, GameObject>();
            _rightFoots = new Dictionary<int, GameObject>();
        }

        public void Update()
        {
            foreach (var device in DeviceManager.AllActiveXRHumanDevices)
            {
                GameObject leftFoot;
                if (!_leftFoots.ContainsKey(device.deviceId))
                {
                    // init foot;
                    leftFoot = Object.Instantiate(leftFootPrefab);
                    leftFoot.SetActive(true);
                    leftFoot.transform.SetParent(gameObject.GetComponent<RectTransform>(), false);
                    _leftFoots[device.deviceId] = leftFoot;
                }
                else
                {
                    leftFoot = _leftFoots[device.deviceId];
                }

                var leftRectTransform = leftFoot.GetComponent<RectTransform>(); 
                leftRectTransform.anchoredPosition = GetBottomUIPosition(device.HumanBody.LeftFoot.position.value);
                
                GameObject rightFoot;
                if (!_rightFoots.ContainsKey(device.deviceId))
                {
                    // init foot;
                    rightFoot = Object.Instantiate(rightFootPrefab);
                    rightFoot.SetActive(true);
                    rightFoot.transform.SetParent(gameObject.GetComponent<RectTransform>(), false);
                    _rightFoots[device.deviceId] = rightFoot;
                }
                else
                {
                    rightFoot = _rightFoots[device.deviceId];
                }
                var rightRectTransform = rightFoot.GetComponent<RectTransform>();
                rightRectTransform.anchoredPosition = GetBottomUIPosition(device.HumanBody.RightFoot.position.value);
                
                Vector3 eulerRotation = device.HumanPose.Rotation.value.eulerAngles;
                Vector3 localEulerAngles = new Vector3(0, 0, -eulerRotation.y);
                leftRectTransform.localEulerAngles = localEulerAngles;
                rightRectTransform.localEulerAngles = localEulerAngles;
            }
        }
    }
}