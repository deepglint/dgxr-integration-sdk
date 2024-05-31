using System;
using System.Collections.Generic;
using Deepglint.XR.Ros;
using UnityEngine;
using Deepglint.XR.Space;


namespace Deepglint.XR
{
    public static class Global
    {
        public static Config.Config.ConfigData.ConfigInfo Config;

        //TODO: 怎么用事件获取同一帧的数据
      
        
        public delegate void MetaGearDataEventHandler(MetaGearInfo.MetaGearData data);

        //TODO: 这个不应该在Source上吗
       

        public static  Action<string> OnMetaGearDataLost;
        public static event MetaGearDataEventHandler OnMetaGearDataReceived;
        public static string UniqueID;
        public static string AppName;
        public static string Version;
        public static string SystemName;
        public static bool IsFilterZero;
        public static XRSpace Space;
        public static Vector3 CavePosition;
        public const string PackageName = "com.deepglint.xr";



      
        
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
