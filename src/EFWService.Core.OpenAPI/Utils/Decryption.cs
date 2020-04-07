using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;

namespace EFWService.Core.OpenAPI.Utils
{
    /// <summary>
    /// sha1加密
    /// </summary>
    public class SHAEncryption
    {
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

    public class MD5Encrypt
    {
        /// <summary>
        /// 获取字符数组的MD5加密值
        /// </summary>
        /// <param name="sortedArray">待计算MD5哈希值的输入字符数组</param>
        /// <param name="charset"></param>
        /// <returns>输入字符数组的MD5哈希值</returns>
        public static string GetMD5ByArray(string[] sortedArray)
        {
            string tomd5 = string.Join("&", sortedArray);
            return GetMD5(tomd5);
        }

        public static string GetMD5(string input)
        {
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

    }

}
