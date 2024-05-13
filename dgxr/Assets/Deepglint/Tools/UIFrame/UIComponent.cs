using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Deepglint.Tools.UIFrame
{
    [PrefabInfo(PathRule.NamespaceHierarchy)]
    public abstract class UIComponent
    {
        private readonly List<UIComponent> _children = new();
        private readonly string _prefab;

        public TargetDisplay Display;

        public GameObject component { get; private set; }
        public Transform transform => component.transform;
        public bool activeSelf => component.activeSelf;


        protected UIComponent()
        {
            _prefab = GetPrefabPath();
        }

        protected UIComponent(string prefab)
        {
            _prefab = prefab;
        }

        public virtual void OnOpen()
        {
        }

        public virtual void OnClose()
        {
        }

        public void SetActive(bool active)
        {
            component.SetActive(active);
        }

        protected T AddComponent<T>() where T : Component => component.AddComponent<T>();

        public void Destroy()
        {
            foreach (var child in _children)
            {
                child.Destroy();
            }

            OnClose();
            UnityEngine.Object.Destroy(component);
        }


        protected T CreateChild<T>() where T : UIComponent
        {
            return CreateChildOnDisplay<T>(Display);
            ;
        }

        protected T CreateChildOnDisplay<T>(TargetDisplay display) where T : UIComponent
        {
            var child = display == Display ? Create<T>(display, component) : Create<T>(display);
            _children.Add(child);
            return child;
        }

        protected T CreateChildOnSubGameObject<T>(string name) where T : UIComponent
        {
            var child = Create<T>(Display, component.FindChildGameObject(name));
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
            string ns = GetType().Namespace ?? throw new InvalidOperationException("Component script namespace not found.");

            string[] arr = ns.Split(".");
            // QUESTION: 我们能把Scene这一层去了吗，从Scripts找就行了
            if (arr[0] != "Scene")
            {
                throw new Exception("component script must under /Assets/Scripts/Scene folder");
            }

            arr[0] = "Prefabs";
            string dir = Path.Combine(arr);
            return Path.Combine(dir, GetType().Name);
        }


        public static T Create<T>(TargetDisplay targetDisplay, GameObject parent = null)
            where T : UIComponent
        {
            var component = Activator.CreateInstance<T>();
            component.component = InitComponent(component._prefab, targetDisplay, parent);
            ;
            component.Display = targetDisplay;
            component.OnOpen();
            return component;
        }

        private static GameObject InitComponent(string path, TargetDisplay targetDisplay, GameObject parent = null)
        {
            var component = UnityEngine.Object.Instantiate(path == null
                ? new GameObject()
                : ResourcesManager.Instance.OnLoadAsset<GameObject>(path));

            if (parent != null)
            {
                component.transform.SetParent(parent.transform, false);
                return component;
            }

            var screen = ScreenCanvasManager.GetScreenCanvas(targetDisplay);
            component.transform.SetParent(screen, false);
            if (component.GetComponent<RectTransform>())
            {
                component.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else if (component.transform)
            {
                component.transform.localPosition = Vector2.zero;
            }

            return component;
        }
    }
}