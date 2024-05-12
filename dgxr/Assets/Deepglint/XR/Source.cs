using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR
{
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
        RightHandDrawCircle   = 1,  //左手画圈
        LeftHandDrawCircle    = 2,  //右手画圈 
        Kick                  = 10, //踢腿
        CombineHandsStraight  = 17, //双手伸直合并
        ThrowBoulder          = 18, //举手投掷巨物 
        SlowRun               = 19, //慢跑
        FastRun               = 20, //快跑  
        ButterflySwim         = 21, //蝶泳  
        FreestyleSwim         = 22, //自由泳   
        KeepRaisingHand       = 23, //持续举手   
        Applaud               = 24, //拍掌   
        Jump                  = 25, //起跳   
        DeepSquat             = 26, //下蹲   
        RaiseOnHand           = 10000,  //举单手
        RaiseBothHand         = 10001,  //举双手
        ArmFlat               = 10002,  //手臂平展 
        ArmFlatIsL            = 10003,  //手臂平展为 L 
        ArmVerticalIsL        = 10004,  //手臂垂直为 L 
    }

    public static class Source
    {
        public static List<SourceData> Data = new List<SourceData>();

        public struct SourceData
        {
            public string FrameId;
            public string BodyId;
            public Dictionary<ActionType, float> Actions;
            public JointData Joints;
        }

        public struct JointData
        {
            public Vector3 Nose;
            public Vector3 LeftEye;
            public Vector3 RightEye;
            public Vector3 LeftEar;
            public Vector3 RightEar;
            public Vector3 LeftShoulder;
            public Vector3 RightShoulder;
            public Vector3 LeftElbow;
            public Vector3 RightElbow;
            public Vector3 LeftWrist;
            public Vector3 RightWrist;
            public Vector3 LeftHip;
            public Vector3 RightHip;
            public Vector3 LeftKnee;
            public Vector3 RightKnee;
            public Vector3 LeftAnkle;
            public Vector3 RightAnkle;
            public Vector3 LeftTiptoe;
            public Vector3 RightTiptoe;
            public Vector3 LeftHeel;
            public Vector3 RightHeel;
            public Vector3 HeadTop;
            public Vector3 LeftHand;
            public Vector3 RightHand;
        }
    }
}