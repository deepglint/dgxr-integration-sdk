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
        private readonly Record _record = new(Global.Config.Record.SavePath,Global.Config.Record.CacheFrameSize,"Ros2");

        public void DealMsg(std_msgs.msg.String msg)
        {
            List<Source.SourceData> data = new List<Source.SourceData>();
            MetaPoseData info = JsonConvert.DeserializeObject<MetaPoseData>(msg.Data);
            if (info.Result!=null&&info.Result.TryGetValue("999001", out  Result result))
            {
                if (result == null)
                {
                    Debug.LogError("991 result is null");
                    return;
                }
                ThreadPool.QueueUserWorkItem(_record.SaveMsgData, msg.Data);
                
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

                    Source.JointData joints = new Source.JointData();
                    if (val.Value.Objs.Count>=24)
                    {
                        joints = new Source.JointData
                        {
                             Nose = UnifyCoordinate(val.Value.Objs[0].value),
                             LeftEye = UnifyCoordinate(val.Value.Objs[1].value),
                             RightEye = UnifyCoordinate(val.Value.Objs[2].value),
                             LeftEar = UnifyCoordinate(val.Value.Objs[3].value),
                             RightEar = UnifyCoordinate(val.Value.Objs[4].value),
                             LeftShoulder = UnifyCoordinate(val.Value.Objs[5].value),
                             RightShoulder = UnifyCoordinate(val.Value.Objs[6].value),
                             LeftElbow = UnifyCoordinate(val.Value.Objs[7].value),
                             RightElbow = UnifyCoordinate(val.Value.Objs[8].value),
                             LeftWrist = UnifyCoordinate(val.Value.Objs[9].value),
                             RightWrist = UnifyCoordinate(val.Value.Objs[10].value),
                             LeftHip = UnifyCoordinate(val.Value.Objs[11].value),
                             RightHip = UnifyCoordinate(val.Value.Objs[12].value),
                             LeftKnee = UnifyCoordinate(val.Value.Objs[13].value),
                             RightKnee = UnifyCoordinate(val.Value.Objs[14].value),
                             LeftAnkle = UnifyCoordinate(val.Value.Objs[15].value),
                             RightAnkle = UnifyCoordinate(val.Value.Objs[16].value),
                             LeftTiptoe = UnifyCoordinate(val.Value.Objs[17].value),
                             RightTiptoe = UnifyCoordinate(val.Value.Objs[18].value),
                             LeftHeel = UnifyCoordinate(val.Value.Objs[19].value),
                             RightHeel = UnifyCoordinate(val.Value.Objs[20].value),
                             HeadTop = UnifyCoordinate(val.Value.Objs[21].value),
                             LeftHand = UnifyCoordinate(val.Value.Objs[22].value),
                             RightHand = UnifyCoordinate(val.Value.Objs[23].value),
                        };
                    }
                    var body = new Source.SourceData{
                        FrameId = info.FrameId,
                        Action = action, 
                        Joints = joints,
                    };
                    Global.TriggerMetaPoseDataReceived(body);
                    data.Add(body);
                }
            }
            Source.Data = data;
        }
        
        private Vector3 UnifyCoordinate(List<double> pose)
        {
            switch (Global.Config.Space.XDirection, Global.Config.Space.ZDirection)
            {
                case ("left", "up"):
                    return new Vector3((float)-pose[0],(float)pose[2], (float)pose[1] );
                case ("left", "down"):
                    return new Vector3((float)-pose[0],(float)pose[2], (float)-pose[1]);
                case ("right", "up"):
                    return new Vector3( (float)pose[0],(float)pose[2], (float)pose[1]);
                case ("right", "down"):
                    return new Vector3( (float)pose[0], (float)pose[2],(float)-pose[1]);
                case ("up", "left"):
                    return new Vector3((float)pose[1], (float)pose[2],(float)-pose[0]);
                case ("up", "right"):
                    return new Vector3((float)-pose[1], (float)pose[2],(float)pose[0]);
                case ("down", "left"):
                    return new Vector3( (float)-pose[1], (float)pose[2],(float)-pose[0]);
                case ("down", "right"):
                    return new Vector3( (float)pose[1], (float)pose[2],(float)-pose[0]);
                default:
                    return new Vector3( (float)pose[0], (float)pose[2],(float)pose[1]);
            }
        }
    }
}