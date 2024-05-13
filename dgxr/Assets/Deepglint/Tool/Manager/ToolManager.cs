using Deepglint.Tools.DebugTool;
using Deepglint.XR;
using UnityEngine;

namespace Deepglint.Tool.Manager
{
    public class ToolManager : MonoBehaviour
    {
        private FPS _fps;
        private VersionCode _versionCode;
        private GameObject _inGameDebugConsole;

        void Start()
        {
            _fps = transform.GetComponent<FPS>();
            _versionCode = transform.GetComponent<VersionCode>();
            _inGameDebugConsole = GameObject.Find("IngameDebugConsole");
            bool openDebug = Global.Config.Debug;
            _fps.enabled = openDebug;
            _versionCode.enabled = openDebug;
            _inGameDebugConsole.SetActive(openDebug);
        }
    }
}