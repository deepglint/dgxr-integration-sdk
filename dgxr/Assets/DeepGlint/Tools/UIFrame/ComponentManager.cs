using System;
using DeepGlint.Tools.Constant;
using DeepGlint.Tools.Utils;
using UnityEngine;

namespace DeepGlint.Tools.UIFrame
{
    public static class ComponentManager
    {
        public static T OpenAndCreate<T>(TargetDisplay targetDisplay, GameObject parent = null)
            where T : BaseComponent
        {
            T component = Activator.CreateInstance<T>();
            component.Component = InitComponent(component.Prefab, targetDisplay, parent);
            component.Display = targetDisplay;
            component.OnOpen();
            return component;
        }

        private static GameObject InitComponent(string path, TargetDisplay targetDisplay, GameObject parent = null)
        {
            var component =
                UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path));

            if (parent != null)
            {
                component.transform.SetParent(parent.transform, false);
                return component;
            }

            var screen = UIUtils.GetScreenCanvas(targetDisplay);
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