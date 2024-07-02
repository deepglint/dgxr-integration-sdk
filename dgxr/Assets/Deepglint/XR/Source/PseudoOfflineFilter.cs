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
    }
    
    public class PseudoOfflineFilter : MonoBehaviour
    {
        internal bool EnableFilter = false;
        public int FrameGap = 60;
        public float DistanceThreshold = 0.5f;
        public float SimilarityThreshold = 0.90f;
        public bool ShowDetailLog = false;
        
        // 60 seconds
        private int _timeout = 20;
        
        private static readonly ConcurrentDictionary<string, PersonFeature> Features = new ConcurrentDictionary<string, PersonFeature>();
        private static Dictionary<string, DateTime> Newbee = new Dictionary<string, DateTime>();
        internal static ConcurrentDictionary<string, PersonFeature> OfflineFeatures = new ConcurrentDictionary<string, PersonFeature>();
        internal static Dictionary<string, string> ChangeLog = new Dictionary<string, string>();
        
        public static PseudoOfflineFilter Instance { get; private set; }
        
        private void OnMetaPoseDataReceived(SourceData data)
        {
            PersonFeature feature = new PersonFeature(data);
            Features[feature.BodyId] = feature;
            if (!Source.Data.Contains(data.BodyId))
            {
                Newbee.Add(data.BodyId, DateTime.Now);
                Debug.LogFormat("add {0} to newbee cache", data.BodyId);
            }
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
                    var duration = (DateTime.Now - value.Time).TotalSeconds;
                    if (duration > _timeout)
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
                if (Newbee.TryGetValue(key, out DateTime value))
                {
                    var duration = (DateTime.Now - value).TotalSeconds;
                    if (duration > _timeout)
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
                        Features[feature.BodyId] = feature; 
                        Debug.LogFormat("person {0} reconnected, remove it from offline cache", data.BodyId);
                    }
                } else if (!Source.Data.Contains(data.BodyId) || Newbee.ContainsKey(data.BodyId))
                {
                    PersonFeature changeFeature = GetMostSimilarOfflineFeature(feature);
                    if (changeFeature != null)
                    {
                        Debug.LogFormat("change body from {0} to {1}", feature.BodyId, changeFeature.BodyId);
                        result = OfflineFeatures.TryRemove(changeFeature.BodyId, out PersonFeature value);
                        if (result)
                        {
                            ChangeLog[feature.BodyId] = changeFeature.BodyId;
                            data.BodyId = changeFeature.BodyId;
                            feature.BodyId = changeFeature.BodyId;
                            Features[feature.BodyId] = feature; 
                            Debug.LogFormat("remove {0} from offline cache", changeFeature.BodyId);
                        }
                    } 
                }
            }

            return result;
        }
        
        private PersonFeature GetMostSimilarOfflineFeature(PersonFeature pf)
        {
            PersonFeature result = null;
            float maxSimilarity = 0f;
            Vector2 headTop = new Vector2(pf.HeadTop.x, pf.HeadTop.z);
            foreach (var item in OfflineFeatures)
            {
                if (pf.FrameId - item.Value.FrameId <= FrameGap)
                {
                    float distance = Vector2.Distance(headTop, new Vector2(item.Value.HeadTop.x, item.Value.HeadTop.z));
                    float similarity = pf.Similarity(item.Value);
                    if (ShowDetailLog)
                    {
                        Debug.LogFormat("person {0} similarity with {1} is {2} and distance is {3}", pf.BodyId, item.Value.BodyId, similarity, distance);
                    }
                    if (distance <= DistanceThreshold && similarity >= SimilarityThreshold)
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