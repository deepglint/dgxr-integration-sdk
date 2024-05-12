using System;
using System.Collections.Generic;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Log;
using Deepglint.XR.Ros;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Deepglint.XR
{
    public class DGXRManager: MonoBehaviour
    {
        public bool isFilterZero;
        private DGXRNode _node;
        private ROS2UnityManager _ros;
        private WsPoseAdapter ws;
        public void Awake()
        {
            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.SystemName = SystemInfo.operatingSystem; 
            Global.Config = new Config().InitConfig();
            GameLogger.Init(Global.Config.Log);
            if (Global.SystemName.Contains("Mac"))
            {
                ws = new WsPoseAdapter();
                ws.Start();
            }
            else
            {
                _ros = new ROS2UnityManager();
                _ros.Start();
            }
        }

        public void Start()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _node = new DGXRNode();
            }
            Global.IsFilterZero = isFilterZero;
        }

        public void Update()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _ros.FixedUpdate();
                _node.InitNode(_ros);
            }
        }
        
        private void OnEnable()
        {
            Global.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
            Global.OnMetaPoseDataLost += OnMetaPoseDataLost;
        }

        // 在禁用对象时取消订阅事件
        private void OnDisable()
        {
            Global.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
            Global.OnMetaPoseDataLost -= OnMetaPoseDataLost;
        }
        
        public void OnDestroy()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _ros.OnApplicationQuit();
            }
            else
            {
                ws.OnDestroy();
            }
        }

        private void OnMetaPoseDataLost(string key)
        {
            DeviceManager.RemoveDevice(key);
        }
        
        private void OnMetaPoseDataReceived(Source.SourceData data)
        {
            InputDevice device = DeviceManager.AddOrActiveDevice(data.BodyId, nameof(DGXRController));
            if (device != null)
            {
                var xrDevice = device as DGXRController;
                if (xrDevice == null) return;
                using (StateEvent.From(xrDevice, out var eventPtr))
                {
                    HandleActionsData(data.Actions, xrDevice, eventPtr);
                    HandleJointsData(data.Joints, xrDevice, eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }
        
        private Quaternion GetQuaternion(Source.JointData data)
        {
            Vector3 hip = new Vector3(
                (data.LeftHip.x + data.RightHip.x) * 0.5f,
                (data.LeftHip.y + data.RightHip.y) * 0.5f,
                (data.LeftHip.z + data.RightHip.z) * 0.5f
            );
            Vector3 forward = -Vector3.Cross(data.LeftShoulder - hip, data.RightShoulder - hip).normalized;
            return Quaternion.LookRotation(forward);
        }

        private void HandleJointsData(Source.JointData data, DGXRController xrDevice, InputEventPtr eventPtr)
        {
            xrDevice.HumanPose.IsTracked.WriteValueIntoEvent(1.0f, eventPtr);
            xrDevice.HumanPose.TrackingState.WriteValueIntoEvent((int)(InputTrackingState.Position | InputTrackingState.Rotation), eventPtr);
            xrDevice.HumanPose.Position.WriteValueIntoEvent(new Vector3(
                (data.LeftHip.x + data.RightHip.x) * 0.5f,
                (data.LeftHip.y + data.RightHip.y) * 0.5f,
                (data.LeftHip.z + data.RightHip.z) * 0.5f
                ), eventPtr);
            
            xrDevice.HumanPose.Rotation.WriteValueIntoEvent(GetQuaternion(data), eventPtr);
            
            xrDevice.HumanBody.HeadTop.position.WriteValueIntoEvent(data.HeadTop, eventPtr);
            xrDevice.HumanBody.Nose.position.WriteValueIntoEvent(data.Nose, eventPtr); 
            xrDevice.HumanBody.LeftEye.position.WriteValueIntoEvent(data.LeftEye, eventPtr); 
            xrDevice.HumanBody.RightEye.position.WriteValueIntoEvent(data.RightEye, eventPtr); 
            xrDevice.HumanBody.LeftEar.position.WriteValueIntoEvent(data.LeftEar, eventPtr); 
            xrDevice.HumanBody.RightEar.position.WriteValueIntoEvent(data.RightEar, eventPtr); 
            xrDevice.HumanBody.LeftShoulder.position.WriteValueIntoEvent(data.LeftShoulder, eventPtr); 
            xrDevice.HumanBody.RightShoulder.position.WriteValueIntoEvent(data.RightShoulder, eventPtr); 
            xrDevice.HumanBody.LeftElbow.position.WriteValueIntoEvent(data.LeftElbow, eventPtr); 
            xrDevice.HumanBody.RightElbow.position.WriteValueIntoEvent(data.RightElbow, eventPtr); 
            xrDevice.HumanBody.LeftWrist.position.WriteValueIntoEvent(data.LeftWrist, eventPtr); 
            xrDevice.HumanBody.RightWrist.position.WriteValueIntoEvent(data.RightWrist, eventPtr); 
            xrDevice.HumanBody.LeftHip.position.WriteValueIntoEvent(data.LeftHip, eventPtr); 
            xrDevice.HumanBody.RightHip.position.WriteValueIntoEvent(data.RightHip, eventPtr); 
            xrDevice.HumanBody.LeftKnee.position.WriteValueIntoEvent(data.LeftKnee, eventPtr); 
            xrDevice.HumanBody.RightKnee.position.WriteValueIntoEvent(data.RightKnee, eventPtr); 
            xrDevice.HumanBody.LeftAnkle.position.WriteValueIntoEvent(data.LeftAnkle, eventPtr); 
            xrDevice.HumanBody.RightAnkle.position.WriteValueIntoEvent(data.RightAnkle, eventPtr); 
            xrDevice.HumanBody.LeftTiptoe.position.WriteValueIntoEvent(data.LeftTiptoe, eventPtr); 
            xrDevice.HumanBody.RightTiptoe.position.WriteValueIntoEvent(data.RightTiptoe, eventPtr); 
            xrDevice.HumanBody.LeftHeel.position.WriteValueIntoEvent(data.LeftHeel, eventPtr); 
            xrDevice.HumanBody.RightHeel.position.WriteValueIntoEvent(data.RightHeel, eventPtr); 
            xrDevice.HumanBody.LeftHand.position.WriteValueIntoEvent(data.LeftHand, eventPtr); 
            xrDevice.HumanBody.RightHand.position.WriteValueIntoEvent(data.RightHand, eventPtr); 
            xrDevice.HumanBody.LeftFoot.position.WriteValueIntoEvent(new Vector3(
                (data.LeftTiptoe.x + data.LeftHeel.x) * 0.5f,
                (data.LeftTiptoe.y + data.LeftHeel.y) * 0.5f,
                Math.Min(data.LeftTiptoe.z, data.LeftHeel.z)
                ), eventPtr);
            xrDevice.HumanBody.RightFoot.position.WriteValueIntoEvent(new Vector3(
                (data.RightTiptoe.x + data.RightHeel.x) * 0.5f,
                (data.RightTiptoe.y + data.RightHeel.y) * 0.5f,
                Math.Min(data.RightTiptoe.z, data.RightHeel.z)
                ), eventPtr);
        }

        private void HandleActionsData(Dictionary<ActionType, float> actions, DGXRController xrDevice, InputEventPtr eventPtr)
        {
            foreach (var action in actions)
            {
                switch (action.Key)
                {
                    case ActionType.FastRun:
                        xrDevice.HighKneeRun.WriteValueIntoEvent(action.Value, eventPtr);
                        break;
                    case ActionType.ButterflySwim:
                        xrDevice.ButterflySwim.WriteValueIntoEvent(action.Value, eventPtr);
                        break;
                    case ActionType.FreestyleSwim:
                        xrDevice.FreeSwim.WriteValueIntoEvent(action.Value, eventPtr);
                        break;
                    case ActionType.DeepSquat:
                        xrDevice.DeepSquat.WriteValueIntoEvent(action.Value, eventPtr);
                        break;
                }
            }
        }
    }
}