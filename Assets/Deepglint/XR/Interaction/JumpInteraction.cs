using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if Jump action is performed
    /// depending on the DGXRController.jump control value.
    /// </summary>
    public class JumpInteraction : MetaverseInteraction, IInputInteraction
    {
        private float _startHeight = 0f;
        private float _head = 0f;
        private float _heightOffset = 0.01f;
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsJumpActionHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            // Debug.Log("jump start");
                            context.Started();
                            _startHeight = dgXRDevice.HumanBody.HeadTop.position.y.ReadValue();
                            break;
                        case InputActionPhase.Started:
                            if (IsJumpActionPerformed())
                            {
                                // Debug.Log($"Jump action performed on device {dgXRDevice.deviceId}");
                                InputSystem.QueueDeltaStateEvent(dgXRDevice.JumpRange, dgXRDevice.HumanBody.HeadTop.position.y.ReadValue() - _startHeight);
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            float value = dgXRDevice.HumanBody.HeadTop.position.y.ReadValue() - _startHeight;
                            InputSystem.QueueDeltaStateEvent(dgXRDevice.JumpRange, value);
                            break;
                    }
                }
                else
                {
                    if (CheckMissCancel(ref context))
                    {
                        InputSystem.QueueDeltaStateEvent(dgXRDevice.JumpRange, 0f);
                    }
                }
            }
        }

        private bool IsJumpActionHappening(DGXRHumanController dgXRDevice)
        {
            float height = dgXRDevice.HumanBody.HeadTop.position.y.ReadValue();
            try
            {
                if (dgXRDevice.Jump.ReadValue() <= Confidence)
                {
                    return false;
                }

                if (height < _head - _heightOffset)
                {
                    return false;
                }
            }
            finally
            {
                _head = height;
            }

            HitCount++;
            return true;
        }

        private bool IsJumpActionPerformed()
        {
            return HitCount >= HitThreshold;
        }

        public new void Reset()
        {
            base.Reset();
            // Debug.Log("reset jump interaction");
        }
    }
}

