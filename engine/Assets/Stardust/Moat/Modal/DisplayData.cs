using UnityEngine;
using Moat;

namespace Moat.Model
{
    [System.Serializable]
    public class DisplayConfigData {
        public CircleAreaData circleArea { get; set; }
        public float[][] interactionArea { get; set; }
        public ResolutionData resolution { get; set; }
        public TargetDisplayData targetDisplay { get; set; }
        public UiCameraPosData uiCameraPos { get; set; }
        public bool wsConnect { get; set; }
        public int ReconnectMaxCount { get; set; }
        public bool supportGamepad { get; set; }
        public bool allowClose { get; set; }
        public bool allowRoam { get; set; }
        public int playerCount { get; set; }
        public float circlePosX { get; set; }
        public float circlePosY { get; set; }
        public int debugLevel { get; set; }
        public int interactionPermissionLevel { get; set; }
        public bool allowCave { get; set; }
        public float moveSpeed { get; set; }
        public float spaceFollowSpeed { get; set; }
        public float spaceUpperOrLowerOffsetProportion { get; set; }
        public bool allowFollowingInSinglePlayer { get; set; }
        public bool forcedSubstitutionsInSinglePlayer { get; set; }
        public float roamSpeed { get; set; }
        public float roamRotationSpeed { get; set; }
    }

    [System.Serializable]
    public class UiCameraPosData
    {
        public float bottom1Y { get; set; }
        public float bottom2Y { get; set; }
    }

    [System.Serializable]
    public class CircleAreaData
    {
        public int roiRadius { get; set; }
        public int playerRadius { get; set; }
        public int roiCircleBoundary { get; set; }
        public int groundSelectorRadius { get; set; }
    }

    [System.Serializable]
    public class ResolutionData
    {
        public float realResolution { get; set; }
        public float cameraRoi { get; set; }
        public bool fullScreen { get; set; }
        public bool projectionFusion { get; set; }
        public int systemWidth { get; set; }
        public int systemHeight { get; set; }
    }

    [System.Serializable]
    public class TargetDisplayData
    {
        public int left { get; set; }
        public int front { get; set; }
        public int right { get; set; }
        public int back { get; set; }
        public int bottom1 { get; set; }
        public int bottom2 { get; set; }
        public int debug { get; set; }
    }
    
    public static class DisplayData
    {
        public static DisplayConfigData configDisplay;

        // 配置开关
        public static bool allowReadConfig = true;
       
        // 数值
        public static int ReconnectMaxCount;
        public static int playerCount;
        public static float circlePosX;
        public static float circlePosY;
        public static int debugLevel;
        public static int interactionPermissionLevel;
        public static float moveSpeed;
        
        // 开关
        public static bool wsConnect;
        public static bool supportGamepad;
        public static bool allowClose;
        public static bool allowRoam;
        public static bool allowCave;
        public static bool allowFollowingInSinglePlayer;
        public static bool forcedSubstitutionsInSinglePlayer; 
        
        // 测试状态
        public static bool IsGlobalTest;
        
        // 空间定义
        public static float SpaceSize = 5f;
        public static float HumanEye = 1.6f;
        public static float SpatialProportion = 1;
        
        // 空间定义 - 走配置
        public static float SpaceFollowSpeed = 1;
        public static float SpaceUpperOrLowerOffsetProportion = 0.3f;
        public static float roamSpeed;
        public static float roamRotationSpeed;

        public static void InitData()
        {
            wsConnect = false;
            IsGlobalTest = true;
            supportGamepad = false;
            allowClose = false;
            allowRoam = false;
            allowCave = false;
            allowFollowingInSinglePlayer = false;
            forcedSubstitutionsInSinglePlayer = false;

            SpaceSize = 5f;
            SpatialProportion = 1;
            SpaceUpperOrLowerOffsetProportion = 0.3f;
            Init();
        }

        public static void ReadConfig()
        {
            // Debug.LogError("0. 读取配置权限 " + allowReadConfig);
            if (configDisplay == null)
            {
                configDisplay = MReadData.ReadJsonFile<DisplayConfigData>(Application.streamingAssetsPath + "/stardust/display.json"); 
            }

            if (allowReadConfig)
            {
                ReconnectMaxCount = configDisplay.ReconnectMaxCount;
                playerCount = configDisplay.playerCount;
                debugLevel = configDisplay.debugLevel;
                interactionPermissionLevel = configDisplay.interactionPermissionLevel;
                moveSpeed = configDisplay.moveSpeed;
                    
                wsConnect = configDisplay.wsConnect;
                // Debug.LogError("0. 设置连接权限 " + wsConnect);
                IsGlobalTest = !wsConnect;
                supportGamepad = configDisplay.supportGamepad;
                allowClose = configDisplay.allowClose;
                allowCave = configDisplay.allowCave;
                allowRoam = configDisplay.allowRoam;
                allowFollowingInSinglePlayer = configDisplay.allowFollowingInSinglePlayer;
                forcedSubstitutionsInSinglePlayer = configDisplay.forcedSubstitutionsInSinglePlayer;

                SpaceFollowSpeed = configDisplay.spaceFollowSpeed;
                SpaceUpperOrLowerOffsetProportion = configDisplay.spaceUpperOrLowerOffsetProportion;
                roamSpeed = configDisplay.roamSpeed;
                roamRotationSpeed = configDisplay.roamRotationSpeed; 

                Init();
            }
        }

        private static void Init()
        {
            SpatialProportion = SpaceSize / 5;
            HumanEye = (float)(1.6 * SpatialProportion);
        }
    }
}