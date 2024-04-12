using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DGXR
{
    public class Source
    {
        readonly int _maxCacheFrameSize = Global.Config.Source.MaxCacheFrame;
        private static Source _instance;
        private ConcurrentDictionary<string, BodyData> _data = new ConcurrentDictionary<string, BodyData>();

        private Source() { }

        private static readonly Lazy<Source> LazyInstance = new Lazy<Source>(() => new Source());

        public static Source Instance => LazyInstance.Value;

        public bool DelData(string key)
        {
            return _data.TryRemove(key, out _);
        }

        public ConcurrentDictionary<string, BodyData> GetData()
        {
            return _data;
        }

        public void SetData(string id, BodyDataSource data)
        {
            if (_data.TryGetValue(id, out var value))
            {
                value.Enqueue(data);
            }
            else
            {
                BodyData body = new BodyData(id, _maxCacheFrameSize);
                body.Enqueue(data);
                _data.TryAdd(id, body);
            }
        }

        public struct BodyDataSource
        {
            public string FrameId;
            public Dictionary<Action.ActionType, float> Action;
            public Dictionary<JointType, JointData> Joints;
        }

        public class BodyData
        {
            public string BodyID;
            readonly int _maxSize;
            private ConcurrentQueue<BodyDataSource> _q = new ConcurrentQueue<BodyDataSource>();

            public BodyData(string id, int maxSize)
            {
                this.BodyID = id;
                this._maxSize = maxSize;
            }

            public void Enqueue(BodyDataSource item)
            {
                if (_q.Count >= _maxSize)
                {
                    _q.TryDequeue(out _);
                }
                _q.Enqueue(item);
            }

            public BodyDataSource GetLatest()
            {
                return _q.TryPeek(out var result) ? result : default;
            }
        }
    }

    public class JointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public JointData(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public UnityEngine.Vector3 Vector3()
        {
            return new UnityEngine.Vector3((float)X,(float)Z, (float)Y);  
        }
    }

    public enum JointType
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
