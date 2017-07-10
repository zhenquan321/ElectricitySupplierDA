using QiDianData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using System.Data;
using SogouSearch.Models;
using MongoDB.Driver;
using SogouSearch.Helper;

namespace SogouSearch.BotTask
{
    public class BotTaskService
    {

        //public NovelLibrary GetNovel(string conn)
        //{
        //    try
        //    {
        //        string sql = "select top 1 Id,NovelName,RealName,AuthorName from NovelLibrary where IsBot=0";
        //        DataTable dt = DBHelper.Query(conn, sql);
        //        NovelLibrary nl = new NovelLibrary();
        //        if (dt.Rows.Count > 0)
        //        {
        //            nl.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
        //            nl.NovelName = dt.Rows[0]["NovelName"].ToString();
        //            nl.RealName = dt.Rows[0]["RealName"].ToString();
        //            nl.AuthorName = dt.Rows[0]["AuthorName"].ToString();
        //        }
        //        return nl;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Get NovelLibrary task error: {0}".FormatStr(ex.Message));
        //        return null;
        //    }
        //}



        //public List<WhiteList> GetWhiteList(string conn)
        //{
        //    string sql = "select Id,AuthorizedName,AuthorizedUrl1,AuthorizedUrl2 from WhiteList";
        //    DataTable dt = DBHelper.Query(conn, sql);
        //    List<WhiteList> list = new List<WhiteList>();
        //    if (dt.Rows.Count > 0)
        //    {

        //        foreach (DataRow item in dt.Rows)
        //        {
        //            WhiteList nl = new WhiteList();
        //            nl.Id = Convert.ToInt32(item["Id"]);
        //            nl.AuthorizedName = item["AuthorizedName"].ToString();
        //            nl.AuthorizedUrl1 = item["AuthorizedUrl1"].ToString();
        //            nl.AuthorizedUrl2 = item["AuthorizedUrl2"].ToString();
        //            list.Add(nl);
        //        }
        //    }
        //    return list;
        //}


        //public List<BlackList> GetBlackList(string conn)
        //{
        //    string sql = "select Id, Domain, DomainType from BlackList";
        //    DataTable dt = DBHelper.Query(conn, sql);
        //    List<BlackList> list = new List<BlackList>();
        //    if (dt.Rows.Count > 0)
        //    {

        //        foreach (DataRow item in dt.Rows)
        //        {
        //            BlackList nl = new BlackList();
        //            nl.Id = Convert.ToInt32(item["Id"]);
        //            nl.Domain = item["Domain"].ToString();
        //            nl.DomainType = item["DomainType"].ToString();
        //            list.Add(nl);
        //        }
        //    }
        //    return list;
        //}


        static object taskToken = new object();
        string com = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");
        Queue<IW2S_SG_BaiduCommend> searchkeyword_pool = new Queue<IW2S_SG_BaiduCommend>();
        public IW2S_SG_BaiduCommend GetBotTask()
        {
            lock (taskToken)
            {
                try
                {

                    var builder = Builders<IW2S_SG_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.WXStatus, 0);
                    //filter &= builder.Eq(x => x.BotStatus, 0);
                    // filter &= builder.Lte(x => x.LastBotEndAt, dt);
                    var col = MongoDBHelper.Instance.Get_IW2S_SG_BaiduCommend();
                    var result = col.Find(filter).FirstOrDefault();//
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}".FormatStr(result.CommendKeyword));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get keywords task error: {0}".FormatStr(ex.Message));
                    return null;
                }
            }
        }


    }
}
