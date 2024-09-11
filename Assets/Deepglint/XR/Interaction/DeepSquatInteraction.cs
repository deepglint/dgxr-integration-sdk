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
        private float _startKneeAngle = 160;
        private float _head = 2f;
        private float _startHeight = 1f;
        private readonly float _heightOffset = 0.01f;
        
        public void Process(ref InputInteractionContext context)
        {
            if (context.control.device is DGXRHumanController dgXRDevice)
            {
                if (IsDeepSquatHappening(dgXRDevice))
                {
                    switch (context.phase)
                    {
                        case InputActionPhase.Waiting:
                            if (GetKneeAngle(dgXRDevice) <= _startKneeAngle)
                            {
                                context.Started();
                                _startHeight = (dgXRDevice.HumanBody.LeftHip.position.y.ReadValue() +
                                                dgXRDevice.HumanBody.RightHip.position.y.ReadValue()) / 2;
                            }
                            break;
                        case InputActionPhase.Started:
                            if (IsDeepSquatActionPerformed(dgXRDevice))
                            {
                                // Debug.Log($"DeepSquat action performed on device {dgXRDevice.deviceId}");
                                InputSystem.QueueDeltaStateEvent(dgXRDevice.SquatRange, GetSquatRange(dgXRDevice));
                                context.PerformedAndStayPerformed();
                            }
                            break;
                        case InputActionPhase.Performed:
                            InputSystem.QueueDeltaStateEvent(dgXRDevice.SquatRange, GetSquatRange(dgXRDevice));
                            break;
                    }
                }
                else
                {
                    if (CheckMissCancel(ref context))
                    {
                        InputSystem.QueueDeltaStateEvent(dgXRDevice.SquatRange, 0f);
                    }
                }
            }
        }

        private bool IsDeepSquatHappening(DGXRHumanController dgXRDevice)
        {
            float height = dgXRDevice.HumanBody.HeadTop.position.y.ReadValue();
            try
            {
                if (dgXRDevice.DeepSquat.ReadValue() <= Confidence)
                {
                    // Debug.Log("Deep-Squat action is happening");
                    return false;
                }
                if (height > _head + _heightOffset)
                {
                    //Debug.Log($"missed by angle: {angle}, {_kneeAngle}, {_angleOffset}");
                    return false;
                }
            }
            finally
            {
                _head = height;
            }
            

            return true;
        }

        private bool IsDeepSquatActionPerformed(DGXRHumanController device)
        {
            float legLength = Vector3.Distance(device.HumanBody.LeftHip.position.ReadValue(),
                device.HumanBody.LeftKnee.position.ReadValue());
            bool heightPerformed = Math.Abs(device.HumanBody.LeftHip.position.y.ReadValue() -
                     device.HumanBody.LeftKnee.position.y.ReadValue()) <= legLength * 0.5f;
            return heightPerformed || device.DeepSquat.ReadValue() >= 0.5f;
        }

        private float GetKneeAngle(DGXRHumanController device)
        {
            Vector3 rightHip = device.HumanBody.RightHip.position.ReadValue();
            Vector3 rightKnee = device.HumanBody.RightKnee.position.ReadValue();
            Vector3 leftHip = device.HumanBody.LeftHip.position.ReadValue();
            Vector3 leftKnee = device.HumanBody.LeftKnee.position.ReadValue();
            float rightAngle = Vector3.Angle(device.HumanBody.RightAnkle.position.ReadValue() - rightKnee,
                rightHip - rightKnee); 
            float leftAngle = Vector3.Angle(device.HumanBody.LeftAnkle.position.ReadValue() - leftKnee,
                leftHip - leftKnee);
            return (rightAngle + leftAngle)/2; 
        }

        private float GetSquatRange(DGXRHumanController device)
        {
            float height = (device.HumanBody.LeftHip.position.y.ReadValue() +
                            device.HumanBody.RightHip.position.y.ReadValue()) / 2;
            float value = Mathf.Clamp((_startHeight - height) * 1.5f / _startHeight, 0f, 1f);
            // Debug.Log($"deep squat: {value}");
            return value;
        }

        public new void Reset()
        {
            base.Reset();
            // Debug.Log("reset Deep-Squat interaction");
        }
    }
}

