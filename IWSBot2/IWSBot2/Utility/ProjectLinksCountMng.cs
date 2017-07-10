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
    class ProjectLinksCountMng
    {
        public static readonly ProjectLinksCountMng Instance = new ProjectLinksCountMng();


        public delegate void UpdateStatus();

        public event UpdateStatus SetBusy;
        public event UpdateStatus SetReady;

        public void Run()
        {
            while (true)
            {
                Random r = new Random();
                var p = get_search_to_count();
                if (p == null)
                {
                    SetReady();
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }
                int LinkCount=0;
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

                    var pro = Process.GetCurrentProcess();
                    string processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                    int botInterval = p.BotIntervalHours == 0 ? 7 * 24 : p.BotIntervalHours;
                    var update = new UpdateDocument { { "$set", new QueryDocument { 
                    { "BotStatus", 1 }, { "NextBotStartAt", DateTime.UtcNow.AddHours((double)botInterval + 8) }
                    ,{"BotTag",string.Format("{0}#", processName)},
                     {"BotId", botId}
                    } } };

                    var result = MongoDBHelper.Instance.GetIW2S_Projects().UpdateOne(new QueryDocument { { "_id", p._id } }, update);

                    LinkCount=count(p);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        log("Project_BaiduLinkCount_Count ERROR.Message:{0},Statck:{1}".FormatStr(ex.Message, ex.StackTrace));
                        ex = ex.InnerException;
                    }
                }
                //Convert.ToDateTime(doc["CreateTime"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                try
                {

                    var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt",DateTime.UtcNow.AddHours(8)},
                {"BotStatus",2},{"BaiduLinkCount",LinkCount}}}};
                    var commendCol = MongoDBHelper.Instance.GetIW2S_Projects();
                    var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);

                }
                catch (Exception ex)
                {
                    log("get_proj_to_count ERROR ." + ex.Message);
                    Thread.Sleep(5000);
                }
            }

        }

        /// <summary>
        /// 输出时间日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }

        /// <summary>
        /// 获取待统计的项目
        /// </summary>
        /// <returns></returns>
        IW2S_Project get_search_to_count()
        {
            try
            {

                var dt = DateTime.UtcNow.AddHours(8);

                var builder = Builders<IW2S_Project>.Filter;
                var filter = builder.Eq(x => x.IsDel, false);
                filter &= builder.Eq(x => x.BotStatus, 0) | builder.Exists(x => x.BotStatus, false);
                //filter &= builder.Eq(x => x.SearchSource, 1);

                var col = MongoDBHelper.Instance.GetIW2S_Projects();
                var result = col.Find(filter).SortByDescending(x => x.CreatedAt).FirstOrDefault();//

                //if (result == null)
                //{
                //    result = col.Find(filter).SortByDescending(x => x.Times).FirstOrDefault();
                //}
                if (result != null)
                {
                    Console.WriteLine("start to search {0}".FormatStr(result.Name));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get searchkeyword error: {0}".FormatStr(ex.Message));
            }

            return null;

        }

        /// <summary>
        /// 统计项目中链接数
        /// </summary>
        /// <param name="p">项目</param>
        private int count(IW2S_Project p)
        {

            try
            {
                //获取项目中已被删除的关键词
                var keywordBuilder = Builders<IW2S_BaiduCommend>.Filter;
                var keywrodFilter = keywordBuilder.Eq(x => x.IsRemoved, true);
                var keywordList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(keywrodFilter).Project(x => x._id.ToString()).ToList();

                //统计项目搜索的链接总数
                var linkBuilder = Builders<IW2S_level1link>.Filter;
                var linkFilter = linkBuilder.Eq(x => x.ProjectId, p._id) & linkBuilder.Nin(x => x.SearchkeywordId, keywordList) & linkBuilder.Eq(x => x.IsDel, false);
                var linkCount = MongoDBHelper.Instance.GetIW2S_level1links().Find(linkFilter).Count();
                return Convert.ToInt32(linkCount);
                
            }
            catch (Exception ex)
            {
                log(ex.Message);
                return 0;
            }

        }
    }
}
