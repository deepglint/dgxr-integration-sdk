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
    [StructLayout(LayoutKind.Explicit, Size = kSizeInBytes)]
    public struct HumanPoseState : IInputStateTypeInfo
    {
        internal const int kSizeInBytes = 36;
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
        public ButtonControl isTracked { get; private set; }
        public IntegerControl trackingState { get; private set; }
        public Vector3Control position { get; private set; }
        public QuaternionControl rotation { get; private set; }

        protected override void FinishSetup()
        {
            isTracked = GetChildControl<ButtonControl>("isTracked");
            trackingState = GetChildControl<IntegerControl>("trackingState");
            position = GetChildControl<Vector3Control>("position");
            rotation = GetChildControl<QuaternionControl>("rotation");

            base.FinishSetup();
        }
        
        public override unsafe HumanPoseState ReadUnprocessedValueFromState(void* statePtr)
        {
            return new HumanPoseState()
            {
                isTracked = isTracked.ReadUnprocessedValueFromStateWithCaching(statePtr) > 0.5f,
                trackingState = (TrackingState)trackingState.ReadUnprocessedValueFromStateWithCaching(statePtr),
                position = position.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rotation = rotation.ReadUnprocessedValueFromStateWithCaching(statePtr),
            };
        }

        public override unsafe void WriteValueIntoState(HumanPoseState humanPose, void* statePtr)
        {
            isTracked.WriteValueIntoState(humanPose.isTracked, statePtr);
            trackingState.WriteValueIntoState((uint)humanPose.trackingState, statePtr);
            position.WriteValueIntoState(humanPose.position, statePtr);
            rotation.WriteValueIntoState(humanPose.rotation, statePtr);
        }
    }
}