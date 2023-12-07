using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moat 
{
    public class GameAppManager: MSingleton<GameAppManager>
    {
        private bool isEnterApp;
        public static Action OnEntered;
        public static Action OnLeft;

        public void Update()
        {
            if (Application.isFocused && !isEnterApp)
            {
                isEnterApp = true;
                OnEntered?.Invoke();
            }
            else if (isEnterApp)
            {
                isEnterApp = false;
                OnLeft?.Invoke();
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