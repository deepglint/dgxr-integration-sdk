using System;
using System.Collections.Generic;
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