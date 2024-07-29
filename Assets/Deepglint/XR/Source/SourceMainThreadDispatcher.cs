using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR.Source
{
    public class SourceMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<SourceData> TriggerMetaPoseDataQueue = new();
        private static readonly Queue<List<SourceData>> TriggerMetaPoseFrameDataQueue= new();
        private static readonly Queue<string> TriggerMetaPostDataLostQueue = new();
        
        private void Update()
        {
            lock (TriggerMetaPoseDataQueue)
            {
                while (TriggerMetaPoseDataQueue.Count > 0)
                {
                    Source.TriggerMetaPoseDataReceived(TriggerMetaPoseDataQueue.Dequeue());
                }
            }
            
            lock (TriggerMetaPoseFrameDataQueue)
            {
                while (TriggerMetaPoseFrameDataQueue.Count > 0)
                {
                    var data = TriggerMetaPoseFrameDataQueue.Dequeue();
                    if (data.Count > 0)
                    {
                        Source.TriggerMetaPoseFrameDataReceived(data[0].FrameId,data);
                    }
                }
            }
            
            lock (TriggerMetaPostDataLostQueue)
            {
                while (TriggerMetaPostDataLostQueue.Count > 0)
                {
                    Source.TriggerMetaPostDataLost(TriggerMetaPostDataLostQueue.Dequeue());
                }
            }
        }

        public static void Enqueue(SourceData data)
        {
            lock (TriggerMetaPoseDataQueue)
            {
                TriggerMetaPoseDataQueue.Enqueue(data);
            }
        }
        
        public static void Enqueue(List<SourceData> data)
        {
            lock (TriggerMetaPoseFrameDataQueue)
            {
                TriggerMetaPoseFrameDataQueue.Enqueue(data);
            }
        }
        
        public static void Enqueue(string key)
        {
            lock (TriggerMetaPostDataLostQueue)
            {
                TriggerMetaPostDataLostQueue.Enqueue(key);
            }
        }
    }
}