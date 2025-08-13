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
            // 算法
            InputSystem.RegisterInteraction<RightHandDrawCircleInteraction>("RightHandDrawCircle");
            InputSystem.RegisterInteraction<LeftHandDrawCircleInteraction>("LeftHandDrawCircle");
            InputSystem.RegisterInteraction<HandBevelCutInteraction>("HandBevelCut");
            InputSystem.RegisterInteraction<HandParryInteraction>("HandParry");
            InputSystem.RegisterInteraction<HandStraightCutInteraction>("HandStraightCut");
            InputSystem.RegisterInteraction<HandTransversalInteraction>("HandTransversal");
            InputSystem.RegisterInteraction<StraightPunchInteraction>("StraightPunch");
            InputSystem.RegisterInteraction<ReadyStraightPunchInteraction>("ReadyStraightPunch");
            InputSystem.RegisterInteraction<UppercutInteraction>("Uppercut");
            InputSystem.RegisterInteraction<KickInteraction>("Kick");
            InputSystem.RegisterInteraction<ThrowOneHandInFistsInteraction>("ThrowOneHandInFists");
            InputSystem.RegisterInteraction<ReadyThrowOneHandInFistsInteraction>("ReadyThrowOneHandInFists");
            InputSystem.RegisterInteraction<ReadyThrowBothHandInFistsInteraction>("ReadyThrowBothHandInFists");
            InputSystem.RegisterInteraction<CombineHandsStraightInteraction>("CombineHandsStraight");
            InputSystem.RegisterInteraction<SlowRunInteraction>("SlowRun");
            InputSystem.RegisterInteraction<HighKneeRunInteraction>("HighKneeRun");
            InputSystem.RegisterInteraction<ButterflySwimInteraction>("ButterflySwim");
            InputSystem.RegisterInteraction<FreeSwimInteraction>("FreeSwim");
            InputSystem.RegisterInteraction<KeepRaisingHandInteraction>("KeepRaisingHand");
            InputSystem.RegisterInteraction<CheerUpInteraction>("CheerUp");
            InputSystem.RegisterInteraction<JumpInteraction>("Jump");
            InputSystem.RegisterInteraction<DeepSquatInteraction>("DeepSquat");
            InputSystem.RegisterInteraction<ArmFlatInteraction>("ArmFlat");
            InputSystem.RegisterInteraction<ArmFlatIsLInteraction>("ArmFlatIsL");
            InputSystem.RegisterInteraction<ArmVerticalIsLInteraction>("ArmVerticalIsL");
            
            // 扩展
            InputSystem.RegisterInteraction<RaiseHandInteraction>("RaiseHand");
            InputSystem.RegisterInteraction<RaiseLeftHandInteraction>("RaiseLeftHand");
            InputSystem.RegisterInteraction<RaiseRightHandInteraction>("RaiseRightHand");
            InputSystem.RegisterInteraction<RaiseSingleHandInteraction>("RaiseSingleHand");
            InputSystem.RegisterInteraction<RaiseBothHandInteraction>("RaiseBothHand");
            InputSystem.RegisterInteraction<SlideRightArmToLeftInteraction>("SlideRightArmToLeft");
            InputSystem.RegisterInteraction<SlideLeftArmToRightInteraction>("SlideLeftArmToRight");
        }

        public static void RegisterInteraction<T>(string name = null)
        {
            InputSystem.RegisterInteraction<T>(name);
        }
    }
}