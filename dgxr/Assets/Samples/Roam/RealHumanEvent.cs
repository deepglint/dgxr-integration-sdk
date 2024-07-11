using System;
using System.Threading.Tasks;
using Deepglint.XR;
using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using Deepglint.XR.Toolkit.RoamStick;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Samples.Roam
{
    public class RealHumanEvent : MonoBehaviour 
    {
        private bool _isJoined;
        private AppCharacter _appCharacter;
        
        public bool isRealHuman;

        private Player _player;
        
        // jumpController
        // private JumpController _jumpController;
        private RoamController _roamController;
        private Vector2 _movePosition = Vector2.zero;
        
        public void OnJoin()
        {
            if (_isJoined) return;
            _isJoined = true;
            _roamController.SetActive(true);
            
            if (isRealHuman)
            {
                _appCharacter ??= (AppCharacter)GetComponent<Player>().Character;
            }

            transform.gameObject.name = $"RealHuman{_appCharacter?.Name}";
            if (_appCharacter == null) return;
            // _jumpController.rb = transform.GetComponent<Rigidbody>();
        }
        
        private void Awake()
        {
            // _jumpController = gameObject.AddComponent<JumpController>();
            _roamController = GameObject.Find("Roam").GetComponent<RoamController>();
        }

        private async void Start()
        {
            _player = GetComponent<Player>();
            isRealHuman = _player != null;
            _roamController.hasPlayer = true; 
            if (isRealHuman) return;
            GetComponent<PlayerInput>().enabled = false;
            
            _appCharacter = CharacterManager.MainCharacter;
            await Task.Delay(100);
            OnJoin();

            GameObject.Find("Body")?.SetActive(DGXR.Config.Debug);
            SetEventSystem();
        }

        private void SetEventSystem()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            HumanControlFootPointerInputModule humanControlFootPointerInputModule =
                eventSystem.GetComponent<HumanControlFootPointerInputModule>();
            if (_appCharacter.IsRealHuman)
            {
                inputModule.enabled = false;
                humanControlFootPointerInputModule.enabled = true;
            }
            else
            {
                inputModule.enabled = true;
                humanControlFootPointerInputModule.enabled = false;
            }
        }

        public void OnDeviceLost()
        {
            _isJoined = false;
            // ⚠️️设备离线了，但是 humanPlayer 的节点还在
            _roamController.SetActive(false);
        }

        private void PutOffCheckLost()
        {
            if (!isRealHuman) return;
            if (_player.PairedDevices.Count > 0)
            {
                OnJoin(); 
            }
            else
            {
                OnDeviceLost(); 
            }
        }

        public void Update()
        {
            PutOffCheckLost();

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                foreach (var device in _appCharacter.Player.PairedDevices)
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        _appCharacter.Player.UnPairDeviceManually(device); 
                    }
                    else
                    {
                        Debug.LogFormat("device {0} is not dgxr device", device.deviceId);
                    }
                }  
                Destroy(transform.gameObject);
                OnDeviceLost();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
            }
        }

        public void PoseControl(InputAction.CallbackContext value)
        {
            if (_appCharacter == null) return;
            if (_appCharacter.IsRealHuman)
            {
                HumanPoseState humanPose = value.ReadValue<HumanPoseState>();
                Vector3 rootPosition = humanPose.position;
                if (_appCharacter is { Device: { HumanBody: { LeftFoot: not null } } })
                {
                    try
                    {
                        var leftFootPosition = _appCharacter.Device.HumanBody.LeftFoot.position.value;
                        var rightFootPosition = _appCharacter.Device.HumanBody.RightFoot.position.value;
                        Vector3 headTopPosition = _appCharacter.Device.HumanBody.HeadTop.position.value;
                        rootPosition = (leftFootPosition + rightFootPosition) / 2;
                        rootPosition = new Vector3(rootPosition.x, headTopPosition.y, rootPosition.z);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                
                Vector2 root2DPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(rootPosition);
                _roamController.Move(rootPosition, root2DPosition);
            }
            else
            {
                _movePosition += value.ReadValue<Vector2>() * 0.1f;
                Vector3 rootPosition =  new Vector3(_movePosition.x, 0, _movePosition.y);
                Vector2 root2DPosition = DGXR.Space.Bottom.SpaceToPixelOnScreen(rootPosition);
                _roamController.Move(rootPosition, root2DPosition);
            }
        }

        public void OnRaiseOneHand(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (_appCharacter == null) return;
            Debug.Log("character.Name: " + _appCharacter.Name + " 举起单手 " + value);
        }
       
        public void OnRaiseBothHand(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (_appCharacter == null) return;
            Debug.Log("character.Name: " + _appCharacter.Name + " 举起双手 ");
        }

        public void OnDeepSquat(InputAction.CallbackContext value)
        {
            if(!value.performed && !value.started) return;
            if (_appCharacter == null) return;
            Debug.Log("character.Name: " + _appCharacter.Name + " 深蹲 ");
            _roamController.Charging();
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (_appCharacter == null) return;
            Debug.Log("character.Name: " + _appCharacter.Name + " 跳跃 ");
            _roamController.Jump();
        }

        public void OnFreeSwim(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (_appCharacter == null) return;
            Debug.Log("character.Name: " + _appCharacter.Name + " 自由泳 ");
            _roamController.Reset();
        }
    }
}