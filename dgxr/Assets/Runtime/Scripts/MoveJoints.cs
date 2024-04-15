using System;
using System.Collections;
using UnityEngine;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DGXR;
using Stardust.log;

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

        if (Source.Data.Count==0)
        {
            // GameLogger.LogRed("errr");
            return;
        }

        var body = Source.Data[0];
        Nose.transform.localPosition = body.Joints.Nose;
        LeftEye.transform.localPosition = body.Joints.LeftEye;
        RightEye.transform.localPosition = body.Joints.RightEye;
        LeftEar.transform.localPosition = body.Joints.LeftEar;
        RightEar.transform.localPosition = body.Joints.RightEar;
        LeftShoulder.transform.localPosition = body.Joints.LeftShoulder;
        RightShoulder.transform.localPosition = body.Joints.RightShoulder;
        LeftElbow.transform.localPosition = body.Joints.LeftElbow;
        RightElbow.transform.localPosition = body.Joints.RightElbow;
        LeftWrist.transform.localPosition = body.Joints.LeftWrist;
        RightWrist.transform.localPosition = body.Joints.RightWrist;
        LeftHip.transform.localPosition = body.Joints.LeftHip;
        RightHip.transform.localPosition = body.Joints.RightHip;
        LeftKnee.transform.localPosition = body.Joints.LeftKnee;
        RightKnee.transform.localPosition = body.Joints.RightKnee;
        LeftAnkle.transform.localPosition = body.Joints.LeftAnkle;
        RightAnkle.transform.localPosition = body.Joints.RightAnkle;
        LeftTiptoe.transform.localPosition = body.Joints.LeftTiptoe;
        RightTiptoe.transform.localPosition = body.Joints.RightTiptoe;
        LeftHeel.transform.localPosition = body.Joints.LeftHeel;
        RightHeel.transform.localPosition = body.Joints.RightHeel;
        HeadTop.transform.localPosition = body.Joints.HeadTop;
        LeftHand.transform.localPosition = body.Joints.LeftHand;
        RightHand.transform.localPosition = body.Joints.RightHand;
        Neck.transform.localPosition = (body.Joints.LeftShoulder+body.Joints.RightShoulder)/2;
        Root.transform.localPosition = (body.Joints.LeftHip + body.Joints.RightHip) / 2;
        
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
   
}

