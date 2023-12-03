using System.Collections;
using System.Collections.Generic;
using Moat.Model;
using UnityEngine;
using DisplayData = Moat.Model.DisplayData;

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
    
        private DisplayConfigData _displayConfigData;
        // Start is called before the first frame update
        void Start()
        {
            // DisplayConfigData displayConfig = DataUtils.ReadJsonFile<DisplayConfigData>(Application.streamingAssetsPath + "/json/display.json");
            DisplayData.ReadConfig();
            _displayConfigData = DisplayData.configDisplay;
            float resolution = _displayConfigData.resolution.systemWidth;
            systemWidth = _displayConfigData.resolution.systemWidth;
            systemHeight = _displayConfigData.resolution.systemHeight;
            fullScreen = _displayConfigData.resolution.fullScreen;
            projectionFusion = _displayConfigData.resolution.projectionFusion;
            
            SetViewport(resolution);
            SetUICamera();
        }
    
        void SetViewport(float resolution)
        {
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
                switch (camera.name)
                {
                    case "Left":
                        cameraLeft = camera; 
                        break;
                    case "Front":
                        cameraFront = camera;
                        break;
                    case "Right":
                        cameraRight = camera;
                        break;
                    case "Back":
                        cameraBack = camera;
                        break;
                    case "Bottom1":
                        cameraBottom1 = camera;
                        break;
                    case "Bottom2":
                        cameraBottom2 = camera;
                        break;
                } 
            }
            
            Transform Bottom1Camera = cameraBottom1.GetComponent<Transform>();
            Bottom1Camera.position = new Vector3(Bottom1Camera.position.x, Model.DisplayData.configDisplay.uiCameraPos.bottom1Y, Bottom1Camera.position.z);
            Transform Bottom2Camera = cameraBottom2.GetComponent<Transform>();
            Bottom2Camera.position = new Vector3(Bottom2Camera.position.x, Model.DisplayData.configDisplay.uiCameraPos.bottom2Y, Bottom2Camera.position.z);
        }

        void InitDisplay()
        {
            if (_displayConfigData.targetDisplay.left > 0) cameraLeft.targetDisplay = _displayConfigData.targetDisplay.left - 1;
            if (_displayConfigData.targetDisplay.front > 0) cameraFront.targetDisplay = _displayConfigData.targetDisplay.front - 1;
            if (_displayConfigData.targetDisplay.right > 0) cameraRight.targetDisplay = _displayConfigData.targetDisplay.right - 1;
            if (_displayConfigData.targetDisplay.back > 0)
                cameraBack.targetDisplay = _displayConfigData.targetDisplay.back - 1;
            if (_displayConfigData.targetDisplay.bottom1 > 0) cameraBottom1.targetDisplay = _displayConfigData.targetDisplay.bottom1 - 1; 
            if (_displayConfigData.targetDisplay.bottom2 > 0) cameraBottom2.targetDisplay = _displayConfigData.targetDisplay.bottom2 - 1; 

            // 获取设置当前屏幕分辩率
            Resolution[] resolutions = Screen.resolutions;
            for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
            {
                // 设置显示器分辨率
                UnityEngine.Display display = UnityEngine.Display.displays[i];
    
                if (projectionFusion)
                {
                    Debug.Log("投影融合分辨率" + display.systemWidth + "--" + display.systemHeight);
                    display.Activate(display.systemWidth - 1, display.systemHeight - 1, 30);
                    Screen.SetResolution(display.systemWidth - 1, display.systemHeight - 1, false);
                }
                else
                {
                    Debug.Log("自定义分辨率" + systemWidth + "--" + systemHeight);
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