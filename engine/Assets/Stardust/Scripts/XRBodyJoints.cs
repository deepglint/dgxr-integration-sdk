using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stardust.Scripts
{
    public class VRBodyJoints : MonoBehaviour
    {

        [FormerlySerializedAs("Joint")] public GameObject[] joint = new GameObject[1];
        [FormerlySerializedAs("BodySourceManager")] public GameObject bodySourceManager;
        private XrdgBodySource _bodyManager;
        void Start()
        {
            String[] arguments = Environment.GetCommandLineArgs();
            _bodyManager = XrdgBodySource.Instance;
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
            if (bodySourceManager == null)
            {
                return;
            }
            if (_bodyManager.Data != null)
            {
                var body = Body(_bodyManager.Data);
                if (body.BodyID == null || body.BodyID == "")
                    return;
                joint[0].transform.localPosition = GetVector3FromJoint(body.Joints[JointType.HeadTop]);
            }


        }

        private static Vector3 GetVector3FromJoint(JointData joint)
        {
            return new Vector3(joint.X, joint.Z, joint.Y);
        }

        private BodyDataSource Body(System.Collections.Concurrent.ConcurrentDictionary<string, BodyDataSource> data)
        {
           
            if (_bodyManager.CavePersonId != "" && data.ContainsKey(_bodyManager.CavePersonId))
            {
                return data[_bodyManager.CavePersonId];
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
