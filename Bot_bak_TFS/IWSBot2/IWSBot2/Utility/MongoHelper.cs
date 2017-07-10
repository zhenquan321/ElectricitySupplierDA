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
        public IMongoCollection<IW2S_ExcludeKeyword> GetIW2S_ExcludeKeywords()
        {
            return base.GetCollection<IW2S_ExcludeKeyword>("IW2S_ExcludeKeyword");
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
    }

    public class Person
    {
        public string name { get; set; }
        public ObjectId _id;
    }
}
