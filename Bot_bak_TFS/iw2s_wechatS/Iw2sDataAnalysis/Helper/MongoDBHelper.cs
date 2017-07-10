using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoV2;
using Iw2sDataAnalysis.Models;

namespace Iw2sDataAnalysis.Helper
{
    public class MongoDBHelper:MDB
    {

        private static string conn = AppSettingHelper.GetAppSetting("mongoCon");
        
        private static string dbName = AppSettingHelper.GetAppSetting("mongoDB");


        public MongoDBHelper()
            : base(conn, dbName)
        {

        }

        public IMongoDatabase GetMongoDB()
        {
            return base.GetDb();
        }


        public static readonly MongoDBHelper Instance = new MongoDBHelper();


        public IMongoCollection<IW2SUser> Get_IW2SUser()
        {
            return base.GetCollection<IW2SUser>("IW2SUser");
        }


        public IMongoCollection<IW2S_BaiduCommend> Get_IW2S_BaiduCommend()
        {
            return base.GetCollection<IW2S_BaiduCommend>("IW2S_BaiduCommend");
        }

        public IMongoCollection<IW2S_WX_BaiduCommend> Get_IW2S_WX_BaiduCommend()
        {
            return base.GetCollection<IW2S_WX_BaiduCommend>("IW2S_WX_BaiduCommend");
        }

        public IMongoCollection<IW2S_SG_BaiduCommend> Get_IW2S_SG_BaiduCommend()
        {
            return base.GetCollection<IW2S_SG_BaiduCommend>("IW2S_SG_BaiduCommend");
        }

        public IMongoCollection<IW2S_Data> Get_IW2S_Data()
        {
            return base.GetCollection<IW2S_Data>("IW2S_Data");
        }

        public IMongoCollection<IW2S_BaiduKeyword> GetIW2S_BaiduKeywords()
        {
            return base.GetCollection<IW2S_BaiduKeyword>("IW2S_BaiduKeyword");
        }

       
        public IMongoCollection<IW2S_SearchKeyword> GetIW2S_SearchKeywords()
        {
            return base.GetCollection<IW2S_SearchKeyword>("IW2S_SearchKeyword");
        }

        public IMongoCollection<IW2S_level1link> GetIW2S_level1links()
        {
            return base.GetCollection<IW2S_level1link>("IW2S_level1link");
        }

        public IMongoCollection<IW2S_KeywordCategory> GetIW2S_KeywordCategorys()
        {
            return base.GetCollection<IW2S_KeywordCategory>("IW2S_KeywordCategory");
        }
        public IMongoCollection<IW2S_KeywordGroup> GetIW2S_KeywordGroups()
        {
            return base.GetCollection<IW2S_KeywordGroup>("IW2S_KeywordGroup");
        }

        public IMongoCollection<IW2S_WX_KeywordCategory> GetIW2S_WX_KeywordCategorys()
        {
            return base.GetCollection<IW2S_WX_KeywordCategory>("IW2S_WX_KeywordCategory");
        }
        public IMongoCollection<IW2S_WX_KeywordGroup> GetIW2S_WX_KeywordGroups()
        {
            return base.GetCollection<IW2S_WX_KeywordGroup>("IW2S_WX_KeywordGroup");
        }

        public IMongoCollection<IW2S_SG_KeywordCategory> GetIW2S_SG_KeywordCategorys()
        {
            return base.GetCollection<IW2S_SG_KeywordCategory>("IW2S_SG_KeywordCategory");
        }
        public IMongoCollection<IW2S_SG_KeywordGroup> GetIW2S_SG_KeywordGroups()
        {
            return base.GetCollection<IW2S_SG_KeywordGroup>("IW2S_SG_KeywordGroup");
        }

        public IMongoCollection<IW2S_KeywordFilter> GetIW2S_KeywordFilters()
        {
            return base.GetCollection<IW2S_KeywordFilter>("IW2S_KeywordFilter");
        }

        public IMongoCollection<IW2S_Project> GetIW2S_Projects()
        {
            return base.GetCollection<IW2S_Project>("IW2S_Project");
        }


        public IMongoCollection<links> GetIW2S_links()
        {
            return base.GetCollection<links>("IW2S_links");
        }

        public IMongoCollection<WX_links> GetIW2S_WX_links()
        {
            return base.GetCollection<WX_links>("IW2S_WX_links");
        }

        public IMongoCollection<SG_links> GetIW2S_SG_links()
        {
            return base.GetCollection<SG_links>("IW2S_SG_links");
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

        public IMongoCollection<Dnl_MappingCoPresent> GetDnl_MappingCoPresent()
        {
            return base.GetCollection<Dnl_MappingCoPresent>("Dnl_MappingCoPresent");
        }
        #endregion

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

        public IMongoCollection<MediaMappingCoPresent> GetMediaMappingCoPresent()
        {
            return base.GetCollection<MediaMappingCoPresent>("MediaMappingCoPresent");
        }
        #endregion
    }
    public class Person
    {
        public string name { get; set; }
        public ObjectId _id;
    }

}
