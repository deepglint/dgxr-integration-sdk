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
            if (context.control.device is DGXRController dgXRDevice)
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

        private bool IsDeepSquatHappening(DGXRController dgXRDevice)
        {
            if (dgXRDevice.DeepSquat.ReadValue() > confidence)
            {
                Debug.Log("Deep-Squat action is happening");
                return true;
            }

            return false;
        }

        private bool IsDeepSquatActionStart(DGXRController device)
        {
            float legLength = Vector3.Distance(device.HumanBody.leftHip.position.ReadValue(),
                device.HumanBody.leftKnee.position.ReadValue());
            return Math.Abs(device.HumanBody.leftHip.position.y.ReadValue() -
                    device.HumanBody.leftKnee.position.y.ReadValue()) <= legLength * 0.5f;
        }

        private bool IsDeepSquatActionHit(DGXRController device)
        {
            return device.HumanBody.rightHip.position.y.ReadValue() -
                            device.HumanBody.rightKnee.position.y.ReadValue() <= 0f;
        }

        public void Reset()
        {
            base.Reset();
            Debug.Log("reset Deep-Squat interaction");
        }
    }
}

