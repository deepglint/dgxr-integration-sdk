using UnityEngine.EventSystems;

namespace Deepglint.XR.EventSystem.EventData
{
    public class HumanPointerEventData : PointerEventData
    {
        public enum InputButton
        {
            LeftFoot = 0,
            RightFoot = 1,
        }

        public HumanPointerEventData(UnityEngine.EventSystems.EventSystem eventSystem) : base(eventSystem)
        {
        }
    }
}