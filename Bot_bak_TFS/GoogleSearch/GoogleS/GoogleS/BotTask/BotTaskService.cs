using GoogleS.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Bson;

namespace GoogleS.BotTask
{
    public class BotTaskService
    {


        static object taskToken = new object();
        public Dnl_Google_BaiduCommend GetBotTask()
        {
            lock (taskToken)
            {
                try
                {
                    var builder = Builders<Dnl_Google_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.BotStatus, 0);
                    var col = MongoDBHelper.Instance.Get_Dnl_Google_BaiduCommend();
                    var result = col.Find(filter).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }

                    //Dnl_Google_BaiduCommend result = new Dnl_Google_BaiduCommend();
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
