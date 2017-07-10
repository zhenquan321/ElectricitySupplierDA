using BingS.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BingS
{
    class Program
    {
        static void Main(string[] args)
        {


            Thread t = new Thread(new ThreadStart(() =>
            {
                BotSearch.Instance.Run();

            }));
            t.Start();

         

            Console.ReadLine();


        }


        public static void sfdsf()
        {
            var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 0 }, { "BotStatus", 0 } } } };

            var result = MongoDBHelper.Instance.Get_IW2S_Bing_BaiduCommend().UpdateMany(new QueryDocument { { "_id", new ObjectId("5875f8c1cba12d0128294251") } }, update);
            Console.WriteLine("完成");
        }

    }
}
