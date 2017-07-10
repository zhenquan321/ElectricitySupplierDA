using AISSystem;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iw2sDataAnalysis
{
    public class MongoDBClass<T>
    {

        private string conn = AppSettingHelper.GetAppSetting("mongoCon");

        private string dbName = AppSettingHelper.GetAppSetting("mongoDB");

        public MongoDatabase GetMongoDB()
        {
            MongoServer server = new MongoClient(conn).GetServer();
            var db = server.GetDatabase(dbName); // WriteConcern defaulted to Acknowledged

            return db;
        }

        public List<T> FindData(string tbName, IMongoQuery query)
        {
            MongoServer server = new MongoClient(conn).GetServer();
            var db = server.GetDatabase(dbName); // WriteConcern defaulted to Acknowledged
            ////创建数据连接
            //MongoServer server = MongoServer.Create(conn);
            ////获取指定数据库
            //MongoDatabase db = server.GetDatabase(dbName);
            //获取表
            MongoCollection<T> col = db.GetCollection<T>(tbName);
            var resultcur = col.Find(query);
            List<T> results = new List<T>();
            foreach (var result in resultcur)
            {
                results.Add(result);
            }
            return results;
        }
        public bool InsertData(T t, string tableName)
        {
            //创建数据连接
            MongoServer server = new MongoClient(conn).GetServer();
            //获取指定数据库
            MongoDatabase db = server.GetDatabase(dbName);

            MongoCollection<T> list = db.GetCollection<T>(tableName);

            var curResult = list.Insert(t);


            return curResult == null ? false : true;
        }
        public bool BatchInsertData(List<T> t, string tableName)
        {
            //创建数据连接
            MongoServer server = new MongoClient(conn).GetServer();
            //获取指定数据库
            MongoDatabase db = server.GetDatabase(dbName);

            MongoCollection<T> list = db.GetCollection<T>(tableName);

            var curResults = list.InsertBatch(t);

            foreach (var curResult in curResults)
            {
                if (curResult==null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Update(string tableName, IMongoQuery query, IMongoUpdate update)
        {
            //创建数据连接
            MongoServer server = new MongoClient(conn).GetServer();
            //获取指定数据库
            MongoDatabase db = server.GetDatabase(dbName);

            MongoCollection list = db.GetCollection(tableName);
            list.Update(query, update, UpdateFlags.Multi);

            return true;
        }


        public bool Delete(string tableName, string objId)
        {
            //创建数据连接
            MongoServer server = new MongoClient(conn).GetServer();
            //获取指定数据库
            MongoDatabase db = server.GetDatabase(dbName);
            //获取表
            MongoCollection<T> col = db.GetCollection<T>(tableName);
            IMongoQuery query = Query.EQ("_id", new ObjectId(objId));
            var bol = col.Remove(query);
            return bol == null ? false : true;
        }

        public T Findone(string tbName, IMongoQuery query)
        {
            MongoServer server = new MongoClient(conn).GetServer();
            var db = server.GetDatabase(dbName);
            MongoCollection<T> col = db.GetCollection<T>(tbName);
            var resultcur = col.FindOne(query);
            return resultcur;
        }

        public List<T> FindDataByTop(string tbName, IMongoQuery query, int topN, string orderby)
        {
            MongoServer server = new MongoClient(conn).GetServer();
            var db = server.GetDatabase(dbName); // WriteConcern defaulted to Acknowledged

            SortByDocument s = new SortByDocument();
            s.Add(orderby, -1); //-1=DESC  
            MongoCollection<T> col = db.GetCollection<T>(tbName);
            var resultcur = col.Find(query).SetSortOrder(s).SetLimit(topN);
            List<T> results = new List<T>();
            foreach (var result in resultcur)
            {
                results.Add(result);
            }
            return results;
        }

        public List<T> FindDataByTop(string tbName, IMongoQuery query, string orderby)
        {
            MongoServer server = new MongoClient(conn).GetServer();
            var db = server.GetDatabase(dbName); // WriteConcern defaulted to Acknowledged

            SortByDocument s = new SortByDocument();
            s.Add(orderby, -1); //-1=DESC  
            MongoCollection<T> col = db.GetCollection<T>(tbName);
            var resultcur = col.Find(query).SetSortOrder(s);
            List<T> results = new List<T>();
            foreach (var result in resultcur)
            {
                results.Add(result);
            }
            return results;
        }


    }
}