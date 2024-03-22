using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace DGXR
{
    public class DGXRDevice : MonoBehaviour
    {
        [Flags]
        internal enum TargetedDevices
        {
            /// <summary>
            /// No target device to update.
            /// </summary>
            None = 0,
            
            /// <summary>
            /// No target device, behaving as an FPS controller.
            /// </summary>
            FPS = 1 << 0,

            /// <summary>
            /// Update human controller or human position and rotation.
            /// </summary>
            Human = 1 << 1,
        }
        
        public enum DeviceMode
        {
            /// <summary>
            /// Motion controller mode.
            /// </summary>
            Controller,

            /// <summary>
            /// Tracked human pose mode.
            /// </summary>
            HumanPose,
        }

        [Flags]
        public enum Axis2DTargets
        {
            /// <summary>
            /// Do not update device state from input.
            /// </summary>
            None = 0,

            /// <summary>
            /// Update device position from input.
            /// </summary>
            Position = 1 << 0,

            /// <summary>
            /// Update the primary touchpad or joystick on a controller device from input.
            /// </summary>
            Primary2DAxis = 1 << 1,
        }
        
        // [SerializeField]
        // [Tooltip("The Input System Action used to control the Grip control of the manipulated controller device(s). Must be a Button Control.")]
        // InputActionReference m_GripAction;
        //
        // public InputActionReference gripAction
        // {
        //     get => m_GripAction;
        //     set
        //     {
        //         UnsubscribeGripAction();
        //         m_GripAction = value;
        //         SubscribeGripAction();
        //     }
        // }
        
        // [SerializeField]
        // [Tooltip("The Input System Action used to control the Trigger control of the manipulated controller device(s). Must be a Button Control.")]
        // InputActionReference m_TriggerAction;
        //
        // public InputActionReference triggerAction
        // {
        //     get => m_TriggerAction;
        //     set
        //     {
        //         UnsubscribeTriggerAction();
        //         m_TriggerAction = value;
        //         SubscribeTriggerAction();
        //     }
        // }
        
        // [SerializeField]
        // [Tooltip("The Input System Action used to control the PrimaryButton control of the manipulated controller device(s). Must be a Button Control.")]
        // InputActionReference m_PrimaryButtonAction;
        //
        // public InputActionReference primaryButtonAction
        // {
        //     get => m_PrimaryButtonAction;
        //     set
        //     {
        //         UnsubscribePrimaryButtonAction();
        //         m_PrimaryButtonAction = value;
        //         SubscribePrimaryButtonAction();
        //     }
        // }
        
        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of the simulated grip on the controller when the Grip control is pressed.")]
        float m_GripAmount = 1f;
    
        public float gripAmount
        {
            get => m_GripAmount;
            set => m_GripAmount = value;
        }
        
        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of the simulated trigger pull on the controller when the Trigger control is pressed.")]
        float m_TriggerAmount = 1f;
        
        public float triggerAmount
        {
            get => m_TriggerAmount;
            set => m_TriggerAmount = value;
        }
        
        [SerializeField]
        [Tooltip("Whether the dg-xr controller should report the pose as fully tracked or unavailable/inferred.")]
        bool m_ControllerIsTracked = true;
        
        public bool controllerIsTracked
        {
            get => m_ControllerIsTracked;
            set => m_ControllerIsTracked = value;
        }
        
        // [SerializeField]
        // [Tooltip("Which tracking values the left-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
        // InputTrackingState m_ControllerTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;
        //
        // public InputTrackingState controllerTrackingState
        // {
        //     get => m_ControllerTrackingState;
        //     set => m_ControllerTrackingState = value;
        // }
        
        // public Axis2DTargets axis2DTargets { get; set; } = Axis2DTargets.Primary2DAxis;
        //
        // public static DGXRDevice instance { get; private set; }
        //
        // TargetedDevices targetedDeviceInput
        // {
        //     get => m_TargetedDeviceInput;
        //     set => m_TargetedDeviceInput = value;
        // }
        
        DeviceMode m_DeviceMode = DeviceMode.Controller;
        
        bool m_DeviceModeDirty;
        bool m_StartedDeviceModeChange;
        bool m_ResetInput;
        Vector2 m_Axis2DInput;
        Vector2 m_RestingHandAxis2DInput;
        
        bool m_GripInput;
        bool m_TriggerInput;
        bool m_PrimaryButtonInput;
        bool m_Primary2DAxisClickInput;

        DGXRControllerState m_ControllerState;
        DGXRController m_ControllerDevice;
        bool m_OnInputDeviceChangeSubscribed;

        protected void Awake()
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<DGXRController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(".*")
            );
        }
    }
}