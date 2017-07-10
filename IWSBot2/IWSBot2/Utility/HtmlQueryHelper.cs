using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot.Utility
{
    public class HtmlQueryHelper
    {
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            Int32 dwFlags,
            IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;
        
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }
        static CookieContainer cc;
        public static string GetContent(string url, int timeout, ref Encoding encoding, out string Rurl)
        {
            cc = GetUriCookieContainer(new Uri(url));

            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "*/*";
            req.Method = "GET";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
            req.Proxy = null;
            req.KeepAlive = true;
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.CookieContainer = cc;
            req.ProtocolVersion = HttpVersion.Version10;
            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {

                    MemoryStream memStream = new MemoryStream(); ;

                    using (Stream stream = response.GetResponseStream())
                    {


                        byte[] buffer = new byte[1024];
                        int byteCount;
                        do
                        {
                            byteCount = stream.Read(buffer, 0, buffer.Length);
                            memStream.Write(buffer, 0, byteCount);
                        } while (byteCount > 0);
                    }


                    var charset = GetEncoding(response.CharacterSet);
                    if (charset == null)
                    {
                        responsestr = Encoding.UTF8.GetString(memStream.ToArray());

                        var charsetStr = "";
                        var charsetReg = new System.Text.RegularExpressions.Regex("<meta [^>]*charset=(.*?)(?=(;|\b|\"))");
                        var match = charsetReg.Match(responsestr);
                        if (match.Groups.Count > 1)
                        {
                            charsetStr = match.Groups[1].Value;
                            if (charsetStr.Trim().ToLower() == "gbk" || charsetStr.Trim().ToLower() == "gb2312")
                            {
                                responsestr = Encoding.GetEncoding("gb2312").GetString(memStream.ToArray());
                                encoding = Encoding.GetEncoding("gb2312");
                            }
                        }


                    }
                    else
                    {
                        responsestr = charset.GetString(memStream.ToArray());
                        encoding = Encoding.GetEncoding("utf-8");
                    }
                    Rurl = response.ResponseUri.AbsoluteUri;
                    response.Close();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

                if (req != null)
                {

                    req.Abort();
                }
            }
            return responsestr;

        }

        public static string GetRealUrl(string url, int timeout, ref Encoding encoding)
        {
            string realUrl = null;
            cc = GetUriCookieContainer(new Uri(url));
                        
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "*/*";
            req.Method = "GET";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
            req.Proxy = null;
            req.KeepAlive = true;
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.CookieContainer = cc;
            req.ProtocolVersion = HttpVersion.Version10;
            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    realUrl = response.ResponseUri.AbsoluteUri;
                    response.Close();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            finally
            {

                if (req != null)
                {

                    req.Abort();
                }
            }
            return realUrl;

        }

        public static Encoding GetEncoding(string CharacterSet)
        {
            switch (CharacterSet.ToLower())
            {
                case "gb2312": return Encoding.GetEncoding("gb2312");
                case "utf-8": return Encoding.UTF8;

                default: return null;
            }
        }
    }
}
