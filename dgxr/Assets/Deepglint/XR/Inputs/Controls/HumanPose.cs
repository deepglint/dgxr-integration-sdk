using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using TrackingState = UnityEngine.XR.InputTrackingState;

namespace Deepglint.XR.Inputs.Controls
{
    [StructLayout(LayoutKind.Explicit, Size = m_SizeInBytes)]
    public struct HumanPoseState : IInputStateTypeInfo
    {
        internal const int m_SizeInBytes = 36;
        internal static readonly FourCC m_Format = new FourCC('H', 'P', 'o', 's');

        public FourCC format => m_Format;
        
        [FieldOffset(0), InputControl(displayName = "Is Tracked", layout = "Button", sizeInBits = 8)]
        public bool isTracked;
        
        [FieldOffset(4), InputControl(displayName = "Tracking State", layout = "Integer")]
        public TrackingState trackingState;
        
        [FieldOffset(8), InputControl(displayName = "Position", noisy = true)]
        public Vector3 position;
        
        [FieldOffset(20), InputControl(displayName = "Rotation", noisy = true)]
        public Quaternion rotation;
    }

    [InputControlLayout(stateType = typeof(HumanPoseState))]
    public class HumanPoseControl : InputControl<HumanPoseState>
    {
        public ButtonControl IsTracked { get; private set; }
        public IntegerControl TrackingState { get; private set; }
        public Vector3Control Position { get; private set; }
        public QuaternionControl Rotation { get; private set; }

        protected override void FinishSetup()
        {
            IsTracked = GetChildControl<ButtonControl>("isTracked");
            TrackingState = GetChildControl<IntegerControl>("trackingState");
            Position = GetChildControl<Vector3Control>("position");
            Rotation = GetChildControl<QuaternionControl>("rotation");

            base.FinishSetup();
        }
        
        public override unsafe HumanPoseState ReadUnprocessedValueFromState(void* statePtr)
        {
            return new HumanPoseState()
            {
                isTracked = IsTracked.ReadUnprocessedValueFromStateWithCaching(statePtr) > 0.5f,
                trackingState = (TrackingState)TrackingState.ReadUnprocessedValueFromStateWithCaching(statePtr),
                position = Position.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rotation = Rotation.ReadUnprocessedValueFromStateWithCaching(statePtr),
            };
        }

        public override unsafe void WriteValueIntoState(HumanPoseState humanPose, void* statePtr)
        {
            IsTracked.WriteValueIntoState(humanPose.isTracked, statePtr);
            TrackingState.WriteValueIntoState((uint)humanPose.trackingState, statePtr);
            Position.WriteValueIntoState(humanPose.position, statePtr);
            Rotation.WriteValueIntoState(humanPose.rotation, statePtr);
        }
    }
}