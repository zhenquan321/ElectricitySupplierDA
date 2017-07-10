using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using System.Web;
using System.Linq;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using iw2swcsserch.Models;
using iw2swcsserch.Helper;
using MongoV2;

namespace WeChartBot
{
    public class BotTaskService
    {

        #region 常规搜索任务调度系统

        static object taskToken = new object();
        string com = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");
        Queue<IW2S_WX_BaiduCommend> searchkeyword_pool = new Queue<IW2S_WX_BaiduCommend>();
        public IW2S_WX_BaiduCommend GetBotTask()
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.WXStatus, 0);
                    filter &= builder.Eq(x => x.BotStatus, 0);
                    // filter &= builder.Lte(x => x.LastBotEndAt, dt);
                    var col = MongoDBHelper.Instance.Get_IW2S_BaiduCommend();
                    var result = col.Find(filter).SortByDescending(x=>x.CreatedAt).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }

                    //IW2S_WX_BaiduCommend result = new IW2S_WX_BaiduCommend();
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


        #endregion








    }
}
