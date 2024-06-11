using Deepglint.XR.EventSystem.InputModules;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Manager
{
    public class ToolManager : MonoBehaviour
    {
        private FPS _fps;
        private VersionCode _versionCode;
        private GameObject _inGameDebugConsole;

        void Start()
        {
            GameObject uiBackGround = GameObject.Find("UIRoot")?.FindChildGameObject("UI_BackGround");
            if (uiBackGround != null)
            {
                uiBackGround.SetActive(false);
            }
            _fps = transform.GetComponent<FPS>();
            _versionCode = transform.GetComponent<VersionCode>();
            _inGameDebugConsole = GameObject.Find("IngameDebugConsole");
            bool openDebug = Global.Config.Debug;
            _fps.enabled = openDebug;
            _versionCode.enabled = openDebug;
            if (_inGameDebugConsole != null)
            {
                _inGameDebugConsole.SetActive(openDebug);
            }

            HumanControlFootPointerInputModule inputModule = GameObject.Find("UIRoot")?.FindChildGameObject("EventSystem")?.GetComponent<HumanControlFootPointerInputModule>();
            if (inputModule != null) inputModule.enableMouseEvent = openDebug;
        }
    }
}