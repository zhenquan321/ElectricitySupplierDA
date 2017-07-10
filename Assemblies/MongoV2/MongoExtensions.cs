using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Driver.Builders;

namespace MongoV2
{
    public static class MongoExtensions
    {

        public static T FindOneById<T>(this IMongoCollection<T> col, ObjectId id) where T : Mbase
        {
            var builder = Builders<T>.Filter;
            var f = builder.Eq(x => x._id, id);
            var item = col.Find(f).FirstOrDefault();
            return item;
        }
        
        public static void DeleteById<T>(this IMongoCollection<T> col, ObjectId id) where T : Mbase
        {
            var builder = Builders<T>.Filter;
            var f = builder.Eq(x => x._id, id);
            col.DeleteOne(f);
        }
        public static void UpdateById<T>(this IMongoCollection<T> col, ObjectId id, UpdateDefinition<T> up) where T : Mbase
        {
            var builder = Builders<T>.Filter;
            var f = builder.Eq(x => x._id, id);
            col.UpdateOne(f, up);
        }

        public static long UpdateByIds<T>(this IMongoCollection<T> col, string ids, UpdateDefinition<T> up) where T : Mbase
        {
            var builder = Builders<T>.Filter;
            var f = builder.In(x => x._id, ids.SplitWith(";").Select(x => ObjectId.Parse(x)));
            var result = col.UpdateMany(f, up);
            return result.ModifiedCount;
        }

        public static void InsertDistinctNew<T>(this IMongoCollection<T> col, IEnumerable<T> items) where T : Mbase
        {
            if (items == null || items.Count() == 0)
                return;

            HashSet<ObjectId> ids = new HashSet<ObjectId>();
            List<T> list = new List<T>();
            foreach (var item in items)
            {
                if (ids.Add(item._id))
                {
                    list.Add(item);
                }
            }
            InsertNew(col, list);

        }
        public static void InsertNew<T>(this IMongoCollection<T> col, IEnumerable<T> items) where T : Mbase
        {
            if (items == null || items.Count() == 0)
                return;
            var ids = items.Select(x => x._id).ToList();
            var filters = Builders<T>.Filter;
            var fs = filters.In(x => x._id, ids);
            var existed = col.Find(fs).Project(r => r._id).ToList();
            if (existed != null && existed.Count > 0)
            {
                items = items.Where(x => !existed.Contains(x._id));
            }
            if (items == null || items.Count() == 0)
            {
                Console.WriteLine("No new item found");
                return;
            }
            Console.WriteLine("****** Save " + items.Count() + " new items");
            col.InsertMany(items);
        }

        
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
