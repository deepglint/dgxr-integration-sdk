using Deepglint.XR.EventSystem.EventData;
using UnityEngine.EventSystems;

namespace Deepglint.XR.EventSystem
{
    public interface IHighFiveEventHandler : IEventSystemHandler
    {
        void OnHighFiveEvent(HumanInteractionEventData eventData);
    }
}