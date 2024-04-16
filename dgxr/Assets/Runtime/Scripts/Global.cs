namespace Unity.XR.DGXR
{
    public static class Global
    {
        public static Config.ConfigData.ConfigInfo Config;
        public delegate void MetaPoseDataEventHandler(Source.SourceData data);
        public delegate void MetaGearDataEventHandler(MetaGearInfo.MetaGearData data);
        public static  event MetaPoseDataEventHandler OnMetaPoseDataReceived;
        public static event MetaGearDataEventHandler OnMetaGearDataReceived; 
        public static string UniqueID;
        public static string AppName;
        public static bool IsFilterZero;
        
        public static void TriggerMetaPoseDataReceived(Source.SourceData data)
        {
            OnMetaPoseDataReceived?.Invoke(data);
        }
        public static void TriggerMetaGearDataReceived(MetaGearInfo.MetaGearData data)
        {
            OnMetaGearDataReceived?.Invoke(data);
        }
    }
    
}