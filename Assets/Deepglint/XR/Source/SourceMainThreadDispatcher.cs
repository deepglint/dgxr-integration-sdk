using System;
using System.Collections.Generic;
using Deepglint.XR.Log;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Source
{
    public class SourceMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> ExecuteRosMsgEventMainThreadQueue = new();

        private void OnDisable()
        {
            Debug.Log($"SourceMainThreadDispatcher was disable, start to clean the queue");
            lock (ExecuteRosMsgEventMainThreadQueue)
            {
                while (ExecuteRosMsgEventMainThreadQueue.Count > 0)
                {
                    ExecuteRosMsgEventMainThreadQueue.Dequeue().Invoke();
                }
            }
        }

        private void Start()
        {
            if (UseRos())
            {
                var ros = Extends.FindChildGameObject(gameObject, "RosConnect");
                Source.DataFrom = SourceType.ROS;
                ros.SetActive(true);
            }
            else
            {
                var ws = Extends.FindChildGameObject(gameObject, "WsConnect");
                Source.DataFrom = SourceType.WS;
                ws.SetActive(true);
            }
        }

    private bool UseRos()
        {
            if (!Application.isEditor && !DGXR.SystemName.Contains("Mac"))
            {
                return true;
            }

            return false;
        }
        
        private void Update()
        {
            lock (ExecuteRosMsgEventMainThreadQueue)
            {
                while (ExecuteRosMsgEventMainThreadQueue.Count > 0)
                {
                    ExecuteRosMsgEventMainThreadQueue.Dequeue().Invoke();
                }
            }
        }
        
        public static void Enqueue(Action action)
        {
            // TODO 无论开启不开启后台执行，都不推送骨骼
            if(!Application.isFocused)
            {
                return;
            }
            lock (ExecuteRosMsgEventMainThreadQueue)
            {
                ExecuteRosMsgEventMainThreadQueue.Enqueue(action);
            }
        }
    }
}