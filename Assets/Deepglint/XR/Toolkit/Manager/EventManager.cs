using System;
using System.Collections.Generic;

namespace Deepglint.XR.Toolkit.Manager
{
    public class EventCallBack
    {
        private object[] _arguments;
        private string _type;
        private object _sender;

        public string Type
        {
            get => _type;
            set => _type = value;
        }

        public object[] Params
        {
            get => _arguments;
            set => _arguments = value;
        }

        public object Sender
        {
            get => _sender;
            set => _sender = value;
        }

        public override string ToString()
        {
            return _type + " [ " + (_sender == null ? "null" : _sender.ToString()) + " ] ";
        }

        public EventCallBack Clone()
        {
            return new EventCallBack(_type, _arguments, _sender);
        }

        public EventCallBack(string type)
        {
            Type = type;
        }

        public EventCallBack(string type, object sender)
        {
            Type = type;
            Sender = sender;
        }

        public EventCallBack(string type, object[] args, object sender)
        {
            Type = type;
            _arguments = args;
            Sender = sender;
        }
    }


    public delegate void EventListenerDelegate(EventCallBack evt);


    public class EventManager
    {
        private static EventManager _instance;

        public static EventManager Instance
        {
            get { return _instance ??= new EventManager(); }
        }

        private static Dictionary<string, EventListenerDelegate> _notifications = new();

        public static void RegisterListener(string type, EventListenerDelegate listener)
        {
            if (listener == null) return;

            _notifications.TryGetValue(type, out var myListener);
            _notifications[type] = (EventListenerDelegate)Delegate.Combine(myListener, listener);
        }

        public static void RemoveListener(string type, EventListenerDelegate listener)
        {
            if (listener == null) return;

            if (_notifications.ContainsKey(type))
                _notifications[type] = (EventListenerDelegate)Delegate.Remove(_notifications[type], listener);
        }

        public static void RemoveAllListeners()
        {
            _notifications.Clear();
        }

        public static void Send(string type, object[] args = null)
        {
            if (_notifications.ContainsKey(type) == false) return;
            var evt = new EventCallBack(type, args, Instance);
            Dispath(evt);
        }

        private static void Dispath(EventCallBack evt)
        {
            if (_notifications.TryGetValue(evt.Type, out var listenerDelegate))
                try
                {
                    if (listenerDelegate != null) listenerDelegate(evt);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        string.Concat(new string[]
                            { "Error dispatching event", evt.Type, ": ", e.Message, " ", e.StackTrace }), e);
                }
        }
    }
}