using System.Collections.Generic;
using System.Collections.Concurrent;
using Moat;
using Moat.Model;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BodySource
{
    public class XREventListener
    {
        private static XREventListener instance;

        private bool Single = true;
        private float HighFiveHandDistanceOnThreshold = 0.09f;
        private float HighFiveHandDistanceOffThreshold = 0.099f;
        private int HighFiveHandThreshold = 3;
        private float logThreshold = 0.2f;

        private float positionThresholdMin = 0.5f;
        

        private ConcurrentDictionary<string, DGXRController> devices =
            new ConcurrentDictionary<string, DGXRController>{};

        private ConcurrentDictionary<string, int> HighFiveOnQueue = new ConcurrentDictionary<string, int> { };
        private ConcurrentDictionary<string, int> HighFiveOffQueue = new ConcurrentDictionary<string, int> { };
        private HashSet<string> HighFiveResult = new HashSet<string> { };
        
        private ConcurrentDictionary<string, int> RaiseHandOnQueue = new ConcurrentDictionary<string, int> { };
        private ConcurrentDictionary<string, int> RaiseHandOffQueue = new ConcurrentDictionary<string, int> { };
        private HashSet<string> RaiseHandResult = new HashSet<string> { };

        // 私有构造函数，防止外部直接实例化
        private XREventListener()
        {
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
        
        public Action OnRaiseHandEvent;

        public Action<Vector2> OnPosition;

        private void OnPositionUpdate(string pId, Vector2 v2)
        {
            Vector2 position = new Vector2(v2.x + 1f, v2.y + 1f);
            var state = new DGXRControllerState();
            if (Math.Abs(v2.x) <= positionThresholdMin)
            {
                state.x = (byte)(127.5f);
            } else if (v2.x >= positionThresholdMin + 1f)
            {
                state.x = (byte)(255);
            } else if (v2.x <= -1 - positionThresholdMin)
            {
                state.x = (byte)(0);
            }
            else if (v2.x > 0)
            {
                state.x = (byte)((v2.x + 1f - positionThresholdMin) * 127.5f);
            }
            else
            {
                state.x = (byte)((v2.x + 1f + positionThresholdMin) * 127.5f); 
            }
            
            if (Math.Abs(v2.y) <= positionThresholdMin)
            {
                state.y = (byte)(127.5f);
            } else if (v2.y >= positionThresholdMin + 1f)
            {
                state.y = (byte)(255);
            } else if (v2.y <= -1 - positionThresholdMin)
            {
                state.y = (byte)(0);
            }
            else if (v2.y > 0)
            {
                state.y = (byte)((v2.y + 1f - positionThresholdMin) * 127.5f);
            }
            else
            {
                state.y = (byte)((v2.y +1f + positionThresholdMin) * 127.5f); 
            }
             
            Debug.Log(pId + " x: "+ (float)state.x + ", y: " + (float)state.y);
            InputDevice device = XRDGBodySource.Instance.Devices[pId];
            if (device != null)
            {
                InputSystem.QueueStateEvent(device, state); 
            }
        }
        
        private void RaiseRightHandEventOff(string pId)
        {
            MDebug.Log("raise hand off event occurs: " + pId);
            InputDevice device = XRDGBodySource.Instance.Devices[pId];
            if (device != null)
            {
                var state = new DGXRControllerState();
                state.buttons = 0; 
                MDebug.Log("off primary button: " + pId);
                InputSystem.QueueStateEvent(device, state); 
            }
        }

        private void RaiseRightHandEventOn(string pId)
        {
            MDebug.Log("raise hand event occurs: " + pId);
            InputDevice device = XRDGBodySource.Instance.Devices[pId];
            if (device != null)
            {
                var state = new DGXRControllerState();
                state.buttons |= 1 << 0;
                MDebug.Log("trigger primary button: " + pId);
                InputSystem.QueueStateEvent(device, state); 
            }
        }
        
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
                OnPositionUpdate(p1.Key, p1.Value.GetRootPositionVector2());
            }
            judgeRaiseHand();
            //judgeHighFive();
        }

        private void judgeRaiseHand()
        {
            foreach (var p1 in XRDGBodySource.Instance.Data)
            {
                if (p1.Value.Joints[JointType.RightHand].Z > p1.Value.Joints[JointType.HeadTop].Z)
                {
                    if (RaiseHandOnQueue.ContainsKey(p1.Key))
                    {
                        RaiseHandOnQueue[p1.Key] += 1;
                    }
                    else
                    {
                        RaiseHandOnQueue[p1.Key] = 1;
                    }

                    RaiseHandOffQueue[p1.Key] = 0;
                    foreach (KeyValuePair<string, int> item in RaiseHandOnQueue)
                    {
                        if (item.Value >= 3)
                        {
                            if (RaiseHandResult.Contains(item.Key))
                            {
                                continue;
                            }
                            else
                            {
                                RaiseRightHandEventOn(item.Key);
                                RaiseHandResult.Add(item.Key);
                            }
                        }
                    }
                }
                else
                {
                    if (RaiseHandResult.Contains(p1.Key))
                    {
                        if (RaiseHandOffQueue.ContainsKey(p1.Key))
                        {
                            RaiseHandOffQueue[p1.Key] += 1;
                        }
                        else
                        {
                            RaiseHandOffQueue[p1.Key] = 1;
                        }

                        if (RaiseHandOffQueue[p1.Key] >= 3)
                        {
                            RaiseRightHandEventOff(p1.Key);
                            RaiseHandResult.Remove(p1.Key);
                        }
                    }

                    RaiseHandOnQueue[p1.Key] = 0;
                }
            }
        }

        private void judgeHighFive()
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

        private float GetAncherThreshold(BodyDataSource p1, BodyDataSource p2)
        {
            return GetLowestShoulder(p1, p2);
        }

        private bool IsHighFiveOnHappened(BodyDataSource p1, BodyDataSource p2)
        {
            if (Single)
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
                if (leftDistance < logThreshold && rightDistance < logThreshold) 
                {
                    MDebug.Log("high-five distance left: " + leftDistance + " right: " + rightDistance);
                }
                if (leftDistance < HighFiveHandDistanceOffThreshold && rightDistance < HighFiveHandDistanceOffThreshold)
                {
                    if ((leftDistance+rightDistance)*0.5f <= HighFiveHandDistanceOnThreshold)
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
            if (handDistance < logThreshold) 
            {
                MDebug.Log("single high-five distance " + handDistance);
                if (handDistance <= HighFiveHandDistanceOnThreshold)
                {
                    result = true;
                }
            }
            
            return result;
        }

        private bool IsHighFiveOffHappened(BodyDataSource p1, BodyDataSource p2)
        {
            if (Single)
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
            
            if (handDistance > HighFiveHandDistanceOffThreshold)
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