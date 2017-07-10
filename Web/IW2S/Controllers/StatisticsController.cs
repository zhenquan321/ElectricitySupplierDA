using AISSystem;
using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IW2S.Controllers
{
    public class StatisticsController : ApiController
    {
        /// <summary>
        /// 统计项目数量变化情况
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public StatisticsDto GetProjectCountSta(string userId)
        {
            List<DateTime> allTime = new List<DateTime>();      //全部时间节点
            var userObjId = new ObjectId(userId);
            //获取我创建的项目
            var builderCreate = Builders<IW2S_Project>.Filter;
            var filterCreate = builderCreate.Eq(x => x.UsrId, userObjId);
            var queryCreate = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterCreate).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            //按时间节点拆分数据
            var timeList = AnalysizeTime(queryCreate);

            //统计我创建的简报数量变化
            List<TimeToCount> myCreate = ComputeTime(timeList, allTime);
            //获取我分享的项目
            var builderShare = Builders<IW2S_ProjectShare>.Filter;
            var filterMyShare = builderShare.Eq(x => x.UsrId, userObjId);
            var colShare=MongoDBHelper.Instance.GetIW2S_ProjectShares();
            var queryMyShare = colShare.Find(filterMyShare).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            timeList = AnalysizeTime(queryMyShare);
            List<TimeToCount> myShare = ComputeTime(timeList, allTime);

            //统计分享给我的项目
            var filterUser = Builders<IW2SUser>.Filter.Eq(x => x._id, userObjId);
            string email = MongoDBHelper.Instance.Get_IW2SUser().Find(filterUser).Project(x => x.UsrEmail).FirstOrDefault();
            var filterShowTo = builderShare.Eq(x => x.SharedEmail, email);
            var queryShareTo = colShare.Find(filterShowTo).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            timeList = AnalysizeTime(queryShareTo);
            List<TimeToCount> shareTo = ComputeTime(timeList, allTime);

            //综合统计三者生成图表
            var sta = new StatisticsDto
            {
                Times = new List<DateTime>(),
                MyCreate = new List<int>(),
                MyShare = new List<int>(),
                ShowToMe = new List<int>()
            };
            allTime = allTime.Distinct().OrderBy(x => x).ToList();   //去重
            for(int i=0;i<allTime.Count;i++)
            {
                sta.Times.Add(allTime[i]);
                ComputeSta(sta.MyCreate, myCreate, allTime[i], i);
                ComputeSta(sta.MyShare, myShare, allTime[i], i);
                ComputeSta(sta.ShowToMe, shareTo, allTime[i], i);
            }
            return sta;
        }

        /// <summary>
        /// 统计简报数量变化情况
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public StatisticsDto GetReportCountSta(string userId)
        {
            List<DateTime> allTime = new List<DateTime>();      //全部时间节点
            var userObjId = new ObjectId(userId);
            //获取我创建的简报
            var builderCreate = Builders<Dnl_Report>.Filter;
            var filterCreate = builderCreate.Eq(x => x.UsrId, userObjId);
            var queryCreate = MongoDBHelper.Instance.GetDnl_Report().Find(filterCreate).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            //按时间节点拆分数据
            var timeList = AnalysizeTime(queryCreate);

            //统计我创建的简报数量变化
            List<TimeToCount> myCreate = ComputeTime(timeList, allTime);
            //获取我分享的简报
            var builderShare = Builders<Dnl_ReportShare>.Filter;
            var filterMyShare = builderShare.Eq(x => x.UsrId, userObjId);
            var colShare = MongoDBHelper.Instance.GetDnl_ReportShare();
            var queryMyShare = colShare.Find(filterMyShare).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            timeList = AnalysizeTime(queryMyShare);
            List<TimeToCount> myShare = ComputeTime(timeList, allTime);

            //统计分享给我的简报
            var filterUser = Builders<IW2SUser>.Filter.Eq(x => x._id, userObjId);
            string email = MongoDBHelper.Instance.Get_IW2SUser().Find(filterUser).Project(x => x.UsrEmail).FirstOrDefault();
            var filterShowTo = builderShare.Eq(x => x.SharedEmail, email);
            var queryShareTo = colShare.Find(filterShowTo).Project(x => new ItemDateInfo
            {
                CreatedAt = x.CreatedAt,
                DelAt = x.DelAt,
                IsDel = x.IsDel
            }).ToList();
            timeList = AnalysizeTime(queryShareTo);
            List<TimeToCount> shareTo = ComputeTime(timeList, allTime);

            //综合统计三者生成图表
            var sta = new StatisticsDto
            {
                Times = new List<DateTime>(),
                MyCreate = new List<int>(),
                MyShare = new List<int>(),
                ShowToMe = new List<int>()
            };
            allTime = allTime.Distinct().OrderBy(x => x).ToList();   //去重
            for (int i = 0; i < allTime.Count; i++)
            {
                sta.Times.Add(allTime[i]);
                ComputeSta(sta.MyCreate, myCreate, allTime[i], i);
                ComputeSta(sta.MyShare, myShare, allTime[i], i);
                ComputeSta(sta.ShowToMe, shareTo, allTime[i], i);
            }
            return sta;
        }

        /// <summary>
        /// 将数据中的创建时间和结束时间拆分开
        /// </summary>
        /// <param name="queryData">源数据</param>
        /// <returns></returns>
        List<TimeToCount> AnalysizeTime(List<ItemDateInfo> queryData)
        {
            DateTime dt = new DateTime();
            var timeToCount = new List<TimeToCount>();
            foreach (var pro in queryData)
            {
                var createTime = new TimeToCount
                {
                    Time = pro.CreatedAt.Date,
                    Count = 1
                };
                //判断项目是否已被删除
                if (pro.IsDel)
                {
                    /* 判断是否存在删除时间
                     * 存在读取删除时间
                     * 不存在则跳过 */
                    if (pro.DelAt != null && pro.DelAt != dt)
                    {
                        string tempDate = DateTime.Parse(pro.DelAt.ToString()).ToString("yyyy-MM-dd");
                        var delTime = new TimeToCount
                        {
                            Time = DateTime.Parse(tempDate),
                            Count = -1
                        };
                        timeToCount.Add(delTime);
                    }
                    else
                    {
                        continue;
                    }
                }
                timeToCount.Add(createTime);
            }
            return timeToCount.OrderBy(x => x.Time).ToList();
        }

        /// <summary>
        /// 计算图表单条线数据
        /// </summary>
        /// <param name="dateList">源数据</param>
        /// <param name="allTime">图表总体时间节点</param>
        /// <returns></returns>
        List<TimeToCount> ComputeTime(List<TimeToCount> dateList, List<DateTime> allTime)
        {
            List<TimeToCount> result = new List<TimeToCount>();
            foreach (var data in dateList)
            {
                if (result.Count == 0)
                {
                    result.Add(data);
                    allTime.Add(data.Time);
                }
                else
                {
                    int index = result.FindIndex(x => x.Time == data.Time);
                    if (index != -1)
                    {
                        result[index].Count += data.Count;
                    }
                    else
                    {
                        TimeToCount temp = new TimeToCount();
                        temp.Time = data.Time;
                        temp.Count = result[result.Count - 1].Count + data.Count;
                        result.Add(temp);
                        allTime.Add(data.Time);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 计算综合图表中该线实际数据
        /// </summary>
        /// <param name="num">图表中单线数据</param>
        /// <param name="dateList">源单线数据</param>
        /// <param name="time">当前时间</param>
        /// <param name="i">坐标点位置</param>
        void ComputeSta(List<int> numList,List<TimeToCount> dateList, DateTime time,int i)
        {
            int index = dateList.FindIndex(x => x.Time == time);
            if (index != -1)
            {
                numList.Add(dateList[index].Count);
            }
            else
            {
                if (i == 0)
                {
                    numList.Add(0);
                }
                else
                {
                    int temp = numList[i - 1];
                    numList.Add(temp);
                }
            }
        }
    }
}