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
        
        private static readonly Queue<Action> ExecuteOnMainThreadQueue = new Queue<Action>();
            
        private static void ExecuteDataLostActionInUpdate(Action action)
        {
            lock (ExecuteOnMainThreadQueue)
            {
                ExecuteOnMainThreadQueue.Enqueue(action);
            }
        }

        private void OnEnable()
        {
            Source.Source.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost += OnMetaPoseDataLost;
        }
        
        private void OnDisable()
        {
            Source.Source.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
            Source.Source.OnMetaPoseDataLost -= OnMetaPoseDataLost;
        }

        private void Update()
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
        
        private void OnMetaPoseDataLost(string key)
        {
            if (Source.Source.DataFrom == SourceType.ROS)
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
            if (Source.Source.DataFrom == SourceType.ROS)
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
            Vector3 humanPosition = new Vector3(
                (data.LeftHip.x + data.RightHip.x) * 0.5f,
                (data.LeftHip.y + data.RightHip.y) * 0.5f,
                (data.LeftHip.z + data.RightHip.z) * 0.5f
            );
            xrDevice.HumanPose.Position.WriteValueIntoEvent(humanPosition, eventPtr);

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

            if (xrDevice.Anchor != null)
            {
                Vector2 point = new Vector2(humanPosition.x, humanPosition.z);
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