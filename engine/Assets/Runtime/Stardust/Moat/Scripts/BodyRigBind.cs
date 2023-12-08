using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BodySource;

namespace Moat
{
    public class BodyRigBind : MonoBehaviour
    {
        #region definition

        public Transform parentObject;

        public float speed = 5f;

        //line renderer*4  
        private LineRenderer lineRenderer1;
        private LineRenderer lineRenderer2;
        private LineRenderer lineRenderer3;
        private LineRenderer lineRenderer4;
        private VRDGBodySource _bodyManager;
        private int lineLength = 300;
        public string PersonId;
        public bool PersonStatus;
        public bool allowSourceControl = true;

        Transform Nose;
        Transform LeftEye;
        Transform RightEye;
        Transform LeftEar;
        Transform RightEar;
        Transform LeftShoulder;
        Transform RightShoulder;
        Transform LeftElbow;
        Transform RightElbow;
        Transform LeftWrist;
        Transform RightWrist;
        Transform LeftHip;
        Transform RightHip;
        Transform LeftKnee;
        Transform RightKnee;
        Transform LeftAnkle;
        Transform RightAnkle;
        Transform LeftTiptoe;
        Transform RightTiptoe;
        Transform LeftHeel;
        Transform RightHeel;
        Transform HeadTop;
        Transform LeftHand;
        Transform RightHand;

        Transform Neck;
        Transform Root;

        #endregion

        //将场景的物体关联到该脚本中
        void InitObject()
        {
            Nose = parentObject.Find("Nose");
            LeftEye = parentObject.Find("LeftEye");
            RightEye = parentObject.Find("RightEye");
            LeftEar = parentObject.Find("LeftEar");
            RightEar = parentObject.Find("RightEar");
            LeftShoulder = parentObject.Find("LeftShoulder");
            RightShoulder = parentObject.Find("RightShoulder");
            LeftElbow = parentObject.Find("LeftElbow");
            RightElbow = parentObject.Find("RightElbow");
            LeftWrist = parentObject.Find("LeftWrist");
            RightWrist = parentObject.Find("RightWrist");
            LeftHip = parentObject.Find("LeftHip");
            RightHip = parentObject.Find("RightHip");
            LeftKnee = parentObject.Find("LeftKnee");
            RightKnee = parentObject.Find("RightKnee");
            LeftAnkle = parentObject.Find("LeftAnkle");
            RightAnkle = parentObject.Find("RightAnkle");
            LeftTiptoe = parentObject.Find("LeftTiptoe");
            RightTiptoe = parentObject.Find("RightTiptoe");
            LeftHeel = parentObject.Find("LeftHeel");
            RightHeel = parentObject.Find("RightHeel");
            HeadTop = parentObject.Find("HeadTop");
            LeftHand = parentObject.Find("LeftHand");
            RightHand = parentObject.Find("RightHand");

            Neck = parentObject.Find("Neck");
            Root = parentObject.Find("Root");

            //   Line1 = GameObject.Find("Line1");

            lineRenderer1 = (LineRenderer)Nose.GetComponent("LineRenderer");
            lineRenderer2 = (LineRenderer)LeftHip.GetComponent("LineRenderer");
            lineRenderer3 = (LineRenderer)LeftShoulder.GetComponent("LineRenderer");
            lineRenderer4 = (LineRenderer)LeftElbow.GetComponent("LineRenderer");
        }

        // Use this for initialization
        void Start()
        {
            _bodyManager = VRDGBodySource.Instance;

            InitObject();
            lineRenderer1.positionCount = lineLength;
            lineRenderer2.positionCount = lineLength;
            lineRenderer3.positionCount = lineLength;
            lineRenderer4.positionCount = lineLength;

            lineRenderer1.positionCount = 10;
            lineRenderer2.positionCount = 10;
            lineRenderer3.positionCount = 9;
            lineRenderer4.positionCount = 4;
        }

        void Update()
        {
            float step = speed * Time.deltaTime;

            var body = Body(_bodyManager.GetData());
            if (allowSourceControl == false) return;
            if (body.BodyID == "" || body.BodyID == null) return;

            Nose.transform.localPosition = new Vector3(body.Joints[JointType.Nose].X, body.Joints[JointType.Nose].Z,
                body.Joints[JointType.Nose].Y);
            LeftEye.transform.localPosition = new Vector3(body.Joints[JointType.LeftEye].X,
                body.Joints[JointType.LeftEye].Z, body.Joints[JointType.LeftEye].Y);
            RightEye.transform.localPosition = new Vector3(body.Joints[JointType.RightEye].X,
                body.Joints[JointType.RightEye].Z, body.Joints[JointType.RightEye].Y);
            LeftEar.transform.localPosition = new Vector3(body.Joints[JointType.LeftEar].X,
                body.Joints[JointType.LeftEar].Z, body.Joints[JointType.LeftEar].Y);
            RightEar.transform.localPosition = new Vector3(body.Joints[JointType.RightEar].X,
                body.Joints[JointType.RightEar].Z, body.Joints[JointType.RightEar].Y);
            LeftShoulder.transform.localPosition = new Vector3(body.Joints[JointType.LeftShoulder].X,
                body.Joints[JointType.LeftShoulder].Z, body.Joints[JointType.LeftShoulder].Y);
            RightShoulder.transform.localPosition = new Vector3(body.Joints[JointType.RightShoulder].X,
                body.Joints[JointType.RightShoulder].Z, body.Joints[JointType.RightShoulder].Y);
            LeftElbow.transform.localPosition = new Vector3(body.Joints[JointType.LeftElbow].X,
                body.Joints[JointType.LeftElbow].Z, body.Joints[JointType.LeftElbow].Y);
            RightElbow.transform.localPosition = new Vector3(body.Joints[JointType.RightElbow].X,
                body.Joints[JointType.RightElbow].Z, body.Joints[JointType.RightElbow].Y);
            LeftWrist.transform.localPosition = new Vector3(body.Joints[JointType.LeftWrist].X,
                body.Joints[JointType.LeftWrist].Z, body.Joints[JointType.LeftWrist].Y);
            RightWrist.transform.localPosition = new Vector3(body.Joints[JointType.RightWrist].X,
                body.Joints[JointType.RightWrist].Z, body.Joints[JointType.RightWrist].Y);
            LeftHip.transform.localPosition = new Vector3(body.Joints[JointType.LeftHip].X,
                body.Joints[JointType.LeftHip].Z, body.Joints[JointType.LeftHip].Y);
            RightHip.transform.localPosition = new Vector3(body.Joints[JointType.RightHip].X,
                body.Joints[JointType.RightHip].Z, body.Joints[JointType.RightHip].Y);
            LeftKnee.transform.localPosition = new Vector3(body.Joints[JointType.LeftKnee].X,
                body.Joints[JointType.LeftKnee].Z, body.Joints[JointType.LeftKnee].Y);
            RightKnee.transform.localPosition = new Vector3(body.Joints[JointType.RightKnee].X,
                body.Joints[JointType.RightKnee].Z, body.Joints[JointType.RightKnee].Y);
            LeftAnkle.transform.localPosition = new Vector3(body.Joints[JointType.LeftAnkle].X,
                body.Joints[JointType.LeftAnkle].Z, body.Joints[JointType.LeftAnkle].Y);
            RightAnkle.transform.localPosition = new Vector3(body.Joints[JointType.RightAnkle].X,
                body.Joints[JointType.RightAnkle].Z, body.Joints[JointType.RightAnkle].Y);
            LeftTiptoe.transform.localPosition = new Vector3(body.Joints[JointType.LeftTiptoe].X,
                body.Joints[JointType.LeftTiptoe].Z, body.Joints[JointType.LeftTiptoe].Y);
            RightTiptoe.transform.localPosition = new Vector3(body.Joints[JointType.RightTiptoe].X,
                body.Joints[JointType.RightTiptoe].Z, body.Joints[JointType.RightTiptoe].Y);
            LeftHeel.transform.localPosition = new Vector3(body.Joints[JointType.LeftHeel].X,
                body.Joints[JointType.LeftHeel].Z, body.Joints[JointType.LeftHeel].Y);
            RightHeel.transform.localPosition = new Vector3(body.Joints[JointType.RightHeel].X,
                body.Joints[JointType.RightHeel].Z, body.Joints[JointType.RightHeel].Y);
            HeadTop.transform.localPosition = new Vector3(body.Joints[JointType.HeadTop].X,
                body.Joints[JointType.HeadTop].Z, body.Joints[JointType.HeadTop].Y);
            LeftHand.transform.localPosition = new Vector3(body.Joints[JointType.LeftHand].X,
                body.Joints[JointType.LeftHand].Z, body.Joints[JointType.LeftHand].Y);
            RightHand.transform.localPosition = new Vector3(body.Joints[JointType.RightHand].X,
                body.Joints[JointType.RightHand].Z, body.Joints[JointType.RightHand].Y);

            Neck.transform.localPosition = new Vector3(
                (body.Joints[JointType.LeftShoulder].X + body.Joints[JointType.RightShoulder].X) / 2,
                (body.Joints[JointType.LeftShoulder].Z + body.Joints[JointType.RightShoulder].Z) / 2,
                (body.Joints[JointType.LeftShoulder].Y + body.Joints[JointType.RightShoulder].Y) / 2);
            Root.transform.localPosition = new Vector3(
                (body.Joints[JointType.LeftHip].X + body.Joints[JointType.RightHip].X) / 2,
                (body.Joints[JointType.LeftHip].Z + body.Joints[JointType.RightHip].Z) / 2,
                (body.Joints[JointType.LeftHip].Y + body.Joints[JointType.RightHip].Y) / 2);

            //it requires 4 lines to connect all the joints
            lineRenderer1.SetPosition(0, LeftEar.transform.localPosition);
            lineRenderer1.SetPosition(1, LeftEye.transform.localPosition);
            lineRenderer1.SetPosition(2, Nose.transform.localPosition);
            lineRenderer1.SetPosition(3, Neck.transform.localPosition);
            lineRenderer1.SetPosition(4, Root.transform.localPosition);
            lineRenderer1.SetPosition(5, RightHip.transform.localPosition);
            lineRenderer1.SetPosition(6, RightKnee.transform.localPosition);
            lineRenderer1.SetPosition(7, RightHeel.transform.localPosition);
            lineRenderer1.SetPosition(8, RightAnkle.transform.localPosition);
            lineRenderer1.SetPosition(9, RightTiptoe.transform.localPosition);


            lineRenderer2.SetPosition(0, RightEar.transform.localPosition);
            lineRenderer2.SetPosition(1, RightEye.transform.localPosition);
            lineRenderer2.SetPosition(2, Nose.transform.localPosition);
            lineRenderer2.SetPosition(3, Neck.transform.localPosition);
            lineRenderer2.SetPosition(4, Root.transform.localPosition);
            lineRenderer2.SetPosition(5, LeftHip.transform.localPosition);
            lineRenderer2.SetPosition(6, LeftKnee.transform.localPosition);
            lineRenderer2.SetPosition(7, LeftHeel.transform.localPosition);
            lineRenderer2.SetPosition(8, LeftAnkle.transform.localPosition);
            lineRenderer2.SetPosition(9, LeftTiptoe.transform.localPosition);

            lineRenderer3.SetPosition(0, LeftHand.transform.localPosition);
            lineRenderer3.SetPosition(1, LeftWrist.transform.localPosition);
            lineRenderer3.SetPosition(2, LeftElbow.transform.localPosition);
            lineRenderer3.SetPosition(3, LeftShoulder.transform.localPosition);
            lineRenderer3.SetPosition(4, Neck.transform.localPosition);
            lineRenderer3.SetPosition(5, RightShoulder.transform.localPosition);
            lineRenderer3.SetPosition(6, RightElbow.transform.localPosition);
            lineRenderer3.SetPosition(7, RightWrist.transform.localPosition);
            lineRenderer3.SetPosition(8, RightHand.transform.localPosition);

            lineRenderer4.SetPosition(0, HeadTop.transform.localPosition);
            lineRenderer4.SetPosition(1, Nose.transform.localPosition);
            lineRenderer4.SetPosition(2, Neck.transform.localPosition);
            lineRenderer4.SetPosition(3, Root.transform.localPosition);
        }

        private BodyDataSource Body(System.Collections.Concurrent.ConcurrentDictionary<string, BodyDataSource> data)
        {
            if (PersonId != "" && data.ContainsKey(PersonId))
            {
                PersonStatus = true;
                transform.gameObject.SetActive(PersonStatus);
                return data[PersonId];
            }

            PersonStatus = false;
            transform.gameObject.SetActive(PersonStatus);

            return new BodyDataSource { };
        }
    }
}