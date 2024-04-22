using System;
using System.Collections.Generic;

namespace Stardust.Scripts
{
    public class MoatGameEvent
    {
        public static string WsConnectSuccess = "WsconnectSuccess";
        public static string WsConnectError = "WsconnectError";
        public static string OnOpenApp = "OnOpenApp";
        public static string PlayerRemove = "PlayerRemove";
        public static string PlayerAdd = "PlayerAdd";
    }

    public class EventCallBack
    {
        protected object[] Arguments;
        protected string TypePro;
        protected Object SenderPro;

        public string Type
        {
            get { return TypePro; }
            set { TypePro = value; }
        }

        public object[] Params
        {
            get { return Arguments; }
            set { Arguments = value; }
        }

        public object Sender
        {
            get { return SenderPro; }
            set { SenderPro = value; }
        }

        public override string ToString()
        {
            return TypePro + " [ " + ((SenderPro == null) ? "null" : SenderPro.ToString()) + " ] ";
        }

        public EventCallBack Clone()
        {
            return new EventCallBack(TypePro, Arguments, SenderPro);
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

        public EventCallBack(string type, object[] args, Object sender)
        {
            Type = type;
            Arguments = args;
            Sender = sender;
        }
    }


    public delegate void EventListenerDelegate(EventCallBack evt);


    public class EventManager
    {

        private static EventManager _instance;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventManager();
                return _instance;
            }
        }

        static Dictionary<string, EventListenerDelegate> _notifications =
            new Dictionary<string, EventListenerDelegate>();

        public static void RegisterListener(string type, EventListenerDelegate listener)
        {
            if (listener == null)
            {
                MDebug.LogError("registerObserver: listener不能为空");
                return;
            }

            EventListenerDelegate myListener;
            _notifications.TryGetValue(type, out myListener);
            _notifications[type] = (EventListenerDelegate)Delegate.Combine(myListener, listener);
        }

        public static void RemoveListener(string type, EventListenerDelegate listener)
        {

            if (listener == null)
            {
                MDebug.LogError("removeObserver: listener不能为空");
                return;
            }

            if (_notifications.ContainsKey(type))
            {
                MDebug.Log("notification: remove " + type);
                _notifications[type] = (EventListenerDelegate)Delegate.Remove(_notifications[type], listener);
            }
        }

        public static void RemoveAllListeners()
        {
            _notifications.Clear();
        }

        public static void Send(string type, object[] args = null)
        {
            if (_notifications.ContainsKey(type) == false) return;
            EventCallBack evt = new EventCallBack(type, args, EventManager.Instance);
            Dispath(evt);
        }

        static void Dispath(EventCallBack evt)
        {
            EventListenerDelegate listenerDelegate;
            if (_notifications.TryGetValue(evt.Type, out listenerDelegate))
            {
                try
                {
                    if (listenerDelegate != null)
                    {
                        listenerDelegate(evt);
                    }

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

}