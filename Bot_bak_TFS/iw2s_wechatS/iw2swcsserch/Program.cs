using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IW2SBotReg;
using iw2swcsserch.Search;
using MongoDB.Driver;
using MongoDB.Bson;

namespace iw2swcsserch
{
    class Program
    {
        static void Main(string[] args)
        {

            LogerHelper.SetConfig();
            Thread t = new Thread(new ThreadStart(() =>
            {
                var br = new IW2SBotRegHelper();
                br.Register(BotType.WeChat);

                BotSearch.Instance.SetBusy += () => br.SentStatus(1);
                BotSearch.Instance.SetReady += () => br.SentStatus(0);
                BotSearch.Instance.Run();
                // WeChartBot.Search.BosonNLP.Instance.Run();
            }));
            t.Start();
            
            Console.ReadLine();

        }

        static void update()
        {
            var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 0 }, { "BotStatus", 0 } } } };

            var result = iw2swcsserch.Helper.MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateMany(new QueryDocument { { "UsrId", new ObjectId("5857cac4cba12d0b10168da4") } }, update);
            Console.WriteLine("完成");
        }

    }
}
