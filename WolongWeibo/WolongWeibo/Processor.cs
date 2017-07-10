using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoV2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DBHelper.Models;
using DBHelper;
using MongoDB.Bson;
using MongoDB.Driver;
using DBHelper.Models.MongoDB;


namespace WolongWeibo
{
    public class SaveWolongWeiboData
    {
        public static void SaveData(JObject wlData,IW2S_WB_BaiduCommend wbb)
        {
            var taskid = wlData["data"]["task_id"].Value<int>();
            var keywords = wlData["data"]["keyword_list"];

            foreach (var item in wlData["data"]["item_list"])
            {
                item["task_id"] = taskid;
                item["keywords"] = keywords;
                var s = item.ToString();
                var bson = BsonDocument.Parse(s);

                var postUrl = item["weibo_url"].ToString();
                var abs = item["text"].ToString();
                //var c = MongoDBHelper.Instance.GetCollection<BsonDocument>("Dnl_WeiboItems");
                //c.UpdateOne(
                //    Builders<BsonDocument>.Filter.Eq("task_id", taskid) & Builders<BsonDocument>.Filter.Eq("weibo_url", item["weibo_url"].Value<string>()),
                //new BsonDocument { { "$set", bson} },
                //new UpdateOptions{IsUpsert = true});                


                var userList = item["user"];

                string PosterUrl = "";
                string weibo_face = "";

                string nickName = "";
                int rank = 0;
                bool IsBlueV = false;
                //foreach (var item2 in userList)
                //{
                nickName = userList["screen_name"].ToString();
                weibo_face = userList["profile_image_url"].ToString();
                PosterUrl = "http://weibo.com/" + userList["profile_url"].ToString();
                string urank = userList["urank"].ToString();
                    rank = int.Parse(urank);
                    IsBlueV = bool.Parse(userList["verified"].ToString());
              //  }

                IW2S_WB_level1link linkData = new IW2S_WB_level1link();
                linkData.PosterUrl = PosterUrl;
                linkData.PostUrl = postUrl;
                linkData.HeadIcon = weibo_face;
                linkData.NickName = nickName;
                linkData.Description = abs;
                linkData.IsBlueV = IsBlueV;
                linkData.UsrId = wbb.UsrId;
                linkData.Keywords = wbb.Keyword;
                linkData.CreatedAt = DateTime.Now.AddHours(8);
                linkData.IsDel = false;
                linkData.ProjectId = wbb.ProjectId;
                linkData.Rank = rank;
                linkData.SearchkeywordId = wbb._id;
                linkData.BizId = string.Format("{0}{1}", postUrl, wbb._id.ToString()).ToObjectId();
                save_level1_links(new List<IW2S_WB_level1link> { linkData }, wbb);

            }
        }

        public static void save_level1_links(List<IW2S_WB_level1link> links,
          IW2S_WB_BaiduCommend tsk)
        {

            if (links == null || links.Count == 0)
            {
                Console.WriteLine("SUCCESS saving 0 Level 1 Links for " + tsk.Keyword);
                return;
            }

            int pagesize = 100;
            int count = 0;
            var col = MongoDBHelper.Instance.GetIW2S_WB_level1links();
            var builder = Builders<IW2S_WB_level1link>.Filter;
            for (int page = 0; page * pagesize < links.Count; page++)
            {
                var list = links.Skip(page * pagesize).Take(pagesize).ToList();
                //list.ForEach(x => x._id = new MongoDB.Bson.ObjectId(IDHelper.GetGuid("{0}/&itemid={1}".FormatStr(x.Domain, x.LinkUrl)).ToString()));
                list = ListDistinctBy(list, x => x.BizId);
                FieldsDocument fd = new FieldsDocument();
                fd.Add("BizId", 1);
                List<ObjectId> bizIds = list.Select(x => x.BizId).ToList();
                var exists_objs = col.Find(builder.In(x => x.BizId, bizIds)).Project(x => x.BizId).ToList();
                List<ObjectId> exists_ids = new List<ObjectId>();
                foreach (var result in exists_objs)
                {
                    exists_ids.Add(result);
                }
                if (exists_ids != null && exists_ids.Count > 0)
                {
                    list = list.Where(x => !exists_ids.Contains(x.BizId)).ToList();
                }
                if (list == null || list.Count == 0)
                    continue;
                count += pagesize;
                col.InsertMany(links);
                Console.WriteLine("SUCCESS saving " + links.Count + " Level 1 Links for " + tsk.Keyword);
            }

        }


        public static List<T> ListDistinctBy<T>(List<T> collection, Func<T, object> selector)
        {
            if (collection == null)
                return null;
            List<T> list = new List<T>();
            var gs = collection.GroupBy(x => selector(x));
            foreach (var g in gs)
            {
                list.Add(g.First());
            }
            return list;
        }

    }
}
