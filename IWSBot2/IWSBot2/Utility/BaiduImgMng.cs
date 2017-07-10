using System.Web.UI.WebControls;
using ProxyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using IWSBot.Utility;
using MongoDB.Driver;
using IWSData.Model;
using System.Threading;
using System.Diagnostics;
using MongoV2;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using System.Runtime.Serialization.Json;
using System.IO;

namespace IWSBot2.Utility
{
    public class BaiduImgMng
    {
        public static readonly BaiduImgMng Instance = new BaiduImgMng();
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

                    //SetReady();
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }

                try
                {
                    //SetBusy();

                    //var internetIp = IWSBot.Utility.Utility.GetInternetIpAddress();

                    //var botId = IWSBot.Utility.Utility.GenerateBotId().ToString().Replace("-", "");

                    //var pro = Process.GetCurrentProcess();
                    //string processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                    //int botInterval = p.BotIntervalHours == 0 ? 7 * 24 : p.BotIntervalHours;
                    //var update = new UpdateDocument { { "$set", new QueryDocument { 
                    //{ "BotStatus", 1 }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)botInterval + 8) }
                    //,{"BotTag",string.Format("{0}#",processName)},
                    // {"BotId", botId}
                    //} } };

                    //var result = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks().UpdateOne(new QueryDocument { { "_id", p._id } }, update);

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
                    var commendCol = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks();
                    var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("get_proj_to_qry ERROR ." + ex.Message);
                    Thread.Sleep(5000);
                }
            }
        }

        IW2S_ImgSearchTask get_search_to_qry()
        {
            try
            {

                var dt = DateTime.UtcNow.AddHours(8);

                var builder = Builders<IW2S_ImgSearchTask>.Filter;
                var filter = builder.Eq(x => x.IsDel, false);
                //filter &= builder.Eq(x => x._id,new ObjectId("58d9e7f07c082505c02a81ca"));
                filter &= builder.Eq(x => x.BotStatus, 2) | builder.Exists(x => x.BotStatus, false);

                var col = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks();
                var result = col.Find(filter).FirstOrDefault();
                if(result == null)
                {
                    filter = builder.Eq(x => x.IsDel, false);
                    filter &= builder.Eq(x => x.BotStatus, 1);

                    result = col.Find(filter).FirstOrDefault();
                }
                if (result != null)
                {
                    Console.WriteLine("start to search {0}".FormatStr(result.Src));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get searchkeyword error: {0}".FormatStr(ex.Message));
            }

            return null;

        }

        private void query(IW2S_ImgSearchTask p)
        {
            //http://www.baidu.com/s?rtt=2&tn=baiduwb&wd=%E8%80%81%E9%85%B8%E5%A5%B6
            try
            {
                string http = "http://211.154.6.166:9000";
                //string baiduUrl = "http://image.baidu.com/n/pc_search?queryImageUrl=http://a.hiphotos.baidu.com/image/pic/item/f9dcd100baa1cd1162eeea1ab112c8fcc3ce2dab.jpg"
                string link = "http://image.baidu.com/n/pc_list?queryImageUrl={0}&pos=moresource#activeTab=1".FormatStr(p.Src);
                //string link = "http://image.baidu.com/n/pc_search?queryImageUrl={0}".FormatStr(p.Src);
                
                    Console.WriteLine(link);
                    var html = proxy.GetFastHtmlWithProxyIpAndARE(link, "utf-8").RemoveSpace();
                    if (html == null)
                    {
                        var update = new UpdateDocument { { "$set", new QueryDocument {  {"BotStatus",0}} }};
                        var commendCol = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks();
                        var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                        return;
                    }
                    var json = "[{"+html.SubAfter("'sameList':").SubBefore("'sameSizeNum':").SubAfter("[{").SubBefore("}]")+"}]";
                    var objImgs = JsonToObject(json);

                    int rank = 1;
                    foreach(var objImg in objImgs)
                    {
                        objImg.fromPageTitle = RemoveInivalidChar(objImg.fromPageTitle.GetTxtFromHtml2().RemoveSpace().GetLower());
                        objImg.textHost = RemoveInivalidChar(objImg.textHost.GetTxtFromHtml2().RemoveSpace().GetLower());
                        HanleTagData(p, objImg.fromPageTitle, objImg.fromURL, objImg.textHost, objImg.fromURLHost, objImg.objURL, rank);
                        rank++;
                    }   
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private List<ImgListDto> JsonToObject(string jsonString)
        {
            List<ImgListDto> result = null;
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<ImgListDto>));
                MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                result = serializer.ReadObject(mStream) as List<ImgListDto>;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }

            return result;
        }

        private void HanleTagData(IW2S_ImgSearchTask tsk, string title, string href, string abs, string domain,string src, int rank)
        {
          
            string realUrl = null, detailHtml = null;
            

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
            if(string.IsNullOrEmpty(detailHtml))
            {
                return;
            }
                        
            Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
            Match m = reg.Match(detailHtml);
            //MatchCollection cols = reg.Matches(item.Html);
            string time = "";
            if (m.Groups.Count > 0)
            {
                time = m.Groups[0].Value;
            }


            IW2S_ImgSearchLink l1 = new IW2S_ImgSearchLink
            {
                UsrId = tsk.UsrId,
                Domain = domain,
                TopDomain = GetLevel1Domain(domain),
                Src = src,
                LinkUrl = href,

                BizId = "{0}{1}".FormatStr(href, tsk._id.ToString()).ToObjectId(),
                IW2S_ImgSearchTaskId = tsk._id,
                CreatedAt = DateTime.UtcNow.AddHours(8),
                Description = abs,
                Title = title,
                
                ProjectId = tsk.ProjectId,
                PublishTime = time,
                
                Rank = rank
            };

            

            save_level1_links(new List<IW2S_ImgSearchLink> { l1 }, tsk);
        }

        public void save_level1_links(List<IW2S_ImgSearchLink> links,
            IW2S_ImgSearchTask tsk)
        {
            
            if (links == null || links.Count == 0)
            {
                Console.WriteLine("SUCCESS saving 0 Level 1 Links for " + tsk.Src);
                return;
            }

            int pagesize = 100;
            int count = 0;
            var col = MongoDBHelper.Instance.GetIW2S_ImgSearchLinks();
            var builder = Builders<IW2S_ImgSearchLink>.Filter;
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
                Console.WriteLine("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.Src);
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

    public class ImgListDto
    {
        public string thumbURL { get;set; }
        public string fromURL { get; set; }
        public string fromPageTitle { get; set; }
        public string textHost { get; set; }
        public string fromURLHost { get; set; }
        public string objURL { get; set; }
    }
}
