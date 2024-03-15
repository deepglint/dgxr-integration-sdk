using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

[Preserve]
[InputControlLayout(displayName = "Deepglint XR Controller")]
public class DGXRController : XRController
{
    // Add control elements specific to your XR controller here
    [Preserve]
    [InputControl(aliases = new[] { "PrimaryButton" })]
    public ButtonControl exampleButton { get; private set; }

    [Preserve]
    [InputControl]
    public Vector3Control examplePosition { get; private set; }


    protected override void FinishSetup()
    {
        base.FinishSetup();

        exampleButton = GetChildControl<ButtonControl>("exampleButton");
        examplePosition = GetChildControl<Vector3Control>("examplePosition");
    }
}