using System.Collections.Generic;
using Deepglint.XR.Space;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Deepglint.XR.Toolkit.DebugTool
{
    /// <summary>
    /// 单屏配置控制
    /// </summary> 
    public enum ScreenStyle
    {
        /// <summary>
        /// 默认样式，多屏渲染
        /// </summary>
        Default,

        /// <summary>
        /// 标准样式
        /// </summary>
        NormalMode,

        /// <summary>
        /// 主屏优先样式
        /// </summary>
        MainMode,
    }

    public class SingleScreen : MonoBehaviour
    {
        private bool _isDoubleClick;
        private float _lastClickTime;
        private Vector2 _lastClickPosition;
        private const float Height = 1200f;
        private const float Width = 1920f;
        private Dictionary<TargetScreen, Rect> _screenRects = new Dictionary<TargetScreen, Rect>();
        private Rect _oldRect;

        private void Start()
        {
            if (Global.Config.Space.ScreenMode != ScreenStyle.Default)
            {
                Screen.SetResolution((int)Width, (int)Height, true);
                Camera cam = gameObject.AddComponent<Camera>();
                cam.cullingMask = 0;
                cam.depth = -100;
                cam.clearFlags = CameraClearFlags.Color;
                cam.backgroundColor = Color.black;

                float partWidth =Width / 15f;
                float edgeBorder = Height - partWidth / 1.6f * 13f;
                float partHeight = (Height - edgeBorder) / 13f;

                float ratioHeight = partHeight * 5f / Height;
                float ratioBottomHeight = partHeight * 8f / Height;
                float ratioWidth = partWidth * 5f / Width;

                float leftX = 0;
                float midX = partWidth * 5f / Width;
                float rightX = 2f * partWidth * 5f / Width;
                float topY = (8f * partHeight + edgeBorder) / Height;
                float midY = 1 - (10f * partHeight) / Height;
                float bottomY = edgeBorder / Height;


                switch (Global.Config.Space.ScreenMode)
                {
                    case ScreenStyle.Default:
                        return;
                    case ScreenStyle.NormalMode:
                        _screenRects.Add(TargetScreen.Front, new Rect(midX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Right, new Rect(rightX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Back, new Rect(rightX, midY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Left, new Rect(leftX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Bottom, new Rect(midX, bottomY, ratioWidth, ratioBottomHeight));
                        break;
                    case ScreenStyle.MainMode:
                        _screenRects.Add(TargetScreen.Front, new Rect(0, 0, 2 * ratioWidth, 2 * ratioHeight));
                        _screenRects.Add(TargetScreen.Right, new Rect(leftX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Back, new Rect(midX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Left, new Rect(rightX, topY, ratioWidth, ratioHeight));
                        _screenRects.Add(TargetScreen.Bottom, new Rect(rightX, bottomY, ratioWidth, ratioBottomHeight));
                        break;
                    default:
                        return;
                }
                SplitScreenMode();
            }
        }

        void Update()
        {
            if (Global.Config.Space.ScreenMode != ScreenStyle.Default)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Cursor.visible = true;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SplitScreenMode();
                }

                if (Input.GetMouseButtonDown(0) && !_isDoubleClick)
                {
                    _lastClickTime = Time.time;
                    _lastClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    _isDoubleClick = true;
                }
                else if (Input.GetMouseButtonDown(0) && _isDoubleClick)
                {
                    float timeSinceLastClick = Time.time - _lastClickTime;
                    if (timeSinceLastClick < 1f)
                    {
                        Vector2 currentClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        if (Vector2.Distance(_lastClickPosition, currentClickPosition) < 10f)
                        {
                            AllScreenMode(new Vector2(currentClickPosition.x / Width, currentClickPosition.y / Height));
                        }
                    }

                    _isDoubleClick = false;
                }
            }
        }

        private void AllScreenMode(Vector2 point)
        {
            foreach (var rec in _screenRects)
            {
                if (rec.Value.Contains(point))
                {
                    Global.Space[rec.Key].SpaceCamera.rect = new Rect(0, 0, 1, 1);
                    Global.Space[rec.Key].SpaceCamera.depth = 100;
                }
            }
        }

        private void SplitScreenMode()
        {
            foreach (var space in Global.Space)
            {
                space.SpaceCamera.targetDisplay = 0;
                space.SpaceCamera.depth = 1;
                foreach (var overCam in space.SpaceCamera.GetUniversalAdditionalCameraData().cameraStack)
                {
                    overCam.targetDisplay = 0;
                    overCam.rect = _screenRects[space.TargetScreen]; 
                }

                space.SpaceCamera.rect = _screenRects[space.TargetScreen];
            }
        }
    }
}