using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    [UnityEditor.InitializeOnLoad] 
    public class InteractionRegister
    {
        static InteractionRegister()
        {
            InputSystem.RegisterInteraction<RaiseHandInteraction>();
            InputSystem.RegisterInteraction<RaiseLeftHandInteraction>();
            InputSystem.RegisterInteraction<RaiseRightHandInteraction>();
            InputSystem.RegisterInteraction<RaiseBothHandInteraction>();
            InputSystem.RegisterInteraction<FreeSwimInteraction>();
            InputSystem.RegisterInteraction<ButterflySwimInteraction>();
            InputSystem.RegisterInteraction<HighKneeRunInteraction>();
            InputSystem.RegisterInteraction<DeepSquatInteraction>();
        }
    }
}