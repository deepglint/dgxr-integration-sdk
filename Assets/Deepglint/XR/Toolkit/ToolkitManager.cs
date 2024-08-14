using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Monitor.Alert;
using Deepglint.XR.Toolkit.SharedComponents.CameraRoi;
using Deepglint.XR.Toolkit.SharedComponents.GameExitButton;
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

        private void Awake()
        {
            if (GameObject.Find("_prefabName") == null)
            {
                GameObject prefab = Instantiate(Resources.Load<GameObject>(IngameDebugConsolePrefabName), XRManager.XRDontDestroy.transform, false);
                prefab.name = IngameDebugConsolePrefabName;
            }
        }


        private void InitToolkitCanvas()
        {
            _toolKitCanvas = GameObject.Find("ToolkitCanvas");
            if (_toolKitCanvas is null)
            {
                var prefab = Resources.Load<GameObject>("ToolkitCanvas");
                _toolKitCanvas = Instantiate(prefab, XRManager.XRDontDestroy.transform, false);
                _toolKitCanvas.name = prefab.name;
                GameExitButton.Create();
                CameraRoi.Create();
                Alert.Create();
            }
            var bottomCanvas = _toolKitCanvas.FindChildGameObject("Bottom").GetComponent<Canvas>();
            bottomCanvas.renderMode = RenderMode.WorldSpace;
            bottomCanvas.worldCamera = DGXR.Space.Bottom.UICamera;
        }

        private void Start()
        {
            InitToolkitCanvas();
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

            HumanControlFootPointerInputModule inputModule = GameObject.Find("UIRoot")
                ?.FindChildGameObject("EventSystem")?.GetComponent<HumanControlFootPointerInputModule>();
            if (inputModule != null) inputModule.enableMouseEvent = openDebug;
        }
    }
}