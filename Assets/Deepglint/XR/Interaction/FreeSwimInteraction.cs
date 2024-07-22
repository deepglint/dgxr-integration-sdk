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
        private bool _rightHandHit = false;

        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsFreeSwimHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsFreeSwimHit(dgXRDevice, true))
                            {
                                _rightHandHit = false;
                                context.Started();
                            }
                            else if (IsFreeSwimHit(dgXRDevice))
                            {
                                _rightHandHit = true;
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsFreeSwimHit(dgXRDevice, _rightHandHit))
                            {
                                // Debug.Log($"FreeSwim action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsFreeSwimHit(dgXRDevice, _rightHandHit))
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

        private bool IsFreeSwimHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.FreeSwim.ReadValue() > Confidence)
            {
                // Debug.Log("Free-Swim action is happening");
                return true;
            }

            return false;
        }

        private bool IsFreeSwimHit(DGXRHumanController device, bool leftHand = false)
        {
            return leftHand
                ? device.HumanBody.LeftWrist.position.y.ReadValue() >
                  device.HumanBody.LeftElbow.position.y.ReadValue()
                : device.HumanBody.RightWrist.position.y.ReadValue() >
                  device.HumanBody.RightElbow.position.y.ReadValue();
        }

        public new void Reset()
        {
            base.Reset();
            _rightHandHit = false;
            // Debug.Log("reset Free-Swim interaction");
        }
    }
}

