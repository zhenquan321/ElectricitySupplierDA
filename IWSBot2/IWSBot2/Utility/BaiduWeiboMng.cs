using AISSystem;
using IWSBot.Utility;
using IWSData.Model;
using MongoDB.Driver;
using ProxyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoV2;
using MongoDB.Bson;

namespace IWSBot2.Utility
{    
    public class BaiduWeiboMng
    {
        public static readonly BaiduWeiboMng Instance = new BaiduWeiboMng();
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        int pages = 0;

        public delegate void UpdateStatus();

        public event UpdateStatus SetBusy;
        public event UpdateStatus SetReady;

        public void Run()
        {

            while (true)
            {
                Random r = new Random();
                var p = get_search_to_qry();
                if (p == null)
                {
                    SetReady();
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }

                try
                {
                    SetBusy();

                    //var internetIp = IWSBot.Utility.Utility.GetInternetIpAddress();
                    var botId = IWSBot.Utility.Utility.GenerateBotId().ToString().Replace("-", "");

                    var pro = Process.GetCurrentProcess();
                    string processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                    int botInterval = p.BotIntervalHours == 0 ? 7 * 24 : p.BotIntervalHours;
                    var update = new UpdateDocument { { "$set", new QueryDocument { 
                    { "BotStatus", 1 }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)botInterval + 8) }
                    ,{"BotTag",string.Format("{0}#", processName)},
                     {"BotId", botId}
                    } } };

                    var result = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends().UpdateOne(new QueryDocument { { "_id", p._id } }, update);

                    query(p);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Console.WriteLine("baidu_query ERROR.Message:{0},Statck:{1}".FormatStr(ex.Message, ex.StackTrace));
                        ex = ex.InnerException;
                    }
                }
                //Convert.ToDateTime(doc["CreateTime"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                try
                {

                    var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt",DateTime.UtcNow.AddHours(8)},
                {"BotStatus",2}} }};
                    var commendCol = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
                    var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);

                    var builder = Builders<IW2S_WB_level1link>.Filter;
                    var filter = builder.Eq(x => x.UsrId, p.UsrId);
                    filter &= builder.Eq(x => x.SearchkeywordId, p._id);
                    filter &= builder.Ne(x => x.DataCleanStatus, (byte)2);
                    filter &= builder.Regex(x => x.Description, new BsonRegularExpression("/.*" + p.Keyword + ".*/i"));

                    var col = MongoDBHelper.Instance.GetIW2S_WB_level1links();
                    var agreresult = col.Aggregate().Match(filter)
                        .Group(new BsonDocument { { "_id", "$_id" }, { "Count", new BsonDocument("$sum", 1) } })
                        .ToListAsync()
                        .Result;

                    var vallinkCount = agreresult.Count;
                    update = new UpdateDocument { { "$set", new QueryDocument { { "ValLinkCount", vallinkCount } } } };

                    commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("get_proj_to_qry ERROR ." + ex.Message);
                    Thread.Sleep(5000);
                }
            }
        }

        IW2S_WB_BaiduCommend get_search_to_qry()
        {
            try
            {

                var dt = DateTime.UtcNow.AddHours(8);

                var builder = Builders<IW2S_WB_BaiduCommend>.Filter;
                var filter = builder.Eq(x => x.IsRemoved, false);
                filter &= builder.Eq(x => x.BotStatus, 0) | builder.Exists(x => x.BotStatus, false);

                var col = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
                var result = col.Find(filter).FirstOrDefault();

                if (result != null)
                {
                    Console.WriteLine("start to search {0}".FormatStr(result.Keyword));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get searchkeyword error: {0}".FormatStr(ex.Message));
            }

            return null;

        }

        private void query(IW2S_WB_BaiduCommend p)
        {
            //http://www.baidu.com/s?rtt=2&tn=baiduwb&wd=%E8%80%81%E9%85%B8%E5%A5%B6
            try
            {
                string link = "http://www.baidu.com/s?rtt=2&tn=baiduwb&wd={0}".FormatStr(p.Keyword);
                                
                int nohist_pages = 0;
                int quried_pages = 0;
                int rank = 1;
                //最多搜索60页 
                while (!string.IsNullOrEmpty(link) && quried_pages <= 3)
                {
                    Console.WriteLine(link);
                    var html = proxy.GetFastHtmlWithProxyIpAndARE(link, "utf-8").RemoveSpace();
                    if (html == null)
                        break;
                    DateTime now = DateTime.Now;           
                    var tags = html.SubAfter("id=\"weibo\"").SubBefore("</ol>").SplitWith("<li");

                    if (tags == null || tags.Length == 0)
                    {
                        Console.WriteLine("BLOCKED " + p.Keyword);
                        break;
                    }
                    bool nohit = true;
                    foreach (string tag in tags)
                    {
                        var a = tag.SubAfter("weibo_detail");
                        string nickName = RemoveInivalidChar(
                            a.RemoveSpace().GetLower().SubBefore("</a>").GetTxtFromHtml2().RemoveSpace().GetLower());
                        string PosterUrl = tag.GetFirstHref2();
                        string weibo_face = tag.GetFirstAttributeValue("img", "src");

                        string abs = tag.SubBefore("weibo_all").GetTxtFromHtml2().GetLower();
                        string postUrl = a.SubAfter("weibo_all").GetFirstAttributeValue("href");
                        
                        //没有包含需要protect item信息的过滤掉
                        string txt = "{0}{1}".FormatStr(PosterUrl, abs);
                        if (string.IsNullOrEmpty(txt))
                            continue;
                        IW2S_WB_level1link linkData = new IW2S_WB_level1link();

                        string PublishDate = tag.SubAfter("weibo_info").GetTxtFromHtml2().RemoveSpace().GetLower();
                        int interval = 0; string intervalStr = "";
                        if (PublishDate.Contains("小时前"))
                        {
                            intervalStr = PublishDate.SubAfter("评论").SubAfter(")").SubBefore("小时前");//
                            int.TryParse(intervalStr, out interval);
                            linkData.PublishTime = now.AddHours(interval * (-1)).Date.AddHours(8);
                        }
                        else if (PublishDate.Contains("分钟前"))
                        {
                            intervalStr = PublishDate.SubAfter("评论").SubAfter(")").SubBefore("分钟前");//
                            int.TryParse(intervalStr, out interval);
                            linkData.PublishTime = now.AddMinutes(interval * (-1)).Date.AddHours(8);
                        }
                        else if (PublishDate.Contains("天前"))
                        {
                            intervalStr = PublishDate.SubAfter("评论").SubAfter(")").SubBefore("天前");//
                            int.TryParse(intervalStr, out interval);
                            linkData.PublishTime = now.AddDays(interval * (-1)).Date.AddHours(8);
                        }
                        linkData.PosterUrl = PosterUrl;
                        linkData.PostUrl = postUrl;
                        linkData.HeadIcon = weibo_face;
                        linkData.NickName = nickName;
                        linkData.Description = abs;
                        linkData.IsBlueV = a.Contains("weibo_level_icon");
                        linkData.UsrId = p.UsrId;
                        linkData.Keywords = p.Keyword;
                        linkData.CreatedAt = DateTime.Now.AddHours(8);
                        linkData.IsDel = false;
                        linkData.ProjectId = p.ProjectId;
                        linkData.Rank = rank;
                        linkData.SearchkeywordId = p._id;
                        linkData.BizId = "{0}{1}".FormatStr(postUrl, p._id.ToString()).ToObjectId();

                        save_level1_links(new List<IW2S_WB_level1link> { linkData }, p);

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
                    link = html.SubAfter("id=\"page\"").SubBefore("下一页").GetLastHref2();
                    if (!string.IsNullOrEmpty(link) && !link.IsStartWith("http"))
                    {
                        if (link.IsStartWith("/"))
                            link = link.SubAfter("/");
                        link = "http://www.baidu.com/".GetContact(link);
                    }

                }
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public void save_level1_links(List<IW2S_WB_level1link> links,
            IW2S_WB_BaiduCommend tsk)
        {

            if (links == null || links.Count == 0)
            {
                Console.WriteLine("SUCCESS saving 0 Level 1 Links for " + tsk.Keyword);
                return;
            }

            int pagesize = 100;
            int count = 0;
            var col = MongoDBHelper.Instance.GetIW2S_WB_level1links();
            var builder = Builders<IW2S_WB_level1link>.Filter;
            for (int page = 0; page * pagesize < links.Count; page++)
            {
                var list = links.Skip(page * pagesize).Take(pagesize).ToList();
                //list.ForEach(x => x._id = new MongoDB.Bson.ObjectId(IDHelper.GetGuid("{0}/&itemid={1}".FormatStr(x.Domain, x.LinkUrl)).ToString()));
                list = ListDistinctBy(list, x => x.BizId);

                FieldsDocument fd = new FieldsDocument();
                fd.Add("BizId", 1);

                List<ObjectId> bizIds = list.Select(x => x.BizId).ToList();
                var exists_objs = col.Find(builder.In(x => x.BizId, bizIds)).Project(x => x.BizId).ToList();
                List<ObjectId> exists_ids = new List<ObjectId>();
                foreach (var result in exists_objs)
                {
                    exists_ids.Add(result);
                }
                if (exists_ids != null && exists_ids.Count > 0)
                {
                    list = list.Where(x => !exists_ids.Contains(x.BizId)).ToList();
                }
                if (list == null || list.Count == 0)
                    continue;
                count += pagesize;

                col.InsertMany(links);
                Console.WriteLine("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.Keyword);
            }

        }

        public List<T> ListDistinctBy<T>(List<T> collection, Func<T, object> selector)
        {
            if (collection == null)
                return null;
            List<T> list = new List<T>();
            var gs = collection.GroupBy(x => selector(x));
            foreach (var g in gs)
            {
                list.Add(g.First());
            }
            return list;
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


            return new Tuple<string, string>(realUrl, html);
        }

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

        static readonly string[] domain_sufixes = new string[]{  ".cn",".la",".top",".so",".biz",".name",".info",".cc",".tv",".中国",".mobi",".me",".asia",
            ".co",".tel",".公司",".网络",".wang",".net",".org",".edu",".gov",".com"};
        static readonly string[] domain_sps = new string[] { "/", "?", " ", "." };

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
