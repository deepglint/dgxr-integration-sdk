using DGXR;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Animator _animator;
    private Source _source;
    void Start()
       {
           _animator = GetComponent<Animator>();
           _source = Source.Instance;
       }
   
       // Update is called once per frame
       void Update()
       {
           foreach (var body in _source.GetData())
           {
               var data = body.Value.GetLatest();
               _animator.GetBoneTransform(HumanBodyBones.Jaw).position = data.Joints[JointType.Nose].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftEye).position = data.Joints[JointType.LeftEar].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightEye).position = data.Joints[JointType.RightEar].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.Head).position = (data.Joints[JointType.LeftEar].Vector3()+data.Joints[JointType.RightEar].Vector3())/2;
               _animator.GetBoneTransform(HumanBodyBones.Hips).position = (data.Joints[JointType.LeftHip].Vector3()+data.Joints[JointType.RightHip].Vector3())/2;
               _animator.GetBoneTransform(HumanBodyBones.Spine).position = (data.Joints[JointType.LeftHip].Vector3()+data.Joints[JointType.RightHip].Vector3())/2;
               _animator.GetBoneTransform(HumanBodyBones.RightShoulder).position = data.Joints[JointType.RightShoulder].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position = data.Joints[JointType.LeftShoulder].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position = data.Joints[JointType.LeftElbow].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position = data.Joints[JointType.RightElbow].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftHand).position = data.Joints[JointType.LeftWrist].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightHand).position = data.Joints[JointType.RightWrist].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position = data.Joints[JointType.LeftHip].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position = data.Joints[JointType.RightHip].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position = data.Joints[JointType.LeftKnee].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position = data.Joints[JointType.RightKnee].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position = data.Joints[JointType.LeftAnkle].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightFoot).position = data.Joints[JointType.RightAnkle].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.LeftToes).position = data.Joints[JointType.LeftTiptoe].Vector3();
               _animator.GetBoneTransform(HumanBodyBones.RightToes).position = data.Joints[JointType.RightTiptoe].Vector3();
           }
       }
}
