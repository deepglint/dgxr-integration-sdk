using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Deepglint.XR;
using Deepglint.XR;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Deepglint.XR
{
    [DefaultExecutionOrder(-99)]
    public class XRSpaceManager : MonoBehaviour
    {
        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        [FormerlySerializedAs("DisplayImagePrefab")]
        public GameObject displayImagePrefab;

        [FormerlySerializedAs("ScreenPrefab")] public GameObject screenPrefab;
        [FormerlySerializedAs("MetaSpace")] public GameObject metaSpace;

        [FormerlySerializedAs("LockAll")] [Header("视角跟随相关设置")]
        public Boolean lockAll;

        [FormerlySerializedAs("LockXZ")] public Boolean lockXZ;
        [FormerlySerializedAs("EyeHeight")] public Vector3 eyePosition = new Vector3(0, 1.6f, 0);
        [FormerlySerializedAs("SpaceScale")] public float spaceScale = 1;

        private GameObject[] _screens;
        private readonly int _caveLayer = 31;
        private GameObject[,] _screenEdges;
        private int _numberScreens;
        private Vector3 _head = new Vector3(0, 1.6f, 0);
        private Vector3 _headLockPosition;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0; // 主屏幕分辨率宽度
        private const int SM_CYSCREEN = 1; // 主屏幕分辨率高度

        private int _screenWidth; // 屏幕宽度
        private int _screenHeight; // 屏幕高度
        private RefreshRate _refreshRate = new RefreshRate
        {
            numerator = 30,
            denominator = 1
        };
#if !UNITY_EDITOR
        private RenderTexture _renderTexture;

#endif
        private void Awake()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _screenWidth = GetSystemMetrics(SM_CXSCREEN);
                _screenHeight = GetSystemMetrics(SM_CYSCREEN);
            }
        }

        void Start()
        {
            metaSpace.SetActive(false);
            if (!Global.SystemName.Contains("Mac"))
            {
                foreach (var display in Display.displays)
                {
                    display.Activate(display.systemWidth, display.systemHeight, _refreshRate);
                }

                Screen.SetResolution(_screenWidth, _screenHeight, true);
            }


            InstantiateXR();
            _screenEdges = new GameObject[_numberScreens, 4];
            for (int screen = 0; screen < _numberScreens; screen++)
            {
                for (int corner = 0; corner < 4; corner++)
                {
                    _screenEdges[screen, corner] = _screens[screen].transform.GetChild(corner).gameObject;
                }
            }
        }

        void Update()
        {
            SetScale();
            SetHead();
#if !UNITY_EDITOR
            foreach (var screen in Global.Config.Space.Screens)
            {
                if (screen.Render.Length > 0)
                {
                    foreach (var render in screen.Render)
                    {
                        Texture tex = ClippedRenderTexture(_renderTexture,
                            new Rect(render.Rect[0], render.Rect[1], render.Rect[2], render.Rect[3]));
                        if (Global.UserView.DisplayImages[render.Display].texture != null)
                        {
                            Destroy(Global.UserView.DisplayImages[render.Display].texture);
                        }
                        Global.UserView.DisplayImages[render.Display].texture = tex;
                    }
                }
            }
#endif
        }

        private void SetScale()
        {
            gameObject.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
        }

        public void SetHead()
        {
            // 处理坐标的比例关系
            //Set the position
            SetHeadPosition();

            for (int cameraIndex = 0; cameraIndex < _numberScreens; cameraIndex++)
            {
                SetHeadFovAndOrientationScreen(cameraIndex, Global.UserView.Cameras, _screenEdges);
            }
        }

        private void SetHeadPosition()
        {
            if (Global.CavePosition.x == 0 && Global.CavePosition.y == 0 && Global.CavePosition.z == 0)
            {
                foreach (var body in Source.Data)
                {
                    _head = body.Joints.HeadTop;
                    break;
                }
            }
            else
            {
                _head = Global.CavePosition;
            }

            Vector3 position = transform.position;
            _headLockPosition = _head + position;
            if (lockAll)
            {
                _headLockPosition = position + eyePosition;
            }
            else if (lockXZ)
            {
                _headLockPosition = new Vector3(eyePosition.x, _head.y, eyePosition.z) + position;
            }

            foreach (var userCamera in Global.UserView.Cameras)
            {
                userCamera.transform.position = _headLockPosition;
            }
        }

        // public Vector3 GetHeadPosition()
        // {
        // if (_head == null) return Vector3.zero;
        // var localPosition = _head.transform.localPosition;
        // Vector3 scaleHead = localPosition;
        //
        // // 基于空间点的移动偏移
        // //TODO 临时设置，后面提供可配置面板修改这些值，人眼默认高度、跟随速度，空间比例
        // float y = 1.6f + (1.6f - scaleHead.z) * 1;
        // // 基于空间点的移动偏移
        // return new Vector3(localPosition.x * 1, y, localPosition.y * -1 * 1);
        // }

        private void SetHeadFovAndOrientationScreen(int index, Camera[] cameras, GameObject[,] edges)
        {
            var bottomToTop = edges[index, 0].transform.position - edges[index, 2].transform.position;
            var leftToRight = edges[index, 1].transform.position - edges[index, 3].transform.position;

            //Set FOV
            cameras[index].ResetProjectionMatrix();
            cameras[index].fieldOfView = -2 * Mathf.Rad2Deg *
                                         Mathf.Atan(bottomToTop.magnitude / 2 /
                                                    (cameras[index].transform.localPosition.z *
                                                     cameras[index].transform.parent.lossyScale.z));
            //Set the orientation
            float obV = cameras[index].transform.localPosition.y *
                cameras[index].transform.parent.lossyScale.y / (bottomToTop.magnitude / 2);
            float obH = cameras[index].transform.localPosition.x *
                cameras[index].transform.parent.lossyScale.x / (leftToRight.magnitude / 2);
            SetObliqueness(-obH, -obV, cameras[index]);
        }

        void SetObliqueness(float horizObl, float vertObl, Camera cam)
        {
            Matrix4x4 mat = cam.projectionMatrix;
            mat[0, 2] = horizObl;
            mat[1, 2] = vertObl;
            cam.projectionMatrix = mat;
        }

        private Texture ClippedRenderTexture(RenderTexture sourceTexture, Rect rect)
        {
            RenderTexture.active = sourceTexture;
            Texture2D croppedTexture = new Texture2D((int)rect.width, (int)rect.height);
            Rect region = new Rect(rect.x, rect.y, rect.width, rect.height);
            croppedTexture.ReadPixels(region, 0, 0);
            RenderTexture.active = null;
            croppedTexture.Apply();
            return croppedTexture;
        }

        private void InstantiateXR()
        {
            _numberScreens = Global.Config.Space.Screens.Count;
            _screens = new GameObject[_numberScreens];
            foreach (var screen in Global.Config.Space.Screens)
            {
                _screens[screen.Display - 1] = Instantiate(screenPrefab, transform);
                _screens[screen.Display - 1].transform.localPosition =
                    new Vector3(screen.Position.x, screen.Position.y, screen.Position.z);
                _screens[screen.Display - 1].transform.localRotation =
                    Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, screen.Rotation.z);
                _screens[screen.Display - 1].transform.localScale =
                    new Vector3(screen.Size.x, screen.Size.y, screen.Size.z);
            }

            Global.UserView.Cameras = new Camera[_numberScreens];
            for (int i = 0; i < _numberScreens; i++)
            {
                int index = Global.Config.Space.Screens[i].Display - 1;

                Global.UserView.Cameras[index] = Instantiate(userViewCameraPrefab, transform.position,
                    _screens[index].transform.rotation, _screens[index].transform);

                Global.UserView.Cameras[index].gameObject.layer = _caveLayer;
                //_userScreenViewCameras[index].cullingMask = 1 << ();      //The user is set to only see the default layer. Change this culling mask if you want the camera to see different layers (like water).
                Global.UserView.Cameras[index].targetDisplay = index;
#if !UNITY_EDITOR
                if (Global.Config.Space.Screens[i].Render.Length > 0)
                {
                    _renderTexture = new RenderTexture(1920, 1920, 24);
                    Global.UserView.Cameras[index].targetTexture = _renderTexture;
                    Global.UserView.Cameras[index].Render();
                    RenderTexture.active = _renderTexture;
                    foreach (var render in Global.Config.Space.Screens[i].Render)
                    {
                        GameObject displayImage = Instantiate(displayImagePrefab,
                            Global.UserView.Cameras[index].transform.position,
                            Global.UserView.Cameras[index].transform.rotation,
                            Global.UserView.Cameras[index].transform);
                        Canvas[] displayCanvas = displayImage.GetComponentsInChildren<Canvas>();
                        foreach (var can in displayCanvas)
                        {
                            can.renderMode = RenderMode.ScreenSpaceOverlay;
                            can.targetDisplay = render.Display - 1;
                        }

                        RawImage[] drawImage = displayImage.GetComponentsInChildren<RawImage>();

                        Global.UserView.DisplayImages ??= new Dictionary<int, RawImage>();

                        if (drawImage.Length > 0)
                        {
                            Global.UserView.DisplayImages[render.Display] = drawImage[0];
                        }
                    }
                }

                Global.UserView.Cameras[index].aspect =
                    _screens[index].transform.localScale.x / _screens[index].transform.localScale.y;
#endif
            }
        }
    }
}