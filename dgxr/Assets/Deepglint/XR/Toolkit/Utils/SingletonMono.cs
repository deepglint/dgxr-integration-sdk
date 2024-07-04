using UnityEngine;

namespace Deepglint.XR.Toolkit.Utils
{
    /// <summary>
    /// An automatic singleton pattern base class that inherits MonoBehaviour
    /// Action: Classes that inherit from this class inherit MonoBehaviour and have their own singleton pattern.
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Constructor privatization prevents external new objects.
        protected SingletonMono()
        {
        }

        // Records whether the singleton exists. Used to prevent errors when accessing singleton objects in the OnDestroy method.
        public static bool IsExisted { get; private set; } = false;

        // Provide a property for external access, which is equivalent to a singleton object.
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                GameObject go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>(); 
                IsExisted = true;

                return _instance;
            }
        }

        public virtual void OnDestroy()
        {
            IsExisted = false;
        }
    }
}