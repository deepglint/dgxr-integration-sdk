using System;
using System.Collections;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using DGXR;

public class MoveJoints : MonoBehaviour
{
    #region definition
    public Transform parentObject;
    public float speed = 5f;
    //line renderer*4  
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;
    private LineRenderer lineRenderer4;
    private DGXR.Source _bodyManager;
    private int lineLength = 300;

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

        Neck= parentObject.Find("Neck");
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
        _bodyManager = DGXR.Source.Instance;

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

    // Update is called once per frame

    void Update()
    {
        float step = speed * Time.deltaTime;

        var body = Body(_bodyManager.GetData());
        if (String.IsNullOrEmpty(body.FrameId) || body.FrameId=="")
        {
            return;
        }
        Nose.transform.localPosition = new Vector3((float)body.Joints[JointType.Nose].X, (float)body.Joints[JointType.Nose].Z, (float)body.Joints[JointType.Nose].Y);
        LeftEye.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftEye].X, (float)body.Joints[JointType.LeftEye].Z, (float)body.Joints[JointType.LeftEye].Y);
        RightEye.transform.localPosition = new Vector3((float)body.Joints[JointType.RightEye].X, (float)body.Joints[JointType.RightEye].Z, (float)body.Joints[JointType.RightEye].Y);
        LeftEar.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftEar].X, (float)body.Joints[JointType.LeftEar].Z, (float)body.Joints[JointType.LeftEar].Y);
        RightEar.transform.localPosition = new Vector3((float)body.Joints[JointType.RightEar].X, (float)body.Joints[JointType.RightEar].Z, (float)body.Joints[JointType.RightEar].Y);
        LeftShoulder.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftShoulder].X, (float)body.Joints[JointType.LeftShoulder].Z, (float)body.Joints[JointType.LeftShoulder].Y);
        RightShoulder.transform.localPosition = new Vector3((float)body.Joints[JointType.RightShoulder].X, (float)body.Joints[JointType.RightShoulder].Z, (float)body.Joints[JointType.RightShoulder].Y);
        LeftElbow.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftElbow].X, (float)body.Joints[JointType.LeftElbow].Z, (float)body.Joints[JointType.LeftElbow].Y);
        RightElbow.transform.localPosition = new Vector3((float)body.Joints[JointType.RightElbow].X, (float)body.Joints[JointType.RightElbow].Z, (float)body.Joints[JointType.RightElbow].Y);
        LeftWrist.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftWrist].X, (float)body.Joints[JointType.LeftWrist].Z, (float)body.Joints[JointType.LeftWrist].Y);
        RightWrist.transform.localPosition = new Vector3((float)body.Joints[JointType.RightWrist].X, (float)body.Joints[JointType.RightWrist].Z, (float)body.Joints[JointType.RightWrist].Y);
        LeftHip.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftHip].X, (float)body.Joints[JointType.LeftHip].Z, (float)body.Joints[JointType.LeftHip].Y);
        RightHip.transform.localPosition = new Vector3((float)body.Joints[JointType.RightHip].X, (float)body.Joints[JointType.RightHip].Z, (float)body.Joints[JointType.RightHip].Y);
        LeftKnee.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftKnee].X, (float)body.Joints[JointType.LeftKnee].Z, (float)body.Joints[JointType.LeftKnee].Y);
        RightKnee.transform.localPosition = new Vector3((float)body.Joints[JointType.RightKnee].X, (float)body.Joints[JointType.RightKnee].Z, (float)body.Joints[JointType.RightKnee].Y);
        LeftAnkle.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftAnkle].X, (float)body.Joints[JointType.LeftAnkle].Z, (float)body.Joints[JointType.LeftAnkle].Y);
        RightAnkle.transform.localPosition = new Vector3((float)body.Joints[JointType.RightAnkle].X, (float)body.Joints[JointType.RightAnkle].Z, (float)body.Joints[JointType.RightAnkle].Y);
        LeftTiptoe.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftTiptoe].X, (float)body.Joints[JointType.LeftTiptoe].Z, (float)body.Joints[JointType.LeftTiptoe].Y);
        RightTiptoe.transform.localPosition = new Vector3((float)body.Joints[JointType.RightTiptoe].X, (float)body.Joints[JointType.RightTiptoe].Z, (float)body.Joints[JointType.RightTiptoe].Y);
        LeftHeel.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftHeel].X, (float)body.Joints[JointType.LeftHeel].Z, (float)body.Joints[JointType.LeftHeel].Y);
        RightHeel.transform.localPosition = new Vector3((float)body.Joints[JointType.RightHeel].X, (float)body.Joints[JointType.RightHeel].Z, (float)body.Joints[JointType.RightHeel].Y);
        HeadTop.transform.localPosition = new Vector3((float)body.Joints[JointType.HeadTop].X, (float)body.Joints[JointType.HeadTop].Z, (float)body.Joints[JointType.HeadTop].Y);
        LeftHand.transform.localPosition = new Vector3((float)body.Joints[JointType.LeftHand].X, (float)body.Joints[JointType.LeftHand].Z, (float)body.Joints[JointType.LeftHand].Y);
        RightHand.transform.localPosition = new Vector3((float)body.Joints[JointType.RightHand].X, (float)body.Joints[JointType.RightHand].Z, (float)body.Joints[JointType.RightHand].Y);

        Neck.transform.localPosition = new Vector3(((float)body.Joints[JointType.LeftShoulder].X+ (float)body.Joints[JointType.RightShoulder].X)/2,((float)body.Joints[JointType.LeftShoulder].Z + (float)body.Joints[JointType.RightShoulder].Z) / 2,((float)body.Joints[JointType.LeftShoulder].Y + (float)body.Joints[JointType.RightShoulder].Y) / 2);
        Root.transform.localPosition = new Vector3(((float)body.Joints[JointType.LeftHip].X + (float)body.Joints[JointType.RightHip].X) / 2, ((float)body.Joints[JointType.LeftHip].Z + (float)body.Joints[JointType.RightHip].Z) / 2, ((float)body.Joints[JointType.LeftHip].Y + (float)body.Joints[JointType.RightHip].Y) / 2);

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

        lineRenderer4.SetPosition(0,HeadTop.transform.localPosition);
        lineRenderer4.SetPosition(1, Nose.transform.localPosition);
        lineRenderer4.SetPosition(2, Neck.transform.localPosition);
        lineRenderer4.SetPosition(3, Root.transform.localPosition);

    }

    private DGXR.Source.BodyDataSource Body( ConcurrentDictionary<string, DGXR.Source.BodyData> data)
    {
        foreach (var val in data)
        {
            return val.Value.GetLatest();
        } 
        return new Source.BodyDataSource() { };
    }
}

