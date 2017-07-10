using GoogleS.BotTask;
using GoogleS.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleS.Search
{
    public class BotSearch
    {



        public static readonly BotSearch Instance = new BotSearch();

        public void Run()
        {
            while (true)
            {

                BotTaskService bt = new BotTaskService();
                Random r = new Random();
                Dnl_Google_BaiduCommend keyTask = bt.GetBotTask();  //get_task();
                if (keyTask == null || keyTask.CommendKeyword == "" || keyTask.CommendKeyword == null)
                {

                    log("No search task ! start search Detail !!!");
                    Thread.Sleep(1000);
                    //WX_Data wscData = bt.GetWxData();
                    //if (wscData == null)
                    //{
                    //}
                    continue;
                }

                var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 1 }, { "BotStatus", 1 } } } };

                var result = MongoDBHelper.Instance.Get_Dnl_Google_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                Snapshot(keyTask);

                try
                {

                    update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 2 }, { "BotStatus", 2 } } } };

                    result = MongoDBHelper.Instance.Get_Dnl_Google_BaiduCommend().UpdateOne(new QueryDocument { { "_id", keyTask._id } }, update);

                }
                catch (Exception ex)
                {
                    log(DateTime.Now + "ERROR ." + ex.Message);
                    Thread.Sleep(2000);
                }

            }
        }


        void Snapshot(Dnl_Google_BaiduCommend searchTask)
        {
            List<Dnl_Google_level1link> xListings = new List<Dnl_Google_level1link>();
            GoogleQuery wc = new GoogleQuery(searchTask.Keyword + searchTask.CommendKeyword);

            var links360 = wc.Query(searchTask);
            //  SaveKeyRecord(searchTask);
        }



        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }



    }
}
