using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AISSystem
{
    public class CryptHelper
    {
        public static string AnsiEncryption(string source, int key)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                int ic = key + ((int)c);
                sb.Append(ic.ToString("D3"));
            }
            return sb.ToString();
        }

        public static string AnsiDecryption(string source, int key)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < source.Length; i += 3)
            {
                string s = source.Substring(i, 3);
                s = s.TrimStart('0');
                int v = int.Parse(s);
                v = v - key;
                char c = (char)v;
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 使用AES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">加密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>加密结果，加密失败则返回源串</returns>
        public static string EncryptAES(string encryptString, string encryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(encryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] data = Encoding.UTF8.GetBytes(encryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return encryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        /// <summary>
        /// 使用AES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <param name="decryptKey">解密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>解密结果，解谜失败则返回源串</returns>
        public static string DecryptAES(string decryptString, string decryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(decryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

                byte[] data = Convert.FromBase64String(decryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            }
            catch
            {
                return decryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }
    }
}
