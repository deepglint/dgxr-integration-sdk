using System.Collections.Generic;
using System.Runtime.InteropServices;
using Stardust.Model;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Stardust.Scripts
{
    public class DisplayManager : MonoBehaviour
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SmCxscreen = 0; // 主屏幕分辨率宽度
        private const int SmCyscreen = 1; // 主屏幕分辨率高度

        private int _screenWidth; // 屏幕宽度
        private int _screenHeight; // 屏幕高度

        private Camera _cameraLeft;
        private Camera _cameraFront;
        private Camera _cameraRight;
        private Camera _cameraBack;
        private Camera _cameraBottom1;
        private Camera _cameraBottom2;

        private float _bottom1Y = -1200f;
        private float _bottom2Y = -1920f;
        private float _bottom1X = 1920f;
        private float _bottom2X = 1920f;
        private float _bottom1RotationZ;
        private float _bottom2RotationZ;
        
        // private bool hasCamera3D = false;
        private bool _hasCameraXR;
        private bool _hasCameraUI;


        private DisplayConfigData _displayConfigData;

        private void Awake()
        {
            if (SystemInfo.operatingSystem.Contains("Windows"))
            {
                _screenWidth = GetSystemMetrics(SmCxscreen);
                _screenHeight = GetSystemMetrics(SmCyscreen);

                Debug.Log("当前屏幕宽度: " + _screenWidth + ", 高度: " + _screenHeight);
            }
            else
            {
                Debug.Log("非Windows系统");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GameObject camera3d = GameObject.Find("3DCameraGroup");
            camera3d.SetActive(false);
            DisplayData.ReadConfig();
            _displayConfigData = DisplayData.ConfigDisplay;
            _bottom1Y = _displayConfigData.UICameraPos.Bottom1Y;
            _bottom2Y = _displayConfigData.UICameraPos.Bottom2Y;
            _bottom1X = _displayConfigData.UICameraPos.Bottom1X;
            _bottom2X = _displayConfigData.UICameraPos.Bottom2X;
            _bottom1RotationZ = _displayConfigData.UICameraPos.Bottom1RotationZ;
            _bottom2RotationZ = _displayConfigData.UICameraPos.Bottom2RotationZ;
        }

        private void Update()
        {
            if (_displayConfigData.SingleSceneDebug)
            {
                SetDisplaySingle();
            }
            else
            {
                SetDisplay();
            }
        }

        void SetDisplay()
        {
            Set3DxrCamera();
            SetUICamera();
        }
        
        void SetDisplaySingle()
        {
            if (_hasCameraXR) return;
            Camera[] cameras = transform.GetComponentsInChildren<Camera>();
            if (cameras.Length > 0)
            {
                _hasCameraXR = true;
            }
            Screen.SetResolution(_screenWidth, _screenHeight, false);
            // 4123
            //  5
            //  6
            List<Rect> rects = new List<Rect>();
            foreach (var rectData in _displayConfigData.SingleSceneRect)
            {
                rects.Add(new Rect(rectData[0], rectData[1], rectData[2], rectData[3]));
            }

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera cameraItem = cameras[i];
                if (cameraItem.tag.Equals("Projector"))
                {
                    cameraItem.targetDisplay = 0;
                    foreach (var stackCamera in cameraItem.GetUniversalAdditionalCameraData().cameraStack)
                    {
                        stackCamera.targetDisplay = 0;
                    }
                    
                    if (cameraItem.name == "projector4") 
                        cameraItem.rect = rects[3];
                    if (cameraItem.name == "projector1") 
                        cameraItem.rect = rects[0];
                    if (cameraItem.name == "projector2") 
                        cameraItem.rect = rects[1];
                    if (cameraItem.name == "projector3")
                        cameraItem.rect = rects[2];
                    if (cameraItem.name == "projector5") 
                        cameraItem.rect = rects[4];
                    if (cameraItem.name == "projector6")
                        cameraItem.rect = rects[5];

                    if (cameraItem.name == "projector1")
                    {
                        Camera secondCamera = Instantiate(cameraItem.gameObject,cameraItem.transform.parent).GetComponent<Camera>();
                        secondCamera.GetComponent<QuadWarp>().Vertices = cameraItem.GetComponent<QuadWarp>().Vertices;
                        secondCamera.targetDisplay = 0;
                        secondCamera.rect = rects[6];
                    }
                }
            }
        }
        
        void Set3DxrCamera()
        {
            if (_hasCameraXR) return;
            Camera[] cameras = transform.GetComponentsInChildren<Camera>();
            GameObject cameraUi = GameObject.Find("XRManager");
            Camera[] cameras1 = cameraUi.GetComponentsInChildren<Camera>();
            if (cameras.Length > 0 && cameras1.Length > 0)
            {
                _hasCameraXR = true;
            }

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera cameraItem = cameras[i];
                if (cameraItem.name == "projector4") _cameraLeft = cameraItem;
                if (cameraItem.name == "projector1") _cameraFront = cameraItem;
                if (cameraItem.name == "projector2") _cameraRight = cameraItem;
                if (cameraItem.name == "projector3") _cameraBack = cameraItem;
                if (cameraItem.name == "projector5") _cameraBottom1 = cameraItem;
                if (cameraItem.name == "projector6") _cameraBottom2 = cameraItem;
            }

            InitDisplay();
        }

        void SetUICamera()
        {
            if (_hasCameraUI) return;
            GameObject cameraUi = GameObject.Find("2DCameraGroup");
            if (cameraUi == null) return;
            Camera[] cameras = cameraUi.GetComponentsInChildren<Camera>();
            if (cameras.Length > 0)
            {
                _hasCameraUI = true;
            }

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera cameraItem = cameras[i];
                if (cameraItem.name == "Left") _cameraLeft = cameraItem;
                if (cameraItem.name == "Front") _cameraFront = cameraItem;
                if (cameraItem.name == "Right") _cameraRight = cameraItem;
                if (cameraItem.name == "Back") _cameraBack = cameraItem;
                if (cameraItem.name == "Bottom1") _cameraBottom1 = cameraItem;
                if (cameraItem.name == "Bottom2") _cameraBottom2 = cameraItem;
            }

            Transform bottom1Camera = _cameraBottom1.GetComponent<Transform>();
            bottom1Camera.position = new Vector3(_bottom1X, _bottom1Y, bottom1Camera.position.z);
            var rotation = bottom1Camera.rotation;
            rotation =
                Quaternion.Euler(rotation.x, rotation.y, _bottom1RotationZ);
            bottom1Camera.rotation = rotation;
            Transform bottom2Camera = _cameraBottom2.GetComponent<Transform>();
            bottom2Camera.position = new Vector3(_bottom2X, _bottom2Y, bottom2Camera.position.z);
            var rotation1 = bottom2Camera.rotation;
            rotation1 =
                Quaternion.Euler(rotation1.x, rotation1.y, _bottom2RotationZ);
            bottom2Camera.rotation = rotation1;


            InitDisplay();
        }

        void InitDisplay()
        {
            if (_cameraLeft == null) return;
            _cameraLeft.targetDisplay = _displayConfigData.TargetDisplay.Left - 1;
            _cameraFront.targetDisplay = _displayConfigData.TargetDisplay.Front - 1;
            _cameraRight.targetDisplay = _displayConfigData.TargetDisplay.Right - 1;
            _cameraBack.targetDisplay = _displayConfigData.TargetDisplay.Back - 1;
            _cameraBottom1.targetDisplay = _displayConfigData.TargetDisplay.Bottom1 - 1;
            _cameraBottom2.targetDisplay = _displayConfigData.TargetDisplay.Bottom2 - 1;

            // 获取设置当前屏幕分辩率
            for (int i = 0; i < Display.displays.Length; i++)
            {
                // 设置显示器分辨率
                Display display = Display.displays[i];
                display.Activate(display.systemWidth, display.systemHeight, new RefreshRate()
                {
                    numerator = 30,
                    denominator = 1U
                });
            }

            Screen.SetResolution(_screenWidth, _screenHeight, true);
        }
    }
}