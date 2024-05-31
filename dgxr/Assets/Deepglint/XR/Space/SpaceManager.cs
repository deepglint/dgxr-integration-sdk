using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        [FormerlySerializedAs("DisplayImagePrefab")]
        public GameObject displayImagePrefab;

        [FormerlySerializedAs("ScreenPrefab")] public
            GameObject screenPrefab;

        public Shader shader;

        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        [FormerlySerializedAs("LockAll")] [Header("视角跟随相关设置")]
        public Boolean lockAll;

        [FormerlySerializedAs("LockXZ")] public Boolean lockXZ;

        public Boolean isCave;

        private GameObject[] _screens;
        [FormerlySerializedAs("SpaceScale")] public float spaceScale = 1;

        private Vector3 _eyePosition = new Vector3(0, 1.6f, 0);

        private readonly int _caveLayer = 31;
        private GameObject[,] _screenEdges;
        private Vector3 _head = new Vector3(0, 1.6f, 0);
        private Vector3 _headLockPosition;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        private int _screenWidth; // 屏幕宽度
        private int _screenHeight; // 屏幕高度

        private Dictionary<int, RawImage> _displayImages;

        private RefreshRate _refreshRate = new RefreshRate
        {
            numerator = 30,
            denominator = 1
        };

#if !UNITY_EDITOR
        private RenderTexture _renderTexture;
        private RenderTexture _uiRenderTexture;
        private RenderTexture _frontBottomTex;
        private RenderTexture _backBottomTex;
#endif
        private void Awake()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                _screenWidth = GetSystemMetrics(SM_CXSCREEN);
                _screenHeight = GetSystemMetrics(SM_CYSCREEN);
            }
            else
            {
                _screenWidth = 1920;
                _screenHeight = 1200;
            }
        }

        private void OnValidate()
        {
            var space = GameObject.Find("XRSpace").transform;
            space.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
        }

        void Start()
        {
            if (!Global.SystemName.Contains("Mac"))
            {
                foreach (var display in Display.displays)
                {
                    display.Activate(display.systemWidth, display.systemHeight, _refreshRate);
                }

                Screen.SetResolution(_screenWidth, _screenHeight, true);
            }
#if !UNITY_EDITOR
            _uiRenderTexture = new RenderTexture(_screenWidth, _screenWidth, 24);
            _frontBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
            _backBottomTex = new RenderTexture(_screenWidth, _screenHeight, 24);
            _frontBottomTex.Create();
            _backBottomTex.Create();
#endif
            InstantiateXR();
            var numberScreens = Global.Config.Space.Screens.Count;
            _screenEdges = new GameObject[numberScreens, 4];
            foreach (var screen in Global.Config.Space.Screens)
            {
                for (int corner = 0; corner < 4; corner++)
                {
                    var sc = Global.Space[screen.TargetScreen];
                    _screenEdges[(int)screen.TargetScreen, corner] =
                        sc.ScreenObject.transform.GetChild(corner).gameObject;
                }
            }
#if !UNITY_EDITOR
            RenderPipelineManager.endFrameRendering += HandleSplitScreen;
#endif
        }

#if !UNITY_EDITOR
        private void OnApplicationQuit()
        {
            RenderPipelineManager.endFrameRendering -= HandleSplitScreen;
        }
#endif
        void Update()
        {
            if (isCave)
            {
                SetHead();
            }
        }

#if !UNITY_EDITOR
        private void ProcessRendersCoroutine()
        {
            foreach (var screen in Global.Config.Space.Screens)
            {
                if (screen.Render.Length > 0)
                {
                    foreach (var render in screen.Render)
                    {
                        ProcessSingleRender(render);
                    }
                }
            }
        }

        private void ProcessSingleRender(Config.Config.RenderInfo render)
        {
            Rect rect = new Rect(render.Rect[0], render.Rect[1], render.Rect[2], render.Rect[3]);
            ClippedRenderTexture(_renderTexture, rect, render.Display);
            foreach (var tarDisplay in render.TarDisplay)
            {
                if (!_displayImages.TryGetValue(tarDisplay, out var dis))
                {
                    return;
                }

                if (render.Display == 4)
                {
                    _displayImages[tarDisplay].texture = _frontBottomTex;
                }
                else if (render.Display == 5)
                {
                    _displayImages[tarDisplay].texture = _backBottomTex;
                }
            }
        }

        public void HandleSplitScreen(ScriptableRenderContext paramContext, Camera[] paramCamera)
        {
            ProcessRendersCoroutine();
        }

        private void ClippedRenderTexture(RenderTexture sourceTexture, Rect rect, int display)
        {
            Material cropMaterial = new Material(shader);
            cropMaterial.SetTexture("_MainTex", sourceTexture);
            cropMaterial.SetFloat("_OffsetX", rect.x / sourceTexture.width);
            cropMaterial.SetFloat("_OffsetY", rect.y / sourceTexture.height);
            cropMaterial.SetFloat("_ScaleX", rect.width / sourceTexture.width);
            cropMaterial.SetFloat("_ScaleY", rect.height / sourceTexture.height);
            if (display == 4)
            {
                Graphics.Blit(sourceTexture, _frontBottomTex, cropMaterial);
            }
            else if (display == 5)
            {
                Graphics.Blit(sourceTexture, _backBottomTex, cropMaterial);
            }

            RenderTexture.active = null;
        }
#endif
        /// <summary>
        /// 设置空间中人的位置 
        /// </summary>
        public void SetHead()
        {
            Transform space = GameObject.Find("XRSpace").transform;
            XRSpace.Instance.Origin = Vector3.zero + space.transform.position;
            gameObject.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
            
            SetHeadPosition();
            foreach (var screen in Global.Config.Space.Screens)
            {
                var tarDisplay = Global.Space[screen.TargetScreen];
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
            if (Global.CavePosition.x != 0 || Global.CavePosition.y != 0 || Global.CavePosition.z != 0)
            {
                _head = Global.CavePosition;
            }

            var space = GameObject.Find("XRSpace");
            Vector3 position = space.transform.position;
            _headLockPosition = _head*spaceScale + position;
            if (lockAll)
            {
                _headLockPosition = position + _eyePosition;
            }
            else if (lockXZ)
            {
                _headLockPosition = new Vector3(_eyePosition.x, _head.y, _eyePosition.z) + position;
            }

            foreach (var userCamera in Global.Config.Space.Screens)
            {
                var cam = Global.Space[userCamera.TargetScreen];
                cam.SpaceCamera.transform.position = _headLockPosition;
            }
        }
        /// <summary>
        /// 计算根据人和屏幕位置计算相机 fov 
        /// </summary>
        private void SetHeadFovAndOrientationScreen(int index, Camera spaceCamera, GameObject[,] edges)
        {
            var cameraTransform = spaceCamera.transform;
            var bottomToTop = edges[index, 0].transform.position - edges[index, 2].transform.position;
            var leftToRight = edges[index, 1].transform.position - edges[index, 3].transform.position;
            spaceCamera.ResetProjectionMatrix();
            spaceCamera.fieldOfView = -2 * Mathf.Rad2Deg *
                                      Mathf.Atan(bottomToTop.magnitude / 2 /
                                                 (cameraTransform.localPosition.z *
                                                  cameraTransform.parent.lossyScale.z));
            float obV = cameraTransform.localPosition.y *
                cameraTransform.parent.lossyScale.y / (bottomToTop.magnitude / 2);
            float obH = cameraTransform.localPosition.x *
                cameraTransform.parent.lossyScale.x / (leftToRight.magnitude / 2);
            SetObliqueness(-obH, -obV, spaceCamera);
        }

        void SetObliqueness(float horizObl, float vertObl, Camera cam)
        {
            Matrix4x4 mat = cam.projectionMatrix;
            mat[0, 2] = horizObl;
            mat[1, 2] = vertObl;
            cam.projectionMatrix = mat;
        }
        
        /// <summary>
        /// 初始化 XR空间
        /// </summary> 
        private void InstantiateXR()
        {
            Transform space = GameObject.Find("XRSpace").transform;
            XRSpace.Instance.Origin = Vector3.zero + space.transform.position;
            GameObject uiCameraGroup = GameObject.Find("2DCameraGroup");

            var uiRoot = GameObject.Find("UIRoot");
            XRSpace.Instance.Length = Global.Config.Space.Length;
            XRSpace.Instance.Width = Global.Config.Space.Width;
            XRSpace.Instance.Height = Global.Config.Space.Height;
            XRSpace.Instance.Roi = new Vector2(Global.Config.Space.Roi[0], Global.Config.Space.Roi[1]);

            foreach (var screen in Global.Config.Space.Screens)
            {
                var position = new Vector3(screen.Position.x, screen.Position.y, screen.Position.z);
                var rotation = Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, screen.Rotation.z);
                var scale = new Vector3(screen.Size.x, screen.Size.y, screen.Size.z);
                var dis = new ScreenInfo(screen)
                {
                    Resolution = new Resolution
                    {
                        width = _screenWidth,
                        height = screen.TargetScreen == TargetScreen.Bottom ? _screenWidth : _screenHeight
                    },
                    ScreenCanvas = uiRoot.transform.Find(screen.TargetScreen.ToString()).gameObject
                };

                Transform quad = space.Find(screen.TargetScreen.ToString());
                Camera spaceCamera;
                if (isCave)
                {
                    Destroy(quad.gameObject);
                    var displayQuad = Instantiate(screenPrefab, space.transform);
                    displayQuad.transform.localPosition = position;
                    displayQuad.transform.localRotation = rotation;
                    displayQuad.transform.localScale = scale;
                    displayQuad.name = screen.TargetScreen.ToString();
                    spaceCamera = Instantiate(userViewCameraPrefab, space.transform.position,
                        displayQuad.transform.rotation, displayQuad.transform);

                    spaceCamera.gameObject.layer = _caveLayer;
                    spaceCamera.targetDisplay = (int)screen.TargetScreen;
                    dis.ScreenObject = displayQuad;
                    dis.SpaceCamera = spaceCamera;
                }
                else
                {
                    dis.ScreenObject = quad.gameObject;
                    MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = false;
                    }
                    spaceCamera = Extends.FindChildGameObject(dis.ScreenObject, "UserViewCamera")
                        .GetComponent<Camera>();
                    dis.SpaceCamera = spaceCamera;
                }
                
                var uiCamera = Extends.FindChildGameObject(uiCameraGroup, screen.TargetScreen.ToString())
                    .GetComponent<Camera>();
                uiCamera.gameObject.SetActive(true);
                dis.UICamera = uiCamera;
                dis.AddCameraToStack(uiCamera);
#if !UNITY_EDITOR
                if (screen.Render.Length > 0)
                {
                    uiCamera.targetTexture = _uiRenderTexture;
                    _renderTexture = new RenderTexture(_screenWidth, _screenWidth, 24);
                    spaceCamera.targetTexture = _renderTexture;
                    spaceCamera.Render();
                    RenderTexture.active = _renderTexture;
                    foreach (var render in screen.Render)
                    {
                        foreach (var tarDisplay in render.TarDisplay)
                        {
                            GameObject displayImage = Instantiate(displayImagePrefab,
                                spaceCamera.transform.position,
                                spaceCamera.transform.rotation,
                                spaceCamera.transform);
                            Canvas[] displayCanvas = displayImage.GetComponentsInChildren<Canvas>();
                            foreach (var can in displayCanvas)
                            {
                                can.renderMode = RenderMode.ScreenSpaceOverlay;
                                can.targetDisplay = tarDisplay;
                            }

                            RawImage[] drawImage = displayImage.GetComponentsInChildren<RawImage>();
                            _displayImages ??= new Dictionary<int, RawImage>();
                            if (drawImage.Length > 0)
                            {
                                _displayImages[tarDisplay] = drawImage[0];
                            }
                        }
                    }
                }
#endif
                XRSpace.AddScreen(screen.TargetScreen, dis);
                
            }
        }
    }
}