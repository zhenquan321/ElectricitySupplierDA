
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AISSystem;
using System.Data;
using IWSBot.Queries;
using IWSData.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using MongoV2;

namespace IWSBot.Utility
{
    public class BotMng
    { 
        BotMng()
        {
            //var it = ProxyLib.IPPool.Instance; 
        }

        public static readonly BotMng Instance = new BotMng();
        
       
        public void Run()
        {           
            while (true)
            {
                Random r = new Random();
                var p = get_search_to_qry();
                if (p == null)
                { 
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }
                try
                {
                    var ipaddrs = System.Net.Dns.GetHostEntry(System.Environment.MachineName).AddressList;
                    string ip =string.Empty;
                    if(ipaddrs.Length >=3)
                    {
                        ip = ipaddrs[2].ToString();
                    }
                    else if (string.IsNullOrEmpty(ip) && ipaddrs.Length >= 0)
                    {
                        ip = ipaddrs[0].ToString();
                    }
                    var pro = Process.GetCurrentProcess();
                    string processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                    var update = new UpdateDocument { { "$set", new QueryDocument { 
                    { "IsBot", true }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)p.BotIntervalHours + 8) }
                    ,{"BotTag",string.Format("{0}#{1}",ip , processName)}
                    } } };

                    var result = MongoDBHelper.Instance.Getiws_searchkeywords().UpdateOne( new QueryDocument { { "_id", p._id } }, update);

                    query(p);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        log("baidu_query ERROR.Message:{0},Statck:{1}".FormatStr(ex.Message, ex.StackTrace));
                        ex = ex.InnerException;
                    }
                }
                //Convert.ToDateTime(doc["CreateTime"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                try
                {                    
                    
                    var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt",DateTime.UtcNow.AddHours(8)},
                {"IsBot",false}} }};

                    var result = MongoDBHelper.Instance.Getiws_searchkeywords().UpdateOne( new QueryDocument { { "_id", p._id } }, update);
            
                                    
                }
                catch (Exception ex)
                {
                    log("get_proj_to_qry ERROR ." + ex.Message); 
                    Thread.Sleep(5000);
                }
            }
        }

        Queue<searchkeyword> searchkeyword_pool  = new Queue<searchkeyword>();

        searchkeyword get_search_to_qry()
        {
            try
            {
                if (searchkeyword_pool.Count > 0)
                    return searchkeyword_pool.Dequeue();
                var dt = new DateTime(2015, 1, 1);

                var builder = Builders<searchkeyword>.Filter;
                var filter = builder.Eq(x => x.IsStarted, true);
                filter &= builder.Eq(x=>x.IsBot,false);
                //filter &= builder.Lte(x => x.NextBotStartAt, dt);

                var col = MongoDBHelper.Instance.Getiws_searchkeywords();
                var result = col.Find(filter).SortBy(x=>x.NextBotStartAt).FirstOrDefault();//

                if (result == null && DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 5)
                {
                    filter = builder.Eq(x => x.IsStarted, true);
                    //Query.EQ("IsBot", false),
                    filter &= builder.Lte(x => x.NextBotStartAt, dt);

                    result = col.Find(filter).SortBy(x => x.NextBotStartAt).FirstOrDefault();
                }
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

        private void query(searchkeyword p)
        {

            try
            {
                var builder = Builders<keyword>.Filter;
                var filter = builder.Eq(x => x.UsrId, p.UsrId);

                var excludedKeywords = MongoDBHelper.Instance.Getiws_excludekeywords().Find(filter).ToList();
                var businessKeywords = MongoDBHelper.Instance.Getiws_businesskeywords().Find(filter).ToList();

                log("loaded {0} excluding keywords ".FormatStr(excludedKeywords == null ? 0 : excludedKeywords.Count));
                if (excludedKeywords.GetCount() > 0)
                    excludedKeywords.ForEach(x => x.Txt = x.Txt.ToLower());

                try
                {
                    foreach (var busKey in businessKeywords)
                    {
                        Queries.BaiduQuery baidu = new Queries.BaiduQuery(p.Keyword+busKey.Txt);

                        var links = baidu.Query(p, busKey, businessKeywords, excludedKeywords);
                        //save_level1_links(links, p, excludedKeywords);

                        //SogouWeixin sogou = new SogouWeixin(tsk.Keyword);
                        //links = sogou.Query(tsk);
                        //save_level1_links(links, tsk, excludedKeywords);
                    }
                    
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }

        }
        

        public void save_level1_links(List<level1link> links,  
            searchkeyword tsk, List<keyword> excludedKeywords)
       {
            links = prehandle_data(links ,tsk ,excludedKeywords);
           
            if (links == null || links.Count == 0)
            {
                log("SUCCESS saving 0 Level 1 Links for " + tsk.Keyword);   
                return;
            } 

            int pagesize = 100;
            int count = 0;
            var col = MongoDBHelper.Instance.Getlevel1links();
            var builder = Builders<level1link>.Filter;
            for (int page = 0; page * pagesize < links.Count; page++)
            {
                var list = links.Skip(page * pagesize).Take(pagesize).ToList();
                //list.ForEach(x => x._id = new MongoDB.Bson.ObjectId(IDHelper.GetGuid("{0}/&itemid={1}".FormatStr(x.Domain, x.LinkUrl)).ToString()));
                list = ListDistinctBy(list, x => x.BizId);

                FieldsDocument fd = new FieldsDocument();
                fd.Add("BizId", 1);
                
                List<Guid> bizIds = list.Select(x => x.BizId).ToList();
                var exists_objs = col.Find(builder.In(x=>x.BizId, bizIds)).Project(x=>x.BizId).ToList();
                List<Guid> exists_ids = new List<Guid>();
                foreach (var result in exists_objs)
                {
                    exists_ids.Add(result);
                }
                if(exists_ids !=null && exists_ids.Count >0)
                {
                    list = list.Where(x => !exists_ids.Contains(x.BizId)).ToList();
                }
                if(list==null || list.Count ==0)
                    continue ;
                count += pagesize;

                col.InsertMany(links);
                log("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.Keyword);               
            }

        }

        public List<level1link> prehandle_data(List<level1link> links, searchkeyword tsk, List<keyword> excludedKeywords)
        {

            if (links == null || links.Count == 0)
            {                
                log("BLOCKED " + tsk.Keyword);
                return links;
            }
            else
            {
                links = links.DistinctBy(x => x.LinkUrl);
                log(links.Count + " Level 1 Links for " + tsk.Keyword); 
            }

            //var itm = MySqlDbHelper.GetEfEntities<protectitem>(ctx,"Id="+tsk.ProtectItemId).FirstOrDefault();
            ////{ScoredKeywords:{aaa:12,bbb:13}}
            //if (itm != null && !string.IsNullOrEmpty(itm.FingerPrints2))
            //{
            //    string[] sks = itm.FingerPrints2.SplitWith("$;");
            //    Dictionary<string, int> scores = new Dictionary<string, int>();
            //    if (sks != null)
            //    {
            //        foreach (var sk in sks)
            //        {
            //            string[] sps = sk.SplitWith(":", "：");
            //            if (sps == null || sps.Length != 2)
            //                continue;
            //            string k = sps[0].GetTrimed();
            //            int? s = sps[1].ExInt();
            //            if (sps[1].IsContains2("-"))
            //                s = -1 * s;
            //            if (string.IsNullOrEmpty(k) || !s.HasValue || scores.ContainsKey(k))
            //                continue;
            //            scores.Add(k, s.Value);
            //        }
            //    }
            //    foreach (var l in links)
            //    {
            //        string txt = string.Format("{0}{1}", l.Title, l.Abstract);
            //        l.Score = scores.Sum(x => txt.IsContain(x.Key) ? x.Value : 0);
            //        l.Title = "[使用了自定义打分]" + l.Title;
            //    }
            //}
            //else if (tsk.ProjectType == (byte)ProjectType.Artical && !string.IsNullOrEmpty(itm.FingerPrints))
            //{
            //    foreach (var l in links)
            //    {
            //        var txt = string.Join("", "{0},{1}".FormatStr(l.Title, l.Abstract).SplitWith(
            //            ";", ",", ";", ".", "，", "。", "；",
            //           "-", " ", "？", "“", "！", "”").Select(x => x.GetTrimed()).Where(x => !string.IsNullOrEmpty(x)));
            //        string[] fps = itm.FingerPrints.SplitWith(",");
            //        l.Score = l.Score / 2 + fps.Where(x => txt.IsContain(x)).Count() * 50 / fps.Length;
            //    }
            //}

            links.ForEach(x =>
            {
                //x.Keywords = tsk.Keyword;
                
                cleaning(x, excludedKeywords);
            });

            links = links.Where(x => x.DataCleanStatus != (byte)DataCleanStatus.Excluded).ToList();
            return links;
        }

        string remove(string txt, params string[] removeds)
        {
            if (removeds == null || removeds.Length == 0)
                return txt;
            foreach (var re in removeds)
            {
                txt = txt.ReplaceWith(re, "");
            }
            return txt;
        }

        

        private void cleaning(level1link x, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
                return;
            string[] kwds = keywords.GetLower().SplitWith(";", ",");
            if (kwds == null || kwds.Length == 0)
                return;
            string txt = "{0}/{1}/{2}".FormatStr(x.Keywords, x.Domain).ToLower();
            string matched = kwds.FirstOrDefault(y => !string.IsNullOrEmpty(y) && txt.IsContain(y));
            if (!string.IsNullOrEmpty(matched))
            {
                x.DataCleanStatus = (byte)DataCleanStatus.Excluded;
                x.Keywords = "{0} $ExcludedByExcludingKeyword:{1}".FormatStr(x.Keywords, matched);
            }            
        }

        private void cleaning(level1link x, List<keyword> excludedKeywords)
        {
            if(excludedKeywords == null || excludedKeywords.Count == 0)
            {
                return;
            }
            string txt = "{0}/{1}/{2}/{3}".FormatStr(x.Description, x.Abstract, x.Title, x.LinkUrl).ToLower();
            var matched_ex_kw = excludedKeywords.FirstOrDefault(k => txt.IsContains(k.Txt));
            if (matched_ex_kw != null)
            {
                x.DataCleanStatus = (byte)DataCleanStatus.Excluded;
                x.Keywords = "{0} $ExcludedByExcludeKeyword:{1}".FormatStr(x.Keywords, matched_ex_kw.Txt);
            }

            
        }
                

        bool match(string txt, string exc, string inc)
        {
            string[] sps = new string[] { ",", ";" };
            if (!string.IsNullOrEmpty(exc))
            {
                if (exc.SplitWith(sps).Any(k => txt.IsContain(k)))
                    return false;
            }
            if (!string.IsNullOrEmpty(inc))
            {
                if (!inc.SplitWith(sps).Any(k => txt.IsContain(k)))
                    return false;
            }
            return true;
        }

        
 
        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }

        public  List<T> ListDistinctBy<T>( List<T> collection, Func<T, object> selector)
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
    }

    public class IDGuidDto
    {
        public ObjectId _id { get; set; }
        public Guid BizId { get; set; }
    }
}
