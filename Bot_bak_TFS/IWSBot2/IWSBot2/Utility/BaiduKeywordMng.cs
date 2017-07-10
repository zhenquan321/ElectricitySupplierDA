using System.IO;
using System.Net;
using IWSBot.Utility;
using IWSData.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using System.Threading;
using System.Diagnostics;
using IWSBot.Queries;
using MongoV2;

namespace IWSBot.Utility
{
    public class BaiduKeywordMng
    {
        public static readonly BaiduKeywordMng Instance = new BaiduKeywordMng();

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
                //var keywordbuilder = Builders<IW2S_BaiduKeyword>.Filter;
                //var keywordFilter = keywordbuilder.Eq(x => x.Keyword, p.Keyword) & keywordbuilder.Eq(x => x.BotStatus, 2);
                //var keywordId = MongoDBHelper.Instance.GetIW2S_BaiduKeywords().Find(keywordFilter).Project(x=>x._id).FirstOrDefault();
                //var col = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
                //var builder = Builders<IW2S_BaiduCommend>.Filter;
                //var filter = builder.Eq(x => x.KeywordId, keywordId);
                //List<string> commends = col.Find(filter).Project(x=>x.CommendKeyword).ToList().Distinct().ToList();
                //if (commends != null && commends.Count > 3)
                //{
                //    foreach (string commend in commends)
                //    {
                //        if (commend == p.Keyword) continue;

                //        IW2S_BaiduCommend baiduCommend = new IW2S_BaiduCommend
                //        {                            
                //            CommendKeyword = commend,
                //            CreatedAt = DateTime.UtcNow.AddHours(8),
                //            Keyword = p.Keyword,
                //            UsrId = p.UsrId,
                //            KeywordId = p._id,
                //            ProjectId = p.ProjectId
                //        };

                //        col.InsertOne(baiduCommend);

                //    }
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "BotStatus", 2 } } } };

                //    var result = MongoDBHelper.Instance.GetIW2S_BaiduKeywords().UpdateOne(new QueryDocument { { "_id", p._id } }, update);
                //}
                //else
                {
                    try
                    {
                        SetBusy();
                        //var internetIp = Utility.GetInternetIpAddress();

                        //var ipaddrs = System.Net.Dns.GetHostEntry(System.Environment.MachineName).AddressList;
//                        //string ip = string.Empty;
//                        if (ipaddrs.Length >= 3)
//                        {
//                            ip = ipaddrs[2].ToString();
//                        }
//                        else if (string.IsNullOrEmpty(ip) && ipaddrs.Length >= 0)
//                        {
//                            ip = ipaddrs[0].ToString();
//                        }
                        var pro = Process.GetCurrentProcess();
                        var processName = IDHelper.GetGuid(pro.MainModule.FileName).ToString();
                        var botId = Utility.GenerateBotId().ToString().Replace("-","");
                        
                        var update = new UpdateDocument
                        {
                            {
                                "$set", new QueryDocument
                                {
                                    {"BotStatus", 1}
                                    ,
                                    {"BotTag", string.Format("{0}#",  processName)},
                                    {"BotId", botId}
                                }
                            }
                        };

                        var result = MongoDBHelper.Instance.GetIW2S_BaiduKeywords().UpdateOne(new QueryDocument { { "_id", p._id } }, update);

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

                        var update = new UpdateDocument { { "$set", new QueryDocument { { "BotStatus", 2 } } } };

                        var result = MongoDBHelper.Instance.GetIW2S_BaiduKeywords().UpdateOne(new QueryDocument { { "_id", p._id } }, update);


                    }
                    catch (Exception ex)
                    {
                        log("get_proj_to_qry ERROR ." + ex.Message);
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }

        private void query(IW2S_BaiduKeyword p)
        {

            try
            {
                var builder = Builders<IW2S_BaiduKeyword>.Filter;

                try
                {
                    BaiduKeywordQuery baidu = new BaiduKeywordQuery(p.Keyword);

                    baidu.Query(p);
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

        IW2S_BaiduKeyword get_search_to_qry()
        {
            try
            {

                var builder = Builders<IW2S_BaiduKeyword>.Filter;
                var filter = builder.Eq(x => x.BotStatus, 0);
                //filter &= builder.Eq(x => x.Keyword, "周黑鸭");
                var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
                var result = col.Find(filter).SortByDescending(x => x.CreatedAt).FirstOrDefault();//

                if (result != null)
                {
                    Console.WriteLine("start to search {0}".FormatStr(result.Keyword));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get IW2S_BaiduKeyword error: {0}".FormatStr(ex.Message));
            }

            return null;

        }

       
    }

    public class Utility
    {
        public static string GetInternetIpAddress()
        {
            var errTime = 0;

            while (true)
            {
                try
                {
                    var webRequest = WebRequest.Create("http://www.ip138.com/ips138.asp");
                    var stream = webRequest.GetResponse().GetResponseStream();
                    if (stream == null)
                    {
                        return null;
                    }
                    var streamReader = new StreamReader(stream, Encoding.Default);
                    var all = streamReader.ReadToEnd();
                    var ip = all.Split(new[] { "您的IP地址是：[" }, StringSplitOptions.RemoveEmptyEntries)[1].
                        Split(new[] { "] 来自" }, StringSplitOptions.RemoveEmptyEntries)[0];

                    streamReader.Close();
                    stream.Close();
                    return ip;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errTime++;
                    if (errTime == 4)
                    {
                        return null;
                    }
                    Console.WriteLine("Retry: {0}", errTime);
                }
            }
        }

        public static int GetTimestamp()
        {
            var time = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var ts = (int)time.TotalSeconds;
            return ts;
        }

        private static Guid _botId = Guid.Empty;

        public static Guid GenerateBotId()
        {
            if (_botId != Guid.Empty)
            {
                return _botId;
            }

            var hostName = Dns.GetHostName();
            var processId = Process.GetCurrentProcess().Id;
            var ts = GetTimestamp();
            var idStr = hostName + processId + ts;
            _botId = IDHelper.GetGuid(idStr);
            return _botId;
        }


    }
}
