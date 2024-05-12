using Deepglint.Tools.Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.Tools.Utils
{
    public static class UIUtils
    {
        public static void UpdateTextMeshPro(this GameObject gameObject, string message)
        {
            gameObject.UpdateTextMeshPro(message, null, null);
        }


        public static void UpdateTextMeshPro(this GameObject gameObject, string message, int? size, Color? color)
        {
            var textObj = gameObject.GetComponent<TextMeshProUGUI>();
            if (textObj != null)
            {
                textObj.text = message;
                if (size.HasValue) textObj.fontSize = size.Value;

                if (color.HasValue) textObj.color = color.Value;
            }
        }

        // TODO 优化,支持更新material - 扩展到 UnityExtentionMethod - UiUtils
        public static void UpdateRawImage(this GameObject gameObject, string textureName)
        {
            var imageObj = gameObject.GetComponent<RawImage>();
            if (imageObj != null) imageObj.texture = Resources.Load<Texture2D>("Image/" + textureName);
        }


        /// <summary>
        /// 通过名称查找子对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="childName">名称</param>
        /// <returns></returns>
        public static GameObject FindChildGameObject(this GameObject obj, string childName)
        {
            if (obj != null)
            {
                var children = obj.GetComponentsInChildren<Transform>(true);

                foreach (var child in children)
                    if (child.name == childName)
                        return child.gameObject;

                Debug.LogWarning($"{obj.name}里找不到名为{childName}的子对象");
                return null;
            }

            return null;
        }


        public static Transform GetScreenCanvas(TargetDisplay display)
        {
            var uiRoot = GameObject.Find("UIRoot");
            return uiRoot.transform.Find(display.ToString());
        }
    }
}