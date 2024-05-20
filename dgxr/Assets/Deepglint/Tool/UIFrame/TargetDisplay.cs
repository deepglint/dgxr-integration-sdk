using UnityEngine;

namespace Deepglint.Tool.UIFrame
{
    public enum TargetDisplay
    {
        Front,
        Right,
        Back,
        Left,
        Bottom,
    }

    public class DisplayInfo
    {
        public string Name;
        public GameObject Screen;
        public Camera UICamera;
        public Camera SpaceCamera; 
        public float ScreenWidth; // 屏幕宽度
        public float ScreenHeight; // 屏幕高度
    }
}