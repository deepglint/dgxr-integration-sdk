using Runtime.Scripts;
using UnityEngine;

public class MoveJoints : MonoBehaviour
{
    #region definition
    public Transform parentObject;
    public float speed = 5f;
    //line renderer*4  
    private LineRenderer _lineRenderer1;
    private LineRenderer _lineRenderer2;
    private LineRenderer _lineRenderer3;
    private LineRenderer _lineRenderer4;
    private const int LineLength = 300;

    private Transform _nose;
    private Transform _leftEye;
    private Transform _rightEye;
    private Transform _leftEar;
    private Transform _rightEar;
    private Transform _leftShoulder;
    private Transform _rightShoulder;
    private Transform _leftElbow;
    private Transform _rightElbow;
    private Transform _leftWrist;
    private Transform _rightWrist;
    private Transform _leftHip;
    private Transform _rightHip;
    private Transform _leftKnee;
    private Transform _rightKnee;
    private Transform _leftAnkle;
    private Transform _rightAnkle;
    private Transform _leftTiptoe;
    private Transform _rightTiptoe;
    private Transform _leftHeel;
    private Transform _rightHeel;
    private Transform _headTop;
    private Transform _leftHand;
    private Transform _rightHand;

    private Transform _neck;
    private Transform _root;

    #endregion

    //将场景的物体关联到该脚本中
    void InitObject()
    {
        _nose = parentObject.Find("Nose");
        _leftEye = parentObject.Find("LeftEye");
        _rightEye = parentObject.Find("RightEye");
        _leftEar = parentObject.Find("LeftEar");
        _rightEar = parentObject.Find("RightEar");
        _leftShoulder = parentObject.Find("LeftShoulder");
        _rightShoulder = parentObject.Find("RightShoulder");
        _leftElbow = parentObject.Find("LeftElbow");
        _rightElbow = parentObject.Find("RightElbow");
        _leftWrist = parentObject.Find("LeftWrist");
        _rightWrist = parentObject.Find("RightWrist");
        _leftHip = parentObject.Find("LeftHip");
        _rightHip = parentObject.Find("RightHip");
        _leftKnee = parentObject.Find("LeftKnee");
        _rightKnee = parentObject.Find("RightKnee");
        _leftAnkle = parentObject.Find("LeftAnkle");
        _rightAnkle = parentObject.Find("RightAnkle");
        _leftTiptoe = parentObject.Find("LeftTiptoe");
        _rightTiptoe = parentObject.Find("RightTiptoe");
        _leftHeel = parentObject.Find("LeftHeel");
        _rightHeel = parentObject.Find("RightHeel");
        _headTop = parentObject.Find("HeadTop");
        _leftHand = parentObject.Find("LeftHand");
        _rightHand = parentObject.Find("RightHand");

        _neck= parentObject.Find("Neck");
        _root = parentObject.Find("Root");

        _lineRenderer1 =_nose.GetComponent<LineRenderer>();
        _lineRenderer2 = _leftHip.GetComponent<LineRenderer>();
        _lineRenderer3 = _leftShoulder.GetComponent<LineRenderer>();
        _lineRenderer4 = _leftElbow.GetComponent<LineRenderer>();
    }

    void Start()
    {
        InitObject();
        _lineRenderer1.positionCount = LineLength;
        _lineRenderer2.positionCount = LineLength;
        _lineRenderer3.positionCount = LineLength;
        _lineRenderer4.positionCount = LineLength;

        _lineRenderer1.positionCount = 10;
        _lineRenderer2.positionCount = 10;
        _lineRenderer3.positionCount = 9;
        _lineRenderer4.positionCount = 4;
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
        _nose.transform.localPosition = body.Joints.Nose;
        _leftEye.transform.localPosition = body.Joints.LeftEye;
        _rightEye.transform.localPosition = body.Joints.RightEye;
        _leftEar.transform.localPosition = body.Joints.LeftEar;
        _rightEar.transform.localPosition = body.Joints.RightEar;
        _leftShoulder.transform.localPosition = body.Joints.LeftShoulder;
        _rightShoulder.transform.localPosition = body.Joints.RightShoulder;
        _leftElbow.transform.localPosition = body.Joints.LeftElbow;
        _rightElbow.transform.localPosition = body.Joints.RightElbow;
        _leftWrist.transform.localPosition = body.Joints.LeftWrist;
        _rightWrist.transform.localPosition = body.Joints.RightWrist;
        _leftHip.transform.localPosition = body.Joints.LeftHip;
        _rightHip.transform.localPosition = body.Joints.RightHip;
        _leftKnee.transform.localPosition = body.Joints.LeftKnee;
        _rightKnee.transform.localPosition = body.Joints.RightKnee;
        _leftAnkle.transform.localPosition = body.Joints.LeftAnkle;
        _rightAnkle.transform.localPosition = body.Joints.RightAnkle;
        _leftTiptoe.transform.localPosition = body.Joints.LeftTiptoe;
        _rightTiptoe.transform.localPosition = body.Joints.RightTiptoe;
        _leftHeel.transform.localPosition = body.Joints.LeftHeel;
        _rightHeel.transform.localPosition = body.Joints.RightHeel;
        _headTop.transform.localPosition = body.Joints.HeadTop;
        _leftHand.transform.localPosition = body.Joints.LeftHand;
        _rightHand.transform.localPosition = body.Joints.RightHand;
        _neck.transform.localPosition = (body.Joints.LeftShoulder+body.Joints.RightShoulder)/2;
        _root.transform.localPosition = (body.Joints.LeftHip + body.Joints.RightHip) / 2;
        
        //it requires 4 lines to connect all the joints
        _lineRenderer1.SetPosition(0, _leftEar.transform.localPosition);
        _lineRenderer1.SetPosition(1, _leftEye.transform.localPosition);
        _lineRenderer1.SetPosition(2, _nose.transform.localPosition);
        _lineRenderer1.SetPosition(3, _neck.transform.localPosition);
        _lineRenderer1.SetPosition(4, _root.transform.localPosition);
        _lineRenderer1.SetPosition(5, _rightHip.transform.localPosition);
        _lineRenderer1.SetPosition(6, _rightKnee.transform.localPosition);
        _lineRenderer1.SetPosition(7, _rightHeel.transform.localPosition);
        _lineRenderer1.SetPosition(8, _rightAnkle.transform.localPosition);
        _lineRenderer1.SetPosition(9, _rightTiptoe.transform.localPosition);


        _lineRenderer2.SetPosition(0, _rightEar.transform.localPosition);
        _lineRenderer2.SetPosition(1, _rightEye.transform.localPosition);
        _lineRenderer2.SetPosition(2, _nose.transform.localPosition);
        _lineRenderer2.SetPosition(3, _neck.transform.localPosition);
        _lineRenderer2.SetPosition(4, _root.transform.localPosition);
        _lineRenderer2.SetPosition(5, _leftHip.transform.localPosition);
        _lineRenderer2.SetPosition(6, _leftKnee.transform.localPosition);
        _lineRenderer2.SetPosition(7, _leftHeel.transform.localPosition);
        _lineRenderer2.SetPosition(8, _leftAnkle.transform.localPosition);
        _lineRenderer2.SetPosition(9, _leftTiptoe.transform.localPosition);

        _lineRenderer3.SetPosition(0, _leftHand.transform.localPosition);
        _lineRenderer3.SetPosition(1, _leftWrist.transform.localPosition);
        _lineRenderer3.SetPosition(2, _leftElbow.transform.localPosition);
        _lineRenderer3.SetPosition(3, _leftShoulder.transform.localPosition);
        _lineRenderer3.SetPosition(4, _neck.transform.localPosition);
        _lineRenderer3.SetPosition(5, _rightShoulder.transform.localPosition);
        _lineRenderer3.SetPosition(6, _rightElbow.transform.localPosition);
        _lineRenderer3.SetPosition(7, _rightWrist.transform.localPosition);
        _lineRenderer3.SetPosition(8, _rightHand.transform.localPosition);

        _lineRenderer4.SetPosition(0,_headTop.transform.localPosition);
        _lineRenderer4.SetPosition(1, _nose.transform.localPosition);
        _lineRenderer4.SetPosition(2, _neck.transform.localPosition);
        _lineRenderer4.SetPosition(3, _root.transform.localPosition);
    }
   
}

