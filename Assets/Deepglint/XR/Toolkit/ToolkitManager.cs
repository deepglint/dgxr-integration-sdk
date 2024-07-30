using System;
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
            var toolkitCanvas = GameObject.Find("ToolkitCanvas");
            if (toolkitCanvas is null)
            {
                var prefab = Resources.Load<GameObject>("ToolkitCanvas");
                var newToolkitCanvas = Instantiate(prefab, this.transform.parent, false);
                newToolkitCanvas.name = prefab.name;
                var canvas = newToolkitCanvas.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = DGXR.Space.Bottom.UICamera;
            }
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