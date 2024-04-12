using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using Stardust.log;

namespace DGXR
{
    [DataContract]
    public class MetaPoseData
    {
        [DataMember(Name = "frameId")]
        public string FrameId { get; set; }
        [DataMember(Name = "timeStamp")]
        public string TimeStamp { get; set; }
        [DataMember(Name = "result")]
        public Dictionary<string, Result> Result { get; set; }
        [DataMember(Name = "event")]
        public Event Event { get; set; }
    }
    
    public class Obj
    {
        public List<double> value { get; set; }
    }

    public class RecActionInfo
    {
        [DataMember(Name = "action")]
        public int Action;
        [DataMember(Name = "confidence")]
        public float Confidence;
        [DataMember(Name = "tagnameid")]
        public int TagNameId;
    }

    public class ThreeDim
    {
        [DataMember(Name = "objs")]
        public List<Obj> Objs { get; set; } 
        [DataMember(Name = "recActions")]
        public List<RecActionInfo> RecActions { get; set; }
        [DataMember(Name = "uid")]
        public int Uid { get; set; }
    }

    public class Result
    {
        [DataMember(Name = "threeDim")]
        public Dictionary<string, ThreeDim> ThreeDim { get; set; }
    }

    [DataContract]
    public class RecAction
    {
        [DataMember(Name = "action")]
        public int Action { get; set; }

        [DataMember(Name = "confidence")]
        public double Confidence { get; set; }

        [DataMember(Name = "tagnameid")]
        public int TagnameId { get; set; }
    }

    [DataContract]
    public class Event
    {
        [DataMember(Name = "matchedInfo")]
        public List<Dictionary<string, List<Value>>> MatchedInfo { get; set; }

        [DataMember(Name = "goalInfo")]
        public List<Dictionary<string, List<Value>>> GoalInfo { get; set; }

        [DataMember(Name = "throwInfo")]
        public List<Dictionary<string, List<Value>>> ThrowInfo { get; set; }

        [DataMember(Name = "scoreInfo")]
        public List<Dictionary<string, List<Value>>> ScoreInfo { get; set; }
    }

    [DataContract]
    public class Value
    {
        [DataMember(Name = "str")]
        public string Str { get; set; }

        [DataMember(Name = "int")]
        public int Int { get; set; }
    }
    
    public class Ros2PoseAdapter
    {
        readonly Source _source = Source.Instance;
        private readonly Record _record = new(Global.Config.Record.SavePath,Global.Config.Record.CacheFrameSize,"Ros2");

        public void DealMsg(std_msgs.msg.String msg)
        {
            ThreadPool.QueueUserWorkItem(_record.SaveMsgData, msg.Data);
            MetaPoseData info = JsonConvert.DeserializeObject<MetaPoseData>(msg.Data);
            if (info.Result!=null&&info.Result.TryGetValue("999001", out  Result result))
            {
                if (result == null)
                {
                    Debug.LogError("991 result is null");
                    return;
                }
                foreach (var val in result.ThreeDim)
                {
                    var action = new Dictionary<Action.ActionType, float>();
                    if (val.Value != null && val.Value.RecActions != null)
                    {
                        foreach (var act in val.Value.RecActions)
                        {
                            action[(Action.ActionType)act.Action] = act.Confidence;
                        } 
                    }
                    var joints = new Dictionary<JointType, JointData>();
                    for (int i = 0; i < val.Value.Objs.Count; i++)
                    {
                        var pose = val.Value.Objs[i];
                        if (Global.IsFilterZero && pose.value[0]==0 && pose.value[1] == 0 && pose.value[2] == 0)
                        {
                            var bodyData = _source.GetData();
                            if (bodyData.TryGetValue(val.Key, out var bodyInfo)&& bodyInfo!=null)
                            {
                                var bodyDataSource = bodyInfo.GetLatest();
                                if (bodyDataSource.Joints != null)
                                {
                                    joints[(JointType)i] = bodyDataSource.Joints[(JointType)i];
                                    continue;
                                }
                            }
                        }

                        var unifyCoordinate = UnifyCoordinate(Global.Config.Space.XDirection,Global.Config.Space.XDirection,pose.value);
                        joints[(JointType)i] = new JointData(unifyCoordinate[0], unifyCoordinate[2], unifyCoordinate[1]);
                    }
                    
                    Source.BodyDataSource body = new Source.BodyDataSource{
                        FrameId = info.FrameId,
                        Action = action, 
                        Joints = joints,
                    }; 
                    _source.SetData(val.Key,body);
                }
                // 遍历 bodyData 的 key 在当前帧存不存在，不存在直接删除
                foreach (var data in _source.GetData())
                {
                    if (!result.ThreeDim.TryGetValue(data.Key, out ThreeDim dim))
                    {
                        _source.DelData(data.Key);
                    }
                }
            }
        }
        
        private double[] UnifyCoordinate(string xDirection, string yDirection, List<double> pose)
        {
            switch (xDirection.ToLower(), yDirection.ToLower())
            {
                case ("left", "up"):
                    return new [] { -pose[0], pose[1], pose[2] };
                case ("left", "down"):
                    return new [] { -pose[0], -pose[1], pose[2] };
                case ("right", "up"):
                    return new [] { pose[0], pose[1], pose[2] };
                case ("right", "down"):
                    return new [] { pose[0], -pose[1], pose[2] };
                case ("up", "left"):
                    return new [] { pose[1], -pose[0], pose[2] };
                case ("up", "right"):
                    return new [] { -pose[1], pose[0], pose[2] };
                case ("down", "left"):
                    return new [] { -pose[1], -pose[0], pose[2] };
                case ("down", "right"):
                    return new [] { pose[1], -pose[0], pose[2] };
                default:
                    return new [] { pose[0], pose[1], pose[2] };
            }
        }
    }
}