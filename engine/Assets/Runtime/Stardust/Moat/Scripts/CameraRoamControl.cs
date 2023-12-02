using System;
using UnityEngine;
using VRKave;

public class CameraRoamControl : MonoBehaviour
{
    public static CameraRoamControl Instance;
    public float speed = 10f; // 移动速度
    public float rotationSpeed = 30f; // 旋转速度
    public bool isCave;
    public GameObject camera3D;
    public GameObject cameraCave;
    private Transform currentCameraObj;
    public float width = 16;
    public float height = 10;
    public float viewHeight = 5f;
    public float humanEye = 1.4f;

    private Camera camera1;
    private Camera camera2;
    private Camera camera3;
    private Camera camera4;
    private Camera camera5;
    private Camera camera6;

    public bool isRoam = true;
    private float verticalInput;
    private float horizontalInput;

    private void Awake()
    {
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
            currentCameraObj = cameraCave.GetComponent<Transform>();
            currentCameraObj.position = new Vector3(0, (float)(viewHeight - humanEye), 0);
            if (camera3D != null)
            {
                camera3D?.SetActive(false);
            }
        }
        else
        {
            currentCameraObj = camera3D.GetComponent<Transform>();
            currentCameraObj.position = new Vector3(0, viewHeight, 0);
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
                switch (camera.name)
                {
                    case "Left":
                        camera1 = camera; 
                        break;
                    case "Front":
                        camera2 = camera;
                        break;
                    case "Right":
                        camera3 = camera;
                        break;
                    case "Back":
                        camera4 = camera;
                        break;
                    case "Bottom1":
                        camera5 = camera;
                        break;
                    case "Bottom2":
                        camera6 = camera;
                        break;
                } 
            } 
        }
    }

    void Update()
    {
        // 获取按键输入
        if (isRoam)
        {
            verticalInput = Input.GetAxis("Vertical"); // W和S键
            horizontalInput = Input.GetAxis("Horizontal"); // A和D键 
        }
        
        if (verticalInput != 0 || horizontalInput != 0 && currentCameraObj != null)
        {
            // 根据输入和速度移动物体
            currentCameraObj.Translate(Vector3.forward * verticalInput * speed * Time.deltaTime);

            // 根据水平输入旋转物体
            currentCameraObj.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);

            if (isCave)
            {
                UpdateVRCamera(currentCameraObj.position);
            }
            else
            {
                Update3DCamera(currentCameraObj.position);
            }
        }
    }

    public void UpdateVRCamera(Vector3 headLockPosition)
    {
        VRWorldManager.instance.centerViewPoint =
        new Vector3(headLockPosition.x, headLockPosition.y + humanEye, headLockPosition.z);
    }

    public void Update3DCamera(Vector3 headLockPosition)
    {
        camera1.transform.position = headLockPosition;
        camera2.transform.position = headLockPosition;
        camera3.transform.position = headLockPosition;
        camera4.transform.position = headLockPosition;
        float diff = (width - height) / 2;
        camera5.transform.position = headLockPosition + new Vector3(0, 0, diff);
        camera6.transform.position = headLockPosition + new Vector3(0, 0, -diff);
    }
}