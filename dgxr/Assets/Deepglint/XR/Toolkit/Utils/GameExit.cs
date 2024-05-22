using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public class GameExit : MonoBehaviour
    {
        private KeyCode _keyCode = KeyCode.Escape;

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
        
#if !UNITY_EDITOR
        private void OnApplicationQuit()
        {
             System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
#endif

        public void SetKeyCode(KeyCode keyCode)
        {
            _keyCode = keyCode;
        }
    }
}