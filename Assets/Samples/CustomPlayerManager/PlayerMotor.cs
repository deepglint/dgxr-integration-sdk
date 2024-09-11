using System.Collections;
using Deepglint.XR;
using Deepglint.XR.EventSystem;
using Deepglint.XR.EventSystem.EventData;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Inputs.Devices;
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
        private float _slideRightArmToLeftRange = 0;
        private int _slideLeftArmToRightCount = 0;
        private float _slideLeftArmToRightRange = 0;
        private Coroutine slideLeftCoroutine;
        private Coroutine slideRightCoroutine;

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
        
        public void JumpRangeControl(InputAction.CallbackContext value)
        {
            float j = value.ReadValue<float>();
            DGXR.Logger.Log($"jumpRange: {j}, {value.phase.ToString()}");
        }

        public void JumpControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                DGXR.Logger.Log("Jump action performed");
                jump();
            } else if (value.canceled)
            {
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                float jumpValue = device.JumpRange.ReadValue();
                if (jumpValue > 0)
                {
                    DGXR.Logger.Log($"cancel Jump action with value: {jumpValue}");
                }
            }
        }
        
        public void CheerUpControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                DGXR.Logger.Log("CheerUp action performed");
            }
        }
    
        public void SlideRightArmToLeftControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _slideRightArmToLeftCount++;
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                DGXR.Logger.Log($"SlideRightArmToLeft action performed count: {_slideRightArmToLeftCount}, value: {device.SlideRightArmToLeftRange.ReadValue()}");
                slideLeftCoroutine = StartCoroutine(ReadSlideRightArmToLeftValue(device));
            } else if (value.canceled)
            {
                if (slideLeftCoroutine != null)
                {
                    StopCoroutine(slideLeftCoroutine);
                    slideLeftCoroutine = null;
                    DGXR.Logger.Log($"SlideRightArmToLeft action canceled with value: {_slideRightArmToLeftRange}");
                    _slideRightArmToLeftRange = 0f;
                }
            }
        }
    
        public void SlideLeftArmToRightControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _slideLeftArmToRightCount++;
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                DGXR.Logger.Log($"SlideLeftArmToRight performed count: {_slideLeftArmToRightCount}, value: {device.SlideLeftArmToRightRange.ReadValue()}");
                slideRightCoroutine = StartCoroutine(ReadSlideLeftArmToRightValue(device));
            } else if (value.canceled)
            {
                if (slideRightCoroutine != null)
                {
                    StopCoroutine(slideRightCoroutine);
                    slideRightCoroutine = null;
                    DGXR.Logger.Log($"SlideLeftArmToRight canceled with value: {_slideLeftArmToRightRange}");
                    _slideLeftArmToRightRange = 0f;
                }
            }
        }
        
        private IEnumerator ReadSlideRightArmToLeftValue(DGXRHumanController device)
        {
            while (true)
            {
                if (device.enabled)
                {
                    _slideRightArmToLeftRange = device.SlideRightArmToLeftRange.ReadValue();
                }
                // 等待下一帧
                yield return null;
            }
        }
        
        private IEnumerator ReadSlideLeftArmToRightValue(DGXRHumanController device)
        {
            while (true)
            {
                if (device.enabled)
                {
                    _slideLeftArmToRightRange = device.SlideLeftArmToRightRange.ReadValue();
                }
                // 等待下一帧
                yield return null;
            }
        }
    
        public void FreeSwimControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _freeSwimCount++;
                DGXR.Logger.Log($"free-swim count: {_freeSwimCount}");
            }
        }
    
        public void ButterflySwimControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _butterflySwimCount++;
                DGXR.Logger.Log($"butterfly-swim count: {_butterflySwimCount}");
            }
        }
    
        public void HighKneeRunControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _highKneeRunSwimCount++;
                DGXR.Logger.Log("high-knee-run count: {0}", _highKneeRunSwimCount);
            }
        }
    
        public void DeepSquatControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _deepSquatCount++;
                DGXR.Logger.Log($"deep-squat performed and count: {_deepSquatCount}");
            } else if (value.canceled)
            {
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                float squatValue = device.SquatRange.ReadValue();
                if (squatValue > 0)
                {
                    DGXR.Logger.Log($"cancel Squat action with value: {squatValue}");
                }
            }
        }

        public void RaiseBothHandControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("raise both hand");
            }
        }
        
        public void RaiseSingleHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                DGXR.Logger.Log("Raise-Single-Hand action performed");
            }
        }

        public void RaiseRightHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                DGXR.Logger.Log("Raise-Right-Hand action performed");
            }
        }
        
        public void RaiseLeftHandControl(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                DGXR.Logger.Log("Raise-Left-Hand action performed");
            }
        }
    
        void OnJump(InputValue value)
        {
            bool data = value.isPressed;
            if(data)
            {
                DGXR.Logger.Log("jump, " + _isOnGround);
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
                    DGXR.Logger.Log("rb is null");
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
                DGXR.Logger.Log($"high-five action with {player.Character.Name}");
            }
        }
    }
}