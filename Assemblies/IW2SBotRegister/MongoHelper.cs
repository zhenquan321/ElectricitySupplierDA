
using MongoDB.Bson;
using MongoDB.Driver;
using MongoV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2SBotReg
{
    public class MongoDBHelper : MDB
    {
        // private static string conn = "mongodb://seedbo:SfNLP1225@119.254.210.251:27000/see";
        // private static string conn = "mongodb://mongo_user:SeE2017!#@119.254.210.251:27000/mongo_test";
        private static string conn = "mongodb://seedbo:SfNLP1225@43.240.138.216:28117/orectoryperdb";

        // private static string dbName = "see";
        // private static string dbName = "mongo_test";
        private static string dbName = "orectoryperdb";

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



        public IMongoCollection<IW2S_BotRegister> GetIW2S_BotRegister()
        {
            return GetCollection<IW2S_BotRegister>("IW2S_BotRegister");
        }
    }

    public class Person
    {
        public string name { get; set; }
        public ObjectId _id;
    }
}
