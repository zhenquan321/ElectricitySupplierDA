using AISSystem;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot.Utility
{
    public class MongoDBHelper : MDB
    {

        static string conn = AppSettingHelper.GetAppSetting("mongoCon");

        static string dbName = AppSettingHelper.GetAppSetting("mongoDB");

        public MongoDBHelper()
            : base(conn, dbName)
        {

        }

        public IMongoDatabase GetMongoDB()
        {
            return base.GetDb();
        }

        public static readonly MongoDBHelper Instance = new MongoDBHelper();

        public IMongoCollection<searchkeyword> Getiws_searchkeywords()
        {
            return base.GetCollection<searchkeyword>("searchkeyword");
        }

        public IMongoCollection<keyword> Getiws_excludekeywords()
        {
            return base.GetCollection<keyword>("excludekeyword");
        }
        public IMongoCollection<keyword> Getiws_businesskeywords()
        {
            return base.GetCollection<keyword>("businesskeyword");
        }
        public IMongoCollection<website> Getiws_websites()
        {
            return base.GetCollection<website>("iws_website");
        }

        public IMongoCollection<level1link> Getlevel1links()
        {
            return base.GetCollection<level1link>("level1link");
        }

        public IMongoCollection<IW2S_BaiduKeyword> GetIW2S_BaiduKeywords()
        {
            return base.GetCollection<IW2S_BaiduKeyword>("IW2S_BaiduKeyword");
        }

        public IMongoCollection<IW2S_BaiduCommend> GetIW2S_BaiduCommends()
        {
            return base.GetCollection<IW2S_BaiduCommend>("IW2S_BaiduCommend");
        }

        public IMongoCollection<IW2S_SearchKeyword> GetIW2S_SearchKeywords()
        {
            return base.GetCollection<IW2S_SearchKeyword>("IW2S_SearchKeyword");
        }
        public IMongoCollection<Dnl_IgnoreDomain> GetDnl_IgnoreDomain()
        {
            return base.GetCollection<Dnl_IgnoreDomain>("Dnl_IgnoreDomain");
        }
        public IMongoCollection<IW2S_level1link> GetIW2S_level1links()
        {
            return base.GetCollection<IW2S_level1link>("IW2S_level1link");
        }

        public IMongoCollection<IW2S_BotRegister> GetIW2S_BotRegister()
        {
            return GetCollection<IW2S_BotRegister>("IW2S_BotRegister");
        }

        public IMongoCollection<WL_Industry> GetWL_Industrys()
        {
            return GetCollection<WL_Industry>("WL_Industry");
        }
        public IMongoCollection<IW2S_KeywordFilter> GetIW2S_KeywordFilters()
        {
            return base.GetCollection<IW2S_KeywordFilter>("IW2S_KeywordFilter");
        }
        public IMongoCollection<IW2S_ImgSearchTask> GetIW2S_ImgSearchTasks()
        {
            return base.GetCollection<IW2S_ImgSearchTask>("IW2S_ImgSearchTask");
        }
        public IMongoCollection<IW2S_ImgSearchLink> GetIW2S_ImgSearchLinks()
        {
            return base.GetCollection<IW2S_ImgSearchLink>("IW2S_ImgSearchLink");
        }
        public IMongoCollection<IW2S_WB_BaiduCommend> GetIW2S_WB_BaiduCommends()
        {
            return base.GetCollection<IW2S_WB_BaiduCommend>("IW2S_WB_BaiduCommend");
        }
        public IMongoCollection<IW2S_WB_level1link> GetIW2S_WB_level1links()
        {
            return base.GetCollection<IW2S_WB_level1link>("IW2S_WB_level1link");
        }

        public IMongoCollection<IW2S_Project> GetIW2S_Projects()
        {
            return base.GetCollection<IW2S_Project>("IW2S_Project");
        }

        public IMongoCollection<IW2SUser> Get_IW2SUser()
        {
            return base.GetCollection<IW2SUser>("IW2SUser");
        }

        public IMongoCollection<IW2S_ProLinksCount> GetIW2S_ProLinksCount()
        {
            return base.GetCollection<IW2S_ProLinksCount>("IW2S_ProLinksCount");
        }

        public IMongoCollection<IW2S_BotData> GetIW2S_BotDataIn6Hours()
        {
            return GetCollection<IW2S_BotData>("IW2S_BotDataIn6Hours");
        }

        public IMongoCollection<IW2S_BotData> GetIW2S_BotDataInDay()
        {
            return GetCollection<IW2S_BotData>("IW2S_BotDataInDay");
        }

        #region 独立关键词与链接库
        public IMongoCollection<Dnl_Keyword> GetDnl_Keyword()
        {
            return base.GetCollection<Dnl_Keyword>("Dnl_Keyword");
        }

        public IMongoCollection<Dnl_KeywordCategory> GetDnl_KeywordCategory()
        {
            return base.GetCollection<Dnl_KeywordCategory>("Dnl_KeywordCategory");
        }

        public IMongoCollection<Dnl_KeywordMapping> GetDnl_KeywordMapping()
        {
            return base.GetCollection<Dnl_KeywordMapping>("Dnl_KeywordMapping");
        }

        public IMongoCollection<Dnl_Link_Baidu> GetDnl_Link_Baidu()
        {
            return base.GetCollection<Dnl_Link_Baidu>("Dnl_Link_Baidu");
        }

        public IMongoCollection<Dnl_LinkMapping_Baidu> GetDnl_LinkMapping_Baidu()
        {
            return base.GetCollection<Dnl_LinkMapping_Baidu>("Dnl_LinkMapping_Baidu");
        }
        #endregion

        public IMongoCollection<IW2S_DomainCategory> GetIW2S_DomainCategorys()
        {
            return base.GetCollection<IW2S_DomainCategory>("IW2S_DomainCategory");
        }

        public IMongoCollection<IW2S_DomainCategoryData> GetIW2S_DomainCategoryDatas()
        {
            return base.GetCollection<IW2S_DomainCategoryData>("IW2S_DomainCategoryData");
        }
        public IMongoCollection<PojectChartMongo> GetPojectChart()
        {
            return base.GetCollection<PojectChartMongo>("PojectChart");
        }

        #region 社交媒体关键词与链接库
        public IMongoCollection<MediaKeywordMongo> GetMediaKeyword()
        {
            return base.GetCollection<MediaKeywordMongo>("MediaKeyword");
        }

        public IMongoCollection<MediaKeywordCategoryMongo> GetMediaKeywordCategory()
        {
            return base.GetCollection<MediaKeywordCategoryMongo>("MediaKeywordCategory");
        }

        public IMongoCollection<MediaKeywordMappingMongo> GetMediaKeywordMapping()
        {
            return base.GetCollection<MediaKeywordMappingMongo>("MediaKeywordMapping");
        }
        public IMongoCollection<WXLinkMainMongo> GetWXLinkMain()
        {
            return base.GetCollection<WXLinkMainMongo>("WXLinkMain");
        }

        public IMongoCollection<WXLinkOtherMongo> GetWXLinkOther()
        {
            return base.GetCollection<WXLinkOtherMongo>("WXLinkOther");
        }

        public IMongoCollection<WXLinkContentMongo> GetWXLinkContent()
        {
            return base.GetCollection<WXLinkContentMongo>("WXLinkContent");
        }

        public IMongoCollection<WXNameMongo> GetWXName()
        {
            return base.GetCollection<WXNameMongo>("WXName");
        }
        #endregion

        public IMongoCollection<ReferChartDescMongo> GetReferChartDesc()
        {
            return base.GetCollection<ReferChartDescMongo>("ReferChartDesc");
        }

        public IMongoCollection<ProductMongo> GetProduct()
        {
            return base.GetCollection<ProductMongo>("Product");
        }

        public IMongoCollection<OrderMongo> GetOrder()
        {
            return base.GetCollection<OrderMongo>("Order");
        }

        public IMongoCollection<Dnl_StopWord> GetDnl_StopWord()
        {
            return base.GetCollection<Dnl_StopWord>("Dnl_StopWord");
        }
    }

    public class Person
    {
        public string name { get; set; }
        public ObjectId _id;
    }
}
