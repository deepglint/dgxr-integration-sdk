using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
namespace DGXR

{
    // #if UNITY_INPUT_SYSTEM
    // [InitializeOnLoad]
    // #endif
    //     static class InputLayoutLoader
    //     {
    //         static InputLayoutLoader()
    //         {
    //             RegisterInputLayouts();
    //         }
    //
    //         public static void RegisterInputLayouts()
    //         {
    //             UnityEngine.InputSystem.InputSystem.RegisterLayout<ExampleVRController>(
    //                 matches: new InputDeviceMatcher()
    //                     .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
    //                     .WithProduct(".*")
    //             );
    //         }
    //     } 
    public class DGXR : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<DGXRController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(".*")
            );
        }

    }
}