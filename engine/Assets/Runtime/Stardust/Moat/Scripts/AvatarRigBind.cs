using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Moat
{
    public class AvatarRigBind : MonoBehaviour
    {
        [Header("常规参数")] public float scaleRatio = 20f;
        public Transform parentObject;

        [Header("模型骨骼")] public Dictionary<string, string> AvatarBody = new Dictionary<string, string>();
        public Dictionary<string, Transform> AvatarBodyObj = new Dictionary<string, Transform>();

        [Header("映射骨骼")] 
        public Dictionary<string, Transform> BodyObj = new Dictionary<string, Transform>();
        
        [Header("AvatarBody")]
        public Transform Nose;
        public Transform LeftEye;
        public Transform RightEye;
        public Transform LeftEar;
        public Transform RightEar;
        public Transform LeftShoulder;
        public Transform RightShoulder;
        public Transform LeftElbow;
        public Transform RightElbow;
        public Transform LeftWrist;
        public Transform RightWrist;
        public Transform LeftHip;
        public Transform RightHip;
        public Transform LeftKnee;
        public Transform RightKnee;
        public Transform LeftAnkle;
        public Transform RightAnkle;
        public Transform LeftTiptoe;
        public Transform RightTiptoe;
        public Transform LeftHeel;
        public Transform RightHeel;
        public Transform HeadTop;
        public Transform LeftHand;
        public Transform RightHand;
        
        public Transform Neck;
        public Transform Root;

        private bool _initFinished = false;

        // Start is called before the first frame update
        void Start()
        {
            AvatarBody.Add("Root", "mixamorig:Hips");
            AvatarBody.Add("HeadTop", "mixamorig:HeadTop_End");
            AvatarBody.Add("Nose", "");
            AvatarBody.Add("Neck", "mixamorig:Neck");
            AvatarBody.Add("LeftEye", "");
            AvatarBody.Add("RightEye", "");
            AvatarBody.Add("LeftEar", "");
            AvatarBody.Add("RightEar", "");
            AvatarBody.Add("LeftShoulder", "mixamorig:LeftShoulder");
            AvatarBody.Add("RightShoulder", "mixamorig:RightShoulder");
            AvatarBody.Add("LeftElbow", "mixamorig:LeftForeArm");
            AvatarBody.Add("RightElbow", "mixamorig:RightForeArm");
            AvatarBody.Add("LeftWrist", "");
            AvatarBody.Add("RightWrist", "");
            AvatarBody.Add("LeftHip", "mixamorig:LeftUpLeg");
            AvatarBody.Add("RightHip", "mixamorig:RightUpLeg");
            AvatarBody.Add("LeftKnee", "mixamorig:LeftLeg");
            AvatarBody.Add("RightKnee", "mixamorig:RightLeg");
            AvatarBody.Add("LeftAnkle", "");
            AvatarBody.Add("RightAnkle", "");
            AvatarBody.Add("LeftTiptoe", "mixamorig:LeftToeBase");
            AvatarBody.Add("RightTiptoe", "mixamorig:RightToeBase");
            AvatarBody.Add("LeftHeel", "");
            AvatarBody.Add("RightHeel", "");
            AvatarBody.Add("LeftHand", "mixamorig:LeftHand");
            AvatarBody.Add("RightHand", "mixamorig:RightHand");

            // Debug.LogError("mixamorig:Hips2: " + parentObject.Find("Avatar/young-bellona-fighting/mixamorig:Hips"));

            Init();
        }

        void Init()
        {
            foreach (KeyValuePair<string, string> kvp in AvatarBody)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                Transform bodyTransform = parentObject.Find(key);
                if (bodyTransform != null)
                {
                    BodyObj.Add(key, bodyTransform);
                }

                if (value != "")
                {
                    Transform avatarTransform = parentObject.Find(value);
                    if (avatarTransform != null)
                    {
                        AvatarBodyObj.Add(key, avatarTransform);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // foreach (KeyValuePair<string, Transform> key in AvatarBodyObj)
            // {
            //     AvatarBodyObj[key.Key].localPosition = BodyObj[key.Key].position * scaleRatio;
            // }
            // AvatarRoot.localPosition = new Vector3(Root.position.x, Root.position.y, Root.position.z) * scaleRatio;

            // if (Root != null) Root.localPosition = BodyObj["Root"].position * scaleRatio;
            // if (HeadTop != null) HeadTop.localPosition = BodyObj["HeadTop"].localPosition * scaleRatio;
            // if (Neck != null) Neck.localPosition = BodyObj["Neck"].localPosition * scaleRatio;
            // if (LeftShoulder != null) LeftShoulder.localPosition = BodyObj["LeftShoulder"].localPosition * scaleRatio;
            // if (RightShoulder != null) RightShoulder.localPosition = BodyObj["RightShoulder"].localPosition * scaleRatio;
            // if (LeftElbow != null) LeftElbow.localPosition = BodyObj["LeftElbow"].localPosition * scaleRatio;
            // if (RightElbow != null) RightElbow.localPosition = BodyObj["RightElbow"].localPosition * scaleRatio;
            // if (LeftHip != null) LeftHip.localPosition = BodyObj["LeftHip"].localPosition * scaleRatio;
            // if (RightHip != null) RightHip.localPosition = BodyObj["RightHip"].localPosition * scaleRatio;
            // if (LeftKnee != null) LeftKnee.localPosition = BodyObj["LeftKnee"].position * scaleRatio;
            // if (RightKnee != null) RightKnee.localPosition = BodyObj["RightKnee"].position * scaleRatio;
            // if (LeftTiptoe != null) LeftTiptoe.localPosition = BodyObj["LeftTiptoe"].position * scaleRatio;
            // if (RightTiptoe != null) RightTiptoe.localPosition = BodyObj["RightTiptoe"].position * scaleRatio;
            // if (LeftHand != null) LeftHand.localPosition = BodyObj["LeftHand"].position * scaleRatio;
            // if (RightHand != null) RightHand.localPosition = BodyObj["RightHand"].position * scaleRatio;
        }
    }
}