using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.UIFrame
{
    /// <summary>
    /// BaseModule类是用于实现模块的抽象基类，模块包含了关联的 gameObject 和对应的控制逻辑，用于关联阈值件与控制逻辑，解决大量脚本挂载混乱的问题
    /// 实例化模块时会按规则从对应的预制件创建gameObject
    /// 模块以树形方式添加和管理，在父销毁时销毁所有的子模块及其 gameObject
    /// </summary>
    [PrefabInfo(PathRule.NamespaceHierarchy)]
    public abstract class BaseModule
    {
        private readonly List<BaseModule> _children = new();
        private readonly string _prefab;

        /// <summary>
        /// 模块包含的 gameObject
        /// </summary>
        public GameObject gameObject { get; private set; }

        /// <summary>
        /// 获取模块包含的 gameObject 的 transform 属性，gameObject.transform 的包装
        /// </summary>
        public Transform transform => gameObject.transform;

        /// <summary>
        /// 获取模块包含的 gameObject 的 activeSelf 属性，gameObject.activeSelf 的包装
        /// </summary>
        public bool activeSelf => gameObject != null && gameObject.activeSelf;

        public bool isDestroyed { get; private set; }


        protected BaseModule()
        {
            _prefab = GetPrefabPath();
        }

        /// <summary>
        /// 该方法在模块的gameObject资源创建后回调
        /// </summary>
        public virtual void OnOpen()
        {
        }

        /// <summary>
        /// 该方法在模块的Destroy方法调用时回调
        /// </summary>
        public virtual void OnClose()
        {
        }

        /// <summary>
        /// 设置模块包含的 gameObject 的Active状态，是对gameObject.SetActive的包装
        /// </summary>
        /// <param name="active"></param>
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
            isDestroyed = true;
        }


        /// <summary>
        /// 为模块创建子模块
        /// </summary>
        /// <typeparam name="T">子模块类型</typeparam>
        /// <returns>创建的子模块</returns>
        protected T CreateChild<T>() where T : BaseModule
        {
            var child = CreateWithAction<T>(gameObject, module => { _children.Add(module); });
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
            var child = CreateWithAction<T>(gameObject.FindChildGameObject(name),
                module => { _children.Add(module); });
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
        /// 通过创建模块
        /// </summary>
        /// <param name="parent">模块的gameObject要挂载的gameObject</param>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>创建的模块</returns>
        public static T Create<T>(GameObject parent = null)
            where T : BaseModule
        {
            return CreateWithAction<T>(parent, null);
        }


        /// <summary>
        /// 创建 Module，允许传入一个Action，action 会在 Module的 OnOpen 方法之前调用，用于初始化数据
        /// </summary>
        /// <param name="parent">模块的gameObject要挂载的gameObject</param>
        /// <param name="action">OnOpen之前执行的操作</param>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>创建的模块</returns>
        public static T CreateWithAction<T>(GameObject parent = null, Action<T> action = null)
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

            action?.Invoke(module);

            module.OnOpen();

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