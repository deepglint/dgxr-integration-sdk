using System;
using Moat;
using Moat.Model;
using UnityEngine;
using UnityEngine.Serialization;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Stardust.Scripts
{
    public class XRWorldManager : MonoBehaviour
    {
        [FormerlySerializedAs("SensorPrefab")] public GameObject sensorPrefab;

        [FormerlySerializedAs("ProjectorPrefab")]
        public Camera projectorPrefab;

        [FormerlySerializedAs("SurfacePrefab")]
        public GameObject surfacePrefab;

        [FormerlySerializedAs("UserViewCameraPrefab")]
        public Camera userViewCameraPrefab;

        [FormerlySerializedAs("ScreenPrefab")] public GameObject screenPrefab;

        public LayerMask cameraLayer;
        private int _caveLayer = 31;

        private XRLoadCalibration _configuration = new XRLoadCalibration();

        private GameObject _sensor;
        private Camera[] _projectors;
        private GameObject[] _surfaces;
        private GameObject[] _screens;
        private GameObject[,] _surfaceEdges;
        private GameObject[,] _screenEdges;
        private static Camera[] _userProjectorViewCameras;
        private Camera[] _userScreenViewCameras;
        private RenderTexture[] _surfaceTextures;

        public static XRWorldManager Instance;

        [FormerlySerializedAs("LockAll")] [Header("视角跟随相关设置")]
        public Boolean lockAll;

        [FormerlySerializedAs("LockXZ")] public Boolean lockXZ;

        // 当LockAll\LockXY被勾选时，相机位置根据_headLockPosition进行设置，而不是再走真实空间中人的头的位置
        private Vector3 _headLockPosition;

        // Kave标定空间的00点在地面上，当LockAll\LockXY被勾选时，根据此值去模拟真实的人站在空间中的眼高
        private Vector3 _eyeHeight = new Vector3(0, 1.6f, 0);

        // 真实世界单位与Unity单位的比例。
        // 例如：一个2米高的墙，spaceScale设置为3，墙壁最终高度为2 * 3个Unity单位
        [FormerlySerializedAs("SpaceScale")] public float spaceScale = 1;

        private GameObject _head;
        private int _numberScreens;
        private int _numberSurfaces;

        private int _textureWidth = 2800;
        private int _textureHeight = 1050;
        [Header("是否使用UI叠加渲染")] public bool isUIRender;
        [FormerlySerializedAs("_uiCameras")] public Camera[] uiCameras;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Load();
            InstantiateVR();
            _head = _sensor.transform.Find("Body").transform.Find("Head").gameObject;

            _surfaceEdges = new GameObject[_numberSurfaces, 4];
            for (int surface = 0; surface < _numberSurfaces; surface++)
            for (int corner = 0; corner < 4; corner++)
                _surfaceEdges[surface, corner] = _surfaces[surface].transform.GetChild(corner).gameObject;

            _screenEdges = new GameObject[_numberScreens, 4];
            for (int screen = 0; screen < _numberScreens; screen++)
            for (int corner = 0; corner < 4; corner++)
                _screenEdges[screen, corner] = _screens[screen].transform.GetChild(corner).gameObject;
        }

        public Camera[] GetCameras()
        {
            return _userProjectorViewCameras;
        }

        private void InstantiateVR()
        {
            _numberScreens = _configuration.Screens.Length;
            _numberSurfaces = _configuration.Surfaces.Length;

            //Instatiate the Sensor:
            switch (_configuration.Sensors.Type)
            {
                case XRLoadCalibration.SensorType.Dg:
                    _sensor = Instantiate(sensorPrefab, transform);
                    break;
                case XRLoadCalibration.SensorType.ArtTrack:
                    break;
            }

            _sensor.transform.localPosition = new Vector3(_configuration.Sensors.Position.x,
                _configuration.Sensors.Position.y, _configuration.Sensors.Position.z);
            _sensor.transform.localRotation = Quaternion.Euler(_configuration.Sensors.Rotation.x,
                _configuration.Sensors.Rotation.y, _configuration.Sensors.Rotation.z);

            _sensor.gameObject.layer = _caveLayer;

            //Instantiate the Surfaces:
            int index = 0;
            _surfaces = new GameObject[_configuration.Surfaces.Length];
            _surfaceTextures = new RenderTexture[_configuration.Surfaces.Length];
            foreach (var surface in _configuration.Surfaces)
            {
                _surfaces[index] = Instantiate(surfacePrefab, transform);
                _surfaces[index].transform.localPosition =
                    new Vector3(surface.Position.x, surface.Position.y, surface.Position.z);
                _surfaces[index].transform.localRotation =
                    Quaternion.Euler(surface.Rotation.x, surface.Rotation.y, surface.Rotation.z);
                _surfaces[index].transform.Rotate(new Vector3(0, 180, 0));
                _surfaces[index].transform.localScale =
                    new Vector3(surface.Size.x / 10, surface.Size.y / 10, surface.Size.z / 10);
                _surfaces[index].layer = _caveLayer;
                _surfaceTextures[index] =
                    new RenderTexture(_textureWidth, _textureHeight, 24, RenderTextureFormat.ARGB32);
                _surfaceTextures[index].antiAliasing = 2;
                _surfaceTextures[index].Create();
                index++;
            }


            //Instantiate the CAVE projectors:
            index = 0;
            _projectors = new Camera[_configuration.Projectors.Length];
            foreach (var projector in _configuration.Projectors)
            {
                _projectors[index] = Instantiate(projectorPrefab, transform);
                _projectors[index].transform.localPosition =
                    new Vector3(projector.Position.x, projector.Position.y, projector.Position.z);
                _projectors[index].transform.localRotation = Quaternion.Euler(projector.Rotation.x,
                    projector.Rotation.y, projector.Rotation.z);
#if !UNITY_EDITOR
                if (Display.displays.Length >= projector.Display)
                    _projectors[index].aspect = (float)Display.displays[projector.Display - 1].renderingWidth /
                                                Display.displays[projector.Display - 1].renderingHeight;
#endif
                _projectors[index].targetDisplay = projector.Display - 1;
                _projectors[index].farClipPlane =
                    (_projectors[index].transform.position -
                     _surfaces[_projectors[index].targetDisplay].transform.position).magnitude * spaceScale * 2;
                _projectors[index].fieldOfView = projector.FOV;
                _projectors[index].name = "projector" + projector.Display;
                SetObliqueness(0, projector.Fy, _projectors[index]);
                _projectors[index].gameObject.layer = _caveLayer;
                //_projectors[index].cullingMask = 1 << (_caveLayer - projector.Display);
                //_projectors[index].cullingMask |= 1 << _caveLayer;
                _projectors[index].gameObject.GetComponent<QuadWarp>().tex.Clear();
                var surfaceIndex = 0;
                foreach (var surface in _configuration.Surfaces)
                {
                    if (surface.Display == projector.Display)
                    {
                        _projectors[index].gameObject.GetComponent<QuadWarp>().tex.Add(_surfaceTextures[surfaceIndex]);
                        _projectors[index].gameObject.GetComponent<QuadWarp>().Vertices.Add(surface.Vertices);
                        _projectors[index].gameObject.GetComponent<QuadWarp>().displayIndex =
                            _projectors[index].targetDisplay;
                    }

                    surfaceIndex++;
                }

                index++;
            }

            //Instantiate the screens:
            index = 0;
            _screens = new GameObject[_configuration.Screens.Length];
            foreach (var screen in _configuration.Screens)
            {
                _screens[index] = Instantiate(screenPrefab, transform);
                _screens[index].transform.localPosition =
                    new Vector3(screen.Position.x, screen.Position.y, screen.Position.z);
                _screens[index].transform.localRotation =
                    Quaternion.Euler(screen.Rotation.x, screen.Rotation.y, screen.Rotation.z);
                _screens[index].transform.localScale = new Vector3(screen.Size.x, screen.Size.y, screen.Size.z);
                index++;
            }

            //Instantiate the user view cameras (cameras attached to the user head in the virtual world) for the projectors:
            _userProjectorViewCameras = new Camera[_configuration.Surfaces.Length];
            for (int i = 0; i < _configuration.Surfaces.Length; i++)
            {
                index = i;
                _userProjectorViewCameras[index] = Instantiate(userViewCameraPrefab, transform.position,
                    _surfaces[index].transform.rotation * Quaternion.Euler(90, 180, 0), _surfaces[index].transform);
                _userProjectorViewCameras[index].aspect = _surfaces[index].transform.localScale.x /
                                                          _surfaces[index].transform.localScale.z;
                _userProjectorViewCameras[index].gameObject.layer = _caveLayer;
                // _userProjectorViewCameras[index].clearFlags = CameraClearFlags.SolidColor;
                _userProjectorViewCameras[index].targetTexture = _surfaceTextures[index];
                // _userProjectorViewCameras[index].cullingMask = -1; //The user is set to only see the default layer. Change this culling mask if you want the camera to see different layers (like water).
                _userProjectorViewCameras[index].cullingMask = cameraLayer;
            }

            //Instantiate the user view cameras (cameras attached to the user head in the virtual world) for the screens:
            _userScreenViewCameras = new Camera[_configuration.Screens.Length];
            for (int i = 0; i < _configuration.Screens.Length; i++)
            {
                index = i;
                _userScreenViewCameras[index] = Instantiate(userViewCameraPrefab, transform.position,
                    _screens[index].transform.rotation, _screens[index].transform);
                _userScreenViewCameras[index].gameObject.layer = _caveLayer;
                //_userScreenViewCameras[index].cullingMask = 1 << ();      //The user is set to only see the default layer. Change this culling mask if you want the camera to see different layers (like water).
                _userScreenViewCameras[index].targetDisplay = _configuration.Screens[index].Display - 1;
            }

            if (isUIRender)
            {
                index = 0;
                foreach (var uicamera in uiCameras)
                {
                    uicamera.clearFlags = CameraClearFlags.SolidColor;
                    var texture = new RenderTexture(1920, 1200, 24, RenderTextureFormat.ARGB32);
                    texture.antiAliasing = 2;
                    texture.Create();
                    uicamera.targetTexture = texture;
                    foreach (var projector in _projectors)
                    {
                        if (projector.gameObject.GetComponent<QuadWarp>().displayIndex == uicamera.targetDisplay)
                        {
                            projector.gameObject.GetComponent<QuadWarp>().tex.Add(texture);
                            projector.gameObject.GetComponent<QuadWarp>().Vertices
                                .Add(projector.gameObject.GetComponent<QuadWarp>().Vertices[0]);
                        }
                    }

                    index++;
                }
            }
        }

        public Vector3 GetHeadPosition()
        {
            var localPosition = _head.transform.localPosition;
            Vector3 scaleHead = localPosition * DisplayData.SpatialProportion;

            // 基于空间点的移动偏移
            float y = scaleHead.z + (scaleHead.z - DisplayData.HumanEye) * DisplayData.SpaceFollowSpeed;
            return new Vector3(localPosition.x, y < 0 ? DisplayData.HumanEye : y, _head.transform.localPosition.y * -1);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
            }

            SetScale();
            SetHead(GetHeadPosition());
        }

        private void SetScale()
        {
            gameObject.transform.localScale = new Vector3(spaceScale, spaceScale, spaceScale);
        }

        public void SetHead(Vector3 headPos)
        {
            // 处理坐标的比例关系

            //Set the position
            SetHeadPosition(headPos);

            //Set the FOV & Orientation
            for (int cameraIndex = 0; cameraIndex < _numberSurfaces; cameraIndex++)
                SetHeadFovAndOrientationProjector(cameraIndex, _userProjectorViewCameras, _surfaceEdges);

            for (int cameraIndex = 0; cameraIndex < _numberScreens; cameraIndex++)
                SetHeadFovAndOrientationScreen(cameraIndex, _userScreenViewCameras, _screenEdges);
        }

        private void SetHeadFovAndOrientationProjector(int index, Camera[] cameras, GameObject[,] edges)
        {
            var bottomToTop = edges[index, 0].transform.position - edges[index, 2].transform.position;
            var leftToRight = edges[index, 1].transform.position - edges[index, 3].transform.position;

            //Set FOV
            cameras[index].ResetProjectionMatrix();
            cameras[index].fieldOfView = 2 * Mathf.Rad2Deg *
                                         Mathf.Atan(bottomToTop.magnitude / 2 /
                                                    (cameras[index].transform.localPosition.y *
                                                     cameras[index].transform.parent.lossyScale.y));

            //Set the orientation
            float obV = cameras[index].transform.localPosition.z *
                cameras[index].transform.parent.lossyScale.z / (bottomToTop.magnitude / 2);
            float obH = cameras[index].transform.localPosition.x *
                cameras[index].transform.parent.lossyScale.x / (leftToRight.magnitude / 2);
            SetObliqueness(obH, obV, cameras[index]);
        }

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

        private void SetHeadPosition(Vector3 headPos)
        {
            _headLockPosition = headPos;
            if (lockAll)
            {
                _headLockPosition = transform.position + _eyeHeight;
            }
            else if (lockXZ)
            {
                var position = transform.position + _eyeHeight;
                _headLockPosition = new Vector3(position.x, headPos.y, position.z);
            }

            foreach (var userCamera in _userProjectorViewCameras)
            {
                userCamera.transform.position = _headLockPosition;
            }

            foreach (var userCamera in _userScreenViewCameras)
                userCamera.transform.position = _headLockPosition;
        }

        private void Load()
        {
            var path = Application.streamingAssetsPath + "/stardust/calibration.xml";
            _configuration.LoadConfiguration(path);
        }

        private void Quit()
        {
            GameAppManager.Instance.CloseApp();
        }

        void SetObliqueness(float horizObl, float vertObl, Camera cam)
        {
            Matrix4x4 mat = cam.projectionMatrix;
            mat[0, 2] = horizObl;
            mat[1, 2] = vertObl;
            cam.projectionMatrix = mat;
        }
    }
}