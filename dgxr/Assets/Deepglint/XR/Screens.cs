using System.Collections.Generic;

namespace Deepglint.XR
{
    public class Screens
    {
        private static Screens _instance;
        private readonly Dictionary<TargetScreen,ScreenInfo> _screenDic;

        private Screens()
        {
            _screenDic = new Dictionary<TargetScreen, ScreenInfo>();
        }

        internal static Screens Instance
        {
            get
            {
                return _instance ??= new Screens();
            }
        }


        internal static void AddScreen(TargetScreen target, ScreenInfo screen)
        {
            Instance._screenDic[target] = screen;
        }

        internal static void RemoveScreen(TargetScreen target)
        {
            Instance._screenDic.Remove(target);
        }



        public ScreenInfo Front => _screenDic[TargetScreen.Front];

        public ScreenInfo Left => _screenDic[TargetScreen.Left];

        public ScreenInfo Right => _screenDic[TargetScreen.Right];

        public ScreenInfo Back => _screenDic[TargetScreen.Back];

        public ScreenInfo Bottom => _screenDic[TargetScreen.Bottom];

        public ScreenInfo this[TargetScreen screen] => _screenDic[screen];

    }
}
