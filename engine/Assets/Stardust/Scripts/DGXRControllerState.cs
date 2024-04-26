// using System.Runtime.InteropServices;
// using UnityEngine;
// using UnityEngine.InputSystem.Layouts;
// using UnityEngine.InputSystem.LowLevel;
// using UnityEngine.InputSystem.Utilities;
// using UnityEngine.XR;
//
// public enum DGXRControllerButton
// {
//     PrimaryButton,
//     GripButton,
//     TriggerButton,
// }
//
// [StructLayout(LayoutKind.Explicit, Size = 64)]
// public struct DGXRControllerState : IInputStateTypeInfo
// {
//     public FourCC format => new FourCC('D', 'G', 'X', 'R');
//
//     // public FourCC format => formatId;
//     
//     // [InputControl(usage = "Head", layout = "Bone", offset = 0)]
//     // [FieldOffset(0)]
//     // public Bone head;
//     
//     [InputControl(usage = "Trigger", layout = "Axis", offset = 0)]
//     [FieldOffset(0)]
//     public float trigger;
//
//     /// <summary>
//     /// Represents the user's grip on the controller.
//     /// </summary>
//     [InputControl(usage = "Grip", layout = "Axis", offset = 4)]
//     [FieldOffset(4)]
//     public float grip;
//     
//     [InputControl(name = nameof(DGXRController.primaryButton), usage = "PrimaryButton", layout = "Button", bit = (uint)DGXRControllerButton.PrimaryButton, offset = 8)]
//     [InputControl(name = nameof(DGXRController.gripButton), usage = "GripButton", layout = "Button", bit = (uint)DGXRControllerButton.GripButton, offset = 8, alias = "gripPressed")]
//     [InputControl(name = nameof(DGXRController.triggerButton), usage = "TriggerButton", layout = "Button", bit = (uint)DGXRControllerButton.TriggerButton, offset = 8, alias = "triggerPressed")]
//     [FieldOffset(8)]
//     public ushort buttons;
//     
//     [InputControl(name = "stick", format = "VC2B", layout = "Stick", displayName = "Main Stick")]
//     [InputControl(name = "stick/x", defaultState = 127, format = "BYTE",
//         offset = 0,
//         parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
//     [FieldOffset(10)]
//     public byte x;
//     [InputControl(name = "stick/y", defaultState = 127, format = "BYTE",
//         offset = 1,
//         parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
//     [InputControl(name = "stick/up", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=0,clampMax=1")]
//     [InputControl(name = "stick/down", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=-1,clampMax=0,invert")]
//     [InputControl(name = "stick/left", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=-1,clampMax=0,invert")]
//     [InputControl(name = "stick/right", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=2,clampMin=0,clampMax=1")]
//     [FieldOffset(11)]
//     public byte y;
//
//     public DGXRControllerState WithButton(DGXRControllerButton button, bool state = true)
//     {
//         var bit = 1 << (int)button;
//         if (state)
//         {
//             buttons |= (ushort)bit;
//         }
//         else
//         {
//             buttons &= (ushort)~bit;
//         }
//
//         return this;
//     }
//
//     public bool HasButton(DGXRControllerButton button)
//     {
//         var bit = 1 << (int)button;
//         return (buttons & bit) != 0;
//     }
//
//     public void Reset()
//     {
//         trigger = default;
//         grip = default;
//         buttons = default;
//     }
// }
