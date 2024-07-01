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

    public struct JointData
    {
        private Vector3 _nose;

        public Vector3 Nose
        {
            get => _nose + Global.Space.Origin;
            set => _nose = value;
        }
        
        private Vector3 _leftEye;
        public Vector3 LeftEye
        {
            get => _leftEye + Global.Space.Origin;
            set => _leftEye = value;
        }

        private Vector3 _rightEye;
        public Vector3 RightEye
        {
            get => _rightEye + Global.Space.Origin;
            set => _rightEye = value;
        }

        private Vector3 _leftEar; 
        public Vector3 LeftEar
        {
            get => _leftEar + Global.Space.Origin;
            set => _leftEar = value;
        }

        private Vector3 _rightEar;
        public Vector3 RightEar
        {
            get => _rightEar + Global.Space.Origin;
            set => _rightEar = value;
        }

        private Vector3 _leftShoulder;
        public Vector3 LeftShoulder
        {
            get => _leftShoulder + Global.Space.Origin;
            set => _leftShoulder = value;
        }

        private Vector3 _rightShoulder;
        public Vector3 RightShoulder
        {
            get => _rightShoulder + Global.Space.Origin;
            set => _rightShoulder = value;
        }

        private Vector3 _leftElbow;
        public Vector3 LeftElbow
        {
            get => _leftElbow + Global.Space.Origin;
            set => _leftElbow = value;
        }

        private Vector3 _rightElbow;
        public Vector3 RightElbow
        {
            get => _rightElbow + Global.Space.Origin;
            set => _rightElbow = value;
        }

        private Vector3 _leftWrist;
        public Vector3 LeftWrist
        {
            get => _leftWrist + Global.Space.Origin;
            set => _leftWrist = value;
        }

        private Vector3 _rightWrist;
        public Vector3 RightWrist
        {
            get => _rightWrist + Global.Space.Origin;
            set => _rightWrist = value;
        }

        private Vector3 _leftHip;
        public Vector3 LeftHip
        {
            get => _leftHip + Global.Space.Origin;
            set => _leftHip = value;
        }

        private Vector3 _rightHip;
        public Vector3 RightHip
        {
            get => _rightHip + Global.Space.Origin;
            set => _rightHip = value;
        }

        private Vector3 _leftKnee;
        public Vector3 LeftKnee
        {
            get => _leftKnee + Global.Space.Origin;
            set => _leftKnee = value;
        }

        private Vector3 _rightKnee;
        public Vector3 RightKnee
        {
            get => _rightKnee + Global.Space.Origin;
            set => _rightKnee = value;
        }

        private Vector3 _leftAnkle;
        public Vector3 LeftAnkle
        {
            get => _leftAnkle + Global.Space.Origin;
            set => _leftAnkle = value;
        }

        private Vector3 _rightAnkle;
        public Vector3 RightAnkle
        {
            get => _rightAnkle + Global.Space.Origin;
            set => _rightAnkle = value;
        }

        private Vector3 _leftTiptoe;
        public Vector3 LeftTiptoe
        {
            get => _leftTiptoe + Global.Space.Origin;
            set => _leftTiptoe = value;
        }

        private Vector3 _rightTiptoe;
        public Vector3 RightTiptoe
        {
            get => _rightTiptoe + Global.Space.Origin;
            set => _rightTiptoe = value;
        }

        private Vector3 _leftHeel;
        public Vector3 LeftHeel
        {
            get => _leftHeel + Global.Space.Origin;
            set => _leftHeel = value;
        }

        private Vector3 _rightHeel;
        public Vector3 RightHeel
        {
            get => _rightHeel + Global.Space.Origin;
            set => _rightHeel = value;
        }

        private Vector3 _headTop;
        public Vector3 HeadTop
        {
            get => _headTop + Global.Space.Origin;
            set => _headTop = value;
        }

        private Vector3 _leftHand;
        public Vector3 LeftHand
        {
            get => _leftHand + Global.Space.Origin;
            set => _leftHand = value;
        }

        private Vector3 _rightHand;
        public Vector3 RightHand
        {
            get => _rightHand + Global.Space.Origin;
            set => _rightHand = value;
        }
    }

    public struct SourceData
    {
        public int FrameId;
        public string BodyId;
        public Dictionary<ActionType, float> Actions;
        public JointData Joints;
        public float FirstAddedTime;
    }


    public class Source : IEnumerable<SourceData>
    {
        private static Source _instance;
        private ConcurrentDictionary<string, SourceData> _dataDic;

        public static SourceType DataFrom;

        public delegate void MetaPoseDataEventHandler(SourceData data);

        public delegate void MetaPoseFrameDataEventHandler(List<SourceData> data);

        public static event MetaPoseDataEventHandler OnMetaPoseDataReceived;
        public static event MetaPoseFrameDataEventHandler OnMetaPoseFrameDataReceived;

        public static Action<string> OnMetaPoseDataLost;

        public int Count => _dataDic.Count;

        public SourceData this[string id] => _dataDic[id];

        public bool TryGetValue(string id, out SourceData data)
        {
            return _dataDic.TryGetValue(id, out data);
        }

        public bool TryGetValue(int index, out SourceData data)
        {
            var sortedList = _dataDic
                .OrderBy(pair => pair.Value.FirstAddedTime)
                .ToDictionary(pair => pair.Key, eventPair => eventPair.Value).ToList();
            if (index < sortedList.Count)
            {
                data = sortedList[index].Value;
                return true;
            }

            data = default;
            return false;
        }

        public SourceData this[int index]
        {
            get
            {
                var sortedList = _dataDic
                    .OrderBy(pair => pair.Value.FirstAddedTime)
                    .ToDictionary(pair => pair.Key, eventPair => eventPair.Value).ToList();
                return sortedList[index].Value;
            }
        }

        private Source()
        {
            _dataDic = new ConcurrentDictionary<string, SourceData>();
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
            if (Data._dataDic.TryGetValue(data.BodyId, out var body))
            {
                data.FirstAddedTime = body.FirstAddedTime;
            }
            else
            {
                data.FirstAddedTime = Time.time;
            }

            Data._dataDic[data.BodyId] = data;
        }

        /// <summary>
        /// 删除数据源Data
        /// </summary> 
        internal static void DelData(string bodyId)
        {
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
        public static void TriggerMetaPoseFrameDataReceived(List<SourceData> data)
        {
            OnMetaPoseFrameDataReceived?.Invoke(data);
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