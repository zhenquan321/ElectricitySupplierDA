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
using Iw2sDataAnalysis.Models;
using Iw2sDataAnalysis.Helper;
using MongoV2;

namespace Iw2sDataAnalysis
{
    public class BotTaskService
    {

        #region 常规搜索任务调度系统

        static object taskToken = new object();
        string com = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");
        Queue<IW2S_BaiduCommend> searchkeyword_pool = new Queue<IW2S_BaiduCommend>();
        public Dnl_KeywordMapping GetBaiduBotTask()
        {
            lock (taskToken)
            {
                try
                {
                    //获取已搜索完成的关键词映射(只计算所有词）
                    var builderMap = Builders<Dnl_KeywordMapping>.Filter;
                    var filterMap = builderMap.Eq(x => x.JisuanStatus, 0) & builderMap.Eq(x => x.IsDel, false) & builderMap.Eq(x => x.CategoryId, ObjectId.Empty);
                    var map = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).FirstOrDefault();
                    if (map != null)
                    {
                        var builderKey = Builders<Dnl_Keyword>.Filter;
                        var filterKey = builderKey.Eq(x => x._id, map.KeywordId) & builderKey.Eq(x => x.BotStatus_Baidu, 2);
                        var keyword = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).FirstOrDefault();
                        if (keyword != null)
                        {
                            Console.WriteLine("开始计算百度关键词： {0}".FormatStr(keyword.Keyword));
                            return map;
                        }
                        
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("获取关键词错误： {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        public MediaKeywordMappingMongo GetMediaBotTask()
        {
            lock (taskToken)
            {
                try
                {
                    //获取已搜索完成的关键词映射(只计算所有词）
                    var builderMap = Builders<MediaKeywordMappingMongo>.Filter;
                    var filterMap = builderMap.Eq(x => x.JisuanStatus, 0) & builderMap.Eq(x => x.IsDel, false) & builderMap.Eq(x => x.CategoryId, ObjectId.Empty);
                    var map = MongoDBHelper.Instance.GetMediaKeywordMapping().Find(filterMap).FirstOrDefault();
                    if (map != null)
                    {
                        var builderKey = Builders<MediaKeywordMongo>.Filter;
                        var filterKey = builderKey.Eq(x => x._id, map.KeywordId) & builderKey.Eq(x => x.WXBotStatus, 2);
                        var keyword = MongoDBHelper.Instance.GetMediaKeyword().Find(filterKey).FirstOrDefault();
                        if (keyword != null)
                        {
                            Console.WriteLine("开始计算微信关键词： {0}".FormatStr(keyword.Keyword));
                            return map;
                        }

                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("获取关键词错误： {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }



        /// <summary>
        /// 获取项目内所有关键词
        /// </summary>
        /// <param name="prijid"></param>
        /// <returns></returns>
        public List<Dnl_KeywordMapping> GetBaiduBotTaskList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<Dnl_KeywordMapping>.Filter;
                    var filter = builder.Eq(x => x.ProjectId, prijid);
                    filter &= builder.Eq(x => x.IsDel, false);
                    filter &= builder.Eq(x => x.CategoryId, ObjectId.Empty);
                  //  filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.GetDnl_KeywordMapping();
                    var result = col.Find(filter);//
                    if (result != null)
                    {
                        Console.WriteLine("成功获取全部关键词映射");
                    }
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取项目内所有关键词
        /// </summary>
        /// <param name="prijid"></param>
        /// <returns></returns>
        public List<MediaKeywordMappingMongo> GetWeiXinBotTaskList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<MediaKeywordMappingMongo>.Filter;
                    var filter = builder.Eq(x => x.ProjectId, prijid);
                    filter &= builder.Eq(x => x.IsDel, false);
                    filter &= builder.Eq(x => x.CategoryId, ObjectId.Empty);
                    //  filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.GetMediaKeywordMapping();
                    var result = col.Find(filter);//
                    if (result != null)
                    {
                        Console.WriteLine("成功获取全部关键词映射");
                    }
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取全部链接的标题和描述
        /// </summary>
        /// <param name="prijid"></param>
        /// <returns></returns>
        public List<Dnl_Link_BaiduDto> GetBaiduLinkTitleList(List<string> keyIds)
        {
            lock (taskToken)
            {
                try
                {
                    var filter = Builders<Dnl_Link_Baidu>.Filter.In(x => x.SearchkeywordId, keyIds);
                    var links = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filter).Project(x => new Dnl_Link_BaiduDto
                    {
                        _id = x._id.ToString(),
                        Keywords = x.Keywords,
                        SearchkeywordId = x.SearchkeywordId,
                        Title = x.Title,
                        Description = x.Description
                    }).ToList();

                    if (links != null&&links.Count!=0)
                    {
                        Console.WriteLine("成功获取全部数据标题");
                    }
                    return links;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取全部链接的标题和描述
        /// </summary>
        /// <param name="prijid"></param>
        /// <returns></returns>
        public List<Dnl_Link_BaiduDto> GetWeiXininkTitleList(List<string> keyIds)
        {
            lock (taskToken)
            {
                try
                {
                    var filter = Builders<WXLinkMainMongo>.Filter.In(x => x.KeywordId, keyIds);
                    var links = MongoDBHelper.Instance.GetWXLinkMain().Find(filter).Project(x => new Dnl_Link_BaiduDto
                    {
                        _id = x._id.ToString(),
                        Keywords = x.Keyword,
                        SearchkeywordId = x.KeywordId,
                        Title = x.Title,
                        Description = x.Description
                    }).ToList();

                    if (links != null && links.Count != 0)
                    {
                        Console.WriteLine("成功获取全部数据标题");
                    }
                    return links;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


        #endregion

    }

    public class SG_BotTaskService
    {

        #region 常规搜索任务调度系统

        static object taskToken = new object();
        string com = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");
        Queue<IW2S_SG_BaiduCommend> searchkeyword_pool = new Queue<IW2S_SG_BaiduCommend>();
        public IW2S_SG_BaiduCommend GetBotTask()
        {
            lock (taskToken)
            {
                try
                {
                    var builder = Builders<IW2S_SG_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.JisuanStatus, 0);
                    filter &= builder.Eq(x => x.BotStatus, 2);
                    filter &= builder.Eq(x => x.SearchSource, 1);
                    filter &= builder.Eq(x => x.IsRemoved, false);
                    // filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.Get_IW2S_SG_BaiduCommend();
                    var result = col.Find(filter).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywords task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


        public List<IW2S_SG_BaiduCommend> GetBotTaskList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<IW2S_SG_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.ProjectId, prijid);
                    filter &= builder.Eq(x => x.SearchSource, 1);
                    filter &= builder.Eq(x => x.IsRemoved, false);
                    //  filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.Get_IW2S_SG_BaiduCommend();
                    var result = col.Find(filter);//
                    if (result != null)
                    {
                        Console.WriteLine("成功获取全部关键词");
                    }
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        public List<IW2S_SG_level1link> GetLinkTitleList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    //var builder = Builders<IW2S_level1link>.Filter;
                    //var filter = builder.Eq(x => x.ProjectId, prijid);
                    //// filter &= builder.Lte(x => x.LastBotEndAt, dt);
                    //var col = MongoDBHelper.Instance.GetIW2S_level1links();
                    //var result = col.Find(filter);//

                    MongoDBClass<IW2S_SG_level1link> helper = new MongoDBClass<IW2S_SG_level1link>();
                    var queryTask = new QueryDocument { { "ProjectId", prijid } };
                    List<IW2S_level1link> list = new List<IW2S_level1link>();
                    FieldsDocument fd = new FieldsDocument();
                    fd.Add("Title", 1);
                    MongoCollection<IW2S_SG_level1link> col = new MongoDBClass<IW2S_SG_level1link>().GetMongoDB().GetCollection<IW2S_SG_level1link>("IW2S_SG_level1link");
                    var TaskList = col.Find(queryTask).SetFields(fd);

                    if (TaskList != null)
                    {
                        Console.WriteLine("成功获取全部数据标题");
                    }
                    return TaskList.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


        #endregion

    }

    public class WX_BotTaskService
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
                    var filter = builder.Eq(x => x.JisuanStatus, 0);
                    filter &= builder.Eq(x => x.BotStatus, 2);
                    filter &= builder.Eq(x => x.SearchSource, 1);
                    filter &= builder.Eq(x => x.IsRemoved, false);
                    // filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.Get_IW2S_WX_BaiduCommend();
                    var result = col.Find(filter).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywords task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


        public List<IW2S_WX_BaiduCommend> GetBotTaskList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.ProjectId, prijid);
                    filter &= builder.Eq(x => x.SearchSource, 1);
                    filter &= builder.Eq(x => x.IsRemoved, false);
                    //  filter &= builder.Eq(x => x.UsrId, new ObjectId("571e283e6ce8b80cb8963e84"));
                    var col = MongoDBHelper.Instance.Get_IW2S_WX_BaiduCommend();
                    var result = col.Find(filter);//
                    if (result != null)
                    {
                        Console.WriteLine("成功获取全部关键词");
                    }
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }

        public List<IW2S_WX_level1link> GetLinkTitleList(ObjectId prijid)
        {
            lock (taskToken)
            {
                try
                {

                    //var builder = Builders<IW2S_level1link>.Filter;
                    //var filter = builder.Eq(x => x.ProjectId, prijid);
                    //// filter &= builder.Lte(x => x.LastBotEndAt, dt);
                    //var col = MongoDBHelper.Instance.GetIW2S_level1links();
                    //var result = col.Find(filter);//

                    MongoDBClass<IW2S_WX_level1link> helper = new MongoDBClass<IW2S_WX_level1link>();
                    var queryTask = new QueryDocument { { "ProjectId", prijid } };
                    List<IW2S_level1link> list = new List<IW2S_level1link>();
                    FieldsDocument fd = new FieldsDocument();
                    fd.Add("Title", 1);
                    MongoCollection<IW2S_WX_level1link> col = new MongoDBClass<IW2S_WX_level1link>().GetMongoDB().GetCollection<IW2S_WX_level1link>("IW2S_WX_level1link");
                    var TaskList = col.Find(queryTask).SetFields(fd);

                    if (TaskList != null)
                    {
                        Console.WriteLine("成功获取全部数据标题");
                    }
                    return TaskList.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywordList task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


        #endregion

    }



}
