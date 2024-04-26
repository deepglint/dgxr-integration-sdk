using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if high-knee-run action is performed
    /// depending on the DGXRController.highKneeRun control value.
    /// </summary>
    public class HighKneeRunInteraction : MetaverseInteraction, IInputInteraction
    {
        private bool rightKneeHit = false;
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRController dgXRDevice)
            {
                if (IsHighKneeRunHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsHighKneeRunHit(dgXRDevice, true))
                            {
                                rightKneeHit = false;
                                context.Started();
                            }
                            else if (IsHighKneeRunHit(dgXRDevice))
                            {
                                rightKneeHit = true;
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsHighKneeRunHit(dgXRDevice, rightKneeHit))
                            {
                                Debug.Log("High-Knee-Run action performed");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsHighKneeRunHit(dgXRDevice, rightKneeHit))
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

        private bool IsHighKneeRunHappening(DGXRController device)
        {
            if (device.HighKneeRun.ReadValue() > confidence)
            {
                Debug.Log("High-Knee-Run action is happening");
                return true;
            }

            return false;
        }

        private bool IsHighKneeRunHit(DGXRController device, bool leftKnee = false)
        {
            return leftKnee
                ? device.HumanBody.leftFoot.position.y.ReadValue() >
                  device.HumanBody.rightAnkle.position.y.ReadValue()
                : device.HumanBody.rightFoot.position.y.ReadValue() >
                  device.HumanBody.leftAnkle.position.y.ReadValue();
        }

        public void Reset()
        {
            base.Reset();
            rightKneeHit = false;
            Debug.Log("reset High-Knee-Run interaction");
        }
    }
}

