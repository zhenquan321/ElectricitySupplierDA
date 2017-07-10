//using HTML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SogouSearch
{

    public class WebHelperNoCookieProxy
    {
        const string UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; Tablet PC 2.0; InfoPath.2; .NET CLR 3.5.21022; .NET CLR 3.0.04506)";
        public static string IPHONE_USERAGENT = "Mozilla/5.0 (iPhone; U; " + "CPU iPhone OS 3_0 like Mac OS X; en-us) AppleWebKit/528.18 " + "(KHTML, like Gecko) Version/4.0 Mobile/7A341 Safari/528.16";
        public static string GoogleNexus = "Mozilla/5.0 (Linux; U; Android 2.2; en-us; Nexus One Build/FRF91) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public static string HTC = "Mozilla/5.0 (Linux; U; Android 2.1-update1; de-de; HTC Desire 1.19.161.5 Build/ERE27) AppleWebKit/530.17 (KHTML, like Gecko) Version/4.0 Mobile Safari/530.17";
        public static string Samsung = "Mozilla/5.0 (Linux; U; Android 2.2; en-gb; GT-P1000 Build/FROYO) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        const string Accept = @"application/json,image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-ms-application, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
        public string ContentType = "application/x-www-form-urlencoded";
        //public string ContentType = "application/x-javascript;charset=GBK";


        //public XDocument GetXDoc(string url, string post_data, string char_set, int max_try = 3, int timeoutSeconds = 10)
        //{
        //    int i = 0;
        //    while (i < max_try)
        //    {
        //        var xdoc = GetXDoc(url, post_data, char_set, timeoutSeconds * 1000);
        //        if (xdoc != null)
        //            return xdoc;
                
        //        i++;
        //    }

            
        //    return null;
        //}

        //public XDocument GetXDoc(string url, string post_data, string charset, int timeout = 10000)
        //{
        //    try
        //    {
        //        string html = GetHtml(url, post_data, charset, timeout);
        //        if (string.IsNullOrEmpty(html))
        //            return null;

        //        HtmlDocument hdoc = new HtmlDocument(html);
        //        XDocument xdoc = hdoc.GetXDocument();
        //        return xdoc;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        
        //public string GetHtml(string url, string post_data, string char_set, int max_try = 3, int timeoutSeconds = 10)
        //{
        //    if (my_ip == null)
        //        ChangeIp();
        //    return GetHtml(url, post_data, char_set, ref my_ip, max_try, timeoutSeconds);
        //}

        public string GetFastHtmlWithProxyIp(string url, string char_set = "utf-8", int max_try = 5, int timeoutSeconds = 2, int max_ip_get_page = 10)
        {
            
            int curr = 0;
            while (curr < max_try)
            {
                string html = GetFastHtml(url, char_set, timeoutSeconds * 1000);
                if (!string.IsNullOrEmpty(html))
                    return html;
                curr++;
                
            }
            return null;
        }

        public string GetFastHtmlWithProxyIpAndARE(string url, string char_set = "utf-8", int max_try = 5, int timeoutSeconds = 2, int max_ip_get_page = 10)
        {
            //curr_ip_get_pages++;
            //if (my_ip == null || curr_ip_get_pages > max_ip_get_page)
            //    ChangeIp();
            int curr = 0;
            while (curr < max_try)
            {
                string html = null;
                AutoResetEvent are = new AutoResetEvent(false);
                Task.Factory.StartNew(() =>
                {
                    html = GetFastHtml(url, char_set, timeoutSeconds * 1000);
                    if(string.IsNullOrEmpty(html))
                    {
                        html = GetFastHtml(url, "gbk", timeoutSeconds * 1000);
                    }
                    if (!string.IsNullOrEmpty(html))
                    {
                        if (html.Contains("charset=\"GBK") || html.Contains("charset=GBK") || html.Contains("charset=\"gbk") || html.Contains("charset=gbk"))
                        {
                            html = GetFastHtml(url, "gbk", timeoutSeconds * 1000);
                        }
                        else if (html.Contains("charset=gb2312"))
                        {
                            html = GetFastHtml(url, "gb2312", timeoutSeconds * 1000);
                        }
                    }
                    are.Set();
                });
                WaitHandle.WaitAll(new WaitHandle[] { are }, (timeoutSeconds + 1) * 1000);
                if (!string.IsNullOrEmpty(html))
                    return html;
                curr++;
                //ChangeIp();
            }
            return null;
        }

        public Tuple<string, string> GetFastHtmlUrlWithProxyIpAndARE(string url, string char_set = "utf-8", int max_try = 5, int timeoutSeconds = 8, int max_ip_get_page = 10)
        {
            //curr_ip_get_pages++;
            //if (my_ip == null || curr_ip_get_pages > max_ip_get_page)
            //    ChangeIp();
            int curr = 0;
            
            while (curr < max_try)
            {
                Tuple<string, string> tuplhtml = null;
                AutoResetEvent are = new AutoResetEvent(false);
                Task.Factory.StartNew(() =>
                {
                    
                    tuplhtml = GetFastHtmlUrl(url, char_set, timeoutSeconds * 1000);
                    if (tuplhtml == null || string.IsNullOrEmpty(tuplhtml.Item2))
                    {
                        tuplhtml = GetFastHtmlUrl(url, "gbk", timeoutSeconds * 1000);
                    }
                    else if (tuplhtml.Item2.Contains("charset=\"GBK") || tuplhtml.Item2.Contains("charset=GBK") || tuplhtml.Item2.Contains("charset=\"gbk") || tuplhtml.Item2.Contains("charset=gbk"))
                    {
                        tuplhtml = GetFastHtmlUrl(url, "gbk", timeoutSeconds * 1000);
                    }
                    else if (tuplhtml.Item2.Contains("charset=gb2312"))
                    {
                        tuplhtml = GetFastHtmlUrl(url, "gb2312", timeoutSeconds * 1000);
                    }
                    are.Set();
                });
                WaitHandle.WaitAll(new WaitHandle[] { are }, (timeoutSeconds + 1) * 1000);
                if (tuplhtml != null && !string.IsNullOrEmpty(tuplhtml.Item2))
                    return tuplhtml;
                curr++;
                //ChangeIp();
            }
            return null;
        }

        

        public string GetHtml(string url, string post_data, string char_set, int max_try = 3, int timeoutSeconds = 10)
        {
            
            int i = 0;
            while (i < max_try)
            {
                var html = GetHtml(url, post_data, char_set, timeoutSeconds * 1000);
                if (html != null)
                    return html;
                
                i++;
            }

            return null;
        }

        public string GetHtml(string url, string post_data, string charset, string proxyIP, int proxyPort, int timeout = 10000)
        {
            try
            {
                string html = null;
                AutoResetEvent are = new AutoResetEvent(false);
                Task.Factory.StartNew(() =>
                {
                    Uri uri = new Uri(url);
                    // 伪造消息
                    var request = GetHttpRequest(url, post_data, null,  timeout);// (HttpWebRequest)WebRequest.Create(url);

                    html = GetHtml(request, charset);
                    are.Set();
                });
                WaitHandle.WaitAll(new WaitHandle[] { are }, timeout);
                return html;
            }
            catch
            {
                return null;
            }

        }

        public static string GetHtml(HttpWebRequest request, string charset)
        {
            bool isGzip = false, retry = false;
            string responseCharset = "utf-8";
            Stream stream = GetStream(request, charset, ref retry, ref responseCharset, ref isGzip);
            if (string.IsNullOrEmpty(charset))
                charset = responseCharset;
            return GetHtml(stream, charset, isGzip);
        }

        public static string GetHtml(Stream stream, string charset, bool isGzip)
        {
            try
            {
                if (stream == null)
                    return null;
                Stream gz = isGzip ? new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress) : stream;
                Encoding currentEncoding = !string.IsNullOrEmpty(charset) ? Encoding.GetEncoding(charset)
                    : Encoding.GetEncoding("utf-8");

                StreamReader reader = new StreamReader(gz, currentEncoding);
                stream.Flush();
                string content = reader.ReadToEnd();
                return content;
            }
            catch
            {
                return null;
            }
        }

        public static Stream GetStream(HttpWebRequest request, string charset, ref bool shouldRetry, ref string charsetResponse, ref bool isGzip)
        {
            HttpWebResponse response = null;
            try
            {
                // Get the response from the server.
                response = (HttpWebResponse)request.GetResponse();

                if (string.IsNullOrEmpty(response.CharacterSet))
                    charsetResponse = "utf-8";
                else
                    charsetResponse = response.CharacterSet;

                isGzip = "gzip".Equals(response.ContentEncoding, StringComparison.CurrentCultureIgnoreCase);

                Stream responseStream = response.GetResponseStream();

                MemoryStream resultStream = new MemoryStream(500000);
                responseStream.CopyTo(resultStream);
                resultStream.Seek(0, SeekOrigin.Begin);
                return resultStream;
            }
            catch (WebException ex)
            {
                // Check to see if the remote host return a response
                if (ex.Response != null)
                    ex.Response.Close();
            }
            catch (Exception ex)
            {
                shouldRetry = false;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return null;
        }


        public static HttpWebRequest GetHttpRequest(string url, string postData, CookieContainer cookies, int timeOut = Timeout.Infinite, string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);


                if (!string.IsNullOrEmpty(postData))
                {
                    request.Method = "POST";
                    request.ContentType = contentType;
                    request.ContentLength = postData.Length;
                }
                else
                    request.Method = "GET";

                request.Timeout = timeOut;
                request.Accept = Accept;
                request.ProtocolVersion = new System.Version("1.1");
                request.AllowAutoRedirect = true;
                request.UserAgent = UserAgent;
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                //if (!string.IsNullOrEmpty(baseUri))
                //    request.Referer = baseUri;
                request.CookieContainer = cookies ?? new CookieContainer();

                //request.Proxy = new WebProxy(ip, port);
                //request.Proxy.Credentials = CredentialCache.DefaultCredentials;


                StreamWriter w;
                if (!string.IsNullOrEmpty(postData))
                {
                    // Get a streamwriter from the request stream to send the form data.
                    w = new StreamWriter(request.GetRequestStream());
                    w.Write(postData);
                    w.Close();
                }
                return request;
            }
            catch
            {
                return null;
            }
        }

        public string GetFastHtml(string url, string charset = "utf-8", int timeout = 5000)
        {

            try
            {
                var uri = new Uri(url);
                var req = WebRequest.CreateDefault(uri);
                //var wp = new WebProxy(ip, port) { BypassProxyOnLocal = true };
                //req.Proxy = wp;
                req.Timeout = timeout;

                var res = req.GetResponse();
                var resStream = res.GetResponseStream();

                var realUrl = res.ResponseUri.AbsoluteUri;

                if (resStream != null)
                {
                    var reader = new StreamReader(resStream, System.Text.Encoding.GetEncoding(charset));
                    resStream.Flush();
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public Tuple<string, string> GetFastHtmlUrl(string url, string charset = "utf-8", int timeout = 8000)
        {

            try
            {
                var uri = new Uri(url);
                var req = WebRequest.CreateDefault(uri);
                //var wp = new WebProxy(ip, port) { BypassProxyOnLocal = true };
                //req.Proxy = wp;
                req.Timeout = timeout;

                var res = req.GetResponse();
                var resStream = res.GetResponseStream();

                string realUrl = res.ResponseUri.AbsoluteUri;

                if (resStream != null)
                {
                    var reader = new StreamReader(resStream, System.Text.Encoding.GetEncoding(charset));
                    resStream.Flush();
                    string content = reader.ReadToEnd();
                    Tuple<string, string> tuplResult = new Tuple<string, string>(realUrl, content);
                    return tuplResult;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }



    }
}
