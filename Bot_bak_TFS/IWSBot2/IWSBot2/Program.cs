using System.Globalization;
using System.Net;
using System.Web.UI;
using AISSystem;
using IW2SBotReg;
using IWSBot.Utility;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IWSBot.Models;
using System.Diagnostics;
using MongoV2;
using Timer = System.Timers.Timer;
using IWSBot2.Utility;
using IW2S_BotRegister = IWSData.Model.IW2S_BotRegister;
using MongoDBHelper = IWSBot.Utility.MongoDBHelper;

namespace IWSBot
{
    class Program
    {
        static void Main(string[] args)
        {
            LogerHelper.SetConfig();
            //Thread t11 = new Thread(new ThreadStart(() =>
            //{
            //    var br = new IW2SBotRegHelper();
            //    br.Register(BotType.BaiduImg);

            //    BaiduImgMng.Instance.SetBusy += () => br.SentStatus(1);
            //    BaiduImgMng.Instance.SetReady += () => br.SentStatus(0);
            //    BaiduImgMng.Instance.Run();
            //}));
            //t11.Start();

            //Thread t12 = new Thread(new ThreadStart(() =>
            //{
            //    var br = new IW2SBotRegHelper();
            //    br.Register(BotType.Weibo);

            //    BaiduWeiboMng.Instance.SetBusy += () => br.SentStatus(1);
            //    BaiduWeiboMng.Instance.SetReady += () => br.SentStatus(0);
            //    BaiduWeiboMng.Instance.Run();
            //}));
            //t12.Start();
            
            ////Utility.WL_IndustryMng.Instance.startWL_Industry();
            //Thread t1 = new Thread(new ThreadStart(() =>
            //{
            //    var br = new IW2SBotRegHelper();
            //    br.Register(BotType.Baidu);

            //    BaiduKeywordMng.Instance.SetBusy += () => br.SentStatus(1);
            //    BaiduKeywordMng.Instance.SetReady += () => br.SentStatus(0);
            //    BaiduKeywordMng.Instance.Run();
            //}));


            Thread t2 = new Thread(new ThreadStart(() =>
            {
                var br = new IW2SBotRegHelper();
                br.Register(BotType.Baidu);

                IW2SBotMng.Instance.SetBusy += () => br.SentStatus(1);
                IW2SBotMng.Instance.SetReady += () => br.SentStatus(0);
                IW2SBotMng.Instance.Run();
            }));


            #region
            var botregTh = new Thread(() =>
            {
                var botId = Utility.Utility.GenerateBotId().ToString().Replace("-", "");

                var timer = new Timer { Interval = 60 * 1000, Enabled = true };
                timer.Elapsed += (s, e) =>
                {
                    // 在mongodb中注册Bot
                    Console.WriteLine("regist bot:{0}", botId);
                    var col = MongoDBHelper.Instance.GetIW2S_BotRegister();
                    var builder = Builders<IW2S_BotRegister>.Filter;
                    var ip = Utility.Utility.GetInternetIpAddress();
                    var host = Dns.GetHostName();
                    var pid = Process.GetCurrentProcess().Id;

                    var bot = col.Find(builder.Eq(x => x.BotId, botId)).FirstOrDefault();
                    if (bot == null)
                    {
                        col.InsertOne(new IW2S_BotRegister
                        {
                            BotId = botId,
                            RegTime = e.SignalTime.ToUniversalTime(),
                            IpAddress = ip,
                            HostName = host,
                            ProcessId = pid.ToString(CultureInfo.InvariantCulture),
                            Status = 0
                        });
                    }
                    else
                    {
                        var updateBot = new UpdateDocument
                        {
                            {"$set", new QueryDocument {{"RegTime", e.SignalTime.ToUniversalTime()}}}
                        };
                        col.UpdateOne(new QueryDocument { { "_id", bot._id } }, updateBot);
                    }
                };
            });
            #endregion


         //   t1.Start();
            t2.Start();
            //botregTh.Start();




            //Thread t2 = new Thread(new ThreadStart(() =>
            //{
            //    Utility.IW2SBotMng.Instance.Run();
            //}));
            //t2.Start();

            //Thread t3 = new Thread(new ThreadStart(() =>
            //    {
            //        Utility.BotMng.Instance.Run();
            //    }));
            //t3.Start();

            //Utility.HandleLinkData.Instance.Run();


            //Thread t2 = new Thread(new ThreadStart(() =>
            //{
            //    Utility.WhoisMng.Instance.start_whois();
            //}));
            //t2.Start();
            Console.Read();
        }

        private static void GenerateKeywordValLinkCount()
        {

        }

        private static void InsertIW2SExcludeKeywords()
        {
            List<string> excludes = new List<string>() { "wenku.baidu.com", "baike.baidu.com", "zhidao.baidu.com", "tieba.baidu.com", "baike.sogou.com", "www.zhihu.com",
            "wenku.it168.com","www.docin.com","www.cnki.net","www.1688.com","www.taobao.com","choshi.tmall.com","www.tmall.com","www.jd.com","www.yhd.com","www.suning.com",
            "www.gome.com","www.vip.com","www.Amazon.cn","www.dangdang.com","www.vancl.com","www.xinhuanet.com","www.cyol.net"};
            List<IW2S_ExcludeKeyword> kws = new List<IW2S_ExcludeKeyword>();
            foreach (string exclude in excludes)
            {
                IW2S_ExcludeKeyword kw = new IW2S_ExcludeKeyword
                {
                    CreatedAt = DateTime.UtcNow.AddHours(8),
                    Keyword = exclude
                };
                kws.Add(kw);
            }

            var col = MongoDBHelper.Instance.GetIW2S_ExcludeKeywords();

            col.InsertMany(kws);
        }

        private static void  handleTitleNull()
        {
            try
            {
                bool isContinue = false;
                do
                {
                    var queryTask = Query.And(Query.EQ("UsrId", new Guid("0CBDAF5F-CA3B-0ADF-E356-88413D97487C"))
                        ) as QueryDocument;
                    var col = MongoDBHelper.Instance.Getlevel1links();
                    var TaskList = col.Find(queryTask);
                    foreach (level1link link in TaskList.Limit(100).ToList())
                    {

                        var result2 = col.DeleteOne(new QueryDocument { {"_id",link._id}});
                    }
                    if (TaskList.Count() > 0)
                    {
                        isContinue = true;
                    }
                    else
                    {
                        isContinue = false;
                    }
                } while (isContinue);
            }
            catch (Exception ex)
            {

            }
        }
        private static void handleusrIdSiteId()
        {
            try
            {
                bool isContinue = false;
                do
                {
                    var builder = Builders<level1link>.Filter;

                    var queryTask = Query.And(Query.NE("UsrId", BsonNull.Value),
                        Query.NE("WebsiteId", BsonNull.Value),
                        Query.EQ("UsrIdSiteId", BsonNull.Value)) as QueryDocument;
                    var col = MongoDBHelper.Instance.Getlevel1links();
                    var TaskList = col.Find(queryTask).Skip(0).Limit(100);
                    foreach (level1link link in TaskList.ToList())
                    {
                        ObjectId usrIdSiteId = "{0}{1}".FormatStr(link.UsrId, link.WebsiteId).ToObjectId();
                        var updateWebsiteId = new UpdateDocument { { "$set", new QueryDocument { { "UsrIdSiteId", usrIdSiteId } } } };

                        var result2 = col.UpdateOne(builder.Eq(x=>x._id , link._id), updateWebsiteId);
                    }
                    if (TaskList.Count() > 0)
                    {
                        isContinue = true;
                    }
                    else
                    {
                        isContinue = false;
                    }
                } while (isContinue);
            }
            catch (Exception ex)
            {

            }
        }
        private static string GetTxtFromHtml2(string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";
            int indexOfLeft = html.GetIndex("<");
            int indexOfRight = html.GetIndex(">");
            //aaaa
            if (indexOfLeft < 0 && indexOfRight < 0)
                return html.GetTrimed();
            // href='''>aba
            if (indexOfLeft < 0)
            {
                if (indexOfRight >= html.Length - 1)
                    return "";
                return html.Substring(indexOfRight + 1).GetTrimed();
            }
            // abc <b
            if (indexOfRight < 0)
            {
                if (indexOfLeft == 0)
                    return "";
                return html.Substring(0, indexOfLeft).GetTrimed();
            }
            //abc<a>cd<
            if (indexOfLeft < indexOfRight)
            {
                string a = "";
                if (indexOfLeft > 0)
                    a = html.Substring(0, indexOfLeft).GetTrimed();
                if (indexOfRight >= html.Length - 1)
                    return a;
                string b = html.Substring(indexOfRight + 1);
                if (string.IsNullOrEmpty(b))
                    return a;
                html = (a + b).GetTrimed();
            }
            if (indexOfLeft > indexOfRight)
            {
                html = html.Substring(indexOfRight + 1);
            }
            return GetTxtFromHtml2(html).GetTrimed();

        }
        

        
    }
}
