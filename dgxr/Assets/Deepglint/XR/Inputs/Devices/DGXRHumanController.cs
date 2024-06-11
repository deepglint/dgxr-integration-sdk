using System.Collections.Generic;
using System.Linq;
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
    public struct StickAnchor
    {
        public Vector2 Anchor { get; set; }
        public float Radius { get; set; }
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
    
        [InputControl(name = "stick", format = "VC2B", layout = "Stick", displayName = "Main Stick")]
        [InputControl(name = "stick/x", defaultState = 127, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        public byte x;
    
        [InputControl(name = "stick/y", defaultState = 127, format = "BYTE", 
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "stick/up", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=0,clampMax=1")]
        [InputControl(name = "stick/down", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=-1,clampMax=0,invert")]
        [InputControl(name = "stick/left", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=-1,clampMax=0,invert")]
        [InputControl(name = "stick/right", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=0,clampMax=1")]
        public byte y;
        
        [InputControl(usage = "Trigger", layout = "Axis")]
        public float trigger;

        [InputControl(usage = "Grip", layout = "Axis")]
        public float grip;
        
        [InputControl(layout = "Axis")] 
        public float freeSwim;
        
        [InputControl( layout = "Axis")]
        public float butterflySwim;

        [InputControl(layout = "Axis")] 
        public float highKneeRun;
        
        [InputControl( layout = "Axis")]
        public float deepSquat;
        
        [InputControl( layout = "Axis")]
        public float jump;
        
        [InputControl(name = nameof(DGXRHumanController.GripButton), usage = "GripButton", layout = "Button", 
            bit = (uint)DGXRControllerButton.GripButton, alias = "gripPressed")]
        [InputControl(name = nameof(DGXRHumanController.TriggerButton), usage = "TriggerButton", layout = "Button", 
            bit = (uint)DGXRControllerButton.TriggerButton, alias = "triggerPressed")]
        public ushort buttons;
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
        public StickControl Stick { get; private set; }
        public AxisControl Trigger { get; private set; }
        public AxisControl Grip { get; private set; }
        public AxisControl FreeSwim { get; private set; }
        public AxisControl ButterflySwim { get; private set; }
        public AxisControl HighKneeRun { get; private set; }
        public AxisControl DeepSquat { get; private set; }
        public AxisControl Jump { get; private set; }
        public ButtonControl GripButton { get; private set; }
        public ButtonControl TriggerButton { get; private set; }
        
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
            ButterflySwim = GetChildControl<AxisControl>(nameof(ButterflySwim));
            FreeSwim = GetChildControl<AxisControl>(nameof(FreeSwim));
            HighKneeRun = GetChildControl<AxisControl>(nameof(HighKneeRun));
            DeepSquat = GetChildControl<AxisControl>(nameof(DeepSquat));
            Jump = GetChildControl<AxisControl>(nameof(Jump));
            
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
