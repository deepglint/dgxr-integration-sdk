using Deepglint.XR.Log;
using Deepglint.XR.Source;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR
{
    [DefaultExecutionOrder(-100)]
    public class XRManager : MonoBehaviour
    {
        public bool isFilterZero;

        public void Awake()
        {

            Global.UniqueID = SystemInfo.deviceUniqueIdentifier;
            Global.AppName = Application.productName;
            Global.Version = Application.version;
            Global.SystemName = SystemInfo.operatingSystem;
            Global.Config = new Config.Config().InitConfig();
            
#if !UNITY_EDITOR
            if (Global.Config.Space.ScreenMode is ScreenStyle.Default)
            {
                Cursor.visible = false;
            }
#endif
            
            GameLogger.Init(Global.Config.Log);
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

        public void Start()
        {
            Global.Space = XRSpace.Instance;
            Global.IsFilterZero = isFilterZero;
        }

        private bool UseRos()
        {
            if (!Application.isEditor && !Global.SystemName.Contains("Mac"))
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
