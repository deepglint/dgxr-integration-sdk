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
            if (context.control.device is DGXRController dgXRDevice)
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
                                Debug.Log("Butterfly-Swim action performed");
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

        private bool IsButterflySwimHappening(DGXRController dgXRDevice)
        {
            if (dgXRDevice.ButterflySwim.ReadValue() > confidence)
            {
                Debug.Log("Butterfly-Swim action is happening");
                return true;
            }

            return false;
        }

        private bool IsButterflySwimStart(DGXRController device)
        {
            return device.HumanBody.leftWrist.position.y.ReadValue() > 
                   device.HumanBody.leftElbow.position.y.ReadValue() && 
                   device.HumanBody.rightWrist.position.y.ReadValue() > 
                   device.HumanBody.rightElbow.position.y.ReadValue(); 
        }

        private bool IsButterflySwimHit(DGXRController device)
        {
            return device.HumanBody.leftWrist.position.y.ReadValue() > 
                   device.HumanBody.headTop.position.y.ReadValue() && 
                   device.HumanBody.rightWrist.position.y.ReadValue() > 
                   device.HumanBody.headTop.position.y.ReadValue();
        }

        public void Reset()
        {
            base.Reset();
            Debug.Log("reset Butterfly-Swim interaction");
        }
    }
}
