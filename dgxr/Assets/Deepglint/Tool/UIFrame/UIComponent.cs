using Deepglint.Tool.Utils;
using UnityEngine;

namespace Deepglint.Tool.UIFrame
{
    public abstract class UIComponent : BaseComponent
    {
        public TargetDisplay Display { get; private set; }

        protected new T CreateChild<T>() where T : UIComponent
        {
            return Create<T>(Display, GameObject);
        }

        protected T CreateChildOnDisplay<T>(TargetDisplay display) where T : UIComponent
        {
            return Create<T>(display, GameObject);
        }

        protected new T CreateChildOnSubGameObject<T>(string name) where T : UIComponent
        {
            return Create<T>(Display, GameObject.FindChildGameObject(name));
        }

        public void MoveToDisplay(TargetDisplay targetDisplay)
        {
            Display = targetDisplay;
            MoveToDisplay(GameObject, targetDisplay);
        }

        public static T Create<T>(TargetDisplay targetDisplay, GameObject parent = null)
            where T : UIComponent
        {
            var component = BaseComponent.Create<T>(parent);
            component.Display = targetDisplay;
            MoveToDisplay(component.GameObject, targetDisplay);
            component.OnOpen();
            return component;
        }

        private static void MoveToDisplay(GameObject gameObject, TargetDisplay targetDisplay)
        {
            var screen = UIUtils.GetScreenCanvas(targetDisplay);
            gameObject.transform.SetParent(screen, false);
            if (gameObject.GetComponent<RectTransform>())
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else if (gameObject.transform)
            {
                gameObject.transform.localPosition = Vector2.zero;
            }
        }
    }
}