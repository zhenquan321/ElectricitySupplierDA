
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AISSystem;
using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using iw2swcsserch.Models;
using iw2swcsserch.Helper;
using iw2swcsserch.Template;
using System.Diagnostics;
using WeChartBot;


namespace iw2swcsserch.Search
{
    public class BotSearch
    {
        public static readonly BotSearch Instance = new BotSearch();

        public delegate void UpdateBotStatus();

        public event UpdateBotStatus SetReady;
        public event UpdateBotStatus SetBusy;

        public void Run()
        {
            while (true)
            {
                BotTaskService bt = new BotTaskService();
                Random r = new Random();
                IW2S_WX_BaiduCommend keyTask = bt.GetBotTask();  //get_task();
                if (keyTask == null || keyTask.CommendKeyword == "" || keyTask.CommendKeyword == null)
                {
                    SetReady();
                    log("No search task ! start search Detail !!!");
                    Thread.Sleep(1000);
                    //WX_Data wscData = bt.GetWxData();
                    //if (wscData == null)
                    //{
                    //}
                    continue;
                }
                SetBusy();

                var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 1 }, { "BotStatus",1 } } } };

                var result = MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                Snapshot(keyTask);

                //if (list.Count > 0)
                //{
                //    foreach (var busKeyword in list)
                //    {
                //        Snapshot(keyTask, busKeyword, ExcludeKeyword);
                //    }
                //}
                try
                {

                    update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 2 }, { "BotStatus", 2 } } } };

                    result = MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                }
                catch (Exception ex)
                {
                    log(DateTime.Now + "ERROR ." + ex.Message);
                    Thread.Sleep(2000);
                }
            }
        }


        void Snapshot(IW2S_WX_BaiduCommend searchTask)
        {
            List<IW2S_WX_level1link> xListings = new List<IW2S_WX_level1link>();
            WeChatQuery wc = new WeChatQuery(searchTask.Keyword + searchTask.CommendKeyword);
            var links360 = wc.Query(searchTask);
          //  SaveKeyRecord(searchTask);
        }

        //void SnapshotByAllBus(WX_SearchKeyword searchTask, GlobalBusKeyWords busTask, List<WX_ExcludeKeyword> ExcludeKeyword)
        //{


        //    List<WX_Data> xListings = new List<WX_Data>();

        //    // TaoBaoSnapshotQuery tbq = new TaoBaoSnapshotQuery();
        //    WeChatQueryByBus wc = new WeChatQueryByBus(searchTask.KeywordName + busTask.KeywordName);
        //    var links360 = wc.Query(searchTask, busTask, ExcludeKeyword);

        //    //for (int page = 0; page < links360.Count; page++)
        //    //{
        //    //    SaveKeyRecord(links360);
        //    //}

        //}
        void PublicNoNameSearch(IW2S_WX_level1link data)
        {


            //List<WX_Data> xListings = new List<WX_Data>();

            //// TaoBaoSnapshotQuery tbq = new TaoBaoSnapshotQuery();
            //WeChatQuery wc = new WeChatQuery(searchTask.KeywordName + busTask.KeywordName);
            //var links360 = wc.Query(searchTask, busTask);
            //var imgsList = wc.imgsList;
            //int pagesize = 1000;
            //for (int page = 0; page * pagesize < links360.Count; page++)
            //{
            //    SaveResult(links360.Skip(page * pagesize).Take(pagesize).ToList(), imgsList);
            //}

        }


       

        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }

    }

    public class ModelsConstants
    {
        public const string MDMSalt = "MDMSalt_1q2w3e!@#";
        public const string MDMKey = "MDMCryp_1q2w3e$%^";
    }



}
