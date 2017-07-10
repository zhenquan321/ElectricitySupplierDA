using GoogleS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using System.Net;
using System.Threading;
using MongoDB.Driver;
using ProxyLib;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace GoogleS.Search
{
    public class GoogleQuery
    {


        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;
        public GoogleQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);
            }
        }


        public List<Dnl_Google_level1link> Query(Dnl_Google_BaiduCommend searchTsk)
        {
            var links = get_url(searchTsk.Keyword, searchTsk.CommendKeyword);
            if (links == null || links == "")
                return null;

            List<Dnl_Google_level1link> result = new List<Dnl_Google_level1link>();

            var list = GetLinks(links, searchTsk);
            if (list != null && list.Count > 0)
                result.AddRange(list);

            return result;
        }

        string get_url(string taskKey, string busKey)
        {
            string sougouUrlFormat = "https://www.google.com.hk/search?q={0}".FormatStr(taskKey);
            return sougouUrlFormat;

        }



        public List<Dnl_Google_level1link> GetLinks(string link, Dnl_Google_BaiduCommend searchTsk)
        {

            List<Dnl_Google_level1link> result = new List<Dnl_Google_level1link>();
            int nohist_pages = 0;
            int quried_pages = 0;
            int fanye = 0;

            //最多搜索10页 
            while (!string.IsNullOrEmpty(link) && quried_pages <= 10)
            {
                log(link);
                CookieContainer cc = new CookieContainer();
                Encoding enc = null;
                CookieCollection cookiesColl = new CookieCollection();
                CookieCollection cookieCollection = new CookieCollection();
                string Rurl = "https://www.google.com";
                string cookie = "";

                WebClient webClient = new WebClient();
                webClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = webClient.DownloadData(link);
                
                string pageHtml = Encoding.GetEncoding("Big5").GetString(pageData);

                pageHtml = Strings.StrConv(pageHtml, VbStrConv.SimplifiedChinese, 0);

                //string hhhtml = TaobaoWebHelper.GetContentByIndex(Rurl, 80000, cc, ref enc, out Rurl, ref cookiesColl, out cookieCollection);
                //cookiesColl = cookieCollection;
                //int gg = new Random().Next(2000, 5000);
                //Thread.Sleep(gg);

                //Rurl = link;
                //var html = get_html(link, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(link, 8000, cc, ref enc, out Rurl);
                //cookiesColl = cookieCollection;

                var html = pageHtml;

                if (html == null)
                    break;
              //  html = Regex.Unescape(html);
                if (html.Contains("没有找到搜索内容！"))
                {
                    break;
                }

                var tags = html.SubAfter("<body").SubAfter("center_col").SubBefore("id=\"foot\"");


                var tagsD = tags.SplitWith("class=\"g\"");
                
                if (tagsD == null || tagsD.Length == 0)
                {
                    log("BLOCKED " + searchTsk.Keyword + " " + searchTsk.CommendKeyword);
                    break;
                }
                bool nohit = true;
                foreach (var tag in tagsD)
                {
                    if (!tag.Contains("h3"))
                    {
                        continue;
                    }

                    //if (!tag.Contains("sp_requery"))
                    //{
                    //    continue;
                    //}
                    var a = tag.SubAfter("h3").SubAfter("a");
                    string title = RemoveInivalidChar(a.RemoveSpace().GetLower().SubBefore("</h3>").GetTxtFromHtml2().RemoveSpace().GetLower()); // RemoveInivalidChar(tag.SubAfter("<h4").SubBefore("</h4>").GetTxtFromHtml2().RemoveSpace());
                    string href = a.GetFirstHref2(); //tag.SubAfter("<h4").SubBefore("</a>").GetFirstHref2();

                    if (href.Contains("/url?q="))
                    {
                        href = href.Replace("/url?q=", "");
                    }


                    if (!href.Contains("http"))
                    {
                        href = "https://www.google.com" + href;
                    }

                    if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(href))
                    {
                        continue;
                    }
                    href = href.Replace("amp;", "");
                    var sdsfdsf = GetDomain(href);
                    string abs = RemoveInivalidChar(tag.SubAfter("class=\"st\"").SubBefore("</span").GetTxtFromHtml2().RemoveSpace().GetLower()); //RemoveInivalidChar(tag.SubAfter("<h4>").SubBefore("\"s-p\"").SubBefore("<script>").GetTxtFromHtml2().RemoveSpace());
                    string timesp = "";
                    string domain = GetDomain(href); 
                    //tag.SubLastStringAfter("\"s-p\"").SubBefore("</a").GetTxtFromHtml2().SubAfter("(").SubAfter("(").SubBefore(",").Replace('"', ' ').Trim();
                    //domain = BaiduQuery.GetDomain(domain);

                    int maxScore = 0;

                    byte appType = 0;
                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0},{1}".FormatStr(title, abs);
                    if (string.IsNullOrEmpty(txt))
                        continue;

                    int nn = new Random().Next(8000, 20000);
                    Thread.Sleep(nn);
                    var htmldetail = "";

                    try
                    {
                       // htmldetail = get_Detailehtml(href, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(href, 8000, cc, ref enc, out Rurl);

                        WebClient webClient2 = new WebClient();
                        webClient2.Credentials = CredentialCache.DefaultCredentials;
                        Byte[] pageData2 = webClient2.DownloadData(href);

                        htmldetail = Encoding.GetEncoding("Big5").GetString(pageData2);

                        htmldetail = Strings.StrConv(htmldetail, VbStrConv.SimplifiedChinese, 0);


                    }
                    catch (Exception)
                    {
                        //htmldetail = "";
                       
                    }

                    Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
                    Match m = reg.Match(htmldetail);
                    //MatchCollection cols = reg.Matches(item.Html);
                   
                    if (m.Groups.Count > 0)
                    {
                        timesp = m.Groups[0].Value;
                    }

                    bool is_title_matched = title.GetLower().IsContains2(searchTsk.Keyword.ToLower(), searchTsk.CommendKeyword.ToLower());
                    bool is_abstr_matched = abs.GetLower().IsContains2(searchTsk.Keyword.GetLower(), searchTsk.CommendKeyword.GetLower());
                    BaiduItemPart part = is_title_matched && is_abstr_matched ? BaiduItemPart.TitleAbstract :
                        is_title_matched ? BaiduItemPart.Title :
                        is_abstr_matched ? BaiduItemPart.Abstract : BaiduItemPart.None;
                    bool is_itm_title_matched = txt.GetLower().IsContains(searchTsk.Keyword.GetLower());
                    bool is_bus_matched = txt.GetLower().IsContains2(searchTsk.CommendKeyword.GetLower());



                    Dnl_Google_level1link l1 = new Dnl_Google_level1link
                    {
                        UsrId = searchTsk.UsrId,
                        Domain = domain.Replace("http://","").Replace("https://",""),
                        TopDomain = GetLevel1Domain(domain),
                        Keywords = string.Format("{0} + {1}", searchTsk.Keyword, searchTsk.CommendKeyword),
                        LinkUrl = href,
                        MatchAt = (byte)part,
                        Html = htmldetail,
                        MatchType = (byte)((is_bus_matched ? 1 : 0) + (is_itm_title_matched ? 2 : 0)),
                        AppType = appType,
                        BizId = IDHelper.GetGuid("{0}/{1}/{2}".FormatStr(href, searchTsk.UsrId, searchTsk.Keyword)),
                        SearchkeywordId = searchTsk._id.ToString(),
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        Description = abs,
                        Title = title,
                        Score = maxScore,
                        Abstract = abs,
                        ProjectId = searchTsk.ProjectId,
                        PublishTime = timesp
                    };
                    if (is_bus_matched)
                    {
                        l1.MatchType = l1.MatchType;
                    }
                    if (is_itm_title_matched)
                    {
                        l1.MatchType = l1.MatchType;
                    }
                    byte MatchType = (byte)((is_bus_matched ? 10 : 0) + (is_itm_title_matched ? 30 : 0));
                    if (is_bus_matched == true && is_itm_title_matched == true)
                    {
                        //l1.Score = busTsk.Score + 5;
                        l1.Score = 80 + 10;
                    }
                    if (is_bus_matched == true && is_itm_title_matched == false)
                    {
                        l1.Score = 80;
                    }
                    if (is_bus_matched == false && is_itm_title_matched == true)
                    {
                        l1.Score = 50;
                    }

                    result.Add(l1);
                    nohit = false;
                    nohist_pages = 0;
                }

                if (nohit)
                    nohist_pages++;
                //如果连续3页都没有结果，就跳出
                if (nohist_pages > 3)
                    break;

                quried_pages++;
                pages++;

                //****** sougou 需要重写 ********************* 
                link = html.SubAfter("id=\"foot\"").SubAfter("text-align:left").SubBefore("下一页").GetLastHref2();
                if (!string.IsNullOrEmpty(link) && !link.IsStartWith("http"))
                {
                    if (link.IsStartWith("/"))
                        link = link.SubAfter("/");
                    link = "https://www.google.com/".GetContact(link);

                    link = link.Replace("amp;", "");

                }
                fanye = fanye + 10;
              
                SaveResult(result);
                result.Clear();

                int n = new Random().Next(8000, 15000);
                Thread.Sleep(n);

            }
            return result;
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
        string get_html(string url, int timeout, CookieContainer cc, ref Encoding enc, out string Rurl, string cookie, ref CookieCollection cookieColl, out CookieCollection cookieCollection)
        {

            var html = TaobaoWebHelper.GetContent(url, 8000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);  //web.GetHtml(url, null);

            //(string url, int timeout, CookieContainer cc, ref Encoding encoding, out string Rurl, string cookie, ref CookieCollection cookiesColl, out CookieCollection cookiesCollection)

            if (!html.IsContains2("sbibtd"))
            {
                WebHelperNoCookieProxy web2 = new WebHelperNoCookieProxy();
                var html2 = false;
                while (html2 == false)
                {
                    //web2.ChangeIp();
                    //IP ip = web2.my_ip;
                    //  html = WeChatQueryByBus.GetContentByIP(url, 8000, cc, ip.Ip, ip.Port, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);//web2.GetHtml(url, null, "utf-8");
                    html = TaobaoWebHelper.GetContent(url, 80000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);  //web.GetHtml(url, null);
                    html2 = html.IsContains2("sbibtd");

                    if (html.Contains("没有找到内容！"))
                    {
                        return html;
                    }

                    if (!string.IsNullOrEmpty(html) && html.Contains("您的访问过于频繁"))
                    {
                        log("您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证。");
                        int n = new Random().Next(1000, 3000);
                        Thread.Sleep(n);
                    }

                }
            }
            return html;
        }


        string get_Detailehtml(string url, int timeout, CookieContainer cc, ref Encoding enc, out string Rurl, string cookie, ref CookieCollection cookieColl, out CookieCollection cookieCollection)
        {

            WebHelperNoCookieProxy web1 = new WebHelperNoCookieProxy();
            var html = TaobaoWebHelper.GetContent(url, 8000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);
            if (!string.IsNullOrEmpty(url))
            {
                if (!html.IsContains2("profile_inner"))
                {
                    WebHelperNoCookieProxy web2 = new WebHelperNoCookieProxy();
                    var html2 = false;
                    html = TaobaoWebHelper.GetContent(url, 8000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);  //web.GetHtml(url, null);
                    html2 = html.IsContains2("profile_inner");
                    if (!string.IsNullOrEmpty(html) && html.Contains("您的访问过于频繁"))
                    {
                        log("您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证。");
                        int n = new Random().Next(1000, 3000);
                        Thread.Sleep(n);
                    }

                }
            }
            return html;
        }


        #region Save Result

        void SaveResult(List<Dnl_Google_level1link> listings)
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
            var col = MongoDBHelper.Instance.Get_Dnl_Google_level1link();
            var builder = Builders<Dnl_Google_level1link>.Filter;

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
                log("to save for Dnl_Google_level1link");
                log("Done");
            }

        }

        #endregion

        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name + "/" + pages + "]:" + msg);
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




    }
}
