using System;
using Moat.Model;
using UnityEngine;
using VRKave;

public class CameraRoamControl : MonoBehaviour
{
    public static CameraRoamControl Instance;
    private float _speed = 10f; // 移动速度
    private float _rotationSpeed = 30f; // 旋转速度
    private float _width = 16;
    private float _height = 10;
    public float viewHeight = 5f;
    public float humanEye = 1.4f;
    
    public GameObject camera3D;
    public GameObject cameraCave;
    private Transform _currentCameraObj;

    private Camera _cameraLeft;
    private Camera _cameraFront;
    private Camera _cameraRight;
    private Camera _cameraBack;
    private Camera _cameraBottom1;
    private Camera _cameraBottom2;

    [HideInInspector] public bool isCave;
    [HideInInspector] public bool isRoam = true;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float horizontalInput;

    private void Awake()
    {
        DisplayData.ReadConfig();
        isRoam = DisplayData.configDisplay.allowRoam;
        isCave = DisplayData.configDisplay.allowCave;
        Instance = this;
        if (isCave)
        {
            if (camera3D != null)
            {
                camera3D?.SetActive(false);
            }
        }
        else
        {
            if (cameraCave != null)
            {
                cameraCave?.SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (isCave)
        {
            _currentCameraObj = cameraCave.GetComponent<Transform>();
            _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, (float)(viewHeight - humanEye), _currentCameraObj.position.z);
            if (camera3D != null)
            {
                camera3D?.SetActive(false);
            }
        }
        else
        {
            _currentCameraObj = camera3D.GetComponent<Transform>();
            _currentCameraObj.position = new Vector3(_currentCameraObj.position.x, viewHeight, _currentCameraObj.position.y);
            if (cameraCave != null)
            {
                cameraCave?.SetActive(false);
            }
        }

        if (!isCave && camera3D != null)
        {
            Camera[] cameras = camera3D.GetComponentsInChildren<Camera>();

            // 遍历相机数组，访问每一个相机对象
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera.name == "Left") _cameraLeft = camera; 
                if (camera.name == "Front") _cameraFront = camera; 
                if (camera.name == "Right") _cameraRight = camera; 
                if (camera.name == "Back") _cameraBack = camera; 
                if (camera.name == "Bottom1") _cameraBottom1 = camera; 
                if (camera.name == "Bottom2") _cameraBottom2 = camera;  
            } 
        }
    }

    void Update()
    {
        // 获取按键输入
        if (isRoam)
        {
            // verticalInput = Input.GetAxis("Vertical"); // W和S键
            // horizontalInput = Input.GetAxis("Horizontal"); // A和D键 
        }
        
        if (verticalInput != 0 || horizontalInput != 0 && _currentCameraObj != null)
        {
            // 根据输入和速度移动物体
            _currentCameraObj.Translate(Vector3.forward * verticalInput * _speed * Time.deltaTime);

            // 根据水平输入旋转物体
            _currentCameraObj.Rotate(Vector3.up * horizontalInput * _rotationSpeed * Time.deltaTime);

            if (isCave)
            {
                UpdateVRCamera(_currentCameraObj.position);
            }
            else
            {
                Update3DCamera(_currentCameraObj.position);
            }
        }
    }

    private void UpdateVRCamera(Vector3 headLockPosition)
    {
        VRWorldManager.instance.centerViewPoint =
        new Vector3(headLockPosition.x, headLockPosition.y + humanEye, headLockPosition.z);
    }

    private void Update3DCamera(Vector3 headLockPosition)
    {
        _cameraLeft.transform.position = headLockPosition;
        _cameraFront.transform.position = headLockPosition;
        _cameraRight.transform.position = headLockPosition;
        _cameraBack.transform.position = headLockPosition;
        float diff = (_width - _height) / 2;
        _cameraBottom1.transform.position = headLockPosition + new Vector3(0, 0, diff);
        _cameraBottom2.transform.position = headLockPosition + new Vector3(0, 0, -diff);
    }
}