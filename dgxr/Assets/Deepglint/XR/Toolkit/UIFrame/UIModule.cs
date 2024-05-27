using System;
using System.Reflection;
using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;

namespace Deepglint.XR.Toolkit.UIFrame
{
    public abstract class UIModule : BaseModule
    {
        public ScreenInfo Screen { get; private set; }

        protected new T CreateChild<T>() where T : UIModule
        {
            return Create<T>(Screen, gameObject);
        }

        protected T CreateChildOnScreen<T>(ScreenInfo screen, GameObject parent=null) where T : UIModule
        {
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }
            return Create<T>(screen, parent);
        }

        protected new T CreateChildOnSubGameObject<T>(string name) where T : UIModule
        {
            return Create<T>(Screen, gameObject.FindChildGameObject(name));
        }

        public void MoveToScreen(ScreenInfo screen, GameObject parent=null)
        {
            Screen = screen;
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }
            gameObject.transform.SetParent(parent.transform,false);
        }

        public void MoveTo(UIModule parent)
        {
            if (parent == null)
            {
                throw new NullReferenceException();
            }
            gameObject.transform.SetParent(parent.transform, false);
            Screen = parent.Screen;
        }

        public static T Create<T>(ScreenInfo screen, GameObject parent = null)
            where T : UIModule
        {
            if (parent == null)
            {
                parent = screen.ScreenCanvas;
            }

            var module = BaseModule.Create<T>(parent);
            module.Screen = screen;
            module.OnOpen();
            return module;
        }

        public static T Create<T>(TargetScreen target, GameObject parent = null)
            where T : UIModule
        {
            var screen = Global.Space[target];
            return Create<T>(screen, parent);
        }

        protected new object CreateChildByClass(Type type)
        {
            var method = GetType().GetMethod(nameof(CreateChild),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var generic = method?.MakeGenericMethod(type);
            return generic?.Invoke(this, null);
        }
    }
}
