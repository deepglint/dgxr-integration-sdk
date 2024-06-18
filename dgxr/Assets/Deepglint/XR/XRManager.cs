using System;
using System.Collections.Generic;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Log;
using Deepglint.XR.Ros;
using Deepglint.XR.Source;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;

namespace Deepglint.XR
{
    [DefaultExecutionOrder(-100)]
    public class XRManager : MonoBehaviour
    {
        public bool isFilterZero;
        private DGXRNode _node;
        private ROS2UnityManager _ros;
        private static readonly Queue<Action> ExecuteOnMainThreadQueue = new Queue<Action>();

        private static void ExecuteDataLostActionInUpdate(Action action)
        {
            lock (ExecuteOnMainThreadQueue)
            {
                ExecuteOnMainThreadQueue.Enqueue(action);
            }
        }


        public void Awake()
        {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.Version = Application.version;
            Global.SystemName = SystemInfo.operatingSystem;
            Global.Config = new Config.Config().InitConfig();
            GameLogger.Init(Global.Config.Log);
            if (UseRos())
            {
                var ros = Extends.FindChildGameObject(gameObject,"RosConnect" );
                ros.SetActive(true);
            }
            else
            {
                var ws = Extends.FindChildGameObject(gameObject,"WsConnect" );
                ws.SetActive(true);
            }
        }

        public void Start()
        {
            if (UseRos())
            {
                _node = new DGXRNode();
            }

            Global.Space = XRSpace.Instance;
            Global.IsFilterZero = isFilterZero;
        }

        public void Update()
        {
            if (UseRos())
            {
                while (ExecuteOnMainThreadQueue.Count > 0)
                {
                    Action action;
                    lock (ExecuteOnMainThreadQueue)
                    {
                        action = ExecuteOnMainThreadQueue.Dequeue();
                    }
                    action?.Invoke();
                }
            }
        }

        private bool UseRos()
        {
            if (!Application.isEditor && !Global.SystemName.Contains("Mac"))
            {
                return true;
            }

            return false;
        }
        
        // TODO @张梦豪 把inputSystem相关逻辑单独拆成脚本和预制体
        private void OnEnable()
        {
            Source.Source.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost += OnMetaPoseDataLost;
        }

        // 在禁用对象时取消订阅事件
        // TODO @张梦豪 把inputSystem相关逻辑单独拆成脚本和预制体
        private void OnDisable()
        {
            Source.Source.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost -= OnMetaPoseDataLost;
        }


        public void OnDestroy()
        {
            GameLogger.Cleanup();
        }

        private void OnMetaPoseDataLost(string key)
        {
            if (UseRos())
            {
                ExecuteDataLostActionInUpdate(() =>
                {
                    DeviceManager.RemoveDevice(key);
                });
            }
            else
            {
                DeviceManager.RemoveDevice(key);
            }
        }

        private void OnMetaPoseDataReceived(SourceData data)
        {
            if (UseRos())
            {
                ExecuteDataLostActionInUpdate(() =>
                {
                    HandleMetaPoseData(data);
                });
            }
            else
            {
                HandleMetaPoseData(data);
            }
        }

        private void HandleMetaPoseData(SourceData data)
        {
            var device = DeviceManager.AddOrActiveDevice(data.BodyId, nameof(DGXRHumanController));
            if (device != null)
            {
                if (!Application.isEditor && !Application.isFocused)
                {
                    return;
                }
                var xrDevice = device as DGXRHumanController;
                if (xrDevice == null) return;
                using (StateEvent.From(xrDevice, out var eventPtr))
                {
                    HandleActionsData(data.Actions, xrDevice, eventPtr);
                    HandleJointsData(data.Joints, xrDevice, eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }

        private Quaternion GetQuaternion(JointData data)
        {
            Vector3 hip = new Vector3(
                (data.LeftHip.x + data.RightHip.x) * 0.5f,
                (data.LeftHip.y + data.RightHip.y) * 0.5f,
                (data.LeftHip.z + data.RightHip.z) * 0.5f
            );
            Vector3 forward = -Vector3.Cross(data.LeftShoulder - hip, data.RightShoulder - hip).normalized;
            return Quaternion.LookRotation(forward);
        }

        private void HandleJointsData(JointData data, DGXRHumanController xrDevice, InputEventPtr eventPtr)
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
                Math.Min(data.LeftTiptoe.y, data.LeftHeel.y),
                (data.LeftTiptoe.z + data.LeftHeel.z) * 0.5f
                ), eventPtr);
            xrDevice.HumanBody.RightFoot.position.WriteValueIntoEvent(new Vector3(
                (data.RightTiptoe.x + data.RightHeel.x) * 0.5f,
                Math.Min(data.RightTiptoe.y, data.RightHeel.y),
                (data.RightTiptoe.z + data.RightHeel.z) * 0.5f
                ), eventPtr);
        }

        private void HandleActionsData(Dictionary<ActionType, float> actions, DGXRHumanController xrDevice, InputEventPtr eventPtr)
        {
            if (!actions.ContainsKey(ActionType.Jump))
            {
                xrDevice.Jump.WriteValueIntoEvent(0f, eventPtr);
            }
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
                    case ActionType.Jump:
                        xrDevice.Jump.WriteValueIntoEvent(action.Value, eventPtr);
                        break;
                }
            }
        }
    }
}
