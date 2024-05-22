using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Utils
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

        
        
    }
}