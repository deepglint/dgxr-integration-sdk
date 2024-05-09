using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Runtime.Scripts.Tools.Constant;
using UnityEngine;
using Runtime.Scripts.Tools.Utils;

namespace Runtime.Scripts.Tools.UIFrame
{
    public abstract class BaseComponent
    {
        private readonly List<BaseComponent> _children = new();
        public readonly string Prefab;
        public TargetDisplay Display;
        public GameObject Component;
        public Transform Transform => Component.transform;
        public bool ActiveSelf => Component.activeSelf;


        protected BaseComponent()
        {
            Prefab = GetPrefabPath();
        }

        protected BaseComponent(string prefab)
        {
            Prefab = prefab;
        }

        public virtual void OnOpen()
        {
        }

        public virtual void OnClose()
        {
        }

        public void SetActive(bool active)
        {
            Component.SetActive(active);
        }

        protected T AddComponent<T>() where T : Component => Component.AddComponent<T>();

        public void Destroy()
        {
            foreach (var child in _children)
            {
                child.Destroy();
            }

            OnClose();
            UnityEngine.Object.Destroy(Component);
        }


        protected T CreateChild<T>() where T : BaseComponent
        {
            return CreateChildOnDisplay<T>(Display);
        }

        protected T CreateChildOnDisplay<T>(TargetDisplay display) where T : BaseComponent
        {
            var child = display == Display
                ? ComponentManager.OpenAndCreate<T>(display, Component)
                : ComponentManager.OpenAndCreate<T>(display);
            _children.Add(child);
            return child;
        }

        protected T CreateChildOnSubGameObject<T>(string name) where T : BaseComponent
        {
            var child = ComponentManager.OpenAndCreate<T>(
                Display, Component.FindChildGameObject(name));
            _children.Add(child);
            return child;
        }

        protected object CreateChildByClass(Type type)
        {
            var method = GetType().GetMethod(nameof(CreateChild),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var generic = method?.MakeGenericMethod(type);
            return generic?.Invoke(this, null);
        }

        public static T Create<T>(TargetDisplay display) where T : BaseComponent
        {
            return ComponentManager.OpenAndCreate<T>(display);
        }

        private string GetPrefabPath()
        {
            string ns = GetType().Namespace;
            if (ns == null)
            {
                throw new Exception("component script namespace not found");
            }

            string[] arr = ns.Split(".");
            if (arr[0] != "Scene")
            {
                throw new Exception("component script must under /Assets/Scripts/Scene folder");
            }

            arr[0] = "Prefabs";
            string dir = Path.Combine(arr);
            return Path.Combine(dir, GetType().Name);
        }
    }
}