using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AISSystem;
using System.Data;
using IWSBot.Queries;
using IWSData.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using MongoV2;

namespace IWSBot.Utility
{
    /// <summary>
    /// 按天记录网站状态
    /// </summary>
    public class BotControlPanelByDay
    {
        BotControlPanelByDay()
        {

        }

        public static readonly BotControlPanelByDay Instance = new BotControlPanelByDay();

        /// <summary>
        /// 上次计算时间
        /// </summary>
        DateTime LastUpdateTime = new DateTime();

        public void Run()
        {
            while (true)
            {
                //判断是否开始计算
                DateTime now = DateTime.Now;
                if (now.Hour == 0)
                {
                    if (now.Date == LastUpdateTime.Date)
                    {
                        //如果今天已经计算过一次，睡眠23小时
                        Thread.Sleep(23 * 60 * 60 * 1000);
                        continue;
                    }
                }
                else
                {
                    //休眠一段时间
                    log("未至计算时间！");
                    Random r = new Random();
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }

                try
                {
                    log("开始计算！");
                    ConsoleCount();
                }
                catch (Exception ex)
                {
                    log("错误原因：" + ex.Message);
                    Thread.Sleep(5000);
                }
            }

        }

        /// <summary>
        /// 计算函数
        /// </summary>
        void ConsoleCount()
        {
            DateTime now = DateTime.Now;
            //计算控制台数据
            IW2S_BotData data = new IW2S_BotData();
            data.ins_time = DateTime.Now.AddHours(8);
            //获取总用户数
            List<UserTypes> exTypes = new List<UserTypes> { UserTypes.Admin, UserTypes.Engineer };
            var buiderUser = Builders<IW2SUser>.Filter;
            var filterUser = buiderUser.Nin(x => x.UsrRole, exTypes);
            var users = MongoDBHelper.Instance.Get_IW2SUser().Find(filterUser).ToList();
            data.users = users.Count;
            //获取活跃用户数
            int acUser = 0;
            foreach (var user in users)
            {
                //登陆时间在7天之内的用户为活跃用户
                var timeInter = now - user.LastLoginAt;
                if (timeInter.Days < 7)
                {
                    acUser++;
                }
            }
            data.active_users = acUser;
            //获取总项目数
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x.IsDel, false);
            var pros = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).ToList();
            data.projects = pros.Count;
            //获取活跃项目数
            var builderProLink = Builders<IW2S_ProLinksCount>.Filter;
            var colProLink = MongoDBHelper.Instance.GetIW2S_ProLinksCount();
            var acPro = 0;
            foreach (var pro in pros)
            {
                //获取最新链接数变化时间
                var filterProLink = builderProLink.Eq(x => x.ProjectId, pro._id);
                var queryProLink = colProLink.Find(filterProLink).ToList().OrderByDescending(x => x.CreatedAt).ToList();
                if(queryProLink.Count>0)
                {
                    var lastTime = queryProLink[0].CreatedAt;
                    //7天之内链接数有变化的项目为活跃项目
                    var timeInter = now - lastTime;
                    if (timeInter.Days < 7)
                    {
                        acPro++;
                    }
                }
            }
            data.active_projects = acPro;
            //获取百度关键词总数
            var builderKey = Builders<IW2S_BaiduCommend>.Filter;
            var filterKey = builderKey.Eq(x => x.IsRemoved, false);
            var colKey = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var queryKey = colKey.Find(filterKey).ToList();
            data.keywords = queryKey.Count;
            //获取各状态下的百度关键词
            int waitKey = 0;                //未搜索关键词
            int searchKey = 0;              //正在搜索关键词
            int completeKey = 0;            //已完成关键词
            foreach (var key in queryKey)
            {
                switch (key.BotStatus)
                {
                    case 0:
                        waitKey++;
                        break;
                    case 1:
                        searchKey++;
                        break;
                    case 2:
                        completeKey++;
                        break;
                    default:
                        break;
                }
            }
            data.kw_wait = waitKey;
            data.kw_sch = searchKey;
            data.kw_complete = completeKey;
            //获取所有链接数
            var filter = Builders<IW2S_level1link>.Filter.Ne(x => x._id, ObjectId.Empty);
            var linkNum = MongoDBHelper.Instance.GetIW2S_level1links().Find(filter).Project(x => x._id).ToList();
            data.links = Convert.ToInt32(linkNum.Count);
            //保存链接
            MongoDBHelper.Instance.GetIW2S_BotDataInDay().InsertOne(data);
            log("计算完成！");
            //更新时间
            LastUpdateTime = DateTime.Now;
            clear6Hours();
        }

        /// <summary>
        /// 清除一天以前的6小时数据
        /// </summary>
        void clear6Hours()
        {
            DateTime end = DateTime.Now.AddDays(-1).ToUniversalTime();
            var builder = Builders<IW2S_BotData>.Filter;
            var filter = builder.Lt(x => x.ins_time, end);
            MongoDBHelper.Instance.GetIW2S_BotDataIn6Hours().DeleteMany(filter);
            log("清除6小时数据成功！");
        }

        /// <summary>
        /// 输出时间日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }
    }
}
