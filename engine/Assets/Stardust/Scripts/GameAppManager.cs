using System;
using UnityEngine;

namespace Stardust.Scripts 
{
    public class GameAppManager: MSingleton<GameAppManager>
    {
        private bool _isEnterApp;
        public static Action OnEntered;
        public static Action OnLeft;

        public void Update()
        {
            if (Application.isFocused && !_isEnterApp)
            {
                _isEnterApp = true;
                OnEntered?.Invoke();
                MDebug.LogFlow("Application.isFocused: 进入主焦点" + Application.isFocused);
            }
            else if (!Application.isFocused && _isEnterApp)
            {
                _isEnterApp = false;
                OnLeft?.Invoke();
                MDebug.LogFlow("Application.isFocused: 离开主焦点" + Application.isFocused);
            }  
        }

        public void OpenUrl(string name)
        {
            string path = Application.streamingAssetsPath + name;
            Application.OpenURL(path);
        }
    
        public void CloseApp()
        {
            //按下ESC键则退出互动
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}