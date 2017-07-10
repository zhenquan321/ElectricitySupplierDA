using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace IWSBot2.Helper
{
    public class Commons
    {
        /// <summary>
        /// 输出时间日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString() + "  :  " + msg);
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

        /// <summary>
        /// 拆分为字符串Id列表
        /// </summary>
        /// <param name="idStr">源字符串</param>
        /// <returns></returns>
        public static List<string> GetIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").ToList();
        }

        /// <summary>
        /// 拆分为ObjectId列表
        /// </summary>
        /// <param name="idStr">源字符串</param>
        /// <returns></returns>
        public static List<ObjectId> GetObjIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
        }

        /// <summary>
        /// 去除文本HTML代码中的HTML标签
        /// </summary>
        /// <param name="html">文本HTML代码</param>
        /// <returns></returns>
        public static string RemoveTextTag(string html)
        {
            //转换换行标签
            Regex reg = new Regex("</p>|<br>|</br>");
            html = reg.Replace(html, System.Environment.NewLine);
            //html = html.Replace("</p>", System.Environment.NewLine);

            //转换带属性的br标签，如：
            //<br style="max-width: 100%; color: rgb(115, 250, 121); font-size: 20px; line-height: 35.5556px; box-sizing: border-box !important; word-wrap: break-word !important;"  />
            int pos = -3;
            while (true)
            {
                pos = html.IndexOf("<br", pos + 3);
                if (pos != -1)
                {
                    html = html.Insert(pos, System.Environment.NewLine);
                }
                else
                {
                    break;
                }
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.DocumentNode;
            string content = node.InnerText;
            content = System.Web.HttpUtility.HtmlDecode(content);       //转换文本内的HTML字符
            //去除多余空行
            content = RemoveSpaceLine(content);

            if (content.Length > 2)
                content = content.Substring(0, content.Length - 2);             //去除末尾换行
            return content;
        }

        /// <summary>
        /// 去除多余换行和空白行
        /// </summary>
        /// <param name="text">源文本</param>
        /// <returns></returns>
        public static string RemoveSpaceLine(string text)
        {
            text = text.Replace(" ", "");       //该处为普通空格
            text = text.Replace(" ", "");       //该处为将形如&nbsp;等字符转义后的空格
            //按段落拆分
            text = text.Replace("\r", "");
            var listStr = text.Split('\n').ToList();
            StringBuilder bu = new StringBuilder();
            foreach (var str in listStr)
            {
                if (!string.IsNullOrEmpty(str))
                    bu.Append(str + System.Environment.NewLine);
            }
            return bu.ToString();
        }
    }
}
