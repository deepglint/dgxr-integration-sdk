using System.Collections.Generic;
using System.Runtime.InteropServices;
using Deepglint.XR.Inputs.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace Deepglint.XR.Inputs.Devices
{
    public class StickAnchor
    {
        /// <summary>
        /// world position
        /// </summary>
        public Vector3 Point { get; set; }
        public float Radius { get; set; }

        public StickAnchor(Vector3 point, float radius)
        {
            Point = point;
            Radius = radius;
        }
    }
    
    public enum DGXRControllerButton
    {
        TriggerButton,
        GripButton,
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DGXRHumanControllerState : IInputStateTypeInfo
    {
        internal static readonly FourCC m_Format = new FourCC('D', 'G', 'X', 'R');

        public FourCC format => m_Format;
        
        [InputControl(name = nameof(DGXRHumanController.HumanPose), layout = "HumanPose")]
        public HumanPoseState humanPose;
        
        [InputControl(name = nameof(DGXRHumanController.HumanBody), layout = "HumanBody")]
        public HumanBodyState humanBody;
        
        [InputControl(name = nameof(DGXRHumanController.Stick), layout = "Stick")]
        public Vector2 stick; 
    
        [InputControl(usage = "Trigger", layout = "Axis")]
        public float trigger;

        [InputControl(usage = "Grip", layout = "Axis")]
        public float grip;

        [InputControl(name = nameof(DGXRHumanController.GripButton), usage = "GripButton", layout = "Button", 
            bit = (uint)DGXRControllerButton.GripButton, alias = "gripPressed")]
        [InputControl(name = nameof(DGXRHumanController.TriggerButton), usage = "TriggerButton", layout = "Button", 
            bit = (uint)DGXRControllerButton.TriggerButton, alias = "triggerPressed")]
        public ushort buttons;
        
        // 算法
        [InputControl(layout = "Axis")] public float rightHandDrawCircle;
        [InputControl(layout = "Axis")] public float leftHandDrawCircle;
        [InputControl(layout = "Axis")] public float handBevelCut;
        [InputControl(layout = "Axis")] public float handParry;
        [InputControl(layout = "Axis")] public float handStraightCut;
        [InputControl(layout = "Axis")] public float handTransversal;
        [InputControl(layout = "Axis")] public float straightPunch;
        [InputControl(layout = "Axis")] public float readyStraightPunch;
        [InputControl(layout = "Axis")] public float uppercut;
        [InputControl(layout = "Axis")] public float kick;
        [InputControl(layout = "Axis")] public float throwOneHandInFists;
        [InputControl(layout = "Axis")] public float readyThrowOneHandInFists;
        [InputControl(layout = "Axis")] public float readyThrowBothHandInFists;
        [InputControl(layout = "Axis")] public float combineHandsStraight;
        [InputControl(layout = "Axis")] public float slowRun;
        [InputControl(layout = "Axis")] public float highKneeRun;
        [InputControl(layout = "Axis")] public float butterflySwim;
        [InputControl(layout = "Axis")] public float freeSwim;
        [InputControl(layout = "Axis")] public float keepRaisingHand;
        [InputControl(layout = "Axis")] public float cheerUp;
        [InputControl(layout = "Axis")] public float jump;
        [InputControl(layout = "Axis")] public float deepSquat;
        [InputControl(layout = "Axis")] public float armFlat;
        [InputControl(layout = "Axis")] public float armFlatIsL;
        [InputControl(layout = "Axis")] public float armVerticalIsL;
       
        // 扩展
        [InputControl( layout = "Axis")]
        public float squatRange;
        
        [InputControl( layout = "Axis")]
        public float jumpRange;
        
        
        [InputControl( layout = "Axis")]
        public float SlideRightArmToLeftRange;
        
        [InputControl( layout = "Axis")]
        public float SlideLeftArmToRightRange;
    }
    
    [Preserve]
    [InputControlLayout(
            stateType = typeof(DGXRHumanControllerState), 
            isGenericTypeOfDevice = false, 
            displayName = "DGXR Controller", 
            updateBeforeRender = true
        )
    ]
    public class DGXRHumanController : InputDevice
    {
        public HumanPoseControl HumanPose { get; private set; }
        public HumanBodyControl HumanBody { get; private set; }
        /// <summary>
        /// 当anchor 不为null 时stick的值有效(使用前需要先给设备的Anchor赋值)，stick 的取值为humanPose所在的位置相对与Anchor坐标在X，Z平面上的方向向量；
        /// </summary>
        public StickControl Stick { get; private set; }
        public AxisControl Trigger { get; private set; }
        public AxisControl Grip { get; private set; }
        public ButtonControl GripButton { get; private set; }
        public ButtonControl TriggerButton { get; private set; }
        // 算法动作
        public AxisControl RightHandDrawCircle { get; private set; }
        public AxisControl LeftHandDrawCircle { get; private set; }
        public AxisControl HandBevelCut { get; private set; }
        public AxisControl HandParry { get; private set; }
        public AxisControl HandStraightCut { get; private set; }
        public AxisControl HandTransversal { get; private set; }
        public AxisControl StraightPunch { get; private set; }
        public AxisControl ReadyStraightPunch { get; private set; }
        public AxisControl Uppercut { get; private set; }
        public AxisControl Kick { get; private set; }
        public AxisControl ThrowOneHandInFists { get; private set; }
        public AxisControl ReadyThrowOneHandInFists { get; private set; }
        public AxisControl ReadyThrowBothHandInFists { get; private set; }
        public AxisControl CombineHandsStraight { get; private set; }
        public AxisControl SlowRun { get; private set; }
        public AxisControl HighKneeRun { get; private set; }
        public AxisControl ButterflySwim { get; private set; }
        public AxisControl FreeSwim { get; private set; }
        public AxisControl KeepRaisingHand { get; private set; }
        public AxisControl CheerUp { get; private set; }
        public AxisControl Jump { get; private set; }
        public AxisControl DeepSquat { get; private set; }
        public AxisControl ArmFlat { get; private set; }
        public AxisControl ArmFlatIsL { get; private set; }
        public AxisControl ArmVerticalIsL { get; private set; } 
        
        // 扩展动作
        public AxisControl SquatRange { get; private set; }
        public AxisControl JumpRange { get; private set; } 
        public AxisControl SlideRightArmToLeftRange { get; private set; }
        public AxisControl SlideLeftArmToRightRange { get; private set; }

        /// <summary>
        /// stick 锚定的世界坐标
        /// </summary>
        public StickAnchor Anchor { get; set; }

        protected override void FinishSetup()
        {
            HumanPose = GetChildControl<HumanPoseControl>(nameof(HumanPose));
            HumanBody = GetChildControl<HumanBodyControl>(nameof(HumanBody));
            Stick = GetChildControl<StickControl>(nameof(Stick));
            Trigger = GetChildControl<AxisControl>(nameof(Trigger));
            Grip = GetChildControl<AxisControl>(nameof(Grip));
            TriggerButton = GetChildControl<ButtonControl>(nameof(TriggerButton));
            GripButton = GetChildControl<ButtonControl>(nameof(GripButton));
            // 算法动作
            RightHandDrawCircle= GetChildControl<AxisControl>(nameof(RightHandDrawCircle));
            LeftHandDrawCircle= GetChildControl<AxisControl>(nameof(LeftHandDrawCircle));
            HandBevelCut= GetChildControl<AxisControl>(nameof(HandBevelCut));
            HandParry= GetChildControl<AxisControl>(nameof(HandParry));
            HandStraightCut= GetChildControl<AxisControl>(nameof(HandStraightCut));
            HandTransversal= GetChildControl<AxisControl>(nameof(HandTransversal));
            StraightPunch= GetChildControl<AxisControl>(nameof(StraightPunch));
            ReadyStraightPunch= GetChildControl<AxisControl>(nameof(ReadyStraightPunch));
            Uppercut= GetChildControl<AxisControl>(nameof(Uppercut));
            Kick= GetChildControl<AxisControl>(nameof(Kick));
            ThrowOneHandInFists= GetChildControl<AxisControl>(nameof(ThrowOneHandInFists));
            ReadyThrowOneHandInFists= GetChildControl<AxisControl>(nameof(ReadyThrowOneHandInFists));
            ReadyThrowBothHandInFists= GetChildControl<AxisControl>(nameof(ReadyThrowBothHandInFists));
            CombineHandsStraight= GetChildControl<AxisControl>(nameof(CombineHandsStraight));
            SlowRun= GetChildControl<AxisControl>(nameof(SlowRun));
            HighKneeRun= GetChildControl<AxisControl>(nameof(HighKneeRun));
            ButterflySwim= GetChildControl<AxisControl>(nameof(ButterflySwim));
            FreeSwim= GetChildControl<AxisControl>(nameof(FreeSwim));
            KeepRaisingHand= GetChildControl<AxisControl>(nameof(KeepRaisingHand));
            CheerUp= GetChildControl<AxisControl>(nameof(CheerUp));
            Jump= GetChildControl<AxisControl>(nameof(Jump));
            DeepSquat= GetChildControl<AxisControl>(nameof(DeepSquat));
            ArmFlat= GetChildControl<AxisControl>(nameof(ArmFlat));
            ArmFlatIsL= GetChildControl<AxisControl>(nameof(ArmFlatIsL));
            ArmVerticalIsL= GetChildControl<AxisControl>(nameof(ArmVerticalIsL));
            
            // 扩展动作
            SquatRange = GetChildControl<AxisControl>(nameof(SquatRange));
            JumpRange = GetChildControl<AxisControl>(nameof(JumpRange));
            SlideRightArmToLeftRange = GetChildControl<AxisControl>(nameof(SlideRightArmToLeftRange));
            SlideLeftArmToRightRange = GetChildControl<AxisControl>(nameof(SlideLeftArmToRightRange));
            
            base.FinishSetup();
        }
        
        

        public GameObject GetPairedPlayer()
        {
            PlayerInput pi = PlayerInput.FindFirstPairedToDevice(this);
            if (pi != null)
            {
                return pi.gameObject;
            }
            return null;
        }

        public ReadOnlyArray<GameObject> GetAllPairedPlayers()
        {
            List<GameObject> players = new List<GameObject>();
            var pis = PlayerInput.all;
            for (var i = 0; i < pis.Count; i++)
            {
                if (ReadOnlyArrayExtensions.ContainsReference(pis[i].devices, this))
                {
                    players.Add(pis[i].gameObject);
                }
            }

            return new ReadOnlyArray<GameObject>(players.ToArray());
        }
    } 
}
