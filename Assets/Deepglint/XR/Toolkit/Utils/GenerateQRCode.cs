using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.Common;

namespace Deepglint.XR.Toolkit.Utils
{
    public static class GenerateQRCode
    {
        /// <summary>
        /// 生成维码
        /// 能生成任意尺寸的正方形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static Texture2D GenerateQRImage(string content, int width, int height, Color color)
        {
            if (color== default)
            {
                color = Color.black; 
            }
            Texture2D texture = GenerateQRImageWithColor(content, width, height, color);

            return texture;
        }

        static Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color)
        {
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
           
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
           
            hints.Add(EncodeHintType.MARGIN, 0);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
            var bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);

            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            Texture2D texture = new Texture2D(w, h);
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    if (bitMatrix[x, y])
                    {
                        texture.SetPixel(y, x, color);
                    }
                    else
                    {
                        texture.SetPixel(y, x, Color.white);
                    }
                }
            }
            texture.Apply();
            return texture;
        }
    }
}