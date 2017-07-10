using AISSystem;

using SogouSearch.BotTask;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SogouSearch.Search;
using MongoDB.Driver;
using SogouSearch.Helper;
using MongoDB.Bson;
using IW2SBotReg;

namespace SogouSearch
{
    class Program
    {
        static void Main(string[] args)
        {

            LogerHelper.SetConfig();
            Thread t = new Thread(new ThreadStart(() =>
            {
                var br = new IW2SBotRegHelper();
                br.Register(BotType.Sogou);
                BotSearch.Instance.SetBusy += () => br.SentStatus(1);
                BotSearch.Instance.SetReady += () => br.SentStatus(0);

                BotSearch.Instance.Run();

                // WeChartBot.Search.BosonNLP.Instance.Run();
            }));
            t.Start();
            Console.ReadLine();

          
          
            //SetUpdate();
            //Console.ReadLine();

        }


        public static void SetUpdate()
        {

            string connStr = AISSystem.AppSettingHelper.GetAppSetting("conStr");
            BotTaskService bt = new BotTaskService();
            //var WLList = bt.GetWhiteList(connStr);
            //var BLList = bt.GetBlackList(connStr);

            string sql = @"select Id, [NovelName],[AuthorName],[LinkUrl],[Domain],[TopDomain],[LinkTitle],[LinkAbstract],[Keyword] from [dbo].[ResultLiks] 
                        where NovelId in('11','12') and States =0";
            DataTable dt = DBHelper.Query(connStr, sql);

            foreach (DataRow item in dt.Rows)
            {
                int States = 0;
                int blackid = 0;
                string topDomain = item["TopDomain"].ToString();
                int nid=Convert.ToInt32(item["Id"]);
                //foreach (var itemBL in BLList)
                //{
                //    if (itemBL.Domain.Trim().ToLower().Equals(topDomain.Trim().ToLower()))
                //    {
                //        States = 2;
                //        blackid = itemBL.Id;
                //    }
                //}
                //foreach (var itemWL in WLList)
                //{
                //    if (itemWL.AuthorizedUrl1.Contains(topDomain))
                //    {
                        
                //        States = 1;
                //    }
                //}
                string updatesql = "update ResultLiks set BlackId={0} ,[States]={1} where Id={2} ".FormatStr(blackid, States, nid);
                var count = DBHelper.RunScript(connStr, updatesql);
                Console.WriteLine("成功修改一条！");
            }

            Console.WriteLine("全部完成");
        }


        static void Setmd5()
        {
            Guid sds = Guid.Parse("1481d9b4-1c80-d72d-ccec-74f826308cc6");
            var update = new UpdateDocument { { "$set", new QueryDocument { { "LoginPwd", sds } } } };

            var result = IW2SBotReg.MongoDBHelper.Instance.Get_IW2SUser().UpdateOne(new QueryDocument { { "_id", new ObjectId("573d8de16ce8ba07a0b3bb28") } }, update);
            Console.WriteLine("完成！");
        }


    }
}
