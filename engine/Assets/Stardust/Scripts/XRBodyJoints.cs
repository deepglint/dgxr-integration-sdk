using System;
using UnityEngine;
using BodySource;

namespace Assets.XR.Scripts
{
    public class VRBodyJoints : MonoBehaviour
    {

        public GameObject[] Joint = new GameObject[1];
        public GameObject BodySourceManager;
        private XRDGBodySource _bodyManager;
        void Start()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            _bodyManager = XRDGBodySource.Instance;
            for (int n = 1; n < arguments.Length; n++)
            {
                switch (arguments[n])
                {
                    case ("-arm"):
                        //Todo: Some behaviour
                        break;
                }
            }
        }

        void Update()
        {
            if (BodySourceManager == null)
            {
                return;
            }
            if (_bodyManager.Data != null)
            {
                var body = Body(_bodyManager.Data);
                if (body.BodyID == null || body.BodyID == "")
                    return;
                Joint[0].transform.localPosition = GetVector3FromJoint(body.Joints[BodySource.JointType.HeadTop]);
            }


        }

        private static Vector3 GetVector3FromJoint(JointData joint)
        {
            return new Vector3(-joint.X, joint.Y, joint.Z);
        }

        private BodyDataSource Body(System.Collections.Concurrent.ConcurrentDictionary<string, BodyDataSource> data)
        {
           
            if (_bodyManager.cavePersonId != "" && data.ContainsKey(_bodyManager.cavePersonId))
            {
                return data[_bodyManager.cavePersonId];
            }
            else if (_bodyManager.cavePersonId == "" || !data.ContainsKey(_bodyManager.cavePersonId))
            {
                foreach (var person in _bodyManager.Data)
                {
                    return person.Value;
                }
            }

            return new BodyDataSource { };
        }
    }
}
