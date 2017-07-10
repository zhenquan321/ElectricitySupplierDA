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
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace IWSBot.Queries
{
    public class IW2SBaiduQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        //HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;

        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;


        public IW2SBaiduQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);

            }
        }

        public void Query(IW2S_BaiduCommend tsk, List<IW2S_ExcludeKeyword> excludedKeywords)
        {
            List<IW2S_level1link> result = new List<IW2S_level1link>();

            var link = get_urls(tsk);

            try
            {
                GetLinks(link, tsk, excludedKeywords);
                //if (list != null && list.Count > 0)
                //    result.AddRange(list);
            }
            catch (Exception ex)
            {
                log(ex.Message + ex.StackTrace);
            }


        }

        void GetLinks(string link, IW2S_BaiduCommend tsk, List<IW2S_ExcludeKeyword> excludedKeywords)
        {
            IW2SBotMng botmng = IW2SBotMng.Instance;

            string[] searchKeywords = tsk.CommendKeyword.GetLower().Trim().Split(';');

            List<KeywordScore> patterns = new List<KeywordScore>();

            patterns.Add(new KeywordScore { Keyword = tsk.CommendKeyword });

            //List<level1link> result = new List<level1link>(); 
            int nohist_pages = 0;
            int quried_pages = 0;
            int rank = 1;
            //最多搜索60页 
            while (!string.IsNullOrEmpty(link) && quried_pages <= 3)
            {
                log(link);
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
                    string title = a.SubBefore("</h3>").GetTxtFromHtml2();
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = title.Trim();
                    }
                    string href = a.GetFirstHref2();

                    string abs = tag.SubAfter("</h3>").SubBefore("</a").GetTxtFromHtml2();
                    if (string.IsNullOrEmpty(abs))
                    {
                        abs = abs.Trim();
                    }
                    string domain = string.Empty;


                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0}{1}".FormatStr(title, abs);
                    if (string.IsNullOrEmpty(txt))
                        continue;



                    HanleTagData(tsk, excludedKeywords, botmng, searchKeywords, patterns, title, href, abs, ref domain, tag, true,rank);
                }

                var tags = html.SubAfter("content_left").SplitWith("c-container");

                if (tags == null || tags.Length == 0)
                {
                    log("BLOCKED " + tsk.CommendKeyword);
                    break;
                }
                bool nohit = true;
                foreach (string tag in tags)
                {
                    var a = tag.SubAfter("h3").SubAfter("a");
                    string title = a.SubBefore("</h3>").GetTxtFromHtml2();
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = title.Trim();
                    }
                    string href = a.GetFirstHref2();


                    string abs = tag.SubAfter("abstract").SubBefore("</div").GetTxtFromHtml2();
                    if (string.IsNullOrEmpty(abs))
                    {
                        abs = abs.Trim();
                    }
                    string domain = tag.SubLastStringAfter("\"f13").SubBefore("</span").GetTxtFromHtml2();
                    domain = GetDomain(domain);


                    //没有包含需要protect item信息的过滤掉
                    string txt = "{0}{1}".FormatStr(title, abs);
                    if (string.IsNullOrEmpty(txt))
                        continue;

                    HanleTagData(tsk, excludedKeywords, botmng, searchKeywords, patterns, title, href, abs, ref domain, tag, false,rank);
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

        private void HanleTagData(IW2S_BaiduCommend tsk, List<IW2S_ExcludeKeyword> excludedKeywords, IW2SBotMng botmng, string[] searchKeywords, List<KeywordScore> patterns, string title, string href, string abs, ref string domain, string tag, bool isMarket,int rank)
        {
            int maxScore = 0;
            string realUrl = null, detailHtml = null, abstracts = null;
            byte appType = 0;

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

            if (!string.IsNullOrWhiteSpace(href))
            {
                //Encoding enc = Encoding.UTF8;
                //detailHtml = HtmlQueryHelper.GetContent(href, 8000, ref enc, out realUrl);
                var tuplehtml = get_htmlUrl(href);
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
                //if (!detailHtml.Contains(tsk.CommendKeyword))
                //{
                //    return;
                //}
                var hrefs = detailHtml.GetDescendents("a", "href");
                StringBuilder sbabstracts = new StringBuilder();
                List<string> abstractlist = new List<string>();
                StringBuilder sbabstractlist = new StringBuilder();

                foreach (KeywordScore pattern in patterns)
                {
                    string[] splitDetailHtmls = detailHtml.SplitWith(pattern.Keyword);
                    if (splitDetailHtmls.Length > 1)
                    {
                        matchpatterns.Add(pattern);
                    }
                    StringBuilder sbpatternStr = new StringBuilder();
                    for (int i = 0; i < splitDetailHtmls.Length - 1; i++)
                    {

                        string splitDetailHtml1 = splitDetailHtmls[i];
                        string splitDetailHtml2 = i < splitDetailHtmls.Length - 2 ? splitDetailHtmls[i + 1] : "";
                        for (int j = splitDetailHtml1.Length - 1; j >= 0; j--)
                        {
                            if (split_bef_commas.Contains(splitDetailHtml1[j]) && j - 1 >= 0 && !split_num_commas.Contains(splitDetailHtml1[j - 1]))
                            { break; }
                            sbpatternStr.Append(splitDetailHtml1[j]);
                        }
                        for (int q = sbpatternStr.Length - 1; q >= 0; q--)
                        {
                            sbabstracts.Append(sbpatternStr[q]);
                        }
                        sbabstracts.Append(pattern.Keyword);
                        sbpatternStr.Clear();
                        for (int j = 0; j < splitDetailHtml2.Length; j++)
                        {
                            if (split_aft_commas.Contains(splitDetailHtml2[j]) && j + 1 < splitDetailHtml2.Length && !split_num_commas.Contains(splitDetailHtml2[j + 1]))
                            { break; }
                            sbpatternStr.Append(splitDetailHtml2[j]);
                        }
                        sbabstracts.Append(sbpatternStr);
                        sbpatternStr.Clear();

                        string tmpsbabstracts = sbabstracts.ToString();
                        tmpsbabstracts = IW2SBaiduQuery.RemoveInivalidChar(tmpsbabstracts.GetTxtFromHtml2().RemoveSpace().GetLower());
                        if (!abstractlist.Contains(tmpsbabstracts))
                        {
                            abstractlist.Add(tmpsbabstracts);
                            sbabstractlist.Append(tmpsbabstracts).Append(" ");
                        }
                        sbabstracts.Clear();
                    }
                }
                //获取摘要
                abstracts = sbabstractlist.ToString();
                if (!string.IsNullOrEmpty(abstracts) && matchpatterns.Count > 0)
                {
                    maxScore = matchpatterns.Max(x => x.Score ?? 50);
                    appType = matchpatterns.Where(x => x.BizType > 0).OrderByDescending(x => x.Score).Select(x => x.BizType).FirstOrDefault();
                    maxScore += matchpatterns.Sum(x => (x.Score ?? 50) / 10);
                    maxScore -= matchpatterns.Max(x => (x.Score ?? 50) / 10);
                }
            }
            if (string.IsNullOrEmpty(abstracts) && !string.IsNullOrEmpty(abs))
            {
                matchpatterns = patterns.Where(x => abs.Contains(x.Keyword)).ToList();
                if (matchpatterns.Count > 0)
                {
                    maxScore = matchpatterns.Max(x => x.Score ?? 50);
                    appType = matchpatterns.Where(x => x.BizType > 0).OrderByDescending(x => x.Score).Select(x => x.BizType).FirstOrDefault();

                    maxScore += matchpatterns.Sum(x => (x.Score ?? 50) / 10);
                    maxScore -= matchpatterns.Max(x => (x.Score ?? 50) / 10);
                }
            }
            if (maxScore > 100)
            {
                maxScore = 100;
            }


            bool is_title_matched = title.GetLower().IsContains2(searchKeywords);
            bool is_abstr_matched = abs.IsContains2(searchKeywords);
            BaiduItemPart part = is_title_matched && is_abstr_matched ? BaiduItemPart.TitleAbstract :
                is_title_matched ? BaiduItemPart.Title :
                is_abstr_matched ? BaiduItemPart.Abstract : BaiduItemPart.None;

            Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
            MatchCollection mc = reg.Matches(detailHtml);
            //MatchCollection cols = reg.Matches(item.Html);
            string time = "";
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


            IW2S_level1link l1 = new IW2S_level1link
            {
                UsrId = tsk.UsrId,
                Domain = domain,
                TopDomain = GetLevel1Domain(domain),
                Keywords = string.Format("{0}", tsk.CommendKeyword),
                LinkUrl = realUrl,
                MatchAt = (byte)part,
                Html = detailHtml,

                AppType = appType,
                BizId = IDHelper.GetGuid("{0}/{1}".FormatStr(realUrl, tsk._id.ToString())),
                SearchkeywordId = tsk._id.ToString(),
                CreatedAt = DateTime.UtcNow.AddHours(8),
                Description = abs,
                Title = title,
                Score = maxScore,
                Abstract = abstracts,
                IsMarket = isMarket,
                ProjectId = tsk.ProjectId,
                PublishTime = time,
                AlternateFields = "0",
                Rank = rank
            };

            if (baiduVStar.HasValue)
            {
                l1.BaiduVStar = baiduVStar.Value;
            }

            botmng.save_level1_links(new List<IW2S_level1link> { l1 }, tsk, excludedKeywords);
        }

        string get_html(string url)
        {
            var html = proxy.GetFastHtmlWithProxyIpAndARE(url, "utf-8").Trim();
            //if (!html.IsContains("c-container"))
            //    html = web.GetHtml(url, null);
            return html;
        }

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

        string get_urls(IW2S_BaiduCommend tsk)
        {

            string searchKeywords = tsk.CommendKeyword.Trim();
            //searchKeywords = searchKeywords.Replace(" ", "%20");
            //string searchKeywords = tsk.CommendKeyword.Trim().GetLower();
            if (!string.IsNullOrEmpty(searchKeywords))
            {
                string baiduUrlFormat = "http://www.baidu.com/s?ie=utf-8&wd={0}";
                return baiduUrlFormat.FormatStr(searchKeywords.GetUrlEncodedString("utf-8"));

            }
            return string.Empty;
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
