using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if raise-left-hand action is performed
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class RaiseLeftHandInteraction : IInputInteraction
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
                if (IsRaiseLeftHandHappening(dgXRDevice, RequiredArmAngle))
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
                            //Debug.Log("perform raise right hand on device: " + device.deviceId);
                        }
                    }
                }
                else
                {
                    _hitDictionary[dgXRDevice.deviceId] = 0;
                    if (context.phase == InputActionPhase.Performed)
                    {
                        //Debug.Log("cancel raise right hand on device" + device.deviceId);
                        context.Canceled();
                    }
                }
            }
        }

        internal static bool IsRaiseLeftHandHappening(DGXRHumanController dgXRDevice, float armAngle)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }
            
            if (dgXRDevice.HumanBody.LeftWrist.position.y.ReadValue() <=
                dgXRDevice.HumanBody.HeadTop.position.y.ReadValue())
            {
                return false;
            }
            
            float leftArmAngle = Vector3.Angle(
                dgXRDevice.HumanBody.LeftWrist.position.ReadValue() - dgXRDevice.HumanBody.LeftElbow.position.ReadValue(),
                dgXRDevice.HumanBody.LeftShoulder.position.ReadValue() - dgXRDevice.HumanBody.LeftElbow.position.ReadValue());
            if (leftArmAngle < armAngle)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
        }
    }
}

