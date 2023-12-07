using System;
using UnityEngine;

namespace Moat
{
    public class RotationControl : MonoBehaviour
    {
        public float rotationSpeed = 5f; // 旋转速度
        public float rotationThreshold = 10f; // 角度阈值

        private Quaternion _targetRotation; // 目标旋转角度

        public float maxThreshold = 500f; //最大可视旋转距离

        private GameObject[] _leftCloseShotGo;
        private GameObject[] _rightCloseShotGo;


        private void Start()
        {
            _leftCloseShotGo = GameObject.FindGameObjectsWithTag("LeftCloseShot");
            _rightCloseShotGo = GameObject.FindGameObjectsWithTag("RightCloseShot");
        }

        void Update()
        {
            Rotate();
        }

        void Rotate()
        {
            foreach (var leftGO in _leftCloseShotGo)
            {
                RectTransform leftGoRectTransform = leftGO.GetComponent<RectTransform>();
                Vector3 leftGOWorldPosition = leftGoRectTransform.TransformPoint(Vector3.zero);
                float distance = leftGOWorldPosition.z - transform.position.z;
                if (distance < maxThreshold)
                {
                    // 朝向相机
                    Vector3 targetDirection = leftGO.transform.position - transform.position;
                    _targetRotation = Quaternion.LookRotation(targetDirection);
                    // 平滑旋转
                    leftGO.transform.rotation =
                        Quaternion.Lerp(leftGO.transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
                }
            }

            foreach (var rightGO in _rightCloseShotGo)
            {
                float distance = Vector3.Distance(rightGO.transform.position, transform.position);

                if (distance < maxThreshold)
                {
                    // 朝向相机
                    Vector3 targetDirection = rightGO.transform.position - transform.position;
                    _targetRotation = Quaternion.LookRotation(targetDirection);
                    // 平滑旋转
                    rightGO.transform.rotation =
                        Quaternion.Lerp(rightGO.transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}