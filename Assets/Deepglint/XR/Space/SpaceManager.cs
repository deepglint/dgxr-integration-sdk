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
        [FormerlySerializedAs("DisplayImagePrefab")]
        public GameObject displayImagePrefab;

        [FormerlySerializedAs("ScreenPrefab")] public
            GameObject screenPrefab;

        public Shader shader;
        private float _buttonRotation;

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
        private Dictionary<int, GameObject[]> _screenEdges;
        private Vector3 _head = new Vector3(0, 1.6f, 0);
        private Vector3 _headLockPosition;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        private int _screenWidth; // 屏幕宽度
        private int _screenHeight; // 屏幕高度

        private Dictionary<int, RawImage> _displayImages;

        private readonly RefreshRate _refreshRate = new RefreshRate
        {
            numerator = 30,
            denominator = 1
        };

#if !UNITY_EDITOR
        private RenderTexture _renderTexture;
        private RenderTexture _uiRenderTexture;
        private RenderTexture _frontBottomTex;
        private RenderTexture _backBottomTex;
        private Material _cropMaterial;
#endif
        private void Awake()
        {
#if !UNITY_EDITOR
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
        }

        private void OnValidate()
        {
            var space = GameObject.Find("XRSpace");
            if (space is not null)
            {
                space.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
            }
        }

        void Start()
        {
            if (!DGXR.SystemName.Contains("Mac") && DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
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
            _cropMaterial = new Material(shader);
            _frontBottomTex.Create();
            _backBottomTex.Create();
#endif
            InstantiateXR();
            var numberScreens = DGXR.Config.Space.Screens.Count;
            _screenEdges = new Dictionary<int, GameObject[]>();
            foreach (var screen in DGXR.Config.Space.Screens)
            {
                GameObject[] games = new GameObject[4];
                for (int corner = 0; corner < 4; corner++)
                {
                    var sc = DGXR.Space[screen.TargetScreen];
                    games[corner] = sc.ScreenObject.transform.GetChild(corner).gameObject;
                }

                _screenEdges.Add((int)screen.TargetScreen, games);
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
            Transform space = GameObject.Find("XRSpace").transform;
            XRSpace.Instance.Origin = space.transform.position;
            if (isCave)
            {
                SetHead();
            }
        }

#if !UNITY_EDITOR
        private void ProcessRendersCoroutine()
        {
            foreach (var screen in DGXR.Config.Space.Screens)
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
            if (DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
            {
                ProcessRendersCoroutine();
            }
        }

        private void ClippedRenderTexture(RenderTexture sourceTexture, Rect rect, int display)
        {
            _cropMaterial.SetTexture("_MainTex", sourceTexture);
            _cropMaterial.SetFloat("_OffsetX", rect.x / sourceTexture.width);
            _cropMaterial.SetFloat("_OffsetY", rect.y / sourceTexture.height);
            _cropMaterial.SetFloat("_ScaleX", rect.width / sourceTexture.width);
            _cropMaterial.SetFloat("_ScaleY", rect.height / sourceTexture.height);
            _cropMaterial.SetFloat("_Rotation", _buttonRotation * Mathf.Deg2Rad);
            if (display == 4)
            {
                Graphics.Blit(sourceTexture, _frontBottomTex, _cropMaterial);
            }
            else if (display == 5)
            {
                Graphics.Blit(sourceTexture, _backBottomTex, _cropMaterial);
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
                var tarDisplay = DGXR.Space[screen.TargetScreen];
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
                _head = DGXR.CavePosition;
            }

            _headLockPosition = _head;
            var space = GameObject.Find("XRSpace");
            Vector3 position = space.transform.position;
            if (lockAll)
            {
                _headLockPosition = position + _eyePosition * spaceScale;
            }
            else if (lockXZ)
            {
                _headLockPosition = new Vector3(_eyePosition.x, _head.y, _eyePosition.z) * spaceScale + position;
            }

            foreach (var userCamera in DGXR.Config.Space.Screens)
            {
                var cam = DGXR.Space[userCamera.TargetScreen];
                cam.SpaceCamera.transform.position = _headLockPosition+position;
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
#if !UNITY_EDITOR
            RenderPipelineManager.endFrameRendering -= HandleSplitScreen;
#endif
        }

        /// <summary>
        /// 初始化 XR空间
        /// </summary> 
        private void InstantiateXR()
        {
            Transform space = GameObject.Find("XRSpace").transform;

            if (isCave)
            {
                foreach (Transform child in space.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            XRSpace.Instance.Origin = Vector3.zero + space.transform.position;
            GameObject uiCameraGroup = GameObject.Find("2DCameraGroup");
            foreach (Transform child in uiCameraGroup.transform)
            {
                child.gameObject.SetActive(false);
            }

            var uiRoot = GameObject.Find("UIRoot");
            XRSpace.Instance.gameObject = space.gameObject;

            XRSpace.Instance.RealSize = new Vector3(DGXR.Config.Space.Length, DGXR.Config.Space.Height,
                DGXR.Config.Space.Width);

            var length = spaceScale * (isCave
                ? (DGXR.Space.RealSize.x > DGXR.Space.RealSize.z
                    ? DGXR.Space.RealSize.x
                    : DGXR.Space.RealSize.z)
                : 5);
            var height = spaceScale * (isCave ? DGXR.Space.RealSize.y : 3.125f);
            XRSpace.Instance.Size = new Vector3(length, height, length);
            if (DGXR.Config.Space.Roi.Length == 4)
            {
                XRSpace.Instance.Roi = new Rect(DGXR.Config.Space.Roi[0], DGXR.Config.Space.Roi[1],
                    DGXR.Config.Space.Roi[2], DGXR.Config.Space.Roi[3]);
            }

            foreach (var screen in DGXR.Config.Space.Screens)
            {
                var position = new Vector3(screen.Position.x, screen.Position.y, screen.Position.z);
                var rotation = Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, screen.Rotation.z);
                if (screen.TargetScreen == TargetScreen.Bottom)
                {
                    _buttonRotation = screen.Rotation.z;
                    rotation = Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, 0);
                }

                var scale = new Vector3(screen.Scale.x, screen.Scale.y, screen.Scale.z);


                Transform quad = space.Find(screen.TargetScreen.ToString());
                var uiCamera = Extends.FindChildGameObject(uiCameraGroup, screen.TargetScreen.ToString())
                    .GetComponent<Camera>();
                uiCamera.gameObject.SetActive(true);

                Camera spaceCamera;
                GameObject screenObject;
                if (isCave)
                {
                    var displayQuad = Instantiate(screenPrefab, space.transform);
                    displayQuad.transform.localPosition = position;
                    displayQuad.transform.localRotation = rotation;
                    displayQuad.transform.localScale = scale;
                    displayQuad.name = screen.TargetScreen.ToString();
                    spaceCamera = Instantiate(userViewCameraPrefab, space.transform.position,
                        displayQuad.transform.rotation, displayQuad.transform);

                    spaceCamera.gameObject.layer = _caveLayer;
                    spaceCamera.targetDisplay = (int)screen.TargetScreen;

                    screenObject = displayQuad;
                }
                else
                {
                    screenObject = quad.gameObject;
                    MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = false;
                    }

                    spaceCamera = Extends.FindChildGameObject(screenObject, "UserViewCamera")
                        .GetComponent<Camera>();
                }

                var screenScale = screenObject.transform.localScale;
                var dis = new ScreenInfo(screen)
                {
                    Resolution = new Resolution
                    {
                        width = _screenWidth,
                        height = screen.TargetScreen == TargetScreen.Bottom ? _screenWidth : _screenHeight
                    },
                    ScreenCanvas = uiRoot.transform.Find(screen.TargetScreen.ToString()).gameObject,
                    SpaceCamera = spaceCamera,
                    UICamera = uiCamera,
                    Size = new Vector2(screenScale.x * spaceScale, screenScale.y * spaceScale),
                    ScreenObject = screenObject
                };
                if (isCave)
                {
                    dis.AddCameraToStack(uiCamera);
                }
#if !UNITY_EDITOR
                if (screen.Render.Length > 0 && DGXR.Config.Space.ScreenMode == ScreenStyle.Default)
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