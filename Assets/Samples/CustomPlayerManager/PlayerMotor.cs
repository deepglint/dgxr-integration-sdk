using System;
using System.Collections;
using System.Collections.Generic;
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
        private int _deepSquatCount = 0;
        private int _slideRightArmToLeftCount = 0;
        private float _slideRightArmToLeftRange = 0;
        private int _slideLeftArmToRightCount = 0;
        private float _slideLeftArmToRightRange = 0;
        private Coroutine slideLeftCoroutine;
        private Coroutine slideRightCoroutine;

        private Dictionary<string, int> _actionsCount = new Dictionary<string, int>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            _actionsCount.Clear();
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
            if (value.performed)
            {
                DGXR.Logger.Log("Jump action performed");
                jump();
            }
            else if (value.canceled)
            {
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                float jumpValue = device.JumpRange.ReadValue();
                if (jumpValue > 0)
                {
                    DGXR.Logger.Log($"cancel Jump action with value: {jumpValue}");
                }
            }
        }

        public void SlideRightArmToLeftControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _slideRightArmToLeftCount++;
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                DGXR.Logger.Log(
                    $"SlideRightArmToLeft action performed count: {_slideRightArmToLeftCount}, value: {device.SlideRightArmToLeftRange.ReadValue()}");
                slideLeftCoroutine = StartCoroutine(ReadSlideRightArmToLeftValue(device));
            }
            else if (value.canceled)
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
            if (value.performed)
            {
                _slideLeftArmToRightCount++;
                DGXRHumanController device = (DGXRHumanController)value.control.device;
                DGXR.Logger.Log(
                    $"SlideLeftArmToRight performed count: {_slideLeftArmToRightCount}, value: {device.SlideLeftArmToRightRange.ReadValue()}");
                slideRightCoroutine = StartCoroutine(ReadSlideLeftArmToRightValue(device));
            }
            else if (value.canceled)
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

        public void DeepSquatControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _deepSquatCount++;
                DGXR.Logger.Log($"deep-squat performed and count: {_deepSquatCount}");
            }
            else if (value.canceled)
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
            if (value.performed)
            {
                DGXR.Logger.Log("Raise-Single-Hand action performed");
            }
        }

        public void RaiseRightHandControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("Raise-Right-Hand action performed");
            }
        }

        public void RaiseLeftHandControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("Raise-Left-Hand action performed");
            }
        }

        void OnJump(InputValue value)
        {
            bool data = value.isPressed;
            if (data)
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

        private int TryAddAction(string key)
        {
            if (_actionsCount.ContainsKey(key))
            {
                _actionsCount[key]++;
            }
            else
            {
                _actionsCount.TryAdd(key, 1);
            }
            
            return _actionsCount[key];
        }

        public void RightHandDrawCircleControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("RightHandDrawCircle-左手画圈 action performed " + TryAddAction("RightHandDrawCircle"));
            }
        }

        public void LeftHandDrawCircleControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("LeftHandDrawCircle-右手画圈 action performed " + TryAddAction("LeftHandDrawCircle"));
            }
        }

        public void HandBevelCutControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("HandBevelCut-手斜切 action performed " + TryAddAction("HandBevelCut"));
            }
        }

        public void HandParryControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("HandParry-手挡开 action performed " + TryAddAction("HandParry"));
            }
        }

        public void HandStraightCutControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("HandStraightCut-手直切 action performed " + TryAddAction("HandStraightCut"));
            }
        }

        public void HandTransversalControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("HandTransversal-手横切 action performed " + TryAddAction("HandTransversal"));
            }
        }

        public void StraightPunchControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("StraightPunch-直拳 action performed " + TryAddAction("StraightPunch"));
            }
        }

        public void ReadyStraightPunchControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ReadyStraightPunch-蓄力直拳 action performed " + TryAddAction("ReadyStraightPunch"));
            }
        }

        public void UppercutControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("Uppercut-上勾拳 action performed " + TryAddAction("Uppercut"));
            }
        }

        public void KickControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("Kick-踢腿 action performed " + TryAddAction("Kick"));
            }
        }

        public void ThrowOneHandInFistsControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ThrowOneHandInFists-单手握拳投掷 action performed " + TryAddAction("ThrowOneHandInFists"));
            }
        }

        public void ReadyThrowOneHandInFistsControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ReadyThrowOneHandInFists-蓄力单手握拳投掷 action performed " +
                                TryAddAction("ReadyThrowOneHandInFists"));
            }
        }

        public void ReadyThrowBothHandInFistsControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ReadyThrowBothHandInFists-蓄力双手握拳投掷 action performed " +
                                TryAddAction("ReadyThrowBothHandInFists"));
            }
        }


        public void CombineHandsStraightControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("CombineHandsStraight-双手伸直合并 action performed " + TryAddAction("CombineHandsStraight"));
            }
        }

        public void SlowRunControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("SlowRun-慢跑 action performed " + TryAddAction("SlowRun"));
            }
        }

        public void HighKneeRunControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("HighKneeRun-高抬腿 action performed " + TryAddAction("HighKneeRun"));
            }
        }

        public void ButterflySwimControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ButterflySwim-蝶泳 action performed " + TryAddAction("ButterflySwim"));
            }
        }

        public void FreeSwimControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("FreeSwim-自由泳 action performed " + TryAddAction("FreeSwim"));
            }
        }

        public void KeepRaisingHandControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("KeepRaisingHand-持续举手 action performed " + TryAddAction("KeepRaisingHand"));
            }
        }

        public void CheerUpControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("CheerUp-拍掌 action performed " + TryAddAction("CheerUp"));
            }
        }

        public void ArmFlatControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ArmFlat-手臂平展 action performed " + TryAddAction("ArmFlat"));
            }
        }

        public void ArmFlatIsLControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ArmFlatIsL-手臂平展为L action performed " + TryAddAction("ArmFlatIsL"));
            }
        }

        public void ArmVerticalIsLControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                DGXR.Logger.Log("ArmVerticalIsL-手臂垂直为L action performed " + TryAddAction("ArmVerticalIsL"));
            }
        }
    }
}