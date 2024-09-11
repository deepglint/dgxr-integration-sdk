using System;
using Deepglint.XR.Config;
using Deepglint.XR.Log;
using Deepglint.XR.Ros;
using UnityEngine;
using Deepglint.XR.Space;

namespace Deepglint.XR
{
    /// <summary>
    /// 全局静态类 
    /// </summary>
    public static class DGXR
    {
        public static Config.Config.ConfigData.ConfigInfo Config;
        public delegate void MetaGearDataEventHandler(MetaGearInfo.MetaGearData data);
        public static  Action<string> OnMetaGearDataLost;
        public static event MetaGearDataEventHandler OnMetaGearDataReceived;
        public static string UniqueID;
        public static string AppName;
        public static string AppVersion;
        public static string Version;
        public static string SystemName;
        internal static XRApplicationSettings Settings;
        public static XRApplicationSettings ApplicationSettings => Settings;
        public static bool IsFilterZero;
        public static XRSpace Space;
        public static Vector3 CavePosition;
        public static Logger Logger = new Logger(new PrefixedLogger(Debug.unityLogger.logHandler, "DGXR"));
        
        public const string PackageName = "com.deepglint.xr";
        
        /// <summary>
        /// 设置 metaGear 消息到订阅
        /// </summary>
        /// <param name="data">MetaGearData结构化消息</param>
        public static void TriggerMetaGearDataReceived(MetaGearInfo.MetaGearData data)
        {
            OnMetaGearDataReceived?.Invoke(data);
        }
      

        public static void TriggerMetaGearDataLost(string key)
        {
            OnMetaGearDataLost?.Invoke(key);
        }
    }
}
