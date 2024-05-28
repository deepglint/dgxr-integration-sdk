using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.UIFrame
{
    [PrefabInfo(PathRule.NamespaceHierarchy)]
    public abstract class BaseModule
    {
        private readonly List<BaseModule> _children = new();
        private readonly string _prefab;
        public GameObject gameObject { get; private set; }
        public Transform transform => gameObject.transform;
        public bool activeSelf => gameObject != null && gameObject.activeSelf;


        protected BaseModule()
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


        protected T CreateChild<T>() where T : BaseModule
        {
            var child = Create<T>(gameObject);
            _children.Add(child);
            child.OnOpen();
            return child;
        }

        protected void AddChild(BaseModule child)
        {
            _children.Add(child);
        }

        protected void SetParent(BaseModule parent)
        {
            parent.AddChild(this);
        }

        protected T CreateChildOnSubGameObject<T>(string name) where T : BaseModule
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
            where T : BaseModule
        {
            var module = Activator.CreateInstance<T>();

            var gameObject = module._prefab == null
                ? new GameObject(typeof(T).Name)
                : UnityEngine.Object.Instantiate(Resources.Load<GameObject>(module._prefab));

            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }

            module.gameObject = gameObject;

            return module;
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
            var ns = GetType().Namespace ?? throw new InvalidOperationException("mudule script namespace not found.");

            var arr = ns.Split(".");
            // QUESTION: 我们能把Scene这一层去了吗，从Scripts找就行了
            if (arr[0] != "Scene")
            {
                throw new Exception("module script must under /Assets/Scripts/Scene folder");
            }

            arr[0] = "Prefabs";
            var dir = Path.Combine(arr);
            return Path.Combine(dir, GetType().Name);
        }
    }
}
