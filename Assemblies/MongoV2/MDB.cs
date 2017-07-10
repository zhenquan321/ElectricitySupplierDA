using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoV2
{
    public class MDB
    {
        MongoClient client;
        string db;
        public MDB(string con, string dbName)
        {
            client = new MongoClient(con);
            db = dbName;
        }

        public IMongoDatabase GetDb()
        {
            var d = client.GetDatabase(db);
            return d;
        }

        public IMongoCollection<T> GetCollection<T>(string col)
        {
            var d = GetDb();
            var c = d.GetCollection<T>(col);
            return c;
        }
    }

    public class Mbase
    {
        public ObjectId _id { get; set; }
    }
}
