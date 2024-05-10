using UnityEngine.InputSystem;

namespace DeepGlint.XR.Interaction
{
    /// <summary>
    /// Base class for metaverse action.
    /// </summary>
    public class MetaverseInteraction
    {
        protected int MissCount = 0;
        public float Confidence = 0f;

        protected void CheckMissCancel(ref InputInteractionContext context)
        {
            MissCount++;
            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
            {
                if (MissCount >= 5)
                {
                    context.Canceled();
                }
            }
        }

        public void Reset()
        {
            MissCount = 0;
        }
    }
}