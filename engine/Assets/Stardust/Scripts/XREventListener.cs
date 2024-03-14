using System.Collections.Generic;
using System.Collections.Concurrent;
using Moat;
using Moat.Model;
using System;

namespace BodySource
{
    public class XREventListener
    {
        private static XREventListener instance;

        private float HighFiveHandDistanceOnThreshold = 0.07f;
        private float HighFiveHandDistanceOffThreshold = 0.099f;
        private int HighFiveHandThreshold = 3;

        private ConcurrentDictionary<string, int> HighFiveOnQueue = new ConcurrentDictionary<string, int> { };
        private ConcurrentDictionary<string, int> HighFiveOffQueue = new ConcurrentDictionary<string, int> { };
        private HashSet<string> HighFiveResult = new HashSet<string> { };

        // 私有构造函数，防止外部直接实例化
        private XREventListener()
        {
            HighFiveHandDistanceOnThreshold = DisplayData.configDisplay.eventListenerConfig.highFiveOnThreshold;
            HighFiveHandDistanceOffThreshold = DisplayData.configDisplay.eventListenerConfig.highFiveOffThreshold;
            MDebug.Log("XR event config high-five on: " + HighFiveHandDistanceOnThreshold + " off " + HighFiveHandDistanceOffThreshold);
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
            MDebug.Log("high-five event occurs betweens " + p1 + " and " + p2);
            // Check if event is subscribed
            if (OnHighFiveEvent != null)
            {
                int personId1 = int.Parse(p1) + 1;
                int personId2 = int.Parse(p2) + 1;
                // Invoke the event, which will call all the subscribed methods
                OnHighFiveEvent(personId1.ToString(), personId2.ToString());
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
                    if (IsHighFiveOnHappened(p1.Value, p2.Value))
                    {
                        if (HighFiveOnQueue.ContainsKey(key))
                        {
                            HighFiveOnQueue[key] += 1;
                        } else
                        {
                            HighFiveOnQueue[key] = 1;
                        }
                        HighFiveOffQueue[key] = 0;
                    } else
                    {
                        if (HighFiveResult.Contains(key))
                        {
                            if(IsHighFiveOffHappened(p1.Value, p2.Value))
                            {
                                if (HighFiveOffQueue.ContainsKey(key))
                                {
                                    HighFiveOffQueue[key] += 1;
                                } else
                                {
                                    HighFiveOffQueue[key] = 1;
                                }
                                if (HighFiveOffQueue[key] >= HighFiveHandThreshold)
                                {
                                    HighFiveResult.Remove(key);
                                }
                            }
                        }
                        HighFiveOnQueue[key] = 0;
                    }
                }
            }
            foreach (KeyValuePair<string, int> item in HighFiveOnQueue)
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

        private bool IsHighFiveOnHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float highThreshold = GetLowestShoulder(p1, p2);
            if (p1.Joints[JointType.LeftHand].Z >= highThreshold && p1.Joints[JointType.RightHand].Z >= highThreshold && p2.Joints[JointType.LeftHand].Z >= highThreshold && p2.Joints[JointType.RightHand].Z >= highThreshold)
            {
                float leftDistance = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
                float rightDistance = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
                if (leftDistance < 0.1f && rightDistance < 0.1f) 
                {
                    MDebug.Log("high-five distance left: " + leftDistance + " right: " + rightDistance);
                }
                if (leftDistance < HighFiveHandDistanceOffThreshold && rightDistance < HighFiveHandDistanceOffThreshold)
                {
                    if ((leftDistance+rightDistance) <= HighFiveHandDistanceOnThreshold)
                    {
                        result = true;
                    }
                }
            }
            
            return result;
        }

        private bool IsHighFiveOffHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float leftDistance = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
            float rightDistance = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
            if (leftDistance > HighFiveHandDistanceOffThreshold || rightDistance > HighFiveHandDistanceOffThreshold)
            {
                result = true;
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

        private float GetLowestShoulder(BodyDataSource p1, BodyDataSource p2)
        {
            float lowestShoulder = p1.Joints[JointType.LeftShoulder].Z <= p1.Joints[JointType.RightShoulder].Z ? p1.Joints[JointType.LeftShoulder].Z : p1.Joints[JointType.RightShoulder].Z;
            if (p2.Joints[JointType.LeftShoulder].Z < lowestShoulder)
            {
                lowestShoulder = p2.Joints[JointType.LeftShoulder].Z;
            }
            if (p2.Joints[JointType.RightShoulder].Z < lowestShoulder)
            {
                lowestShoulder = p2.Joints[JointType.RightShoulder].Z;
            }
            return lowestShoulder;
        }
    }

}
