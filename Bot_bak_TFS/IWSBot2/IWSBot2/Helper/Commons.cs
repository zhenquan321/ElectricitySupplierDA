using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IWSBot2.Helper
{
    public class Commons
    {
        public static void Log(string message)
        {
            Console.WriteLine(DateTime.Now + ": " + message);
        }

        /// <summary>
        ///  获取时间戳，从1970年1月1日到现在的秒数
        /// </summary>
        public static int GetTimestamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            var cstName = (int)timeSpan.TotalSeconds;
            return cstName;
        }

        /// <summary>
        /// 返回参数签名
        /// </summary>
        public static string GenerateWolongSign(Dictionary<string, object> param, string secret, out string url)
        {
            var paraStr = GetParaUrl(param, out url);
            var sign = Md5Encoding(secret + paraStr + secret);
            return sign;
        }

        /// <summary>
        /// 拼接参数
        /// </summary>
        public static string GetParaUrl(Dictionary<string, object> param, out string url)
        {
            var orderParas = param.OrderBy(x => x.Key);
            var sb = new StringBuilder();   // 带URL中的链接符
            var sb1 = new StringBuilder();  // 首尾相接，不带连接符
            var n = 0;

            foreach (var p in orderParas)
            {
                sb1.Append(UrlEncoding(p.Key)).Append(UrlEncoding(p.Value.ToString()));
                if (n == 0)
                {
                    sb.Append("?").Append(UrlEncoding(p.Key)).Append("=").Append(UrlEncoding(p.Value.ToString()));
                }
                else
                {
                    sb.Append("&").Append(UrlEncoding(p.Key)).Append("=").Append(UrlEncoding(p.Value.ToString()));
                }
                n++;
            }

            url = sb.ToString();
            return sb1.ToString();
        }

        /// <summary>
        /// URL 编码
        /// </summary>
        public static string UrlEncoding(string para)
        {
            string str = HttpUtility.UrlEncode(para);
            if (string.IsNullOrEmpty(str)) return null;

            if (para.Contains("/"))
            {
                str = str.Replace("%2f", "/");
            }

            return !para.Equals(str) ? str.ToUpper() : str;
        }

        /// <summary>
        /// 计算参数拼接后的MD5值
        /// </summary>
        public static string Md5Encoding(string para)
        {
            var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(para);
            var md5Bytes = md5.ComputeHash(bytes);
            // var s = Convert.ToBase64String(md5Bytes);
            var s = string.Empty;
            foreach (var b in md5Bytes)
            {
                s += b.ToString("X2");
            }

            return s.ToLower();
        }
    }
}
