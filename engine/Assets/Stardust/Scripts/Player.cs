using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float jumpSpeed = 400f;
    private bool isOnGround = true;
    private Vector3 moveDistance;
    private Rigidbody rb;

    void Start()
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
            transform.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
    }
    
    void OnJump(InputValue value)
    {
        bool data = value.isPressed;
        if(data)
        {
            Debug.Log("jump, " + isOnGround);
            if (isOnGround)
            {
                //瞬移效果
                //transform.Translate(Vector3.up * Time.deltaTime * jumpSpeed);
 
                // 实现跳跃效果
                rb.AddForce(Vector3.up * jumpSpeed);
                // 此时物体不在地面上
                isOnGround = false;
                transform.GetComponent<MeshRenderer>().material.color = Color.red; 
            }
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 data = value.Get<Vector2>();
        Debug.Log("move: " + data);
        Vector3 moveDir = new Vector3(data.x, 0, data.y).normalized;
        moveDistance = moveDir * moveSpeed * Time.deltaTime;
 
        Vector3 targetDir = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(targetDir); 
    }
}