using UnityEngine;
using UnityEngine.EventSystems;

namespace Deepglint.XR.EventSystem.EventData
{
    public class HumanInteractionEventData : BaseEventData
    {
        public GameObject Player;
        public HumanInteractionEventData(UnityEngine.EventSystems.EventSystem eventSystem, GameObject player) 
            : base(eventSystem)
        {
            Player = player;
        }
    }
}