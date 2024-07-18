using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// A Interaction to judge if raise-right-hand action is performed
    /// depending on the DGXRController.humanBody control value.
    /// </summary>
    public class RaiseRightHandInteraction : IInputInteraction
    {
        /// <summary>
        /// required hit count to perform this action
        /// </summary>
        public int RequiredHits = 6; 
        
        /// <summary>
        /// required arm angle to perform this action
        /// </summary>
        public float RequiredArmAngle = 120f;
        
        private Dictionary<int, int> _hitDictionary = new Dictionary<int, int>();
        private Dictionary<int, int> _missDictionary = new Dictionary<int, int>();

        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsRaiseRightHandHappening(dgXRDevice, RequiredArmAngle))
                {
                    _missDictionary[dgXRDevice.deviceId] = 0;
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
                            // Debug.Log($"RaiseRightHand action performed on device {dgXRDevice.deviceId}");
                        }
                    }
                }
                else
                {
                    _hitDictionary[dgXRDevice.deviceId] = 0;
                    if (_missDictionary.ContainsKey(dgXRDevice.deviceId))
                    {
                        _missDictionary[dgXRDevice.deviceId] += 1;
                    }
                    else
                    {
                        _missDictionary[dgXRDevice.deviceId] = 1;
                    }
                    if (context.phase == InputActionPhase.Performed)
                    {
                        if (_missDictionary[dgXRDevice.deviceId] >= RequiredHits)
                        {
                            //Debug.Log("cancel raise right hand on device" + device.deviceId);
                            context.Canceled();
                        }
                    }
                }
            }
        }

        internal static bool IsRaiseRightHandHappening(DGXRHumanController dgXRDevice, float armAngle)
        {
            if (dgXRDevice.HumanBody is null)
            {
                return false;
            }
            
            if (dgXRDevice.HumanBody.RightWrist.position.y.ReadValue() <=
                dgXRDevice.HumanBody.HeadTop.position.y.ReadValue())
            {
                return false;
            }
            
            float rightArmAngle = Vector3.Angle(
                dgXRDevice.HumanBody.RightWrist.position.ReadValue() - dgXRDevice.HumanBody.RightElbow.position.ReadValue(),
                dgXRDevice.HumanBody.RightShoulder.position.ReadValue() - dgXRDevice.HumanBody.RightElbow.position.ReadValue());
            if (rightArmAngle < armAngle)
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

