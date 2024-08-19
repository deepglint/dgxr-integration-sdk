using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if cheer-up action is performed
    /// depending on the DGXRController.applaud control value.
    /// </summary>
    public class CheerUpInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsCheerUpHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsCheerUpStart(dgXRDevice))
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsCheerUpHit(dgXRDevice))
                            {
                                // Debug.Log($"CheerUp action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsCheerUpHit(dgXRDevice))
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
        
        private bool IsCheerUpHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.CheerUp.ReadValue() > Confidence)
            {
                return true;
            }

            return false;
        }

        private bool IsCheerUpStart(DGXRHumanController device)
        {
            return device.HumanBody.LeftWrist.position.y.ReadValue() > 
                   device.HumanBody.LeftShoulder.position.y.ReadValue() && 
                   device.HumanBody.RightWrist.position.y.ReadValue() > 
                   device.HumanBody.RightShoulder.position.y.ReadValue(); 
        }

        private bool IsCheerUpHit(DGXRHumanController device)
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