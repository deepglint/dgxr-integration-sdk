using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Stardust.Model;

namespace Stardust.Scripts
{
    public class XREventListener
    {
        private static XREventListener _instance;

        private bool _single;
        private float _highFiveHandDistanceOnThreshold;
        private float _highFiveHandDistanceOffThreshold;
        private int _highFiveHandThreshold = 3;
        private float _logThreshold = 0.2f;

        private ConcurrentDictionary<string, int> _highFiveOnQueue = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> _highFiveOffQueue = new ConcurrentDictionary<string, int>();
        private HashSet<string> _highFiveResult = new HashSet<string>();

        // 私有构造函数，防止外部直接实例化
        private XREventListener()
        {
            _highFiveHandDistanceOnThreshold = DisplayData.ConfigDisplay.EventListenerConfig.HighFiveOnThreshold;
            _highFiveHandDistanceOffThreshold = DisplayData.ConfigDisplay.EventListenerConfig.HighFiveOffThreshold;
            _single = DisplayData.ConfigDisplay.EventListenerConfig.Single;
            MDebug.Log("XR event config high-five on: " + _highFiveHandDistanceOnThreshold + " off " + _highFiveHandDistanceOffThreshold);
        }

        // 获取GameManager的实例
        public static XREventListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new XREventListener();
                }
                return _instance;
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
            foreach (var p1 in XrdgBodySource.Instance.Data)
            {
                int personId1 = int.Parse(p1.Key);
                foreach (var p2 in XrdgBodySource.Instance.Data)
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
                        if (_highFiveOnQueue.ContainsKey(key))
                        {
                            _highFiveOnQueue[key] += 1;
                        } else
                        {
                            _highFiveOnQueue[key] = 1;
                        }
                        _highFiveOffQueue[key] = 0;
                    } else
                    {
                        if (_highFiveResult.Contains(key))
                        {
                            if(IsHighFiveOffHappened(p1.Value, p2.Value))
                            {
                                if (_highFiveOffQueue.ContainsKey(key))
                                {
                                    _highFiveOffQueue[key] += 1;
                                } else
                                {
                                    _highFiveOffQueue[key] = 1;
                                }
                                if (_highFiveOffQueue[key] >= _highFiveHandThreshold)
                                {
                                    _highFiveResult.Remove(key);
                                }
                            }
                        }
                        _highFiveOnQueue[key] = 0;
                    }
                }
            }
            foreach (KeyValuePair<string, int> item in _highFiveOnQueue)
            {
                if (item.Value >= _highFiveHandThreshold)
                {
                    if (_highFiveResult.Contains(item.Key))
                    {
                    } else
                    {
                        // callback;
                        string[] persons = item.Key.Split("_");
                        RaiseHighFiveEvent(persons[0], persons[1]);
                        _highFiveResult.Add(item.Key);
                    }
                }
            }
        }

        private float GetAncherThreshold(BodyDataSource p1, BodyDataSource p2)
        {
            return GetLowestShoulder(p1, p2);
        }

        private bool IsHighFiveOnHappened(BodyDataSource p1, BodyDataSource p2)
        {
            if (_single)
            {
                return IsSingleHighFiveOnHappened(p1, p2);
            } else 
            {
                return IsDoubleHighFiveOnHappened(p1, p2);
            }
        }

        private bool IsDoubleHighFiveOnHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float ancherThreshold = GetAncherThreshold(p1, p2);
            if (p1.Joints[JointType.LeftHand].Z >= ancherThreshold && p1.Joints[JointType.RightHand].Z >= ancherThreshold && p2.Joints[JointType.LeftHand].Z >= ancherThreshold && p2.Joints[JointType.RightHand].Z >= ancherThreshold)
            {
                float leftDistance = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
                float rightDistance = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
                if (leftDistance < _logThreshold && rightDistance < _logThreshold) 
                {
                    MDebug.Log("high-five distance left: " + leftDistance + " right: " + rightDistance);
                }
                if (leftDistance < _highFiveHandDistanceOffThreshold && rightDistance < _highFiveHandDistanceOffThreshold)
                {
                    if ((leftDistance+rightDistance)*0.5f <= _highFiveHandDistanceOnThreshold)
                    {
                        result = true;
                    }
                }
            }
            
            return result;
        }

        private bool IsSingleHighFiveOnHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float ancherThreshold = GetAncherThreshold(p1, p2);
            if (p1.Joints[JointType.LeftHand].Z < ancherThreshold && p1.Joints[JointType.RightHand].Z < ancherThreshold)
            {
                return false;
            }
            if (p2.Joints[JointType.LeftHand].Z < ancherThreshold && p2.Joints[JointType.RightHand].Z < ancherThreshold)
            {
                return false;
            }
            float left1 = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
            float left2 = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.LeftHand]);
            float leftDistance = left2 < left1 ? left2 : left1;
            float right1 = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.RightHand]);
            float right2 = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
            float rightDistance = right2 < right1 ? right2 : right1;
            float handDistance = rightDistance < leftDistance ? rightDistance : leftDistance;
            if (handDistance < _logThreshold) 
            {
                MDebug.Log("single high-five distance " + handDistance);
                if (handDistance <= _highFiveHandDistanceOnThreshold)
                {
                    result = true;
                }
            }
            
            return result;
        }

        private bool IsHighFiveOffHappened(BodyDataSource p1, BodyDataSource p2)
        {
            if (_single)
            {
                return IsSingleHighFiveOffHappened(p1, p2);
            } else 
            {
                return IsDoubleHighFiveOffHappened(p1, p2);
            }
        }

        private bool IsSingleHighFiveOffHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float left1 = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
            float left2 = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.LeftHand]);
            float leftDistance = left2 < left1 ? left2 : left1;
            float right1 = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.RightHand]);
            float right2 = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
            float rightDistance = right2 < right1 ? right2 : right1;
            float handDistance = rightDistance < leftDistance ? rightDistance : leftDistance;
            
            if (handDistance > _highFiveHandDistanceOffThreshold)
            {
                result = true;
            }
            
            return result;
        }

        private bool IsDoubleHighFiveOffHappened(BodyDataSource p1, BodyDataSource p2)
        {
            bool result = false;
            float leftDistance = p1.Joints[JointType.LeftHand].Distance(p2.Joints[JointType.RightHand]);
            float rightDistance = p1.Joints[JointType.RightHand].Distance(p2.Joints[JointType.LeftHand]);
            if (leftDistance > _highFiveHandDistanceOffThreshold || rightDistance > _highFiveHandDistanceOffThreshold)
            {
                result = true;
            }
            
            return result;
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
