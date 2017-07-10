
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
using Iw2sDataAnalysis.Models;
using Iw2sDataAnalysis.Helper;
using Iw2sDataAnalysis.Template;
using System.Diagnostics;




namespace Iw2sDataAnalysis.Search
{
    public class BotSearch
    {
        public static readonly BotSearch Instance = new BotSearch();
        BotSearch()
        {

        }

        public void Run()
        {
            while (true)
            {
                #region 百度
                //百度
                BotTaskService bt = new BotTaskService();
                Dnl_KeywordMapping keyTask = bt.GetBaiduBotTask();  //get_task();
                //if (keyTask == null || keyTask.Keyword == "" || keyTask.Keyword == null)
                //{
                //    log("无计算目标，休眠一天！");
                //    Thread.Sleep(24 * 60 * 60 * 1000);
                //    //WX_Data wscData = bt.GetWxData();
                //    //if (wscData == null)
                //    //{
                //    //}
                //    continue;
                //}
                //更新项目内所有该关键词的计算状态
                if (keyTask != null)
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "JisuanStatus", 1 } } } };
                    var builderMap = Builders<Dnl_KeywordMapping>.Filter;
                    var filterMap = builderMap.Eq(x => x.ProjectId, keyTask.ProjectId) & builderMap.Eq(x => x.KeywordId, keyTask.KeywordId);
                    var result = MongoDBHelper.Instance.GetDnl_KeywordMapping().UpdateMany(filterMap, update);
                    List<Dnl_KeywordMapping> listKey = bt.GetBaiduBotTaskList(keyTask.ProjectId);
                    BaiduSnapshot(keyTask, listKey);
                    try
                    {
                        update = new UpdateDocument { { "$set", new QueryDocument { { "JisuanStatus", 2 } } } };
                        result = MongoDBHelper.Instance.GetDnl_KeywordMapping().UpdateMany(filterMap, update);
                    }
                    catch (Exception ex)
                    {
                        log(DateTime.Now + "错误：" + ex.Message);
                        Thread.Sleep(2000);
                    }
                }
                #endregion

                #region 微信
                MediaKeywordMappingMongo WXKeyTask = bt.GetMediaBotTask();  //get_task();
                if (WXKeyTask == null || WXKeyTask.Keyword == "" || WXKeyTask.Keyword == null)
                {
                    log("无计算目标，休眠一天！");
                    Thread.Sleep(24 * 60 * 60 * 1000);
                    //WX_Data wscData = bt.GetWxData();
                    //if (wscData == null)
                    //{
                    //}
                    continue;
                }
                //更新项目内所有该关键词的计算状态
                var WXupdate = new UpdateDocument { { "$set", new QueryDocument { { "JisuanStatus", 1 } } } };
                var builderMediaMap = Builders<MediaKeywordMappingMongo>.Filter;
                var filterMediaMap = builderMediaMap.Eq(x => x.ProjectId, WXKeyTask.ProjectId) & builderMediaMap.Eq(x => x.KeywordId, WXKeyTask.KeywordId);
                MongoDBHelper.Instance.GetMediaKeywordMapping().UpdateMany(filterMediaMap, WXupdate);
                List<MediaKeywordMappingMongo> listMediaKey = bt.GetWeiXinBotTaskList(WXKeyTask.ProjectId);
                WeiXinSnapshot(WXKeyTask, listMediaKey);
                try
                {
                    WXupdate = new UpdateDocument { { "$set", new QueryDocument { { "JisuanStatus", 2 } } } };
                    MongoDBHelper.Instance.GetMediaKeywordMapping().UpdateMany(filterMediaMap, WXupdate);
                }
                catch (Exception ex)
                {
                    log(DateTime.Now + "错误：" + ex.Message);
                    Thread.Sleep(2000);
                }
                #endregion
            }
        }

        void BaiduSnapshot(Dnl_KeywordMapping searchTask, List<Dnl_KeywordMapping> listKey)
        {
            List<IW2S_Data> xListings = new List<IW2S_Data>();
            WeChatQuery wc = new WeChatQuery(searchTask.Keyword);
            var links360 = wc.BaiduQuery(searchTask, listKey);

        }

        void WeiXinSnapshot(MediaKeywordMappingMongo searchTask, List<MediaKeywordMappingMongo> listKey)
        {
            List<IW2S_Data> xListings = new List<IW2S_Data>();
            WeChatQuery wc = new WeChatQuery(searchTask.Keyword);
            var links360 = wc.WeiXinQuery(searchTask, listKey);

        }

        void SG_Snapshot(IW2S_SG_BaiduCommend searchTask, List<IW2S_SG_BaiduCommend> listKey)
        {
            List<IW2S_SG_Data> xListings = new List<IW2S_SG_Data>();
            SG_WeChatQuery wc = new SG_WeChatQuery(searchTask.Keyword + searchTask.CommendKeyword);
            var links360 = wc.Query(searchTask, listKey);

        }

        void WX_Snapshot(IW2S_WX_BaiduCommend searchTask, List<IW2S_WX_BaiduCommend> listKey)
        {
            List<IW2S_WX_Data> xListings = new List<IW2S_WX_Data>();
            WX_WeChatQuery wc = new WX_WeChatQuery(searchTask.Keyword + searchTask.CommendKeyword);
            var links360 = wc.Query(searchTask, listKey);

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
