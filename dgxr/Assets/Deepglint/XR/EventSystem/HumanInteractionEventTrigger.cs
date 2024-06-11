using System;
using System.Collections.Generic;
using System.Linq;
using Deepglint.XR.EventSystem.EventData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Deepglint.XR.EventSystem
{
    public enum HumanInteractionEventTriggerType
    {
        HighFiveEvent = 0
    }
    
    [AddComponentMenu("Event/Human Interaction Event Trigger")]
    public class HumanInteractionEventTrigger : 
        MonoBehaviour, 
        IHighFiveEventHandler 
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<BaseEventData>
        {
        }

        [Serializable]
        public class Entry
        {
            [FormerlySerializedAs("EventID")]
            public HumanInteractionEventTriggerType eventID = HumanInteractionEventTriggerType.HighFiveEvent;
            [FormerlySerializedAs("Callback")] 
            public TriggerEvent callback = new TriggerEvent();
        }
        
        [SerializeField]
        private List<Entry> delegates;

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("Please use triggers instead (UnityUpgradable) -> triggers", true)]
        public List<Entry> Delegates { 
            get => Triggers;
            set => Triggers = value;
        }

        protected HumanInteractionEventTrigger()
        {}
        
        public List<Entry> Triggers
        {
            get { return delegates ??= new List<Entry>(); }
            set => delegates = value;
        }
        
        private void Execute(HumanInteractionEventTriggerType id, BaseEventData eventData)
        {
            foreach (var ent in Triggers.Where(ent => ent.eventID == id && ent.callback != null))
            {
                ent.callback.Invoke(eventData);
            }
        }

        public virtual void OnHighFiveEvent(HumanInteractionEventData eventData)
        {
            Execute(HumanInteractionEventTriggerType.HighFiveEvent, eventData);
        }
    }
}