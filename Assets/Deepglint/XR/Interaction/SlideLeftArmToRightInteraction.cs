using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if the left arm is performing on turning page action(slide left arm to right)
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class SlideLeftArmToRightInteraction : IInputInteraction
    {
        private float _armAngle = 180;
        private float _distance = 10;
        private readonly float _distanceOffset = 0.05f;
        private readonly float _angleOffset = 0.5f;
        private int _missCount = 0;
        private float _startArmAngle = 0;
        private float StartArmAngleThreshold = 40;
        public float SlideArmAngleThreshold = 40;
        
        public void Process(ref InputInteractionContext context)
        {
            if (StartArmAngleThreshold < SlideArmAngleThreshold)
            {
                StartArmAngleThreshold = SlideArmAngleThreshold;
            }
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsSlideLeftArmToRightHappening(dgXRDevice))
                {
                    // 手臂夹角小于startArmAngle时进入started状态，小于performArmAngle度时进入perform状态；
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            var distance = Vector3.Distance(dgXRDevice.HumanBody.LeftWrist.position.ReadValue(),
                                dgXRDevice.HumanBody.LeftShoulder.position.ReadValue());
                            if (_armAngle >= StartArmAngleThreshold && distance < _distance)
                            {
                                context.Started();
                                _startArmAngle = _armAngle;
                            }
                            break;
                        case InputActionPhase.Started:
                            if (_armAngle <= _startArmAngle - SlideArmAngleThreshold)
                            {
                                context.PerformedAndStayPerformed();
                                // Debug.Log($"SlideLeftArmToRight action performed on device {dgXRDevice.deviceId}");
                            }

                            break;
                    }
                }
                else
                {
                    _missCount++;
                    if (_missCount >= 3 &&
                        (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started))
                    {
                        _startArmAngle = 0;
                        context.Canceled();
                    }
                }
            }
        }

        /// <summary>
        /// Check if SlideLeftArmToRight Action is happening
        /// 1. 做动作期间左手高度不能超过鼻子高度，不能低于髋关节高度；
        /// 2. 左手与右肩的距离越来越小；
        /// </summary>
        /// <param name="dgXRDevice"></param>
        /// <returns></returns>
        private bool IsSlideLeftArmToRightHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }

            Vector3 leftWrist = dgXRDevice.HumanBody.LeftWrist.position.ReadValue();
            Vector3 leftShoulder = dgXRDevice.HumanBody.LeftShoulder.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
            
            float currentAngle = Vector3.Angle(leftWrist - leftShoulder, rightShoulder - leftShoulder);
            float currentDistance = Vector3.Distance(leftWrist, rightShoulder);
            try
            {
                if (leftWrist.y > dgXRDevice.HumanBody.HeadTop.position.y.ReadValue() || 
                    leftWrist.y < dgXRDevice.HumanBody.LeftHip.position.y.ReadValue() ||
                    !IsHandInFrontOfBody(dgXRDevice))
                {
                    return false;
                }

                if (currentDistance > _distance + _distanceOffset) 
                {
                    return false;
                }
                if (currentAngle > _armAngle + _angleOffset)
                {
                    return false;
                }
            }
            finally
            {
                _distance = currentDistance; 
                _armAngle = currentAngle;
                var dis = Vector3.Distance(dgXRDevice.HumanBody.LeftWrist.position.ReadValue(), dgXRDevice.HumanBody.LeftShoulder.position.ReadValue());
                //Debug.LogFormat("distance: {0}, angle: {1}, rd: {2}", _distance, _armAngle, dis);
            }
            
            _missCount = 0;
            return true;
        }

        private bool IsHandInFrontOfBody(DGXRHumanController dgXRDevice)
        {
            Vector3 leftHip = dgXRDevice.HumanBody.LeftHip.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
            Vector3 leftShoulder = dgXRDevice.HumanBody.LeftShoulder.position.ReadValue();
            Vector3 leftWrist = dgXRDevice.HumanBody.LeftWrist.position.ReadValue();
            Vector3 planeNormal = Vector3.Cross(leftHip - leftShoulder, rightShoulder - leftShoulder).normalized;
        
            float distance = Vector3.Dot(planeNormal, leftWrist - leftShoulder);
            return Mathf.Sign(distance) > 0;
        }
        
        public void Reset()
        {
            _missCount = 0;
            _distance = 10;
            _armAngle = 180;
            _startArmAngle = 0;
        }
    }
}

