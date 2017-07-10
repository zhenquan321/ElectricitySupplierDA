using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoV2;
using System.Configuration;
using MongoDB.Driver;
using DBHelper.Models.MongoDB;
using MongoDB.Bson;


namespace DBHelper
{
    public class MongoDBHelper:MDB
    {
    
        static string conn = ConfigurationManager.AppSettings["mongoCon"].ToString();
        static string dbName = ConfigurationManager.AppSettings["mongoDB"].ToString();


        public MongoDBHelper()
            : base(conn, dbName){}

        public static readonly MongoDBHelper Instance = new MongoDBHelper();
        
        //public IMongoCollection<WolongWeiboTask> GetWolongWeiboTask()
        //{
        //    return base.GetCollection<WolongWeiboTask>("Dnl_WolongWeiboTask");
        //}


        public IMongoCollection<IW2S_WB_BaiduCommend> GetIW2S_WB_BaiduCommends()
        {
            return base.GetCollection<IW2S_WB_BaiduCommend>("IW2S_WB_BaiduCommend");
        }

        public IMongoCollection<IW2S_WB_level1link> GetIW2S_WB_level1links()
        {
            return base.GetCollection<IW2S_WB_level1link>("IW2S_WB_level1link");
        }

        public IMongoCollection<IW2S_BaiduCommend> GetIW2S_BaiduCommends()
        {
            return base.GetCollection<IW2S_BaiduCommend>("IW2S_BaiduCommend");
        }

        public IMongoCollection<IW2S_level1link> GetIW2S_level1links()
        {
            return base.GetCollection<IW2S_level1link>("IW2S_level1link");
        }

        public IMongoCollection<IW2S_ExcludeKeyword> GetIW2S_ExcludeKeywords()
        {
            return base.GetCollection<IW2S_ExcludeKeyword>("IW2S_ExcludeKeyword");
        }
        public IMongoCollection<IW2S_KeywordFilter> GetIW2S_KeywordFilters()
        {
            return base.GetCollection<IW2S_KeywordFilter>("IW2S_KeywordFilter");
        }
    }
}
