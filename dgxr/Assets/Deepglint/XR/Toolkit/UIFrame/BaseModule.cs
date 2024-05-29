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

        /// <summary>
        /// 为模块关联的 gameObject 添加 Component，该方法是 .gameObject.AddComponent 的包装方法
        /// </summary>
        /// <typeparam name="T"> Component 类型</typeparam>
        /// <returns>添加的Component</returns>
        protected T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

        /// <summary>
        /// 销毁模块，模块销毁时会递归销毁其所有子模块，然后调用模块上的OnClose方法，最后销毁关联的 gameObject
        /// </summary>
        public void Destroy()
        {
            foreach (var child in _children)
            {
                child.Destroy();
            }

            OnClose();
            UnityEngine.Object.Destroy(gameObject);
        }


        /// <summary>
        /// 为模块创建子模块
        /// </summary>
        /// <typeparam name="T">子模块类型</typeparam>
        /// <returns>创建的子模块</returns>
        protected T CreateChild<T>() where T : BaseModule
        {
            var child = Create<T>(gameObject);
            _children.Add(child);
            child.OnOpen();
            return child;
        }

        /// <summary>
        /// 为模块添加子模块
        /// </summary>
        /// <param name="child">子模块</param>
        protected void AddChild(BaseModule child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// 为模块设置父模块
        /// </summary>
        /// <param name="parent">父模块</param>
        protected void SetParent(BaseModule parent)
        {
            parent.AddChild(this);
        }

        /// <summary>
        /// 创建一个子模块，并挂载在模块的某个子 gameObject 下
        /// </summary>
        /// <param name="name">子 gameObject 名称</param>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>创建的子模块</returns>
        protected T CreateChildOnSubGameObject<T>(string name) where T : BaseModule
        {
            var child = Create<T>(gameObject.FindChildGameObject(name));
            _children.Add(child);
            child.OnOpen();
            return child;
        }

        /// <summary>
        /// 通过反射，使用模块类型创建子模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <returns>创建的子模块</returns>
        protected object CreateChildByClass(Type type)
        {
            var method = GetType().GetMethod(nameof(CreateChild),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var generic = method?.MakeGenericMethod(type);
            return generic?.Invoke(this, null);
        }

        /// <summary>
        /// 通过范型方法添加
        /// </summary>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
