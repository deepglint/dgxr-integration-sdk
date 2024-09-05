using Deepglint.XR.EventSystem;
using Deepglint.XR.EventSystem.EventData;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Log;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Samples.CustomPlayerManager
{
    public class PlayerMotor : MonoBehaviour, IHighFiveEventHandler
    {
        public float moveSpeed = 10f;
        public float rotateSpeed = 10f;
        public float jumpSpeed = 400f;
        private bool _isOnGround = true;
        private Vector3 _moveDistance;
        private Rigidbody _rb;
        private int _freeSwimCount = 0;
        private int _butterflySwimCount = 0;
        private int _highKneeRunSwimCount = 0;
        private int _deepSquatCount = 0;
        private int _slideRightArmToLeftCount = 0;
        private int _slideLeftArmToRightCount = 0;
        
        private static Logger _sampleLogger = new Logger(new PrefixedLogger(Debug.unityLogger.logHandler, "Player-Samples"));

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>(); 
        }

        private void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + _moveDistance);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                _isOnGround = true;
            }
        }
    
        public void MoveControl(InputAction.CallbackContext value)
        {
            Vector2 data = value.ReadValue<Vector2>();
            // Debug.LogFormat("stick: x {0}, y {1}", data.x, data.y);
            Vector3 moveDir = new Vector3(data.x * 2, 0, data.y * 2).normalized;
            _moveDistance = moveDir * moveSpeed * Time.deltaTime;
 
            Vector3 targetDir = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(targetDir); 
        }
    
        public void PoseControl(InputAction.CallbackContext value)
        {
            HumanPoseState humanPose = value.ReadValue<HumanPoseState>();
            transform.position = new Vector3(humanPose.position.x, transform.position.y, humanPose.position.z);
            transform.rotation = humanPose.rotation;
        }
    
        public void JumpControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _sampleLogger.Log("Jump action performed");
                jump();
            }
        }
        
        public void CheerUpControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _sampleLogger.Log("CheerUp action performed");
            }
        }
    
        public void SlideRightArmToLeftControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _slideRightArmToLeftCount++;
                _sampleLogger.Log($"SlideRightArmToLeftCount count: {_slideRightArmToLeftCount}");
            }
        }
    
        public void SlideLeftArmToRightControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _slideLeftArmToRightCount++;
                _sampleLogger.Log($"SlideLeftArmToRightCount count: {_slideLeftArmToRightCount}");
            }
        }
    
        public void FreeSwimControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _freeSwimCount++;
                _sampleLogger.Log($"free-swim count: {_freeSwimCount}");
            }
        }
    
        public void ButterflySwimControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _butterflySwimCount++;
                _sampleLogger.Log($"butterfly-swim count: {_butterflySwimCount}");
            }
        }
    
        public void HighKneeRunControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _highKneeRunSwimCount++;
                _sampleLogger.Log("high-knee-run count: {0}", _highKneeRunSwimCount);
            }
        }
    
        public void DeepSquatControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _deepSquatCount++;
                _sampleLogger.Log("deep-squat count: {0}", _deepSquatCount);
            }
        }

        public void RaiseBothHandControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _sampleLogger.Log("raise both hand");
            }
        }
        
        public void RaiseSingleHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _sampleLogger.Log("Raise-Single-Hand action performed");
            }
        }

        public void RaiseRightHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _sampleLogger.Log("Raise-Right-Hand action performed");
            }
        }
        
        public void RaiseLeftHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _sampleLogger.Log("Raise-Left-Hand action performed");
            }
        }
    
        void OnJump(InputValue value)
        {
            bool data = value.isPressed;
            if(data)
            {
                _sampleLogger.Log("jump, " + _isOnGround);
                if (_isOnGround)
                {
                    //瞬移效果
                    //transform.Translate(Vector3.up * Time.deltaTime * jumpSpeed);
 
                    // 实现跳跃效果
                    jump();
                }
            }
        }

        private void jump()
        {
            if (_isOnGround)
            {
                // 实现跳跃效果
                if (_rb == null)
                {
                    _sampleLogger.Log("rb is null");
                }
                _rb.AddForce(Vector3.up * jumpSpeed);
                // 此时物体不在地面上
                _isOnGround = false;
            }
        }

        public void OnHighFiveEvent(HumanInteractionEventData eventData)
        {
            Player player = eventData.Player.GetComponent<Player>();
            if (player != null)
            {
                _sampleLogger.Log($"high-five action with {player.Character.Name}");
            }
        }
    }
}