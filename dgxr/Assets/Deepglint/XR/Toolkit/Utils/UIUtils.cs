using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Utils
{
    
    public static class UIUtils
    {
        public static Color HexToRGBColor(string hexColor)
        {
            // 移除十六进制颜色值中的井号
            hexColor = hexColor.Replace("#", "");

            // 将十六进制颜色值转换为RGB颜色值
            int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // 构造Color对象并返回
            return new Color(r / 255f, g / 255f, b / 255f);
        }
        
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