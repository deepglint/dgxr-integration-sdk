using DeepGlint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeepGlint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if the right arm is performing on turning page action(slide right arm to left)
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class SlideRightArmToLeftInteraction : IInputInteraction
    {
        private float _armAngle = 180;
        private float _distance = 3;
        private readonly float _distanceOffset = 0.05f;
        private readonly float _angleOffset = 0.5f;

        public float StartArmAngle = 140;
        public float PerformArmAngle = 90;
        
        private int _missCount = 0;
        
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRController dgXRDevice)
            {
                if (IsSlideRightArmToLeftHappening(dgXRDevice))
                {
                    // 右手肘夹角小于startArmAngle时进入started状态，小于performArmAngle度时进入perform状态；
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (_armAngle >= StartArmAngle)
                            {
                                // Debug.Log("SlideRightArmToLeft action started");
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (_armAngle <= PerformArmAngle)
                            {
                                // Debug.Log("SlideRightArmToLeft action performed");
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
        private bool IsSlideRightArmToLeftHappening(DGXRController dgXRDevice)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }

            Vector3 rightWrist = dgXRDevice.HumanBody.RightWrist.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.RightShoulder.position.ReadValue();
            
            if (rightWrist.y > rightShoulder.y || rightWrist.y < dgXRDevice.HumanBody.RightHip.position.y.ReadValue())
            {
                // Debug.Log("SlideRightArmToLeft action miss by position.y");
                return false;
            }

            float currentDistance = Vector3.Distance(rightWrist, dgXRDevice.HumanBody.LeftShoulder.position.ReadValue());
            if (currentDistance > _distance + _distanceOffset) 
            {
                // Debug.LogFormat("SlideRightArmToLeft action miss by distance {0},{1}", distance, currentDistance);
                _distance = currentDistance; 
                return false;
            }
            
            _distance = currentDistance;

            Vector3 rightElbow = dgXRDevice.HumanBody.RightElbow.position.ReadValue();
            float currentAngle = Vector3.Angle(rightWrist - rightElbow, rightShoulder - rightElbow);
            if (currentAngle > _armAngle + _angleOffset)
            {
                // Debug.LogFormat("SlideRightArmToLeft action miss by angle {0}, {1}", armAngle, currentAngle);
                _armAngle = currentAngle;
                return false;
            }

            _armAngle = currentAngle;
            // Debug.Log("SlideRightArmToLeft action is happening");
            _missCount = 0;
            return true;
        }

        public void Reset()
        {
            _missCount = 0;
            _distance = 180;
            _armAngle = 3;
        }
    }
}

