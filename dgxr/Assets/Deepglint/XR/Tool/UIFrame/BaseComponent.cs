using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Deepglint.Tool.Utils;
using UnityEngine;

namespace Deepglint.Tool.UIFrame
{
    [PrefabInfo(PathRule.NamespaceHierarchy)]
    public abstract class BaseComponent
    {
        private readonly List<BaseComponent> _children = new();
        private readonly string _prefab;
        public GameObject gameObject { get; private set; }
        public Transform transform => gameObject.transform;
        public bool activeSelf => gameObject.activeSelf;


        protected BaseComponent()
        {
            _prefab = GetPrefabPath();
        }

        public virtual void OnOpen()
        {
        }

        public virtual void OnClose()
        {
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        protected T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        public void Destroy()
        {
            foreach (var child in _children)
            {
                child.Destroy();
            }

            OnClose();
            UnityEngine.Object.Destroy(gameObject);
        }


        protected T CreateChild<T>() where T : BaseComponent
        {
            var child = Create<T>(gameObject);
            _children.Add(child);
            child.OnOpen();
            return child;
        }
        

        protected T CreateChildOnSubGameObject<T>(string name) where T : BaseComponent
        {
            var child = Create<T>(gameObject.FindChildGameObject(name));
            _children.Add(child);
            child.OnOpen();
            return child;
        }

        protected object CreateChildByClass(Type type)
        {
            var method = GetType().GetMethod(nameof(CreateChild),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var generic = method?.MakeGenericMethod(type);
            return generic?.Invoke(this, null);
        }

        public static T Create<T>(GameObject parent = null)
            where T : BaseComponent
        {
            var component = Activator.CreateInstance<T>();
            component.gameObject = InitComponent(component._prefab, parent);
            component.OnOpen();
            return component;
        }

        private static GameObject InitComponent(string path, GameObject parent = null)
        {
            var component = UnityEngine.Object.Instantiate(path == null
                ? new GameObject()
                : Resources.Load<GameObject>(path));

            if (parent != null)
            {
                component.transform.SetParent(parent.transform, false);
            }
            return component;
        }
        
        
        private string GetPrefabPath()
        {
            var prefabInfo = GetType().GetCustomAttribute<PrefabInfoAttribute>(inherit: true);
            return prefabInfo.Rule switch
            {
                PathRule.Empty => null,
                PathRule.Specified => prefabInfo.Path,
                PathRule.NamespaceHierarchy => GetPathByNamespace(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetPathByNamespace()
        {
            var ns = GetType().Namespace ?? throw new InvalidOperationException("Component script namespace not found.");

            var arr = ns.Split(".");
            // QUESTION: 我们能把Scene这一层去了吗，从Scripts找就行了
            if (arr[0] != "Scene")
            {
                throw new Exception("component script must under /Assets/Scripts/Scene folder");
            }

            arr[0] = "Prefabs";
            var dir = Path.Combine(arr);
            return Path.Combine(dir, GetType().Name);
        }
    }
}