using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;
namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if rightHandDrawCircle action is performed
    /// depending on the DGXRController.rightHandDrawCircle control value.
    /// </summary>
    public class RightHandDrawCircleInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                DGXR.Logger.Log("------------------Action 调试信息：RightHandDrawCircle = " + dgXRDevice.RightHandDrawCircle.ReadValue() + " " + context.phase);
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
            return dgXRDevice.RightHandDrawCircle.ReadValue() > Confidence;
        }
        private bool IsStart(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.RightHandDrawCircle.ReadValue() > Confidence;
        }
        private bool IsHit(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.RightHandDrawCircle.ReadValue() > Confidence;
        }
        public new void Reset()
        {
            base.Reset();
        }
    }
}