using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
<<<<<<< dgxr/Assets/Runtime/InputSystem/Deepglint/XR/Interaction/InteractionRegister.cs
#endif 
=======
#endif
>>>>>>> dgxr/Assets/Runtime/InputSystem/Deepglint/XR/Interaction/InteractionRegister.cs
    public class InteractionRegister
    {
        static InteractionRegister()
        {
            InputSystem.RegisterInteraction<RaiseHandInteraction>();
            InputSystem.RegisterInteraction<RaiseLeftHandInteraction>();
            InputSystem.RegisterInteraction<RaiseRightHandInteraction>();
            InputSystem.RegisterInteraction<RaiseBothHandInteraction>();
            InputSystem.RegisterInteraction<SlideRightArmToLeftInteraction>();
            InputSystem.RegisterInteraction<SlideLeftArmToRightInteraction>();
            InputSystem.RegisterInteraction<FreeSwimInteraction>();
            InputSystem.RegisterInteraction<ButterflySwimInteraction>();
            InputSystem.RegisterInteraction<HighKneeRunInteraction>();
            InputSystem.RegisterInteraction<DeepSquatInteraction>();
        }
    }
}