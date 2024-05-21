using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if raise-both-hand action is performed
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class RaiseBothHandInteraction : IInputInteraction
    {
        /// <summary>
        /// required hit count to perform this action
        /// </summary>
        public int RequiredHits = 3;

        /// <summary>
        /// required arm angle to perform this action
        /// </summary>
        public float RequiredArmAngle = 120f;
        
        private Dictionary<int, int> _hitDictionary = new Dictionary<int, int>();

        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsRaiseBothHandHappening(dgXRDevice, RequiredArmAngle))
                {
                    if (_hitDictionary.ContainsKey(dgXRDevice.deviceId))
                    {
                        _hitDictionary[dgXRDevice.deviceId] += 1;
                    }
                    else
                    {
                        _hitDictionary[dgXRDevice.deviceId] = 1;
                    }

                    if (context.phase == InputActionPhase.Waiting)
                    {
                        context.Started();
                    } else if (context.phase == InputActionPhase.Started)
                    {
                        if (_hitDictionary[dgXRDevice.deviceId] >= RequiredHits)
                        {
                            context.PerformedAndStayPerformed();
                            //Debug.Log("perform raise both hand on device " + device.deviceId);
                        }
                    }
                }
                else
                {
                    _hitDictionary[dgXRDevice.deviceId] = 0;
                    if (context.phase == InputActionPhase.Performed)
                    {
                        //Debug.Log("cancel raise both hand on device" + device.deviceId);
                        context.Canceled();
                    }
                }
            }
        }

        internal static bool IsRaiseBothHandHappening(DGXRHumanController dgXRDevice, float armAngle)
        {
            if (RaiseRightHandInteraction.IsRaiseRightHandHappening(dgXRDevice, armAngle) &&
                RaiseLeftHandInteraction.IsRaiseLeftHandHappening(dgXRDevice, armAngle))
            {
                return true;
            }

            return false;
        }
        
        public void Reset()
        {
        }
    }
}
