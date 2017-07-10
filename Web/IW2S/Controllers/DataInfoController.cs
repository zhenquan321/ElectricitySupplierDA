using System;
using System.Collections.Generic;
using System.Web.Http;
using IW2S.Helpers;
using IW2S.Models;
using MongoDB.Driver;
using IWSData.Model;
using MongoDB.Bson;

namespace IW2S.Controllers
{
    /// <summary>
    /// 获取数据统计信息
    /// </summary>
    public class DataInfoController : ApiController
    {

        /// <summary>
        /// 获取Bot列表
        /// </summary>
        [HttpGet]
        public List<IW2S_BotRegisterDto> GetBotList()
        {
            var list = new List<IW2S_BotRegisterDto>();
            var bots = MongoDBHelper.Instance.GetIW2S_BotRegister().Find(_=>true).ToList();
            foreach (var bot in bots)
            {
                var dto = new IW2S_BotRegisterDto
                {
                    _id = bot._id.ToString(),
                    BotId = bot.BotId,
                    HostName = bot.HostName,
                    ProcessId = bot.ProcessId,
                    IpAddress = bot.IpAddress,
                    RegTime = bot.RegTime.AddHours(8),
                    Status = bot.Status,
                    BotType = bot.BotType
                };
                list.Add(dto);
            }
            return list;
        }



        [HttpGet]
        public List<IW2S_BotDataDto> GetDataInfoIn72Hours(DateTime start, DateTime end)
        {
            var list = new List<IW2S_BotDataDto>();
            var datas = GetDataInfo("IW2S_BotDataIn72Hours", start, end);
            foreach (var data in datas)
            {
                var dto = new IW2S_BotDataDto
                {
                    _id = data._id.ToString(),
                    active_projects = data.active_projects,
                    active_users = data.active_users,
                    ins_time = data.ins_time,
                    keywords = data.keywords,
                    kw_complete = data.kw_complete,
                    kw_sch = data.kw_sch,
                    kw_wait = data.kw_wait,
                    projects = data.projects,
                    users = data.users,
                    links = data.links,
                    wb_links = data.wb_links,
                    wx_links = data.wx_links,
                    sg_links = data.sg_links,
                    img_links = data.img_links,
                    img_keywords =data.img_keywords,
                    img_kw_sch = data.img_kw_sch,
                    img_kw_complete = data.img_kw_complete,
                    img_kw_wait = data.img_kw_wait,
                    
                    sg_keywords = data.sg_keywords,
                    sg_kw_sch = data.sg_kw_sch,
                    sg_kw_complete = data.sg_kw_complete,
                    sg_kw_wait = data.sg_kw_wait,

                    wx_keywords = data.wx_keywords,
                    wx_kw_sch = data.wx_kw_sch,
                    wx_kw_complete = data.wx_kw_complete,
                    wx_kw_wait = data.wx_kw_wait,
                    
                    wb_keywords = data.wb_keywords,
                    wb_kw_sch = data.wb_kw_sch,
                    wb_kw_complete = data.wb_kw_complete,
                    wb_kw_wait = data.wb_kw_wait,
                };
                list.Add(dto);
            }

            return list;
        }
        
        /// <summary>
        /// 以天为单位获取数据
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_BotDataDto> GetDataInfoInDay(DateTime start, DateTime end)
        {
            var list = new List<IW2S_BotDataDto>();
            var datas = GetDataInfo("IW2S_BotDataInDay", start, end);
            foreach (var data in datas)
            {
                var dto = new IW2S_BotDataDto
                {
                    _id = data._id.ToString(),
                    active_projects = data.active_projects,
                    active_users = data.active_users,
                    ins_time = data.ins_time,
                    keywords = data.keywords,
                    kw_complete = data.kw_complete,
                    kw_sch = data.kw_sch,
                    kw_wait = data.kw_wait,
                    projects = data.projects,
                    users = data.users,
                    links = data.links,
                    wb_links = data.wb_links,
                    wx_links = data.wx_links,
                    sg_links = data.sg_links,
                    img_links = data.img_links,
                    img_keywords = data.img_keywords,
                    img_kw_sch = data.img_kw_sch,
                    img_kw_complete = data.img_kw_complete,
                    img_kw_wait = data.img_kw_wait,

                    sg_keywords = data.sg_keywords,
                    sg_kw_sch = data.sg_kw_sch,
                    sg_kw_complete = data.sg_kw_complete,
                    sg_kw_wait = data.sg_kw_wait,

                    wx_keywords = data.wx_keywords,
                    wx_kw_sch = data.wx_kw_sch,
                    wx_kw_complete = data.wx_kw_complete,
                    wx_kw_wait = data.wx_kw_wait,

                    wb_keywords = data.wb_keywords,
                    wb_kw_sch = data.wb_kw_sch,
                    wb_kw_complete = data.wb_kw_complete,
                    wb_kw_wait = data.wb_kw_wait,
                };
                list.Add(dto);
            }

            return list;
        }

        /// <summary>
        /// 获取控制台所需数据
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="isByDay">是否按天获取</param>
        /// <returns></returns>
        [HttpGet]
        public LineChartDto GetLineChartDatas(DateTime start, DateTime end, bool isByDay)
        {
            var lc = new LineChartDto()
            {
                Legend = new List<string>{"用户", "活跃用户", "项目", "活跃项目", "关键词",
                    "在搜关键词", "完成关键词", "等待关键词", "总链接","新增用户数","新增项目数","新增链接数",
                     "微博关键词","微博在搜关键词", "微博完成关键词", "微博等待关键词", "微博总链接","微博新增链接数",
                     "微信关键词","微信在搜关键词", "微信完成关键词", "微信等待关键词", "微信总链接","微信新增链接数",
                     "搜狗关键词","搜狗在搜关键词", "搜狗完成关键词", "搜狗等待关键词", "搜狗总链接","搜狗新增链接数",
                     "图片关键词","图片在搜关键词", "图片完成关键词", "图片等待关键词", "图片总链接","图片新增链接数",
                },
                LineAvProjs = new List<int>(),
                LineAvUsers = new List<int>(),
                LineKeywords = new List<int>(),
                LineKwComplete = new List<int>(),
                LineKwSearch = new List<int>(),
                LineKwWait = new List<int>(),
                LineLinks = new List<int>(),
                LineProjects = new List<int>(),
                LineUsers = new List<int>(),
                XAxis = new List<string>(),
                LineNewLinks=new List<int>(),
                LineNewProjs = new List<int>(),
                LineNewUsers = new List<int>(),
                LineWBKeywords = new List<int>(),
                LineWBKwComplete = new List<int>(),
                LineWBKwSearch = new List<int>(),
                LineWBKwWait = new List<int>(),
                LineWBLinks = new List<int>(),
                LineWBNewLinks = new List<int>(),
                LineWXKeywords = new List<int>(),
                LineWXKwComplete = new List<int>(),
                LineWXKwSearch = new List<int>(),
                LineWXKwWait = new List<int>(),
                LineWXLinks = new List<int>(),
                LineWXNewLinks = new List<int>(),
                LineSGKeywords = new List<int>(),
                LineSGKwComplete = new List<int>(),
                LineSGKwSearch = new List<int>(),
                LineSGKwWait = new List<int>(),
                LineSGLinks = new List<int>(),
                LineSGNewLinks = new List<int>(),
                LineImgKeywords = new List<int>(),
                LineImgKwComplete = new List<int>(),
                LineImgKwSearch = new List<int>(),
                LineImgKwWait = new List<int>(),
                LineImgLinks = new List<int>(),
                LineImgNewLinks = new List<int>(),
            };

            var datas = isByDay ? GetDataInfoInDay(start, end) : GetDataInfoIn72Hours(start, end);
            int i = 0;
            foreach (var p in datas)
            {
                lc.XAxis.Add(isByDay ? p.ins_time.ToString("yyyy-MM-dd") : 
                    p.ins_time.ToString("yyyy-MM-dd HH:mm"));    // 转为北京时间 UTC+8
                lc.LineAvProjs.Add(p.active_projects);
                lc.LineAvUsers.Add(p.active_users);
                lc.LineKeywords.Add(p.keywords);
                lc.LineKwComplete.Add(p.kw_complete);
                lc.LineKwSearch.Add(p.kw_sch);
                lc.LineKwWait.Add(p.kw_wait);
                lc.LineLinks.Add(p.links);
                lc.LineProjects.Add(p.projects);
                lc.LineUsers.Add(p.users);

                lc.LineWBKeywords.Add(p.wb_keywords);
                lc.LineWBKwComplete.Add(p.wb_kw_complete);
                lc.LineWBKwSearch.Add(p.wb_kw_sch);
                lc.LineWBKwWait.Add(p.wb_kw_wait);
                lc.LineWBLinks.Add(p.wb_links);

                lc.LineWXKeywords.Add(p.wx_keywords);
                lc.LineWXKwComplete.Add(p.wx_kw_complete);
                lc.LineWXKwSearch.Add(p.wx_kw_sch);
                lc.LineWXKwWait.Add(p.wx_kw_wait);
                lc.LineWXLinks.Add(p.wx_links);

                lc.LineSGKeywords.Add(p.sg_keywords);
                lc.LineSGKwComplete.Add(p.sg_kw_complete);
                lc.LineSGKwSearch.Add(p.sg_kw_sch);
                lc.LineSGKwWait.Add(p.sg_kw_wait);
                lc.LineSGLinks.Add(p.sg_links);

                lc.LineImgKeywords.Add(p.img_keywords);
                lc.LineImgKwComplete.Add(p.img_kw_complete);
                lc.LineImgKwSearch.Add(p.img_kw_sch);
                lc.LineImgKwWait.Add(p.img_kw_wait);
                lc.LineImgLinks.Add(p.img_links);
                if (i == 0)
                {
                    lc.LineNewUsers.Add(0);
                }
                else
                {
                    lc.LineNewUsers.Add(p.users - datas[i-1].users);
                }
                if (i == 0)
                {
                    lc.LineNewLinks.Add(0);
                }
                else
                {
                    lc.LineNewLinks.Add(p.links - datas[i - 1].links);
                }
                if (i == 0)
                {
                    lc.LineNewProjs.Add(0);
                }
                else
                {
                    lc.LineNewProjs.Add(p.projects - datas[i - 1].projects);
                }
                if (i == 0)
                {
                    lc.LineImgNewLinks.Add(0);
                }
                else
                {
                    lc.LineImgNewLinks.Add(p.img_links - datas[i - 1].img_links);
                }
                if (i == 0)
                {
                    lc.LineWBNewLinks.Add(0);
                }
                else
                {
                    lc.LineWBNewLinks.Add(p.wb_links - datas[i - 1].wb_links);
                }
                if (i == 0)
                {
                    lc.LineWXNewLinks.Add(0);
                }
                else
                {
                    lc.LineWXNewLinks.Add(p.wx_links - datas[i - 1].wx_links);
                }
                if (i == 0)
                {
                    lc.LineSGNewLinks.Add(0);
                }
                else
                {
                    lc.LineSGNewLinks.Add(p.sg_links - datas[i - 1].sg_links);
                }
                i++;
            }

            return lc;
        }

        /// <summary>
        /// 获取监测运行数据
        /// </summary>
        /// <param name="collection">获取数据时间单位</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        private List<IW2S_BotData> GetDataInfo(string collection, DateTime start, DateTime end)
        {
            var  datas = new List<IW2S_BotData>();
            var builder = Builders<IW2S_BotData>.Filter;
            start = start.AddHours(8);
            var filter = builder.Gt(x => x.ins_time, start);
            end = end.AddHours(8);
            filter &= builder.Lt(x => x.ins_time, end);   // 要这样转为UTC，不能直接减8小时，否则传到mongo时还会再减8小时。

            switch (collection)
            {
                case "IW2S_BotDataIn72Hours":
                    datas = MongoDBHelper.Instance.GetIW2S_BotDataIn6Hours().Find(filter).SortBy(x => x.ins_time).ToList();
                    break;
                case "IW2S_BotDataInDay":
                    datas = MongoDBHelper.Instance.GetIW2S_BotDataInDay().Find(filter).SortBy(x => x.ins_time).ToList();
                    break;
            }
            return datas;
        }
        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2SUserDto> GetUserInfoStatis()
        {
            QueryResult<IW2SUserDto> result = new QueryResult<IW2SUserDto>();

            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Ne(x => x.UsrRole, UserTypes.Admin) & builder.Ne(x => x.UsrRole, UserTypes.Engineer);
            var userInfos = MongoDBHelper.Instance.Get_IW2SUser().Find(filter)
                .Project(x => new IW2SUserDto
                {
                    _id = x._id.ToString(),
                    CreatedAt = x.CreatedAt,
                    LoginName = x.LoginName,
                    UsrEmail = x.UsrEmail,
                })
                .ToList();
            foreach(var ui in userInfos)
            {
                ui.ProjectCount = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x.UsrId, new ObjectId(ui._id)) & Builders<IW2S_Project>.Filter.Eq(x => x.IsDel, false)).Project(x => x._id).Count();
                ui.SG_KeywordCount = MongoDBHelper.Instance.GetIW2S_SG_BaiduCommends().Find(Builders<IW2S_SG_BaiduCommend>.Filter.Eq(x => x.UsrId, new ObjectId(ui._id)) & Builders<IW2S_SG_BaiduCommend>.Filter.Eq(x => x.IsRemoved, false)).Project(x => x._id).Count();
                var buiderMap = Builders<Dnl_KeywordMapping>.Filter;
                var filterMap = buiderMap.Eq(x => x.UserId, new ObjectId(ui._id)) & buiderMap.Eq(x => x.IsDel, false) & buiderMap.Eq(x => x.CategoryId, ObjectId.Empty);
                ui.BD_KeywordCount = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).Project(x => x._id).Count();
                ui.WB_KeywordCount = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends().Find(Builders<IW2S_WB_BaiduCommend>.Filter.Eq(x => x.UsrId, new ObjectId(ui._id)) & Builders<IW2S_WB_BaiduCommend>.Filter.Eq(x => x.IsRemoved, false)).Project(x => x._id).Count();
                ui.WX_KeywordCount = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(Builders<IW2S_WX_BaiduCommend>.Filter.Eq(x => x.UsrId, new ObjectId(ui._id)) & Builders<IW2S_WX_BaiduCommend>.Filter.Eq(x => x.IsRemoved, false)).Project(x => x._id).Count();
                ui.BDImg_KeywordCount = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks().Find(Builders<IW2S_ImgSearchTask>.Filter.Eq(x => x.UsrId, new ObjectId(ui._id)) & Builders<IW2S_ImgSearchTask>.Filter.Eq(x => x.IsDel, false)).Project(x => x._id).Count();
                
            }
            return result;
        } 
    }

    public class LineChartDto
    {
        public List<string> Legend { get; set; }
        public List<string> XAxis { get; set; }
        public List<int> LineProjects { get; set; }
        public List<int> LineUsers { get; set; }
        public List<int> LineLinks { get; set; }
        public List<int> LineKeywords { get; set; }
        public List<int> LineKwComplete { get; set; }
        public List<int> LineKwSearch { get; set; }
        public List<int> LineKwWait { get; set; }
        public List<int> LineAvProjs { get; set; }
        public List<int> LineAvUsers { get; set; }
        public List<int> LineNewUsers { get; set; }
        public List<int> LineNewProjs { get; set; }
        public List<int> LineNewLinks { get; set; }
        public List<int> LineWBKeywords {get;set;}
        public List<int> LineWBKwComplete {get;set;}
        public List<int> LineWBKwSearch {get;set;}
        public List<int> LineWBKwWait {get;set;}
        public List<int> LineWBLinks {get;set;}
        public List<int> LineWBNewLinks {get;set;}
        public List<int> LineWXKeywords {get;set;}
        public List<int> LineWXKwComplete {get;set;}
        public List<int> LineWXKwSearch {get;set;}
        public List<int> LineWXKwWait {get;set;}
        public List<int> LineWXLinks {get;set;}
        public List<int> LineWXNewLinks {get;set;}
        public List<int> LineSGKeywords {get;set;}
        public List<int> LineSGKwComplete {get;set;}
        public List<int> LineSGKwSearch {get;set;}
        public List<int> LineSGKwWait {get;set;}
        public List<int> LineSGLinks {get;set;}
        public List<int> LineSGNewLinks {get;set;}
        public List<int> LineImgKeywords {get;set;}
        public List<int> LineImgKwComplete {get;set;}
        public List<int> LineImgKwSearch {get;set;}
        public List<int> LineImgKwWait {get;set;}
        public List<int> LineImgLinks {get;set;}
        public List<int> LineImgNewLinks { get; set; }
    }
}
