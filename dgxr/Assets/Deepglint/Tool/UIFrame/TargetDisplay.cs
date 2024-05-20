using UnityEngine;

namespace Deepglint.Tool.UIFrame
{
    public struct TargetDisplay
    {
        public DisplayInfo Front;
        public DisplayInfo Right;
        public DisplayInfo Back;
        public DisplayInfo Left;
        public DisplayInfo Bottom;
    }

    public class DisplayInfo
    {
        public string Name;
        public GameObject Screen;
        public Camera UICamera;
        public Camera SpaceCamera;
    }

    

}