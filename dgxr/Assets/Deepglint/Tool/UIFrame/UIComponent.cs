using Deepglint.Tool.Utils;
using Deepglint.XR;
using UnityEngine;

namespace Deepglint.Tool.UIFrame
{
    public abstract class UIComponent : BaseComponent
    {
        public ScreenInfo Screen { get; private set; }

        protected new T CreateChild<T>() where T : UIComponent
        {
            return Create<T>(Screen, gameObject);
        }

        protected T CreateChildOnScreen<T>(ScreenInfo screen) where T : UIComponent
        {
            return Create<T>(screen, gameObject);
        }

        protected new T CreateChildOnSubGameObject<T>(string name) where T : UIComponent
        {
            return Create<T>(Screen, gameObject.FindChildGameObject(name));
        }

        public void MoveToScreen(ScreenInfo screen)
        {
            Screen = screen;
            MoveToScreen(gameObject, screen);
        }

        public static T Create<T>(ScreenInfo screen, GameObject parent = null)
            where T : UIComponent
        {
            var component = BaseComponent.Create<T>(parent);
            component.Screen = screen;
            MoveToScreen(component.gameObject, screen);
            component.OnOpen();
            return component;
        }

        public static T Create<T>(TargetScreen target, GameObject parent = null)
            where T : UIComponent
        {
            var screen = Global.Space[target];
            return Create<T>(screen, parent);
        }

        private static void MoveToScreen(GameObject gameObject, ScreenInfo screen)
        {
            gameObject.transform.SetParent(screen.ScreenCanvas.transform, false);
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
