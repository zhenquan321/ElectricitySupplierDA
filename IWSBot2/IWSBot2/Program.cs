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
            Thread computePanelIn6 = new Thread(new ThreadStart(() =>
            {
                BotControlPanelIn6Hours.Instance.Run();
            }));

            Thread computePanelByDay = new Thread(new ThreadStart(() =>
            {
                BotControlPanelByDay.Instance.Run();
            }));

            Thread computeChart = new Thread(new ThreadStart(() =>
            {
                ComputeProjectChart.Instance.Run();
            }));

            Thread baiduBot = new Thread(new ThreadStart(() =>
            {
                var br = new IW2SBotRegHelper();
                br.Register(BotType.Baidu);

                BaiduSearchMng.Instance.SetBusy += () => br.SentStatus(1);
                BaiduSearchMng.Instance.SetReady += () => br.SentStatus(0);
                BaiduSearchMng.Instance.Run();
            }));

            Thread weixinBot = new Thread(new ThreadStart(() =>
            {
                WeiXinSearchTools weixin = new WeiXinSearchTools();
                while (true)
                {
                    bool IsOK;
                    Console.Write("输入开始日期（yyyy-MM-dd）：");
                    string startStr = Console.ReadLine();
                    DateTime startDate = new DateTime();
                    IsOK = DateTime.TryParse(startStr, out startDate);
                    if (!IsOK)
                        continue;
                    else
                        weixin.StartDate = startDate;
                    Console.Write("输入结束日期（yyyy-MM-dd）：");
                    string endStr = Console.ReadLine();
                    DateTime endDate = new DateTime();
                    IsOK = DateTime.TryParse(endStr, out endDate);
                    if (!IsOK)
                        continue;
                    else
                    {
                        weixin.EndDate = endDate;
                        Console.WriteLine("日期为：{0}至{1}".FormatStr(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")));
                        Console.Write("是否无误？（Y/N）：");
                        string code = Console.ReadLine().ToUpper();
                        if(code=="Y")
                            break;
                    }
                }
                var br = new IW2SBotRegHelper();
                br.Register(BotType.WeiXin);
                weixin.SetBusy += () => br.SentStatus(1);
                weixin.SetReady += () => br.SentStatus(0);
                weixin.SetError += () => br.SentStatus(3);
                weixin.Run();
            }));

            Console.WriteLine("1、百度Bot\t2、百度图表预计算");
            Console.WriteLine("3、控制台-分钟计算\t4、控制台-天计算");
            Console.WriteLine("5、微信BOT");
            Console.Write("输入命令：");
            while (true)
            {
                string str = Console.ReadLine();
                int num = Convert.ToInt32(str);
                if (num >= 1 && num <= 5)
                {
                    switch (num)
                    {
                        case 1:         //百度BOT
                            baiduBot.Start();
                            break;
                        case 2:         //项目图表预计算
                            computeChart.Start();
                            break;
                        case 3:         //控制台最近6小时变化数计算
                            computePanelIn6.Start();
                            break;
                        case 4:         //控制台每天数据计算
                            computePanelByDay.Start();
                            break;
                        case 5:
                            weixinBot.Start();
                            break;
                    }
                    break;
                }
            }

            
            Console.Read();
        }

        private static void GenerateKeywordValLinkCount()
        {

        }

        //private static void InsertIW2SExcludeKeywords()
        //{
        //    List<string> excludes = new List<string>() { "wenku.baidu.com", "baike.baidu.com", "zhidao.baidu.com", "tieba.baidu.com", "baike.sogou.com", "www.zhihu.com",
        //    "wenku.it168.com","www.docin.com","www.cnki.net","www.1688.com","www.taobao.com","choshi.tmall.com","www.tmall.com","www.jd.com","www.yhd.com","www.suning.com",
        //    "www.gome.com","www.vip.com","www.Amazon.cn","www.dangdang.com","www.vancl.com","www.xinhuanet.com","www.cyol.net"};
        //    List<Dnl_IgnoreDomain> kws = new List<Dnl_IgnoreDomain>();
        //    foreach (string exclude in excludes)
        //    {
        //        Dnl_IgnoreDomain kw = new Dnl_IgnoreDomain
        //        {
        //            CreatedAt = DateTime.UtcNow.AddHours(8),
        //            Keyword = exclude
        //        };
        //        kws.Add(kw);
        //    }

        //    var col = MongoDBHelper.Instance.GetIW2S_ExcludeKeywords();

        //    col.InsertMany(kws);
        //}

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
