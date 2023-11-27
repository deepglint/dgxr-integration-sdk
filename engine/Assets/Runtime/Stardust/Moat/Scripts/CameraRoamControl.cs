using System;
using UnityEngine;
using VRKave;

public class CameraRoamControl : MonoBehaviour
{
    public static CameraRoamControl Instance;
    public float speed = 80f; // 移动速度
    public float rotationSpeed = 30f; // 旋转速度
    public bool isKave;
    public GameObject camera3D;
    public GameObject cameraKave;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;
    public Camera camera5;
    public Camera camera6;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 获取按键输入
        float verticalInput = Input.GetAxis("Vertical"); // W和S键
        float horizontalInput = Input.GetAxis("Horizontal"); // A和D键

        if (verticalInput != 0 || horizontalInput != 0)
        {
            // 根据输入和速度移动物体
            transform.Translate(Vector3.forward * verticalInput * speed * Time.deltaTime);

            // 根据水平输入旋转物体
            transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);

            if (isKave)
            {
                if (camera3D != null)
                {
                    camera3D?.SetActive(false);
                }

                UpdateVRCamera(transform.position);
            }
            else
            {
                if (cameraKave)
                {
                    cameraKave?.SetActive(false);
                }

                Update3DCamera(transform.position);
            }
        }
    }

    public void UpdateVRCamera(Vector3 headLockPosition)
    {
        VRWorldManager.instance.centerViewPoint =
        new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
    }

    public void Update3DCamera(Vector3 headLockPosition)
    {
        camera1.transform.position = headLockPosition;
        camera2.transform.position = headLockPosition;
        camera3.transform.position = headLockPosition;
        camera4.transform.position = headLockPosition;
        // 84
        // camera5.transform.position = headLockPosition + new Vector3(0, 0, 77f);
        // camera6.transform.position = headLockPosition + new Vector3(0, 0, -77f);
        // 5
        camera5.transform.position = headLockPosition + new Vector3(0, 0, 2.5f);
        camera6.transform.position = headLockPosition + new Vector3(0, 0, -2.5f);
    }
}