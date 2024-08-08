using System;
using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.SharedComponents.CameraRoi;
using Deepglint.XR.Toolkit.SharedComponents.GameExitButton;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deepglint.XR.Toolkit
{
    [DefaultExecutionOrder(-90)]
    public class ToolkitManager : MonoBehaviour
    {
        const string IngameDebugConsolePrefabName = "IngameDebugConsole";
        const string ToolKitCanvasNodeName = "ToolkitCanvasNode";
        private FPS _fps;
        private VersionCode _versionCode;
        private GameObject _inGameDebugConsole;
        
        private GameObject _toolKitCanvasNode;

        private void Awake()
        {
            if (GameObject.Find("_prefabName") == null)
            {
                GameObject prefab = Instantiate(Resources.Load<GameObject>(IngameDebugConsolePrefabName));
                prefab.name = IngameDebugConsolePrefabName;
                DontDestroyOnLoad(prefab);
            }
        }


        private void InitToolkitCanvas()
        {
            _toolKitCanvasNode = GameObject.Find(ToolKitCanvasNodeName);
            if (_toolKitCanvasNode is null)
            {
                _toolKitCanvasNode = Instantiate(new GameObject(), null, false);
                _toolKitCanvasNode.name = ToolKitCanvasNodeName;
                var prefab = Resources.Load<GameObject>("ToolkitCanvas");
                var toolkitCanvas = Instantiate(prefab, _toolKitCanvasNode.transform, false);
                toolkitCanvas.name = prefab.name;
                var canvas = toolkitCanvas.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = DGXR.Space.Bottom.UICamera;
                DontDestroyOnLoad(_toolKitCanvasNode);
                GameExitButton.CreateComponent();
                CameraRoi.CreateComponent();
            }
        }

        private void Update()
        {
            _toolKitCanvasNode.transform.SetPositionAndRotation(transform.position, transform.rotation);
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