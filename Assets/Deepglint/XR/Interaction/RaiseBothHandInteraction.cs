using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
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
                if (IsRaiseBothHandHappening(dgXRDevice, RequiredArmAngle))
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
                            // Debug.Log($"RaiseBothHand action performed on device {dgXRDevice.deviceId}");
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
                            //Debug.Log("cancel raise both hand on device" + device.deviceId);
                            context.Canceled(); 
                            _phases[dgXRDevice.deviceId] = InputActionPhase.Waiting;
                        }
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
