using BingS.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using AISSystem;
using MongoDB.Bson;

namespace BingS.BotTask
{
    public class BotTaskService
    {

        static object taskToken = new object();
        public IW2S_Bing_BaiduCommend GetBotTask()
        {
            lock (taskToken)
            {
                try
                {
                    var builder = Builders<IW2S_Bing_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.BotStatus, 0);
                    var col = MongoDBHelper.Instance.Get_IW2S_Bing_BaiduCommend();
                    var result = col.Find(filter).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }

                    //IW2S_Bing_BaiduCommend result = new IW2S_Bing_BaiduCommend();
                    //result._id = new ObjectId("57d25dbecba12d0408ab5b85");
                    //result.Keyword = "砚台";
                    //result.KeywordId = new ObjectId("57d25dbecba12d0408ab5b84");
                    //result.CommendKeyword = "砚台";
                    //result.UsrId = new ObjectId("5784a7bffbd6fc0b04224747");


                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywords task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


    }
}
