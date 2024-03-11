using System.Collections.Generic;
using System.Collections.Concurrent;
using Moat;
using System;

namespace BodySource
{
    public class XREventListener
    {
        private static XREventListener instance;

        public float HighFiveHandDistanceThreshold = 0.05f;
        public int HighFiveHandThreshold = 5;

        public ConcurrentDictionary<string, int> HighFiveQueue = new ConcurrentDictionary<string, int> { };
        public HashSet<string> HighFiveResult = new HashSet<string> { };

        // 私有构造函数，防止外部直接实例化
        public XREventListener()
        {
        }

        // 获取GameManager的实例
        public static XREventListener Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XREventListener();
                }
                return instance;
            }
        }

        public Action<string, string> OnHighFiveEvent;


        private void RaiseHighFiveEvent(string p1, string p2)
        {
            MDebug.LogError("high five event occurs betweens " + p1 + " and " + p2);
            // Check if event is subscribed
            if (OnHighFiveEvent != null)
            {
                // Invoke the event, which will call all the subscribed methods
                OnHighFiveEvent(p1, p2);
            }
        }


        public void OnFrame()
        {
            foreach (var p1 in XRDGBodySource.Instance.Data)
            {
                int personId1 = int.Parse(p1.Key);
                foreach (var p2 in XRDGBodySource.Instance.Data)
                {
                    int personId2 = int.Parse(p2.Key);
                    if (personId2 <= personId1)
                    {
                        continue;
                    }
                    string key = p1.Key + "_" + p2.Key;
                    if (personId1 > personId2)
                    {
                        key = p2.Key + "_" + p1.Key;
                    }
                    if (IsHighFiveHappened(p1.Value, p2.Value))
                    {
                        if (HighFiveQueue.ContainsKey(key))
                        {
                            HighFiveQueue[key] += 1;
                        } else
                        {
                            HighFiveQueue[key] = 1;
                        }
                    } else
                    {
                        HighFiveQueue[key] = 0;
                        HighFiveResult.Remove(key);
                    }
                }
            }
            foreach (KeyValuePair<string, int> item in HighFiveQueue)
            {
                if (item.Value >= HighFiveHandThreshold)
                {
                    if (HighFiveResult.Contains(item.Key))
                    {
                        continue;
                    } else
                    {
                        // callback;
                        string[] persons = item.Key.Split("_");
                        RaiseHighFiveEvent(persons[0], persons[1]);
                        HighFiveResult.Add(item.Key);
                    }
                }
            }
        }

        private bool IsHighFiveHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float elbowThreshold = GetLowestElbow(p1, p2);
            if (p1.Joints[JointType.LeftHand].Z > elbowThreshold && p1.Joints[JointType.RightHand].Z > elbowThreshold && p2.Joints[JointType.LeftHand].Z > elbowThreshold && p2.Joints[JointType.RightHand].Z > elbowThreshold)
            {
                MDebug.LogError("distance: " + p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]));
                if (p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]) <= HighFiveHandDistanceThreshold && p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]) <= HighFiveHandDistanceThreshold)
                {
                    result = true;
                }
            }
            
            return result;
        }

        private float GetLowestElbow(BodyDataSource p1, BodyDataSource p2)
        {
            float lowestElbow = p1.Joints[JointType.LeftElbow].Z <= p1.Joints[JointType.RightElbow].Z ? p1.Joints[JointType.LeftElbow].Z : p1.Joints[JointType.RightElbow].Z;
            if (p2.Joints[JointType.LeftElbow].Z < lowestElbow)
            {
                lowestElbow = p2.Joints[JointType.LeftElbow].Z;
            }
            if (p2.Joints[JointType.RightElbow].Z < lowestElbow)
            {
                lowestElbow = p2.Joints[JointType.RightElbow].Z;
            }
            return lowestElbow;
        }
    }

}
