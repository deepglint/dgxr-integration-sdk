using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Deepglint.XR.Source;
using Deepglint.XR.Toolkit.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Joint = Deepglint.XR.Source.Joint;

namespace Deepglint.XR.Ros
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

    /// <summary>
    /// ros2人体骨骼适配器，处理算法推出人体关节点的消息映射和转换
    /// </summary>
    public class Ros2PoseAdapter
    {
        private readonly Record _record = new(Global.Config.Record.SavePath,
            "Ros2");

        private readonly Dictionary<String, Source.SourceData> _oldData = new Dictionary<string, Source.SourceData>();
        
        public void DealMsg(std_msgs.msg.String msg)
        {
            DealMsgData(msg.Data);
        }
        
        /// <summary>
        /// 人体骨骼消息处理
        /// </summary>
        /// <param name="msg">ros 接收到的string消息</param> 
        public void DealMsgData(string msg)
        {
            Dictionary<string, SourceData> data = new Dictionary<string, SourceData>();
            MetaPoseData info = JsonConvert.DeserializeObject<MetaPoseData>(msg);
            HashSet<string> humans = new HashSet<string>();
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
                    humans.Add(val.Key);
                    var action = new Dictionary<ActionType, float>();
                    if (val.Value is { RecActions: not null })
                    {
                        foreach (var act in val.Value.RecActions)
                        {
                            action[(ActionType)act.Action] = act.Confidence;
                        }
                    }

                    JointData joints = new JointData();
                    if (val.Value.Objs.Count >= 24)
                    {
                        for (var i = 0; i < 24; i++)
                        {
                            var pose = val.Value.Objs[i].Value;
                            (bool isZero, SourceData sourceData) = IsZero(pose, val.Key);
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
                    var body = new SourceData
                    {
                        FrameId = info.FrameId,
                        BodyId = val.Key,
                        Actions = action,
                        Joints = joints,
                    };
                    Source.Source.TriggerMetaPoseDataReceived(body);
                    data[val.Key] = body;
                    if (Global.IsFilterZero)
                    {
                        _oldData[val.Key] = body;
                    }
                }
            }

            foreach (var human in Source.Source.Data)
            {
                if (!humans.Contains(human.BodyId))
                {
                    Source.Source.TriggerMetaPostDataLost(human.BodyId);
                  
                }
            }

            Source.Source.SetData(data); 
            Source.Source.TriggerMetaPoseFrameDataReceived(data.Values.ToList());
        }

        /// <summary>
        /// 出去骨骼抖动零点
        /// </summary>
        /// <param name="pose">人体骨骼点</param>
        /// /// <param name="BodyId">人员 id</param>  
        private (bool, SourceData) IsZero(IReadOnlyList<float> pose, string BodyId)
        {
            if (Global.IsFilterZero && pose[0] == 0 && pose[1] == 0 && pose[2] == 0 &&
                _oldData.TryGetValue(BodyId, out var sourceData))
            {
                return (true, sourceData);
            }

            return (false, new SourceData());
        }
        
        /// <summary>
        /// 坐标标准化处理
        /// </summary>
        /// <param name="pose">人体骨骼点</param>
        private Vector3 UnifyCoordinate(List<float> poseList)
        {
            Vector3 pose =Global.Space.Origin;
            if (Global.Space != null)
            {
                var size = Global.Space.Size;
                poseList[0] = (size.x / Global.Space.RealSize.x) *
                    poseList[0];
                poseList[1] = (size.z / Global.Space.RealSize.z) *
                              poseList[1];  
                pose += new Vector3(poseList[0], poseList[2], poseList[1]);
            }
           
            switch (Global.Config.Space.XDirection, Global.Config.Space.ZDirection)
            {
                case ("left", "up"):
                    return new Vector3(-pose.x, pose.y, pose.z);
                case ("left", "down"):
                    return new Vector3(-pose.x, pose.y, -pose.z);
                case ("right", "up"):
                    return new Vector3(pose.x, pose.y, pose.z);
                case ("right", "down"):
                    return new Vector3(pose.x, pose.y, -pose.z);
                case ("up", "left"):
                    return new Vector3(pose.z, pose.y, -pose.x);
                case ("up", "right"):
                    return new Vector3(-pose.z, pose.y, pose.x);
                case ("down", "left"):
                    return new Vector3(-pose.z, pose.y, -pose.x);
                case ("down", "right"):
                    return new Vector3(pose.z, pose.y, -pose.x);
                default:
                    return new Vector3(pose.x, pose.y, pose.z);
            }
        }
    }
}