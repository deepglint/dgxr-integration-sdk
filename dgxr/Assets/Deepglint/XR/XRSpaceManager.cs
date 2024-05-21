using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Deepglint.Tool.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

namespace Deepglint.XR
{
    [DefaultExecutionOrder(-99)]
    public class XRSpaceManager : MonoBehaviour
    {
        [FormerlySerializedAs("DisplayImagePrefab")]
        public GameObject displayImagePrefab;

        [FormerlySerializedAs("ScreenPrefab")] public GameObject screenPrefab;
        public Shader shader;

        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        [FormerlySerializedAs("LockAll")] [Header("视角跟随相关设置")]
        public Boolean lockAll;

        [FormerlySerializedAs("LockXZ")] public Boolean lockXZ;

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
            SetHead();
        }

#if !UNITY_EDITOR
        public void HandleSplitScreen(ScriptableRenderContext paramContext, Camera[] paramCamera)
        {
            foreach (var screen in Global.Config.Space.Screens)
            {
                if (screen.Render.Length > 0)
                {
                    foreach (var render in screen.Render)
                    {
                        Rect rect = new Rect(render.Rect[0], render.Rect[1], render.Rect[2], render.Rect[3]);

                        Texture tex = ClippedRenderTexture(_renderTexture,
                            rect);
                        if (_displayImages[render.Display]?.texture != null)
                        {
                            Destroy(_displayImages[render.Display].texture);
                        }
                        _displayImages[render.Display].texture = tex;
                    }
                }
            }
        }
#endif
        public void SetHead()
        {
            Transform space = GameObject.Find("Space").transform;
            Space.Instance.Origin = Vector3.zero + space.transform.position;
            gameObject.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
            // 处理坐标的比例关系
            //Set the position
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

        private void SetHeadPosition()
        {
            if (Global.CavePosition.x != 0 || Global.CavePosition.y != 0 || Global.CavePosition.z != 0)
            {
                _head = Global.CavePosition;
            }

            var space = GameObject.Find("Space");
            Vector3 position = space.transform.position;
            _headLockPosition = _head + position;
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

        private Texture ClippedRenderTexture(RenderTexture sourceTexture, Rect rect)
        {
            int width = (int)rect.width;
            int height = (int)rect.height;
            RenderTexture croppedTexture = new RenderTexture(width, height, 24);
            croppedTexture.Create();
            RenderTexture.active = sourceTexture;
            Material cropMaterial = new Material(shader);
            cropMaterial.SetTexture("_MainTex", sourceTexture);
            cropMaterial.SetFloat("_OffsetX", rect.x / sourceTexture.width);
            cropMaterial.SetFloat("_OffsetY", rect.y / sourceTexture.height);
            cropMaterial.SetFloat("_ScaleX", rect.width / sourceTexture.width);
            cropMaterial.SetFloat("_ScaleY", rect.height / sourceTexture.height);
            Graphics.Blit(sourceTexture, croppedTexture, cropMaterial);
            RenderTexture.active = null;
            return croppedTexture;
        }


        private void InstantiateXR()
        {
            Transform space = GameObject.Find("Space").transform;
            Space.Instance.Origin = Vector3.zero + space.transform.position;
            GameObject uiCameraGroup = GameObject.Find("2DCameraGroup");

            var uiRoot = GameObject.Find("UIRoot");
            Space.Instance.Length = Global.Config.Space.Length;
            Space.Instance.Width = Global.Config.Space.Width;
            Space.Instance.Height = Global.Config.Space.Height;
            // Space.Instance.Roi = new Rect(Global.Config.Space.Roi[0], Global.Config.Space.Roi[1],
            //     Global.Config.Space.Roi[2], Global.Config.Space.Roi[3]);

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
                Destroy(quad.gameObject);
                var displayQuad = Instantiate(screenPrefab, space.transform);
                displayQuad.transform.localPosition = position;
                displayQuad.transform.localRotation = rotation;
                displayQuad.transform.localScale = scale;
                displayQuad.name = screen.TargetScreen.ToString();
                var spaceCamera = Instantiate(userViewCameraPrefab, space.transform.position,
                    displayQuad.transform.rotation, displayQuad.transform);

                spaceCamera.gameObject.layer = _caveLayer;
                spaceCamera.targetDisplay = (int)screen.TargetScreen;
                dis.ScreenObject = displayQuad;
                var uiCamera = Extends.FindChildGameObject(uiCameraGroup, screen.TargetScreen.ToString())
                    .GetComponent<Camera>();
                uiCamera.gameObject.SetActive(true);
                if (uiCamera != null)
                {
                    UniversalAdditionalCameraData cameraData = uiCamera.GetUniversalAdditionalCameraData();
                    cameraData.renderType = CameraRenderType.Overlay;
                    spaceCamera.GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
                }

#if !UNITY_EDITOR
                if (screen.Render.Length > 0)
                {
                    var _renderTexture = new RenderTexture(_screenWidth	, _screenWidth, 24);
                    spaceCamera.targetTexture = _renderTexture;
                    spaceCamera.Render();
                    RenderTexture.active = _renderTexture;
                    foreach (var render in screen.Render)
                    {
                        GameObject displayImage = Instantiate(displayImagePrefab,
                            spaceCamera.transform.position,
                            spaceCamera.transform.rotation,
                            spaceCamera.transform);
                        Canvas[] displayCanvas = displayImage.GetComponentsInChildren<Canvas>();
                        foreach (var can in displayCanvas)
                        {
                            can.renderMode = RenderMode.ScreenSpaceOverlay;
                            can.targetDisplay = render.Display;
                        }
                        RawImage[] drawImage = displayImage.GetComponentsInChildren<RawImage>();
                        _displayImages ??= new Dictionary<int, RawImage>();
                        if (drawImage.Length > 0)
                        {
                            _displayImages[render.Display] = drawImage[0];
                        }
                    }
                }
#endif
                dis.SpaceCamera = spaceCamera;
                dis.UICamera = uiCamera;
                Space.AddScreen(screen.TargetScreen, dis);
            }
        }
    }
}