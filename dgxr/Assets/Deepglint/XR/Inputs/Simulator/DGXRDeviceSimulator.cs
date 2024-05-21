using System.Linq;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
using UnityEditor;
#endif

[InputControlLayout(stateType = typeof(DGXRHumanControllerState), isGenericTypeOfDevice = false, displayName = "DGXR Controller Simulator", updateBeforeRender = true)]
public class DGXRDeviceSimulator : InputDevice, IInputUpdateCallbackReceiver 
{
    public AxisControl trigger { get; private set; }
    public AxisControl grip { get; private set; }
    
    public StickControl stick { get; private set; }
    
    protected override void FinishSetup()
    {
        base.FinishSetup();

        trigger = GetChildControl<AxisControl>(nameof(trigger));
        grip = GetChildControl<AxisControl>(nameof(grip));
    }
    
    public static DGXRDeviceSimulator current { get; private set; }

    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }
    
    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }
   
    #if UNITY_EDITOR
    [MenuItem("Tools/DGXR Device Simulator/Create Device")]
    private static void CreateDevice()
    {
        // This is the code that you would normally run at the point where
        // you discover devices of your custom type.
        var device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "DGXRDeviceSimulator",
            product = "DGXRDeviceSimulator"
        });
        
        Debug.Log("device: " + device.deviceId + " was created");
    }
    
    [MenuItem("Tools/DGXR Device Simulator/Remove Device")]
    private static void RemoveDevice()
    {
        var simulatorDevice = InputSystem.devices.FirstOrDefault(x => x is DGXRDeviceSimulator);
        if (simulatorDevice != null)
        {
            InputSystem.RemoveDevice(simulatorDevice);
            Debug.Log("device: " + simulatorDevice.deviceId + " was removed");
        }
    }
    #endif

    public void OnUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        var state = new DGXRHumanControllerState();

        state.x = 127;
        state.y = 127;
        
        // Map WASD to stick.
        var wPressed = keyboard.wKey.isPressed;
        var aPressed = keyboard.aKey.isPressed;
        var sPressed = keyboard.sKey.isPressed;
        var dPressed = keyboard.dKey.isPressed;

        if (aPressed)
        {
            //Debug.Log("A is pressed");
            state.x -= 127;
        }
        if (dPressed)
        {
            //Debug.Log("D is pressed");
            state.x += 127;
        }
        if (wPressed)
        {
            //Debug.Log("W is pressed");
            state.y += 127;
        }
        if (sPressed)
        {
            //Debug.Log("S is pressed");
            state.y -= 127;
        }
        
        // Map buttons to 1, 2, and 3.
        if (keyboard.digit1Key.isPressed)
            state.buttons |= 1 << 0;
        if (keyboard.digit2Key.isPressed)
            state.buttons |= 1 << 1;
        if (keyboard.digit3Key.isPressed)
            state.buttons |= 1 << 2;

        InputSystem.QueueStateEvent(this, state);
    }
}
