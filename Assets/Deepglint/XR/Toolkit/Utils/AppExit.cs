using System;
using Deepglint.XR.Toolkit.Manager;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    public class AppExit : MonoBehaviour
    {
        private KeyCode _keyCode = KeyCode.Escape;
        public static Action OnAppExit;

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode))
            {
                Quit();
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            OnAppExit?.Invoke();
#if !UNITY_EDITOR
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetKeyCode(KeyCode keyCode)
        {
            _keyCode = keyCode;
        }
    }
}