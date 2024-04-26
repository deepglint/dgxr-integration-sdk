using System.Collections.Generic;
using System.Collections.Concurrent;
using Moat;
using System;
using Deepglint.XR.Inputs;
using UnityEngine;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Inputs.Devices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

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

        private Quaternion GetQuaternion(BodyDataSource data)
        {
            Vector3 leftShoulder = new Vector3(data.Joints[JointType.LeftShoulder].X, 
                data.Joints[JointType.LeftShoulder].Z,
                data.Joints[JointType.LeftShoulder].Y);
            Vector3 rightShoulder = new Vector3(data.Joints[JointType.RightShoulder].X, 
                data.Joints[JointType.RightShoulder].Z,
                data.Joints[JointType.RightShoulder].Y);
            Vector3 hip = new Vector3(
                (data.Joints[JointType.LeftHip].X + data.Joints[JointType.RightHip].X) * 0.5f,
                (data.Joints[JointType.LeftHip].Z + data.Joints[JointType.RightHip].Z) * 0.5f,
                (data.Joints[JointType.LeftHip].Y + data.Joints[JointType.RightHip].Y) * 0.5f
            );
            Vector3 forward = -Vector3.Cross(leftShoulder - hip, rightShoulder - hip).normalized;
            return Quaternion.LookRotation(forward);
        }

        private void OnBonesUpdate(string pId, BodyDataSource data)
        {
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
            if (device != null)
            {
                var xrDevice = device as DGXRController;
                if (xrDevice == null) return;
                using (StateEvent.From(xrDevice, out var eventPtr))
                {
                    // actions
                    var actions = XRDGBodySource.Instance.Actions[pId];
                    foreach (var action in actions)
                    {
                        switch (action.Key)
                        {
                            case "20":
                                xrDevice.HighKneeRun.WriteValueIntoEvent(action.Value, eventPtr);
                                Debug.LogFormat("fast_run action event");
                                break;
                            case "21":
                                xrDevice.ButterflySwim.WriteValueIntoEvent(action.Value, eventPtr);
                                Debug.LogFormat("butterfly_swim action event");
                                break;
                            case "22":
                                xrDevice.FreeSwim.WriteValueIntoEvent(action.Value, eventPtr);
                                Debug.LogFormat("free_swim action event");
                                // Debug.LogFormat("free_swim action event: {0}", action.Value);
                                break;
                            case "26":
                                xrDevice.DeepSquat.WriteValueIntoEvent(action.Value, eventPtr);
                                Debug.LogFormat("deep_squat action event");
                                break;
                        }
                    }
                    
                    // bones
                    xrDevice.HumanPose.isTracked.WriteValueIntoEvent(1.0f, eventPtr);
                    xrDevice.HumanPose.trackingState.WriteValueIntoEvent((int)(InputTrackingState.Position | InputTrackingState.Rotation), eventPtr);
                    xrDevice.HumanPose.position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.LeftHip].X + data.Joints[JointType.RightHip].X) * 0.5f,
                        (data.Joints[JointType.LeftHip].Z + data.Joints[JointType.RightHip].Z) * 0.5f,
                        (data.Joints[JointType.LeftHip].Y + data.Joints[JointType.RightHip].Y) * 0.5f
                    ), eventPtr);
                    xrDevice.HumanPose.rotation.WriteValueIntoEvent(GetQuaternion(data), eventPtr);
                    
                    xrDevice.HumanBody.headTop.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.HeadTop].X, 
                        data.Joints[JointType.HeadTop].Z,
                        data.Joints[JointType.HeadTop].Y
                    ), eventPtr);
                    xrDevice.HumanBody.nose.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.Nose].X, 
                        data.Joints[JointType.Nose].Z,
                        data.Joints[JointType.Nose].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftEye.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftEye].X, 
                        data.Joints[JointType.LeftEye].Z,
                        data.Joints[JointType.LeftEye].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightEye.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightEye].X, 
                        data.Joints[JointType.RightEye].Z,
                        data.Joints[JointType.RightEye].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftEar.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftEar].X, 
                        data.Joints[JointType.LeftEar].Z,
                        data.Joints[JointType.LeftEar].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightEar.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightEar].X, 
                        data.Joints[JointType.RightEar].Z,
                        data.Joints[JointType.RightEar].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftShoulder.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftShoulder].X, 
                        data.Joints[JointType.LeftShoulder].Z,
                        data.Joints[JointType.LeftShoulder].Y
                        ), eventPtr);
                    xrDevice.HumanBody.rightShoulder.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightShoulder].X, 
                        data.Joints[JointType.RightShoulder].Z,
                        data.Joints[JointType.RightShoulder].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftElbow.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftElbow].X, 
                        data.Joints[JointType.LeftElbow].Z,
                        data.Joints[JointType.LeftElbow].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightElbow.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightElbow].X, 
                        data.Joints[JointType.RightElbow].Z,
                        data.Joints[JointType.RightElbow].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftWrist.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftWrist].X, 
                        data.Joints[JointType.LeftWrist].Z,
                        data.Joints[JointType.LeftWrist].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightWrist.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightWrist].X, 
                        data.Joints[JointType.RightWrist].Z,
                        data.Joints[JointType.RightWrist].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftHip.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHip].X, 
                        data.Joints[JointType.LeftHip].Z,
                        data.Joints[JointType.LeftHip].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightHip.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHip].X, 
                        data.Joints[JointType.RightHip].Z,
                        data.Joints[JointType.RightHip].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftKnee.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftKnee].X, 
                        data.Joints[JointType.LeftKnee].Z,
                        data.Joints[JointType.LeftKnee].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightKnee.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightKnee].X, 
                        data.Joints[JointType.RightKnee].Z,
                        data.Joints[JointType.RightKnee].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftAnkle.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftAnkle].X, 
                        data.Joints[JointType.LeftAnkle].Z,
                        data.Joints[JointType.LeftAnkle].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightAnkle.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightAnkle].X, 
                        data.Joints[JointType.RightAnkle].Z,
                        data.Joints[JointType.RightAnkle].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftTiptoe.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftTiptoe].X, 
                        data.Joints[JointType.LeftTiptoe].Z,
                        data.Joints[JointType.LeftTiptoe].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightTiptoe.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightTiptoe].X, 
                        data.Joints[JointType.RightTiptoe].Z,
                        data.Joints[JointType.RightTiptoe].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftHeel.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHeel].X, 
                        data.Joints[JointType.LeftHeel].Z,
                        data.Joints[JointType.LeftHeel].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightHeel.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHeel].X, 
                        data.Joints[JointType.RightHeel].Z,
                        data.Joints[JointType.RightHeel].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftHand.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHand].X, 
                        data.Joints[JointType.LeftHand].Z,
                        data.Joints[JointType.LeftHand].Y
                    ), eventPtr);
                    xrDevice.HumanBody.rightHand.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHand].X, 
                        data.Joints[JointType.RightHand].Z,
                        data.Joints[JointType.RightHand].Y
                    ), eventPtr);
                    xrDevice.HumanBody.leftFoot.position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.LeftTiptoe].X+data.Joints[JointType.LeftHeel].X)*0.5f, 
                        (data.Joints[JointType.LeftTiptoe].Z+data.Joints[JointType.LeftHeel].Z)*0.5f,
                        Math.Min(data.Joints[JointType.LeftTiptoe].Y, data.Joints[JointType.LeftHeel].Y)
                    ), eventPtr);
                    xrDevice.HumanBody.rightFoot.position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.RightTiptoe].X+data.Joints[JointType.RightHeel].X)*0.5f, 
                        (data.Joints[JointType.RightTiptoe].Z+data.Joints[JointType.RightHeel].Z)*0.5f,
                        Math.Min(data.Joints[JointType.LeftTiptoe].Y, data.Joints[JointType.LeftHeel].Y)
                    ), eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }

        private void OnBonesUpdate1(string pId, BodyDataSource data)
        {
            unsafe
            {
                if (!DeviceManager.s_ActiveDevices.ContainsKey(pId))
                {
                    return;
                }

                var state = new DGXRControllerState();
                // state.buttons |= 1 << 0;
                state.humanPose = new HumanPoseState();
                state.humanPose.isTracked = true;
                state.humanPose.trackingState = InputTrackingState.Position | InputTrackingState.Rotation;
                state.humanPose.position= new Vector3(
                    (data.Joints[JointType.LeftHip].X + data.Joints[JointType.RightHip].X) * 0.5f,
                    (data.Joints[JointType.LeftHip].Z + data.Joints[JointType.RightHip].Z) * 0.5f,
                    (data.Joints[JointType.LeftHip].Y + data.Joints[JointType.RightHip].Y) * 0.5f
                );
                state.humanBody = new HumanBodyState();
                // state.humanBody.m_Head.position = new Vector3(data.Joints[JointType.HeadTop].X, data.Joints[JointType.HeadTop].Z, data.Joints[JointType.HeadTop].Y);
                // state.humanBody.m_Nose.position = new Vector3(data.Joints[JointType.Nose].X, data.Joints[JointType.Nose].Z, data.Joints[JointType.Nose].Y);
                // state.humanBody.leftEye.position = new Vector3(data.Joints[JointType.LeftEye].X, data.Joints[JointType.LeftEye].Z, data.Joints[JointType.LeftEye].Y);
                // state.humanBody.rightEye.position = new Vector3(data.Joints[JointType.RightEye].X, data.Joints[JointType.RightEye].Z, data.Joints[JointType.RightEye].Y);
                // state.humanBody.leftEar.position = new Vector3(data.Joints[JointType.LeftEar].X, data.Joints[JointType.LeftEar].Z, data.Joints[JointType.LeftEar].Y);
                // state.humanBody.rightEar.position = new Vector3(data.Joints[JointType.RightEar].X, data.Joints[JointType.RightEar].Z, data.Joints[JointType.RightEar].Y);
                state.humanBody.leftShoulder.position = new Vector3(data.Joints[JointType.LeftShoulder].X, data.Joints[JointType.LeftShoulder].Z, data.Joints[JointType.LeftShoulder].Y);
                state.humanBody.rightShoulder.position = new Vector3(data.Joints[JointType.RightShoulder].X, data.Joints[JointType.RightShoulder].Z, data.Joints[JointType.RightShoulder].Y);
                state.humanBody.leftElbow.position = new Vector3(data.Joints[JointType.LeftElbow].X, data.Joints[JointType.LeftElbow].Z, data.Joints[JointType.LeftElbow].Y);
                state.humanBody.rightElbow.position = new Vector3(data.Joints[JointType.RightElbow].X, data.Joints[JointType.RightElbow].Z, data.Joints[JointType.RightElbow].Y);
                state.humanBody.leftWrist.position = new Vector3(data.Joints[JointType.LeftWrist].X, data.Joints[JointType.LeftWrist].Z, data.Joints[JointType.LeftWrist].Y);
                state.humanBody.rightWrist.position = new Vector3(data.Joints[JointType.RightWrist].X, data.Joints[JointType.RightWrist].Z, data.Joints[JointType.RightWrist].Y);
                state.humanBody.leftHip.position = new Vector3(data.Joints[JointType.LeftHip].X, data.Joints[JointType.LeftHip].Z, data.Joints[JointType.LeftHip].Y);
                state.humanBody.rightHip.position = new Vector3(data.Joints[JointType.RightHip].X, data.Joints[JointType.RightHip].Z, data.Joints[JointType.RightHip].Y);
                state.humanBody.leftKnee.position = new Vector3(data.Joints[JointType.LeftKnee].X, data.Joints[JointType.LeftKnee].Z, data.Joints[JointType.LeftKnee].Y);
                state.humanBody.rightKnee.position = new Vector3(data.Joints[JointType.RightKnee].X, data.Joints[JointType.RightKnee].Z, data.Joints[JointType.RightKnee].Y);
                // state.humanBody.leftAnkle.position = new Vector3(data.Joints[JointType.LeftAnkle].X, data.Joints[JointType.LeftAnkle].Z, data.Joints[JointType.LeftAnkle].Y);
                // state.humanBody.rightAnkle.position = new Vector3(data.Joints[JointType.RightAnkle].X, data.Joints[JointType.RightAnkle].Z, data.Joints[JointType.RightAnkle].Y);
                // state.humanBody.leftTiptoe.position = new Vector3(data.Joints[JointType.LeftTiptoe].X, data.Joints[JointType.LeftTiptoe].Z, data.Joints[JointType.LeftTiptoe].Y);
                // state.humanBody.rightTiptoe.position = new Vector3(data.Joints[JointType.RightTiptoe].X, data.Joints[JointType.RightTiptoe].Z, data.Joints[JointType.RightTiptoe].Y);
                // state.humanBody.leftHeel.position = new Vector3(data.Joints[JointType.LeftHeel].X, data.Joints[JointType.LeftHeel].Z, data.Joints[JointType.LeftHeel].Y);
                // state.humanBody.rightHeel.position = new Vector3(data.Joints[JointType.RightHeel].X, data.Joints[JointType.RightHeel].Z, data.Joints[JointType.RightHeel].Y);
                // state.humanBody.m_HeadTop.position = new Vector3(data.Joints[JointType.HeadTop].X, data.Joints[JointType.HeadTop].Z, data.Joints[JointType.HeadTop].Y);
                state.humanBody.leftHand.position = new Vector3(data.Joints[JointType.LeftHand].X, data.Joints[JointType.LeftHand].Z, data.Joints[JointType.LeftHand].Y);
                state.humanBody.rightHand.position = new Vector3(data.Joints[JointType.RightHand].X, data.Joints[JointType.RightHand].Z, data.Joints[JointType.RightHand].Y);
                // state.humanBody.leftFoot.position = new Vector3(
                //     (data.Joints[JointType.LeftTiptoe].X + data.Joints[JointType.LeftHeel].X)*0.5f,
                //     (data.Joints[JointType.LeftTiptoe].Z + data.Joints[JointType.LeftHeel].Z)*0.5f,
                //     (data.Joints[JointType.LeftTiptoe].Y + data.Joints[JointType.LeftHeel].Y)*0.5f);
                // state.humanBody.rightFoot.position = new Vector3(
                //     (data.Joints[JointType.RightTiptoe].X + data.Joints[JointType.RightHeel].X)*0.5f,
                //     (data.Joints[JointType.RightTiptoe].Z + data.Joints[JointType.RightHeel].Z)*0.5f,
                //     (data.Joints[JointType.RightTiptoe].Y + data.Joints[JointType.RightHeel].Y)*0.5f);

                InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
                if (device != null)
                {
                    var xrDevice = (DGXRController)device;
                    // 分配一个StateEvent并设置其状态数据
                    // using (StateEvent.From(xrDevice, out var eventPtr))
                    // {
                    //     UnsafeUtility.MemCpy(eventPtr.ToPointer(), UnsafeUtility.AddressOf(ref state), UnsafeUtility.SizeOf<DGXRControllerState>());
                    //     // 将创建的事件加入到输入系统的事件队列中
                    //     InputSystem.QueueEvent(eventPtr);
                    // }
                    
                    // using (StateEvent eventPtr = StateEvent.From(xrDevice, out var ptr))
                    // {
                    //     UnsafeUtility.MemCpy(ptr, UnsafeUtility.AddressOf(ref state), UnsafeUtility.SizeOf<DGXRControllerState>());
                    //
                    //     // 将创建的事件加入到输入系统的事件队列中
                    //     InputSystem.QueueEvent(eventPtr);
                    // }
                    
                    // var state = new DGXRControllerState();
                    // state.buttons |= 1 << 0;
                    // state.position = new Vector3(
                    //     (data.Joints[JointType.LeftHip].X + data.Joints[JointType.RightHip].X) * 0.5f,
                    //     (data.Joints[JointType.LeftHip].Z + data.Joints[JointType.RightHip].Z) * 0.5f,
                    //     (data.Joints[JointType.LeftHip].Y + data.Joints[JointType.RightHip].Y) * 0.5f
                    // );
                    
                    using (StateEvent.From(xrDevice, out var eventPtr))
                    {
                        // xrDevice.position.WriteValueIntoEvent(new Vector3(0.1f, 0.1f, 0.1f), eventPtr);
                        // xrDevice.isTracked.WriteValueIntoEvent(1.0f, eventPtr);
                        // xrDevice.humanBody.WriteValueFromObjectIntoEvent(eventPtr, xrDevice.humanBody.ReadValueAsObject());
                        UnsafeUtility.MemCpy(eventPtr.ToPointer(), UnsafeUtility.AddressOf(ref state), UnsafeUtility.SizeOf<DGXRControllerState>());
                        InputSystem.QueueEvent(eventPtr);
                    }   
                    Debug.Log("update dgxr device");
                    //InputSystem.QueueStateEvent(device, state);
                    //Debug.Log("update bones for device " + device.deviceId);
                }
            }
        }

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
             
            //Debug.Log(pId + " x: "+ (float)state.x + ", y: " + (float)state.y);
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
            if (device != null)
            {
                InputSystem.QueueStateEvent(device, state); 
            }
        }
        
        private void RaiseRightHandEventOff(string pId)
        {
            MDebug.Log("raise hand off event occurs: " + pId);
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
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
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
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
                OnBonesUpdate(p1.Key, p1.Value);
                // OnBonesUpdate1(p1.Key, p1.Value);
                // OnPositionUpdate(p1.Key, p1.Value.GetRootPositionVector2());
            }
            //judgeRaiseHand();
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