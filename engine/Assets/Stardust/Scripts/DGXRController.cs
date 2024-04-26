// using System.Linq;
// using BodySource;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Controls;
// using UnityEngine.InputSystem.Layouts;
// using UnityEngine.InputSystem.LowLevel;
// using UnityEngine.InputSystem.XR;
// using UnityEngine.Scripting;
//
// [Preserve]
// [InputControlLayout(stateType = typeof(DGXRControllerState), isGenericTypeOfDevice = false,
//     displayName = "DGXR Controller", updateBeforeRender = true)]
// public class DGXRController : InputDevice
// {
//     //public BoneControl head { get; private set; }
//     
//     public AxisControl trigger { get; private set; }
//     public AxisControl grip { get; private set; }
//     public ButtonControl primaryButton { get; private set; }
//     public ButtonControl gripButton { get; private set; }
//     public ButtonControl triggerButton { get; private set; }
//     
//     public StickControl stick { get; private set; }
//
//     protected override void FinishSetup()
//     {
//         base.FinishSetup();
//
//         //head = GetChildControl<BoneControl>(nameof(head));
//         trigger = GetChildControl<AxisControl>(nameof(trigger));
//         grip = GetChildControl<AxisControl>(nameof(grip));
//         primaryButton = GetChildControl<ButtonControl>(nameof(primaryButton));
//         gripButton = GetChildControl<ButtonControl>(nameof(gripButton));
//         triggerButton = GetChildControl<ButtonControl>(nameof(triggerButton));
//     }
//     
//     public InputDevice CreateDevice()
//     {
//         // This is the code that you would normally run at the point where
//         // you discover devices of your custom type.
//         var device = InputSystem.AddDevice(new InputDeviceDescription
//         {
//             interfaceName = "DGXRController",
//             product = "DGXRController",
//             manufacturer = "deepglint",
//         });
//         Debug.Log("DGXR device: " + device.deviceId + " was created");
//         return device;
//     }
//     
//     public void RemoveDevice(int deviceId)
//     {
//         foreach (var inputDevice in InputSystem.devices)
//         {
//             if (inputDevice is DGXRController && inputDevice.deviceId == deviceId)
//             {
//                 Debug.Log("DGXR device: " + deviceId + " was removed");
//                 XREventListener.Instance.OnRaiseHandEvent -= OnRaiseHand;
//                 InputSystem.RemoveDevice(inputDevice); 
//             }
//         }
//     }
//
//     public void OnRaiseHand()
//     {
//         var state = new DGXRControllerState();
//         state.buttons |= 1 << 0;
//         InputSystem.QueueStateEvent(this, state);
//     }
//
//     public void OnMove(Vector2 position)
//     {
//         var state = new DGXRControllerState();
//         if (position.x > 0.5f)
//         {
//             state.x += 127;
//         }
//         else if (position.x < -0.5f)
//         {
//             state.x -= 127;
//         }
//        
//         if (position.y > 0.5f)
//         {
//             state.y += 127;
//         }
//         else if (position.y < -0.5f)
//         {
//             state.y -= 127;
//         }
//         InputSystem.QueueStateEvent(this, state);
//     }
// }