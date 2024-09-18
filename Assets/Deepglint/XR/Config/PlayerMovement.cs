using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f; // 移动速度

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // 获取水平输入
        float moveVertical = Input.GetAxis("Vertical"); // 获取垂直输入

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); // 计算移动方向
        transform.Translate(movement * speed * Time.deltaTime); // 移动GameObject
    }
}