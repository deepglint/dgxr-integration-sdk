using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if applaud action is performed
    /// depending on the DGXRController.applaud control value.
    /// </summary>
    public class ApplaudInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsApplaudHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsApplaudStart(dgXRDevice))
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsApplaudHit(dgXRDevice))
                            {
                                // Debug.Log($"Applaud action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsApplaudHit(dgXRDevice))
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
        
        private bool IsApplaudHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.Applaud.ReadValue() > Confidence)
            {
                return true;
            }

            return false;
        }

        private bool IsApplaudStart(DGXRHumanController device)
        {
            return device.HumanBody.LeftWrist.position.y.ReadValue() > 
                   device.HumanBody.LeftShoulder.position.y.ReadValue() && 
                   device.HumanBody.RightWrist.position.y.ReadValue() > 
                   device.HumanBody.RightShoulder.position.y.ReadValue(); 
        }

        private bool IsApplaudHit(DGXRHumanController device)
        {
            return device.HumanBody.LeftWrist.position.y.ReadValue() > 
                   device.HumanBody.HeadTop.position.y.ReadValue() && 
                   device.HumanBody.RightWrist.position.y.ReadValue() > 
                   device.HumanBody.HeadTop.position.y.ReadValue();
        }

        public new void Reset()
        {
            base.Reset();
        }
    }
}