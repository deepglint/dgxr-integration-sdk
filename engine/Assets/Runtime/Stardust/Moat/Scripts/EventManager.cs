using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moat
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
        protected System.Object[] arguments;
        protected string type;
        protected System.Object sender;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public System.Object[] Params
        {
            get { return arguments; }
            set { arguments = value; }
        }

        public System.Object Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public override string ToString()
        {
            return type + " [ " + ((sender == null) ? "null" : sender.ToString()) + " ] ";
        }

        public EventCallBack Clone()
        {
            return new EventCallBack(type, arguments, sender);
        }

        public EventCallBack(string type)
        {
            Type = type;
        }

        public EventCallBack(string type, System.Object sender)
        {
            Type = type;
            Sender = sender;
        }

        public EventCallBack(string type, System.Object[] args, System.Object sender)
        {
            Type = type;
            arguments = args;
            Sender = sender;
        }
    }


    public delegate void EventListenerDelegate(EventCallBack evt);


    public class EventManager
    {

        private static EventManager instance;

        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new EventManager();
                return instance;
            }
        }

        static Dictionary<string, EventListenerDelegate> notifications =
            new Dictionary<string, EventListenerDelegate>();

        public static void RegisterListener(string type, EventListenerDelegate listener)
        {
            if (listener == null)
            {
                MDebug.LogError("registerObserver: listener不能为空");
                return;
            }

            // MDebug.Log("NotifacitionCenter: 添加监视" + type);

            EventListenerDelegate myListener = null;
            notifications.TryGetValue(type, out myListener);
            notifications[type] = (EventListenerDelegate)Delegate.Combine(myListener, listener);
        }

        public static void RemoveListener(string type, EventListenerDelegate listener)
        {

            if (listener == null)
            {
                MDebug.LogError("removeObserver: listener不能为空");
                return;
            }

            if (notifications.ContainsKey(type))
            {
                MDebug.Log("NotifacitionCenter: 移除监视" + type);
                notifications[type] = (EventListenerDelegate)Delegate.Remove(notifications[type], listener);
            }
        }

        public static void RemoveAllListeners()
        {
            notifications.Clear();
        }

        public static void Send(string type, object[] args = null)
        {
            if (notifications.ContainsKey(type) == false) return;
            EventCallBack evt = new EventCallBack(type, args, EventManager.Instance);
            Dispath(evt);
        }

        static void Dispath(EventCallBack evt)
        {
            EventListenerDelegate listenerDelegate;
            if (notifications.TryGetValue(evt.Type, out listenerDelegate))
            {
                try
                {
                    if (listenerDelegate != null)
                    {
                        listenerDelegate(evt);
                    }

                }
                catch (System.Exception e)
                {
                    throw new Exception(
                        string.Concat(new string[]
                            { "Error dispatching event", evt.Type.ToString(), ": ", e.Message, " ", e.StackTrace }), e);
                }
            }
        }

    }

}