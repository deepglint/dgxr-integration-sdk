using System.Collections;
using System.Collections.Generic;
using Moat.Model;
// using Moat.Model;
using UnityEngine;
// using DisplayData = Moat.Model.DisplayData;

namespace Moat 
{
    public class DisplayManager : MonoBehaviour 
    {
        private Camera cameraLeft;
        private Camera cameraFront;
        private Camera cameraRight;
        private Camera cameraBack;
        private Camera cameraBottom1; 
        private Camera cameraBottom2;

        private int systemWidth = 1920;
        private int systemHeight = 1200;
        private bool fullScreen = false;
        private bool projectionFusion = false;
        private float bottom1Y = -1200f;
        private float bottom2Y = -1920f;
    
        private DisplayConfigData _displayConfigData;
        // Start is called before the first frame update
        void Start()
        {
            // DisplayConfigData displayConfig = MReadData.ReadJsonFile<DisplayConfigData>(Application.streamingAssetsPath + "/display.json");
            DisplayData.ReadConfig();
            _displayConfigData = DisplayData.configDisplay;
            float resolution = _displayConfigData.resolution.systemWidth;
            systemWidth = _displayConfigData.resolution.systemWidth;
            systemHeight = _displayConfigData.resolution.systemHeight;
            fullScreen = _displayConfigData.resolution.fullScreen;
            projectionFusion = _displayConfigData.resolution.projectionFusion;
            bottom1Y = _displayConfigData.uiCameraPos.bottom1Y;
            bottom2Y = _displayConfigData.uiCameraPos.bottom2Y;
            
            SetViewport();
            // 如果是 isUIRender = false，执行此方法
            // SetUICamera();
            Invoke("SetDisplay", 0);
        }
    
        void SetViewport()
        {
        }

        void SetDisplay()
        {
            GameObject roam = GameObject.Find("Roam");
            if (roam != null)
            {
                CameraRoamControl roamControl = roam.GetComponent<CameraRoamControl>();
                if (roamControl.isCave)
                {
                    Set3DVRCamera();
                }
                else
                {
                    Set3DCamera();
                    // SetUICamera();
                }
            }
            else
            {
                Set3DVRCamera(); 
            }
            SetUICamera();
        }

        void Set3DCamera()
        {
            GameObject camera3D = GameObject.Find("SimpleVRManager");
            if (camera3D == null) return;
            Camera[] cameras = camera3D.GetComponentsInChildren<Camera>();

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera.name == "Left") cameraLeft = camera; 
                if (camera.name == "Front") cameraFront = camera; 
                if (camera.name == "Right") cameraRight = camera; 
                if (camera.name == "Back") cameraBack = camera; 
                if (camera.name == "Bottom1") cameraBottom1 = camera; 
                if (camera.name == "Bottom2") cameraBottom2 = camera; 
            }
            
            InitDisplay();  
        }

        void Set3DVRCamera()
        {
            GameObject cameraVR3D = GameObject.Find("VRManager");
            if (cameraVR3D == null) return;
            Camera[] cameras = cameraVR3D.GetComponentsInChildren<Camera>();

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera.name == "projector4") cameraLeft = camera; 
                if (camera.name == "projector1") cameraFront = camera; 
                if (camera.name == "projector2") cameraRight = camera; 
                if (camera.name == "projector3") cameraBack = camera; 
                if (camera.name == "projector5") cameraBottom1 = camera; 
                if (camera.name == "projector6") cameraBottom2 = camera; 
            }
            
            InitDisplay(); 
        }

        void SetUICamera()
        {
            GameObject cameraUi = GameObject.Find("2DCameraGroup");
            if (cameraUi == null) return;
            Camera[] cameras = cameraUi.GetComponentsInChildren<Camera>();

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera.name == "Left") cameraLeft = camera; 
                if (camera.name == "Front") cameraFront = camera; 
                if (camera.name == "Right") cameraRight = camera; 
                if (camera.name == "Back") cameraBack = camera; 
                if (camera.name == "Bottom1") cameraBottom1 = camera; 
                if (camera.name == "Bottom2") cameraBottom2 = camera; 
            }
            
            Transform Bottom1Camera = cameraBottom1.GetComponent<Transform>();
            Bottom1Camera.position = new Vector3(Bottom1Camera.position.x, bottom1Y, Bottom1Camera.position.z);
            Transform Bottom2Camera = cameraBottom2.GetComponent<Transform>();
            Bottom2Camera.position = new Vector3(Bottom2Camera.position.x, bottom2Y, Bottom2Camera.position.z);

            GameObject maskUICanvas = GameObject.Find("MaskUICanvas");
            if (maskUICanvas != null)
            {
                maskUICanvas.GetComponent<Canvas>().targetDisplay = DisplayData.configDisplay.targetDisplay.bottom1 - 1;
            }

            InitDisplay();
        }

        void InitDisplay()
        {
            if (_displayConfigData.targetDisplay.left != null) cameraLeft.targetDisplay = _displayConfigData.targetDisplay.left - 1;
            if (_displayConfigData.targetDisplay.front != null) cameraFront.targetDisplay = _displayConfigData.targetDisplay.front - 1;
            if (_displayConfigData.targetDisplay.right != null) cameraRight.targetDisplay = _displayConfigData.targetDisplay.right - 1;
            if (_displayConfigData.targetDisplay.back != null)
                cameraBack.targetDisplay = _displayConfigData.targetDisplay.back - 1;
            if (_displayConfigData.targetDisplay.bottom1 != null) cameraBottom1.targetDisplay = _displayConfigData.targetDisplay.bottom1 - 1; 
            if (_displayConfigData.targetDisplay.bottom2 != null) cameraBottom2.targetDisplay = _displayConfigData.targetDisplay.bottom2 - 1; 

            // 获取设置当前屏幕分辩率
            Resolution[] resolutions = Screen.resolutions;
            for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
            {
                // 设置显示器分辨率
                UnityEngine.Display display = UnityEngine.Display.displays[i];
    
                if (projectionFusion)
                {
                    MDebug.Log("projection fusion: " + display.systemWidth + "--" + display.systemHeight);
                    display.Activate(display.systemWidth - 1, display.systemHeight - 1, 30);
                    Screen.SetResolution(display.systemWidth - 1, display.systemHeight - 1, false);
                }
                else
                {
                    MDebug.Log("custom screen: " + systemWidth + "--" + systemHeight);
                    display.Activate(display.systemWidth, display.systemHeight, 30);
                    Screen.SetResolution(display.systemWidth, display.systemHeight, false);
                }
            }
    
            // 设置成全屏
            if (fullScreen)
            {
                Screen.fullScreen = true;
            }
        }
    }
}