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
        public long FrameId;
        public Vector3 HeadTop;
        public float[] Features;

        public PersonFeature(SourceData data)
        {
            Time = DateTime.Now;
            BodyId = data.BodyId;
            FrameId = data.FrameId;
            HeadTop = data.Joints.HeadTop.LocalPosition;
            Features = new float[]
            {
                Vector3.Distance(data.Joints.LeftShoulder.LocalPosition, data.Joints.RightShoulder.LocalPosition),
                Vector3.Distance(data.Joints.LeftShoulder.LocalPosition, data.Joints.LeftHip.LocalPosition),
                Vector3.Distance(data.Joints.RightShoulder.LocalPosition, data.Joints.RightHip.LocalPosition),
                Vector3.Distance(data.Joints.LeftHip.LocalPosition, data.Joints.LeftKnee.LocalPosition),
                Vector3.Distance(data.Joints.RightHip.LocalPosition, data.Joints.RightKnee.LocalPosition),
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

        public bool IsFarFromRoi()
        {
            bool result = false;
            var position = DGXR.Space.Bottom.SpaceToPixelOnScreen(HeadTop);
            Rect roi = DGXR.Space.Roi;
            if (roi.height == 0)
            {
                roi.height = 0.65f / DGXR.Space.Bottom.Size.y * DGXR.Space.Bottom.Resolution.height; 
            }
            if (!roi.Contains(position))
            {
                result = true;
                Debug.Log($"{BodyId} is far from ROI");
            }

            return result;
        }
    }
    
    public class PseudoOfflineFilter : MonoBehaviour
    {
        public int OfflineFrameGap = 150;
        public int NewbeeFrameGap = 90;
        public float DistanceThreshold = 0.65f;
        public float SimilarityThreshold = 0.90f;
        public float MAEThreshold = 0.06f; 
        private Int64 currentFrameId = 0;
        private float _checkInterval = 1f;
        private bool _messageReceived = true;
        private float _timeSinceLastCheck = 0f;

        private static bool EnableFilter = false;
        private static readonly ConcurrentDictionary<string, PersonFeature> Features = new ConcurrentDictionary<string, PersonFeature>();
        private static readonly Dictionary<string, PersonFeature> Newbee = new Dictionary<string, PersonFeature>();
        internal static ConcurrentDictionary<string, PersonFeature> OfflineFeatures = new ConcurrentDictionary<string, PersonFeature>();
        internal static Dictionary<string, PersonFeature> ChangeLog = new Dictionary<string, PersonFeature>();

        public static bool Enabled => EnableFilter;
        public static PseudoOfflineFilter Instance { get; private set; }
        public static Action<string> OnPersonOffline;
        
        private void CheckMessageStatus()
        {
            if (!_messageReceived)
            {
                Source.IsOnline = false;
                Debug.Log("No message received. Status: Offline");
            }
            else
            {
                Source.IsOnline = true;
                Debug.Log("Message received. Status: Online");
            }
            _messageReceived = false;
        }
        
        private void OnMetaPoseFrameDataReceived(long frameId, List<SourceData> data)
        {
            _messageReceived = true;
            currentFrameId = frameId;
        }
        
        private void OnMetaPoseDataReceived(SourceData data)
        {
            currentFrameId = data.FrameId;
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
                Debug.LogWarning($"add {bodyId} to offline cache");
                value.Time = DateTime.Now;
                OfflineFeatures[bodyId] = value; 
                if (Newbee.Remove(bodyId))
                {
                    Debug.Log($"remove {bodyId} from newbee cache"); 
                }
            } 
        }

        private void OnEnable()
        {
            EnableFilter = true;
            Debug.Log("enable pseudo-offline-filter");
            Source.OnMetaPoseDataLost += OnMetaPoseDataLost;
            Source.OnMetaPoseDataReceived += OnMetaPoseDataReceived;
            Source.OnMetaPoseFrameDataReceived += OnMetaPoseFrameDataReceived;
        }

        private void OnDisable()
        {
            EnableFilter = false;
            Debug.Log("disable pseudo-offline-filter");
            Source.OnMetaPoseDataLost -= OnMetaPoseDataLost;
            Source.OnMetaPoseDataReceived -= OnMetaPoseDataReceived;
            Source.OnMetaPoseFrameDataReceived -= OnMetaPoseFrameDataReceived;
            
            List<string> offlineKeys = new List<string>(OfflineFeatures.Keys);
            foreach (var key in offlineKeys)
            {
                if (OfflineFeatures.TryRemove(key, out _))
                {
                    OnPersonOffline?.Invoke(key);
                    Debug.Log($"remove {key} from offline cache");
                }
            }
            ChangeLog.Clear();
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
            _timeSinceLastCheck += Time.deltaTime;

            if (_timeSinceLastCheck >= _checkInterval)
            {
                CheckMessageStatus();
                _timeSinceLastCheck = 0f; 
            }
            
            List<string> offlineKeys = new List<string>(OfflineFeatures.Keys);
            foreach (var key in offlineKeys)
            {
                if (OfflineFeatures.TryGetValue(key, out PersonFeature value))
                {
                    if (Mathf.Abs(currentFrameId - value.FrameId) > OfflineFrameGap)
                    {
                        if (OfflineFeatures.TryRemove(key, out _))
                        {
                            TriggerPersonOffline(key); 
                            Debug.Log($"remove {key} from offline cache, cause out of frame gap");
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
                        Debug.Log($"remove {key} from newbee cache, cause out of frame gap"); 
                    }
                }
            }
        }

        private void TriggerPersonOffline(string bodyId)
        {
            OnPersonOffline?.Invoke(bodyId);
            List<string> keys = new List<string>(ChangeLog.Keys);
            foreach (var key in keys)
            {
                if (ChangeLog[key].BodyId == bodyId)
                {
                    ChangeLog.Remove(key);
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
                    result = OfflineFeatures.TryRemove(data.BodyId, out _);
                    if (result)
                    {
                        Debug.Log($"person {data.BodyId} reconnected, remove it from offline cache");
                    }
                } else if (!Source.Data.Contains(data.BodyId))
                {
                    // 首次出现的骨骼根据距离ROI的位置放宽找回条件
                    PersonFeature changeFeature = null;
                    if (feature.IsFarFromRoi())
                    {
                        changeFeature = GetMostSimilarOfflineFeature(feature, false);
                    }
                    else
                    {
                        changeFeature = GetMostSimilarOfflineFeature(feature);
                    } 
                    if (changeFeature != null)
                    {
                        Debug.LogWarning($"change body from {feature.BodyId} to {changeFeature.BodyId}");
                        result = OfflineFeatures.TryRemove(changeFeature.BodyId, out _);
                        if (result)
                        {
                            ChangeLog[feature.BodyId] = changeFeature;
                            data.BodyId = changeFeature.BodyId;
                            Debug.Log($"remove {changeFeature.BodyId} from offline cache");
                        }
                    }
                    else
                    {
                        Newbee.Add(data.BodyId, feature); 
                        Debug.Log($"add {data.BodyId} to newbee cache"); 
                    }
                } else if (Newbee.ContainsKey(data.BodyId))
                {
                    PersonFeature changeFeature = GetMostSimilarOfflineFeature(feature);
                    if (changeFeature != null)
                    {
                        Debug.LogWarning($"change body from {feature.BodyId} to {changeFeature.BodyId}");
                        result = OfflineFeatures.TryRemove(changeFeature.BodyId, out _);
                        if (result)
                        {
                            ChangeLog[feature.BodyId] = changeFeature;
                            data.BodyId = changeFeature.BodyId;
                            Debug.Log($"remove {changeFeature.BodyId} from offline cache");
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
                    if (DGXR.Config.Debug)
                    {
                        Debug.Log($"person {pf.BodyId} similarity with {item.Value.BodyId} is {similarity} and distance is {distance}");
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
                } else if (DGXR.Config.Debug)
                {
                    Debug.Log($"person {pf.BodyId} missed offline person {item.Value.BodyId} because of too big frame gap");
                }
            }

            return result;
        }
    }
}