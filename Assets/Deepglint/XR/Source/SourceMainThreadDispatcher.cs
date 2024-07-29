using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR.Source
{
    public class SourceMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> ExecuteRosMsgEventMainThreadQueue = new(); 
        
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
            lock (ExecuteRosMsgEventMainThreadQueue)
            {
                ExecuteRosMsgEventMainThreadQueue.Enqueue(action);
            }
        }
    }
}