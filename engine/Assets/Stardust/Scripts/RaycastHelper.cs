using UnityEngine;
using System;
using BodySource;

public class RaycastHelper : MonoBehaviour
{
    [Header("射线相关配置")] public Boolean ShowRay = false;
    public bool ShowRayLine = false;
    public float RayDistance = 2000f;
    public LayerMask RayMask = -1;
    public Color RayLineHitColor = Color.red;
    public Color RayLineNoHitColor = Color.green;
    private XRDGBodySource _bodyManager;

    void Start()
    {
        _bodyManager = XRDGBodySource.Instance;
    }

    void Update()
    {
        SetRay();
    }

    private void SetRay()
    {
        if (ShowRay)
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

                if (Physics.Raycast(bodyData.LeftRay, out bodyData.LeftHit, RayDistance, RayMask))
                {
                    if (ShowRayLine && bodyData.LeftHit.collider)
                    {
                        Debug.DrawLine(bodyData.LeftRay.origin, bodyData.LeftHit.point, RayLineHitColor);
                    }
                }
                else
                {
                    if (ShowRayLine)
                    {
                        Debug.DrawLine(bodyData.LeftRay.origin,
                            bodyData.LeftRay.origin + bodyData.LeftRay.direction * RayDistance, RayLineNoHitColor);
                    }
                }
                
                if (Physics.Raycast(bodyData.RightRay, out bodyData.RightHit, RayDistance, RayMask))
                {
                    if (ShowRayLine && bodyData.RightHit.collider)
                    {
                        Debug.DrawLine(bodyData.RightRay.origin, bodyData.RightHit.point, RayLineHitColor);
                    }
                }
                else
                {
                    if (ShowRayLine)
                    {
                        Debug.DrawLine(bodyData.RightRay.origin,
                            bodyData.RightRay.origin + bodyData.RightRay.direction * RayDistance, RayLineNoHitColor);
                    }
                }

                XRDGBodySource.Instance.Data[person.Key] = bodyData;
            }
        }
    }
}