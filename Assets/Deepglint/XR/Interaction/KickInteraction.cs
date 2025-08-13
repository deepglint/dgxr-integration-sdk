using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;
namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if kick action is performed
    /// depending on the DGXRController.kick control value.
    /// </summary>
    public class KickInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                DGXR.Logger.Log("------------------Action 调试信息：Kick = " + dgXRDevice.Kick.ReadValue() + " " + context.phase);
                if (IsHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsStart(dgXRDevice))
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsHit(dgXRDevice))
                            {
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsHit(dgXRDevice))
                            {
                                context.Canceled();
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
        private bool IsHappening(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.Kick.ReadValue() > Confidence;
        }
        private bool IsStart(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.Kick.ReadValue() > Confidence;
        }
        private bool IsHit(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.Kick.ReadValue() > Confidence;
        }
        public new void Reset()
        {
            base.Reset();
        }
    }
}