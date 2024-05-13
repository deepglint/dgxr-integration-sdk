using Deepglint.Tools.DebugTool;
using UnityEngine;

namespace Deepglint.Tools.Manager
{
    public class ToolManager : MonoBehaviour
    {
        private FPS _fps;
        private VersionCode _versionCode;
        private GameObject _inGameDebugConsole;

        void Start()
        {
            DataManager.Init();
            _fps = transform.GetComponent<FPS>();
            _versionCode = transform.GetComponent<VersionCode>();
            _inGameDebugConsole = GameObject.Find("IngameDebugConsole");
            bool openDebug = DataManager.Config.OpenDebug;
            _fps.enabled = openDebug;
            _versionCode.enabled = openDebug;
            _inGameDebugConsole.SetActive(openDebug);
        }
    }
}