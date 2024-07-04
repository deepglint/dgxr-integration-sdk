using System.Threading.Tasks;
using Deepglint.XR;
using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Samples.Roam
{
    public class RealHumanEvent : MonoBehaviour 
    {
        private bool _isJoined;
        public AppCharacter AppCharacter;
        
        public bool isRealHuman;

        private Player _player;
        private Vector2 _lastMovePos;
        
        // jumpController
        private JumpController _jumpController;
        // moveController
        private MoveController _moveController;
        
        public void OnJoin()
        {
            if (_isJoined) return;
            _isJoined = true;
            _moveController.roamStick.SetActive(true);
            
            if (isRealHuman)
            {
                AppCharacter ??= (AppCharacter)GetComponent<Player>().Character;
            }

            transform.gameObject.name = $"RealHuman{AppCharacter?.Name}";
            if (AppCharacter == null) return;
            _jumpController.rb = transform.GetComponent<Rigidbody>();
        }
        
        private void Awake()
        {
            _jumpController = gameObject.AddComponent<JumpController>();
            _moveController = gameObject.AddComponent<MoveController>();
        }

        private async void Start()
        {
            _player = GetComponent<Player>();
            isRealHuman = _player != null;
            SetEventSystem();
            
            if (isRealHuman) return;
            GetComponent<PlayerInput>().enabled = false;
            
            AppCharacter = CharacterManager.MainCharacter;
            await Task.Delay(100);
            OnJoin();

            GameObject.Find("Body")?.SetActive(Global.Config.Debug);
        }

        private void SetEventSystem()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            HumanControlFootPointerInputModule humanControlFootPointerInputModule =
                eventSystem.GetComponent<HumanControlFootPointerInputModule>();
            if (isRealHuman)
            {
                inputModule.enabled = false;
            }
            else
            {
                humanControlFootPointerInputModule.enabled = false;
            }
        }

        public void OnDeviceLost()
        {
            // TODO SDK 如果是瞬时丢失，处理不要丢失
            _isJoined = false;
            // ⚠️️设备离线了，但是 humanPlayer 的节点还在
            _moveController.roamStick.SetActive(false);
            _moveController.ResetMove(false);
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
        }

        public void MockMovementPosition(Vector2 movePosition)
        {
            Vector2 human2DPosition = _lastMovePos + movePosition;
            float ratio = Global.Space.Size.x / 1920f;
            Vector3 human3DPosition = new Vector3(human2DPosition.x * ratio, 1.6f, human2DPosition.y * ratio);
            HumanPoseState humanPose = new HumanPoseState()
            {
                position = human3DPosition,
            };
           
            HumanLocalPoseState humanLocalPose = new HumanLocalPoseState()
            {
                Move2DPos = human2DPosition,
                Move3DPos = human3DPosition,
            };

            UpdateMove(humanPose, humanLocalPose);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
            }
        }

        // TODO 这个地方速度上有问题，需要调试
        // public void OnStick(InputValue value)
        // {
        //     Vector2 roamPosition = value.Get<Vector2>();
        //     Debug.LogFormat("OnStick: {0}", roamPosition);
        //     Roam.Instance.UpdateRoamMoveFromStick(roamPosition * 3);
        // }

        public void PoseControl(InputAction.CallbackContext value)
        {
            HumanPoseState humanPose = value.ReadValue<HumanPoseState>();

            Vector3 rootPosition = humanPose.position;
            // Vector3 headTopPosition = humanPose.position;
            // if (AppCharacter is { Device: { HumanBody: { LeftFoot: not null } }, CharacterInfo: { IsIdle: false } })
            // {
            //     try
            //     {
            //         var leftFootPosition = AppCharacter.Device.HumanBody.LeftFoot.position.value;
            //         var rightFootPosition = AppCharacter.Device.HumanBody.RightFoot.position.value;
            //         headTopPosition = AppCharacter.Device.HumanBody.HeadTop.position.value;
            //         // rootPosition = (leftFootPosition + rightFootPosition) / 2;
            //         // rootPosition = new Vector3(rootPosition.x, headTopPosition.y, rootPosition.z);
            //         rootPosition = headTopPosition;
            //     }
            //     catch (Exception e)
            //     {
            //         Debug.LogWarning(e);
            //     }
            // }
           
            Vector2 root2DPosition = Global.Space.Bottom.SpaceToPixelOnScreen(rootPosition);
            HumanLocalPoseState humanLocalPose = new HumanLocalPoseState()
            {
                Move2DPos = root2DPosition,
                Move3DPos = rootPosition,
            };

            UpdateMove(humanPose, humanLocalPose);
        }

        private void UpdateMove(HumanPoseState humanPose, HumanLocalPoseState humanLocalPose)
        {
            humanLocalPose.Angle = CalculateRotationAngle(humanLocalPose.Move2DPos);
            _moveController.UpdateMove(humanPose, humanLocalPose);
            _lastMovePos = humanLocalPose.Move2DPos;
        }

        private static float CalculateRotationAngle(Vector2 pointA)
        {
            float angleInRadians = Mathf.Atan2(pointA.y, pointA.x);
            float angleInDegrees = Mathf.Rad2Deg * angleInRadians;
        
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360;
            }

            return angleInDegrees - 90;
        }

        public void RaiseOneHand()
        {
        }

        public void RaiseBothHand()
        {
            _moveController.ResetMove(true); 
        }

        public void Jump()
        {
            _jumpController.Jump(); 
        }

        public void OnRaiseOneHand(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (AppCharacter == null) return;
            Debug.Log("character.Name: " + AppCharacter.Name + " 举起单手 " + value);
            RaiseOneHand();
        }
       
        public void OnRaiseBothHand(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (AppCharacter == null) return;
            Debug.Log("character.Name: " + AppCharacter.Name + " 举起双手 ");
        }

        public void OnDeepSquat(InputAction.CallbackContext value)
        {
            if (AppCharacter == null) return;
            Debug.Log("character.Name: " + AppCharacter.Name + " 深蹲 ");
            _jumpController.Charging();
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (AppCharacter == null) return;
            Debug.Log("character.Name: " + AppCharacter.Name + " 跳跃 ");
            Jump();
        }

        public void OnFreeSwim(InputAction.CallbackContext value)
        {
            if(!value.performed) return;
            if (AppCharacter == null) return;
            Debug.Log("character.Name: " + AppCharacter.Name + " 自由泳 ");
            _moveController.ResetMove(true);
        }
    }
}