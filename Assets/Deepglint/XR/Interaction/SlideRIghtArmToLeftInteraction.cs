using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if the right arm is performing on turning page action(slide right arm to left)
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class SlideRightArmToLeftInteraction : IInputInteraction
    {
        private float _armAngle = 180;
        private float _distance = 10;
        private readonly float _distanceOffset = 0.05f;
        private readonly float _angleOffset = 0.5f;
        private Vector3 _startShoulder = Vector3.zero;
        private int _missCount = 0;

        public float StartArmAngle = 120;
        public float PerformArmAngle = 76;
        
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsSlideRightArmToLeftHappening(dgXRDevice))
                {
                    // 手臂夹角小于startArmAngle时进入started状态，小于performArmAngle度时进入perform状态；
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            var distance = Vector3.Distance(dgXRDevice.HumanBody.RightWrist.position.ReadValue(),
                                dgXRDevice.HumanBody.RightShoulder.position.ReadValue());
                            // Debug.LogFormat("arm angle: {0}, distance {1}", _armAngle, distance);
                            if (_armAngle >= StartArmAngle  && distance < _distance)
                            {
                                // Debug.Log("SlideRightArmToLeft action started");
                                context.Started();
                                _startShoulder = dgXRDevice.HumanBody.LeftShoulder.position.ReadValue() -
                                                 dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
                            }
                            break;
                        case InputActionPhase.Started:
                            float shoulderAngle = Vector3.Angle(dgXRDevice.HumanBody.LeftShoulder.position.ReadValue() -
                                                              dgXRDevice.HumanBody.RightShoulder.position.ReadValue(), _startShoulder);
                            // Debug.LogFormat("arm angle: {0}, shoulder angle {1}", _armAngle, shoulderAngle);
                            if (_armAngle - shoulderAngle * 0.5f <= PerformArmAngle)
                            {
                                // Debug.Log($"SlideRightArmToLeft action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
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
                        // Debug.Log("SlideRightArmToLeft action canceled");
                        context.Canceled();
                    }
                }
            }
        }

        /// <summary>
        /// check if SlideRightArmToLeft Action is happening
        /// 1. 做动作期间右手高度不能超过肩膀高度，不能低于髋关节高度；
        /// 2. 右手与左肩的距离越来越小；
        /// </summary>
        /// <param name="dgXRDevice"></param>
        /// <returns></returns>
        private bool IsSlideRightArmToLeftHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }

            Vector3 rightWrist = dgXRDevice.HumanBody.RightWrist.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
            Vector3 leftShoulder = dgXRDevice.HumanBody.LeftShoulder.position.ReadValue();
            
            float currentAngle = Vector3.Angle(rightWrist - rightShoulder, leftShoulder - rightShoulder);
            float currentDistance = Vector3.Distance(rightWrist, leftShoulder);
            try
            {
                if (rightWrist.y > dgXRDevice.HumanBody.HeadTop.position.y.ReadValue() || 
                    rightWrist.y < dgXRDevice.HumanBody.RightHip.position.y.ReadValue() || 
                    !IsHandInFrontOfBody(dgXRDevice))
                {
                    // Debug.Log("SlideRightArmToLeft action miss by wrong direction");
                    return false;
                }
                if (currentDistance > _distance + _distanceOffset) 
                {
                    // Debug.LogFormat("SlideRightArmToLeft action miss by distance {0},{1}", _distance, currentDistance);
                    _distance = currentDistance; 
                    return false;
                }
                if (currentAngle > _armAngle + _angleOffset)
                {
                    // Debug.LogFormat("SlideRightArmToLeft action miss by angle {0}, {1}", _armAngle, currentAngle);
                    return false;
                }
            }
            finally
            {
                _distance = currentDistance;
                _armAngle = currentAngle;
            }

            _missCount = 0;
            return true;
        }

        private bool IsHandInFrontOfBody(DGXRHumanController dgXRDevice)
        {
            Vector3 rightHip = dgXRDevice.HumanBody.RightHip.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
            Vector3 leftShoulder = dgXRDevice.HumanBody.LeftShoulder.position.ReadValue();
            Vector3 rightWrist = dgXRDevice.HumanBody.RightWrist.position.ReadValue();
            Vector3 planeNormal = -Vector3.Cross(rightHip - rightShoulder, leftShoulder - rightShoulder).normalized;
        
            float distance = Vector3.Dot(planeNormal, rightWrist - rightShoulder);
            return Mathf.Sign(distance) > 0;
        }

        public void Reset()
        {
            _missCount = 0;
            _distance = 10;
            _armAngle = 180;
        }
    }
}

