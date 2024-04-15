using System.Collections.Generic;
using UnityEngine;

namespace DGXR
{
    public static class Source
    {
        public static List<SourceData> Data = new List<SourceData>();
        public static void SetData(List<SourceData> data)
        {
            Data = data;
        }

        // datasouce
        public struct SourceData
        {
            public string FrameId;
            public Dictionary<Action.ActionType, float> Action;
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
