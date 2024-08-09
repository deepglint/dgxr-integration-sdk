using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Deepglint.XR.Source;
using Deepglint.XR.Toolkit.Utils;
using Newtonsoft.Json;
using UnityEditor;
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
        private readonly Record _record = new(DGXR.Config.Record.SavePath,
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
            long frameId = long.Parse(info.FrameId);
            if (info.Result != null && info.Result.TryGetValue("999001", out Result result))
            {
                if (result == null)
                {
                    Debug.LogError("991 result is null");
                    return;
                }

                if (!DGXR.SystemName.Contains("Mac"))
                {
                    ThreadPool.QueueUserWorkItem(_record.SaveMsgData, msg);
                }

                foreach (var val in result.ThreeDim)
                {
                    string bodyId = val.Key;
                    if (PseudoOfflineFilter.ChangeLog.TryGetValue(val.Key, out var personFeature))
                    {
                        bodyId =  personFeature.BodyId;
                    }
                    humans.Add(bodyId);
                    var actions = new Dictionary<ActionType, float>();
                    if (val.Value is { RecActions: not null })
                    {
                        foreach (var act in val.Value.RecActions)
                        {
                            actions[(ActionType)act.Action] = act.Confidence;
                        }
                    }

                    JointData joints = new JointData();
                    if (val.Value.Objs.Count >= 24)
                    {
                        for (var i = 0; i < 24; i++)
                        {
                            var pose = val.Value.Objs[i].Value;
                            (bool isZero, SourceData sourceData) = IsZero(pose, bodyId);
                            switch ((Joint)i)
                            {
                                case Joint.Nose:
                                    if (isZero)
                                    {
                                        joints.Nose.LocalPosition = sourceData.Joints.Nose.LocalPosition;
                                        continue;
                                    }

                                    joints.Nose.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftEye:
                                    if (isZero)
                                    {
                                        joints.LeftEye.LocalPosition = sourceData.Joints.LeftEye.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftEye.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightEye:
                                    if (isZero)
                                    {
                                        joints.RightEye.LocalPosition = sourceData.Joints.RightEye.LocalPosition;
                                        continue;
                                    }

                                    joints.RightEye.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftEar:
                                    if (isZero)
                                    {
                                        joints.LeftEar.LocalPosition = sourceData.Joints.LeftEar.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftEar.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightEar:
                                    if (isZero)
                                    {
                                        joints.RightEar.LocalPosition = sourceData.Joints.RightEar.LocalPosition;
                                        continue;
                                    }

                                    joints.RightEar.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftShoulder:
                                    if (isZero)
                                    {
                                        joints.LeftShoulder.LocalPosition = sourceData.Joints.LeftShoulder.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftShoulder.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightShoulder:
                                    if (isZero)
                                    {
                                        joints.RightShoulder.LocalPosition = sourceData.Joints.RightShoulder.LocalPosition;
                                        continue;
                                    }

                                    joints.RightShoulder.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftElbow:
                                    if (isZero)
                                    {
                                        joints.LeftElbow.LocalPosition = sourceData.Joints.LeftElbow.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftElbow.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightElbow:
                                    if (isZero)
                                    {
                                        joints.RightElbow.LocalPosition = sourceData.Joints.RightElbow.LocalPosition;
                                        continue;
                                    }

                                    joints.RightElbow.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftWrist:
                                    if (isZero)
                                    {
                                        joints.LeftWrist.LocalPosition = sourceData.Joints.LeftWrist.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftWrist.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightWrist:
                                    if (isZero)
                                    {
                                        joints.RightWrist.LocalPosition = sourceData.Joints.RightWrist.LocalPosition;
                                        continue;
                                    }

                                    joints.RightWrist.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHip:
                                    if (isZero)
                                    {
                                        joints.LeftHip.LocalPosition = sourceData.Joints.LeftHip.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftHip.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHip:
                                    if (isZero)
                                    {
                                        joints.RightHip.LocalPosition = sourceData.Joints.RightHip.LocalPosition;
                                        continue;
                                    }

                                    joints.RightHip.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftKnee:
                                    if (isZero)
                                    {
                                        joints.LeftKnee.LocalPosition = sourceData.Joints.LeftKnee.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftKnee.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightKnee:
                                    if (isZero)
                                    {
                                        joints.RightKnee.LocalPosition = sourceData.Joints.RightKnee.LocalPosition;
                                        continue;
                                    }

                                    joints.RightKnee.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftAnkle:
                                    if (isZero)
                                    {
                                        joints.LeftAnkle.LocalPosition = sourceData.Joints.LeftAnkle.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftAnkle.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightAnkle:
                                    if (isZero)
                                    {
                                        joints.RightAnkle.LocalPosition = sourceData.Joints.RightAnkle.LocalPosition;
                                        continue;
                                    }

                                    joints.RightAnkle.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftTiptoe:
                                    if (isZero)
                                    {
                                        joints.LeftTiptoe.LocalPosition = sourceData.Joints.LeftTiptoe.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftTiptoe.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightTiptoe:
                                    if (isZero)
                                    {
                                        joints.RightTiptoe.LocalPosition = sourceData.Joints.RightTiptoe.LocalPosition;
                                        continue;
                                    }

                                    joints.RightTiptoe.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHeel:
                                    if (isZero)
                                    {
                                        joints.LeftHeel.LocalPosition = sourceData.Joints.LeftHeel.LocalPosition;
                                        continue;
                                    }

                                    joints.LeftHeel.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHeel:
                                    if (isZero)
                                    {
                                        joints.RightHeel.LocalPosition = sourceData.Joints.RightHeel.LocalPosition;
                                        continue;
                                    }

                                    joints.RightHeel.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.HeadTop:
                                    if (isZero)
                                    {
                                        joints.HeadTop.LocalPosition = sourceData.Joints.HeadTop.LocalPosition;
                                        continue;
                                    }
                                    joints.HeadTop.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.LeftHand:
                                    if (isZero)
                                    {
                                        joints.LeftHand = sourceData.Joints.LeftHand;
                                        continue;
                                    }

                                    joints.LeftHand.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                case Joint.RightHand:
                                    if (isZero)
                                    {
                                        joints.RightHand.LocalPosition = sourceData.Joints.RightHand.LocalPosition;
                                        continue;
                                    }

                                    joints.RightHand.LocalPosition = UnifyCoordinate(pose);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                    var body = new SourceData
                    {
                        FrameId = frameId,
                        BodyId = bodyId,
                        Actions = actions,
                        Joints = joints,
                    };
                    data[bodyId] = body;
                    if (DGXR.IsFilterZero)
                    {
                        _oldData[bodyId] = body;
                    }
                }
            }
            
            foreach (var human in Source.Source.Data)
            {
                if (!humans.Contains(human.BodyId))
                {
                    SourceMainThreadDispatcher.Enqueue(() =>
                    {
                        Source.Source.DelData(human.BodyId); 
                        Source.Source.TriggerMetaPostDataLost(human.BodyId);
                    });
                }
            }

            List<string> keys = new List<string>(data.Keys);
            foreach (var key in keys)
            {
                SourceMainThreadDispatcher.Enqueue(() =>
                {
                    var sourceData = data[key];
                    if (PseudoOfflineFilter.Instance.Filter(ref sourceData))
                    {
                        data.Remove(key);
                        Source.Source.DelData(key);
                        data[sourceData.BodyId] = sourceData;
                    } 
                    Source.Source.SetData(sourceData);
                    Source.Source.TriggerMetaPoseDataReceived(sourceData);
                });
                Source.Source.TriggerRealTimePoseReceived( data[key]);
            }
            SourceMainThreadDispatcher.Enqueue(() =>
            {
                Source.Source.TriggerMetaPoseFrameDataReceived(frameId,data.Values.ToList());
            }); 
            Source.Source.TriggerRealTimePoseFrameReceived(frameId,data.Values.ToList());
        }

        /// <summary>
        /// 出去骨骼抖动零点
        /// </summary>
        /// <param name="pose">人体骨骼点</param>
        /// /// <param name="BodyId">人员 id</param>  
        private (bool, SourceData) IsZero(IReadOnlyList<float> pose, string BodyId)
        {
            if (DGXR.IsFilterZero && pose[0] == 0 && pose[1] == 0 && pose[2] == 0 &&
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
            Vector3 pose =Vector3.zero;
            if (DGXR.Space != null)
            {
                var size = DGXR.Space.Size;
                poseList[0] = (size.x / DGXR.Space.RealSize.x) *
                    poseList[0];
                poseList[1] = (size.z / DGXR.Space.RealSize.z) *
                              poseList[1]; 
                poseList[2] = (size.y / DGXR.Space.RealSize.y) *
                              poseList[2];  
                pose = new Vector3(poseList[0], poseList[2], poseList[1]);
            }
           
            switch (DGXR.Config.Space.XDirection, DGXR.Config.Space.ZDirection)
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