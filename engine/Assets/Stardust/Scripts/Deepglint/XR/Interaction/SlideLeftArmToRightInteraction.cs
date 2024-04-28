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
                if (IsSlideLeftArmToRightHappening(dgXRDevice))
                {
                    // 右手肘夹角小于startArmAngle时进入started状态，小于performArmAngle度时进入perform状态；
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (armAngle >= startArmAngle)
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (armAngle <= performArmAngle)
                            {
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
                        context.Canceled();
                    }
                }
            }
        }

        /// <summary>
        /// check if SlideLeftArmToRight Action is happening
        /// 1. 做动作期间左手高度不能超过肩膀高度，不能低于髋关节高度；
        /// 2. 左手与右肩的距离越来越小；
        /// </summary>
        /// <param name="dgXRDevice"></param>
        /// <returns></returns>
        private bool IsSlideLeftArmToRightHappening(DGXRController dgXRDevice)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }

            Vector3 leftWrist = dgXRDevice.HumanBody.leftWrist.position.ReadValue();
            Vector3 leftShoulder = dgXRDevice.HumanBody.leftShoulder.position.ReadValue();
            
            if (leftWrist.y > leftShoulder.y || leftWrist.y < dgXRDevice.HumanBody.leftHip.position.y.ReadValue())
            {
                return false;
            }

            float currentDistance = Vector3.Distance(leftWrist, dgXRDevice.HumanBody.rightShoulder.position.ReadValue());
            if (currentDistance > distance + distanceOffset) 
            {
                distance = currentDistance; 
                return false;
            }
            
            distance = currentDistance;

            Vector3 leftElbow = dgXRDevice.HumanBody.leftElbow.position.ReadValue();
            float currentAngle = Vector3.Angle(leftWrist - leftElbow, leftShoulder - leftElbow);
            if (currentAngle > armAngle + angleOffset)
            {
                armAngle = currentAngle;
                return false;
            }

            armAngle = currentAngle;
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

