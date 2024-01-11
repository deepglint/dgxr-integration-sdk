using Moat.Model;
using UnityEngine;
using DGXR;

namespace Moat
{
    public class CameraRoamControl : MonoBehaviour
    {
        public static CameraRoamControl Instance;
        
        private Transform _currentCameraObj;

        private Camera _cameraLeft;
        private Camera _cameraFront;
        private Camera _cameraRight;
        private Camera _cameraBack;
        private Camera _cameraBottom1;
        private Camera _cameraBottom2;

        [HideInInspector]public float verticalInput;
        [HideInInspector]public float horizontalInput;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameObject cameraXR = GameObject.Find("XRManager");
            GameObject camera3D = GameObject.Find("3DCameraGroup");
           
            DisplayData.ReadConfig();
            if (DisplayData.allowCave)
            {
                if (cameraXR != null)
                {
                    _currentCameraObj = cameraXR.GetComponent<Transform>();
                    XRWorldManager.instance.SetCameraPosition(new Vector3(_currentCameraObj.position.x, DisplayData.HumanEye, _currentCameraObj.position.z));
                    if (camera3D != null) camera3D?.SetActive(false);
                }
            }
            else
            {
                if (camera3D != null)
                {
                    _currentCameraObj = camera3D.GetComponent<Transform>();
                    _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, DisplayData.HumanEye, _currentCameraObj.position.z);
                    if (cameraXR != null) cameraXR?.SetActive(false);
                }
            }

            if (!DisplayData.allowCave && camera3D != null)
            {
                Camera[] cameras = camera3D.GetComponentsInChildren<Camera>();

                // 遍历相机数组，访问每一个相机对象
                for (int i = 0; i < cameras.Length; i++)
                {
                    Camera camera = cameras[i];
                    if (camera.name == "Left") _cameraLeft = camera;
                    if (camera.name == "Front") _cameraFront = camera;
                    if (camera.name == "Right") _cameraRight = camera;
                    if (camera.name == "Back") _cameraBack = camera;
                    if (camera.name == "Bottom1") _cameraBottom1 = camera;
                    if (camera.name == "Bottom2") _cameraBottom2 = camera;
                }
            }
        }

        void Update()
        {
            if (verticalInput != 0 || horizontalInput != 0 && _currentCameraObj != null)
            {
                // 根据输入和速度移动物体
                _currentCameraObj.Translate(Vector3.forward * verticalInput * DisplayData.roamSpeed * Time.deltaTime);

                // 根据水平输入旋转物体
                _currentCameraObj.Rotate(Vector3.up * horizontalInput * DisplayData.roamRotationSpeed * Time.deltaTime);

                if (DisplayData.allowCave)
                {
                    UpdateXRCamera(_currentCameraObj.position);
                }
                else
                {
                    Update3DCamera(_currentCameraObj.position);
                }
            }
        }

        private void UpdateXRCamera(Vector3 headLockPosition)
        {
            XRWorldManager.instance.centerViewPoint =
                new Vector3(headLockPosition.x, XRWorldManager.instance.centerViewPoint.y, headLockPosition.z);
        }

        public void Update3DCamera(Vector3 headLockPosition)
        {
            MDebug.Log("headLockPosition: " + headLockPosition.ToString());
            _cameraLeft.transform.position = headLockPosition;
            _cameraFront.transform.position = headLockPosition;
            _cameraRight.transform.position = headLockPosition;
            _cameraBack.transform.position = headLockPosition;
            _cameraBottom1.transform.position = headLockPosition + new Vector3(0, 0, 3);
            _cameraBottom2.transform.position = headLockPosition + new Vector3(0, 0, -3);
        }

        public void Start(Vector2 move)
        {
            verticalInput = move.y;
            horizontalInput = move.x;
        }

        public void Stop()
        {
            verticalInput = 0;
            horizontalInput = 0;
        }
    }
}