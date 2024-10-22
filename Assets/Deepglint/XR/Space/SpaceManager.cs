using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Deepglint.XR.Space
{
    [DefaultExecutionOrder(-99)]
    public class SpaceManager : MonoBehaviour
    {
        public int id = 0;

        [FormerlySerializedAs("DisplayImagePrefab")]
        public GameObject displayImagePrefab;

        [FormerlySerializedAs("ScreenPrefab")] public
            GameObject screenPrefab;

        public Shader shader;
        private float _buttonRotation;

        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        public Boolean isCave;

        private GameObject[] _screens;
        [FormerlySerializedAs("SpaceScale")] public float spaceScale = 1;


        private readonly int _caveLayer = 31;
        private Dictionary<int, GameObject[]> _screenEdges;
        public Vector3 eyePosition = new Vector3(0, 1.6f, 0);
        private Vector3 _headLockPosition;
        // private List<Config.Config.ScreenConfig> _screen;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        private int _screenWidth; // 屏幕宽度
        private int _screenHeight; // 屏幕高度

        private Dictionary<int, RenderImage> _displayImages;
        
        private readonly RefreshRate _refreshRate = new RefreshRate
        {
            numerator = 30,
            denominator = 1
        };

        public struct RenderImage
        {
            public RenderTexture RenderTexture;
            public RenderTexture FrontBottomTex;
            public RenderTexture BackBottomTex;
            public Dictionary<int,RawImage> DisplayImages;
            public Material CropMaterial;
        }

#if UNITY_EDITOR
        // private RenderTexture _renderTexture;
        // private RenderTexture _uiRenderTexture;
        // private RenderTexture _frontBottomTex;
        // private RenderTexture _backBottomTex;
        
#endif
        private void Awake()
        {
#if UNITY_EDITOR
            if (!DGXR.SystemName.Contains("Mac"))
            {
                _screenWidth = GetSystemMetrics(SM_CXSCREEN);
                _screenHeight = GetSystemMetrics(SM_CYSCREEN);
            }
            else
            {
                _screenWidth = 1920;
                _screenHeight = 1200;
            }
#else
            _screenWidth = 1920;
            _screenHeight = 1200;
#endif
            InitRenderTexture();
            InstantiateXR();
        }

        private void OnValidate()
        {
            var space = GameObject.Find("XRSpace");
            if (space is not null)
            {
                space.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
            }
        }

        void InitRenderTexture()
        {
            if (!DGXR.SystemName.Contains("Mac") && DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
            {
                foreach (var display in Display.displays)
                {
                    display.Activate(display.systemWidth, display.systemHeight, _refreshRate);
                }

                Screen.SetResolution(_screenWidth, _screenHeight, true);
            }
#if UNITY_EDITOR
            // _uiRenderTexture = new RenderTexture(_screenWidth, _screenWidth, 24);
            // _frontBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
            // _backBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
            // _cropMaterial = new Material(shader);
            // _frontBottomTex.Create();
            // _backBottomTex.Create();
#endif
        }

        private void Start()
        {
            _screenEdges = new Dictionary<int, GameObject[]>();
            foreach (var screen in DGXR.Config.Space.Screens)
            {
                GameObject[] games = new GameObject[4];
                for (int corner = 0; corner < 4; corner++)
                {
                    var sc = DGXR.Space[screen.Screen];
                    games[corner] = sc.ScreenObject.transform.GetChild(corner).gameObject;
                }

                _screenEdges.Add((int)screen.Screen, games);
            }

            SetHead();
#if UNITY_EDITOR
            RenderPipelineManager.endFrameRendering += HandleSplitScreen;
#endif
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            RenderPipelineManager.endFrameRendering -= HandleSplitScreen;
        }
#endif
        void Update()
        {
            Transform space = GameObject.Find("XRSpace").transform;
            foreach (var s in DGXR.Config.Space.Screens)
            {
                if (!s.Enable)
                {
                    var cam = space.Find(s.Screen.ToString()).Find("UserViewCamera");
                    cam.gameObject.SetActive(false);
                    // Destroy(cam.gameObject);
                    //     .GetComponent<Camera>();
                    // cam.cullingMask = 0;
                    // Destroy(cam);
                }
            }

            XRSpace.Instance.Origin = space.transform.position;
            if (isCave)
            {
                SetHead();
            }
        }

#if UNITY_EDITOR
        private void ProcessRendersCoroutine()
        {
            foreach (var screen in DGXR.Config.Space.Screens)
            {
                if (screen.Render.Length > 0)
                {
                    foreach (var render in screen.Render)
                    {
                        ProcessSingleRender(_displayImages[(int)screen.Screen],render);
                    }
                }
            }
        }

        private void ProcessSingleRender(RenderImage screen,Config.Config.RenderInfo render)
        {
            Rect rect = new Rect(render.Rect[0], render.Rect[1], render.Rect[2], render.Rect[3]);
            ClippedRenderTexture(screen, rect, render.Display);
            foreach (var tarDisplay in render.TarDisplay)
            {
                if (!_displayImages.TryGetValue(tarDisplay, out var dis))
                {
                    return;
                }

                screen.DisplayImages[tarDisplay].texture = render.Display switch
                {
                    4 => screen.FrontBottomTex,
                    5 => screen.BackBottomTex,
                    _ => screen.DisplayImages[tarDisplay].texture
                };
            }
        }

        public void HandleSplitScreen(ScriptableRenderContext paramContext, Camera[] paramCamera)
        {
            if (DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
            {
                ProcessRendersCoroutine();
            }
        }

        private void ClippedRenderTexture(RenderImage image, Rect rect, int display)
        {
            RenderTexture sourceTexture = image.RenderTexture;
            image.CropMaterial.SetTexture("_MainTex", sourceTexture);
            image.CropMaterial.SetFloat("_OffsetX", rect.x / sourceTexture.width);
            image.CropMaterial.SetFloat("_OffsetY", rect.y / sourceTexture.height);
            image.CropMaterial.SetFloat("_ScaleX", rect.width / sourceTexture.width);
            image.CropMaterial.SetFloat("_ScaleY", rect.height / sourceTexture.height);
            image.CropMaterial.SetFloat("_Rotation", _buttonRotation * Mathf.Deg2Rad);
            if (display == 4)
            {
                Graphics.Blit(sourceTexture, image.FrontBottomTex,image.CropMaterial);
            }
            else if (display == 5)
            {
                Graphics.Blit(sourceTexture, image.BackBottomTex,image.CropMaterial);
            }

            RenderTexture.active = null;
        }
#endif
        /// <summary>
        /// 设置空间中人的位置 
        /// </summary>
        public void SetHead()
        {
            SetHeadPosition();
            foreach (var screen in DGXR.Config.Space.Screens)
            {
                var tarDisplay = DGXR.Space[screen.Screen];
                if (tarDisplay != null && tarDisplay.SpaceCamera != null)
                {
                    SetHeadFovAndOrientationScreen((int)screen.TargetScreen, tarDisplay.SpaceCamera, _screenEdges);
                }
            }
        }

        /// <summary>
        /// 设置空间中人的位置3d坐标 
        /// </summary>
        private void SetHeadPosition()
        {
            if (DGXR.CavePosition.x != 0 || DGXR.CavePosition.y != 0 || DGXR.CavePosition.z != 0)
            {
                eyePosition = DGXR.CavePosition;
            }

            var space = GameObject.Find("XRSpace");

            Vector3 position = space.transform.position;
            foreach (var userCamera in DGXR.Config.Space.Screens)
            {
                var cam = DGXR.Space[userCamera.Screen];
                Debug.LogError($"set {userCamera.Screen.ToString() }eye{ eyePosition }");
                cam.SpaceCamera.transform.position = eyePosition + position;
            }
        }

        /// <summary>
        /// 计算根据人和屏幕位置计算相机 fov 
        /// </summary>
        private void SetHeadFovAndOrientationScreen(int index, Camera spaceCamera, Dictionary<int, GameObject[]> edges)
        {
            var cameraTransform = spaceCamera.transform;
            var localPosition = cameraTransform.localPosition;
            var parentLocalScale = cameraTransform.parent.lossyScale;
            var bottomToTop = edges[index][0].transform.position - edges[index][2].transform.position;
            var leftToRight = edges[index][1].transform.position - edges[index][3].transform.position;
            spaceCamera.ResetProjectionMatrix();
            spaceCamera.fieldOfView = -2 * Mathf.Rad2Deg *
                                      Mathf.Atan(bottomToTop.magnitude / 2 /
                                                 (cameraTransform.localPosition.z *
                                                  parentLocalScale.z));
            float obV = localPosition.y *
                parentLocalScale.y / (bottomToTop.magnitude / 2);
            float obH = localPosition.x *
                parentLocalScale.x / (leftToRight.magnitude / 2);
            SetObliqueness(-obH, -obV, spaceCamera);
        }

        void SetObliqueness(float horizObl, float vertObl, Camera cam)
        {
            Matrix4x4 mat = cam.projectionMatrix;
            mat[0, 2] = horizObl;
            mat[1, 2] = vertObl;
            cam.projectionMatrix = mat;
        }

        public void OnDestroy()
        {
#if UNITY_EDITOR
            RenderPipelineManager.endFrameRendering -= HandleSplitScreen;
#endif
        }

        /// <summary>
        /// 初始化 XR空间
        /// </summary> 
        private void InstantiateXR()
        {
            Transform space = gameObject.FindChildGameObject("XRSpace").transform;

            XRSpace.Instance.Origin = Vector3.zero + space.transform.position;
            // TODO UI相机和 UIROOT待更新、先完成 3D 部分
            GameObject uiCameraGroup = GameObject.Find("2DCameraGroup");
            var uiRoot = GameObject.Find("UIRoot");

            XRSpace.Instance.gameObject = space.gameObject;

            // TODO 配置文件待实地考察测量
            XRSpace.Instance.RealSize = new Vector3(DGXR.Config.Space.Length, DGXR.Config.Space.Height,
                DGXR.Config.Space.Width);

            var length = spaceScale * (isCave ? DGXR.Space.RealSize.x : 15f);
            var width = spaceScale * (isCave ? DGXR.Space.RealSize.z : 10f);
            var height = spaceScale * (isCave ? DGXR.Space.RealSize.y : 3.125f);
            XRSpace.Instance.Size = new Vector3(length, height, width);

            if (DGXR.Config.Space.Roi.Length == 4)
            {
                XRSpace.Instance.Roi = new Rect(DGXR.Config.Space.Roi[0], DGXR.Config.Space.Roi[1],
                    DGXR.Config.Space.Roi[2], DGXR.Config.Space.Roi[3]);
            }

            foreach (var screen in DGXR.Config.Space.Screens)
            {
                // var position = new Vector3(screen.Position.x, screen.Position.y, screen.Position.z);
                // var rotation = Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, screen.Rotation.z);
                // if (screen.Screen == TargetScreen.Bottom)
                // {
                //     _buttonRotation = screen.Rotation.z;
                //     rotation = Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, 0);
                // }

                // var scale = new Vector3(screen.Scale.x, screen.Scale.y, screen.Scale.z);

                Transform quad = space.Find(screen.Screen.ToString());
                // TODO UI相机待实现
                // var uiCamera = Extends.FindChildGameObject(uiCameraGroup, screen.Screen.ToString())
                //     .GetComponent<Camera>();
                // uiCamera.gameObject.SetActive(true);

                Camera spaceCamera;
                GameObject screenObject;

                screenObject = quad.gameObject;
                MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }

                spaceCamera = Extends.FindChildGameObject(screenObject, "UserViewCamera")
                    .GetComponent<Camera>();
                spaceCamera.targetDisplay = (int)screen.TargetScreen;
                if (!screen.Enable)
                {
                    // spaceCamera.clearFlags = CameraClearFlags.Depth;
                    // spaceCamera.cullingMask = 0;
                    spaceCamera.enabled = false;
                }
                // Vector2 size = new Vector2();
                // switch (screen.Screen)
                // {
                //     case TargetScreen.Front:
                //         size = new Vector2(length, height);
                //         break;
                //     case TargetScreen.Back:
                //         size = new Vector2(length, height);
                //         break;
                //     case TargetScreen.Left:
                //         size = new Vector2(width, height);
                //         break;
                //     case TargetScreen.Right:
                //         size = new Vector2(width, height);
                //         break;
                //     case TargetScreen.Bottom:
                //         size = new Vector2(length, width);
                //         break;
                // }
            
               Debug.LogError(screen.TargetScreen+"-"+screen.Enable); 
                var dis = new ScreenInfo(screen)
                {
                    Resolution = new Resolution
                    {
                        width = _screenWidth,
                        height = (screen.Screen is TargetScreen.TopLeftByBottom or TargetScreen.TopMidByBottom or TargetScreen.TopRightByBottom or TargetScreen.BottomLeftByBottom or TargetScreen.BottomMidByBottom or TargetScreen.BottomRightByBottom )? _screenWidth : _screenHeight
                    },
                   
                    SpaceCamera = spaceCamera,
                    // TODO UI 相机待实现
                    // UICamera = uiCamera,
                    // Size = size,
                    ScreenObject = screenObject
                };
                // ScreenCanvas = uiRoot.transform.Find(screen.Screen.ToString()).gameObject,
                // if (isCave)
                // {
                //     dis.AddCameraToStack(uiCamera);
                // }
#if UNITY_EDITOR
                if (screen.Render.Length > 0 && DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
                {
                    _displayImages ??= new Dictionary<int, RenderImage>();
                    
                    var _frontBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
                    var _backBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
                    var _cropMaterial = new Material(shader);
                    _frontBottomTex.Create();
                    _backBottomTex.Create();
                    var renderImage = new RenderImage
                    {
                        FrontBottomTex = _frontBottomTex,
                        BackBottomTex = _backBottomTex,
                        CropMaterial = _cropMaterial
                    };
                    //根据 render 中配置决定渲染大小，设置_screenWidth
                    // uiCamera.targetTexture = new RenderTexture(_screenWidth, _screenWidth, 24);
                    var renderTexture = new RenderTexture(_screenWidth, _screenWidth, 24);
                    spaceCamera.targetTexture = renderTexture;
                    spaceCamera.Render();
                    RenderTexture.active = renderTexture;
                    renderImage.RenderTexture = renderTexture;
                    foreach (var render in screen.Render)
                    {
                        foreach (var tarDisplay in render.TarDisplay)
                        {
                            GameObject displayImage = Instantiate(displayImagePrefab,
                                DGXR.Space.gameObject.transform.position,
                                DGXR.Space.gameObject.transform.rotation,
                                DGXR.Space.gameObject.transform);
                            Canvas[] displayCanvas = displayImage.GetComponentsInChildren<Canvas>();
                            foreach (var can in displayCanvas)
                            {
                                can.renderMode = RenderMode.ScreenSpaceOverlay;
                                can.targetDisplay = tarDisplay;
                            }

                            RawImage[] drawImage = displayImage.GetComponentsInChildren<RawImage>();
                           
                            if (drawImage.Length > 0)
                            {
                                renderImage.DisplayImages ??= new Dictionary<int, RawImage>();
                                renderImage.DisplayImages[tarDisplay] = drawImage[0];
                            }
                        }
                    }

                    _displayImages[(int)screen.Screen] = renderImage;
                }
#endif
                XRSpace.AddScreen(screen.Screen, dis);
            }
        }
    }
}