using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Text;

namespace IW2S.Helpers
{
    public class WebHelper
    {
        /// <summary>
        /// 获取网页源代码
        /// </summary>
        /// <param name="url">网页链接</param>
        /// <returns></returns>
        public static string GetHtml(string url)
        {
            //Http请求
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //使用GET方法获取链接指向的网页
            req.Method = "GET";
            //设置用户代理，防止网站判定为用户代理
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
            //如果方法验证网页来源就加上这一句如果不验证那就可以不写了
            //req.Referer = "http://sufei.cnblogs.com";
            //添加Cookie，用来抓取需登陆可见的网页
            //CookieContainer objcok = new CookieContainer();
            //Cookie cook1 = new Cookie();
            ////cook1.Name = "CNZZDATA1257708097";
            ////cook1.Value = "1167517418-1479817270-%7C1488717892";
            ////objcok.Add(cook1);
            //objcok.Add(new Cookie("LOGGED_USER", "2Gsu8lGqckigfryi4J%2BxqQ%3D%3D%3AEPYACL1Ic4QgUm9bW2hOXg%3D%3D", "/", ".bcy.net"));
            //req.CookieContainer = objcok;
            //设置超时
            req.Timeout = 3000;
            //Http响应
            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception)
            {
                return null;
            }
            //使用utf-8去解码网页
            string htmlCharset = "utf-8";
            Encoding htmlEncoding = Encoding.GetEncoding(htmlCharset);
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream(), htmlEncoding);
            //读取返回的网页
            string respHtml = sr.ReadToEnd();
            resp.Close();
            return respHtml;
        }
    }
}