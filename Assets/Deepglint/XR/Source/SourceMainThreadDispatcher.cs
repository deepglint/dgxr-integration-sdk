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
        private static GameObject _connectObject; 
        private void Awake()
        {
            if (UseRos())
            {
                var ros = Extends.FindChildGameObject(gameObject,"RosConnect" );
                Source.DataFrom = SourceType.ROS;
                ros.SetActive(true);
            }
            else
            {
                var ws = Extends.FindChildGameObject(gameObject,"WsConnect" );
                Source.DataFrom = SourceType.WS;
                ws.SetActive(true);
            }
            GameLogger.Init(DGXR.Config.Log);
        }
        
        private bool UseRos()
        {
            if (!Application.isEditor && !DGXR.SystemName.Contains("Mac"))
            {
                return true;
            }

            return false;
        }
        
        private void Start()
        {
            gameObject.transform.parent = XRManager.XRDontDestroy.transform;
        }

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
            GameLogger.Cleanup();
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