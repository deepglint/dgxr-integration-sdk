using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Deepglint.XR.Source
{
    public enum SourceType
    {
        ROS,
        WS,
    }

    public enum Joint
    {
        Nose,
        LeftEye,
        RightEye,
        LeftEar,
        RightEar,
        LeftShoulder,
        RightShoulder,
        LeftElbow,
        RightElbow,
        LeftWrist,
        RightWrist,
        LeftHip,
        RightHip,
        LeftKnee,
        RightKnee,
        LeftAnkle,
        RightAnkle,
        LeftTiptoe,
        RightTiptoe,
        LeftHeel,
        RightHeel,
        HeadTop,
        LeftHand,
        RightHand
    }

    public enum ActionType
    {
        RightHandDrawCircle = 1, //左手画圈
        LeftHandDrawCircle = 2, //右手画圈 
        Kick = 10, //踢腿
        CombineHandsStraight = 17, //双手伸直合并
        ThrowBoulder = 18, //举手投掷巨物 
        SlowRun = 19, //慢跑
        FastRun = 20, //快跑  
        ButterflySwim = 21, //蝶泳  
        FreestyleSwim = 22, //自由泳   
        KeepRaisingHand = 23, //持续举手   
        Applaud = 24, //拍掌   
        Jump = 25, //起跳   
        DeepSquat = 26, //下蹲   
        RaiseOnHand = 10000, //举单手
        RaiseBothHand = 10001, //举双手
        ArmFlat = 10002, //手臂平展 
        ArmFlatIsL = 10003, //手臂平展为 L 
        ArmVerticalIsL = 10004, //手臂垂直为 L 
    }

    public struct JointInfo
    {
        public Vector3 LocalPosition { get; internal set; }
        public Vector3 Position => LocalPosition + DGXR.Space.Origin;
    }

    public struct JointData
    {
        public JointInfo Nose;

        public JointInfo LeftEye;

        public JointInfo RightEye;

        public JointInfo LeftEar;

        public JointInfo RightEar;

        public JointInfo LeftShoulder;

        public JointInfo RightShoulder;

        public JointInfo LeftElbow;

        public JointInfo RightElbow;

        public JointInfo LeftWrist;

        public JointInfo RightWrist;

        public JointInfo LeftHip;

        public JointInfo RightHip;

        public JointInfo LeftKnee;

        public JointInfo RightKnee;

        public JointInfo LeftAnkle;

        public JointInfo RightAnkle;

        public JointInfo LeftTiptoe;

        public JointInfo RightTiptoe;

        public JointInfo LeftHeel;

        public JointInfo RightHeel;

        public JointInfo HeadTop;

        public JointInfo LeftHand;

        public JointInfo RightHand;
    }

    public struct SourceData
    {
        public long FrameId;
        public string BodyId;
        public Dictionary<ActionType, float> Actions;
        public JointData Joints;
    }


    public class Source : IEnumerable<SourceData>
    {
        private static Source _instance;
        private ConcurrentDictionary<string, SourceData> _dataDic;
        private List<string> _dataList;
        public static SourceType DataFrom;

        public delegate void MetaPoseDataEventHandler(SourceData data);
        public delegate void MetaPoseFrameDataEventHandler(long frameId, List<SourceData> data);
        
        public static  event MetaPoseDataEventHandler OnMetaPoseDataReceived;
        public static  event MetaPoseFrameDataEventHandler OnMetaPoseFrameDataReceived;

        public static Action<string> OnMetaPoseDataLost;

        public int Count => _dataDic.Count;

        public SourceData this[string id] => _dataDic[id];

        public bool Contains(string id)
        {
            return _dataDic.ContainsKey(id);
        }

        public bool TryGetValue(string id, out SourceData data)
        {
            return _dataDic.TryGetValue(id, out data);
        }

        public bool TryGetValue(int index, out SourceData data)
        {
            if (index < _dataList.Count)
            {
                data = _dataDic[_dataList[index]];
                return true;
            }

            data = default;
            return false;
        }

        public SourceData this[int index] => _dataDic[_dataList[index]];

        private Source()
        {
            _dataDic = new ConcurrentDictionary<string, SourceData>();
            _dataList = new List<string>();
        }

        public static Source Data
        {
            get { return _instance ??= new Source(); }
        }

        /// <summary>
        /// 设置数据源Data
        /// </summary> 
        internal static void SetData(SourceData data)
        {
            if (!Data._dataDic.TryGetValue(data.BodyId, out var body))
            {
                Data._dataList.Add(data.BodyId);
            }

            Data._dataDic[data.BodyId] = data;
        }

        /// <summary>
        /// 删除数据源Data
        /// </summary> 
        internal static void DelData(string bodyId)
        {
            Data._dataList.RemoveAll(item => item == bodyId);
            Data._dataDic.TryRemove(bodyId, out _);
        }

        /// <summary>
        /// 设置数据单个人骨骼数据到订阅
        /// </summary> 
        public static void TriggerMetaPoseDataReceived(SourceData data)
        {
            OnMetaPoseDataReceived?.Invoke(data);
        }

        /// <summary>
        /// 设置当前帧所有骨骼数据到订阅
        /// </summary> 
        public static void TriggerMetaPoseFrameDataReceived(long frameId, List<SourceData> data)
        {
            OnMetaPoseFrameDataReceived?.Invoke(frameId, data);
        }

        /// <summary>
        /// 设置具体人员骨骼消失到订阅
        /// </summary>
        /// <param name="key">人员 id</param>
        public static void TriggerMetaPostDataLost(string key)
        {
            OnMetaPoseDataLost?.Invoke(key);
        }

        public IEnumerator<SourceData> GetEnumerator()
        {
            return _dataDic.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}