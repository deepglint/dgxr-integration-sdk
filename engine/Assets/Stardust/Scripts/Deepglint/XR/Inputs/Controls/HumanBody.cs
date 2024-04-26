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
    [StructLayout(LayoutKind.Explicit, Size = kSizeInBytes)]
    public struct HumanBodyState : IInputStateTypeInfo
    {
        internal const int kSizeInBytes = 32*26;
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
        public BoneControl headTop { get; private set; }
        public BoneControl nose { get; private set; }
        public BoneControl leftEye { get; private set; }
        public BoneControl rightEye { get; private set; }
        public BoneControl leftEar { get; private set; }
        public BoneControl rightEar { get; private set; }
        public BoneControl leftShoulder { get; private set; }
        public BoneControl rightShoulder { get; private set; }
        public BoneControl leftElbow { get; private set; }
        public BoneControl rightElbow { get; private set; }
        public BoneControl leftWrist { get; private set; }
        public BoneControl rightWrist { get; private set; }
        public BoneControl leftHip { get; private set; }
        public BoneControl rightHip { get; private set; }
        public BoneControl leftKnee { get; private set; }
        public BoneControl rightKnee { get; private set; }
        public BoneControl leftAnkle { get; private set; }
        public BoneControl rightAnkle { get; private set; }
        public BoneControl leftTiptoe { get; private set; }
        public BoneControl rightTiptoe { get; private set; }
        public BoneControl leftHeel { get; private set; }
        public BoneControl rightHeel { get; private set; }
        public BoneControl leftHand { get; private set; }
        public BoneControl rightHand { get; private set; }
        public BoneControl leftFoot { get; private set; }
        public BoneControl rightFoot { get; private set; }
        
        protected override void FinishSetup()
        {
            headTop = GetChildControl<BoneControl>(nameof(headTop));
            nose = GetChildControl<BoneControl>(nameof(nose));
            leftEye = GetChildControl<BoneControl>(nameof(leftEye));
            rightEye = GetChildControl<BoneControl>(nameof(rightEye));
            leftEar = GetChildControl<BoneControl>(nameof(leftEar));
            rightEar = GetChildControl<BoneControl>(nameof(rightEar));
            leftShoulder = GetChildControl<BoneControl>(nameof(leftShoulder));
            rightShoulder = GetChildControl<BoneControl>(nameof(rightShoulder));
            leftElbow = GetChildControl<BoneControl>(nameof(leftElbow));
            rightElbow = GetChildControl<BoneControl>(nameof(rightElbow));
            leftWrist = GetChildControl<BoneControl>(nameof(leftWrist));
            rightWrist = GetChildControl<BoneControl>(nameof(rightWrist));
            leftHip = GetChildControl<BoneControl>(nameof(leftHip));
            rightHip = GetChildControl<BoneControl>(nameof(rightHip));
            leftKnee = GetChildControl<BoneControl>(nameof(leftKnee));
            rightKnee = GetChildControl<BoneControl>(nameof(rightKnee));
            leftAnkle = GetChildControl<BoneControl>(nameof(leftAnkle));
            rightAnkle = GetChildControl<BoneControl>(nameof(rightAnkle));
            leftTiptoe = GetChildControl<BoneControl>(nameof(leftTiptoe));
            rightTiptoe = GetChildControl<BoneControl>(nameof(rightTiptoe));
            leftHeel = GetChildControl<BoneControl>(nameof(leftHeel));
            rightHeel = GetChildControl<BoneControl>(nameof(rightHeel));
            leftHand = GetChildControl<BoneControl>(nameof(leftHand));
            rightHand = GetChildControl<BoneControl>(nameof(rightHand));
            leftFoot = GetChildControl<BoneControl>(nameof(leftFoot));
            rightFoot = GetChildControl<BoneControl>(nameof(rightFoot));

            base.FinishSetup();
        }

        public override unsafe HumanBodyState ReadUnprocessedValueFromState(void* statePtr)
        {
            return new HumanBodyState()
            {
                headTop = headTop.ReadUnprocessedValueFromStateWithCaching(statePtr),
                nose = nose.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftEye = leftEye.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightEye = rightEye.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftEar = leftEar.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightEar = rightEar.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftShoulder = leftShoulder.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightShoulder = rightShoulder.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftElbow = leftElbow.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightElbow = rightElbow.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftWrist = leftWrist.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightWrist = rightWrist.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHip = leftHip.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHip = rightHip.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftKnee = leftKnee.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightKnee = rightKnee.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftAnkle = leftAnkle.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightAnkle = rightAnkle.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftTiptoe = leftTiptoe.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightTiptoe = rightTiptoe.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHeel = leftHeel.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHeel = rightHeel.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftHand = leftHand.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightHand = rightHand.ReadUnprocessedValueFromStateWithCaching(statePtr),
                leftFoot = leftFoot.ReadUnprocessedValueFromStateWithCaching(statePtr),
                rightFoot = rightFoot.ReadUnprocessedValueFromStateWithCaching(statePtr),
            };
        }
        
        public override unsafe void WriteValueIntoState(HumanBodyState humanBody, void* statePtr)
        {
            headTop.WriteValueIntoState(humanBody.headTop, statePtr);
            nose.WriteValueIntoState(humanBody.nose, statePtr);
            leftEye.WriteValueIntoState(humanBody.leftEye, statePtr);
            rightEye.WriteValueIntoState(humanBody.rightEye, statePtr);
            leftEar.WriteValueIntoState(humanBody.leftEar, statePtr);
            rightEar.WriteValueIntoState(humanBody.rightEar, statePtr);
            leftShoulder.WriteValueIntoState(humanBody.leftShoulder, statePtr);
            rightShoulder.WriteValueIntoState(humanBody.rightShoulder, statePtr);
            leftElbow.WriteValueIntoState(humanBody.leftElbow, statePtr);
            rightElbow.WriteValueIntoState(humanBody.rightElbow, statePtr);
            leftWrist.WriteValueIntoState(humanBody.leftWrist, statePtr);
            rightWrist.WriteValueIntoState(humanBody.rightWrist, statePtr);
            leftHip.WriteValueIntoState(humanBody.leftHip, statePtr);
            rightHip.WriteValueIntoState(humanBody.rightHip, statePtr);
            leftKnee.WriteValueIntoState(humanBody.leftKnee, statePtr);
            rightKnee.WriteValueIntoState(humanBody.rightKnee, statePtr);
            leftAnkle.WriteValueIntoState(humanBody.leftAnkle, statePtr);
            rightAnkle.WriteValueIntoState(humanBody.rightAnkle, statePtr);
            leftTiptoe.WriteValueIntoState(humanBody.leftTiptoe, statePtr);
            rightTiptoe.WriteValueIntoState(humanBody.rightTiptoe, statePtr);
            leftHeel.WriteValueIntoState(humanBody.leftHeel, statePtr);
            rightHeel.WriteValueIntoState(humanBody.rightHeel, statePtr);
            leftHand.WriteValueIntoState(humanBody.leftHand, statePtr);
            rightHand.WriteValueIntoState(humanBody.rightHand, statePtr);
            leftFoot.WriteValueIntoState(humanBody.leftFoot, statePtr);
            rightFoot.WriteValueIntoState(humanBody.rightFoot, statePtr);
        }
    }
}
