using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Deepglint.XR.Inputs.Controls
{
    [Preserve]
    [StructLayout(LayoutKind.Explicit, Size = m_SizeInBytes)]
    public struct HumanBodyState : IInputStateTypeInfo
    {
        internal const int m_SizeInBytes = 32*26;
        internal static readonly FourCC m_Format = new FourCC('H', 'B', 'o', 'd');

        public FourCC format => m_Format;
        
        [FieldOffset(0), InputControl(layout = "Bone", noisy = true)]
        public Bone headTop;
        
        [FieldOffset(32), InputControl(layout = "Bone", noisy = true)]
        public Bone nose;
        
        [FieldOffset(32*2), InputControl(layout = "Bone", noisy = true)]
        public Bone leftEye;
        
        [FieldOffset(32*3), InputControl(layout = "Bone", noisy = true)]
        public Bone rightEye;
        
        [FieldOffset(32*4), InputControl(layout = "Bone", noisy = true)]
        public Bone leftEar;
        
        [FieldOffset(32*5), InputControl(layout = "Bone", noisy = true)]
        public Bone rightEar;
        
        [FieldOffset(32*6), InputControl(layout = "Bone", noisy = true)]
        public Bone leftShoulder;
        
        [FieldOffset(32*7), InputControl(layout = "Bone", noisy = true)]
        public Bone rightShoulder;
        
        [FieldOffset(32*8), InputControl(layout = "Bone", noisy = true)]
        public Bone leftElbow;
        
        [FieldOffset(32*9), InputControl(layout = "Bone", noisy = true)]
        public Bone rightElbow;
        
        [FieldOffset(32*10), InputControl(layout = "Bone", noisy = true)]
        public Bone leftWrist;
        
        [FieldOffset(32*11), InputControl(layout = "Bone", noisy = true)]
        public Bone rightWrist;
        
        [FieldOffset(32*12), InputControl(layout = "Bone", noisy = true)]
        public Bone leftHip;
        
        [FieldOffset(32*13), InputControl(layout = "Bone", noisy = true)]
        public Bone rightHip;
        
        [FieldOffset(32*14), InputControl(layout = "Bone", noisy = true)]
        public Bone leftKnee;
        
        [FieldOffset(32*15), InputControl(layout = "Bone", noisy = true)]
        public Bone rightKnee;
        
        [FieldOffset(32*16), InputControl(layout = "Bone", noisy = true)]
        public Bone leftAnkle;
        
        [FieldOffset(32*17), InputControl(layout = "Bone", noisy = true)]
        public Bone rightAnkle;
        
        [FieldOffset(32*18), InputControl(layout = "Bone", noisy = true)]
        public Bone leftTiptoe;
        
        [FieldOffset(32*19), InputControl(layout = "Bone", noisy = true)]
        public Bone rightTiptoe;
        
        [FieldOffset(32*20), InputControl(layout = "Bone", noisy = true)]
        public Bone leftHeel;
        
        [FieldOffset(32*21), InputControl(layout = "Bone", noisy = true)]
        public Bone rightHeel;
        
        [FieldOffset(32*22), InputControl(layout = "Bone", noisy = true)]
        public Bone leftHand;
        
        [FieldOffset(32*23), InputControl(layout = "Bone", noisy = true)]
        public Bone rightHand;
        
        [FieldOffset(32*24), InputControl(layout = "Bone", noisy = true)]
        public Bone leftFoot;
        
        [FieldOffset(32*25), InputControl(layout = "Bone", noisy = true)]
        public Bone rightFoot;

        public ReadOnlySpan<Bone> GetBones()
        {
            return new ReadOnlySpan<Bone>(new[]
            {
                headTop,
                nose,
                leftEye,
                rightEye,
                leftEar,
                rightEar,
                leftShoulder,
                rightShoulder,
                leftElbow,
                rightElbow,
                leftWrist,
                rightWrist,
                leftHip,
                rightHip,
                leftKnee,
                rightKnee,
                leftAnkle,
                rightAnkle,
                leftTiptoe,
                rightTiptoe,
                leftHeel,
                rightHeel,
                leftHand,
                rightHand,
                leftFoot,
                rightFoot
            });
        }
    }

    [InputControlLayout(stateType = typeof(HumanBodyState))]
    public class HumanBodyControl : InputControl<HumanBodyState>
    {
        public BoneControl HeadTop { get; private set; }
        public BoneControl Nose { get; private set; }
        public BoneControl LeftEye { get; private set; }
        public BoneControl RightEye { get; private set; }
        public BoneControl LeftEar { get; private set; }
        public BoneControl RightEar { get; private set; }
        public BoneControl LeftShoulder { get; private set; }
        public BoneControl RightShoulder { get; private set; }
        public BoneControl LeftElbow { get; private set; }
        public BoneControl RightElbow { get; private set; }
        public BoneControl LeftWrist { get; private set; }
        public BoneControl RightWrist { get; private set; }
        public BoneControl LeftHip { get; private set; }
        public BoneControl RightHip { get; private set; }
        public BoneControl LeftKnee { get; private set; }
        public BoneControl RightKnee { get; private set; }
        public BoneControl LeftAnkle { get; private set; }
        public BoneControl RightAnkle { get; private set; }
        public BoneControl LeftTiptoe { get; private set; }
        public BoneControl RightTiptoe { get; private set; }
        public BoneControl LeftHeel { get; private set; }
        public BoneControl RightHeel { get; private set; }
        public BoneControl LeftHand { get; private set; }
        public BoneControl RightHand { get; private set; }
        public BoneControl LeftFoot { get; private set; }
        public BoneControl RightFoot { get; private set; }
        
        protected override void FinishSetup()
        {
            HeadTop = GetChildControl<BoneControl>(nameof(HeadTop));
            Nose = GetChildControl<BoneControl>(nameof(Nose));
            LeftEye = GetChildControl<BoneControl>(nameof(LeftEye));
            RightEye = GetChildControl<BoneControl>(nameof(RightEye));
            LeftEar = GetChildControl<BoneControl>(nameof(LeftEar));
            RightEar = GetChildControl<BoneControl>(nameof(RightEar));
            LeftShoulder = GetChildControl<BoneControl>(nameof(LeftShoulder));
            RightShoulder = GetChildControl<BoneControl>(nameof(RightShoulder));
            LeftElbow = GetChildControl<BoneControl>(nameof(LeftElbow));
            RightElbow = GetChildControl<BoneControl>(nameof(RightElbow));
            LeftWrist = GetChildControl<BoneControl>(nameof(LeftWrist));
            RightWrist = GetChildControl<BoneControl>(nameof(RightWrist));
            LeftHip = GetChildControl<BoneControl>(nameof(LeftHip));
            RightHip = GetChildControl<BoneControl>(nameof(RightHip));
            LeftKnee = GetChildControl<BoneControl>(nameof(LeftKnee));
            RightKnee = GetChildControl<BoneControl>(nameof(RightKnee));
            LeftAnkle = GetChildControl<BoneControl>(nameof(LeftAnkle));
            RightAnkle = GetChildControl<BoneControl>(nameof(RightAnkle));
            LeftTiptoe = GetChildControl<BoneControl>(nameof(LeftTiptoe));
            RightTiptoe = GetChildControl<BoneControl>(nameof(RightTiptoe));
            LeftHeel = GetChildControl<BoneControl>(nameof(LeftHeel));
            RightHeel = GetChildControl<BoneControl>(nameof(RightHeel));
            LeftHand = GetChildControl<BoneControl>(nameof(LeftHand));
            RightHand = GetChildControl<BoneControl>(nameof(RightHand));
            LeftFoot = GetChildControl<BoneControl>(nameof(LeftFoot));
            RightFoot = GetChildControl<BoneControl>(nameof(RightFoot));

            base.FinishSetup();
        }

        public override unsafe HumanBodyState ReadUnprocessedValueFromState(void* statePtr)
        {
            return new HumanBodyState()
            {
                headTop = HeadTop.ReadUnprocessedValueFromStateWithCaching(statePtr),
                nose = Nose.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftEye = LeftEye.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightEye = RightEye.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftEar = LeftEar.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightEar = RightEar.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftShoulder = LeftShoulder.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightShoulder = RightShoulder.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftElbow = LeftElbow.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightElbow = RightElbow.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftWrist = LeftWrist.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightWrist = RightWrist.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHip = LeftHip.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHip = RightHip.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftKnee = LeftKnee.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightKnee = RightKnee.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftAnkle = LeftAnkle.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightAnkle = RightAnkle.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftTiptoe = LeftTiptoe.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightTiptoe = RightTiptoe.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHeel = LeftHeel.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHeel = RightHeel.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHand = LeftHand.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHand = RightHand.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftFoot = LeftFoot.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightFoot = RightFoot.ReadUnprocessedValueFromStateWithCaching(statePtr),
            };
        }
        
        public override unsafe void WriteValueIntoState(HumanBodyState humanBody, void* statePtr)
        {
            HeadTop.WriteValueIntoState(humanBody.headTop, statePtr);
            Nose.WriteValueIntoState(humanBody.nose, statePtr);
            LeftEye.WriteValueIntoState(humanBody.leftEye, statePtr);
            RightEye.WriteValueIntoState(humanBody.rightEye, statePtr);
            LeftEar.WriteValueIntoState(humanBody.leftEar, statePtr);
            RightEar.WriteValueIntoState(humanBody.rightEar, statePtr);
            LeftShoulder.WriteValueIntoState(humanBody.leftShoulder, statePtr);
            RightShoulder.WriteValueIntoState(humanBody.rightShoulder, statePtr);
            LeftElbow.WriteValueIntoState(humanBody.leftElbow, statePtr);
            RightElbow.WriteValueIntoState(humanBody.rightElbow, statePtr);
            LeftWrist.WriteValueIntoState(humanBody.leftWrist, statePtr);
            RightWrist.WriteValueIntoState(humanBody.rightWrist, statePtr);
            LeftHip.WriteValueIntoState(humanBody.leftHip, statePtr);
            RightHip.WriteValueIntoState(humanBody.rightHip, statePtr);
            LeftKnee.WriteValueIntoState(humanBody.leftKnee, statePtr);
            RightKnee.WriteValueIntoState(humanBody.rightKnee, statePtr);
            LeftAnkle.WriteValueIntoState(humanBody.leftAnkle, statePtr);
            RightAnkle.WriteValueIntoState(humanBody.rightAnkle, statePtr);
            LeftTiptoe.WriteValueIntoState(humanBody.leftTiptoe, statePtr);
            RightTiptoe.WriteValueIntoState(humanBody.rightTiptoe, statePtr);
            LeftHeel.WriteValueIntoState(humanBody.leftHeel, statePtr);
            RightHeel.WriteValueIntoState(humanBody.rightHeel, statePtr);
            LeftHand.WriteValueIntoState(humanBody.leftHand, statePtr);
            RightHand.WriteValueIntoState(humanBody.rightHand, statePtr);
            LeftFoot.WriteValueIntoState(humanBody.leftFoot, statePtr);
            RightFoot.WriteValueIntoState(humanBody.rightFoot, statePtr);
        }
    }
}
