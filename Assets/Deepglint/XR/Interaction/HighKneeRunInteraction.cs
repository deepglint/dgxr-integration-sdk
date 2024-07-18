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
        private bool _rightKneeHit = false;
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsHighKneeRunHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsHighKneeRunHit(dgXRDevice, true))
                            {
                                _rightKneeHit = false;
                                context.Started();
                            }
                            else if (IsHighKneeRunHit(dgXRDevice))
                            {
                                _rightKneeHit = true;
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsHighKneeRunHit(dgXRDevice, _rightKneeHit))
                            {
                                // Debug.Log($"HighKneeRun action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsHighKneeRunHit(dgXRDevice, _rightKneeHit))
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

        private bool IsHighKneeRunHappening(DGXRHumanController device)
        {
            if (device.HighKneeRun.ReadValue() > Confidence)
            {
                // Debug.Log("High-Knee-Run action is happening");
                return true;
            }

            return false;
        }

        private bool IsHighKneeRunHit(DGXRHumanController device, bool leftKnee = false)
        {
            return leftKnee
                ? device.HumanBody.LeftFoot.position.y.ReadValue() >
                  device.HumanBody.RightAnkle.position.y.ReadValue()
                : device.HumanBody.RightFoot.position.y.ReadValue() >
                  device.HumanBody.LeftAnkle.position.y.ReadValue();
        }

        public new void Reset()
        {
            base.Reset();
            _rightKneeHit = false;
            // Debug.Log("reset High-Knee-Run interaction");
        }
    }
}

