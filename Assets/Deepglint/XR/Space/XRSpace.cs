using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Deepglint.XR.Space
{
    public class XRSpace : IEnumerable<ScreenInfo>
    {
        private static XRSpace _instance;
        private readonly Dictionary<TargetScreen, ScreenInfo> _screenDic;
        public GameObject gameObject { get; internal set; }
       
        public Vector3 RealSize { get; internal set; }
        
        public Vector3 Size { get; internal set; }
        
        public int ActiveScreens => _screenDic.Count;

        public Vector3 Origin { get; internal set; }

        public Rect Roi { get; internal set; }


        private XRSpace()
        {
            _screenDic = new Dictionary<TargetScreen, ScreenInfo>();
        }

        internal static XRSpace Instance
        {
            get { return _instance ??= new XRSpace(); }
        }

        /// <summary>
        /// 增加空间屏幕
        /// </summary>
        /// <param name="target">屏幕枚举值</param>
        /// <param name="screen">屏幕类对象</param>
        internal static void AddScreen(TargetScreen target, ScreenInfo screen)
        {
            Instance._screenDic[target] = screen;
        }

        /// <summary>
        /// 删除空间屏幕
        /// </summary>
        /// <param name="target">屏幕枚举值</param>
        internal static void RemoveScreen(TargetScreen target)
        {
            Instance._screenDic.Remove(target);
        }

        public ScreenInfo LeftByFront => _screenDic[TargetScreen.LeftByFront];
        public ScreenInfo MidLeftByFront => _screenDic[TargetScreen.MidLeftByFront];
        public ScreenInfo MidRightByFront => _screenDic[TargetScreen.MidRightByFront];
        public ScreenInfo RightByFront => _screenDic[TargetScreen.RightByFront];

        public ScreenInfo TopByRight => _screenDic[TargetScreen.TopByRight];
        public ScreenInfo BottomByRight => _screenDic[TargetScreen.BottomByRight];

        public ScreenInfo TopByLeft => _screenDic[TargetScreen.TopByLeft];
        public ScreenInfo BottomByLeft => _screenDic[TargetScreen.BottomByLeft];

        public ScreenInfo LeftByBack => _screenDic[TargetScreen.LeftByBack];
        public ScreenInfo MidLeftByBack => _screenDic[TargetScreen.MidLeftByBack];
        public ScreenInfo MidRightByBack => _screenDic[TargetScreen.MidRightByBack];
        public ScreenInfo RightByBack => _screenDic[TargetScreen.RightByBack];

        public ScreenInfo TopLeftByBottom => _screenDic[TargetScreen.TopLeftByBottom];
        public ScreenInfo TopMidByBottom => _screenDic[TargetScreen.TopMidByBottom];
        public ScreenInfo TopRightByBottom => _screenDic[TargetScreen.TopRightByBottom];
        public ScreenInfo BottomLeftByBottom => _screenDic[TargetScreen.BottomLeftByBottom];
        public ScreenInfo BottomMidByBottom => _screenDic[TargetScreen.BottomMidByBottom];
        public ScreenInfo BottomRightByBottom => _screenDic[TargetScreen.BottomRightByBottom];

        public ScreenInfo this[TargetScreen screen] => _screenDic[screen];

        public IEnumerator<ScreenInfo> GetEnumerator()
        {
            return _screenDic.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}