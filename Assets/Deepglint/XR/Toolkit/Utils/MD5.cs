using System.Text;

namespace Deepglint.XR.Toolkit.Utils
{
    public static class MD5
    {
        public static string Hash(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为16进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}