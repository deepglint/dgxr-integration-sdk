using Moat.Model;
using UnityEngine;
using DGXR;

namespace Moat
{
    public class CameraRoamControl : MonoBehaviour
    {
        public static CameraRoamControl Instance;
        public float Speed = 10f; // 移动速度
        public float RotationSpeed = 30f; // 旋转速度
        // public float Width = 5;
        // public float Height = 3.2f;

        // public GameObject camera3D;
        // public GameObject cameraXR;
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
            
            if (DGXRConfig.Instance.isCave)
            {
                if (cameraXR != null)
                {
                    _currentCameraObj = cameraXR.GetComponent<Transform>();
                    _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, DGXRConfig.Instance.ViewHeight, _currentCameraObj.position.z);
                    if (DGXRConfig.Instance.allowSetViewCenterPoint)
                    {
                        UpdateXRCamera(new Vector3(_currentCameraObj.position.x, DGXRConfig.Instance.ViewHeight, _currentCameraObj.position.z)); 
                    }
                    if (camera3D != null) camera3D?.SetActive(false);;
                }
            }
            else
            {
                if (camera3D != null)
                {
                    _currentCameraObj = camera3D.GetComponent<Transform>();
                    _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, DGXRConfig.Instance.ViewHeight, _currentCameraObj.position.z);
                    if (cameraXR != null) cameraXR?.SetActive(false);
                }
            }

            if (!DGXRConfig.Instance.isCave && camera3D != null)
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
            // 获取按键输入
            if (DGXRConfig.Instance.isRoam && DisplayData.configDisplay.playerCount <= 0)
            {
                verticalInput = Input.GetAxis("Vertical"); // W和S键
                horizontalInput = Input.GetAxis("Horizontal"); // A和D键 
            }

            if (verticalInput != 0 || horizontalInput != 0 && _currentCameraObj != null)
            {
                // 根据输入和速度移动物体
                _currentCameraObj.Translate(Vector3.forward * verticalInput * Speed * Time.deltaTime);

                // 根据水平输入旋转物体
                _currentCameraObj.Rotate(Vector3.up * horizontalInput * RotationSpeed * Time.deltaTime);

                if (DGXRConfig.Instance.isCave)
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
            MDebug.Log("headLockPosition: " + headLockPosition.ToString());
            XRWorldManager.instance.centerViewPoint =
                new Vector3(headLockPosition.x, headLockPosition.y, headLockPosition.z);
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