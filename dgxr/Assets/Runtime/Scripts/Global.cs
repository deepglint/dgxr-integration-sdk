using ROS2;

namespace DGXR
{
    public class Global
    {
        public static Config.ConfigData.ConfigInfo Config;
        public Source SourceData = Source.Instance;
        public delegate void MetaGearDataEventHandler(MetaGearInfo.MetaGearData data);
        public static event MetaGearDataEventHandler OnMetaGearDataReceived; 
        public static string UniqueID;
        public static string AppName; 
        public static ROS2UnityComponent Ros2Unity;
        public static bool IsFilterZero;
        
        public static void TriggerMetaGearDataReceived(MetaGearInfo.MetaGearData data)
        {
            Global.OnMetaGearDataReceived?.Invoke(data);
        }
    }
    
}