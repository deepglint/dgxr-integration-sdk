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
        private float armAngle = 180;
        private float distance = 3;
        private float distanceOffset = 0.05f;
        private float angleOffset = 0.5f;

        public float startArmAngle = 140;
        public float performArmAngle = 90;
        
        private int missCount = 0;
        
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
                            if (armAngle >= startArmAngle)
                            {
                                // Debug.Log("SlideRightArmToLeft action started");
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (armAngle <= performArmAngle)
                            {
                                // Debug.Log("SlideRightArmToLeft action performed");
                                context.PerformedAndStayPerformed();
                            }

                            break;
                    }
                }
                else
                {
                    missCount++;
                    if (missCount >= 3 &&
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

            Vector3 rightWrist = dgXRDevice.HumanBody.rightWrist.position.ReadValue();
            Vector3 rightShoulder = dgXRDevice.HumanBody.rightShoulder.position.ReadValue();
            
            if (rightWrist.y > rightShoulder.y || rightWrist.y < dgXRDevice.HumanBody.rightHip.position.y.ReadValue())
            {
                // Debug.Log("SlideRightArmToLeft action miss by position.y");
                return false;
            }

            float currentDistance = Vector3.Distance(rightWrist, dgXRDevice.HumanBody.leftShoulder.position.ReadValue());
            if (currentDistance > distance + distanceOffset) 
            {
                // Debug.LogFormat("SlideRightArmToLeft action miss by distance {0},{1}", distance, currentDistance);
                distance = currentDistance; 
                return false;
            }
            
            distance = currentDistance;

            Vector3 rightElbow = dgXRDevice.HumanBody.rightElbow.position.ReadValue();
            float currentAngle = Vector3.Angle(rightWrist - rightElbow, rightShoulder - rightElbow);
            if (currentAngle > armAngle + angleOffset)
            {
                // Debug.LogFormat("SlideRightArmToLeft action miss by angle {0}, {1}", armAngle, currentAngle);
                armAngle = currentAngle;
                return false;
            }

            armAngle = currentAngle;
            // Debug.Log("SlideRightArmToLeft action is happening");
            missCount = 0;
            return true;
        }

        public void Reset()
        {
            missCount = 0;
            distance = 180;
            armAngle = 3;
        }
    }
}

