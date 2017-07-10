using QiDianData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using ProxyLib;
using System.Threading;
using System.Data;
using System.Net;
using System.IO;
using SogouSearch.Models;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using SogouSearch.Helper;

namespace SogouSearch.Search
{
    public class SogouQuery
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





        int pages = 0;

        public void Query(IW2S_SG_BaiduCommend tsk)
        {
            List<ResultLiks> result = new List<ResultLiks>();
            var link = get_urls(tsk);
            try
            {
                GetLinks(link, tsk);
                //if (list != null && list.Count > 0)
                //    result.AddRange(list);
            }
            catch (Exception ex)
            {
                log(ex.Message + ex.StackTrace);
            }

        }

        void GetLinks(string link, IW2S_SG_BaiduCommend tsk)
        {
            List<IW2S_SG_level1link> result = new List<IW2S_SG_level1link>();
            int nohist_pages = 0;
            int quried_pages = 0;
            while (!string.IsNullOrEmpty(link) && quried_pages <= 2)
            {
                log(link);

                CookieContainer cc = new CookieContainer();
                Encoding enc = null;
                CookieCollection cookiesColl = new CookieCollection();
                CookieCollection cookieCollection = new CookieCollection();
                string Rurl = "https://www.sogou.com/";
                string cookie = "";
                string hhhtml = GetContentByIndex(Rurl, 8000, cc, ref enc, out Rurl, ref cookiesColl, out cookieCollection);
                cookiesColl = cookieCollection;
                string realUrl = "";
                var html = GetContent(link, 8000, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection); //(link);
                cookiesColl = cookieCollection;
                if (html == null)
                    break;
                //处理 vrwrap
                var tags = html.SubAfter("<body").SubAfter("results").SubBefore("hint_container").SplitWith("<h3");
                if (tags == null || tags.Length == 0)
                {
                    log("已被sogou屏蔽，请调试！ " + tsk.Keyword);
                    break;
                }
                bool nohit = true;
                foreach (var tag in tags)
                {
                    try
                    {
                        if (!tag.Contains("<a"))
                        {
                            continue;
                        }
                        string title = RemoveInivalidChar(tag.SubAfter("<a").SubBefore("</a>").GetTxtFromHtml2().RemoveSpace());
                        string href = tag.SubAfter("<a").SubBefore("</a>").GetFirstHref2();
                        string Jianjie = "";
                        if (tag.Contains("简介："))
                        {
                            Jianjie = tag.SubAfter("简介：").SubBefore("</").GetTxtFromHtml2().RemoveSpace();
                        }
                        if (tag.Contains("cacheresult_summary"))
                        {
                            Jianjie = tag.SubAfter("cacheresult_summary").SubBefore("</div>").GetTxtFromHtml2().RemoveSpace();
                        }
                        if (string.IsNullOrEmpty(Jianjie))
                        {
                            Jianjie = tag.SubAfter("summary_beg").SubBefore("summary_end").GetTxtFromHtml2().RemoveSpace();
                        }
                        int n = new Random().Next(8000, 15000);
                        Thread.Sleep(n);
                        var tuplehtml = GetContent(href, 8000, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection); // get_htmlUrl(href);
                        if (!string.IsNullOrEmpty(tuplehtml))
                        {
                            if (tuplehtml.Contains("window.location.replace"))
                            {
                                realUrl = tuplehtml.SubAfter("window.location.replace").SubBefore("</script>").Replace('"', ' ').Replace("(", "").Replace(")", "").RemoveSpace();
                            }
                            else
                            {
                                realUrl = Rurl;
                            }
                        }
                        string domain = "";
                        if (!string.IsNullOrEmpty(realUrl))
                        {
                            domain = GetDomain(realUrl);
                        }
                        else
                        {
                            realUrl = href;
                            domain = GetDomain(href);
                        }
                        string topDomain = GetLevel1Domain(domain);
                        bool IsContains = false;
                        int States = 0;
                        int blackid = 0;
                        realUrl = realUrl.Replace("amp;", "");
                        int nn = new Random().Next(6000, 15000);
                        Thread.Sleep(nn);
                        var htmldetail = GetContent(realUrl, 8000, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// get_Detailehtml(href, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(href, 8000, cc, ref enc, out Rurl);
                        Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
                        Match m = reg.Match(htmldetail);
                        //MatchCollection cols = reg.Matches(item.Html);
                        string time = "";
                        if (m.Groups.Count > 0)
                        {
                            time = m.Groups[0].Value;
                        }
                        //foreach (var item in BLtb)
                        //{
                        //    if (item.Domain.Trim().ToLower().Equals(topDomain.Trim().ToLower()))
                        //    {
                        //        States = 2;
                        //        blackid = item.Id;
                        //    }
                        //}
                        //foreach (var item in excludedKeywords)
                        //{
                        //    if (item.AuthorizedUrl1.Contains(topDomain))
                        //    {
                        //        IsContains = true;
                        //        States = 1;
                        //    }
                        //}
                        //if (IsContains == true)
                        //    continue;

                        IW2S_SG_level1link l1 = new IW2S_SG_level1link
                        {
                            UsrId = tsk.UsrId,
                            Domain = domain,
                            TopDomain = topDomain,
                            Keywords = string.Format("{0}", tsk.Keyword),
                            LinkUrl = realUrl,
                            Html = htmldetail,
                            BizId = IDHelper.GetGuid("{0}/{1}".FormatStr(realUrl, tsk._id.ToString())),
                            SearchkeywordId = tsk._id.ToString(),
                            CreatedAt = DateTime.UtcNow.AddHours(8),
                            Description = Jianjie,
                            Title = title,
                            ProjectId = tsk.ProjectId,
                            PublishTime = time,
                            AlternateFields = "0",
                            DataCleanStatus = 0
                        };
                        result.Add(l1);
                        nohit = false;
                        nohist_pages = 0;
                    }
                    catch (Exception ex)
                    {
                        log("有错误信息！" + ex.Message);
                    }
                }
                if (nohit)
                    nohist_pages++;
                //如果连续3页都没有结果，就跳出
                if (nohist_pages > 3)
                    break;
                quried_pages++;
                pages++;
                link = html.SubAfter("sogou_next").SubBefore("下一页").GetLastHref2();
                if (!string.IsNullOrEmpty(link) && !link.IsStartWith("http"))
                {
                    if (link.IsStartWith("/"))
                        link = link.SubAfter("/");
                    link = "https://www.sogou.com/web".GetContact(link);
                }
                SaveResult(result);
                result.Clear();
                int nn1 = new Random().Next(6000, 15000);
                Thread.Sleep(nn1);
            }
        }

        #region Save Result

        void SaveResult(List<IW2S_SG_level1link> listings)
        {
            if (listings.Count <= 0)
            {
                return;
            }
            var shopList = listings;
            shopList = shopList.DistinctBy(x => x.BizId);
            FieldsDocument shopfd = new FieldsDocument();
            shopfd.Add("BizId", 1);
            //   MongoCollection<IDGuidDto> shopcol = MongoDBHelper<IDGuidDto>.GetMongoDB().GetCollection<IDGuidDto>("WX_Data");
            var col = MongoDBHelper.Instance.Get_IW2S_SG_Data();
            var builder = Builders<IW2S_SG_level1link>.Filter;
            List<Guid> BizId = shopList.Select(x => x.BizId).ToList();
            //  var existsshop_objs = col.Find(builder.In(x => x.WeChatId, new BsonArray(WeChatId))).Project(x => x.WeChatId).ToList();
            //col.Find(MongoDB.Driver.Builders.Query.In("WeChatId", new BsonArray(WeChatId))).SetFields(shopfd);
            var existsshop_objs = col.Find(builder.In(x => x.BizId, BizId)).Project(x => x.BizId).ToList();
            List<Guid> exists_ids = new List<Guid>();
            foreach (var result in existsshop_objs)
            {
                exists_ids.Add(result);
            }
            if (existsshop_objs.Count() > 0)
            {
                shopList = shopList.Where(x => !exists_ids.Contains(x.BizId)).ToList();
            }
            if (shopList == null || shopList.Count == 0)
                return;
            if (shopList.Count > 0)
            {
                col.InsertMany(shopList);
                log("to save for IW2S_WX_level1link");
                log("Done");
            }

//            foreach (var item in shopList)
//            {
//                string sqlSel = "select id from ResultLiks where Rid='{0}'".FormatStr(item.Rid);

//                DataTable dt = DBHelper.Query(connStr, sqlSel);

//                if (dt.Rows.Count > 0)
//                {
//                    continue;
//                }
//                else
//                {
//                    string sqlIns = @"insert into ResultLiks( Rid, NovelId, NovelName, RealName, AuthorName, LinkTitle, LinkAbstract, LinkUrl, Keyword, Domain, TopDomain,BlackId, States) 
//                                    values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') "
//                        .FormatStr(item.Rid, item.NovelId, item.NovelName, item.RealName, item.AuthorName, item.LinkTitle, item.LinkAbstract, item.LinkUrl, item.Keyword, item.Domain, item.TopDomain, item.BlackId, item.States);
//                    int count = DBHelper.RunScript(connStr, sqlIns);
//                    log("保存一条数据到数据库中！");
//                }

//            }

        }

        #endregion


        HTML.WebHelper web = new HTML.WebHelper();
        string get_html(string url)
        {
            var html = web.GetHtml(url, null);
            return html;
        }

        public Tuple<string, string> get_htmlUrl(string url)
        {
            WebHelperNoCookieProxy wnp = new WebHelperNoCookieProxy();

            var tuplhtml = wnp.GetFastHtmlUrlWithProxyIpAndARE(url, "utf-8", 3, 3); //web.GetHtml2(url, null, "utf-8");

            string html = null;
            string realUrl = null;
            if (tuplhtml != null)
            {
                html = tuplhtml.Item2;
                realUrl = tuplhtml.Item1;
            }
            return new Tuple<string, string>(realUrl, html);
        }

        string get_urls(IW2S_SG_BaiduCommend tsk)
        {

            string searchKeywords = tsk.Keyword.RemoveSpace().GetLower();
            if (!string.IsNullOrEmpty(searchKeywords))
            {
                string baiduUrlFormat = "https://www.sogou.com/web?query={0}&cid=&page=1&ie=utf8&dr=1";// "https://www.sogou.com/web?query={0}&ie=utf8";
                return baiduUrlFormat.FormatStr(searchKeywords.GetUrlEncodedString("utf-8"));

            }
            return string.Empty;
        }


        static HashSet<char> chinese_commas = new HashSet<char> { '？', '。', '，', '！', '》', '《', '‘', '“', '；' };
        public static string RemoveInivalidChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (chinese_commas.Contains(c))
                    continue;
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c > 255)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        static readonly string[] domain_sufixes = new string[]{  ".cn",".la",".top",".so",".biz",".name",".info",".cc",".tv",".中国",".mobi",".me",".asia",
            ".co",".tel",".公司",".网络",".wang",".net",".org",".edu",".gov",".com"};
        static readonly string[] domain_sps = new string[] { "/", "?", " ", "." };

        public static string GetDomain(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            string prefix = "";
            foreach (var sufix in domain_sufixes)
            {
                foreach (var sp in domain_sps)
                {
                    prefix = url.SubLastStringBefore(sufix + sp);
                    if (!string.IsNullOrEmpty(prefix))
                        return prefix + sufix;
                }
            }
            return url;
        }

        public static string GetLevel1Domain(string domain)
        {
            if (string.IsNullOrEmpty(domain) || !domain.IsContain("."))
                return domain;
            if (domain.IsStartWith("www."))
                domain = domain.SubAfter("www.");
            if (domain.IsContain("//"))
                domain = domain.SubAfter("//");
            if (domain.IsContain("?"))
                domain = domain.SubBefore("?");
            if (domain.IsContains("/"))
                domain = domain.SubBefore("/");
            if (string.IsNullOrEmpty(domain) || !domain.IsContain("."))
                return domain;
            //.com.cn, .net.cn
            int level2Index = int.MaxValue;
            foreach (var sufix in domain_sufixes)
            {
                int index = domain.IndexOf(sufix + ".");
                if (index > 0 && index < level2Index)
                    level2Index = index;
            }
            if (level2Index < int.MaxValue && level2Index > 0)
            {
                string tmp = domain.Substring(0, level2Index);
                if (tmp.Contains("."))
                    tmp = tmp.SubLastStringAfter(".");
                domain = tmp + domain.Substring(level2Index);
                return domain;
            }
            else
            {
                string tmp = domain.SubLastStringBefore(".");
                if (tmp.Contains("."))
                    tmp = tmp.SubLastStringAfter(".");
                domain = tmp.GetContact(".").GetContact(domain.SubLastStringAfter("."));
                return domain;
            }
        }







        public static string GetContent(string url, int timeout, ref Encoding encoding, out string Rurl, string cookie, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)
        {
            //int n = new Random().Next(0, arayList.Length - 1);
            //Uagent = arayList[n].ToString();

            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "*/*";
            req.Method = "GET";
            req.UserAgent = Uagent;//"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            req.Proxy = null;
            req.KeepAlive = true;
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(cookiesColl);
            //   req.Headers.Add(HttpRequestHeader.Cookie, "CXID=7E7921DF0B61F4BF096305E400A08956; SUID=F682E9674E6C860A567CE02C0003617F; SUV=00212B1C67E982F6567CE02ECE4C6407; weixinIndexVisited=1; ssuid=1634306985; ABTEST=0|1456304254|v1; ld=Dqr6slllll2QZIgmlllllVbK0IllllllBMujdkllll9llllljvoll5@@@@@@@@@@; ad=4lllllllll2QKnl7lllllVbr6DYlllllBMujdklllxllllll9Oxlw@@@@@@@@@@@; SNUID=116A018FE8E2C9A4089226FBE87D79E5; sct=51; IPLOC=CN88; LSTMV=1009%2C75; LCLKINT=107158");
            // req.ProtocolVersion = HttpVersion.Version11;
            //  req.Host = "weixin.sogou.com";

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
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




        public static string GetContentByIndex(string url, int timeout, CookieContainer cc, ref Encoding encoding, out string Rurl, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)
        {
            int n = new Random().Next(0, arayList.Length - 1);
            Uagent = arayList[n].ToString();

            string responsestr = "";
            System.GC.Collect();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.Method = "GET";
            req.UserAgent = Uagent;//"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            req.Proxy = null;
            req.KeepAlive = true;
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(cookiesColl);
            // req.Headers.Add(HttpRequestHeader.Cookie, cookies);
            req.ProtocolVersion = HttpVersion.Version11;
            //  req.Host = "weixin.sogou.com";

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    cookiesCollection = req.CookieContainer.GetCookies(req.RequestUri);
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





        public static Encoding GetEncoding(string CharacterSet)
        {
            switch (CharacterSet.ToLower())
            {
                case "gb2312": return Encoding.GetEncoding("gb2312");
                case "utf-8": return Encoding.UTF8;

                default: return null;
            }
        }




        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + msg);
        }

    }
}
