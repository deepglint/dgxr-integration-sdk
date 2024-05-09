using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Runtime.Scripts.Ros
{
    [DataContract]
    public class MetaPoseData
    {
        [DataMember(Name = "frameId")] public string FrameId { get; set; }
        [DataMember(Name = "timeStamp")] public string TimeStamp { get; set; }
        [DataMember(Name = "result")] public Dictionary<string, Result> Result { get; set; }
        [DataMember(Name = "event")] public Event Event { get; set; }
    }

    public class RecActionInfo
    {
        [DataMember(Name = "action")] public int Action;
        [DataMember(Name = "confidence")] public float Confidence;
        [DataMember(Name = "tagnameid")] public int TagNameId;
    }

    public class Obj
    {
        [DataMember(Name = "value")] public List<float> Value { get; set; }
    }

    public class ThreeDim
    {
        [DataMember(Name = "objs")] public List<Obj> Objs { get; set; }
        [DataMember(Name = "recActions")] public List<RecActionInfo> RecActions { get; set; }
        [DataMember(Name = "uid")] public int Uid { get; set; }
    }

    public class Result
    {
        [DataMember(Name = "threeDim")] public Dictionary<string, ThreeDim> ThreeDim { get; set; }
    }

    [DataContract]
    public class RecAction
    {
        [DataMember(Name = "action")] public int Action { get; set; }

        [DataMember(Name = "confidence")] public double Confidence { get; set; }

        [DataMember(Name = "tagnameid")] public int TagnameId { get; set; }
    }

    [DataContract]
    public class Event
    {
        [DataMember(Name = "matchedInfo")] public List<Dictionary<string, List<Value>>> MatchedInfo { get; set; }

        [DataMember(Name = "goalInfo")] public List<Dictionary<string, List<Value>>> GoalInfo { get; set; }

        [DataMember(Name = "throwInfo")] public List<Dictionary<string, List<Value>>> ThrowInfo { get; set; }

        [DataMember(Name = "scoreInfo")] public List<Dictionary<string, List<Value>>> ScoreInfo { get; set; }
    }

    [DataContract]
    public class Value
    {
        [DataMember(Name = "str")] public string Str { get; set; }

        [DataMember(Name = "int")] public int Int { get; set; }
    }

    public class Ros2PoseAdapter
    {
        private readonly Record _record = new(Global.Config.Record.SavePath,
            "Ros2");

        private readonly Dictionary<String, Source.SourceData> _oldData = new Dictionary<string, Source.SourceData>();

        public void DealMsg(std_msgs.msg.String msg)
        {
            DealMsgData(msg.Data);
        }

        public void DealMsgData(string msg)
        {
            List<Source.SourceData> data = new List<Source.SourceData>();
            MetaPoseData info = JsonConvert.DeserializeObject<MetaPoseData>(msg);
            if (info.Result != null && info.Result.TryGetValue("999001", out Result result))
            {
                if (result == null)
                {
                    Debug.LogError("991 result is null");
                    return;
                }

                if (!Global.SystemName.Contains("Mac"))
                {
                    ThreadPool.QueueUserWorkItem(_record.SaveMsgData, msg);
                }

                foreach (var val in result.ThreeDim)
                {
                    var action = new Dictionary<Action.ActionType, float>();
                    if (val.Value is { RecActions: not null })
                    {
                        foreach (var act in val.Value.RecActions)
                        {
                            action[(Action.ActionType)act.Action] = act.Confidence;
                        }
                    }

                    Source.JointData joints = new Source.JointData();
                    if (val.Value.Objs.Count >= 24)
                    {
                        for (var i = 0; i < 24; i++)
                        {
                            var pose = val.Value.Objs[i].Value;
                            (bool isZero, Source.SourceData sourceData) = IsZero(pose, val.Key);
                            switch ((Joint)i)
                            {
                                case Joint.Nose:
                                    if (isZero)
                                    {
                                        joints.Nose = sourceData.Joints.Nose;
                                        continue;
                                    }

                                    joints.Nose = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftEye:
                                    if (isZero)
                                    {
                                        joints.LeftEye = sourceData.Joints.LeftEye;
                                        continue;
                                    }

                                    joints.LeftEye = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightEye:
                                    if (isZero)
                                    {
                                        joints.RightEye = sourceData.Joints.RightEye;
                                        continue;
                                    }

                                    joints.RightEye = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftEar:
                                    if (isZero)
                                    {
                                        joints.LeftEar = sourceData.Joints.LeftEar;
                                        continue;
                                    }

                                    joints.LeftEar = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightEar:
                                    if (isZero)
                                    {
                                        joints.RightEar = sourceData.Joints.RightEar;
                                        continue;
                                    }

                                    joints.RightEar = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftShoulder:
                                    if (isZero)
                                    {
                                        joints.LeftShoulder = sourceData.Joints.LeftShoulder;
                                        continue;
                                    }

                                    joints.LeftShoulder = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightShoulder:
                                    if (isZero)
                                    {
                                        joints.RightShoulder = sourceData.Joints.RightShoulder;
                                        continue;
                                    }

                                    joints.RightShoulder = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftElbow:
                                    if (isZero)
                                    {
                                        joints.LeftElbow = sourceData.Joints.LeftElbow;
                                        continue;
                                    }

                                    joints.LeftElbow = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightElbow:
                                    if (isZero)
                                    {
                                        joints.RightElbow = sourceData.Joints.RightElbow;
                                        continue;
                                    }

                                    joints.RightElbow = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftWrist:
                                    if (isZero)
                                    {
                                        joints.LeftWrist = sourceData.Joints.LeftWrist;
                                        continue;
                                    }

                                    joints.LeftWrist = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightWrist:
                                    if (isZero)
                                    {
                                        joints.RightWrist = sourceData.Joints.RightWrist;
                                        continue;
                                    }

                                    joints.RightWrist = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHip:
                                    if (isZero)
                                    {
                                        joints.LeftHip = sourceData.Joints.LeftHip;
                                        continue;
                                    }

                                    joints.LeftHip = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHip:
                                    if (isZero)
                                    {
                                        joints.RightHip = sourceData.Joints.RightHip;
                                        continue;
                                    }

                                    joints.RightHip = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftKnee:
                                    if (isZero)
                                    {
                                        joints.LeftKnee = sourceData.Joints.LeftKnee;
                                        continue;
                                    }

                                    joints.LeftKnee = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightKnee:
                                    if (isZero)
                                    {
                                        joints.RightKnee = sourceData.Joints.RightKnee;
                                        continue;
                                    }

                                    joints.RightKnee = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftAnkle:
                                    if (isZero)
                                    {
                                        joints.LeftAnkle = sourceData.Joints.LeftAnkle;
                                        continue;
                                    }

                                    joints.LeftAnkle = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightAnkle:
                                    if (isZero)
                                    {
                                        joints.RightAnkle = sourceData.Joints.RightAnkle;
                                        continue;
                                    }

                                    joints.RightAnkle = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftTiptoe:
                                    if (isZero)
                                    {
                                        joints.LeftTiptoe = sourceData.Joints.LeftTiptoe;
                                        continue;
                                    }

                                    joints.LeftTiptoe = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightTiptoe:
                                    if (isZero)
                                    {
                                        joints.RightTiptoe = sourceData.Joints.RightTiptoe;
                                        continue;
                                    }

                                    joints.RightTiptoe = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHeel:
                                    if (isZero)
                                    {
                                        joints.LeftHeel = sourceData.Joints.LeftHeel;
                                        continue;
                                    }

                                    joints.LeftHeel = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHeel:
                                    if (isZero)
                                    {
                                        joints.RightHeel = sourceData.Joints.RightHeel;
                                        continue;
                                    }

                                    joints.RightHeel = UnifyCoordinate(pose);
                                    break;
                                case Joint.HeadTop:
                                    if (isZero)
                                    {
                                        joints.HeadTop = sourceData.Joints.HeadTop;
                                        continue;
                                    }

                                    joints.HeadTop = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHand:
                                    if (isZero)
                                    {
                                        joints.LeftHand = sourceData.Joints.LeftHand;
                                        continue;
                                    }

                                    joints.LeftHand = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHand:
                                    if (isZero)
                                    {
                                        joints.RightHand = sourceData.Joints.RightHand;
                                        continue;
                                    }

                                    joints.RightHand = UnifyCoordinate(pose);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    var body = new Source.SourceData
                    {
                        FrameId = info.FrameId,
                        BodyId = val.Key,
                        Action = action,
                        Joints = joints,
                    };
                    Global.TriggerMetaPoseDataReceived(body);
                    data.Add(body);
                    if (Global.IsFilterZero)
                    {
                        _oldData[val.Key] = body;
                    }
                }
            }

            Source.Data = data;
        }


        private (bool, Source.SourceData) IsZero(IReadOnlyList<float> pose, string key)
        {
            if (Global.IsFilterZero && pose[0] == 0 && pose[1] == 0 && pose[2] == 0 &&
                _oldData.TryGetValue(key, out var sourceData))
            {
                return (true, sourceData);
            }

            return (false, new Source.SourceData());
        }

        private Vector3 UnifyCoordinate(List<float> pose)
        {
            switch (Global.Config.Space.XDirection, Global.Config.Space.ZDirection)
            {
                case ("left", "up"):
                    return new Vector3(-pose[0], pose[2], pose[1]);
                case ("left", "down"):
                    return new Vector3(-pose[0], pose[2], -pose[1]);
                case ("right", "up"):
                    return new Vector3(pose[0], pose[2], pose[1]);
                case ("right", "down"):
                    return new Vector3(pose[0], pose[2], -pose[1]);
                case ("up", "left"):
                    return new Vector3(pose[1], pose[2], -pose[0]);
                case ("up", "right"):
                    return new Vector3(-pose[1], pose[2], pose[0]);
                case ("down", "left"):
                    return new Vector3(-pose[1], pose[2], -pose[0]);
                case ("down", "right"):
                    return new Vector3(pose[1], pose[2], -pose[0]);
                default:
                    return new Vector3(pose[0], pose[2], pose[1]);
            }
        }
    }
}