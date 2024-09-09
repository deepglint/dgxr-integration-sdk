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
        const string XrDontDestroyName = "XRManager_DontDestroy";
        public bool isFilterZero;
        public static GameObject XRDontDestroy;
        public static Action OnXrConstantNodeInit;

        public void Awake()
        {
            // 获取当前的 Input System 设置
            var inputSystemSettings = InputSystem.settings;
            // 修改 maxEventBytesPerUpdate 属性为0 移除限制
            inputSystemSettings.maxEventBytesPerUpdate = 0;
            // 应用修改后的设置
            InputSystem.settings = inputSystemSettings;
            DGXR.Space = XRSpace.Instance;
            DGXR.IsFilterZero = isFilterZero;
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
                DGXR.Logger.LogError("XRManager", "load XRApplicationSettings failed");
#endif
            }


#if !UNITY_EDITOR
            if (DGXR.Config.Space.ScreenMode is ScreenStyle.Default)
            {
                Cursor.visible = false;
            }
#endif
        }

        private void Start()
        {
            InitXrConstantNode();
        }


        private void InitXrConstantNode()
        {
            XRDontDestroy = GameObject.Find(XrDontDestroyName);
            if (XRDontDestroy is null)
            {
                XRDontDestroy = Instantiate(Resources.Load<GameObject>($"Prefabs/{XrDontDestroyName}"), null, false);
                XRDontDestroy.name = XrDontDestroyName;
                GameLogger.Init(DGXR.Config.Log);
                DontDestroyOnLoad(XRDontDestroy);
            }
            else
            {
                OnXrConstantNodeInit?.Invoke();
            }
        }

        private void Update()
        {
            XRDontDestroy.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        private void OnApplicationQuit()
        {
            GameLogger.Cleanup();
        }
    }
}