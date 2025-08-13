using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if handBevelCut action is performed
    /// depending on the DGXRController.handBevelCut control value.
    /// </summary>
    public class HandBevelCutInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                DGXR.Logger.Log("------------------Action 调试信息：HandBevelCut = " + dgXRDevice.HandBevelCut.ReadValue() +
                                " " + context.phase);
                if (IsHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsStart(dgXRDevice))
                            {
                                DGXR.Logger.Log("手斜切切切======= started：");
                                context.Started();
                            }

                            break;
                        case InputActionPhase.Started:
                            if (IsHit(dgXRDevice))
                            {
                                DGXR.Logger.Log("手斜切切切======= preformed：");
                                context.PerformedAndStayPerformed();
                            }

                            break;
                        case InputActionPhase.Performed:
                            if (!IsHit(dgXRDevice))
                            {
                                DGXR.Logger.Log("手斜切切切======= canceled：");
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
            return dgXRDevice.HandBevelCut.ReadValue() > Confidence;
        }

        private bool IsStart(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.HandBevelCut.ReadValue() > Confidence;
        }

        private bool IsHit(DGXRHumanController dgXRDevice)
        {
            return dgXRDevice.HandBevelCut.ReadValue() > Confidence;
        }

        public new void Reset()
        {
            base.Reset();
        }
    }
}