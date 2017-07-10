using ProxyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iw2sDataAnalysis.Models;
using AISSystem;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using Iw2sDataAnalysis.Helper;
using MongoDB.Driver;

using MongoDB.Bson;
using System.Net;

namespace Iw2sDataAnalysis.Template
{

    public class WeChatQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;
        public WeChatQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);
            }
        }
        public List<Dnl_MappingCoPresent> BaiduQuery(Dnl_KeywordMapping searchTsk, List<Dnl_KeywordMapping> taskList)
        {
            List<Dnl_MappingCoPresent> linkvaluelist = new List<Dnl_MappingCoPresent>();
            BotTaskService bt = new BotTaskService();
            var keyIds = taskList.Select(x => x.KeywordId.ToString()).ToList();
            //获取链接数据
            List<Dnl_Link_BaiduDto> linklist = bt.GetBaiduLinkTitleList(keyIds);
            //获取当前计算映射的位置
            int index = 0;
            for (int i = 0; i < taskList.Count; i++)
            {
                if (searchTsk.Keyword == taskList[i].Keyword)
                {
                    index = i;
                }
            }
            //计算与其他映射关系
            for (int i = 0; i < taskList.Count; i++)
            {
                Dnl_MappingCoPresent lk = new Dnl_MappingCoPresent();
                lk.source = index;
                lk.target = i;
                lk.KeywordMappingId = searchTsk._id;
                lk.ProjectId = searchTsk.ProjectId;
                lk.CategoryId = searchTsk.CategoryId;
                int linkNum = 0;
                foreach (var item in linklist)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        if (item.Title.Contains(searchTsk.Keyword) && item.Title.Contains(taskList[i].Keyword))
                        {
                            linkNum = linkNum + 1;
                        }
                        else if (!string.IsNullOrEmpty(item.Description) && item.Description.Contains(searchTsk.Keyword) && item.Description.Contains(taskList[i].Keyword))
                        {
                            linkNum = linkNum + 1;
                        }
                    }
                }
                if (index == i && linkNum == 0)
                {
                    linkNum = 1;
                }
                lk.value = linkNum;
                linkvaluelist.Add(lk);
                //  }
            }
            SaveBaiduResult(linkvaluelist);
            return linkvaluelist;
        }
        #region Save Result

        void SaveBaiduResult(List<Dnl_MappingCoPresent> listings)
        {
            if (listings.Count <= 0)
            {
                return;
            }
            var shopList = listings;

            var col = MongoDBHelper.Instance.GetDnl_MappingCoPresent();
            var builder = Builders<Dnl_MappingCoPresent>.Filter;

            if (shopList == null || shopList.Count == 0)
                return;
            if (shopList.Count > 0)
            {
                col.InsertMany(shopList);
                log("to save for IW2S_Data");
                log("Done\n");
            }

        }

        #endregion

        public List<MediaMappingCoPresent> WeiXinQuery(MediaKeywordMappingMongo searchTsk, List<MediaKeywordMappingMongo> taskList)
        {
            List<MediaMappingCoPresent> linkvaluelist = new List<MediaMappingCoPresent>();
            BotTaskService bt = new BotTaskService();
            var keyIds = taskList.Select(x => x.KeywordId.ToString()).ToList();
            //获取链接数据
            List<Dnl_Link_BaiduDto> linklist = bt.GetWeiXininkTitleList(keyIds);
            //获取当前计算映射的位置
            int index = 0;
            for (int i = 0; i < taskList.Count; i++)
            {
                if (searchTsk.Keyword == taskList[i].Keyword)
                {
                    index = i;
                }
            }
            //计算与其他映射关系
            for (int i = 0; i < taskList.Count; i++)
            {
                MediaMappingCoPresent lk = new MediaMappingCoPresent();
                lk.source = index;
                lk.target = i;
                lk.KeywordMappingId = searchTsk._id;
                lk.ProjectId = searchTsk.ProjectId;
                lk.CategoryId = searchTsk.CategoryId;
                int linkNum = 0;
                foreach (var item in linklist)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        if (item.Title.Contains(searchTsk.Keyword) && item.Title.Contains(taskList[i].Keyword))
                        {
                            linkNum = linkNum + 1;
                        }
                        else if (!string.IsNullOrEmpty(item.Description) && item.Description.Contains(searchTsk.Keyword) && item.Description.Contains(taskList[i].Keyword))
                        {
                            linkNum = linkNum + 1;
                        }
                    }
                }
                if (index == i && linkNum == 0)
                {
                    linkNum = 1;
                }
                lk.value = linkNum;
                linkvaluelist.Add(lk);
                //  }
            }
            SaveWeiXinResult(linkvaluelist);
            return linkvaluelist;
        }
        #region Save Result

        void SaveWeiXinResult(List<MediaMappingCoPresent> listings)
        {
            if (listings.Count <= 0)
            {
                return;
            }
            var shopList = listings;

            var col = MongoDBHelper.Instance.GetMediaMappingCoPresent();
            var builder = Builders<MediaMappingCoPresent>.Filter;

            if (shopList == null || shopList.Count == 0)
                return;
            if (shopList.Count > 0)
            {
                col.InsertMany(shopList);
                log("to save for IW2S_Data");
                log("Done\n");
            }

        }

        #endregion


        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name + "/" + pages + "]:" + msg);
        }

    }

    public class SG_WeChatQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;
        public SG_WeChatQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);
            }
        }
        public List<SG_links> Query(IW2S_SG_BaiduCommend searchTsk, List<IW2S_SG_BaiduCommend> taskList)
        {
            List<SG_links> linkvaluelist = new List<SG_links>();
            SG_BotTaskService SG_bt = new SG_BotTaskService();
            List<IW2S_SG_level1link> linklist = SG_bt.GetLinkTitleList(searchTsk.ProjectId);
            int index = 0;
            for (int i = 0; i < taskList.Count; i++)
            {
                if (searchTsk.CommendKeyword == taskList[i].CommendKeyword)
                {
                    index = i;
                }
            }
            for (int i = 0; i < taskList.Count; i++)
            {
                SG_links lk = new SG_links();
                //if (searchTsk.CommendKeyword == taskList[i].CommendKeyword)
                //{
                //}
                //else
                //{
                lk.source = index;
                lk.target = i;
                lk.KeywordId = searchTsk._id;
                lk.ProjectId = searchTsk.ProjectId;
                int linkNum = 0;
                foreach (var item in linklist)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        if (item.Title.Contains(searchTsk.CommendKeyword) && item.Title.Contains(taskList[i].CommendKeyword))
                        {
                            linkNum = linkNum + 1;
                        }
                    }
                }
                lk.value = linkNum;
                lk.Gid = IDHelper.GetGuid("{0}/{1}/{2}/{3}".FormatStr(lk.source, lk.target, lk.KeywordId, lk.ProjectId));
                linkvaluelist.Add(lk);
                //  }
            }
            SG_SaveResult(linkvaluelist);
            return linkvaluelist;
        }
        #region Save Result

        void SG_SaveResult(List<SG_links> listings)
        {
            if (listings.Count <= 0)
            {
                return;
            }
            var shopList = listings;
            shopList = shopList.DistinctBy(x => x.Gid);

            FieldsDocument shopfd = new FieldsDocument();
            shopfd.Add("Gid", 1);
            //   MongoCollection<IDGuidDto> shopcol = MongoDBHelper<IDGuidDto>.GetMongoDB().GetCollection<IDGuidDto>("WX_Data");
            var col = MongoDBHelper.Instance.GetIW2S_SG_links();
            var builder = Builders<SG_links>.Filter;

            List<Guid> WeChatId = shopList.Select(x => x.Gid).ToList();
            //  var existsshop_objs = col.Find(builder.In(x => x.WeChatId, new BsonArray(WeChatId))).Project(x => x.WeChatId).ToList();
            //col.Find(MongoDB.Driver.Builders.Query.In("WeChatId", new BsonArray(WeChatId))).SetFields(shopfd);
            var existsshop_objs = col.Find(builder.In(x => x.Gid, WeChatId)).Project(x => x.Gid).ToList();
            List<Guid> exists_ids = new List<Guid>();
            foreach (var result in existsshop_objs)
            {
                exists_ids.Add(result);
            }
            if (existsshop_objs.Count() > 0)
            {
                shopList = shopList.Where(x => !exists_ids.Contains(x.Gid)).ToList();
            }
            if (shopList == null || shopList.Count == 0)
                return;
            if (shopList.Count > 0)
            {
                col.InsertMany(shopList);
                log("to save for IW2S_SG_Data");
                log("Done");
            }

        }

        #endregion




        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name + "/" + pages + "]:" + msg);
        }

    }

    public class WX_WeChatQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;
        public WX_WeChatQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_" + count);
            }
        }
        public List<WX_links> Query(IW2S_WX_BaiduCommend searchTsk, List<IW2S_WX_BaiduCommend> taskList)
        {
            List<WX_links> linkvaluelist = new List<WX_links>();
            WX_BotTaskService WX_bt = new WX_BotTaskService();
            List<IW2S_WX_level1link> linklist = WX_bt.GetLinkTitleList(searchTsk.ProjectId);
            int index = 0;
            for (int i = 0; i < taskList.Count; i++)
            {
                if (searchTsk.CommendKeyword == taskList[i].CommendKeyword)
                {
                    index = i;
                }
            }
            for (int i = 0; i < taskList.Count; i++)
            {
                WX_links lk = new WX_links();
                //if (searchTsk.CommendKeyword == taskList[i].CommendKeyword)
                //{
                //}
                //else
                //{
                lk.source = index;
                lk.target = i;
                lk.KeywordId = searchTsk._id;
                lk.ProjectId = searchTsk.ProjectId;
                int linkNum = 0;
                foreach (var item in linklist)
                {
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        if (item.Title.Contains(searchTsk.CommendKeyword) && item.Title.Contains(taskList[i].CommendKeyword))
                        {
                            linkNum = linkNum + 1;
                        }
                    }
                }
                lk.value = linkNum;
                lk.Gid = IDHelper.GetGuid("{0}/{1}/{2}/{3}".FormatStr(lk.source, lk.target, lk.KeywordId, lk.ProjectId));
                linkvaluelist.Add(lk);
                //  }
            }
            WX_SaveResult(linkvaluelist);
            return linkvaluelist;
        }
        #region Save Result

        void WX_SaveResult(List<WX_links> listings)
        {
            if (listings.Count <= 0)
            {
                return;
            }
            var shopList = listings;
            shopList = shopList.DistinctBy(x => x.Gid);

            FieldsDocument shopfd = new FieldsDocument();
            shopfd.Add("Gid", 1);
            //   MongoCollection<IDGuidDto> shopcol = MongoDBHelper<IDGuidDto>.GetMongoDB().GetCollection<IDGuidDto>("WX_Data");
            var col = MongoDBHelper.Instance.GetIW2S_WX_links();
            var builder = Builders<WX_links>.Filter;

            List<Guid> WeChatId = shopList.Select(x => x.Gid).ToList();
            //  var existsshop_objs = col.Find(builder.In(x => x.WeChatId, new BsonArray(WeChatId))).Project(x => x.WeChatId).ToList();
            //col.Find(MongoDB.Driver.Builders.Query.In("WeChatId", new BsonArray(WeChatId))).SetFields(shopfd);
            var existsshop_objs = col.Find(builder.In(x => x.Gid, WeChatId)).Project(x => x.Gid).ToList();
            List<Guid> exists_ids = new List<Guid>();
            foreach (var result in existsshop_objs)
            {
                exists_ids.Add(result);
            }
            if (existsshop_objs.Count() > 0)
            {
                shopList = shopList.Where(x => !exists_ids.Contains(x.Gid)).ToList();
            }
            if (shopList == null || shopList.Count == 0)
                return;
            if (shopList.Count > 0)
            {
                col.InsertMany(shopList);
                log("to save for IW2S_WX_Data");
                log("Done");
            }

        }

        #endregion




        void log(string mWX)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name + "/" + pages + "]:" + mWX);
        }

    }

}
