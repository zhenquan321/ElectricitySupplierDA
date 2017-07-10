using Iw2sDataAnalysis.Helper;
using Iw2sDataAnalysis.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Iw2sDataAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            LogerHelper.SetConfig();
            Thread t = new Thread(new ThreadStart(() =>
            {
                Iw2sDataAnalysis.Search.BotSearch.Instance.Run();
            }));
            t.Start();

            Console.ReadLine();
            
        }


        static void setindex()
        {
            //  var builder = Builders<IW2S_KeywordCategory>.Filter;
            //  var filter = builder.Eq(x => x.ProjectId, prijid);
            // filter &= builder.Lte(x => x.LastBotEndAt, dt);
            //var col = MongoDBHelper.Instance.Get_IW2S_BaiduCommend();
            //var result = col.Find();//

            MongoCollection<IW2S_KeywordCategory> col = new MongoDBClass<IW2S_KeywordCategory>().GetMongoDB().GetCollection<IW2S_KeywordCategory>("IW2S_KeywordCategory");
            var categoryList = col.FindAll();
            var clist = categoryList.ToList();

            MongoCollection<IW2S_KeywordGroup> col2 = new MongoDBClass<IW2S_KeywordGroup>().GetMongoDB().GetCollection<IW2S_KeywordGroup>("IW2S_KeywordGroup");
            var groupList = col2.FindAll();

            var glist = groupList.ToList();

            int categoryIndex = 0;
            for (int i = 0; i < clist.Count; i++)
            {
                categoryIndex = i + 1;
                var id = clist[i]._id;

                var projlist = glist.Where(x => x.CommendCategoryId == id);

                foreach (var item in projlist)
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "GroupNumber", categoryIndex } } } };
                    var result = MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateOne(new QueryDocument { { "_id", item.BaiduCommendId } }, update);
                    Console.WriteLine("完成关键词位置标注：" + item.BaiduCommendId + "/" + item.BaiduCommend);
                }

                var update1 = new UpdateDocument { { "$set", new QueryDocument { { "GroupNumber", categoryIndex } } } };
                var result1 = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().UpdateOne(new QueryDocument { { "_id", id } }, update1);
                Console.WriteLine("完成分组位置标注：" + clist[i]._id + "/");
            }


            Console.WriteLine("完成!");

        }



        static void setindex2()
        {

            //var queryTask = new QueryDocument { { "SearchSource", 1 }, { "IsRemoved", false } };
            //MongoCollection<IW2S_BaiduCommend> col = new MongoDBClass<IW2S_BaiduCommend>().GetMongoDB().GetCollection<IW2S_BaiduCommend>("IW2S_BaiduCommend");
            //var categoryList = col.Find(queryTask);
            //var clist = categoryList.ToList();

            //foreach (var item in clist)
            //{
            //    var update = new UpdateDocument { { "$set", new QueryDocument { { "JisuanStatus", 0 } } } };
            //    var result = MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateOne(new QueryDocument { { "_id", item._id } }, update);
            //    Console.WriteLine("完成关键词状态标注：" + item._id + "/" + item.CommendKeyword);
            //}


            var update = new UpdateDocument { { "$set", new QueryDocument { { "AlternateFields", "0" } } } };
            var result = MongoDBHelper.Instance.GetIW2S_level1links().UpdateMany(new QueryDocument { { "AppType", 0 } }, update);
            Console.WriteLine("完成修改");


            Console.WriteLine("完成!");

        }



        public static void Setdate()
        {


            while (true)
            {


                MongoDBClass<IW2S_level1link> helper = new MongoDBClass<IW2S_level1link>();
                var queryTask = new QueryDocument { { "AlternateFields", "0" } };
                List<IW2S_level1link> list = new List<IW2S_level1link>();
                FieldsDocument fd = new FieldsDocument();
                fd.Add("_id", 1);
                fd.Add("LinkUrl", 1);
                fd.Add("Html", 1);
                MongoCollection<IW2S_level1link> col = new MongoDBClass<IW2S_level1link>().GetMongoDB().GetCollection<IW2S_level1link>("IW2S_level1link");
                var TaskList = col.Find(queryTask).SetFields(fd).SetLimit(1).ToList();
                if (TaskList.Count > 0)
                {
                    try
                    {
                        // Pattern p = Pattern.compile("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)", Pattern.CASE_INSENSITIVE|Pattern.MULTILINE);
                        // public string PublishTime { get; set; }
                        //public string AlternateFields { get; set; }
                        foreach (var item in TaskList)
                        {

                            Regex reg = new Regex("(20\\d{2}[-/]\\d{1,2}[-/]\\d{1,2})|(20\\d{2}年\\d{1,2}月\\d{1,2}日)");
                            Match m = reg.Match(item.Html);
                            //MatchCollection cols = reg.Matches(item.Html);
                            string time = "";
                            if (m.Groups.Count > 0)
                            {
                                time = m.Groups[0].Value;
                            }


                            var update = new UpdateDocument { { "$set", new QueryDocument { { "PublishTime", time }, { "AlternateFields", "1" } } } };
                            var result = MongoDBHelper.Instance.GetIW2S_level1links().UpdateOne(new QueryDocument { { "_id", item._id } }, update);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("已经完成！");
                }


            }

        }


        public static void removeKeyword()
        {

            var queryTask = new QueryDocument { { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
            // var queryTask = new QueryDocument { { "UId", new ObjectId("56c6943697c21d0ffc3fc478") },{ "IsStarted", true } };
            var col = MongoDBHelper.Instance.GetIW2S_Projects();
            var result = col.Find(queryTask).ToList();

            foreach (var item in result)
            {



                var queryTask5 = new QueryDocument { { "ProjectId", item._id }, { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
                var groups3 = MongoDBHelper.Instance.GetIW2S_level1links().DeleteMany(queryTask5).DeletedCount;
                Console.WriteLine(DateTime.Now + "  " + item.Name + " 链接清除完成！");


                var queryTask2 = new QueryDocument { { "ProjectId", item._id }, { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
                var groups = MongoDBHelper.Instance.GetIW2S_KeywordGroups().DeleteMany(queryTask2).DeletedCount;
                Console.WriteLine(DateTime.Now + "  " + item.Name + " 分组关系信息清除完成！");


                var queryTask4 = new QueryDocument { { "ProjectId", item._id }, { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
                var groups2 = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().DeleteMany(queryTask4).DeletedCount;
                Console.WriteLine(DateTime.Now + "  " + item.Name + " 分组信息清除完成！");



                var queryTask1 = new QueryDocument { { "ProjectId", item._id }, { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
                var ssdfs = MongoDBHelper.Instance.Get_IW2S_BaiduCommend().DeleteMany(queryTask1).DeletedCount;
                Console.WriteLine(DateTime.Now + "  " + item.Name + " 百度推荐词清除完成！");


                var queryTask3 = new QueryDocument { { "ProjectId", item._id }, { "UsrId", new ObjectId("56fb46ac1b2d6417d06d4439") } };
                var shops = MongoDBHelper.Instance.GetIW2S_BaiduKeywords().DeleteMany(queryTask3).DeletedCount;
                Console.WriteLine(DateTime.Now + "  " + item.Name + " 百度关键词清除完成！");


                var builder = Builders<IW2S_Project>.Filter;
                var filter = builder.Eq(x => x._id, item._id);
                filter &= builder.Eq(x => x.UsrId, new ObjectId("56fb46ac1b2d6417d06d4439"));
                var result1 = MongoDBHelper.Instance.GetIW2S_Projects().DeleteOne(filter).DeletedCount;
                Console.WriteLine(DateTime.Now + item.Name + "  项目清除完成！");


            }
            Console.WriteLine(DateTime.Now + "  用户数据清除完成！");

            var builder2 = Builders<IW2SUser>.Filter;
            var filter2 = builder2.Eq(x => x._id, new ObjectId("56fb46ac1b2d6417d06d4439"));
            var result12 = MongoDBHelper.Instance.Get_IW2SUser().DeleteOne(filter2).DeletedCount;
            Console.WriteLine(DateTime.Now + "  账号清除完成！");
        }


        //	"ProjectId" : ObjectId("571e28d76ce8b80cb8963e8d"),
        static void update()
        {
            var update = new UpdateDocument { { "$set", new QueryDocument { { "WXStatus", 2 }, { "BotStatus", 2 } } } };

            var result = Helper.MongoDBHelper.Instance.Get_IW2S_BaiduCommend().UpdateMany(new QueryDocument { { "ProjectId", new ObjectId("5875b6fbcba12d0870ceb303") } }, update);
            Console.WriteLine("完成");
        }


    }
}
