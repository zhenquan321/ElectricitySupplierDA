
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
    /// <summary>
    /// 百度搜索Bot
    /// </summary>
    public class BaiduSearchMng
    {
        BaiduSearchMng()
        {
            //var it = ProxyLib.IPPool.Instance; 
        }

        /// <summary>
        /// 静态实体，仅供调用
        /// </summary>
        public static readonly BaiduSearchMng Instance = new BaiduSearchMng();

        public delegate void UpdateStatus();
        /// <summary>
        /// 设置Bot为工作状态，供控制台查看
        /// </summary>
        public event UpdateStatus SetBusy;
        /// <summary>
        /// 设置Bot为空闲状态，供控制台查看
        /// </summary>
        public event UpdateStatus SetReady;

        public void Run()
        {
            while (true)
            {
                Random r = new Random();
                //获取搜索关键词信息
                var p = get_search_to_qry();
                //若没有需要搜索的关键词，进行休眠
                if (p == null)
                {
                    SetReady();
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }

                try
                {
                    SetBusy();
                    var botId = Utility.GenerateBotId().ToString().Replace("-", "");                //获取BotId
                    int botInterval = p.BotIntervalHours == 0 ? 7 * 24 : p.BotIntervalHours;        //获取搜索间隔
                    //更新关键词状态为正在搜索
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "BotStatus_Baidu", 1 } } } };
                    var result = MongoDBHelper.Instance.GetDnl_Keyword().UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                    //搜索关键词
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
                    //更新关键词状态为已完成
                    var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt_Baidu",DateTime.UtcNow.AddHours(8)},{"BotStatus_Baidu",2}} }};
                    var commendCol = MongoDBHelper.Instance.GetDnl_Keyword();
                    var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                    //更新关键词有效链接数
                    var builder = Builders<Dnl_Link_Baidu>.Filter;
                    var filter = builder.Eq(x => x.SearchkeywordId, p._id.ToString());
                    //    filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + p.Keyword + ".*/i")) |
                    //builder.Regex(x => x.Description, new BsonRegularExpression("/.*" + p.Keyword + ".*/i"));

                        //var col = MongoDBHelper.Instance.GetIW2S_level1links();
                        //var agreresult = col.Aggregate().Match(filter)
                        //    .Group(new BsonDocument { { "_id", "$_id" }, { "Count", new BsonDocument("$sum", 1) } })
                        //    .ToListAsync()
                        //    .Result;

                        //var vallinkCount = agreresult.Count;
                    int linkCount = Convert.ToInt32(MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filter).Count());    //获取全部链接数
                    filter &= builder.Eq(x => x.MatchAt, 4);
                    int vallinkCount = Convert.ToInt32(MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filter).Count());
                    update = new UpdateDocument { { "$set", new QueryDocument { { "ValLinkCount_Baidu", vallinkCount }, { "LinkCount_Baidu", linkCount } } } };

                    commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                }
                catch (Exception ex)
                {
                    log("get_proj_to_qry ERROR ." + ex.Message);
                    Thread.Sleep(5000);
                }
            }

        }

        /// <summary>
        /// 获取要搜索的关键词信息
        /// </summary>
        /// <returns></returns>
        Dnl_Keyword get_search_to_qry()
        {
            try
            {
                //Dnl_Keyword tp = new Dnl_Keyword
                //{
                //    _id = ObjectId.GenerateNewId(),
                //    Keyword = "刘图耻"
                //};
                //return tp;

                //查询未被搜索的关键词
                var dtNow = DateTime.UtcNow.AddHours(8);
                var builder = Builders<Dnl_Keyword>.Filter;;
                var filter = builder.Eq(x => x.BotStatus_Baidu, 0) | builder.Exists(x => x.BotStatus_Baidu, false);

                var col = MongoDBHelper.Instance.GetDnl_Keyword();
                var result = col.Find(filter).SortByDescending(x=>x.CreatedAt).FirstOrDefault();

                //如果没有未被搜索的关键词，则查询需要重新搜索的关键词
                if (result == null)
                {
                    ////获取最早被搜索的关键词
                    //filter = builder.Eq(x => x.BotStatus_Baidu, 2);
                    //var keyword = col.Find(filter).SortBy(x => x.LastBotEndAt_Baidu).FirstOrDefault();
                    ////判断是否已到需被重新搜索的时间
                    //DateTime nextBot = keyword.LastBotEndAt_Baidu.AddHours(keyword.BotIntervalHours);
                    //if (nextBot < dtNow)
                    //{
                    //    Console.WriteLine("开始搜索： {0}".FormatStr(keyword.Keyword));
                    //    return keyword;
                    //}
                }
                if (result != null)
                {
                    Console.WriteLine("开始搜索： {0}".FormatStr(result.Keyword));
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取搜索关键词出错： {0}".FormatStr(ex.Message));
            }

            return null;
           
        }

        /// <summary>
        /// 百度搜索关键词
        /// </summary>
        /// <param name="p"></param>
        public void query(Dnl_Keyword p)
        {

            try
            {
                //获取要过滤的域名
                var builder = Builders<Dnl_IgnoreDomain>.Filter;
                var excludedDomains = MongoDBHelper.Instance.GetDnl_IgnoreDomain().Find(builder.Empty).ToList();

                //log("加载 {0} 个排除关键词 ".FormatStr(excludedKeywords == null ? 0 : excludedKeywords.Count));

                //var filterbuilder = Builders<IW2S_KeywordFilter>.Filter;
                //var filterfilter = filterbuilder.Eq(x => x.UsrId, p.UsrId) & filterbuilder.Eq(x => x.ProjectId, p.ProjectId);
                //var filterKeywords = MongoDBHelper.Instance.GetIW2S_KeywordFilters().Find(filterfilter).Project(x => new IW2S_ExcludeKeyword
                //{
                //    Keyword = x.Keyword
                //}).ToList();
                //excludedKeywords.AddRange(filterKeywords);

                try
                {
                    //搜索关键词
                    Queries.DnlBaiduSearchQuery baidu = new Queries.DnlBaiduSearchQuery(p.Keyword );
                    baidu.Query(p, excludedDomains);
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
        
        /// <summary>
        /// 保存链接信息
        /// </summary>
        /// <param name="link">链接类</param>
        /// <param name="tsk">关键词类</param>
        /// <param name="excludedKeywords">排除词</param>
        public void save_level1_links(List<IW2S_level1link> links, Dnl_Keyword tsk, List<Dnl_IgnoreDomain> excludedKeywords)
       {
           //link = prehandle_data(link ,tsk ,excludedKeywords);
           try
           {
               if (links != null)
               {
                   log("成功保存1条链接： " + tsk.Keyword);
                   return;
               }

               int pagesize = 100;
               int count = 0;
               var col = MongoDBHelper.Instance.GetIW2S_level1links();
               var builder = Builders<Dnl_Link_Baidu>.Filter;
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
                   if (exists_ids != null && exists_ids.Count > 0)
                   {
                       list = list.Where(x => !exists_ids.Contains(x.BizId)).ToList();
                   }
                   if (list == null || list.Count == 0)
                       continue;
                   count += pagesize;

                   col.InsertMany(links);

                   log("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.Keyword);
               }
           }
           catch (Exception ex)
           {
               log(ex.Message);
               log("保存出错！");
           }

        }

        ///// <summary>
        ///// 判断链接是否需要被排除
        ///// </summary>
        ///// <param name="link">链接类</param>
        ///// <param name="tsk">关键词类</param>
        ///// <param name="excludedKeywords">排除词</param>
        ///// <returns></returns>
        //public List<IW2S_level1link> prehandle_data(Dnl_Link_Baidu link, Dnl_Keyword tsk, List<IW2S_ExcludeKeyword> excludedKeywords)
        //{
        //        log(" 保存1条链接： " + tsk.Keyword); 

        //    links.ForEach(x =>
        //    {
        //        //x.Keywords = tsk.Keyword;
                
        //        cleaning(x, excludedKeywords);
        //    });

        //    links = links.Where(x => x.DataCleanStatus != (byte)DataCleanStatus.Excluded).ToList();
        //    return links;
        //}

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



        //private void cleaning(Dnl_Link_Baidu x, string keywords)
        //{
        //    if (string.IsNullOrEmpty(keywords))
        //        return;
        //    string[] kwds = keywords.GetLower().SplitWith(";", ",");
        //    if (kwds == null || kwds.Length == 0)
        //        return;
        //    string txt = "{0}/{1}/{2}".FormatStr(x.Keywords, x.Domain).ToLower();
        //    string matched = kwds.FirstOrDefault(y => !string.IsNullOrEmpty(y) && txt.IsContain(y));
        //    if (!string.IsNullOrEmpty(matched))
        //    {
        //        x.DataCleanStatus = (byte)DataCleanStatus.Excluded;
        //        x.Keywords = "{0} $ExcludedByExcludingKeyword:{1}".FormatStr(x.Keywords, matched);
        //    }            
        //}

        //private void cleaning(Dnl_Link_Baidu x, List<IW2S_ExcludeKeyword> excludedKeywords)
        //{
        //    if(excludedKeywords == null || excludedKeywords.Count == 0)
        //    {
        //        return;
        //    }
        //    string txt = "{0}/{1}/{2}/{3}".FormatStr(x.Description, x.Abstract, x.Title, x.LinkUrl).ToLower();
        //    var matched_ex_kw = excludedKeywords.FirstOrDefault(k => txt.IsContains(k.Keyword));
        //    if (matched_ex_kw != null)
        //    {
        //        x.DataCleanStatus = (byte)DataCleanStatus.Excluded;
        //        x.Keywords = "{0} $ExcludedByExcludeKeyword:{1}".FormatStr(x.Keywords, matched_ex_kw.Keyword);
        //    }

            
        //}

       
                

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
            Console.WriteLine(DateTime.Now.ToString() + "  :  " + msg);
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
