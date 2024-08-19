using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] 
#endif
    public static class InteractionRegister
    {
        static InteractionRegister()
        {
            Initialize();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            InputSystem.RegisterInteraction<RaiseHandInteraction>("RaiseHand");
            InputSystem.RegisterInteraction<RaiseLeftHandInteraction>("RaiseLeftHand");
            InputSystem.RegisterInteraction<RaiseRightHandInteraction>("RaiseRightHand");
            InputSystem.RegisterInteraction<RaiseSingleHandInteraction>("RaiseSingleHand");
            InputSystem.RegisterInteraction<RaiseBothHandInteraction>("RaiseBothHand");
            InputSystem.RegisterInteraction<SlideRightArmToLeftInteraction>("SlideRightArmToLeft");
            InputSystem.RegisterInteraction<SlideLeftArmToRightInteraction>("SlideLeftArmToRight");
            InputSystem.RegisterInteraction<FreeSwimInteraction>("FreeSwim");
            InputSystem.RegisterInteraction<ButterflySwimInteraction>("ButterflySwim");
            InputSystem.RegisterInteraction<HighKneeRunInteraction>("HighKneeRun");
            InputSystem.RegisterInteraction<DeepSquatInteraction>("DeepSquat");
            InputSystem.RegisterInteraction<JumpInteraction>("Jump");
            InputSystem.RegisterInteraction<CheerUpInteraction>("CheerUp");
        }

        public static void RegisterInteraction<T>(string name = null)
        {
            InputSystem.RegisterInteraction<T>(name); 
        }
    }
}