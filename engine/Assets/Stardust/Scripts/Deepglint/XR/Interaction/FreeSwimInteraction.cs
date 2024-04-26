using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if free-swim action is performed
    /// depending on the DGXRController.freeSwim control value.
    /// </summary>
    public class FreeSwimInteraction : MetaverseInteraction, IInputInteraction
    {
        private bool rightHandHit = false;

        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRController dgXRDevice)
            {
                if (IsFreeSwimHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsFreeSwimHit(dgXRDevice, true))
                            {
                                rightHandHit = false;
                                context.Started();
                            }
                            else if (IsFreeSwimHit(dgXRDevice))
                            {
                                rightHandHit = true;
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsFreeSwimHit(dgXRDevice, rightHandHit))
                            {
                                Debug.Log("FreeSwim action performed");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsFreeSwimHit(dgXRDevice, rightHandHit))
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

        private bool IsFreeSwimHappening(DGXRController dgXRDevice)
        {
            if (dgXRDevice.FreeSwim.ReadValue() > confidence)
            {
                Debug.Log("Free-Swim action is happening");
                return true;
            }

            return false;
        }

        private bool IsFreeSwimHit(DGXRController device, bool leftHand = false)
        {
            return leftHand
                ? device.HumanBody.leftWrist.position.y.ReadValue() >
                  device.HumanBody.leftElbow.position.y.ReadValue()
                : device.HumanBody.rightWrist.position.y.ReadValue() >
                  device.HumanBody.rightElbow.position.y.ReadValue();
        }

        public void Reset()
        {
            base.Reset();
            rightHandHit = false;
            Debug.Log("reset Free-Swim interaction");
        }
    }
}

