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
using IWSBot2.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using IWSBot2.Helper;

using System.IO;

namespace IWSBot.Utility
{
    /// <summary>
    /// 按天记录网站状态
    /// </summary>
    public class ComputeProjectChart
    {
        ComputeProjectChart()
        {

        }

        public static readonly ComputeProjectChart Instance = new ComputeProjectChart();


        public void Run()
        {
            while (true)
            {
                try
                {
                    Commons.Log("开始计算！");
                    //WeiXinChart();
                    BaiduChart();
                    Thread.Sleep(24 * 60 * 60 * 1000);      //计算结束后休眠24小时
                }
                catch (Exception ex)
                {
                    Commons.Log("错误原因：" + ex.Message);
                    Thread.Sleep(5000);
                }
            }

        }

        #region 百度图表计算
        //命中关键词域名分布图
        public void BaiduChart()
        {
            //获取所有项目
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x.IsDel, false);
            var queryPro = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).ToList();
            int i = 0;
            foreach (var pro in queryPro)
            {
                Commons.Log("当前计算百度项目[{0}/{1}] - {2}".FormatStr(i+1, queryPro.Count, pro.Name));
                i++;
                //if (i < 137)
                //{
                //    continue;
                //}
                if (i != 6)
                {
                    continue;
                }
                if (pro.Name == "银杏基金会投资社会创业家项目")
                {
                    continue;
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();       //Json序列化与反序列化
                serializer.MaxJsonLength = Int32.MaxValue;

                //获取图表数据
                var builderChart = Builders<PojectChartMongo>.Filter;
                var filterChart = builderChart.Eq(x => x.ProjectId, pro._id) & builderChart.Eq(x => x.Source, SourceType.Enginee);
                filterChart &= builderChart.Eq(x => x.Name, "默认");
                var colChart = MongoDBHelper.Instance.GetPojectChart();
                var queryChart = colChart.Find(filterChart).ToList();

                //获取项目内所有关键词组信息
                var cateInfos = BaiduGetAllFenZhu(pro.UsrId.ToString(), pro._id.ToString());

                //获取所有关键词分组Id
                var cateIds = cateInfos.Select(x => x.id).ToList();
                var tempCateIds = cateInfos.Select(x => x.id).ToList();
                tempCateIds.Remove(ObjectId.Empty.ToString());
                tempCateIds.Sort();
                string cateIdFacotr = string.Join(";", tempCateIds);       //实际保存的关键词分组参数

                /* 对比判断是否需要重新计算数据 */
                if (queryChart.Count > 0)
                {
                    //对比气泡图分组Id（气泡图使用所有分组的信息），判断分组是否有变更
                    var factorStr = queryChart.Find(x => x.Type == ChartType.Bubble).FactorJson;
                    JObject factorJson = JObject.Parse(factorStr);
                    string cpCateFactor = factorJson.Property("categoryIds").Value.ToString();          //上次计算时的所有分组Id
                    if (cpCateFactor == cateIdFacotr)
                    {
                        //如果分组无变动，则再检查关键词是否有变动
                        var buiderMap = Builders<MediaKeywordMappingMongo>.Filter;
                    }
                }
                

                //获取项目内前10关键词组及对应参数
                var cate10Ids = cateInfos.Take(10).Select(x => x.id).ToList();
                tempCateIds = cateInfos.Take(10).Select(x => x.id).ToList();
                tempCateIds.Remove(ObjectId.Empty.ToString());
                tempCateIds.Sort();
                string cate10IdFacotr = string.Join(";", tempCateIds);       //实际保存的前10关键词分组参数

                #region 折线图及饼图计算
                var lineData = BaiduGetTimeLinkCount(string.Join(";", cate10Ids), pro._id.ToString(), null, null, 0, 10, 25, 7);
                string lineDataStr = serializer.Serialize(lineData).ToString();       //数据Json字符串

                //生成参数Json
                JObject lineFactorJson = new JObject();
                lineFactorJson.Add(new JProperty("categoryIds", cate10IdFacotr));
                lineFactorJson.Add(new JProperty("startTime", null));
                lineFactorJson.Add(new JProperty("endTime", null));
                lineFactorJson.Add(new JProperty("percent", 0));
                lineFactorJson.Add(new JProperty("topNum", 10));
                lineFactorJson.Add(new JProperty("sumNum", 25));
                lineFactorJson.Add(new JProperty("timeInterval", 7));

                var lineChart = queryChart.Find(x => x.Type == ChartType.Line);
                if (lineChart != null) //判断保存方式
                {
                    var filterUp = builderChart.Eq(x => x._id, lineChart._id);
                    //更新数据
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", lineDataStr }, { "FactorJson", lineFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                    colChart.UpdateOne(filterUp, update);
                }
                else
                {
                    //创建数据
                    var chart = new PojectChartMongo
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        DataJson = lineDataStr,
                        FactorJson = lineFactorJson.ToString(),
                        Name = "默认",
                        ProjectId = pro._id,
                        Type = ChartType.Line,
                        Source = SourceType.Enginee
                    };
                    colChart.InsertOne(chart);
                }
                Commons.Log("百度折线图计算完毕！");
                #endregion

                #region 气泡图计算
                var bubbleData = BaiduGetDomainStatis(string.Join(";", cateIds), pro._id.ToString());
                string bubbleDataStr = serializer.Serialize(bubbleData).ToString();       //数据Json字符串


                //生成参数Json
                JObject bubbleFactorJson = new JObject();
                bubbleFactorJson.Add(new JProperty("categoryIds", cateIdFacotr));

                var bubbleChart = queryChart.Find(x => x.Type == ChartType.Bubble);
                if (bubbleChart != null) //判断保存方式
                {
                    var filterUp = builderChart.Eq(x => x._id, bubbleChart._id);
                    //更新数据
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", bubbleDataStr }, { "FactorJson", bubbleFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                    colChart.UpdateOne(filterUp, update);
                }
                else
                {
                    //创建数据
                    var chart = new PojectChartMongo
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        DataJson = bubbleDataStr,
                        FactorJson = bubbleFactorJson.ToString(),
                        Name = "默认",
                        ProjectId = pro._id,
                        Type = ChartType.Bubble,
                        Source = SourceType.Enginee
                    };
                    colChart.InsertOne(chart);
                }
                Commons.Log("百度气泡图计算完毕！");
                #endregion

                #region 词云图及词频图
                var jieba = new JiebaHelper();
                /* 词云图 */
                var wordCloudData = jieba.BaiduExtract(pro.UsrId.ToString(), pro._id.ToString(), string.Join(";", cate10Ids));
                string wordCloudDataStr = serializer.Serialize(wordCloudData).ToString();       //数据Json字符串

                //生成参数Json
                JObject wordCloudFactorJson = new JObject();
                wordCloudFactorJson.Add(new JProperty("categoryIds", cate10IdFacotr));

                var wordCloudChart = queryChart.Find(x => x.Type == ChartType.WordCloud);
                if (wordCloudChart != null) //判断保存方式
                {
                    var filterUp = builderChart.Eq(x => x._id, wordCloudChart._id);
                    //更新数据
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", wordCloudDataStr }, { "FactorJson", wordCloudFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                    colChart.UpdateOne(filterUp, update);
                }
                else
                {
                    //创建数据
                    var chart = new PojectChartMongo
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        DataJson = wordCloudDataStr,
                        FactorJson = wordCloudFactorJson.ToString(),
                        Name = "默认",
                        ProjectId = pro._id,
                        Type = ChartType.WordCloud,
                        Source = SourceType.Enginee
                    };
                    colChart.InsertOne(chart);
                }
                Commons.Log("百度词云图计算完毕！");

                /* 词频图 */
                var wordFreData = jieba.BaiduFrequency(pro.UsrId.ToString(), pro._id.ToString(), string.Join(";", cate10Ids));
                string wordFreDataStr = serializer.Serialize(wordFreData).ToString();       //数据Json字符串

                //生成参数Json
                JObject wordFreFactorJson = new JObject();
                wordFreFactorJson.Add(new JProperty("categoryIds", cate10IdFacotr));

                var wordFreChart = queryChart.Find(x => x.Type == ChartType.WordFrequence);
                if (wordFreChart != null) //判断保存方式
                {
                    var filterUp = builderChart.Eq(x => x._id, wordFreChart._id);
                    //更新数据
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", wordFreDataStr }, { "FactorJson", wordFreFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                    colChart.UpdateOne(filterUp, update);
                }
                else
                {
                    //创建数据
                    var chart = new PojectChartMongo
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        DataJson = wordFreDataStr,
                        FactorJson = wordFreFactorJson.ToString(),
                        Name = "默认",
                        ProjectId = pro._id,
                        Type = ChartType.WordFrequence,
                        Source = SourceType.Enginee
                    };
                    colChart.InsertOne(chart);
                }
                Commons.Log("百度词频图计算完毕！");
                #endregion

                #region 网页关系图计算
                //var linkReferData = WeiXinGetLinkReference(pro._id.ToString(), 0);
                //string linkReferDataStr = serializer.Serialize(linkReferData).ToString();       //数据Json字符串
                //FileStream fs = new FileStream(@"F:\测试.txt", FileMode.OpenOrCreate);
                //StreamWriter sw = new StreamWriter(fs);
                //sw.Write(linkReferDataStr);
                //sw.Flush();
                //sw.Close();
                //fs.Close();

                ////生成参数Json
                //JObject linkReferFactorJson = new JObject();
                //linkReferFactorJson.Add(new JProperty("timeInterval", 0));

                //var linkReferChart = queryChart.Find(x => x.Type == ChartType.LinkReference);
                //if (linkReferChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, linkReferChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", linkReferDataStr }, { "FactorJson", linkReferFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = linkReferDataStr,
                //        FactorJson = linkReferFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.LinkReference,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信网页关系图计算完毕！");
                #endregion


            }

            Commons.Log("本次计算完毕！\n");
        }

        //命中关键词域名分布图
        public List<DomainStatisDto> BaiduGetDomainStatis(string categoryIds, string projectId)
        {
            List<DomainStatisDto> result = new List<DomainStatisDto>();
            ObjectId proObjId = new ObjectId(projectId);

            /* 计算图表数据 */
            //获取项目内所有关键词Id
            var keyIds = new List<string>();
            var usrId = ObjectId.Empty;
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & groupBuilder.Eq(x => x.IsDel, false);

            if (!string.IsNullOrEmpty(categoryIds))
            {
                var cateObjIds = categoryIds.Split(';').Select(x => new ObjectId(x)).ToList();
                //判断是否有分组
                if (cateObjIds.Count == 1 && cateObjIds[0].Equals(ObjectId.Empty))
                {
                    //无分组时获取所有关键词
                    groupFilter &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
                }
                else
                {
                    //有分组时仅获取选定分组内关键词
                    cateObjIds.Remove(ObjectId.Empty);      //去除根结点
                    groupFilter &= groupBuilder.In(x => x.CategoryId, cateObjIds);

                }
                var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
                keyIds = groupCol.Find(groupFilter).Project(x => x.KeywordId).ToList().Select(x => x.ToString()).ToList();      //关键词Id组
                usrId = groupCol.Find(groupBuilder.Eq(x => x.ProjectId, proObjId)).Project(x => x.UserId).FirstOrDefault();      //用户Id
            }
            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表
            //获取项目内所有符合条件的链接
            var buiderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = buiderLink.In(x => x.SearchkeywordId, keyIds) & buiderLink.Nin(x => x._id, exLinkObjIds);
            var links = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Id = x._id.ToString(),
                KeywordId = x.SearchkeywordId,
                PulishTime = x.PublishTime,
                Domain = x.Domain,
                DomainColNum = x.DCNum,
            }).ToList();

            var linkByDomamin = links.GroupBy(x => x.Domain);
            foreach (var x in linkByDomamin)
            {
                var stastic = new DomainStatisDto
                {
                    Domain = x.Key,
                    Count = x.Count(),
                    DomainColNum = x.ElementAtOrDefault(0).DomainColNum,
                };
                List<string> keys = new List<string>();
                int publishNum = 0;
                foreach (var y in x)
                {
                    keys.Add(y.KeywordId);
                    DateTime dt = new DateTime();
                    DateTime.TryParse(y.PulishTime, out dt);
                    if (dt > DateTime.MinValue)
                    {
                        publishNum++;
                    }
                }
                stastic.KeywordTotal = keys.Distinct().Count();
                if (publishNum > 0)
                {
                    stastic.PublishRatio = publishNum / (float)x.Count() * 100 + "%";
                }
                else
                {
                    stastic.PublishRatio = "0%";
                }
                result.Add(stastic);
            }

            if (result == null || result.Count == 0)
            {
                return result;
            }
            List<string> domainNameList = result.Select(x => x.Domain).Distinct().ToList();

            var domainCatBuilder = Builders<IW2S_DomainCategoryData>.Filter;
            var domainCatFilter = domainCatBuilder.Eq(x => x.UsrId, usrId) & domainCatBuilder.In(x => x.DomainName, domainNameList);
            var domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(domainCatFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
            //判断是否有私有域名分组
            if (domainCategoryDatas.Count == 0)
            {
                usrId = ObjectId.Empty;
                domainCatFilter = domainCatBuilder.Eq(x => x.UsrId, usrId) & domainCatBuilder.In(x => x.DomainName, domainNameList);
                domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(domainCatFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
            }
            DomainCategoryInfo dicdomainCategoryData = new DomainCategoryInfo();
            dicdomainCategoryData.Domain = new List<string>();
            dicdomainCategoryData.DomainCategoryId = new List<string>();
            dicdomainCategoryData.DomainCategoryName = new List<string>();
            foreach (var domainCategoryData in domainCategoryDatas)
            {
                if (!dicdomainCategoryData.Domain.Contains(domainCategoryData.DomainName))
                {
                    dicdomainCategoryData.Domain.Add(domainCategoryData.DomainName);
                    dicdomainCategoryData.DomainCategoryId.Add(domainCategoryData.DomainCategoryId.ToString());
                    var filter2 = Builders<IW2S_DomainCategory>.Filter.Eq(x => x._id, domainCategoryData.DomainCategoryId);
                    string v = MongoDBHelper.Instance.GetIW2S_DomainCategorys().Find(filter2).Project(x => x.Name).FirstOrDefault();
                    dicdomainCategoryData.DomainCategoryName.Add(v);
                }
            }
            foreach (var r in result)
            {
                if (dicdomainCategoryData.Domain.Contains(r.Domain))
                {
                    int index = dicdomainCategoryData.Domain.IndexOf(r.Domain);
                    r.DomainCategoryId = dicdomainCategoryData.DomainCategoryId[index];
                    r.DomainCategoryName = dicdomainCategoryData.DomainCategoryName[index];

                }
                else
                {
                    r.DomainCategoryId = null;
                    r.DomainCategoryName = "未分组";
                }
            }

            return result;
        }

        /// <summary>
        /// 有效链接统计图
        /// </summary>
        /// <param name="categoryId">关键词分组ID,多个用;相连</param>
        /// <param name="prjId">项目ID,多个用;相连</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="percent">显示百分比以上的节点</param>
        /// <param name="topNum">标记前多少位</param>
        /// <param name="sumNum">饼图统计Top数/param>
        /// <param name="timeInterval">坐标点时间间隔</param>
        /// <returns></returns>
        private TimeLinkCountDto BaiduGetTimeLinkCount(string categoryIds, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval)
        {
            DateTime tpStart = new DateTime();
            DateTime tpEnd = new DateTime();
            DateTime.TryParse(startTime, out tpStart);
            DateTime.TryParse(endTime, out tpEnd);

            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateIds = categoryIds.Split(';').Select(x => new ObjectId(x)).ToList();
            }
            //判断是否为根分组
            bool cateIsRoot = false;
            if (cateIds.Count == 1 && cateIds[0].Equals(ObjectId.Empty))
            {
                cateIsRoot = true;
            }
            //多个分组时剔除根分组
            cateIds.Remove(ObjectId.Empty);


            int years = 3;      //图表时间范围，按天的是3年，按周和按月是5年
            switch (timeInterval)
            {
                case 1:
                    years = 3;
                    break;
                case 7:
                    years = 5;
                    break;
                case 30:
                    years = 5;
                    break;
                default:
                    break;
            }

            TimeLinkCountDto result = new TimeLinkCountDto();
            if (string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var builder = Builders<Dnl_Link_Baidu>.Filter;

            //获取关键词列表
            List<string> keywordList = new List<string>();      //关键词列表
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId)) & groupBuilder.Eq(x => x.IsDel, false);
            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            var keyToCate = new Dictionary<string, string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (cateIsRoot)
            {
                groupFilter &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
            }
            else
            {
                //从分组中获取所有关键词Id
                groupFilter &= groupBuilder.In(x => x.CategoryId, cateIds);
            }
            var TaskList = groupCol.Find(groupFilter).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.CategoryId.ToString()
            }).ToList();
            foreach (var x in TaskList)
            {
                if (!keywordList.Contains(x.KeywordId) && !keyToCate.ContainsKey(x.KeywordId))
                {
                    keywordList.Add(x.KeywordId);
                    keyToCate.Add(x.KeywordId, x.CategoryId);
                }
            }

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Enginee);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取发表时间
            var filterLink = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");
            filterLink &= builder.Nin(x => x._id, exLinkObjIds);
            var queryDatas = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                PublishTime = x.PublishTime,
                KeywordId = x.SearchkeywordId,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            //获取包含分组ID的发布时间信息
            List<LinkStatus> linkList = new List<LinkStatus>();
            foreach (var x in queryDatas)
            {

                DateTime tmpDt = new DateTime();
                DateTime.TryParse(x.PublishTime, out tmpDt);
                int i = keywordList.IndexOf(x.KeywordId);
                while (i != -1)
                {
                    LinkStatus v = new LinkStatus();
                    v.PublishTime = tmpDt;
                    if (!cateIsRoot)
                    {
                        v.CategoryId = keyToCate[x.KeywordId];
                    }
                    else
                    {
                        //无分组以空定义为所有分组
                        v.CategoryId = "000000000000000000000000";
                    }
                    v.Title = x.Title;
                    v.Description = x.Description;
                    linkList.Add(v);
                    i = keywordList.IndexOf(x.KeywordId, i + 1);
                }
            }

            //删除异常时间 如0001-01-01与2063-23-12等时间，并排序
            linkList = linkList.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.PublishTime).ToList();

            //建立时间坐标
            List<DateTime> xCoordinate = new List<DateTime>();
            //int i = 1;
            if (linkList.Count > 0)
            {
                DateTime now = linkList[0].PublishTime;
                DateTime end = now.AddYears(-years);
                while (now >= end)
                {
                    xCoordinate.Add(now);
                    now = now.AddDays(-timeInterval);
                }
            }
            xCoordinate.Reverse();
            result.Times = xCoordinate;

            //获取起止时间位置
            int xStart;
            int xEnd;
            if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                for (int i = 0; i < xCoordinate.Count; i++)
                {
                    if (xCoordinate[i] <= tpStart) { xStart = i; }
                    if (xCoordinate[i] <= tpEnd) { xEnd = i; }
                }
            }

            //将发布时间依分组拆分
            List<CategoryList> categoryList = new List<CategoryList>();
            if (!cateIsRoot)
            {
                foreach (var x in cateIds)
                {
                    CategoryList v = new CategoryList();
                    v.PublishTime = new List<DateTime>();
                    v.CategoryId = x.ToString();
                    categoryList.Add(v);
                }

                //获取分组名并分配到数据中去
                var namefilter = Builders<Dnl_KeywordCategory>.Filter.In(x => x._id, cateIds);
                var nameList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(namefilter).Project(x => new
                {
                    Name = x.Name,
                    CategoryId = x._id.ToString()
                }).ToList();
                foreach (var x in nameList)
                {
                    CategoryList cat = categoryList.Find(s => s.CategoryId.Equals(x.CategoryId));
                    cat.CategoryName = x.Name;
                }
            }
            else
            {
                var cat = new CategoryList
                {
                    CategoryId = "000000000000000000000000",
                    CategoryName = "所有词",
                    PublishTime = new List<DateTime>()
                };
                categoryList.Add(cat);
            }

            //获取各分组内数据的发布时间
            foreach (var x in linkList)
            {
                CategoryList cat = categoryList.Find(s => s.CategoryId.Equals(x.CategoryId));
                cat.PublishTime.Add(x.PublishTime);
            }

            List<LineData> lineData = new List<LineData>();

            //top数据
            List<TopData> topData = new List<TopData>();

            //遍历数组，获取不同分组的数据
            foreach (var categoryData in categoryList)
            {
                LineData link = new LineData();
                link.name = categoryData.CategoryName;

                List<int> linkCounts = new List<int>();
                if (categoryData.PublishTime.Count > 0)
                {
                    DateTime now = linkList[0].PublishTime;
                    DateTime end = new DateTime();
                    end = now.AddYears(-years);
                    while (now >= end)
                    {
                        linkCounts.Add(categoryData.PublishTime.Where(x => x <= now && x > now.AddDays(-timeInterval)).Count());
                        now = now.AddDays(-timeInterval);
                    }
                }
                else
                {
                    continue;
                }
                //将链接数倒序
                linkCounts.Reverse();

                link.LinkCount = linkCounts;
                lineData.Add(link);

                //将坐标添加到临时数据列表中
                List<DateTime> temp = new List<DateTime>();
                for (int i = 0; i < xCoordinate.Count; i++)
                {
                    TopData v = new TopData();
                    v.name = categoryData.CategoryName;
                    v.CategoryId = categoryData.CategoryId;
                    v.X = xCoordinate[i];
                    v.Y = linkCounts[i];
                    topData.Add(v);
                }

            }

            //获取top数据及自动摘要节点

            if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                topData = topData.Where(x => x.X > linkList[0].PublishTime.AddYears(-1)).ToList().OrderByDescending(x => x.Y).ToList<TopData>();

            }
            else
            {
                topData = topData.Where(x => x.X > tpStart).Where(x => x.X < tpEnd).OrderByDescending(x => x.Y).ToList<TopData>();
            }
            //获取限定数量的摘要时间节点
            List<TopData> tempSum = new List<TopData>();
            if (sumNum > 0)
            {
                tempSum = topData.Take(sumNum).ToList();
            }
            else
            {
                tempSum = topData.Take(1).ToList();
            }
            topData = topData.Take(topNum).ToList();

            List<SumData> sumData = new List<SumData>();        //摘要
            //获取摘要节点
            for (var i = 0; i < tempSum.Count; i++)
            {
                SumData sum = new SumData();
                sum.Y = tempSum[i].Y;
                sum.X = tempSum[i].X;
                sum.CategoryName = tempSum[i].name;
                sum.CategoryId = tempSum[i].CategoryId;
                sumData.Add(sum);
            }
            //依节点查询数据库，生成摘要
            var jieba = new JiebaHelper();
            for (var i = 0; i < sumData.Count; i++)
            {
                DateTime time = sumData[i].X;     //当前时间节点
                string source = "";
                foreach (var x in linkList)
                {
                    if (x.PublishTime <= time && x.PublishTime > time.AddDays(-timeInterval))
                        if (x.CategoryId.Equals(sumData[i].CategoryId))
                            source += x.Title + "。" + System.Environment.NewLine + x.Description + "。" + System.Environment.NewLine;
                }
                var tempStr = jieba.GetSummary(sumData[i].CategoryName, time.ToString(), prjId, source);
                if (tempStr.Count > 0 && tempStr[0].Length > 40)
                {
                    tempStr[0] = tempStr[0].Substring(0, 39);
                    tempStr[0] += "…";
                }
                if (tempStr.Count > 0)
                    sumData[i].Summary = tempStr[0];
            }
            result.Sum = sumData;

            //在percent大于0时，获取最大值，将不高于最大值percent百分比的值设为0,topData值删除
            List<TopData> delList = new List<TopData>();
            if (percent > 0)
            {
                int maxCount = topData[0].Y;
                int limit = maxCount * percent / 100;
                for (int i = 0; i < lineData.Count; i++)
                {
                    for (int j = 0; j < lineData[i].LinkCount.Count; j++)
                    {
                        if (lineData[i].LinkCount[j] < limit) lineData[i].LinkCount[j] = 0;
                    }
                }
                foreach (var x in topData)
                {
                    if (x.Y < limit) delList.Add(x);
                }
                foreach (var x in delList)
                {
                    topData.Remove(x);
                }
                //for (int i = 0; i < topData.Count; i++)
                //{
                //    if (topData[i].Y < limit) topData[i].Y = 0;
                //}
            }

            //将top数据分配到各组中去
            for (int i = 0; i < lineData.Count; i++)
            {
                lineData[i].topData = new List<TopData>();
                foreach (var x in topData)
                {
                    if (lineData[i].name.Equals(x.name))
                    {
                        lineData[i].topData.Add(x);
                    }
                }
            }

            result.LineDataList = lineData;
            return result;
        }

        /// <summary>
        /// 获取该项目下所有分组(词组文件夹列表)
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <returns>关键词数组</returns>
        private List<GroupTree2Dto> BaiduGetAllFenZhu(string usr_id, string projectId)
        {

            List<GroupTree2Dto> list = new List<GroupTree2Dto>();

            GroupTree2Dto result = new GroupTree2Dto();
            result.name = "所有词";
            //根目录ID默认为"000000000000000000000000"
            result.id = "000000000000000000000000";
            result.pId = "0";
            list.Add(result);


            var listGT = BaiduGetCategoryTree2(usr_id, projectId, new ObjectId("000000000000000000000000"), list);
            if (listGT == null)
                listGT = list;

            return listGT;

        }

        /// <summary>
        /// 获取关键词数组
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="parentId">父ID</param>
        /// <param name="list">关键词数组</param>
        /// <returns>关键词数组</returns>
        private List<GroupTree2Dto> BaiduGetCategoryTree2(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name
            }).ToList();

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0)
                return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (var treedata in TaskList)
            {
                BaiduGetCategoryTree2(usr_id, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
                GroupTree2Dto gt = new GroupTree2Dto();
                gt.id = treedata.id;
                gt.pId = treedata.pId;
                gt.name = treedata.name;
                list.Add(gt);
            }

            return list;
        }
        #endregion

        #region 微信图表计算
        /// <summary>
        /// 微信图表计算
        /// </summary>
        public void WeiXinChart()
        {
            //获取所有项目
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x.IsDel, false);
            var queryPro = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).ToList();
            int i = 1;
            foreach (var pro in queryPro)
            {
                Commons.Log("当前计算微信项目[{0}/{1}] - {2}".FormatStr(i, queryPro.Count, pro.Name));
                i++;
                //if (i < 159)
                //{
                //    i++;
                //    continue;
                //}
                if (pro.Name != "主内微信评测")
                {
                    continue;
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();       //Json序列化与反序列化
                serializer.MaxJsonLength = Int32.MaxValue;

                //获取图表数据
                var builderChart = Builders<PojectChartMongo>.Filter;
                var filterChart = builderChart.Eq(x => x.ProjectId, pro._id) & builderChart.Eq(x => x.Source, SourceType.Media);
                filterChart &= builderChart.Eq(x => x.Name, "默认");
                var colChart = MongoDBHelper.Instance.GetPojectChart();
                var queryChart = colChart.Find(filterChart).ToList();

                //获取项目内所有关键词组信息
                var cateInfos = WeiXinGetAllFenZhu(pro.UsrId.ToString(), pro._id.ToString());

                //获取所有关键词分组Id
                var cateIds = cateInfos.Select(x => x.id).ToList();
                cateIds.Remove(ObjectId.Empty.ToString());
                cateIds.Sort();

                ////获取项目内前8关键词组及对应参数
                //var cate8Ids = cateInfos.Take(8).Select(x => x.id).ToList();

                //var tempCateIds = cateInfos.Take(8).Select(x => x.id).ToList();
                //tempCateIds.Remove(ObjectId.Empty.ToString());
                //tempCateIds.Sort();
                //string cate8IdFacotr = string.Join(";", tempCateIds);       //实际保存的前8关键词分组参数
                #region 折线图及饼图计算

                //var lineData = WeiXinGetTimeLinkCount(string.Join(";", cateIds), pro._id.ToString(), null, null, 0, 10, 25, 1);
                //string lineDataStr = serializer.Serialize(lineData).ToString();       //数据Json字符串

                ////生成参数Json
                //JObject lineFactorJson = new JObject();
                //lineFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));
                //lineFactorJson.Add(new JProperty("startTime", null));
                //lineFactorJson.Add(new JProperty("endTime", null));
                //lineFactorJson.Add(new JProperty("percent", 0));
                //lineFactorJson.Add(new JProperty("topNum", 10));
                //lineFactorJson.Add(new JProperty("sumNum", 25));
                //lineFactorJson.Add(new JProperty("timeInterval", 1));

                //var lineChart = queryChart.Find(x => x.Type == ChartType.Line);
                //if (lineChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, lineChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", lineDataStr }, { "FactorJson", lineFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = lineDataStr,
                //        FactorJson = lineFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.Line,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信折线图计算完毕！");
                #endregion

                #region 气泡图计算
                //var bubbleData = WeiXinGetDomainStatis(string.Join(";", cateIds), pro._id.ToString());
                //string bubbleDataStr = serializer.Serialize(bubbleData).ToString();       //数据Json字符串

                
                ////生成参数Json
                //JObject bubbleFactorJson = new JObject();
                //bubbleFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));

                //var bubbleChart = queryChart.Find(x => x.Type == ChartType.Bubble);
                //if (bubbleChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, bubbleChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", bubbleDataStr }, { "FactorJson", bubbleFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = bubbleDataStr,
                //        FactorJson = bubbleFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.Bubble,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信气泡图计算完毕！");
                #endregion

                #region 词云图及词频图
                //var jieba = new JiebaHelper();
                ///* 词云图 */
                //var wordCloudData = jieba.MediaExtract(pro.UsrId.ToString(), pro._id.ToString(), string.Join(";", cateIds));
                //string wordCloudDataStr = serializer.Serialize(wordCloudData).ToString();       //数据Json字符串

                ////生成参数Json
                //JObject wordCloudFactorJson = new JObject();
                //wordCloudFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));

                //var wordCloudChart = queryChart.Find(x => x.Type == ChartType.WordCloud);
                //if (wordCloudChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, wordCloudChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", wordCloudDataStr }, { "FactorJson", wordCloudFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = wordCloudDataStr,
                //        FactorJson = wordCloudFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.WordCloud,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信词云图计算完毕！");

                ///* 词频图 */
                //var wordFreData = jieba.MediaFrequency(pro.UsrId.ToString(), pro._id.ToString(), string.Join(";", cateIds));
                //string wordFreDataStr = serializer.Serialize(wordFreData).ToString();       //数据Json字符串

                ////生成参数Json
                //JObject wordFreFactorJson = new JObject();
                //wordFreFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));

                //var wordFreChart = queryChart.Find(x => x.Type == ChartType.WordFrequence);
                //if (wordFreChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, wordFreChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", wordFreDataStr }, { "FactorJson", wordFreFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = wordFreDataStr,
                //        FactorJson = wordFreFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.WordFrequence,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信词频图计算完毕！");
                #endregion

                #region 网页关系图计算
                var linkReferData = WeiXinGetLinkReference(pro._id.ToString(), 0);
                string linkReferDataStr = serializer.Serialize(linkReferData).ToString();       //数据Json字符串
                FileStream fs = new FileStream(@"F:\测试.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(linkReferDataStr);
                sw.Flush();
                sw.Close();
                fs.Close();

                //生成参数Json
                JObject linkReferFactorJson = new JObject();
                linkReferFactorJson.Add(new JProperty("timeInterval", 0));

                var linkReferChart = queryChart.Find(x => x.Type == ChartType.LinkReference);
                if (linkReferChart != null) //判断保存方式
                {
                    var filterUp = builderChart.Eq(x => x._id, linkReferChart._id);
                    //更新数据
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", linkReferDataStr }, { "FactorJson", linkReferFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                    colChart.UpdateOne(filterUp, update);
                }
                else
                {
                    //创建数据
                    var chart = new PojectChartMongo
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        DataJson = linkReferDataStr,
                        FactorJson = linkReferFactorJson.ToString(),
                        Name = "默认",
                        ProjectId = pro._id,
                        Type = ChartType.LinkReference,
                        Source = SourceType.Media
                    };
                    colChart.InsertOne(chart);
                }
                Commons.Log("微信网页关系图计算完毕！");
                #endregion

                #region 公众号热度排行表计算
                //var NameStatisData = WeiXinGetNameStatistic(string.Join(";", cateIds), pro._id.ToString());
                //string NameStatisDataStr = serializer.Serialize(NameStatisData).ToString();       //数据Json字符串

                //int count = NameStatisDataStr.Length;

                ////生成参数Json
                //JObject NameStatisFactorJson = new JObject();
                //NameStatisFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));

                //var NameStatisChart = queryChart.Find(x => x.Type == ChartType.NameStatis);
                //if (NameStatisChart != null) //判断保存方式
                //{
                //    var filterUp = builderChart.Eq(x => x._id, NameStatisChart._id);
                //    //更新数据
                //    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataJson", NameStatisDataStr }, { "FactorJson", NameStatisFactorJson.ToString() }, { "CreatedAt", DateTime.Now.AddHours(8) } } } };
                //    colChart.UpdateOne(filterUp, update);
                //}
                //else
                //{
                //    //创建数据
                //    var chart = new PojectChartMongo
                //    {
                //        CreatedAt = DateTime.Now.AddHours(8),
                //        DataJson = NameStatisDataStr,
                //        FactorJson = NameStatisFactorJson.ToString(),
                //        Name = "默认",
                //        ProjectId = pro._id,
                //        Type = ChartType.NameStatis,
                //        Source = SourceType.Media
                //    };
                //    colChart.InsertOne(chart);
                //}
                //Commons.Log("微信公众号热度排行表计算完毕！");
                #endregion
            }

            Commons.Log("本次计算完毕！\n");
        }

        /// <summary>
        /// 获取该项目下所有分组(词组文件夹列表)
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <returns>关键词数组</returns>
        private List<GroupTree2Dto> WeiXinGetAllFenZhu(string usr_id, string projectId)
        {

            List<GroupTree2Dto> list = new List<GroupTree2Dto>();

            GroupTree2Dto result = new GroupTree2Dto();
            result.name = "所有词";
            //根目录ID默认为"000000000000000000000000"
            result.id = "000000000000000000000000";
            result.pId = "0";
            list.Add(result);


            var listGT = WeiXinGetCategoryTree2(usr_id, projectId, new ObjectId("000000000000000000000000"), list);
            if (listGT == null)
                listGT = list;

            return listGT;

        }

        /// <summary>
        /// 获取关键词数组
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="parentId">父ID</param>
        /// <param name="list">关键词数组</param>
        /// <returns>关键词数组</returns>
        private List<GroupTree2Dto> WeiXinGetCategoryTree2(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<MediaKeywordCategoryMongo>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetMediaKeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name
            }).ToList();

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0)
                return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (var treedata in TaskList)
            {
                WeiXinGetCategoryTree2(usr_id, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
                GroupTree2Dto gt = new GroupTree2Dto();
                gt.id = treedata.id;
                gt.pId = treedata.pId;
                gt.name = treedata.name;
                list.Add(gt);
            }

            return list;
        }

        //命中关键词域名分布图
        private List<WXDomainStatisDto> WeiXinGetDomainStatis(string categoryIds, string projectId)
        {
            var result = new List<WXDomainStatisDto>();
            if (string.IsNullOrEmpty(categoryIds) || string.IsNullOrEmpty(projectId))
            {
                return result;
            }

            ObjectId proObjId = new ObjectId(projectId);
            JavaScriptSerializer serializer = new JavaScriptSerializer();       //Json序列化与反序列化

            var cateIds = new List<string>();
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateIds = Commons.GetIdListFromStr(categoryIds);
                cateIds.Remove(ObjectId.Empty.ToString());
                cateIds.Sort();
            }
            /* 计算图表数据 */
            //获取项目内所有关键词Id
            var keyIds = new List<string>();
            var usrId = ObjectId.Empty;
            var groupBuilder = Builders<MediaKeywordMappingMongo>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & groupBuilder.Eq(x => x.IsDel, false);

            if (!string.IsNullOrEmpty(categoryIds))
            {
                var cateObjIds = categoryIds.Split(';').Select(x => new ObjectId(x)).ToList();
                //判断是否有分组
                if (cateObjIds.Count == 1 && cateObjIds[0].Equals(ObjectId.Empty))
                {
                    //无分组时获取所有关键词
                    groupFilter &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
                }
                else
                {
                    //有分组时仅获取选定分组内关键词
                    cateObjIds.Remove(ObjectId.Empty);      //去除根结点
                    groupFilter &= groupBuilder.In(x => x.CategoryId, cateObjIds);

                }
                var groupCol = MongoDBHelper.Instance.GetMediaKeywordMapping();
                keyIds = groupCol.Find(groupFilter).Project(x => x.KeywordId).ToList().Select(x => x.ToString()).ToList();      //关键词Id组
                usrId = groupCol.Find(groupBuilder.Eq(x => x.ProjectId, proObjId)).Project(x => x.UserId).FirstOrDefault();      //用户Id
            }
            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Media);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表
            //获取项目内所有符合条件的链接
            var buiderLink = Builders<WXLinkMainMongo>.Filter;
            var filterLink = buiderLink.In(x => x.KeywordId, keyIds) & buiderLink.Nin(x => x._id, exLinkObjIds);
            var querylink = MongoDBHelper.Instance.GetWXLinkMain().Find(filterLink).Project(x => new
            {
                _id = x._id.ToString(),
                KeywordId = x.KeywordId,
                ReadNum = x.ReadNum,
                LikeNum = x.LikeNum,
                Name = x.Nickname,
                Url = x.Url,
                ContentLen=x.ContentLen
            }).ToList();

            ////如果关键词大于30个，去除只有一个关键词且文本长度小于50的链接
            //if (keyIds.Count > 30)
            //{
            //    for (int i = 0; i < querylink.Count; i++)
            //    {
            //        int pos = querylink.FindIndex(i + 1, x => x.Url == querylink[i].Url);
            //        if (pos == -1 && querylink[i].ContentLen < 50)
            //        {
            //            querylink.Remove(querylink[i]);
            //            i--;
            //        }
            //    }
            //}

            //按公众号分组
            var linkByName = querylink.GroupBy(x => x.Name);
            foreach (var links in linkByName)
            {
                var stastic = new WXDomainStatisDto
                {
                    Name = links.Key,
                    Count = links.Count(),
                };
                int hotNum = 0;
                var tempLinks = links.ToList();
                stastic.KeywordTotal = tempLinks.Select(x => x.KeywordId).Distinct().Count();     //获取公众号涉及到的所有关键词数
                tempLinks = tempLinks.DistinctBy(x => x.Url);
                foreach (var link in tempLinks)
                {
                    hotNum += link.LikeNum * 12 + link.ReadNum;
                }
                stastic.HotNum = hotNum;
                stastic.PublishRatio = 100;
                result.Add(stastic);
            }

            if (result == null || result.Count == 0)
            {
                return result;
            }
            List<string> domainNameList = result.Select(x => x.Name).Distinct().ToList();

            var domainCatBuilder = Builders<IW2S_DomainCategoryData>.Filter;
            var domainCatFilter = domainCatBuilder.Eq(x => x.UsrId, usrId) & domainCatBuilder.In(x => x.DomainName, domainNameList);
            var domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(domainCatFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
            //判断是否有私有公众号分组
            if (domainCategoryDatas.Count == 0)
            {
                usrId = ObjectId.Empty;
                domainCatFilter = domainCatBuilder.Eq(x => x.UsrId, usrId) & domainCatBuilder.In(x => x.DomainName, domainNameList);
                domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(domainCatFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
            }
            DomainCategoryInfo dicdomainCategoryData = new DomainCategoryInfo();
            dicdomainCategoryData.Domain = new List<string>();
            dicdomainCategoryData.DomainCategoryId = new List<string>();
            dicdomainCategoryData.DomainCategoryName = new List<string>();
            foreach (var domainCategoryData in domainCategoryDatas)
            {
                if (!dicdomainCategoryData.Domain.Contains(domainCategoryData.DomainName))
                {
                    dicdomainCategoryData.Domain.Add(domainCategoryData.DomainName);
                    dicdomainCategoryData.DomainCategoryId.Add(domainCategoryData.DomainCategoryId.ToString());
                    var filter2 = Builders<IW2S_DomainCategory>.Filter.Eq(x => x._id, domainCategoryData.DomainCategoryId);
                    string v = MongoDBHelper.Instance.GetIW2S_DomainCategorys().Find(filter2).Project(x => x.Name).FirstOrDefault();
                    dicdomainCategoryData.DomainCategoryName.Add(v);
                }
            }
            foreach (var r in result)
            {
                if (dicdomainCategoryData.Domain.Contains(r.Name))
                {
                    int index = dicdomainCategoryData.Domain.IndexOf(r.Name);
                    r.DomainCategoryId = dicdomainCategoryData.DomainCategoryId[index];
                    r.DomainCategoryName = dicdomainCategoryData.DomainCategoryName[index];

                }
                else
                {
                    r.DomainCategoryId = null;
                    r.DomainCategoryName = "未分组";
                }
            }
            return result;
        }

        /// <summary>
        /// 有效链接统计图
        /// </summary>
        /// <param name="categoryIds">关键词分组ID,多个用;相连</param>
        /// <param name="prjId">项目ID,多个用;相连</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="percent">显示百分比以上的节点</param>
        /// <param name="topNum">标记前多少位</param>
        /// <param name="sumNum">饼图统计Top数/param>
        /// <param name="timeInterval">坐标点时间间隔</param>
        /// <returns></returns>
        private TimeLinkCountDto WeiXinGetTimeLinkCount(string categoryIds, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval)
        {
            DateTime tpStart = new DateTime();
            DateTime tpEnd = new DateTime();
            DateTime.TryParse(startTime, out tpStart);
            DateTime.TryParse(endTime, out tpEnd);

            TimeLinkCountDto result = new TimeLinkCountDto();

            List<ObjectId> cateObjIds = new List<ObjectId>();
            var cateIds = new List<string>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateObjIds = categoryIds.Split(';').Select(x => new ObjectId(x)).ToList();
                cateIds = cateObjIds.Select(x => x.ToString()).ToList();
                cateIds.Remove(ObjectId.Empty.ToString());
                cateIds.Sort();
            }

            if (string.IsNullOrEmpty(prjId))
            {
                return null;
            }
            var proObjId = new ObjectId(prjId);

            //判断是否为根分组
            bool cateIsRoot = false;
            if (cateObjIds.Count == 1 && cateObjIds[0].Equals(ObjectId.Empty))
            {
                cateIsRoot = true;
            }
            //多个分组时剔除根分组
            cateObjIds.Remove(ObjectId.Empty);

            int years = 3;      //图表时间范围，按天的数据截止时间，按周和按月是5年
            switch (timeInterval)
            {
                case 1:
                    years = 0;
                    break;
                case 7:
                    years = 5;
                    break;
                case 30:
                    years = 5;
                    break;
                default:
                    break;
            }

            var groupBuilder = Builders<MediaKeywordMappingMongo>.Filter;
            var builder = Builders<WXLinkMainMongo>.Filter;

            //获取关键词列表
            var keyIds = new List<string>();      //关键词列表
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, proObjId) & groupBuilder.Eq(x => x.IsDel, false);
            var groupCol = MongoDBHelper.Instance.GetMediaKeywordMapping();
            var keyToCate = new Dictionary<string, string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (cateIsRoot)
            {
                groupFilter &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
            }
            else
            {
                //从分组中获取所有关键词Id
                groupFilter &= groupBuilder.In(x => x.CategoryId, cateObjIds);
            }
            var TaskList = groupCol.Find(groupFilter).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.CategoryId.ToString()
            }).ToList();
            foreach (var x in TaskList)
            {
                if (!keyIds.Contains(x.KeywordId) && !keyToCate.ContainsKey(x.KeywordId))
                {
                    keyIds.Add(x.KeywordId);
                    keyToCate.Add(x.KeywordId, x.CategoryId);
                }
            }

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Media);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取发表时间
            var filterLink = builder.In(x => x.KeywordId, keyIds) & builder.Ne(x => x.PostTime, DateTime.MinValue);
            filterLink &= builder.Nin(x => x._id, exLinkObjIds);
            var queryDatas = MongoDBHelper.Instance.GetWXLinkMain().Find(filterLink).Project(x => new WeiXinLinkDto
            {
                PublishTime = x.PostTime,
                KeywordId = x.KeywordId,
                Title = x.Title,
                //Content = x.Content,
                Description = x.Description
            }).ToList();

            //获取包含分组ID的发布时间信息
            List<LinkStatus> linkList = new List<LinkStatus>();
            foreach (var x in queryDatas)
            {

                //DateTime tmpDt = new DateTime();
                //DateTime.TryParse(x.PublishTime, out tmpDt);
                int i = keyIds.IndexOf(x.KeywordId);
                while (i != -1)
                {
                    LinkStatus v = new LinkStatus();
                    //v.PublishTime = tmpDt;
                    if (!cateIsRoot)
                    {
                        v.CategoryId = keyToCate[x.KeywordId];
                    }
                    else
                    {
                        //无分组以空定义为所有分组
                        v.CategoryId = "000000000000000000000000";
                    }
                    v.Title = x.Title;
                    v.Description = x.Description;
                    v.PublishTime = x.PublishTime;
                    linkList.Add(v);
                    i = keyIds.IndexOf(x.KeywordId, i + 1);
                }
            }

            //删除异常时间 如0001-01-01与2063-23-12等时间，并排序
            linkList = linkList.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.PublishTime).ToList();

            //建立时间坐标
            List<DateTime> xCoordinate = new List<DateTime>();
            //int i = 1;
            if (linkList.Count > 0)
            {
                DateTime now = linkList[0].PublishTime;
                DateTime end = new DateTime();
                if (years == 0)
                {
                    end = linkList.Last().PublishTime;
                }
                else
                {
                    end = now.AddYears(-years);
                }
                while (now >= end)
                {
                    xCoordinate.Add(now);
                    now = now.AddDays(-timeInterval);
                }
            }
            xCoordinate.Reverse();
            result.Times = xCoordinate;

            //获取起止时间位置
            int xStart;
            int xEnd;
            if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                for (int i = 0; i < xCoordinate.Count; i++)
                {
                    if (xCoordinate[i] <= tpStart) { xStart = i; }
                    if (xCoordinate[i] <= tpEnd) { xEnd = i; }
                }
            }

            //将发布时间依分组拆分
            List<CategoryList> categoryList = new List<CategoryList>();
            if (!cateIsRoot)
            {
                foreach (var x in cateObjIds)
                {
                    CategoryList v = new CategoryList();
                    v.PublishTime = new List<DateTime>();
                    v.CategoryId = x.ToString();
                    categoryList.Add(v);
                }

                //获取分组名并分配到数据中去
                var namefilter = Builders<MediaKeywordCategoryMongo>.Filter.In(x => x._id, cateObjIds);
                var nameList = MongoDBHelper.Instance.GetMediaKeywordCategory().Find(namefilter).Project(x => new
                {
                    Name = x.Name,
                    CategoryId = x._id.ToString()
                }).ToList();
                foreach (var x in nameList)
                {
                    CategoryList cat = categoryList.Find(s => s.CategoryId.Equals(x.CategoryId));
                    cat.CategoryName = x.Name;
                }
            }
            else
            {
                var cat = new CategoryList
                {
                    CategoryId = "000000000000000000000000",
                    CategoryName = "所有词",
                    PublishTime = new List<DateTime>()
                };
                categoryList.Add(cat);
            }

            //获取各分组内数据的发布时间
            foreach (var x in linkList)
            {
                CategoryList cat = categoryList.Find(s => s.CategoryId.Equals(x.CategoryId));
                cat.PublishTime.Add(x.PublishTime);
            }

            List<LineData> lineData = new List<LineData>();

            //top数据
            List<TopData> topData = new List<TopData>();

            //遍历数组，获取不同分组的数据
            foreach (var categoryData in categoryList)
            {
                LineData link = new LineData();
                link.name = categoryData.CategoryName;

                List<int> linkCounts = new List<int>();
                if (categoryData.PublishTime.Count > 0)
                {
                    DateTime now = linkList[0].PublishTime;
                    DateTime end = new DateTime();
                    if (years == 0)
                    {
                        end = linkList.Last().PublishTime;
                    }
                    else
                    {
                        end = now.AddYears(-years);
                    }
                    while (now >= end)
                    {
                        linkCounts.Add(categoryData.PublishTime.Where(x => x <= now && x > now.AddDays(-timeInterval)).Count());
                        now = now.AddDays(-timeInterval);
                    }
                }
                else
                {
                    continue;
                }
                //将链接数倒序
                linkCounts.Reverse();

                link.LinkCount = linkCounts;
                lineData.Add(link);

                //将坐标添加到临时数据列表中
                List<DateTime> temp = new List<DateTime>();
                for (int i = 0; i < xCoordinate.Count; i++)
                {
                    TopData v = new TopData();
                    v.name = categoryData.CategoryName;
                    v.CategoryId = categoryData.CategoryId;
                    v.X = xCoordinate[i];
                    v.Y = linkCounts[i];
                    topData.Add(v);
                }

            }

            //获取top数据及自动摘要节点

            if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                topData = topData.Where(x => x.X > linkList[0].PublishTime.AddYears(-1)).ToList().OrderByDescending(x => x.Y).ToList<TopData>();

            }
            else
            {
                topData = topData.Where(x => x.X > tpStart).Where(x => x.X < tpEnd).OrderByDescending(x => x.Y).ToList<TopData>();
            }
            //获取限定数量的摘要时间节点
            List<TopData> tempSum = new List<TopData>();
            if (sumNum > 0)
            {
                tempSum = topData.Take(sumNum).ToList();
            }
            else
            {
                tempSum = topData.Take(1).ToList();
            }
            topData = topData.Take(topNum).ToList();

            List<SumData> sumData = new List<SumData>();        //摘要
            //获取摘要节点
            for (var i = 0; i < tempSum.Count; i++)
            {
                SumData sum = new SumData();
                sum.Y = tempSum[i].Y;
                sum.X = tempSum[i].X;
                sum.CategoryName = tempSum[i].name;
                sum.CategoryId = tempSum[i].CategoryId;
                sumData.Add(sum);
            }
            //依节点查询数据库，生成摘要
            var jieba = new JiebaHelper();
            for (var i = 0; i < sumData.Count; i++)
            {
                DateTime time = sumData[i].X;     //当前时间节点
                string source = "";
                foreach (var x in linkList)
                {
                    if (x.PublishTime <= time && x.PublishTime > time.AddDays(-timeInterval))
                        if (x.CategoryId.Equals(sumData[i].CategoryId))
                            source += x.Title + "。" + System.Environment.NewLine + x.Description + "。" + System.Environment.NewLine;
                }
                var tempStr = jieba.GetSummary(sumData[i].CategoryName, time.ToString(), prjId, source);
                if (tempStr.Count > 0 && tempStr[0].Length > 40)
                {
                    tempStr[0] = tempStr[0].Substring(0, 39);
                    tempStr[0] += "…";
                }
                if (tempStr.Count > 0)
                    sumData[i].Summary = tempStr[0];
            }
            result.Sum = sumData;

            //在percent大于0时，获取最大值，将不高于最大值percent百分比的值设为0,topData值删除
            List<TopData> delList = new List<TopData>();
            if (percent > 0)
            {
                int maxCount = topData[0].Y;
                int limit = maxCount * percent / 100;
                for (int i = 0; i < lineData.Count; i++)
                {
                    for (int j = 0; j < lineData[i].LinkCount.Count; j++)
                    {
                        if (lineData[i].LinkCount[j] < limit) lineData[i].LinkCount[j] = 0;
                    }
                }
                foreach (var x in topData)
                {
                    if (x.Y < limit) delList.Add(x);
                }
                foreach (var x in delList)
                {
                    topData.Remove(x);
                }
                //for (int i = 0; i < topData.Count; i++)
                //{
                //    if (topData[i].Y < limit) topData[i].Y = 0;
                //}
            }

            //将top数据分配到各组中去
            for (int i = 0; i < lineData.Count; i++)
            {
                lineData[i].topData = new List<TopData>();
                foreach (var x in topData)
                {
                    if (lineData[i].name.Equals(x.name))
                    {
                        lineData[i].topData.Add(x);
                    }
                }
            }

            result.LineDataList = lineData;
            return result;
        }


        #region 链接关系图
        /// <summary>
        /// 获取链接关系图谱
        /// </summary>
        /// <param name="prjId">项目Id</param>
        /// <param name="timeInterval">时间间隔，0为全部，1为月，2为季，3为年</param>
        /// <returns></returns>
        private TimeLinkRefer WeiXinGetLinkReference(string prjId, int timeInterval)
        {
            var result = new TimeLinkRefer();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (string.IsNullOrEmpty(prjId))
            {
                return null;
            }
            List<ObjectId> cateIds = new List<ObjectId>();
            //获取第1级关键词组
            var builderCate = Builders<MediaKeywordCategoryMongo>.Filter;
            var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(prjId));
            filterCate &= builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
            var queryCate = MongoDBHelper.Instance.GetMediaKeywordCategory().Find(filterCate).Project(x => new
            {
                Id = x._id,
                Name = x.Name
            }).ToList();
            var cateObjIds = queryCate.Select(x => x.Id).ToList();      //词组ObjectId列表
            //建立分组信息
            var cateInfo = new List<LinkRefer_Cate>();
            foreach (var x in queryCate)
            {
                var cate = new LinkRefer_Cate();
                //判断是否需要裁剪分组名
                if (x.Name.Length <= 15)
                {
                    cate.name = x.Name;
                    cate.baseName = x.Name;
                }
                else
                {
                    cate.name = x.Name.Substring(0, 14) + "…";
                    cate.baseName = x.Name.Substring(0, 14) + "…";
                }
                cate.id = x.Id.ToString();
                cateInfo.Add(cate);
            }

            //获取组内关键词
            var builderGroup = Builders<MediaKeywordMappingMongo>.Filter;
            var filterGroup = builderGroup.In(x => x.CategoryId, cateObjIds) & builderGroup.Eq(x => x.IsDel, false);
            var queryKey = MongoDBHelper.Instance.GetMediaKeywordMapping().Find(filterGroup).Project(x => new
            {
                KeywordId = x.KeywordId,
                CategoryId = x.CategoryId,
                Keyword = x.Keyword,
            }).ToList();
            var keyIds = queryKey.Select(x => x.KeywordId.ToString()).ToList();    //关键词ObjectId列表
            keyIds = keyIds.Distinct().ToList();

            //建议关键词与词组字典
            var keyToCate = new Dictionary<string, string>();
            //var keyGroup = queryKey.GroupBy(x => x.KeywordId).ToList();
            //queryKey = queryKey.DistinctBy(x => x.KeywordId);
            foreach (var key in queryKey)
            {
                var cateId = queryCate.Find(x => x.Id.Equals(key.CategoryId)).Id.ToString();
                keyToCate.Add(key.KeywordId.ToString(), cateId);
            }

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Media);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取链接总数
            var cpLinks = new List<LinkRefer_Info>();
            var builderLink = Builders<WXLinkMainMongo>.Filter;
            var filterLink = builderLink.In(x => x.KeywordId, keyIds);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIds);
            var colLink = MongoDBHelper.Instance.GetWXLinkMain();
            var allLinkNum = colLink.Find(filterLink).Count();
            //判断是否需要缩放数量
            if (allLinkNum <= 6000)
            {
                cpLinks = MongoDBHelper.Instance.GetWXLinkMain().Find(filterLink).Project(x => new LinkRefer_Info
                {
                    Title = x.Title,
                    Description = x.Description,
                    Keyword = x.Keyword,
                    KeywordId = x.KeywordId,
                    PublishTime = x.PostTime,
                    LinkUrl = x.Url
                }).ToList();
            }
            else
            {
                //遍历所有关键词，按比例获取数据
                foreach (var id in keyIds)
                {
                    filterLink = builderLink.Eq(x => x.KeywordId, id);
                    var linkNum = colLink.Find(filterLink).Count();             //当前关键词对应链接数
                    if (linkNum == 0)
                        continue;
                    int useNum = (int)(linkNum * 6000 / allLinkNum);      //实际使用链接数
                    if (useNum == 0)
                        useNum = 1;
                    var tempLinks = colLink.Find(filterLink).SortByDescending(x => x.LikeNum).Limit(useNum).Project(x => new LinkRefer_Info
                    {
                        Title = x.Title,
                        Description = x.Description,
                        Keyword = x.Keyword,
                        KeywordId = x.KeywordId,
                        PublishTime = x.PostTime,
                        LinkUrl = x.Url
                    }).ToList();
                    cpLinks.AddRange(tempLinks);
                }
            }

            //建立节点信息
            var linkNodes = new List<LinkRefer_Node>();         //节点信息
            for (int i = 0; i < cpLinks.Count; i++)
            {
                //未存在发布信息时跳过该链接
                //DateTime tpdt = new DateTime();
                //DateTime.TryParse(queryLink[i].PublishTime, out tpdt);
                if (cpLinks[i].PublishTime < new DateTime(1753, 1, 09) || cpLinks[i].PublishTime > DateTime.Now)
                {
                    continue;
                }
                //获取链接信息
                var link = new LinkRefer_Node();
                link.publishTime = cpLinks[i].PublishTime;
                link.linkUrl = cpLinks[i].LinkUrl;
                if (cpLinks[i].Title != null && cpLinks[i].Title.Length > 20)
                    link.name = cpLinks[i].Title.Substring(0, 19) + "…";
                else
                    link.name = cpLinks[i].Title;
                if (cpLinks[i].Description != null && cpLinks[i].Description.Length > 50)
                    link.describe = cpLinks[i].Description.Substring(0, 49) + "…";
                else
                    link.describe = cpLinks[i].Description;
                link.value = 1;

                //获取链接所含关键词及数量
                var repeat = cpLinks.FindAll(s => s.Title == cpLinks[i].Title).DistinctBy(s => s.KeywordId);

                link.keyWordCount = repeat.Count;
                link.keyWordList = new List<string>();
                link.keyWordIdList = new List<string>();
                //移除重复的链接
                foreach (var y in repeat)
                {
                    link.keyWordList.Add(y.Keyword);
                    link.keyWordIdList.Add(y.KeywordId.ToString());
                    if (i == cpLinks.Count)
                        i--;
                    if (cpLinks[i].KeywordId != y.KeywordId)
                    {
                        cpLinks.Remove(y);
                    }
                }
                //获取归属组序号
                var cateId = keyToCate[cpLinks[i].KeywordId].ToString();
                var cateIndex = cateInfo.FindIndex(s => s.id == cateId);
                link.category = cateIndex;
                linkNodes.Add(link);
            }

            //建立时间坐标并将链接信息按时间分组
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            if (linkNodes.Count > 0)
            {
                startTime = linkNodes.Min(x => x.publishTime);
                endTime = linkNodes.Max(x => x.publishTime);
            }
            result.DateTimeList = new List<DateTime>();
            int year = startTime.Year;
            int month = startTime.Month;
            int season = (month - 1) / 3;
            var timeLinkNodeList = new List<List<LinkRefer_Node>>();        //按时间分组的链接节点信息
            switch (timeInterval)
            {
                case 0:     //一次返回所有时间段
                    result.DateTimeList.Add(startTime);
                    timeLinkNodeList.Add(linkNodes);
                    break;
                case 1:     //按月返回
                    {
                        DateTime timeCoordinate = new DateTime(year, month, 1);
                        while (timeCoordinate <= endTime)
                        {
                            DateTime temp = timeCoordinate;
                            result.DateTimeList.Add(temp);
                            timeCoordinate = timeCoordinate.AddMonths(1);
                        }
                        //将链接信息按月分组
                        foreach (var time in result.DateTimeList)
                        {
                            var tempNode = new List<LinkRefer_Node>();
                            tempNode = linkNodes.Where(x => x.publishTime >= time && x.publishTime < time.AddMonths(1)).ToList();
                            timeLinkNodeList.Add(tempNode);
                        }
                        break;
                    }
                case 2:     //按季返回
                    {
                        DateTime timeCoordinate = new DateTime(year, 1 + 3 * season, 1);
                        while (timeCoordinate < endTime)
                        {
                            DateTime temp = timeCoordinate;
                            result.DateTimeList.Add(temp);
                            timeCoordinate = timeCoordinate.AddMonths(3);
                        }
                        //将链接信息按季分组
                        foreach (var time in result.DateTimeList)
                        {
                            var tempNode = new List<LinkRefer_Node>();
                            tempNode = linkNodes.Where(x => x.publishTime >= time && x.publishTime < time.AddMonths(3)).ToList();
                            timeLinkNodeList.Add(tempNode);
                        }
                        break;
                    }
                case 3:     //按年返回
                    {
                        DateTime timeCoordinate = new DateTime(year, 1, 1);
                        while (timeCoordinate < endTime)
                        {
                            DateTime temp = timeCoordinate;
                            result.DateTimeList.Add(temp);
                            timeCoordinate = timeCoordinate.AddYears(1);
                        }
                        //将链接信息按月分组
                        foreach (var time in result.DateTimeList)
                        {
                            var tempNode = new List<LinkRefer_Node>();
                            tempNode = linkNodes.Where(x => x.publishTime >= time && x.publishTime < time.AddYears(1)).ToList();
                            timeLinkNodeList.Add(tempNode);
                        }
                        break;
                    }
                default:
                    return null;
            }
            result.ReferList = new List<LinkRefer>();
            for (int i = 0; i < result.DateTimeList.Count; i++)
            {
                var referData = ComputerLinkRefer(timeLinkNodeList[i], cateInfo);
                result.ReferList.Add(referData);
            }

            sw.Stop();
            return result;

        }

        /// <summary>
        /// 计算节点关系
        /// </summary>
        /// <param name="linkNodes">节点信息</param>
        /// <param name="cateInfo">分组信息</param>
        /// <param name="publishTime">当前发布时间</param>
        /// <returns></returns>
        LinkRefer ComputerLinkRefer(List<LinkRefer_Node> linkNodes, List<LinkRefer_Cate> cateInfo)
        {
            LinkRefer result = new LinkRefer();
            //建立节点关系
            var linkRefers = new List<LinkRefer_Refer>();        //节点间关系
            for (int i = 0; i < linkNodes.Count; i++)
            {
                for (int j = i + 1; j < linkNodes.Count; j++)
                {
                    //判断i和j两个节点是否有关联
                    bool isRefer = false;
                    foreach (var keyId in linkNodes[i].keyWordIdList)
                    {
                        if (isRefer)
                        {
                            break;
                        }
                        if (linkNodes[j].keyWordIdList.Contains(keyId))
                        {
                            //判断是否已经有核心节点同时和这两个节点相连
                            var refer1 = linkRefers.FindIndex(x => x.target == i);
                            var refer2 = linkRefers.FindIndex(x => x.target == j);
                            if (refer1 != -1 && refer2 != -1)
                            {
                                continue;
                            }
                            else
                            {
                                var refer = new LinkRefer_Refer
                                {
                                    source = i,
                                    target = j
                                };
                                linkRefers.Add(refer);
                                isRefer = true;
                            }
                        }
                    }
                }
            }

            //清理重复集群关系
            var tempGroup = linkRefers.GroupBy(x => x.source).ToList();              //统计获取集群中心点
            var tempCount = tempGroup.Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();               //统计获取集群中心点
            var usedIndex = new List<int>();    //已清理节点
            int num = 1;
            foreach (var x in tempCount)
            {
                if (usedIndex.Contains(x.Key))
                {
                    continue;
                }
                var centerNode = linkNodes[x.Key];      //集群中心点
                var referNodeList = tempGroup.Find(s => s.Key == x.Key);
                var referIndexs = referNodeList.Select(s => s.target).ToList();      //集群内所有相关点位置
                int j = 1;
                foreach (var y in referNodeList)            //遍历集群相关点
                {
                    var referNode = linkNodes[y.target];    //单个相关点
                    if (centerNode.category != referNode.category)
                    {
                        continue;
                    }
                    //清除相关点与集群内其他点的关系
                    var referInfos = linkRefers.FindAll(s => s.source == y.target);
                    var tempIndexs = referInfos.Select(s => s.target).ToList();     //相关点其所有关系节点位置
                    int k = 1;
                    foreach (var z in referInfos)
                    {
                        if (referIndexs.Contains(z.target))
                        {
                            var targetRefer = linkRefers.FindAll(s => s.source == z.target).Select(s => s.target).ToList();     //当前对应相关点的关系节点其自身所有关系节点位置
                            //判断关系节点是否在同一集群
                            if (targetRefer.All(s => referIndexs.Contains(s)))
                            {
                                linkRefers.Remove(z);
                            }
                        }
                        k++;
                    }
                    usedIndex.Add(y.target);
                    j++;
                }
                num++;
            }


            //生成Json对象
            JObject json = new JObject();
            JProperty jCtart = new JProperty("type", "force");
            json.Add(jCtart);

            JArray jArrayCate = new JArray();
            foreach (var x in cateInfo)
            {
                JObject cate = new JObject();
                JProperty name = new JProperty("name", x.name);
                cate.Add(name);
                JProperty keyword = new JProperty("keyword", x.keyword);
                cate.Add(keyword);
                JProperty baseName = new JProperty("base", x.baseName);
                cate.Add(baseName);
                jArrayCate.Add(cate);
            }
            JProperty jCate = new JProperty("categories", jArrayCate);
            json.Add(jCate);

            JArray jArrayLink = new JArray();
            foreach (var x in linkNodes)
            {
                JObject link = new JObject();
                JProperty name = new JProperty("name", x.name);
                link.Add(name);
                JProperty value = new JProperty("value", x.value);
                link.Add(value);
                JProperty keyWordCount = new JProperty("keyWordCount", x.keyWordCount);
                link.Add(keyWordCount);


                JArray jArrayKey = new JArray();
                //添加链接对应关键词
                foreach (var y in x.keyWordList)
                {
                    jArrayKey.Add(y);
                }
                JProperty keyWordList = new JProperty("keyWordList", jArrayKey);
                link.Add(keyWordList);

                JProperty category = new JProperty("category", x.category);
                link.Add(category);
                JProperty describe = new JProperty("base", x.describe);
                link.Add(describe);
                JProperty linkUrl = new JProperty("linkUrl", x.linkUrl);
                link.Add(linkUrl);
                jArrayLink.Add(link);
            }
            JProperty jLink = new JProperty("nodes", jArrayLink);
            json.Add(jLink);

            JArray jArrayRefer = new JArray();
            foreach (var x in linkRefers)
            {
                JObject refer = new JObject();
                JProperty source = new JProperty("source", x.source);
                refer.Add(source);
                JProperty target = new JProperty("target", x.target);
                refer.Add(target);
                jArrayRefer.Add(refer);
            }
            JProperty jRefer = new JProperty("links", jArrayRefer);
            json.Add(jRefer);
            result.Json = json.ToString();

            //获取Top10的集群中心点
            var topIndex = linkRefers.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
            result.TopData = new List<LinkRefer_Node>();
            foreach (var x in topIndex)
            {
                var data = new LinkRefer_Node(linkNodes[x.Key]);
                data.value = x.Count;
                result.TopData.Add(data);
            }
            if (result.TopData.Count < 10)
            {
                foreach (var x in topIndex)
                {
                    //获取集群内其他点
                    var temp = linkRefers.FindAll(s => s.source == x.Key);
                    foreach (var y in temp)
                    {
                        var data = new LinkRefer_Node(linkNodes[y.target]);
                        data.value = linkRefers.Count(s => s.source == y.target);
                        result.TopData.Add(data);
                        if (result.TopData.Count == 10)
                            break;
                    }
                    if (result.TopData.Count == 10)
                        break;
                }
                //获取孤立结点
                foreach (var x in linkNodes)
                {
                    if (result.TopData.Count == 10)
                        break;
                    if (!result.TopData.Contains(x))
                    {
                        var data = new LinkRefer_Node(x);
                        data.value = 1;
                        result.TopData.Add(data);
                    }
                }
            }


            //获取包含关键词最多的10个节点
            result.TopKeyData = new List<LinkRefer_Node>();
            var tpData = linkNodes.OrderByDescending(x => x.keyWordCount).Take(10).ToList();
            for (int i = 0; i < tpData.Count; i++)
            {
                var data = new LinkRefer_Node(tpData[i]);
                data.value = data.keyWordCount;
                result.TopKeyData.Add(data);
            }

            //获取组外链接数最多的10个节点
            var linkGroupCount = new List<LinkGroupCount>();
            for (int i = 0; i < linkNodes.Count; i++)
            {
                int count = 0;
                //获取该结点所有相关节点
                var referIndexs = linkRefers.FindAll(x => x.source == i).Select(x => x.target).ToList();
                referIndexs.AddRange(linkRefers.FindAll(x => x.target == i).Select(x => x.source).ToList());
                referIndexs = referIndexs.Distinct().ToList();
                foreach (var x in referIndexs)
                {
                    if (linkNodes[i].category != linkNodes[x].category)
                        count++;
                }
                if (count == 0)
                    continue;
                var linkGroup = new LinkGroupCount
                {
                    Pos = i,
                    Count = count
                };
                linkGroupCount.Add(linkGroup);
            }
            linkGroupCount = linkGroupCount.OrderByDescending(x => x.Count).Take(10).ToList();
            result.TopCateData = new List<LinkRefer_Node>();
            foreach (var x in linkGroupCount)
            {
                var data = new LinkRefer_Node(linkNodes[x.Pos]);
                data.value = x.Count;
                result.TopCateData.Add(data);
            }

            //获取词组影响力
            result.CateWeights = new List<CategoryWeight>();
            for (int i = 0; i < cateInfo.Count; i++)
            {
                var cateWeight = new CategoryWeight();
                cateWeight.Category = cateInfo[i].name;
                topIndex = linkRefers.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
                int referNum = 0;
                int keywordNum = 0;
                for (int j = 0; j < linkNodes.Count; j++)
                {
                    if (linkNodes[j].category == i)
                    {
                        referNum += linkRefers.Where(x => x.source == j || x.target == j).Count();
                        keywordNum += linkNodes[j].keyWordCount;
                    }
                }
                cateWeight.Weight = keywordNum * 99 + referNum * 12;
                result.CateWeights.Add(cateWeight);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 获取公众号热度排行
        /// </summary>
        /// <param name="categoryIds">分组Id，多个用分号隔开</param>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        private QueryResult<NameStatisticDto> WeiXinGetNameStatistic(string categoryIds, string projectId)
        {
            var result = new QueryResult<NameStatisticDto>();

            List<ObjectId> cateObjIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateObjIds = Commons.GetObjIdListFromStr(categoryIds);
            }
            //判断是否为根分组
            bool cateIsRoot = false;
            if (cateObjIds.Count == 1 && cateObjIds[0].Equals(ObjectId.Empty))
            {
                cateIsRoot = true;
            }

            if (string.IsNullOrEmpty(projectId))
            {
                return result;
            }
            ObjectId proObjId = new ObjectId(projectId);

            var groupBuilder = Builders<MediaKeywordMappingMongo>.Filter;
            var filterMapping = groupBuilder.Eq(x => x.IsDel, false);
            var builder = Builders<WXLinkMainMongo>.Filter;

            var keyIds = new List<string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (!cateIsRoot)
            {
                //去除根结点
                cateObjIds.Remove(ObjectId.Empty);
                //获取所给节点ID下关键词ID
                filterMapping &= groupBuilder.In(x => x.CategoryId, cateObjIds);
            }
            else
            {
                filterMapping &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & groupBuilder.Eq(x => x.ProjectId, proObjId);
            }
            keyIds = MongoDBHelper.Instance.GetMediaKeywordMapping().Find(filterMapping).Project(x => x.KeywordId.ToString()).ToList();

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Media);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.KeywordId, keyIds);
            allTimeFilter &= builder.Nin(x => x._id, exLinkObjIds);
            var colLink = MongoDBHelper.Instance.GetWXLinkMain();
            var queryLink = colLink.Find(allTimeFilter).Project(x => new
            {
                _id = x._id.ToString(),
                Url = x.Url,
                Name = x.Nickname,
                Keyword = x.Keyword,
                KeywordId = x.KeywordId,
                ReadNum = x.ReadNum,
                LikeNum = x.LikeNum,
                CommentNum = 0,
                NameId=x.NameId.ToString()
            }).ToList();

            var nameInfos = new List<NameStatisticDto>();
            var linkByName = queryLink.GroupBy(x => x.Name);        //按公众号分组
            foreach (var name in linkByName)
            {
                var nameInfo = new NameStatisticDto();
                nameInfo.Name = name.Key;
                var allLinks = name.ToList();                       //公众号所有文章
                nameInfo.NameId = allLinks[0].NameId;
                nameInfo.KeywordNum = allLinks.Select(x => x.Keyword).Distinct().Count();
                var trueLinks = allLinks.DistinctBy(x => x.Url);
                nameInfo.LinkNum = trueLinks.Count;
                nameInfo.LikeNum = trueLinks.Sum(x => x.LikeNum);
                nameInfo.ReadNum = trueLinks.Sum(x => x.ReadNum);
                nameInfo.InfluenceNum = nameInfo.ReadNum + nameInfo.LikeNum * 11 + nameInfo.CommentNum * 99;
                nameInfos.Add(nameInfo);
            }
            nameInfos = nameInfos.OrderByDescending(x => x.InfluenceNum).ToList();
            result.Count = nameInfos.Count;
            result.Result = nameInfos.ToList();
            return result;
        }
        #endregion
    }
}
