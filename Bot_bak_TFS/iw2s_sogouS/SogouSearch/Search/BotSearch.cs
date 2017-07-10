using AISSystem;
using MongoDB.Driver;
using QiDianData.EF;
using SogouSearch.BotTask;
using SogouSearch.Helper;
using SogouSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SogouSearch.Search
{
    public class BotSearch
    {
        public static readonly BotSearch Instance = new BotSearch();

        string connStr = AISSystem.AppSettingHelper.GetAppSetting("conStr");

        public delegate void UpdateBotStatus();

        public event UpdateBotStatus SetReady;
        public event UpdateBotStatus SetBusy;


        public void Run()
        {
            while (true)
            {
                BotTaskService bt = new BotTaskService();
                Random r = new Random();
                IW2S_SG_BaiduCommend keyTask = bt.GetBotTask();  //get_task();
                if (keyTask == null || keyTask.CommendKeyword == "" || keyTask.CommendKeyword == null)
                {
                    SetReady();
                    log("没有搜索任务 !!!");
                    Thread.Sleep(3000);
                    continue;
                }
                SetBusy();
                var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 1 }, { "BotStatus", 1 } } } };

                var result = MongoDBHelper.Instance.Get_IW2S_SG_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                Snapshot(keyTask);
                try
                {
                    update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 2 }, { "BotStatus", 2 } } } };

                    result = MongoDBHelper.Instance.Get_IW2S_SG_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                }
                catch (Exception ex)
                {
                    log(DateTime.Now + "ERROR ." + ex.Message);
                    Thread.Sleep(2000);
                }

            }
        }


        void Snapshot(IW2S_SG_BaiduCommend tsk)
        {

            SogouQuery wc = new SogouQuery();
            wc.Query(tsk);
            //  SaveKeyRecord(searchTask);
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
