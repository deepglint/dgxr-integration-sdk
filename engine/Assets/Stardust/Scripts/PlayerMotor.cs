using Deepglint.XR.Inputs.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float jumpSpeed = 400f;
    private bool isOnGround = true;
    private Vector3 moveDistance;
    private Rigidbody rb;
    private PlayerInput pi;
    private int freeSwimCount = 0;
    private int butterflySwimCount = 0;
    private int highKneeRunSwimCount = 0;
    private int deepSquatCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDistance);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isOnGround = true;
        }
    }
    
    public void MoveControl(InputAction.CallbackContext value)
    {
        Vector2 data = value.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(data.x * 2, 0, data.y * 2).normalized;
        moveDistance = moveDir * moveSpeed * Time.deltaTime;
 
        Vector3 targetDir = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(targetDir); 
    }
    
    public void PoseControl(InputAction.CallbackContext value)
    {
        HumanPoseState humanPose = value.ReadValue<HumanPoseState>();
        transform.position = new Vector3(humanPose.position.x, transform.position.y, humanPose.position.z);
        transform.rotation = humanPose.rotation;
    }
    
    public void JumpControl(InputAction.CallbackContext value)
    {
        //Debug.Log("on callback");
        if(value.performed)
        {
            //Debug.Log("raise right hand, " + isOnGround);
            if (isOnGround)
            {
                // 实现跳跃效果
                if (rb == null)
                {
                    Debug.Log("rb is null");
                }
                rb.AddForce(Vector3.up * jumpSpeed);
                // 此时物体不在地面上
                isOnGround = false;
            }
        }
    }
    
    public void FreeSwimControl(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            freeSwimCount++;
            Debug.LogFormat("free-swim count: {0}", freeSwimCount);
        }
    }
    
    public void ButterflySwimControl(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            freeSwimCount++;
            Debug.LogFormat("butterfly-swim count: {0}", butterflySwimCount);
        }
    }
    
    public void HighKneeRunControl(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            freeSwimCount++;
            Debug.LogFormat("high-knee-run count: {0}", highKneeRunSwimCount);
        }
    }
    
    public void DeepSquatControl(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            freeSwimCount++;
            Debug.LogFormat("deep-squat count: {0}", deepSquatCount);
        }
    }
    
    public void RaiseRightHandControl(InputAction.CallbackContext value)
    {
        //Debug.Log("on callback");
        if(value.performed)
        {
            //Debug.Log("raise right hand, " + isOnGround);
            if (isOnGround)
            {
                // 实现跳跃效果
                if (rb == null)
                {
                    Debug.Log("rb is null");
                }
                rb.AddForce(Vector3.up * jumpSpeed);
                // 此时物体不在地面上
                isOnGround = false;
                //transform.GetComponent<MeshRenderer>().material.color = Color.red; 
            }
        }
    }
    
    void OnJump(InputValue value)
    {
        return;
        bool data = value.isPressed;
        if(data)
        {
            Debug.Log("jump, " + isOnGround);
            if (isOnGround)
            {
                //瞬移效果
                //transform.Translate(Vector3.up * Time.deltaTime * jumpSpeed);
 
                // 实现跳跃效果
                if (rb == null)
                {
                    Debug.Log("rb is null");
                }
                rb.AddForce(Vector3.up * jumpSpeed);
                // 此时物体不在地面上
                isOnGround = false;
                //transform.GetComponent<MeshRenderer>().material.color = Color.red; 
            }
        }
    }
}