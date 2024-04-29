using System;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Controls;
using Deepglint.XR.Inputs.Devices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Stardust.Scripts
{
    public static class WSSourceAdapter
    {
        private static float _positionThresholdMin = 0.5f;
        private static Quaternion GetQuaternion(BodyDataSource data)
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

        private static void OnBonesUpdate(string pId, BodyDataSource data)
        {
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
            if (device != null)
            {
                var xrDevice = device as DGXRController;
                if (xrDevice == null) return;
                using (StateEvent.From(xrDevice, out var eventPtr))
                {
                    // actions
                    var actions = XrdgBodySource.Instance.Actions[pId];
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
                    xrDevice.HumanPose.IsTracked.WriteValueIntoEvent(1.0f, eventPtr);
                    xrDevice.HumanPose.TrackingState.WriteValueIntoEvent(
                        (int)(InputTrackingState.Position | InputTrackingState.Rotation), eventPtr);
                    xrDevice.HumanPose.Position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.LeftHip].X + data.Joints[JointType.RightHip].X) * 0.5f,
                        (data.Joints[JointType.LeftHip].Z + data.Joints[JointType.RightHip].Z) * 0.5f,
                        (data.Joints[JointType.LeftHip].Y + data.Joints[JointType.RightHip].Y) * 0.5f
                    ), eventPtr);
                    xrDevice.HumanPose.Rotation.WriteValueIntoEvent(GetQuaternion(data), eventPtr);

                    xrDevice.HumanBody.HeadTop.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.HeadTop].X,
                        data.Joints[JointType.HeadTop].Z,
                        data.Joints[JointType.HeadTop].Y
                    ), eventPtr);
                    xrDevice.HumanBody.Nose.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.Nose].X,
                        data.Joints[JointType.Nose].Z,
                        data.Joints[JointType.Nose].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftEye.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftEye].X,
                        data.Joints[JointType.LeftEye].Z,
                        data.Joints[JointType.LeftEye].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightEye.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightEye].X,
                        data.Joints[JointType.RightEye].Z,
                        data.Joints[JointType.RightEye].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftEar.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftEar].X,
                        data.Joints[JointType.LeftEar].Z,
                        data.Joints[JointType.LeftEar].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightEar.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightEar].X,
                        data.Joints[JointType.RightEar].Z,
                        data.Joints[JointType.RightEar].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftShoulder.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftShoulder].X,
                        data.Joints[JointType.LeftShoulder].Z,
                        data.Joints[JointType.LeftShoulder].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightShoulder.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightShoulder].X,
                        data.Joints[JointType.RightShoulder].Z,
                        data.Joints[JointType.RightShoulder].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftElbow.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftElbow].X,
                        data.Joints[JointType.LeftElbow].Z,
                        data.Joints[JointType.LeftElbow].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightElbow.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightElbow].X,
                        data.Joints[JointType.RightElbow].Z,
                        data.Joints[JointType.RightElbow].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftWrist.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftWrist].X,
                        data.Joints[JointType.LeftWrist].Z,
                        data.Joints[JointType.LeftWrist].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightWrist.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightWrist].X,
                        data.Joints[JointType.RightWrist].Z,
                        data.Joints[JointType.RightWrist].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftHip.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHip].X,
                        data.Joints[JointType.LeftHip].Z,
                        data.Joints[JointType.LeftHip].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightHip.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHip].X,
                        data.Joints[JointType.RightHip].Z,
                        data.Joints[JointType.RightHip].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftKnee.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftKnee].X,
                        data.Joints[JointType.LeftKnee].Z,
                        data.Joints[JointType.LeftKnee].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightKnee.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightKnee].X,
                        data.Joints[JointType.RightKnee].Z,
                        data.Joints[JointType.RightKnee].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftAnkle.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftAnkle].X,
                        data.Joints[JointType.LeftAnkle].Z,
                        data.Joints[JointType.LeftAnkle].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightAnkle.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightAnkle].X,
                        data.Joints[JointType.RightAnkle].Z,
                        data.Joints[JointType.RightAnkle].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftTiptoe.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftTiptoe].X,
                        data.Joints[JointType.LeftTiptoe].Z,
                        data.Joints[JointType.LeftTiptoe].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightTiptoe.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightTiptoe].X,
                        data.Joints[JointType.RightTiptoe].Z,
                        data.Joints[JointType.RightTiptoe].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftHeel.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHeel].X,
                        data.Joints[JointType.LeftHeel].Z,
                        data.Joints[JointType.LeftHeel].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightHeel.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHeel].X,
                        data.Joints[JointType.RightHeel].Z,
                        data.Joints[JointType.RightHeel].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftHand.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.LeftHand].X,
                        data.Joints[JointType.LeftHand].Z,
                        data.Joints[JointType.LeftHand].Y
                    ), eventPtr);
                    xrDevice.HumanBody.RightHand.position.WriteValueIntoEvent(new Vector3(
                        data.Joints[JointType.RightHand].X,
                        data.Joints[JointType.RightHand].Z,
                        data.Joints[JointType.RightHand].Y
                    ), eventPtr);
                    xrDevice.HumanBody.LeftFoot.position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.LeftTiptoe].X + data.Joints[JointType.LeftHeel].X) * 0.5f,
                        (data.Joints[JointType.LeftTiptoe].Z + data.Joints[JointType.LeftHeel].Z) * 0.5f,
                        Math.Min(data.Joints[JointType.LeftTiptoe].Y, data.Joints[JointType.LeftHeel].Y)
                    ), eventPtr);
                    xrDevice.HumanBody.RightFoot.position.WriteValueIntoEvent(new Vector3(
                        (data.Joints[JointType.RightTiptoe].X + data.Joints[JointType.RightHeel].X) * 0.5f,
                        (data.Joints[JointType.RightTiptoe].Z + data.Joints[JointType.RightHeel].Z) * 0.5f,
                        Math.Min(data.Joints[JointType.LeftTiptoe].Y, data.Joints[JointType.LeftHeel].Y)
                    ), eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }

        public static void OnFrame()
        {
            foreach (var p1 in XrdgBodySource.Instance.Data)
            {
                OnBonesUpdate(p1.Key, p1.Value);
            }
        }
        
        private static void OnStateUpdate(string pId, BodyDataSource data)
        {
            unsafe
            {
                if (!DeviceManager.m_ActiveDevices.ContainsKey(pId))
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
        
        private static void OnPositionUpdate(string pId, Vector2 v2)
        {
            Vector2 position = new Vector2(v2.x + 1f, v2.y + 1f);
            var state = new DGXRControllerState();
            if (Math.Abs(v2.x) <= _positionThresholdMin)
            {
                state.x = (byte)(127.5f);
            } else if (v2.x >= _positionThresholdMin + 1f)
            {
                state.x = (byte)(255);
            } else if (v2.x <= -1 - _positionThresholdMin)
            {
                state.x = (byte)(0);
            }
            else if (v2.x > 0)
            {
                state.x = (byte)((v2.x + 1f - _positionThresholdMin) * 127.5f);
            }
            else
            {
                state.x = (byte)((v2.x + 1f + _positionThresholdMin) * 127.5f); 
            }
            
            if (Math.Abs(v2.y) <= _positionThresholdMin)
            {
                state.y = (byte)(127.5f);
            } else if (v2.y >= _positionThresholdMin + 1f)
            {
                state.y = (byte)(255);
            } else if (v2.y <= -1 - _positionThresholdMin)
            {
                state.y = (byte)(0);
            }
            else if (v2.y > 0)
            {
                state.y = (byte)((v2.y + 1f - _positionThresholdMin) * 127.5f);
            }
            else
            {
                state.y = (byte)((v2.y +1f + _positionThresholdMin) * 127.5f); 
            }
             
            //Debug.Log(pId + " x: "+ (float)state.x + ", y: " + (float)state.y);
            InputDevice device = DeviceManager.GetActiveDeviceBySerial(pId);
            if (device != null)
            {
                InputSystem.QueueStateEvent(device, state); 
            }
        }
    }
}