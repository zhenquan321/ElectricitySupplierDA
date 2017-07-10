
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
    public class IW2SBotMng
    {
        IW2SBotMng()
        {
            //var it = ProxyLib.IPPool.Instance; 
        }

        public static readonly IW2SBotMng Instance = new IW2SBotMng();


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
//                    var ipaddrs = System.Net.Dns.GetHostEntry(System.Environment.MachineName).AddressList;
//                    string ip = string.Empty;
//                    if (ipaddrs.Length >= 3)
//                    {
//                        ip = ipaddrs[2].ToString();
//                    }
//                    else if (string.IsNullOrEmpty(ip) && ipaddrs.Length >= 0)
//                    {
//                        ip = ipaddrs[0].ToString();
//                    }

                    //var internetIp = Utility.GetInternetIpAddress();
                    var botId = Utility.GenerateBotId().ToString().Replace("-", "");

                    //var pro = Process.GetCurrentProcess();
                    //string processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                    int botInterval = p.BotIntervalHours == 0 ? 7 * 24 : p.BotIntervalHours;
                    //var update = new UpdateDocument { { "$set", new QueryDocument { 
                    //{ "BotStatus", 1 }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)botInterval + 8) }
                    //,{"BotTag",string.Format("{0}#", processName)},
                    // {"BotId", botId}
                    //} } };
                    var update = new UpdateDocument { { "$set", new QueryDocument { 
                    { "BotStatus", 1 }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)botInterval + 8) }
                    , {"BotId", botId}
                    } } };
                    var result = MongoDBHelper.Instance.GetIW2S_BaiduCommends().UpdateOne(new QueryDocument { { "_id", p._id } }, update);

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
                {"BotStatus",2}} }};
                    var commendCol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
                    var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);

                    var builder = Builders<IW2S_level1link>.Filter;
                    var filter = builder.Eq(x => x.UsrId, p.UsrId);
                    filter &= builder.Eq(x => x.SearchkeywordId, p._id.ToString());
                    filter &= builder.Ne(x => x.DataCleanStatus, (byte)2);
                        filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/i")) |
                    builder.Regex(x => x.Description, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/i"));

                        var col = MongoDBHelper.Instance.GetIW2S_level1links();
                        var agreresult = col.Aggregate().Match(filter)
                            .Group(new BsonDocument { { "_id", "$_id" }, { "Count", new BsonDocument("$sum", 1) } })
                            .ToListAsync()
                            .Result;

                        var vallinkCount = agreresult.Count;
                        update = new UpdateDocument { { "$set", new QueryDocument { {"ValLinkCount",vallinkCount}} }};

                        commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                }
                catch (Exception ex)
                {
                    log("get_proj_to_qry ERROR ." + ex.Message);
                    Thread.Sleep(5000);
                }
            }

        }

        IW2S_BaiduCommend get_search_to_qry()
        {
            try
            {

                var dt = DateTime.UtcNow.AddHours(8);

                var builder = Builders<IW2S_BaiduCommend>.Filter;
                var filter = builder.Eq(x=>x.IsRemoved,false);
                filter &= builder.Eq(x=>x.BotStatus,0) | builder.Exists(x => x.BotStatus, false);
                //filter &= builder.Eq(x => x.SearchSource, 1);

                var col = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
                var result = col.Find(filter).SortByDescending(x=>x.CreatedAt).FirstOrDefault();//

                if (result == null)
                {
                    
                    //filter &= builder.Eq(x => x.BotStatus, 0) | builder.Exists(x => x.BotStatus, false);

                    result = col.Find(filter).SortByDescending(x => x.Times).FirstOrDefault();
                }
                if (result != null)
                {
                    Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get searchkeyword error: {0}".FormatStr(ex.Message));
            }

            return null;
           
        }

        public void query(IW2S_BaiduCommend p)
        {

            try
            {
                var builder = Builders<IW2S_ExcludeKeyword>.Filter;
                var filter = builder.Eq(x => x.UsrId, p.UsrId);

                var excludedKeywords = MongoDBHelper.Instance.GetIW2S_ExcludeKeywords().Find(builder.Empty).ToList();
                
                log("loaded {0} excluding keywords ".FormatStr(excludedKeywords == null ? 0 : excludedKeywords.Count));
                if (excludedKeywords.GetCount() > 0)
                    excludedKeywords.ForEach(x => x.Keyword = x.Keyword.ToLower());

                var filterbuilder = Builders<IW2S_KeywordFilter>.Filter;
                var filterfilter = filterbuilder.Eq(x => x.UsrId, p.UsrId) & filterbuilder.Eq(x => x.ProjectId, p.ProjectId);
                var filterKeywords = MongoDBHelper.Instance.GetIW2S_KeywordFilters().Find(filterfilter).Project(x => new IW2S_ExcludeKeyword
                {
                    Keyword = x.Keyword
                }).ToList();
                excludedKeywords.AddRange(filterKeywords);

                try
                {

                    Queries.IW2SBaiduQuery baidu = new Queries.IW2SBaiduQuery(p.Keyword );

                         baidu.Query(p,  excludedKeywords);
                        //save_level1_links(links, p, excludedKeywords);

                        //SogouWeixin sogou = new SogouWeixin(tsk.Keyword);
                        //links = sogou.Query(tsk);
                        //save_level1_links(links, tsk, excludedKeywords);
                    
                    
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
        

        public void save_level1_links(List<IW2S_level1link> links,  
            IW2S_BaiduCommend tsk, List<IW2S_ExcludeKeyword> excludedKeywords)
       {
            links = prehandle_data(links ,tsk ,excludedKeywords);
           
            if (links == null || links.Count == 0)
            {
                log("SUCCESS saving 0 Level 1 Links for " + tsk.CommendKeyword);   
                return;
            } 

            int pagesize = 100;
            int count = 0;
            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            var builder = Builders<IW2S_level1link>.Filter;
            for (int page = 0; page * pagesize < links.Count; page++)
            {
                var list = links.Skip(page * pagesize).Take(pagesize).ToList();
                //list.ForEach(x => x._id = new MongoDB.Bson.ObjectId(IDHelper.GetGuid("{0}/&itemid={1}".FormatStr(x.Domain, x.LinkUrl)).ToString()));
                list = ListDistinctBy(list, x => x.BizId);

                FieldsDocument fd = new FieldsDocument();
                fd.Add("BizId", 1);

                List<Guid> BizId = list.Select(x => x.BizId).ToList();
                //var exists_objs = col.Find(builder.In(x => x.BizId, BizId)).Project(x => x.BizId).ToList();
                List<Guid> exists_ids = new List<Guid>();
                //foreach (var result in exists_objs)
                //{
                //    exists_ids.Add(result);
                //}
                if(exists_ids !=null && exists_ids.Count >0)
                {
                    list = list.Where(x => !exists_ids.Contains(x.BizId)).ToList();
                }
                if(list==null || list.Count ==0)
                    continue ;
                count += pagesize;

                col.InsertMany(links);
                log("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.CommendKeyword);               
            }

        }

        public List<IW2S_level1link> prehandle_data(List<IW2S_level1link> links, IW2S_BaiduCommend tsk, List<IW2S_ExcludeKeyword> excludedKeywords)
        {

            if (links == null || links.Count == 0)
            {
                log("BLOCKED " + tsk.CommendKeyword);
                return links;
            }
            else
            {
                links = links.DistinctBy(x => x.LinkUrl);
                log(links.Count + " Level 1 Links for " + tsk.CommendKeyword); 
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



        private void cleaning(IW2S_level1link x, string keywords)
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

        private void cleaning(IW2S_level1link x, List<IW2S_ExcludeKeyword> excludedKeywords)
        {
            if(excludedKeywords == null || excludedKeywords.Count == 0)
            {
                return;
            }
            string txt = "{0}/{1}/{2}/{3}".FormatStr(x.Description, x.Abstract, x.Title, x.LinkUrl).ToLower();
            var matched_ex_kw = excludedKeywords.FirstOrDefault(k => txt.IsContains(k.Keyword));
            if (matched_ex_kw != null)
            {
                x.DataCleanStatus = (byte)DataCleanStatus.Excluded;
                x.Keywords = "{0} $ExcludedByExcludeKeyword:{1}".FormatStr(x.Keywords, matched_ex_kw.Keyword);
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

   
}
