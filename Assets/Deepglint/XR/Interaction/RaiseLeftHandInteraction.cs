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
        public int RequiredHits = 6; 
        
        /// <summary>
        /// required arm angle to perform this action
        /// </summary>
        public float RequiredArmAngle = 120f;
        
        private Dictionary<int, int> _hitDictionary = new Dictionary<int, int>();
        private Dictionary<int, int> _missDictionary = new Dictionary<int, int>();
        private Dictionary<int, InputActionPhase> _phases = new Dictionary<int, InputActionPhase>();

        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (!_phases.ContainsKey(dgXRDevice.deviceId))
                {
                    _phases[dgXRDevice.deviceId] = context.phase;
                }
                if (IsRaiseLeftHandHappening(dgXRDevice, RequiredArmAngle))
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
                    
                    if (_phases[dgXRDevice.deviceId] == InputActionPhase.Waiting)
                    {
                        context.Started();
                        _phases[dgXRDevice.deviceId] = InputActionPhase.Started;
                    } else if (_phases[dgXRDevice.deviceId] == InputActionPhase.Started)
                    {
                        if (_hitDictionary[dgXRDevice.deviceId] >= RequiredHits)
                        {
                            context.PerformedAndStayPerformed();
                            _phases[dgXRDevice.deviceId] = InputActionPhase.Performed;
                            // Debug.Log($"RaiseLeftHand performed on device {dgXRDevice.deviceId}");
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
                    if (_phases[dgXRDevice.deviceId] == InputActionPhase.Performed)
                    {
                        if (_missDictionary[dgXRDevice.deviceId] >= RequiredHits)
                        {
                            //Debug.Log("cancel raise right hand on device" + device.deviceId);
                            context.Canceled();
                            _phases[dgXRDevice.deviceId] = InputActionPhase.Waiting;
                        }
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

