// using System;
// using System.Collections.Generic;
// using UnityEditor.Graphs;
// using UnityEngine;
// using UnityEngine.UI;
// using ZXing;
// using ZXing.Common;
//  
// public class ZXingQRCodeWrapper_GenerateQRCode : MonoBehaviour
// {
//     
//     public RawImage img1;
//     public RawImage img2;
//     public RawImage img3;
//
//     private int size=256;
//     private int n = 1;
//     // Use this for initialization
//     void Start()
//     {
//         //注意：这个宽高度大小256不要变(变的话，最好是倍数变化)。不然生成的信息可能不正确
//         //256有可能是这个ZXingNet插件指定大小的绘制像素点数值
//         img1.texture = GenerateQRImageWithColor("http://192.168.30.201:8080/auth?i=1&s=123&t=1721206182&sc=18,2000&a=1&m=s",size , size,Color.black);
//         img2.texture = GenerateQRImageWithColor("http://192.168.30.201:8080/",size , size,Color.black);
//         // img3.texture = GenerateQRImageWithColor("https://static-1253924368.cos.ap-beijing.myqcloud.com/share.html?name=大富翁&score=100&gameCount=101&play=飞翔且～～～～",size, size,Color.black);
//     }
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             n++;
//             img1.texture = GenerateQRImageWithColor("https://static-1253924368.cos.ap-beijing.myqcloud.com/share.html?name=大富翁&score=100&gameCount=101&play=飞翔且～～～～", size*n, size*n,Color.black);
//             img2.texture = GenerateQRImageWithColor("https://static-1253924368.cos.ap-beijing.myqcloud.com/share.html?name=大富翁&score=100&gameCount=101&play=飞翔且～～～～",size*n , size*n,Color.black);
//             
//             DateTime now = DateTime.UtcNow;
//             
//             GameInfo g = new GameInfo()
//             {
//                 Id = "ppx",
//                 AvatarId = 1,
//                 Model = "s",
//                 Score = new[] { 19 },
//                 Time = now
//             };
//             img3.texture = GenerateShareImage(g);
//         }
//     }
//     
//     //i=1&s=123&t=612412312415&sco=18,2000&a=1&m=s
//     
//     public struct GameInfo
//     {
//         public string Id;
//         // public string SpaceId;
//         public DateTime Time;
//         public int[] Score;
//         public int AvatarId;
//         public string Model;
//     }
//     
//     Texture2D GenerateShareImage(GameInfo game)
//     {
//         var url = "http://192.168.61.149:8080/auth";
//         string score = "";
//         for (int i = 0; i < game.Score.Length; i++)
//         {
//             score += game.Score[i].ToString();
//             if (i != game.Score.Length - 1)
//             {
//                 score += ",";
//             }
//         }
//         var timestamp = (long)(game.Time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
//         Debug.LogError(timestamp);
//         var shareUrl = $"{url}?i={game.Id}&t={timestamp.ToString()}&sc={score}&a={game.AvatarId}&m={game.Model}&s=96102";
//         Texture2D texture = GenerateQRImageWithColor(shareUrl, 256, 256, Color.black);
//         return texture;
//     }
//     
//     
//     /// <summary>
//     /// 生成维码
//     /// 经测试：能生成任意尺寸的正方形
//     /// </summary>
//     /// <param name="content"></param>
//     /// <param name="width"></param>
//     /// <param name="height"></param>
//     Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color) {
//         BitMatrix bitMatrix;
//         Texture2D texture = GenerateQRImageWithColor(content, width, height, color, out bitMatrix);
//  
//         return texture;
//     }
//  
//     /// <summary>
//     /// 生成2维码 方法二
//     /// 经测试：能生成任意尺寸的正方形
//     /// </summary>
//     /// <param name="content"></param>
//     /// <param name="width"></param>
//     /// <param name="height"></param>
//     Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color, out BitMatrix bitMatrix)
//     {
//         // 编码成color32
//         MultiFormatWriter writer = new MultiFormatWriter();
//         Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
//         //设置字符串转换格式，确保字符串信息保持正确
//         hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
//         // 设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
//         hints.Add(EncodeHintType.MARGIN, 1);
//         hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
//         //实例化字符串绘制二维码工具
//         bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);
//  
//         // 转成texture2d
//         int w = bitMatrix.Width;
//         int h = bitMatrix.Height;
//         print(string.Format("w={0},h={1}", w, h));
//         Texture2D texture = new Texture2D(w, h);
//         for (int x = 0; x < h; x++)
//         {
//             for (int y = 0; y < w; y++)
//             {
//                 if (bitMatrix[x, y])
//                 {
//                     texture.SetPixel(y, x, color);
//                 }
//                 else
//                 {
//                     texture.SetPixel(y, x, Color.white);
//                 }
//             }
//         }
//         texture.Apply();
//        
//         // 存储成文件
//         //byte[] bytes = texture.EncodeToPNG();
//         //string path = System.IO.Path.Combine(Application.dataPath, "qr.png");
//         //System.IO.File.WriteAllBytes(path, bytes);
//  
//         return texture;
//     }
//     
// }