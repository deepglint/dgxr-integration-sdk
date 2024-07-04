using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Deepglint.XR.Source
{
    public class PersonFeature
    {
        public DateTime Time;
        public string BodyId;
        public Int64 FrameId;
        public Vector3 HeadTop;
        public float[] Features;

        public PersonFeature(SourceData data)
        {
            Time = DateTime.Now;
            BodyId = data.BodyId;
            FrameId = long.Parse(data.FrameId);
            HeadTop = data.Joints.HeadTop;
            Features = new float[]
            {
                Vector3.Distance(data.Joints.LeftShoulder, data.Joints.RightShoulder),
                Vector3.Distance(data.Joints.LeftShoulder, data.Joints.LeftHip),
                Vector3.Distance(data.Joints.RightShoulder, data.Joints.RightHip),
                Vector3.Distance(data.Joints.LeftHip, data.Joints.LeftKnee),
                Vector3.Distance(data.Joints.RightHip, data.Joints.RightKnee),
            };
        }

        // 计算与目标特征之间的平均绝对误差
        public float MAE(PersonFeature target)
        {
            float sum = 0;
            for (int i= 0; i < target.Features.Length; i++)
            {
                sum += Mathf.Abs(Features[i] - target.Features[i]);
            }

            return sum / Features.Length;
        }

        public float Similarity(PersonFeature target)
        {
            if (Features.Length != target.Features.Length)
            {
                return 0f;
            }
            float dotProduct = 0;
            float magnitudeA = 0;
            float magnitudeB = 0;

            for (int i = 0; i < Features.Length; i++)
            {
                dotProduct += Features[i] * target.Features[i];
                magnitudeA += Features[i] * Features[i];
                magnitudeB += target.Features[i] * target.Features[i];
            }

            magnitudeA = Mathf.Sqrt(magnitudeA);
            magnitudeB = Mathf.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                Debug.LogError("向量的模不能为零");
                return -1f;
            }

            float similarity = 1f - (1f - dotProduct / (magnitudeA * magnitudeB)) * 100;
            return similarity > 0 ? similarity : 0;
        }

        public bool IsFarFromROI()
        {
            bool result = false;
            var head = Global.Space.gameObject.transform.InverseTransformPoint(HeadTop);
            var position = Global.Space.Bottom.SpaceToPixelOnScreen(head);
            Rect ROI = new Rect(Global.Space.Roi.x, Global.Space.Roi.y, Global.Space.Roi.width, Global.Space.Roi.width);
            if (ROI.height == 0)
            {
                ROI.height = 0.65f / Global.Space.Bottom.Size.y * Global.Space.Bottom.Resolution.height; 
            }
            if (!ROI.Contains(position))
            {
                result = true;
                Debug.LogFormat("{0} is far from ROI", BodyId);
            }

            return result;
        }
    }
    
    public class PseudoOfflineFilter : MonoBehaviour
    {
        internal bool EnableFilter = false;
        public int OfflineFrameGap = 150;
        public int NewbeeFrameGap = 90;
        public float DistanceThreshold = 0.65f;
        public float SimilarityThreshold = 0.90f;
        public bool ShowDetailLog = false;
        public float MAEThreshold = 0.06f; 
        private Int64 currentFrameId = 0;
        
        private static readonly ConcurrentDictionary<string, PersonFeature> Features = new ConcurrentDictionary<string, PersonFeature>();
        private static Dictionary<string, PersonFeature> Newbee = new Dictionary<string, PersonFeature>();
        private static HashSet<string> oldPersons = new HashSet<string>();
        internal static ConcurrentDictionary<string, PersonFeature> OfflineFeatures = new ConcurrentDictionary<string, PersonFeature>();
        internal static Dictionary<string, PersonFeature> ChangeLog = new Dictionary<string, PersonFeature>();
        
        public static PseudoOfflineFilter Instance { get; private set; }
        
        private void OnMetaPoseDataReceived(SourceData data)
        {
            currentFrameId = long.Parse(data.FrameId);
            PersonFeature feature = new PersonFeature(data);
            if (Features.TryGetValue(data.BodyId, out var oldFeature))
            {
                // 过滤异常抖动
                if (feature.MAE(oldFeature) >= MAEThreshold)
                {
                    feature.HeadTop = oldFeature.HeadTop;
                    feature.Features = oldFeature.Features;
                }
            }
            Features[feature.BodyId] = feature;
        }

        private void OnMetaPoseDataLost(string bodyId)
        {
            if (Features.TryRemove(bodyId, out PersonFeature value))
            {
                Debug.LogFormat("add {0} to offline cache", bodyId);
                value.Time = DateTime.Now;
                OfflineFeatures[bodyId] = value; 
                if (Newbee.Remove(bodyId))
                {
                    Debug.LogFormat("remove {0} from newbee cache", bodyId); 
                }
            } 
        }

        private void OnEnable()
        {
            Debug.Log("enable pseudo-offline-filter");
            EnableFilter = true;
            Source.OnMetaPoseDataLost += OnMetaPoseDataLost;
            Source.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
        }

        private void OnDisable()
        {
            Debug.Log("disable pseudo-offline-filter");
            EnableFilter = false;
            Source.OnMetaPoseDataLost -= OnMetaPoseDataLost;
            Source.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            List<string> offlineKeys = new List<string>(OfflineFeatures.Keys);
            foreach (var key in offlineKeys)
            {
                if (OfflineFeatures.TryGetValue(key, out PersonFeature value))
                {
                    if (Mathf.Abs(currentFrameId - value.FrameId) > OfflineFrameGap)
                    {
                        if (OfflineFeatures.TryRemove(key, out PersonFeature timeoutValue))
                        {
                            Debug.LogFormat("remove {0} from offline cache", key);
                        } 
                    }
                }
            }
            
            List<string> newbeeKeys = new List<string>(Newbee.Keys);
            foreach (var key in newbeeKeys)
            {
                if (Newbee.TryGetValue(key, out PersonFeature value))
                {
                    if (Mathf.Abs(currentFrameId - value.FrameId) > NewbeeFrameGap)
                    {
                        Newbee.Remove(key);
                        Debug.LogFormat("remove {0} from newbee cache", key); 
                    }
                }
            }
        }

        internal bool Filter(ref SourceData data)
        {
            bool result = false;
            if (EnableFilter)
            {
                PersonFeature feature = new PersonFeature(data);
                if (OfflineFeatures.ContainsKey(data.BodyId))
                {
                    result = OfflineFeatures.TryRemove(data.BodyId, out PersonFeature value);
                    if (result)
                    {
                        Debug.LogFormat("person {0} reconnected, remove it from offline cache", data.BodyId);
                    }
                } else if (!Source.Data.Contains(data.BodyId))
                {
                    // 首次出现的骨骼根据距离ROI的位置放宽找回条件
                    PersonFeature changeFeature = null;
                    if (feature.IsFarFromROI())
                    {
                        changeFeature = GetMostSimilarOfflineFeature(feature, false);
                    }
                    else
                    {
                        changeFeature = GetMostSimilarOfflineFeature(feature);
                    } 
                    if (changeFeature != null)
                    {
                        Debug.LogWarningFormat("change body from {0} to {1}", feature.BodyId, changeFeature.BodyId);
                        result = OfflineFeatures.TryRemove(changeFeature.BodyId, out PersonFeature value);
                        if (result)
                        {
                            ChangeLog[feature.BodyId] = changeFeature;
                            data.BodyId = changeFeature.BodyId;
                            Debug.LogFormat("remove {0} from offline cache", changeFeature.BodyId);
                        }
                    }
                    else
                    {
                        Newbee.Add(data.BodyId, feature); 
                        Debug.LogFormat("add {0} to newbee cache", data.BodyId); 
                    }
                } else if (Newbee.ContainsKey(data.BodyId))
                {
                    PersonFeature changeFeature = GetMostSimilarOfflineFeature(feature);
                    if (changeFeature != null)
                    {
                        Debug.LogWarningFormat("change body from {0} to {1}", feature.BodyId, changeFeature.BodyId);
                        result = OfflineFeatures.TryRemove(changeFeature.BodyId, out PersonFeature value);
                        if (result)
                        {
                            ChangeLog[feature.BodyId] = changeFeature;
                            data.BodyId = changeFeature.BodyId;
                            Debug.LogFormat("remove {0} from offline cache", changeFeature.BodyId);
                        }
                    }  
                }
            }

            return result;
        }
        
        private PersonFeature GetMostSimilarOfflineFeature(PersonFeature pf, bool useThreshold = true)
        {
            PersonFeature result = null;
            float maxSimilarity = 0f;
            Vector2 headTop = new Vector2(pf.HeadTop.x, pf.HeadTop.z);
            foreach (var item in OfflineFeatures)
            {
                if (pf.FrameId - item.Value.FrameId <= OfflineFrameGap)
                {
                    float distance = Vector2.Distance(headTop, new Vector2(item.Value.HeadTop.x, item.Value.HeadTop.z));
                    float similarity = pf.Similarity(item.Value);
                    if (ShowDetailLog)
                    {
                        Debug.LogFormat("person {0} similarity with {1} is {2} and distance is {3}", pf.BodyId, item.Value.BodyId, similarity, distance);
                    }
                    if (useThreshold)
                    {
                        if (distance <= DistanceThreshold && similarity >= SimilarityThreshold)
                        {
                            if (similarity >= maxSimilarity)
                            {
                                result = item.Value;
                                maxSimilarity = similarity;
                            } 
                        }
                    }
                    else
                    {
                        if (similarity >= maxSimilarity)
                        {
                            result = item.Value;
                            maxSimilarity = similarity;
                        }
                    }
                } else if (ShowDetailLog)
                {
                    Debug.LogFormat("person {0} missed offline person {1} because of too big frame gap", pf.BodyId, item.Value.BodyId);
                }
            }

            return result;
        }
    }
}