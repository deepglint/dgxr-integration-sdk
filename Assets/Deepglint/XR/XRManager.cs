using System;
using Deepglint.XR.Config;
using Deepglint.XR.Inputs;
using Deepglint.XR.Log;
using Deepglint.XR.Source;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR
{
    [DefaultExecutionOrder(-100)]
    public class XRManager : MonoBehaviour
    {
        const string XrDontDestroyName = "DGXR_DontDestroy";
        public bool isFilterZero;
        public static GameObject XRDontDestroy;

        public void Awake()
        {
            InitXrConstantNode();
            // 获取当前的 Input System 设置
            var inputSystemSettings = InputSystem.settings;
            // 修改 maxEventBytesPerUpdate 属性为0 移除限制
            inputSystemSettings.maxEventBytesPerUpdate = 0;
            // 应用修改后的设置
            InputSystem.settings = inputSystemSettings;
            
            DGXR.UniqueID = SystemInfo.deviceUniqueIdentifier;
            DGXR.AppName = Application.productName;
            DGXR.AppVersion = Application.version;
            DGXR.Version = VersionCode.SdkVersionCode;
            DGXR.SystemName = SystemInfo.operatingSystem;
            DGXR.Config = new Config.Config().InitConfig();
            XRApplicationSettings settings = Resources.Load<XRApplicationSettings>("XRApplicationSettings");
            if (settings != null)
            {
                DGXR.Settings = settings;
                DeviceManager.MaxActiveHumanDeviceCount = settings.playerSetting.maxPlayerCount;
            }
            else
            {
#if !UNITY_EDITOR
                Debug.LogError("load XRApplicationSettings failed");
#endif
            }
            
            
#if !UNITY_EDITOR
            if (DGXR.Config.Space.ScreenMode is ScreenStyle.Default)
            {
                Cursor.visible = false;
            }
#endif
            
            GameLogger.Init(DGXR.Config.Log);
            if (UseRos())
            {
                var ros = Extends.FindChildGameObject(gameObject,"RosConnect" );
                Source.Source.DataFrom = SourceType.ROS;
                ros.SetActive(true);
            }
            else
            {
                var ws = Extends.FindChildGameObject(gameObject,"WsConnect" );
                Source.Source.DataFrom = SourceType.WS;
                ws.SetActive(true);
            }
        }

        private void InitXrConstantNode()
        {
            XRDontDestroy = GameObject.Find(XrDontDestroyName);
            if (XRDontDestroy is null)
            {
                XRDontDestroy = new GameObject(XrDontDestroyName);
                DontDestroyOnLoad(XRDontDestroy);
            }
        }
        
        private void Update()
        {
            XRDontDestroy.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        public void Start()
        {
            DGXR.Space = XRSpace.Instance;
            DGXR.IsFilterZero = isFilterZero;
        }

        private bool UseRos()
        {
            if (!Application.isEditor && !DGXR.SystemName.Contains("Mac"))
            {
                return true;
            }

            return false;
        }
        
        public void OnDestroy()
        {
            GameLogger.Cleanup();
        }
    }
}
