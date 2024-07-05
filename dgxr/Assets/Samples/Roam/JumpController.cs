using System.Threading.Tasks;
using UnityEngine;

namespace Samples.Roam
{
    public class JumpController : MonoBehaviour
    {
        private enum JumpStatus 
        {
            Idle,
            Charging,
            Jumping
        }
        
        public float jumpForce = 10f;
        private float _currentChargeTime;
        private JumpStatus _jumpStatus = JumpStatus.Idle; 

        public Rigidbody rb;

        private void Update()
        {
            if (_jumpStatus != JumpStatus.Charging) return;
            _currentChargeTime = Mathf.Min(_currentChargeTime + Time.deltaTime * jumpForce, 20);
            Debug.LogFormat("jump height is {0} and jump force is {1}", _currentChargeTime + Time.deltaTime * jumpForce, jumpForce);
        }

        public void Charging()
        {
            if (_jumpStatus != JumpStatus.Idle) return;
            _jumpStatus = JumpStatus.Charging;
        }

        private void RestCharging()
        {
            _currentChargeTime = 0;
            _jumpStatus = JumpStatus.Idle;
        }

        // 跳跃方法
        public async void Jump()
        {
            if (_jumpStatus == JumpStatus.Jumping || rb == null) return;
            _jumpStatus = JumpStatus.Jumping;
            rb.AddForce(Vector3.up * _currentChargeTime, ForceMode.Impulse);
            await Task.Delay(1000);
            RestCharging();
        }
    }
}