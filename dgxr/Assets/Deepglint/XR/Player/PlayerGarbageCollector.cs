using UnityEngine;
using UnityEngine.InputSystem;

namespace Deepglint.XR.Player
{
    public class PlayerGarbageCollector : MonoBehaviour
    {
        private void OnDestroy()
        {
            PlayerManager.Instance.DestroyPlayer(gameObject.GetComponent<PlayerInput>());
        }
    }
}