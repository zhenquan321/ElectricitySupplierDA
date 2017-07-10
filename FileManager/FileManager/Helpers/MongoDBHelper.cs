using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoV2;
using FileManager.Models;

namespace FileManager.Helpers
{
    public class MongoDBHelper:MDB
    {

        static string conn = AppSettingHelper.GetAppSetting("mongoCon");

        static string dbName = AppSettingHelper.GetAppSetting("mongoDB");


        public MongoDBHelper()
            : base(conn, dbName)
        {

        }

        public static readonly MongoDBHelper Instance = new MongoDBHelper();


        public IMongoCollection<Dnl_File> GetDnl_File()
        {
            return base.GetCollection<Dnl_File>("Dnl_File");
        }


       
    }

    public static class MongoExtension
    {
        public static ObjectId ToObjectId(this string txt)
        {
            var bs = Encoding.Default.GetBytes(txt);
            byte[] arr = new byte[12];
            foreach (var b in bs)
            {
                Mutl(arr, 31);
                Add(arr, b);
            }
            string hex = GetHex(arr);
            return ObjectId.Parse(hex);
        }


        static void Mutl(byte[] bytes, byte a)
        {
            uint preAdd = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                uint b = ((uint)bytes[i]) * ((uint)a) + preAdd;
                preAdd = b / 256;
                bytes[i] = (byte)(b % 256);
            }
        }

        static void Add(byte[] bytes, byte a)
        {
            uint preAdd = a;
            for (var i = 0; i < bytes.Length; i++)
            {
                uint b = ((uint)bytes[i]) + preAdd;
                bytes[i] = (byte)(b % 256);
                if (b < 256)
                    break;
                preAdd = b / 256;
            }
        }

        static string GetHex(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < bs.Length; i++)
            {
                sb.Append(bs[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
