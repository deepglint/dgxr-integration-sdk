using System.Collections.Concurrent;
using System.Collections.Generic;
using Deepglint.XR.EventSystem.EventData;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Source;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Deepglint.XR.EventSystem.InputModules
{
    public sealed class HumanControlInteractionInputModule : MonoBehaviour
    {
        [SerializeField]
        public float HighFiveOnDistanceThreshold = 0.09f;
        [SerializeField]
        public float HighFiveOffHandDistanceThreshold = 0.099f;
        [SerializeField]
        public int HighFiveHitThreshold = 3;
        
        private float _logThreshold = 0.2f;
        private ConcurrentDictionary<string, int> _highFiveOnQueue = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> _highFiveOffQueue = new ConcurrentDictionary<string, int>();
        private HashSet<string> _highFiveResult = new HashSet<string>();
        
        private void OnEnable()
        {
            Source.Source.OnMetaPoseFrameDataReceived += OnMetaPoseFrameDataReceived;
        }

        private void OnDisable()
        {
            Source.Source.OnMetaPoseFrameDataReceived -= OnMetaPoseFrameDataReceived;
        }

        private float GetAnchorThreshold(SourceData p1, SourceData p2)
        {
            return GetLowestShoulder(p1, p2);
        }

        private bool IsHighFiveOnHappened(SourceData p1, SourceData p2)
        {
            bool result = false;
            float anchorThreshold = GetAnchorThreshold(p1, p2);
            
            if (p1.Joints.LeftHand.y < anchorThreshold && p1.Joints.RightHand.y < anchorThreshold)
            {
                return false;
            }
            
            if (p2.Joints.LeftHand.y < anchorThreshold && p2.Joints.RightHand.y < anchorThreshold)
            {
                return false;
            }
            
            float left1 = Vector3.Distance(p1.Joints.LeftHand, p2.Joints.RightHand);
            float left2 = Vector3.Distance(p1.Joints.LeftHand, p2.Joints.LeftHand);
            float leftDistance = left2 < left1 ? left2 : left1;
            float right1 = Vector3.Distance(p1.Joints.RightHand, p2.Joints.RightHand);
            float right2 = Vector3.Distance(p1.Joints.RightHand, p2.Joints.LeftHand);
            float rightDistance = right2 < right1 ? right2 : right1;
            float handDistance = rightDistance < leftDistance ? rightDistance : leftDistance;
            
            if (handDistance <= HighFiveOnDistanceThreshold)
            {
                result = true;
            }
            
            return result;
        }

        private bool IsHighFiveOffHappened(SourceData p1, SourceData p2)
        {
            bool result = false;
            float left1 = Vector3.Distance(p1.Joints.LeftHand, p2.Joints.RightHand);
            float left2 = Vector3.Distance(p1.Joints.LeftHand, p2.Joints.LeftHand);
            float leftDistance = left2 < left1 ? left2 : left1;
            float right1 = Vector3.Distance(p1.Joints.RightHand, p2.Joints.RightHand);
            float right2 = Vector3.Distance(p1.Joints.RightHand, p2.Joints.LeftHand);
            float rightDistance = right2 < right1 ? right2 : right1;
            float handDistance = rightDistance < leftDistance ? rightDistance : leftDistance;
            
            if (handDistance > HighFiveOffHandDistanceThreshold) 
            {
                result = true;
            }
            
            return result;
        }

        private float GetLowestShoulder(SourceData p1, SourceData p2)
        {
            float shoulder1 = p1.Joints.LeftShoulder.y <= p1.Joints.RightShoulder.y ? 
                p1.Joints.LeftShoulder.y : p1.Joints.RightShoulder.y;
            float shoulder2 = p2.Joints.LeftShoulder.y <= p2.Joints.RightShoulder.y ? 
                p2.Joints.LeftShoulder.y : p2.Joints.RightShoulder.y;
            return shoulder1 <= shoulder2 ? shoulder1 : shoulder2;
        }

        
        
        private void OnMetaPoseFrameDataReceived(List<SourceData> data)
        {
            foreach (var p1 in data)
            {
                var device1 = DeviceManager.GetActiveDeviceBySerial(p1.BodyId) as DGXRHumanController;
                if (device1 == null)
                {
                    continue;
                }
                foreach (var p2 in data)
                {
                    var device2 = DeviceManager.GetActiveDeviceBySerial(p2.BodyId) as DGXRHumanController;
                    if (device2 == null)
                    {
                        continue;
                    }
                    if (device2.deviceId <= device1.deviceId)
                    {
                        continue;
                    }

                    if (PlayerInput.FindFirstPairedToDevice(device1) == null ||
                        PlayerInput.FindFirstPairedToDevice(device2) == null)
                    {
                        continue;
                    }
                    
                    string key = p1.BodyId + "_" + p2.BodyId;
                    if (IsHighFiveOnHappened(p1, p2))
                    {
                        if (_highFiveOnQueue.ContainsKey(key))
                        {
                            _highFiveOnQueue[key] += 1;
                        } else
                        {
                            _highFiveOnQueue[key] = 1;
                        }
                        _highFiveOffQueue[key] = 0;
                    }
                    else
                    {
                        if (_highFiveResult.Contains(key))
                        {
                            if(IsHighFiveOffHappened(p1, p2))
                            {
                                if (_highFiveOffQueue.ContainsKey(key))
                                {
                                    _highFiveOffQueue[key] += 1;
                                } else
                                {
                                    _highFiveOffQueue[key] = 1;
                                }
                                if (_highFiveOffQueue[key] >= HighFiveHitThreshold) 
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
                if (item.Value >= HighFiveHitThreshold)
                {
                    if (!_highFiveResult.Contains(item.Key))
                    {
                        string[] persons = item.Key.Split("_");
                        // trigger high-five event
                        // Debug.Log("trigger high-five event");
                        DGXRHumanController xrDevice1 = DeviceManager.GetActiveDeviceBySerial(persons[0]) as DGXRHumanController;
                        DGXRHumanController xrDevice2 = DeviceManager.GetActiveDeviceBySerial(persons[1]) as DGXRHumanController;
                        if (xrDevice1 != null && xrDevice2 != null)
                        {
                            TriggerHighFiveEvent(xrDevice1, xrDevice2);
                        }
                        _highFiveResult.Add(item.Key);
                    } 
                }
            }
        }

        private void TriggerHighFiveEvent(DGXRHumanController device1, DGXRHumanController device2)
        {
            var players = device1.GetAllPairedPlayers();
            foreach (var player in players)
            {
                var targets = device2.GetAllPairedPlayers();
                foreach (var target in targets)
                {
                    HumanInteractionEventData eventData1 =
                        new HumanInteractionEventData(UnityEngine.EventSystems.EventSystem.current, target);
                    ExecuteEvents.Execute<IHighFiveEventHandler>(
                        player,
                        eventData1,
                        (handler, data) => handler.OnHighFiveEvent((HumanInteractionEventData)data)
                    ); 
                    HumanInteractionEventData eventData2 =
                        new HumanInteractionEventData(UnityEngine.EventSystems.EventSystem.current, player);
                    ExecuteEvents.Execute<IHighFiveEventHandler>(
                        target,
                        eventData2,
                        (handler, data) => handler.OnHighFiveEvent((HumanInteractionEventData)data)
                    ); 
                }
            }
        }
    }
}