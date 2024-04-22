using Stardust.Scripts;
using UnityEngine;

namespace Stardust.Model
{
    [System.Serializable]
    public class DisplayConfigData {
        public EventListenerConfig EventListenerConfig { get; set; }
        public ResolutionData Resolution { get; set; }
        public TargetDisplayData TargetDisplay { get; set; }
        public UiCameraPosData UICameraPos { get; set; }
        public bool WsConnect { get; set; }
        public int PlayerCount { get; set; }
        public float HipOffsetX { get; set; }
        public float HipOffsetY { get; set; }
        public int DebugLevel { get; set; }
        public int InteractionPermissionLevel { get; set; }
        public float SpaceFollowSpeed { get; set; }
        public float SpaceUpperOrLowerOffset { get; set; }
        public bool AllowFollowingInSinglePlayer { get; set; }
        public bool ForcedSubstitutionsInSinglePlayer { get; set; }
        public bool SingleSceneDebug { get; set; }
        public float[][] SingleSceneRect { get; set; }
    }

    [System.Serializable]
    public class UiCameraPosData
    {
        public float Bottom1X { get; set; }
        public float Bottom2X { get; set; }
        public float Bottom1Y { get; set; }
        public float Bottom2Y { get; set; }
        public float Bottom1RotationZ { get; set; }
        public float Bottom2RotationZ { get; set; }
    }

    [System.Serializable]
    public class EventListenerConfig
    {
        public bool Single { get; set; }
        public float HighFiveOnThreshold { get; set; }
        public float HighFiveOffThreshold { get; set; }
    }

    [System.Serializable]
    public class ResolutionData
    {
        public float RealResolution { get; set; }
        public float CameraRoi { get; set; }
        public float SystemWidth { get; set; }
        public float SystemHeight { get; set; }
    }

    [System.Serializable]
    public class TargetDisplayData
    {
        public int Left { get; set; }
        public int Front { get; set; }
        public int Right { get; set; }
        public int Back { get; set; }
        public int Bottom1 { get; set; }
        public int Bottom2 { get; set; }
        public int Debug { get; set; }
    }
    
    public static class DisplayData
    {
        public static DisplayConfigData ConfigDisplay;

       
        // 数值
        public static int InteractionPermissionLevel;
        
        // 开关
        public static bool WsConnect;
        public static bool AllowFollowingInSinglePlayer;
        public static bool ForcedSubstitutionsInSinglePlayer; 
        
        // 空间定义
        public static float SpaceSize;
        public static float HumanEye = 1.6f;
        public static float SpatialProportion = 1;
        
        // 空间定义 - 走配置
        public static float SpaceFollowSpeed = 1;
        public static float SpaceUpperOrLowerOffset;

        public static void InitData()
        {
            WsConnect = false;
            AllowFollowingInSinglePlayer = false;
            ForcedSubstitutionsInSinglePlayer = false;

            SpatialProportion = 1;
            SpaceUpperOrLowerOffset = 0f;
        }

        public static void ReadConfig()
        {
            // Debug.LogError("0. 读取配置权限 " + allowReadConfig);
            if (ConfigDisplay == null)
            {
                ConfigDisplay = MReadData.ReadJsonFile<DisplayConfigData>(Application.streamingAssetsPath + "/stardust/display.json"); 
            }

            InteractionPermissionLevel = ConfigDisplay.InteractionPermissionLevel;
                    
            WsConnect = ConfigDisplay.WsConnect;
            // Debug.LogError("0. 设置连接权限 " + wsConnect);
            AllowFollowingInSinglePlayer = ConfigDisplay.AllowFollowingInSinglePlayer;
            ForcedSubstitutionsInSinglePlayer = ConfigDisplay.ForcedSubstitutionsInSinglePlayer;

            SpaceFollowSpeed = ConfigDisplay.SpaceFollowSpeed;
            SpaceUpperOrLowerOffset = ConfigDisplay.SpaceUpperOrLowerOffset;
            SpaceSize = ConfigDisplay.Resolution.RealResolution;
                
            SpatialProportion = SpaceSize / 5; // 5是标准空间尺寸
        }

    }
}