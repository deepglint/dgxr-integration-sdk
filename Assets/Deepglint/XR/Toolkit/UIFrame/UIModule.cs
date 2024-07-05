using System;
using System.Reflection;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.UIFrame
{
    /// <summary>
    /// UIModule 基于 BaseModule，在 BaseModule 的基础上为UI模块增加了所在的屏幕
    /// UIModule 的 gameObject 会按父子层级添加到 XRManager 的 UIRoot 下对应的屏幕对象下
    /// 提供了与屏幕相关的工具方法，方便在多个屏幕以及虚拟3维空间及真实空间的转换与映射
    /// </summary>
    public abstract class UIModule : BaseModule
    {
        /// <summary>
        /// UIModule 所在的屏幕描述
        /// </summary>
        public ScreenInfo Screen { get; private set; }

        /// <summary>
        /// 创建子模块，子模块的屏幕与父模块一致
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>创建的子模块</returns>
        protected new T CreateChild<T>() where T : UIModule
        {
            return Create<T>(Screen, this, gameObject);
        }

        /// <summary>
        /// 在某个屏幕创建子模块 
        /// </summary>
        /// <param name="screen">屏幕描述</param>
        /// <param name="parent">挂载的父 gameObject， 如果为空则挂载到 UIRoot下 screen 对应的屏幕对象下 </param>
        /// <typeparam name="T">创建的子模块</typeparam>
        /// <returns></returns>
        protected T CreateChildOnScreen<T>(ScreenInfo screen, GameObject parent = null) where T : UIModule
        {
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }

            return Create<T>(screen, this, parent);
        }


        /// <summary>
        /// 创建一个子模块，并挂载在模块的某个子 gameObject 下
        /// </summary>
        /// <param name="name">子 gameObject 名称</param>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>创建的子模块</returns>
        protected new T CreateChildOnSubGameObject<T>(string name) where T : UIModule
        {
            return Create<T>(Screen, this, gameObject.FindChildGameObject(name));
        }

        /// <summary>
        /// 将模块移动到某个屏幕
        /// </summary>
        /// <param name="screen">目标屏幕</param>
        /// <param name="parent">移动后gameObject 要挂载的父gameObject，为空则不修改</param>
        public void MoveToScreen(ScreenInfo screen, GameObject parent = null)
        {
            Screen = screen;
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }

            gameObject.transform.SetParent(parent.transform, false);
        }

        /// <summary>
        /// 将模块移动到另一个模块下
        /// </summary>
        /// <param name="parent">新父模块</param>
        /// <exception cref="NullReferenceException"></exception>
        public void MoveTo(UIModule parent)
        {
            if (parent == null)
            {
                throw new NullReferenceException();
            }

            gameObject.transform.SetParent(parent.transform, false);
            Screen = parent.Screen;
        }

        /// <summary>
        /// 通过ScreenInfo描述创建 UIModule
        /// </summary>
        /// <param name="screen">目标屏幕</param>
        /// <param name="moduleParent">父模块</param>
        /// <param name="parent">挂载的父gameObject</param>
        /// <typeparam name="T">返回的模块</typeparam>
        /// <returns></returns>
        public static T Create<T>(ScreenInfo screen, UIModule moduleParent = null, GameObject parent = null)
            where T : UIModule
        {
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }

            var module = CreateWithAction<T>(parent, module =>
            {
                if (moduleParent != null)
                {
                    module.SetParent(moduleParent);
                }

                module.Screen = screen;
            });

            return module;
        }

        /// <summary>
        /// 通过TargetScreen 枚举描述创建 UIModule
        /// </summary>
        /// <param name="target">目标屏幕</param>
        /// <param name="moduleParent">父模块</param>
        /// <param name="parent">挂载的父gameObject</param>
        /// <typeparam name="T">返回的模块</typeparam>
        public static T Create<T>(TargetScreen target, UIModule moduleParent = null, GameObject parent = null)
            where T : UIModule
        {
            var screen = DGXR.Space[target];
            return Create<T>(screen, moduleParent, parent);
        }

        /// <summary>
        /// 通过类型反射创建Module
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <returns>创建的模块</returns>
        protected new object CreateChildByClass(Type type)
        {
            var method = GetType().GetMethod(nameof(CreateChild),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var generic = method?.MakeGenericMethod(type);
            return generic?.Invoke(this, null);
        }
    }
}