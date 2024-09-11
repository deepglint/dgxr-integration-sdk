using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// Base class for metaverse action.
    /// </summary>
    public class MetaverseInteraction
    {
        protected int HitCount = 0;
        protected int MissCount = 0;
        public float Confidence = 0f;
        public int MissThreshold = 5;
        public int HitThreshold = 3;

        protected bool CheckMissCancel(ref InputInteractionContext context)
        {
            MissCount++;
            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
            {
                if (MissCount >= MissThreshold)
                {
                    context.Canceled();
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            MissCount = 0;
            HitCount = 0;
        }
    }
}