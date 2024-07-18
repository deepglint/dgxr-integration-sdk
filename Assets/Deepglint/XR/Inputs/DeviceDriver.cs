using System;
using System.Collections.Generic;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Source;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;

namespace Deepglint.XR.Inputs
{
    public class DeviceDriver : MonoBehaviour
    {
        private static readonly Queue<Action> ExecuteRosMsgEventMainThreadQueue = new Queue<Action>();
            
        private static void ExecuteRosMsgEventInUpdate(Action action)
        {
            lock (ExecuteRosMsgEventMainThreadQueue)
            {
                ExecuteRosMsgEventMainThreadQueue.Enqueue(action);
            }
        }

        private void OnEnable()
        {
            Source.Source.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost += OnMetaPoseDataLost;
            PseudoOfflineFilter.OnPersonOffline += OnPersonOffline;
        }
        
        private void OnDisable()
        {
            Source.Source.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost -= OnMetaPoseDataLost;
            PseudoOfflineFilter.OnPersonOffline -= OnPersonOffline;
        }

        private void Update()
        {
            while (ExecuteRosMsgEventMainThreadQueue.Count > 0)
            {
                Action action;
                lock (ExecuteRosMsgEventMainThreadQueue)
                {
                    action = ExecuteRosMsgEventMainThreadQueue.Dequeue();
                }
                action?.Invoke();
            }
        }
        
        private void OnMetaPoseDataLost(string key)
        {
            if (PseudoOfflineFilter.Enabled)
            {
                return;
            }
            if (Source.Source.DataFrom == SourceType.ROS)
            {
                ExecuteRosMsgEventInUpdate(() =>
                {
                    DeviceManager.RemoveDevice(key);
                });
            }
            else
            {
                DeviceManager.RemoveDevice(key);
            }
        }

        private void OnPersonOffline(string key)
        {
            DeviceManager.RemoveDevice(key); 
        }
        
        private void OnMetaPoseDataReceived(SourceData data)
        {
            if (Source.Source.DataFrom == SourceType.ROS)
            {
                ExecuteRosMsgEventInUpdate(() =>
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
                (data.LeftHip.LocalPosition.x + data.RightHip.LocalPosition.x) * 0.5f,
                (data.LeftHip.LocalPosition.y + data.RightHip.LocalPosition.y) * 0.5f,
                (data.LeftHip.LocalPosition.z + data.RightHip.LocalPosition.z) * 0.5f
            );
            Vector3 forward = -Vector3.Cross(data.LeftShoulder.LocalPosition - hip, data.RightShoulder.LocalPosition - hip).normalized;
            return Quaternion.LookRotation(forward);
        }
        
        private void HandleJointsData(JointData data, DGXRHumanController xrDevice, InputEventPtr eventPtr)
        {
            xrDevice.HumanPose.IsTracked.WriteValueIntoEvent(1.0f, eventPtr);
            xrDevice.HumanPose.TrackingState.WriteValueIntoEvent((int)(InputTrackingState.Position | InputTrackingState.Rotation), eventPtr);
            Vector3 humanPosition = new Vector3(
                (data.LeftHip.LocalPosition.x + data.RightHip.LocalPosition.x) * 0.5f,
                (data.LeftHip.LocalPosition.y + data.RightHip.LocalPosition.y) * 0.5f,
                (data.LeftHip.LocalPosition.z + data.RightHip.LocalPosition.z) * 0.5f
            );
            xrDevice.HumanPose.Position.WriteValueIntoEvent(humanPosition, eventPtr);

            xrDevice.HumanPose.Rotation.WriteValueIntoEvent(GetQuaternion(data), eventPtr);

            xrDevice.HumanBody.HeadTop.position.WriteValueIntoEvent(data.HeadTop.LocalPosition, eventPtr);
            xrDevice.HumanBody.Nose.position.WriteValueIntoEvent(data.Nose.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftEye.position.WriteValueIntoEvent(data.LeftEye.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightEye.position.WriteValueIntoEvent(data.RightEye.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftEar.position.WriteValueIntoEvent(data.LeftEar.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightEar.position.WriteValueIntoEvent(data.RightEar.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftShoulder.position.WriteValueIntoEvent(data.LeftShoulder.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightShoulder.position.WriteValueIntoEvent(data.RightShoulder.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftElbow.position.WriteValueIntoEvent(data.LeftElbow.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightElbow.position.WriteValueIntoEvent(data.RightElbow.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftWrist.position.WriteValueIntoEvent(data.LeftWrist.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightWrist.position.WriteValueIntoEvent(data.RightWrist.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftHip.position.WriteValueIntoEvent(data.LeftHip.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightHip.position.WriteValueIntoEvent(data.RightHip.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftKnee.position.WriteValueIntoEvent(data.LeftKnee.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightKnee.position.WriteValueIntoEvent(data.RightKnee.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftAnkle.position.WriteValueIntoEvent(data.LeftAnkle.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightAnkle.position.WriteValueIntoEvent(data.RightAnkle.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftTiptoe.position.WriteValueIntoEvent(data.LeftTiptoe.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightTiptoe.position.WriteValueIntoEvent(data.RightTiptoe.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftHeel.position.WriteValueIntoEvent(data.LeftHeel.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightHeel.position.WriteValueIntoEvent(data.RightHeel.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftHand.position.WriteValueIntoEvent(data.LeftHand.LocalPosition, eventPtr);
            xrDevice.HumanBody.RightHand.position.WriteValueIntoEvent(data.RightHand.LocalPosition, eventPtr);
            xrDevice.HumanBody.LeftFoot.position.WriteValueIntoEvent(new Vector3(
                (data.LeftTiptoe.LocalPosition.x + data.LeftHeel.LocalPosition.x) * 0.5f,
                Math.Min(data.LeftTiptoe.LocalPosition.y, data.LeftHeel.LocalPosition.y),
                (data.LeftTiptoe.LocalPosition.z + data.LeftHeel.LocalPosition.z) * 0.5f
                ), eventPtr);
            xrDevice.HumanBody.RightFoot.position.WriteValueIntoEvent(new Vector3(
                (data.RightTiptoe.LocalPosition.x + data.RightHeel.LocalPosition.x) * 0.5f,
                Math.Min(data.RightTiptoe.LocalPosition.y, data.RightHeel.LocalPosition.y),
                (data.RightTiptoe.LocalPosition.z + data.RightHeel.LocalPosition.z) * 0.5f
                ), eventPtr);

            if (xrDevice.Anchor != null)
            {
                Vector3 position = humanPosition;
                Vector2 point = new Vector2(position.x, position.z);
                Vector2 anchorPoint = new Vector2(xrDevice.Anchor.Point.x, xrDevice.Anchor.Point.z);
                if (Vector2.Distance(point, anchorPoint) < xrDevice.Anchor.Radius)
                {
                    xrDevice.Stick.WriteValueIntoEvent(Vector2.zero, eventPtr);
                }
                else
                {
                    Vector2 direction = (point - anchorPoint).normalized;
                    xrDevice.Stick.WriteValueIntoEvent(direction, eventPtr);
                }
            }
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