using DGXR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[Preserve]
public static class SimulatedInputLayoutLoader
{
    [Preserve]
    static SimulatedInputLayoutLoader()
    {
        RegisterInputLayouts();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
    public static void Initialize()
    {
        // Will execute the static constructor as a side effect.
    }
    
    static void RegisterInputLayouts()
    {
        InputSystem.RegisterLayout<DGXRController>(
            matches: new InputDeviceMatcher()
                .WithProduct(nameof(DGXRController)));
        InputSystem.RegisterLayout<DGXRDeviceSimulator>(
            matches: new InputDeviceMatcher()
                .WithProduct(nameof(DGXRDeviceSimulator))); 
    }
}

