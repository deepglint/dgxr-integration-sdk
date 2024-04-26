using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if raise-hand action is performed
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class RaiseHandInteraction : IInputInteraction
    {
        /// <summary>
        /// required tap count to perform this action
        /// </summary>
        public int requiredTaps = 3; 
        
        /// <summary>
        /// required arm angle to perform this action
        /// </summary>
        public float requiredArmAngle = 120f;
        
        private Dictionary<int, int> tapDictionary = new Dictionary<int, int>();
        
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRController dgXRDevice)
            {
                if (IsRaiseHandHappening(dgXRDevice, requiredArmAngle))
                {
                    if (tapDictionary.ContainsKey(dgXRDevice.deviceId))
                    {
                        tapDictionary[dgXRDevice.deviceId] += 1;
                    }
                    else
                    {
                        tapDictionary[dgXRDevice.deviceId] = 1;
                    } 
                    
                    if (context.phase == InputActionPhase.Waiting)
                    {
                        context.Started();
                    } else if (context.phase == InputActionPhase.Started)
                    {
                        if (tapDictionary[dgXRDevice.deviceId] >= requiredTaps)
                        {
                            context.PerformedAndStayPerformed();
                            //Debug.Log("perform raise right hand on device: " + device.deviceId);
                        }
                    }
                }
                else
                {
                    tapDictionary[dgXRDevice.deviceId] = 0;
                    if (context.phase == InputActionPhase.Performed)
                    {
                        //Debug.Log("cancel raise right hand on device" + device.deviceId);
                        context.Canceled();
                    }
                }
            }
        }

        internal static bool IsRaiseHandHappening(DGXRController dgXRDevice, float armAngle)
        {
            if (RaiseRightHandInteraction.IsRaiseRightHandHappening(dgXRDevice, armAngle) ||
                RaiseLeftHandInteraction.IsRaiseRightHandHappening(dgXRDevice, armAngle))
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

