using ProxyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iw2swcsserch.Models;
using AISSystem;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using iw2swcsserch.Helper;
using MongoDB.Driver;

using MongoDB.Bson;
using System.Net;

namespace iw2swcsserch.Template
{

    public class WeChatQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;
        public WeChatQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);
            }
        }
        public List<IW2S_WX_level1link> Query(IW2S_WX_BaiduCommend searchTsk)
        {
            var links = get_url(searchTsk.Keyword, searchTsk.CommendKeyword);
            if (links == null || links == "")
                return null;

            List<IW2S_WX_level1link> result = new List<IW2S_WX_level1link>();

            var list = GetLinks(links, searchTsk);
            if (list != null && list.Count > 0)
                result.AddRange(list);

            return result;
        }
        string get_url(string taskKey, string busKey)
        {
            string sougouUrlFormat = "http://weixin.sogou.com/weixin?type=2&query={0}&ie=utf8".FormatStr(taskKey);
            return sougouUrlFormat;
            //string searchKeywords = taskKey + "+" + busKey;
            //if (!string.IsNullOrEmpty(searchKeywords))
            //{
            //    string baiduUrlFormat = "http://weixin.sogou.com/weixin?type=2&query={0}&ie=utf8";
            //    return baiduUrlFormat.FormatStr(searchKeywords.GetUrlEncodedString("utf-8"));

            //}
            //return string.Empty;

        }
        public List<IW2S_WX_level1link> GetLinks(string link, IW2S_WX_BaiduCommend searchTsk)
        {


            List<IW2S_WX_level1link> result = new List<IW2S_WX_level1link>();
            int nohist_pages = 0;
            int quried_pages = 0;
            //最多搜索10页 
            while (!string.IsNullOrEmpty(link) && quried_pages <= 2)
            {
                log(link);
                CookieContainer cc = new CookieContainer();
                Encoding enc = null;
                CookieCollection cookiesColl = new CookieCollection();
                CookieCollection cookieCollection = new CookieCollection();
                string Rurl = "http://weixin.sogou.com/";
                string cookie = "";
                string hhhtml = TaobaoWebHelper.GetContentByIndex(Rurl, 8000, cc, ref enc, out Rurl, ref cookiesColl, out cookieCollection);
                cookiesColl = cookieCollection;
                int gg = new Random().Next(5000, 8000);
                Thread.Sleep(gg);

                Rurl = link;
                var html = get_html(link, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(link, 8000, cc, ref enc, out Rurl);
                cookiesColl = cookieCollection;
                if (html == null)
                    break;

                if (html.Contains("没有找到相关的微信公众号文章"))
                {
                    break;
                }

                var tags = html.SplitWith("wx-rb wx-rb3");
                if (tags == null || tags.Length == 0 || tags.Length == 1)
                {
                    tags = html.SplitWith("wx-rbwx-rb3");
                }
                if (tags == null || tags.Length == 0)
                {
                    log("BLOCKED " + searchTsk.Keyword + " " + searchTsk.CommendKeyword);
                    break;
                }
                bool nohit = true;
                foreach (var tag in tags)
                {
                    if (!tag.Contains("txt-box"))
                    {
                        continue;
                    }
                    string title = RemoveInivalidChar(tag.SubAfter("<h4").SubBefore("</h4>").GetTxtFromHtml2().RemoveSpace());
                    string href = tag.SubAfter("<h4").SubBefore("</a>").GetFirstHref2();
                    string abs = RemoveInivalidChar(tag.SubAfter("<h4>").SubBefore("\"s-p\"").SubBefore("<script>").GetTxtFromHtml2().RemoveSpace());
                    string domain = tag.SubLastStringAfter("\"s-p\"").SubBefore("</a").GetTxtFromHtml2().SubAfter("(").SubAfter("(").SubBefore(",").Replace('"', ' ').Trim();
                    //domain = BaiduQuery.GetDomain(domain);
                    string SourceLink = tag.SubLastStringAfter("\"s-p\"").SubBefore("</a").GetFirstHref2();

                    string TitleImg = tag.SubAfter("img_box2").SubBefore("</a").SubAfter("src=").Replace(">", "").Replace('"', ' ').RemoveSpace();


                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0},{1}".FormatStr(title, abs);

                    if (string.IsNullOrEmpty(txt))
                        continue;

                    //var excludekwdcount = ExcludeKeyword.Count(c => txt.Contains(c.KeywordName));
                    //if (excludekwdcount > 0)
                    //    continue;

                    if (href.IsStartWith("/websearch"))
                        href = "http://weixin.sogou.com" + href.Replace("amp;", "");
                    if (href.IsStartWith("s?__biz"))
                    {
                        var href1 = href.Replace("amp;", "");
                    }
                    href = href.Replace("amp;", "");
                    int nn = new Random().Next(8000, 20000);
                    Thread.Sleep(nn);

                    var htmldetail = get_Detailehtml(href, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(href, 8000, cc, ref enc, out Rurl);


                    Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
                    Match m = reg.Match(htmldetail);
                    //MatchCollection cols = reg.Matches(item.Html);
                    string time = "";
                    if (m.Groups.Count > 0)
                    {
                        time = m.Groups[0].Value;
                    }
                    href = Rurl;
                    var hrefNew = href + "&f=json";
                    var htmldetailNewUrl = get_Detailehtml(hrefNew, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);// GetContent(href, 8000, cc, ref enc, out Rurl);
                    try
                    {
                        var uuurl = htmldetailNewUrl.SubAfter("\"link\":").SubBefore(",\"source_url\":").Replace('"', ' ').Replace("\\", "").RemoveSpace();
                        href = uuurl;
                    }
                    catch (Exception)
                    {
                    }
                    bool is_title_matched = title.GetLower().IsContains2(searchTsk.Keyword.ToLower(), searchTsk.CommendKeyword.ToLower());
                    bool is_abstr_matched = abs.GetLower().IsContains2(searchTsk.Keyword.GetLower(), searchTsk.CommendKeyword.GetLower());
                    BaiduItemPart part = is_title_matched && is_abstr_matched ? BaiduItemPart.TitleAbstract :
                        is_title_matched ? BaiduItemPart.Title :
                        is_abstr_matched ? BaiduItemPart.Abstract : BaiduItemPart.None;
                    bool is_itm_title_matched = txt.GetLower().IsContains(searchTsk.Keyword.GetLower());
                    bool is_bus_matched = txt.GetLower().IsContains2(searchTsk.CommendKeyword.GetLower());
                    var no = "";
                    var qrcode = "";
                    var function = "";
                    var NoIcon = "";
                    var QrcodeIcon = "";
                    SourceLink = SourceLink.Replace("amp;", "");
                    int nnn = new Random().Next(8000, 15000);
                    Thread.Sleep(nnn);
                    var htmlNo = get_Nohtml(SourceLink, 8000, cc, ref enc, out Rurl, cookie, ref cookiesColl, out cookieCollection);
                    if (!string.IsNullOrEmpty(htmlNo) && htmlNo.Contains("em_weixinhao"))
                    {
                        no = htmlNo.SubAfter("em_weixinhao").SubBefore("/label").GetTxtFromHtml2().RemoveSpace();
                        qrcode = htmlNo.SubAfter("v-box").SubBefore("<em").SubAfter("src=").Replace(">", "").Replace('"', ' ').RemoveSpace();
                        function = htmlNo.SubAfter("功能介绍:</").SubBefore("/span").GetTxtFromHtml2().RemoveSpace();
                        SourceLink = htmlNo.SubAfter("微信认证：").SubBefore("/div").GetTxtFromHtml2().RemoveSpace();
                        NoIcon = htmlNo.SubAfter("img-box").SubBefore("</a").SubAfter("src=").SubBefore("onload").Replace(">", "").Replace('"', ' ').RemoveSpace();
                        QrcodeIcon = htmlNo.SubAfter("img-box").SubBefore("</a").SubAfter("err:").SubBefore(">").Replace(">", "").Replace('"', ' ').Replace("'", "").RemoveSpace();
                    }
                    IW2S_WX_level1link l1 = new IW2S_WX_level1link
                    {
                        BizId = IDHelper.GetGuid("{0}/{1}/{2}".FormatStr(title, domain, searchTsk.UsrId)),
                        Description = abs,
                        Domain = domain,
                        UsrId = searchTsk.UsrId,
                        LinkUrl = href,
                        MatchAt = (byte)part,
                        Title = title,
                        CreatedAt = DateTime.Now,
                        DataCleanStatus = 0,
                        Function = function,
                        SearchkeywordId = searchTsk._id.ToString(),
                        Keywords = searchTsk.Keyword ,
                        PublicNo = no,
                        QrCode = qrcode,
                        SourceLink = SourceLink,
                        TagType = 0,
                        ImgIcon = NoIcon,
                        QrCodeIcon = QrcodeIcon,
                        ProjectId = searchTsk.ProjectId,
                        TitleImg = TitleImg,
                        PublishTime = time,
                        Html = htmldetail
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
                link = html.SubAfter("sogou_next").SubBefore("下一页").GetLastHref2();
                if (!string.IsNullOrEmpty(link) && !link.IsStartWith("http"))
                {
                    if (link.IsStartWith("/"))
                        link = link.SubAfter("/");
                    link = "http://weixin.sogou.com/weixin".GetContact(link);
                }

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

            if (!html.IsContains2("wx-rb wx-rb3", "wx-rbwx-rb3"))
            {
                WebHelperNoCookieProxy web2 = new WebHelperNoCookieProxy();
                var html2 = false;
                while (html2 == false)
                {
                    //web2.ChangeIp();
                    //IP ip = web2.my_ip;
                    //  html = WeChatQueryByBus.GetContentByIP(url, 8000, cc, ip.Ip, ip.Port, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);//web2.GetHtml(url, null, "utf-8");
                    html = TaobaoWebHelper.GetContent(url, 8000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);  //web.GetHtml(url, null);
                    html2 = html.IsContains2("wx-rb wx-rb3", "wx-rbwx-rb3");

                    if (html.Contains("没有找到相关的微信公众号文章"))
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

                    //web2.ChangeIp();
                    //IP ip = web2.my_ip;
                    //  html = WeChatQueryByBus.GetContentByIP(url, 8000, cc, ip.Ip, ip.Port, ref enc, out Rurl, cookie,ref cookieColl,out cookieCollection);//web2.GetHtml(url, null, "utf-8");
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
        string get_Nohtml(string url, int timeout, CookieContainer cc, ref Encoding enc, out string Rurl, string cookie, ref CookieCollection cookieColl, out CookieCollection cookieCollection)
        {
            // var html = web.GetHtml(url, null);
            var html = TaobaoWebHelper.GetContent(url, 8000, cc, ref enc, out Rurl, cookie, ref cookieColl, out cookieCollection);
            if (!html.IsContains2("wx-rb wx-rb2", "wx-rbwx-rb2"))
            {
                WebHelperNoCookieProxy web2 = new WebHelperNoCookieProxy();
                var html2 = false;
                //while (html2 == false)
                //{
                //    //  web2.ChangeIp();
                //    html = web2.GetHtml(url, null, "utf-8");
                //    html2 = html.IsContains2("wx-rb wx-rb2", "wx-rbwx-rb2");
                //    if (!string.IsNullOrEmpty(html) && html.Contains("您的访问过于频繁"))
                //    {
                //        log("您的访问过于频繁，为确认本次访问为正常用户行为，需要您协助验证。");
                //        //int n = new Random().Next(1000, 3000);
                //        //Thread.Sleep(n);
                //    }

                //}
            }
            return html;
        }


        #region Save Result

        void SaveResult(List<IW2S_WX_level1link> listings)
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
            var col = MongoDBHelper.Instance.Get_IW2S_Data();
            var builder = Builders<IW2S_WX_level1link>.Filter;

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

        }

        #endregion




        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name + "/" + pages + "]:" + msg);
        }

    }

}
