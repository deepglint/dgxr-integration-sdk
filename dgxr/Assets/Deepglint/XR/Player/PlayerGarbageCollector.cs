using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Deepglint.XR.Player
{
    public class PlayerGarbageCollector : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput component not found");
            }
        }

        private void OnDestroy()
        {
            if (_playerInput != null)
            {
                // _playerInput.user.UnpairDevices();
                PlayerManager.Instance.DestroyPlayer(_playerInput);
            }
        }
    }
}