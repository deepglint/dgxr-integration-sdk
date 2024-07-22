using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if butterfly-swim action is performed
    /// depending on the DGXRController.butterflySwim control value.
    /// </summary>
    public class ButterflySwimInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsButterflySwimHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsButterflySwimStart(dgXRDevice))
                            {
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsButterflySwimHit(dgXRDevice))
                            {
                                // Debug.Log($"ButterflySwim action performed on device {dgXRDevice.deviceId}");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsButterflySwimHit(dgXRDevice))
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

        private bool IsButterflySwimHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.ButterflySwim.ReadValue() > Confidence)
            {
                // Debug.Log("Butterfly-Swim action is happening");
                return true;
            }

            return false;
        }

        private bool IsButterflySwimStart(DGXRHumanController device)
        {
            return device.HumanBody.LeftWrist.position.y.ReadValue() > 
                   device.HumanBody.LeftElbow.position.y.ReadValue() && 
                   device.HumanBody.RightWrist.position.y.ReadValue() > 
                   device.HumanBody.RightElbow.position.y.ReadValue(); 
        }

        private bool IsButterflySwimHit(DGXRHumanController device)
        {
            return device.HumanBody.LeftWrist.position.y.ReadValue() > 
                   device.HumanBody.HeadTop.position.y.ReadValue() && 
                   device.HumanBody.RightWrist.position.y.ReadValue() > 
                   device.HumanBody.HeadTop.position.y.ReadValue();
        }

        public new void Reset()
        {
            base.Reset();
            // Debug.Log("reset Butterfly-Swim interaction");
        }
    }
}
