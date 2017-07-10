using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoV2;
using BingS.Models;

namespace BingS
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

        public IMongoCollection<IW2S_Bing_BaiduCommend> Get_IW2S_Bing_BaiduCommend()
        {
            return base.GetCollection<IW2S_Bing_BaiduCommend>("IW2S_Bing_BaiduCommend");
        }


        public IMongoCollection<IW2S_Bing_level1link> Get_IW2S_Bing_level1link()
        {
            return base.GetCollection<IW2S_Bing_level1link>("IW2S_Bing_level1link");
        }

       

    }
    public class Person
    {
        public string name { get; set; }
        public ObjectId _id;
    }

}
