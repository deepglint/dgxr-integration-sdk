using System;
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Runtime.Samples.CustomPlayerManager
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
    }
}
