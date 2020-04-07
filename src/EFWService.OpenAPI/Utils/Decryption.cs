using System;
using System.Security.Cryptography;
using System.Text;

namespace EFWService.OpenAPI.Utils
{
    /// <summary>
    /// sha1加密
    /// </summary>
    internal class SHAEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            byte[] StrRes = Encoding.Default.GetBytes(source);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
    }
}
