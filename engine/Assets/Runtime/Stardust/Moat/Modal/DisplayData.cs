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
        public int gamepad { get; set; }
        public bool allowClose { get; set; }
        public bool allowRoam { get; set; }
        public int playerCount { get; set; }
        public float circlePosX { get; set; }
        public float circlePosY { get; set; }
        public int debugLevel { get; set; }
        public int interactionPermissionLevel { get; set; }
        public bool allowCave { get; set; }
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

        public static void ReadConfig()
        {
            if (configDisplay == null)
            {
                configDisplay = MReadData.ReadJsonFile<DisplayConfigData>(Application.streamingAssetsPath + "/stardust/display.json"); 
            }
        }
    }
}