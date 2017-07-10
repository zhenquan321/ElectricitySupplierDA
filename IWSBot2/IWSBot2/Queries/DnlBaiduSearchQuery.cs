using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLib;
using AISSystem;
using System.Threading;
using IWSData.Model;
using IWSBot.Utility;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using IWSBot2.Helper;

namespace IWSBot.Queries
{
    public class DnlBaiduSearchQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        //HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;

        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;


        public DnlBaiduSearchQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);

            }
        }

        /// <summary>
        /// 百度搜索
        /// </summary>
        /// <param name="tsk">要搜索的关键词信息</param>
        /// <param name="excludedDomains">排除域名</param>
        public void Query(Dnl_Keyword tsk, List<Dnl_IgnoreDomain> excludedDomains)
        {
            var link = get_urls(tsk);
            try
            {
                GetLinks(link, tsk, excludedDomains);
            }
            catch (Exception ex)
            {
                log(ex.Message + ex.StackTrace);
            }


        }

        /// <summary>
        /// 抓取搜索页面
        /// </summary>
        /// <param name="link">搜索链接</param>
        /// <param name="tsk">要搜索的关键词信息</param>
        /// <param name="excludedDomains">排除关键词</param>
        void GetLinks(string link, Dnl_Keyword tsk, List<Dnl_IgnoreDomain> excludedDomains)
        {
            string searchKeyword = tsk.Keyword.Trim();

            int nohist_pages = 0;       //未命中页面
            int quried_pages = 0;       //已搜索页面
            int rank = 1;               //页面中网址排名
            //最多搜索3页 
            while (!string.IsNullOrEmpty(link) && quried_pages <= 2)
            {
                log(link);
                //获取搜索页面源码
                var html = get_html(link);
                if (html == null)
                    break;

                //处理百度推广链接
                var propContents = new List<string>();
                if (!string.IsNullOrEmpty(html.SubAfter("content_left").SubAfter("div id=\"400")))
                {
                    propContents = html.SubAfter("content_left").SubAfter("div id=\"400").SubBefore("c-container").SplitWith("div id=\"400").ToList();
                }
                else if (!string.IsNullOrEmpty(html.SubAfter("content_left").SubAfter("divid=\"400")))
                {
                    propContents = html.SubAfter("content_left").SubAfter("divid=\"400").SubBefore("c-container").SplitWith("divid=\"400").ToList();
                }
                foreach (var tag in propContents)
                {
                    var a = tag.SubAfter("h3").SubAfter("a");
                    //获取标题
                    string title = a.SubBefore("</h3>").GetTxtFromHtml2();
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = title.Trim();
                    }
                    string href = a.GetFirstHref2();
                    //获取描述
                    string abs = tag.SubAfter("</h3>").SubBefore("</a").GetTxtFromHtml2();
                    if (string.IsNullOrEmpty(abs))
                    {
                        abs = abs.Trim();
                    }
                    string domain = string.Empty;   //二级域名

                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0}{1}".FormatStr(title, abs);
                    if (string.IsNullOrEmpty(txt))
                        continue;

                    HanleTagData(tsk, excludedDomains, searchKeyword, title, href, abs, ref domain, tag, true);
                }

                //获取搜索结果部分页面
                var tags = html.SubAfter("content_left").SplitWith("c-container");
                if (tags == null || tags.Length == 0)
                {
                    log("BLOCKED " + tsk.Keyword);
                    break;
                }
                bool nohit = true;
                foreach (string tag in tags)
                {
                    //获取单个搜索结果信息
                    var a = tag.SubAfter("h3").SubAfter("a");
                    //获取标题
                    string title = a.SubBefore("</h3>").GetTxtFromHtml2();
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = title.Trim();
                    }
                    string href = a.GetFirstHref2();    //链接
                    //获取描述
                    string description = tag.SubAfter("abstract").SubBefore("</div").GetTxtFromHtml2();
                    if (string.IsNullOrEmpty(description))
                    {
                        description = description.Trim();
                    }
                    string domain = tag.SubLastStringAfter("\"f13").SubBefore("</span").GetTxtFromHtml2();
                    domain = GetDomain(domain);         //域名

                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0}{1}".FormatStr(title, description);
                    if (string.IsNullOrEmpty(txt))
                        continue;

                    //解析搜索结果数据
                    HanleTagData(tsk, excludedDomains, searchKeyword, title, href, description, ref domain, tag, false);
                    nohit = false;
                    nohist_pages = 0;
                    rank++;
                }

                if (nohit)
                    nohist_pages++;
                //如果连续3页都没有结果，就跳出
                if (nohist_pages > 3)
                    break;

                quried_pages++;
                pages++;
                //获取下一页搜索页面链接
                link = html.SubAfter("fk fk_cur").SubBefore("下一页").GetLastHref2();
                if (!string.IsNullOrEmpty(link) && !link.IsStartWith("http"))
                {
                    if (link.IsStartWith("/"))
                        link = link.SubAfter("/");
                    link = "http://www.baidu.com/".GetContact(link);
                }

            }
            //return result;
        }

        /// <summary>
        /// 解析搜索结果数据
        /// </summary>
        /// <param name="tsk">关键词信息</param>
        /// <param name="excludedDomains">排除域名列表</param>
        /// <param name="searchKeywords">搜索关键词</param>
        /// <param name="title">标题</param>
        /// <param name="href">链接</param>
        /// <param name="description">描述</param>
        /// <param name="domain">域名</param>
        /// <param name="tag">搜索结果源码</param>
        /// <param name="isMarket">是否为推广链接</param>
        private void HanleTagData(Dnl_Keyword tsk, List<Dnl_IgnoreDomain> excludedDomains, string searchKeywords, string title, string href, string description, ref string domain, string tag, bool isMarket)
        {
            string realUrl = null, detailHtml = null;     //真实网址、网页源码
            //判断百度蓝V等级
            int? baiduVStar = null;
            if (tag.Contains("c-icon-v1"))
            {
                baiduVStar = 1;
            }
            else if (tag.Contains("c-icon-v2"))
            {
                baiduVStar = 2;
            }
            else if (tag.Contains("c-icon-v3"))
            {
                baiduVStar = 3;
            }
            //获取真实网址、网页源码和网页摘要
            if (!string.IsNullOrWhiteSpace(href))
            {
                //获取网页源码及真实地址
                var tuplehtml = get_htmlUrl(href);
                if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item1))
                {
                    realUrl = tuplehtml.Item1;
                }
                if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item2))
                {
                    detailHtml = tuplehtml.Item2;
                }
                //获取网页二级域名
                if (!string.IsNullOrEmpty(realUrl) && string.IsNullOrEmpty(domain))
                {
                    domain = GetDomain(realUrl);
                }
            }
            //如果网页本身也是跳转链接，进一步获取获取真实网页源码并解析数据
            if (!string.IsNullOrEmpty(detailHtml) && detailHtml.Contains("document.getElementById(\"link\").click()"))
            {
                var gourl = detailHtml.GetFirstHref2();
                if (!string.IsNullOrEmpty(gourl))
                {
                    var tuplehtml = get_htmlUrl(gourl);
                    if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item1))
                    {
                        realUrl = tuplehtml.Item1;
                    }
                    if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item2))
                    {
                        detailHtml = tuplehtml.Item2;
                    }
                    if (!string.IsNullOrEmpty(realUrl) && string.IsNullOrEmpty(domain))
                    {
                        domain = GetDomain(realUrl);
                    }
                }
            }
            //去除前缀
            Regex regDomain = new Regex("http://|https://");
            long collectionNum = 0;
            if (!string.IsNullOrEmpty(domain))
            {
                domain = regDomain.Replace(domain, "");
                collectionNum = GetDomainCollectionNum(domain);
            }

            if (string.IsNullOrEmpty(realUrl))
            {
                realUrl = href;
            }
            List<KeywordScore> matchpatterns = new List<KeywordScore>();
            if (string.IsNullOrEmpty(detailHtml))
            {
                return;
            }
            else
            {
                var hrefs = detailHtml.GetDescendents("a", "href");
            }

            string content = GetMainContentHelper.GetMainContent(detailHtml);       //获取网页中文正文

            bool is_title_matched = title.IsContains2(searchKeywords);              //标题是否匹配到关键词
            bool is_desc_matched = description.IsContains2(searchKeywords);         //描述是否匹配到关键词
            BaiduItemPart part = is_title_matched && is_desc_matched ? BaiduItemPart.TitleAbstract :
                is_title_matched ? BaiduItemPart.Title :
                is_desc_matched ? BaiduItemPart.Abstract : BaiduItemPart.None;

            /* 匹配发布时间 */
            Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
            string time = "";
            //先匹配搜索结果里是否有数据
            string timeStr = tag.SubAfter("newTimeFactor_before_abs").SubBefore("</span>");
            if (!string.IsNullOrEmpty(timeStr))
            {
                Match mt = reg.Match(timeStr);
                time = mt.Value;
            }
            else
            {
                //匹配网页源码里的时间
                MatchCollection mc = reg.Matches(detailHtml);
                if (mc.Count > 0)
                {
                    foreach (Match x in mc)
                    {
                        //判断是正文中的还是代码和注释中的时间
                        if (!string.IsNullOrEmpty(x.Value))
                        {
                            var txt = detailHtml.SubAfter(x.Value);
                            var index1 = txt.IndexOf('<');
                            var index2 = txt.IndexOf('>');
                            var index3 = txt.IndexOf('\"');
                            //只使用正文中的时间
                            if (index1 < index2 && index1 < index3)
                            {
                                time = x.Value;
                                break;
                            }
                        }
                    }
                }
            }

            //生成链接信息
            Dnl_Link_Baidu link = new Dnl_Link_Baidu
            {
                Domain = domain,
                TopDomain = GetLevel1Domain(domain),
                Keywords = tsk.Keyword,
                LinkUrl = realUrl,
                MatchAt = (byte)part,
                Html = detailHtml,
                SearchkeywordId = tsk._id.ToString(),
                CreatedAt = DateTime.UtcNow.AddHours(8),
                Description = description,
                Title = title,
                IsPromotion = isMarket,
                PublishTime = time,
                Content = content,
                DCNum = collectionNum
            };

            if (baiduVStar.HasValue)
            {
                link.BaiduVStar = baiduVStar.Value;
            }

            SaveLink(link, tsk);
        }

        /// <summary>
        /// 保存链接
        /// </summary>
        /// <param name="link">链接</param>
        /// <param name="task">关键词</param>
        public void SaveLink(Dnl_Link_Baidu link, Dnl_Keyword task)
        {
            //查询该链接是否已保存过
            var builder=Builders<Dnl_Link_Baidu>.Filter;
            var filter = builder.Eq(x => x.SearchkeywordId, task._id.ToString()) & builder.Eq(x => x.LinkUrl, link.LinkUrl);
            var col = MongoDBHelper.Instance.GetDnl_Link_Baidu();
            var query = col.Find(filter).FirstOrDefault();
            if (query != null)
            {
                Console.WriteLine(DateTime.Now + "  :  " + "该链接已保存 - " + task.Keyword);
            }
            else
            {
                Console.WriteLine(DateTime.Now + "  :  " + "成功保存1条链接 - " + task.Keyword);
                col.InsertOne(link);
            }
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="url">网页链接</param>
        /// <returns></returns>
        string get_html(string url)
        {
            var html = proxy.GetFastHtmlWithProxyIpAndARE(url, "utf-8").Trim();
            //if (!html.IsContains("c-container"))
            //    html = web.GetHtml(url, null);
            return html;
        }

        /// <summary>
        /// 获取网页源码及真实地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Tuple<string, string> get_htmlUrl(string url)
        {
            var tuplhtml = proxy.GetFastHtmlUrlWithProxyIpAndARE(url, "utf-8", 3, 3); //web.GetHtml2(url, null, "utf-8");

            string html = null;
            string realUrl = null;
            if (tuplhtml != null)
            {
                html = tuplhtml.Item2;
                realUrl = tuplhtml.Item1;
            }

            //if (realUrl == url)
            //{
            //    Encoding enc = Encoding.UTF8;

            //    int tryCount = 0;
            //    while (tryCount < 2)
            //    {
            //        realUrl = HtmlQueryHelper.GetRealUrl(url, 8000, ref enc);
            //        if (!string.IsNullOrEmpty(realUrl))
            //        {
            //            break;
            //        }
            //        tryCount++;
            //    }
            //    if (string.IsNullOrEmpty(realUrl))
            //    {
            //        realUrl = url;
            //    }
            //}

            return new Tuple<string, string>(realUrl, html);
        }

        /// <summary>
        /// 获取搜索网址
        /// </summary>
        /// <param name="tsk">要搜索的关键词信息</param>
        /// <returns></returns>
        string get_urls(Dnl_Keyword tsk)
        {

            string searchKeywords = tsk.Keyword.Trim();
            if (!string.IsNullOrEmpty(searchKeywords))
            {
                string baiduUrlFormat = "http://www.baidu.com/s?ie=utf-8&wd={0}";
                return baiduUrlFormat.FormatStr(searchKeywords.GetUrlEncodedString("utf-8"));
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取域名收录量
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        public long GetDomainCollectionNum(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return (long)0;
            }
            string url = "http://www.baidu.com/s?ie=utf-8&wd=site:{0}";
            url = url.FormatStr(domain.GetUrlEncodedString("utf-8"));
            string html = get_html(url);        //获取网页源码
            //解析并获取域名收录量
            if (html != null)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode collection = doc.DocumentNode.SelectSingleNode("//*[@id=\"1\"]/div/div[1]/div/p[3]/span/b");
                long num = 0;        //域名收录量
                if (collection != null)
                {
                    string numStr = collection.InnerText;
                    if (!string.IsNullOrEmpty(numStr))
                    {
                        numStr = numStr.Trim().Replace(",", "");
                        if (numStr.Contains("亿"))
                        {
                            if (numStr.Contains("万"))
                            {
                                int p1 = numStr.IndexOf("亿");
                                int p2 = numStr.IndexOf("万");
                                long a = Convert.ToInt32(numStr.SubBefore("亿"));
                                long b = Convert.ToInt32(numStr.SubAfter("亿").SubBefore("万"));
                                num = a * 100000000 + b * 10000;
                            }
                            else
                            {
                                int p1 = numStr.IndexOf("亿");
                                int p2 = numStr.IndexOf("万");
                                long a = Convert.ToInt32(numStr.SubBefore("亿"));
                                long b = Convert.ToInt32(numStr.SubAfter("亿"));
                                num = a * 100000000 + b;
                            }
                        }
                        else if (numStr.Contains("万"))
                        {
                            int p2 = numStr.IndexOf("万");
                            long a = Convert.ToInt32(numStr.SubBefore("万"));
                            long b = Convert.ToInt32(numStr.SubAfter("万"));
                            num = a * 10000 + b;
                        }
                        else
                        {
                            num = Convert.ToInt64(numStr);
                        }
                    }
                    return num;
                }
            }
            return (long)0;
        }


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

        static HashSet<char> chinese_commas = new HashSet<char> { '？', '。', '，', '！', '》', '《', '‘', '“', '；' };
        static HashSet<char> split_num_commas = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static HashSet<char> split_bef_commas = new HashSet<char> { '？', '。', '！', '；', '.', '>', '，', ',' };//'，', ','
        static HashSet<char> split_aft_commas = new HashSet<char> { '？', '。', '！', '；', '.', '<', '，', ',' };
        public static string RemoveInivalidChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (chinese_commas.Contains(c))
                    continue;
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c > 255)
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
