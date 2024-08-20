using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit
{
    [DefaultExecutionOrder(-90)]
    public class ToolkitManager : MonoBehaviour
    {
        const string IngameDebugConsolePrefabName = "IngameDebugConsole";
        private FPS _fps;
        private VersionCode _versionCode;
        private GameObject _inGameDebugConsole;

        private GameObject _toolKitCanvas;

     

        private void UpdateToolkitCanvas()
        {
            _toolKitCanvas = GameObject.Find("ToolkitCanvas");
            var bottomCanvas = _toolKitCanvas.FindChildGameObject("Bottom").GetComponent<Canvas>();
            bottomCanvas.renderMode = RenderMode.WorldSpace;
            bottomCanvas.worldCamera = DGXR.Space.Bottom.UICamera;
            UpdateMouseEvent();
        }

        private void UpdateMouseEvent()
        {
            HumanControlFootPointerInputModule inputModule = GameObject.Find("UIRoot")
                ?.FindChildGameObject("EventSystem")?.GetComponent<HumanControlFootPointerInputModule>();
            if (inputModule != null) inputModule.enableMouseEvent = DGXR.Config.Debug;
        }


        private void Start()
        {
            UpdateToolkitCanvas();
            XRManager.OnXrConstantNodeInit += UpdateToolkitCanvas;
            GameObject uiBackGround = GameObject.Find("UIRoot")?.FindChildGameObject("UI_BackGround");
            if (uiBackGround != null)
            {
                uiBackGround.SetActive(false);
            }

            _fps = transform.GetComponent<FPS>();
            _versionCode = transform.GetComponent<VersionCode>();
            _inGameDebugConsole = GameObject.Find(IngameDebugConsolePrefabName);
            bool openDebug = DGXR.Config.Debug;
            _fps.enabled = openDebug;
            _versionCode.enabled = openDebug;
            if (_inGameDebugConsole != null)
            {
                _inGameDebugConsole.SetActive(openDebug);
            }

            UpdateMouseEvent();
        }
    }
}