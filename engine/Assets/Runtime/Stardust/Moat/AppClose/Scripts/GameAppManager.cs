using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGC
{
    public class GameAppManager
    {
        private static GameAppManager sInstance = null;

        public static GameAppManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new GameAppManager();
                }

                return sInstance;
            }
        }

        private bool isEnterApp;
        public static Action EnteredAppCallback;
        public static Action LeftAppCallback;

        public void Update()
        {
            if (Application.isFocused && !isEnterApp)
            {
                isEnterApp = true;
                EventManager.Send(GameEvent.OnOpen);
                EnteredAppCallback?.Invoke();
            }
            else if (isEnterApp)
            {
                isEnterApp = false;
                LeftAppCallback?.Invoke();
            } 
        }

        public void OpenUrl(string name)
        {
            string path = Application.streamingAssetsPath + "/App/" + name;
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