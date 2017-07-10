using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using System.Web;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GoogleS
{
    public class TaobaoWebHelper
    {

        static string[] arayList ={"Mozilla/5.0 (X11; Linux i686) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1200.0 Iron/21.0.1200.0 Safari/537.1",
                            "Mozilla/5.0 (X11; Linux i686; U; pl; rv:1.8.1) Gecko/20061208 Firefox/2.0.0",
                            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1200.0 Iron/21.0.1200.0 Safari/537.1",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)",
                            "Mozilla/5.0 (X11; Linux i686 on x86_64; rv:6.0.2) Gecko/20100101 Firefox/6.0.2 Iceweasel/6.0.2",
                            "Mozilla/5.0 (X11; Linux i686; U; en; rv:1.8.1) Gecko/20061208 Firefox/2.0.0 Opera 9.51",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.0; en) AppleWebKit/528.16 (KHTML, like Gecko) Version/4.0 Safari/528.16",
                            "Mozilla/5.0 (X11; Linux i686) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36",
                            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1284.0 Safari/537.13",
                            "Mozilla/5.0 (X11; U; Linux amd64) Iron/21.0.1200.0 Chrome/21.0.1200.0 Safari/537.1",
                            "Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/418.9.1 (KHTML, like Gecko) Safari/419.3",
                            "Mozilla/5.0 (Macintosh; U; PPC Mac OS X; ca-es) AppleWebKit/522.11.1 (KHTML, like Gecko) Version/3.0.3 Safari/522.12.1",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Iron/6.0.475.1 Chrome/6.0.475.1 Safari/89895776.534",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Iron/6.0.475.1 Chrome/6.0.475.1 Safari/94403424.534",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Iron/6.0.475.1 Chrome/6.0.475.1 Safari/95066112.534",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Iron/6.0.475.1 Chrome/6.0.475.1 Safari/9724672.534",
                            "Mozilla/5.0 (Windows NT 5.1; U; zh-cn; rv:1.9.1.6) Gecko/20091201 Firefox/3.5.6 Opera 10.70",
                            "Opera/9.80 (Windows NT 6.1; U; sv) Presto/2.7.62 Version/11.01",
                            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.2.15 Version/10.00",
                            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.5.22 Version/10.50",
                            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.6.30 Version/10.61",
                            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.6.37 Version/11.00",
                            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.7.62 Version/11.01",
                            "Opera/9.80 (Windows NT 6.1; U; zh-tw) Presto/2.5.22 Version/10.50",
                            "Opera/9.80 (Windows NT 6.1; U; zh-tw) Presto/2.7.62 Version/11.01",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.596.0 Safari/534.13",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.0 Safari/534.13",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.19 Safari/534.13",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) Chrome/9.0.597.98 Safari/534.13 ChromePlus/1.6.0.0",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.13 (KHTML, like Gecko) RockMelt/0.9.48.51 Chrome/9.0.597.107 Safari/534.13",
                            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.14 (KHTML, like Gecko) Chrome/10.0.601.0 Safari/534.14"};
        List<string> AList = new List<string>();
        static string Uagent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";

        public static string GetSnapshotHtml(string url)
        {
            if (url.IsContains2("javascr", "&bcoffset=4"))
                return null;

            ProxyLib.WebHelperNoCookieProxy web = new ProxyLib.WebHelperNoCookieProxy();
            HTML.WebHelper web2 = new HTML.WebHelper();
            var html = web2.GetHtml(url, null);
            var total2 = html.SubBefore("personalbar").SubLastStringAfter("totalCount");
            int? total = html.SubBefore("personalbar").SubLastStringAfter("totalCount").ExInt();
            while (!total.HasValue)
            {
                web.ChangeIp();
                html = web.GetHtml(url, null, "utf-8");
                var html2 = html.SubBefore("personalbar").SubLastStringAfter("totalCount");
                total = html2.ExInt();
                if (html2.IsContain("万"))
                    total = total * 10000;
            }
            return html;
        }

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
            uri = new Uri("http://weixin.sogou.com");
            CookieContainer cookies = null;
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder();
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                cookieData = new StringBuilder();
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
                //  string cookiesStr = cookies.SetCookies(uri);

            }
            return cookies;
        }

        private static string cookies = AppSettingHelper.GetAppSetting("cookies");

        public static string GetContent(string url, int timeout, CookieContainer cc, ref Encoding encoding, out string Rurl, string cookie, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)
        {

            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Accept = "*/*";
            //req.Method = "GET";
            //req.UserAgent = Uagent;//"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            //req.Proxy = null;
            //req.KeepAlive = true;
            //req.Timeout = timeout;
            //req.ReadWriteTimeout = timeout;
            //req.CookieContainer = new CookieContainer();
            //req.CookieContainer.Add(cookiesColl);
            //// req.Headers.Add(HttpRequestHeader.Cookie, ""); 
            ////    req.Headers.Add(HttpRequestHeader.Cookie, "CXID=7E7921DF0B61F4BF096305E400A08956; SUID=F682E9674E6C860A567CE02C0003617F; SUV=00212B1C67E982F6567CE02ECE4C6407; weixinIndexVisited=1; ssuid=1634306985; ABTEST=0|1456304254|v1; ld=Dqr6slllll2QZIgmlllllVbK0IllllllBMujdkllll9llllljvoll5@@@@@@@@@@; ad=4lllllllll2QKnl7lllllVbr6DYlllllBMujdklllxllllll9Oxlw@@@@@@@@@@@; SNUID=116A018FE8E2C9A4089226FBE87D79E5; sct=51; IPLOC=CN88; LSTMV=1009%2C75; LCLKINT=107158");
            //req.ProtocolVersion = HttpVersion.Version11;
            ////  req.Host = "weixin.sogou.com";

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    
                    //cookiesCollection = null; 
                    //req.CookieContainer.GetCookies(req.RequestUri);
                  //  response.Cookies = cc.GetCookies(req.RequestUri);
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

                            else
                            {
                                responsestr = Encoding.GetEncoding("Big5").GetString(memStream.ToArray()); //charset.GetString(memStream.ToArray());
                                encoding = Encoding.GetEncoding("utf-8");


                           //     responsestr = Regex.Unescape(responsestr);


                                responsestr = HttpUtility.UrlDecode(responsestr);


                            }




                        }




                    }
                    else
                    {
                        responsestr = charset.GetString(memStream.ToArray());
                        encoding = Encoding.GetEncoding("utf-8");
                    }

                    Rurl = response.ResponseUri.AbsoluteUri;
                    string mycc = response.Headers["set-cookie"];
                    string gfgf = req.Headers["Cookie"];
                    mycc = gfgf;
                    cookiesCollection = cc.GetCookies(req.RequestUri); ;
                    response.Close();

                }
            }
            catch (Exception ex)
            {
                // throw;
                Rurl = url;
                cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri); ; // req.CookieContainer.GetCookies(req.RequestUri);
                Console.WriteLine(DateTime.Now + " " + ex.Message);
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

        public static string GetContentByIP(string url, int timeout, CookieContainer cc, string ip, int port, ref Encoding encoding, out string Rurl, string cookie, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)
        {

            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "*/*";
            req.Method = "GET";
            req.UserAgent = Uagent;//"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            req.Proxy = new WebProxy(ip, port);
            req.KeepAlive = true;
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(cookiesColl); //req.Headers.Add(HttpRequestHeader.Cookie, ""); 

            req.ProtocolVersion = HttpVersion.Version11;
            //  req.Host = "weixin.sogou.com";

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
                    response.Cookies = cc.GetCookies(req.RequestUri);
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
                    string mycc = response.Headers["set-cookie"];
                    string gfgf = req.Headers["Cookie"];
                    mycc = gfgf;
                    response.Close();

                }
            }
            catch (Exception ex)
            {
                // throw;
                cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
                Rurl = null;
                Console.WriteLine(DateTime.Now + ex.Message);
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

        public static Encoding GetEncoding(string CharacterSet)
        {
            switch (CharacterSet.ToLower())
            {
                case "gb2312": return Encoding.GetEncoding("gb2312");
                case "utf-8": return Encoding.UTF8;

                default: return null;
            }
        }


        public static string GetContentByIndex(string url, int timeout, CookieContainer cc, ref Encoding encoding, out string Rurl, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)
        {




            string strBuff = "";//定义文本字符串，用来保存下载的html  
            int byteRead = 0;

            int n = new Random().Next(0, arayList.Length - 1);
            Uagent = arayList[n].ToString();
            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            //req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //req.Method = "GET";
            //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36"; //Uagent; //"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            //req.Proxy = null;
            //req.KeepAlive = true;
            //req.Timeout = timeout;
            //req.ReadWriteTimeout = timeout;
            //req.CookieContainer = new CookieContainer();
            //req.CookieContainer.Add(cookiesColl); //req.Headers.Add(HttpRequestHeader.Cookie, ""); 
            //// req.Headers.Add(HttpRequestHeader.Cookie, cookies);
            //req.ProtocolVersion = HttpVersion.Version11;
            ////  req.Host = "weixin.sogou.com";
            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                 //   cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
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
                    response.Cookies = cc.GetCookies(req.RequestUri);
                    StringBuilder cookieData = new StringBuilder();
                    Uri uurl = req.RequestUri;

                    string mycc = response.Headers["set-cookie"];
                    string gfgf = req.Headers["Cookie"];
                    mycc += ";" + gfgf;
                    response.Close();
                    cookiesCollection = cc.GetCookies(req.RequestUri); //req.CookieContainer.GetCookies(req.RequestUri);
                }
            }
            catch (Exception ex)
            {
                // throw;
                Rurl = url;
                cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
                Console.WriteLine(DateTime.Now + " " + ex.Message);
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






    }
}
