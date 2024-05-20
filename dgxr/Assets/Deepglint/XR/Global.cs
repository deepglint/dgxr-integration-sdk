using System;
using System.Collections.Generic;
using Deepglint.Tool.UIFrame;
using Deepglint.XR.Ros;
using UnityEngine;
using UnityEngine.UI;


namespace Deepglint.XR
{
    public static class Global
    {
        public static Config.ConfigData.ConfigInfo Config;
        public delegate void MetaPoseDataEventHandler(Source.SourceData data);
        public delegate void MetaGearDataEventHandler(MetaGearInfo.MetaGearData data);
        public static  event MetaPoseDataEventHandler OnMetaPoseDataReceived;
        
        public static  Action<string> OnMetaPoseDataLost;
        
        public static  Action<string> OnMetaGearDataLost;
        public static event MetaGearDataEventHandler OnMetaGearDataReceived; 
        public static string UniqueID;
        public static string AppName;
        public static string SystemName;
        public static bool IsFilterZero;
        public static ViewInfo ViewData;
        public static Vector3 CavePosition;
        public const string PackageName = "com.deepglint.xr";
        
        public struct ViewInfo
        {
            public TargetDisplay Displays;
            public Dictionary<int, RawImage> DisplayImages;
        } 
        
       
        
        public static void TriggerMetaPoseDataReceived(Source.SourceData data)
        {
            OnMetaPoseDataReceived?.Invoke(data);
        }
        public static void TriggerMetaGearDataReceived(MetaGearInfo.MetaGearData data)
        {
            OnMetaGearDataReceived?.Invoke(data);
        }

        public static void TriggerMetaPostDataLost(string key)
        {
            OnMetaPoseDataLost?.Invoke(key);
        }
        
        public static void TriggerMetaGearDataLost(string key)
        {
            OnMetaGearDataLost?.Invoke(key);
        }
    }
}