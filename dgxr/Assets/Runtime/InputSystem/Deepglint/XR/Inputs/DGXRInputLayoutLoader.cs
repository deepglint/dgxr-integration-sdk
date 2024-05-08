using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deepglint.XR.Inputs
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    public static class DeepglintInputLayoutLoader
    {
        [Preserve]
        static DeepglintInputLayoutLoader()
        {
            RegisterInputLayouts();
        }
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
        public static void Initialize()
        {
            // Will execute the static constructor as a side effect.
        }
    
        [InitializeOnLoadMethod]
        public static void RegisterInputLayouts()
        {
            InputSystem.RegisterLayout<HumanPoseControl>("HumanPose");
            InputSystem.RegisterLayout<HumanBodyControl>("HumanBody");
            InputSystem.RegisterLayout<DGXRController>(
                matches: new InputDeviceMatcher()
                    .WithProduct(nameof(DGXRController)));
            InputSystem.RegisterLayout<DGXRDeviceSimulator>(
                matches: new InputDeviceMatcher()
                    .WithProduct(nameof(DGXRDeviceSimulator))); 
        }
    } 
}
