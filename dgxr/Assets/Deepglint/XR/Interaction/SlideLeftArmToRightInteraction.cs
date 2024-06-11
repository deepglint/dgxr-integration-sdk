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

        public float StartArmAngle = 120;
        public float PerformArmAngle = 60;
        
        private int _missCount = 0;
        
        public void Process(ref InputInteractionContext context)
        {
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
                            if (_armAngle >= StartArmAngle && distance < _distance)
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (_armAngle <= PerformArmAngle)
                            {
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
            Vector3 nose = dgXRDevice.HumanBody.Nose.position.ReadValue();
            
            float currentAngle = Vector3.Angle(leftWrist - leftShoulder, rightShoulder - leftShoulder);
            float currentDistance = Vector3.Distance(leftWrist, rightShoulder);
            try
            {
                if (leftWrist.y > nose.y || leftWrist.y < dgXRDevice.HumanBody.LeftHip.position.y.ReadValue())
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

        public void Reset()
        {
            _missCount = 0;
            _distance = 10;
            _armAngle = 180;
        }
    }
}

