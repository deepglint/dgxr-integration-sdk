using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stardust.Scripts
{
    public class RaycastHelper : MonoBehaviour
    {
        [FormerlySerializedAs("ShowRay")] [Header("射线相关配置")] public Boolean showRay;
        [FormerlySerializedAs("ShowRayLine")] public bool showRayLine;
        [FormerlySerializedAs("RayDistance")] public float rayDistance = 2000f;
        [FormerlySerializedAs("RayMask")] public LayerMask rayMask = -1;
        [FormerlySerializedAs("RayLineHitColor")] public Color rayLineHitColor = Color.red;
        [FormerlySerializedAs("RayLineNoHitColor")] public Color rayLineNoHitColor = Color.green;
        private XrdgBodySource _bodyManager;

        void Start()
        {
            _bodyManager = XrdgBodySource.Instance;
        }

        void Update()
        {
            SetRay();
        }

        private void SetRay()
        {
            if (showRay)
            {
                foreach (var person in _bodyManager.Data)
                {
                    Vector3 leftPosition = new Vector3(person.Value.Joints[JointType.LeftWrist].X,
                        person.Value.Joints[JointType.LeftWrist].Z, person.Value.Joints[JointType.LeftWrist].Y);
                    Vector3 rightPosition = new Vector3(person.Value.Joints[JointType.RightWrist].X,
                        person.Value.Joints[JointType.RightWrist].Z, person.Value.Joints[JointType.RightWrist].Y);
                    Vector3 leftDic = new Vector3(person.Value.Joints[JointType.LeftHand].X,
                        person.Value.Joints[JointType.LeftHand].Z, person.Value.Joints[JointType.LeftHand].Y);
                    Vector3 rightDic = new Vector3(person.Value.Joints[JointType.RightHand].X,
                        person.Value.Joints[JointType.RightHand].Z, person.Value.Joints[JointType.RightHand].Y);

                    var bodyData = person.Value;
                    bodyData.LeftRay.origin = leftPosition;
                    bodyData.RightRay.origin = rightPosition;
                    bodyData.LeftRay.direction = leftDic - leftPosition;
                    bodyData.RightRay.direction = rightDic - rightPosition;

                    if (Physics.Raycast(bodyData.LeftRay, out bodyData.LeftHit, rayDistance, rayMask))
                    {
                        if (showRayLine && bodyData.LeftHit.collider)
                        {
                            Debug.DrawLine(bodyData.LeftRay.origin, bodyData.LeftHit.point, rayLineHitColor);
                        }
                    }
                    else
                    {
                        if (showRayLine)
                        {
                            Debug.DrawLine(bodyData.LeftRay.origin,
                                bodyData.LeftRay.origin + bodyData.LeftRay.direction * rayDistance, rayLineNoHitColor);
                        }
                    }
                
                    if (Physics.Raycast(bodyData.RightRay, out bodyData.RightHit, rayDistance, rayMask))
                    {
                        if (showRayLine && bodyData.RightHit.collider)
                        {
                            Debug.DrawLine(bodyData.RightRay.origin, bodyData.RightHit.point, rayLineHitColor);
                        }
                    }
                    else
                    {
                        if (showRayLine)
                        {
                            Debug.DrawLine(bodyData.RightRay.origin,
                                bodyData.RightRay.origin + bodyData.RightRay.direction * rayDistance, rayLineNoHitColor);
                        }
                    }

                    XrdgBodySource.Instance.Data[person.Key] = bodyData;
                }
            }
        }
    }
}