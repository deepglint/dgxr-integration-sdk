using Stardust.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stardust.Scripts
{
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
        private XrdgBodySource _bodyManager;
        //private int _lineLength = 300;
        [FormerlySerializedAs("WsSource")] public Source wsSource;

        Transform _nose;
        Transform _leftEye;
        Transform _rightEye;
        Transform _leftEar;
        Transform _rightEar;
        Transform _leftShoulder;
        Transform _rightShoulder;
        Transform _leftElbow;
        Transform _rightElbow;
        Transform _leftWrist;
        Transform _rightWrist;
        Transform _leftHip;
        Transform _rightHip;
        Transform _leftKnee;
        Transform _rightKnee;
        Transform _leftAnkle;
        Transform _rightAnkle;
        Transform _leftTiptoe;
        Transform _rightTiptoe;
        Transform _leftHeel;
        Transform _rightHeel;
        Transform _headTop;
        Transform _leftHand;
        Transform _rightHand;

        // Transform _neck;
        // Transform _root;

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
            // _neck= parentObject.Find("Neck");
            // _root = parentObject.Find("Root");

            //   Line1 = GameObject.Find("Line1");

            _lineRenderer1 = _nose.GetComponent<LineRenderer>();
            _lineRenderer2 = _leftHip.GetComponent<LineRenderer>();
            _lineRenderer3 = _leftShoulder.GetComponent<LineRenderer>();
            _lineRenderer4 = _leftElbow.GetComponent<LineRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            if (wsSource == null) return;
            DisplayData.WsConnect = true;
            wsSource.Init(new Options());
        
            _bodyManager = XrdgBodySource.Instance;

            InitObject();
            // _lineRenderer1.positionCount = _lineLength;
            // _lineRenderer2.positionCount = _lineLength;
            // _lineRenderer3.positionCount = _lineLength;
            // _lineRenderer4.positionCount = _lineLength;

            _lineRenderer1.positionCount = 10;
            _lineRenderer2.positionCount = 10;
            _lineRenderer3.positionCount = 9;
            _lineRenderer4.positionCount = 4;
        }

        // Update is called once per frame
        void Update()
        {
            var body = Body(_bodyManager.GetData());
            if (string.IsNullOrEmpty(body.BodyID))
            {
                return;
            }
            MDebug.Log(body.BodyID + "_bodyManager.GetData().Count: " + _bodyManager.GetData().Count);

            Transform[] joints = new Transform[] {
                _nose, _leftEye, _rightEye, _leftEar, _rightEar, _leftShoulder, _rightShoulder,
                _leftElbow, _rightElbow, _leftWrist, _rightWrist, _leftHip, _rightHip, _leftKnee,
                _rightKnee, _leftAnkle, _rightAnkle, _leftTiptoe, _rightTiptoe, _leftHeel, _rightHeel,
                _headTop, _leftHand, _rightHand
            };

            JointType[] jointTypes = new JointType[] {
                JointType.Nose, JointType.LeftEye, JointType.RightEye, JointType.LeftEar,
                JointType.RightEar, JointType.LeftShoulder, JointType.RightShoulder,
                JointType.LeftElbow, JointType.RightElbow, JointType.LeftWrist, JointType.RightWrist,
                JointType.LeftHip, JointType.RightHip, JointType.LeftKnee, JointType.RightKnee,
                JointType.LeftAnkle, JointType.RightAnkle, JointType.LeftTiptoe, JointType.RightTiptoe,
                JointType.LeftHeel, JointType.RightHeel, JointType.HeadTop, JointType.LeftHand, JointType.RightHand
            };

            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].transform.localPosition = new Vector3(
                    body.Joints[jointTypes[i]].X, body.Joints[jointTypes[i]].Z, body.Joints[jointTypes[i]].Y);
            }

            #region 替换掉的代码
            // _nose.transform.localPosition = new Vector3(body.Joints[JointType.Nose].X, body.Joints[JointType.Nose].Z, body.Joints[JointType.Nose].Y);
            // _leftEye.transform.localPosition = new Vector3(body.Joints[JointType.LeftEye].X, body.Joints[JointType.LeftEye].Z, body.Joints[JointType.LeftEye].Y);
            // _rightEye.transform.localPosition = new Vector3(body.Joints[JointType.RightEye].X, body.Joints[JointType.RightEye].Z, body.Joints[JointType.RightEye].Y);
            // _leftEar.transform.localPosition = new Vector3(body.Joints[JointType.LeftEar].X, body.Joints[JointType.LeftEar].Z, body.Joints[JointType.LeftEar].Y);
            // _rightEar.transform.localPosition = new Vector3(body.Joints[JointType.RightEar].X, body.Joints[JointType.RightEar].Z, body.Joints[JointType.RightEar].Y);
            // _leftShoulder.transform.localPosition = new Vector3(body.Joints[JointType.LeftShoulder].X, body.Joints[JointType.LeftShoulder].Z, body.Joints[JointType.LeftShoulder].Y);
            // _rightShoulder.transform.localPosition = new Vector3(body.Joints[JointType.RightShoulder].X, body.Joints[JointType.RightShoulder].Z, body.Joints[JointType.RightShoulder].Y);
            // _leftElbow.transform.localPosition = new Vector3(body.Joints[JointType.LeftElbow].X, body.Joints[JointType.LeftElbow].Z, body.Joints[JointType.LeftElbow].Y);
            // _rightElbow.transform.localPosition = new Vector3(body.Joints[JointType.RightElbow].X, body.Joints[JointType.RightElbow].Z, body.Joints[JointType.RightElbow].Y);
            // _leftWrist.transform.localPosition = new Vector3(body.Joints[JointType.LeftWrist].X, body.Joints[JointType.LeftWrist].Z, body.Joints[JointType.LeftWrist].Y);
            // _rightWrist.transform.localPosition = new Vector3(body.Joints[JointType.RightWrist].X, body.Joints[JointType.RightWrist].Z, body.Joints[JointType.RightWrist].Y);
            // _leftHip.transform.localPosition = new Vector3(body.Joints[JointType.LeftHip].X, body.Joints[JointType.LeftHip].Z, body.Joints[JointType.LeftHip].Y);
            // _rightHip.transform.localPosition = new Vector3(body.Joints[JointType.RightHip].X, body.Joints[JointType.RightHip].Z, body.Joints[JointType.RightHip].Y);
            // _leftKnee.transform.localPosition = new Vector3(body.Joints[JointType.LeftKnee].X, body.Joints[JointType.LeftKnee].Z, body.Joints[JointType.LeftKnee].Y);
            // _rightKnee.transform.localPosition = new Vector3(body.Joints[JointType.RightKnee].X, body.Joints[JointType.RightKnee].Z, body.Joints[JointType.RightKnee].Y);
            // _leftAnkle.transform.localPosition = new Vector3(body.Joints[JointType.LeftAnkle].X, body.Joints[JointType.LeftAnkle].Z, body.Joints[JointType.LeftAnkle].Y);
            // _rightAnkle.transform.localPosition = new Vector3(body.Joints[JointType.RightAnkle].X, body.Joints[JointType.RightAnkle].Z, body.Joints[JointType.RightAnkle].Y);
            // _leftTiptoe.transform.localPosition = new Vector3(body.Joints[JointType.LeftTiptoe].X, body.Joints[JointType.LeftTiptoe].Z, body.Joints[JointType.LeftTiptoe].Y);
            // _rightTiptoe.transform.localPosition = new Vector3(body.Joints[JointType.RightTiptoe].X, body.Joints[JointType.RightTiptoe].Z, body.Joints[JointType.RightTiptoe].Y);
            // _leftHeel.transform.localPosition = new Vector3(body.Joints[JointType.LeftHeel].X, body.Joints[JointType.LeftHeel].Z, body.Joints[JointType.LeftHeel].Y);
            // _rightHeel.transform.localPosition = new Vector3(body.Joints[JointType.RightHeel].X, body.Joints[JointType.RightHeel].Z, body.Joints[JointType.RightHeel].Y);
            // _headTop.transform.localPosition = new Vector3(body.Joints[JointType.HeadTop].X, body.Joints[JointType.HeadTop].Z, body.Joints[JointType.HeadTop].Y);
            // _leftHand.transform.localPosition = new Vector3(body.Joints[JointType.LeftHand].X, body.Joints[JointType.LeftHand].Z, body.Joints[JointType.LeftHand].Y);
            // _rightHand.transform.localPosition = new Vector3(body.Joints[JointType.RightHand].X, body.Joints[JointType.RightHand].Z, body.Joints[JointType.RightHand].Y);
            #endregion


            Vector3 neckLocalPosition = new Vector3((body.Joints[JointType.LeftShoulder].X+ body.Joints[JointType.RightShoulder].X)/2,(body.Joints[JointType.LeftShoulder].Z + body.Joints[JointType.RightShoulder].Z) / 2,(body.Joints[JointType.LeftShoulder].Y + body.Joints[JointType.RightShoulder].Y) / 2);
            Vector3 rootLocalPosition = new Vector3((body.Joints[JointType.LeftHip].X + body.Joints[JointType.RightHip].X) / 2, (body.Joints[JointType.LeftHip].Z + body.Joints[JointType.RightHip].Z) / 2, (body.Joints[JointType.LeftHip].Y + body.Joints[JointType.RightHip].Y) / 2);
        
            //it requires 4 lines to connect all the joints
            _lineRenderer1.SetPosition(0, _leftEar.transform.localPosition);
            _lineRenderer1.SetPosition(1, _leftEye.transform.localPosition);
            _lineRenderer1.SetPosition(2, joints[0].localPosition);
            _lineRenderer1.SetPosition(3, neckLocalPosition);
            _lineRenderer1.SetPosition(4, rootLocalPosition);
            _lineRenderer1.SetPosition(5, _rightHip.transform.localPosition);
            _lineRenderer1.SetPosition(6, _rightKnee.transform.localPosition);
            _lineRenderer1.SetPosition(7, _rightHeel.transform.localPosition);
            _lineRenderer1.SetPosition(8, _rightAnkle.transform.localPosition);
            _lineRenderer1.SetPosition(9, _rightTiptoe.transform.localPosition);


            _lineRenderer2.SetPosition(0, _rightEar.transform.localPosition);
            _lineRenderer2.SetPosition(1, _rightEye.transform.localPosition);
            _lineRenderer2.SetPosition(2, joints[0].localPosition);
            _lineRenderer2.SetPosition(3, neckLocalPosition);
            _lineRenderer2.SetPosition(4, rootLocalPosition);
            _lineRenderer2.SetPosition(5, _leftHip.transform.localPosition);
            _lineRenderer2.SetPosition(6, _leftKnee.transform.localPosition);
            _lineRenderer2.SetPosition(7, _leftHeel.transform.localPosition);
            _lineRenderer2.SetPosition(8, _leftAnkle.transform.localPosition);
            _lineRenderer2.SetPosition(9, _leftTiptoe.transform.localPosition);

            _lineRenderer3.SetPosition(0, _leftHand.transform.localPosition);
            _lineRenderer3.SetPosition(1, _leftWrist.transform.localPosition);
            _lineRenderer3.SetPosition(2, _leftElbow.transform.localPosition);
            _lineRenderer3.SetPosition(3, _leftShoulder.transform.localPosition);
            _lineRenderer3.SetPosition(4, neckLocalPosition);
            _lineRenderer3.SetPosition(5, _rightShoulder.transform.localPosition);
            _lineRenderer3.SetPosition(6, _rightElbow.transform.localPosition);
            _lineRenderer3.SetPosition(7, _rightWrist.transform.localPosition);
            _lineRenderer3.SetPosition(8, _rightHand.transform.localPosition);

            _lineRenderer4.SetPosition(0,_headTop.transform.localPosition);
            _lineRenderer4.SetPosition(1, joints[0].localPosition);
            _lineRenderer4.SetPosition(2, neckLocalPosition);
            _lineRenderer4.SetPosition(3, rootLocalPosition);

        }

        private string _personId;

        private BodyDataSource Body(System.Collections.Concurrent.ConcurrentDictionary<string, BodyDataSource> data)
        {
            int i = 0;
            foreach (var person in _bodyManager.Data)
            {
                if (i == 0)
                {
                    _personId = person.Key;
                }

                i += 1;
            }
            if (_personId != "" && data.TryGetValue(_personId,out var personValue))
            {
                return personValue;
            }
            else if (_bodyManager.CavePersonId != "" && data.TryGetValue(_bodyManager.CavePersonId,out var cavePersonValue))
            {
                return cavePersonValue;
            }
            else if (_bodyManager.CavePersonId == "" || !data.ContainsKey(_bodyManager.CavePersonId))
            {
                foreach (var person in _bodyManager.Data)
                {
                    return person.Value;
                }
            }

            return new BodyDataSource();
        }
    }
}

