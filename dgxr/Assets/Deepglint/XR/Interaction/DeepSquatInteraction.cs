using System;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if Deep-Squat action is performed
    /// depending on the DGXRController.deepSquat control value.
    /// </summary>
    public class DeepSquatInteraction : MetaverseInteraction, IInputInteraction
    {
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsDeepSquatHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (IsDeepSquatActionStart(dgXRDevice))
                            {
                                Debug.Log("deep-squat start");
                                context.Started();
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsDeepSquatActionHit(dgXRDevice))
                            {
                                Debug.Log("Deep-Squat action performed");
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            if (!IsDeepSquatActionHit(dgXRDevice))
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

        private bool IsDeepSquatHappening(DGXRHumanController dgXRDevice)
        {
            if (dgXRDevice.DeepSquat.ReadValue() > Confidence)
            {
                Debug.Log("Deep-Squat action is happening");
                return true;
            }

            return false;
        }

        private bool IsDeepSquatActionStart(DGXRHumanController device)
        {
            float legLength = Vector3.Distance(device.HumanBody.LeftHip.position.ReadValue(),
                device.HumanBody.LeftKnee.position.ReadValue());
            return Math.Abs(device.HumanBody.LeftHip.position.y.ReadValue() -
                    device.HumanBody.LeftKnee.position.y.ReadValue()) <= legLength * 0.5f;
        }

        private bool IsDeepSquatActionHit(DGXRHumanController device)
        {
            return device.HumanBody.RightHip.position.y.ReadValue() -
                            device.HumanBody.RightKnee.position.y.ReadValue() <= 0f;
        }

        public new void Reset()
        {
            base.Reset();
            Debug.Log("reset Deep-Squat interaction");
        }
    }
}

