using System.Collections.Generic;

namespace BodySource
{
    public struct BodyDataSource
    {
        public bool IsTracked;
        public string BodyID { get; set; }
        public Dictionary<JointType, JointData> Joints { get; set; }
        //public Dictionary<JointType, OrientationData> Orientations { get; set; }
    }

    public class JointData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public JointData(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public enum JointType : int
    {
        Nose = 0,
        LeftEye = 1,
        RightEye = 2,
        LeftEar = 3,
        RightEar = 4,
        LeftShoulder = 5,
        RightShoulder = 6,
        LeftElbow = 7,
        RightElbow = 8,
        LeftWrist = 9,
        RightWrist = 10,
        LeftHip = 11,
        RightHip = 12,
        LeftKnee = 13,
        RightKnee = 14,
        LeftAnkle = 15,
        RightAnkle = 16,
        LeftTiptoe = 17,
        RightTiptoe = 18,
        LeftHeel = 19,
        RightHeel = 20,
        HeadTop = 21,
        LeftHand = 22,
        RightHand = 23,
    }
}

    
