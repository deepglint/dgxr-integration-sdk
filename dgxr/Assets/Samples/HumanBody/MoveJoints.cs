using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Deepglint.XR;
using Deepglint.XR.Source;
using UnityEngine;

namespace Samples.HumanBody
{
    public class MoveJoints : MonoBehaviour
    {
        public GameObject bodyPrefab; 

        private ConcurrentDictionary<string, Body> _bodyMap ;

        private class Body
        {
            public GameObject Obj;
            public LineRenderer LineRenderer1;
            public LineRenderer LineRenderer2;
            public LineRenderer LineRenderer3;
            public LineRenderer LineRenderer4;
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
        }

        private void Start()
        {
           _bodyMap	 = new ConcurrentDictionary<string, Body>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Source.Data.Count == 0)
            {
                return;
            }
            
            foreach (var it in from it in _bodyMap let result = Source.Data.FirstOrDefault(item => item.BodyId == it.Key) where EqualityComparer<SourceData>.Default.Equals(result, default(SourceData)) select it)
            {
                Destroy	(it.Value.Obj);
                _bodyMap.TryRemove(it.Key, out var body);
            }
            var bodyData = Source.Data;
            
            foreach (var body in bodyData)
            {
                if (!_bodyMap.TryGetValue(body.BodyId, out var data))
                {
                    var bodyInfo = new Body();
                    var obj = Instantiate(bodyPrefab, transform);
                    
                    bodyInfo.Obj = obj;
                    InitObject(bodyInfo);
                    _bodyMap.TryAdd(body.BodyId, bodyInfo);
                }

                _bodyMap[body.BodyId].Nose.transform.localPosition = body.Joints.Nose;
                _bodyMap[body.BodyId].LeftEye.transform.localPosition = body.Joints.LeftEye;
                _bodyMap[body.BodyId].RightEye.transform.localPosition = body.Joints.RightEye;
                _bodyMap[body.BodyId].LeftEar.transform.localPosition = body.Joints.LeftEar;
                _bodyMap[body.BodyId].RightEar.transform.localPosition = body.Joints.RightEar;
                _bodyMap[body.BodyId].LeftShoulder.transform.localPosition = body.Joints.LeftShoulder;
                _bodyMap[body.BodyId].RightShoulder.transform.localPosition = body.Joints.RightShoulder;
                _bodyMap[body.BodyId].LeftElbow.transform.localPosition = body.Joints.LeftElbow;
                _bodyMap[body.BodyId].RightElbow.transform.localPosition = body.Joints.RightElbow;
                _bodyMap[body.BodyId].LeftWrist.transform.localPosition = body.Joints.LeftWrist;
                _bodyMap[body.BodyId].RightWrist.transform.localPosition = body.Joints.RightWrist;
                _bodyMap[body.BodyId].LeftHip.transform.localPosition = body.Joints.LeftHip;
                _bodyMap[body.BodyId].RightHip.transform.localPosition = body.Joints.RightHip;
                _bodyMap[body.BodyId].LeftKnee.transform.localPosition = body.Joints.LeftKnee;
                _bodyMap[body.BodyId].RightKnee.transform.localPosition = body.Joints.RightKnee;
                _bodyMap[body.BodyId].LeftAnkle.transform.localPosition = body.Joints.LeftAnkle;
                _bodyMap[body.BodyId].RightAnkle.transform.localPosition = body.Joints.RightAnkle;
                _bodyMap[body.BodyId].LeftTiptoe.transform.localPosition = body.Joints.LeftTiptoe;
                _bodyMap[body.BodyId].RightTiptoe.transform.localPosition = body.Joints.RightTiptoe;
                _bodyMap[body.BodyId].LeftHeel.transform.localPosition = body.Joints.LeftHeel;
                _bodyMap[body.BodyId].RightHeel.transform.localPosition = body.Joints.RightHeel;
                _bodyMap[body.BodyId].HeadTop.transform.localPosition = body.Joints.HeadTop;
                _bodyMap[body.BodyId].LeftHand.transform.localPosition = body.Joints.LeftHand;
                _bodyMap[body.BodyId].RightHand.transform.localPosition = body.Joints.RightHand;
                _bodyMap[body.BodyId].Neck.transform.localPosition =
                    (body.Joints.LeftShoulder + body.Joints.RightShoulder) / 2;
                _bodyMap[body.BodyId].Root.transform.localPosition = (body.Joints.LeftHip + body.Joints.RightHip) / 2;

                //it requires 4 lines to connect all the joints
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(0, _bodyMap[body.BodyId].LeftEar.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(1, _bodyMap[body.BodyId].LeftEye.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1.SetPosition(2, _bodyMap[body.BodyId].Nose.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1.SetPosition(3, _bodyMap[body.BodyId].Neck.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1.SetPosition(4, _bodyMap[body.BodyId].Root.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(5, _bodyMap[body.BodyId].RightHip.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(6, _bodyMap[body.BodyId].RightKnee.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(7, _bodyMap[body.BodyId].RightHeel.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(8, _bodyMap[body.BodyId].RightAnkle.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer1
                    .SetPosition(9, _bodyMap[body.BodyId].RightTiptoe.transform.localPosition);


                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(0, _bodyMap[body.BodyId].RightEar.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(1, _bodyMap[body.BodyId].RightEye.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2.SetPosition(2, _bodyMap[body.BodyId].Nose.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2.SetPosition(3, _bodyMap[body.BodyId].Neck.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2.SetPosition(4, _bodyMap[body.BodyId].Root.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(5, _bodyMap[body.BodyId].LeftHip.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(6, _bodyMap[body.BodyId].LeftKnee.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(7, _bodyMap[body.BodyId].LeftHeel.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(8, _bodyMap[body.BodyId].LeftAnkle.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer2
                    .SetPosition(9, _bodyMap[body.BodyId].LeftTiptoe.transform.localPosition);

                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(0, _bodyMap[body.BodyId].LeftHand.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(1, _bodyMap[body.BodyId].LeftWrist.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(2, _bodyMap[body.BodyId].LeftElbow.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(3, _bodyMap[body.BodyId].LeftShoulder.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3.SetPosition(4, _bodyMap[body.BodyId].Neck.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(5, _bodyMap[body.BodyId].RightShoulder.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(6, _bodyMap[body.BodyId].RightElbow.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(7, _bodyMap[body.BodyId].RightWrist.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer3
                    .SetPosition(8, _bodyMap[body.BodyId].RightHand.transform.localPosition);

                _bodyMap[body.BodyId].LineRenderer4
                    .SetPosition(0, _bodyMap[body.BodyId].HeadTop.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer4.SetPosition(1, _bodyMap[body.BodyId].Nose.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer4.SetPosition(2, _bodyMap[body.BodyId].Neck.transform.localPosition);
                _bodyMap[body.BodyId].LineRenderer4.SetPosition(3, _bodyMap[body.BodyId].Root.transform.localPosition);
            }
        }

        void InitObject(Body body)
        {
            var parentObject = body.Obj.transform;
           
            body.Nose = parentObject.Find("Nose");
            body.LeftEye = parentObject.Find("LeftEye");
            body.RightEye = parentObject.Find("RightEye");
            body.LeftEar = parentObject.Find("LeftEar");
            body.RightEar = parentObject.Find("RightEar");
            body.LeftShoulder = parentObject.Find("LeftShoulder");
            body.RightShoulder = parentObject.Find("RightShoulder");
            body.LeftElbow = parentObject.Find("LeftElbow");
            body.RightElbow = parentObject.Find("RightElbow");
            body.LeftWrist = parentObject.Find("LeftWrist");
            body.RightWrist = parentObject.Find("RightWrist");
            body.LeftHip = parentObject.Find("LeftHip");
            body.RightHip = parentObject.Find("RightHip");
            body.LeftKnee = parentObject.Find("LeftKnee");
            body.RightKnee = parentObject.Find("RightKnee");
            body.LeftAnkle = parentObject.Find("LeftAnkle");
            body.RightAnkle = parentObject.Find("RightAnkle");
            body.LeftTiptoe = parentObject.Find("LeftTiptoe");
            body.RightTiptoe = parentObject.Find("RightTiptoe");
            body.LeftHeel = parentObject.Find("LeftHeel");
            body.RightHeel = parentObject.Find("RightHeel");
            body.HeadTop = parentObject.Find("HeadTop");
            body.LeftHand = parentObject.Find("LeftHand");
            body.RightHand = parentObject.Find("RightHand");

            body.Neck = parentObject.Find("Neck");
            body.Root = parentObject.Find("Root");

            body.LineRenderer1 = body.Nose.GetComponent<LineRenderer>();
            body.LineRenderer2 = body.LeftHip.GetComponent<LineRenderer>();
            body.LineRenderer3 = body.LeftShoulder.GetComponent<LineRenderer>();
            body.LineRenderer4 = body.LeftElbow.GetComponent<LineRenderer>();
            body.LineRenderer1.positionCount = 10;
            body.LineRenderer2.positionCount = 10;
            body.LineRenderer3.positionCount = 9;
            body.LineRenderer4.positionCount = 4;
        }
    }
}