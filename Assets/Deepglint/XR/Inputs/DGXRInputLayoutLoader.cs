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
        static DeepglintInputLayoutLoader()
        {
            Initialize();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            RegisterInputLayouts();
        }
    
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif  
        public static void RegisterInputLayouts()
        {
            InputSystem.RegisterLayout<HumanPoseControl>("HumanPose");
            InputSystem.RegisterLayout<HumanBodyControl>("HumanBody");
            InputSystem.RegisterLayout<DGXRHumanController>(
                matches: new InputDeviceMatcher()
                    .WithProduct(nameof(DGXRHumanController)));
            InputSystem.RegisterLayout<DGXRDeviceSimulator>(
                matches: new InputDeviceMatcher()
                    .WithProduct(nameof(DGXRDeviceSimulator))); 
        }
    } 
}
