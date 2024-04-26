using UnityEngine.InputSystem;

namespace Deepglint.XR.Interaction
{
    /// <summary>
    /// Base class for metaverse action.
    /// </summary>
    public class MetaverseInteraction
    {
        protected int missCount = 0;
        public float confidence = 0f;

        protected void CheckMissCancel(ref InputInteractionContext context)
        {
            missCount++;
            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
            {
                if (missCount >= 5)
                {
                    context.Canceled();
                }
            }
        }

        public void Reset()
        {
            missCount = 0;
        }
    }
}