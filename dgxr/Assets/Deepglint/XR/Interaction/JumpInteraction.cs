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
                            break;
                        case InputActionPhase.Started:
                            if (IsJumpActionPerformed())
                            {
                                // Debug.Log("jump action performed");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                    }
                }
                else
                {
                    CheckMissCancel(ref context);
                }
            }
        }

        private bool IsJumpActionHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.Jump.ReadValue() > Confidence)
            {
                // Debug.Log("jump action is happening");
                HitCount++;
                // Debug.LogFormat("hit {0}, miss {1}, confidence: {2}", HitCount, MissCount, dgXRDevice.Jump.ReadValue());
                return true;
            }
            // Debug.LogFormat("hit {0}, miss {1}, confidence: {2}", HitCount, MissCount, dgXRDevice.Jump.ReadValue());

            return false;
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

