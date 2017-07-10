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
    public class ProCategoryController : ApiController
    {
        /// <summary>
        /// 创建项目组
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="name">项目组名</param>
        /// <param name="description">描述</param>
        /// <param name="projectIds">项目组内含项目Id，为复数时用“;”隔开</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertProjectCategory(string userId, string name, string description,string projectIds)
        {
            ResultDto result = new ResultDto();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(userId))
            {
                return result;
            }
            var projectIdList = GetObjIdListFromStr(projectIds);      //项目列表

            //新建项目组
            var builder = Builders<Dnl_ProjectCategory>.Filter;
            var col = MongoDBHelper.Instance.GetDnl_ProjectCategory();
            var filter = builder.Eq(x => x.Name, name);
            filter &= builder.Eq(x => x.UsrId, new ObjectId(userId));
            filter &= builder.Eq(x => x.IsDel, false);

            var dto = col.Find(filter).FirstOrDefault();
            if (dto != null)
            {
                result.Message = "项目组‘" + name + "’已存在！";
                return result;
            }

            Dnl_ProjectCategory prj = new Dnl_ProjectCategory
            {
                UsrId = new ObjectId(userId),
                Name = name,
                CreatedAt = DateTime.Now.AddHours(8),
                IsDel = false,
                ProjectCount = projectIdList.Count,
                Description = description
            };
            col.InsertOne(prj);

            if (projectIdList.Count > 0)
            {
                //获取新创建的项目组Id
                var builderId = Builders<Dnl_ProjectCategory>.Filter;
                var filterId = builderId.Eq(x => x.Name, name);
                filterId &= builderId.Eq(x => x.UsrId, new ObjectId(userId));
                filterId &= builderId.Eq(x => x.IsDel, false);
                var proCategoryId = col.Find(filterId).Project(x => x._id).FirstOrDefault();

                var builderProGroup = Builders<Dnl_ProjectGroup>.Filter;
                var colProGroup = MongoDBHelper.Instance.GetDnl_ProjectGroup();
                var builderPro = Builders<IW2S_Project>.Filter;
                var colPro = MongoDBHelper.Instance.GetIW2S_Projects();
                
                //创建项目信息
                foreach (var proId in projectIdList)
                {
                    var filterPro = builderPro.Eq(x => x._id, proId);
                    var query = colPro.Find(filterPro).Project(x=>new{
                        Name=x.Name,
                        Description=x.Description
                    }).FirstOrDefault();
                    var proGroup = new Dnl_ProjectGroup
                    {
                        Name = query.Name,
                        Description = query.Description,
                        UsrId = new ObjectId(userId),
                        ProjectCategoryId = proCategoryId,
                        ProjectId = proId,
                    };
                    colProGroup.InsertOne(proGroup);
                }
            }
            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 获取项目组列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_ProjectCategoryDto> GetProjectCategory(string userId, int page, int pagesize)
        {
            QueryResult<Dnl_ProjectCategoryDto> result = new QueryResult<Dnl_ProjectCategoryDto>();
            if (string.IsNullOrEmpty(userId))
            {
                return result;
            }
            var builder = Builders<Dnl_ProjectCategory>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(userId));
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_ProjectCategory().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<Dnl_ProjectCategoryDto> data = new List<Dnl_ProjectCategoryDto>();
            List<ObjectId> categoryIdList = new List<ObjectId>();      //项目组Id列表，用于获取所有组内项目
            foreach (var item in TaskList)
            {
                Dnl_ProjectCategoryDto v = new Dnl_ProjectCategoryDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.CreatedAt = item.CreatedAt.AddHours(-8);
                v.Description = item.Description;
                data.Add(v);
                categoryIdList.Add(item._id);
            }

            //获取所有组内项目
            var builderGroup = Builders<Dnl_ProjectGroup>.Filter;
            var filterGroup = builderGroup.In(x => x.ProjectCategoryId, categoryIdList);
            var allProList = MongoDBHelper.Instance.GetDnl_ProjectGroup().Find(filterGroup).Project(x => new Dnl_ProjectGroupDto
            {
                ProjectId = x.ProjectId.ToString(),
                Name = x.Name,
                Description = x.Description,
                ProjectCategoryId = x.ProjectCategoryId.ToString()
            }).ToList();
            
            //将项目分配到各组中去
            foreach(var pro in allProList){
                for (int i = 0; i < data.Count; i++)
                {
                    var proList = allProList.Where(x => x.ProjectCategoryId == data[i]._id).ToList();
                    data[i].ProjectList = proList;
                }
            }
            result.Result = data;
            result.Count = totalCount;
            return result;
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="filterIds">以;隔开</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelProjectCategory(string categoryIds)
        {
            ResultDto result = new ResultDto();
            var cateObjIds = GetObjIdListFromStr(categoryIds);

            var builder = Builders<Dnl_ProjectCategory>.Filter;
            var filter = builder.In(x => x._id, cateObjIds);
            DateTime now = DateTime.Now.AddHours(8);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", now } } } };
            MongoDBHelper.Instance.GetDnl_ProjectCategory().UpdateMany(filter, update);

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public ResultDto UpdateProjectCategory(string categoryId, string name, string description, string projectIds)
        {
            ResultDto result = new ResultDto();
            var newProList = GetObjIdListFromStr(projectIds);      //新的项目列表

            //获取Group内原有项目，并删除
            var builderGroup = Builders<Dnl_ProjectGroup>.Filter;
            var filterGroup = builderGroup.Eq(x => x.ProjectCategoryId, new ObjectId(categoryId));
            var colGroup = MongoDBHelper.Instance.GetDnl_ProjectGroup();
            colGroup.DeleteMany(filterGroup);

            //添加新项目
            if (newProList.Count > 0)
            {
                //获取新创建的项目组Id
                var builderId = Builders<Dnl_ProjectCategory>.Filter;
                var filterId = builderId.Eq(x => x.Name, name);
                filterId &= builderId.Eq(x => x.IsDel, false);

                var builderProGroup = Builders<Dnl_ProjectGroup>.Filter;
                var colProGroup = MongoDBHelper.Instance.GetDnl_ProjectGroup();
                var builderPro = Builders<IW2S_Project>.Filter;
                var colPro = MongoDBHelper.Instance.GetIW2S_Projects();

                //创建项目信息
                var proCreate = new List<Dnl_ProjectGroup>();
                foreach (var proId in newProList)
                {
                    var filterPro = builderPro.Eq(x => x._id, proId);
                    var query = colPro.Find(filterPro).Project(x => new
                    {
                        Name = x.Name,
                        Description = x.Description,
                        UsrId=x.UsrId
                    }).FirstOrDefault();
                    var proGroup = new Dnl_ProjectGroup
                    {
                        Name = query.Name,
                        Description = query.Description,
                        UsrId = query.UsrId,
                        ProjectCategoryId = new ObjectId(categoryId),
                        ProjectId = proId
                    };
                    proCreate.Add(proGroup);
                }
                colProGroup.InsertMany(proCreate);
            }

            //更新项目组字段
            var builder = Builders<Dnl_ProjectCategory>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(categoryId));
            if (string.IsNullOrEmpty(name))
            {
                result.Message = "项目组名不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "项目组描述不能为空";
                return result;
            }
            var updateCategory = new UpdateDocument { { "$set", new QueryDocument { { "Name", name }, { "Description", description } ,{"ProjectCount",newProList.Count}} } };
            MongoDBHelper.Instance.GetDnl_ProjectCategory().UpdateOne(filter, updateCategory);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取该项目组下所有项目及词组
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <returns>关键词数组</returns>
        [HttpGet]
        public List<GroupTree2Dto> GetAllFenZhu(string userId, string categoryId)
        {

            List<GroupTree2Dto> list = new List<GroupTree2Dto>();

            GroupTree2Dto root = new GroupTree2Dto();
            //获取该项目组名
            var filterCate = Builders<Dnl_ProjectCategory>.Filter.Eq(x => x._id, new ObjectId(categoryId));
            root.name = MongoDBHelper.Instance.GetDnl_ProjectCategory().Find(filterCate).Project(x => x.Name).FirstOrDefault();
            //根目录ID默认为"000000000000000000000000"
            root.id = "000000000000000000000000";
            root.pId = "0";
            list.Add(root);

            //获取次级项目组名
            var builder = Builders<Dnl_ProjectGroup>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(userId));
            filter &= builder.Eq(x => x.ProjectCategoryId, new ObjectId(categoryId));
            var TaskList = MongoDBHelper.Instance.GetDnl_ProjectGroup().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x.ProjectId.ToString(),
                pId = "000000000000000000000000",
                name = x.Name,
                isNode=true
            }).ToList();
            list.AddRange(TaskList);

            int proNum = list.Count;
            for (int i = 1; i < proNum; i++)
            {
                GetKeywordGroup(userId, list[i].id,new ObjectId("000000000000000000000000"), list);
            }
            return list;
        }

        /// <summary>
        /// 获取项目内词组列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="parentId">父ID</param>
        /// <param name="list">关键词数组</param>
        /// <returns>关键词数组</returns>
        private List<GroupTree2Dto> GetKeywordGroup(string userId, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取第一级词组名
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(userId)) & 
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name,
                isNode=false
            }).ToList();

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0)
                return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (var treedata in TaskList)
            {
                //GetKeywordGroup(userId, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
                GroupTree2Dto gt = new GroupTree2Dto();
                gt.id = treedata.id;
                gt.pId = projectId;
                gt.name = treedata.name;
                list.Add(gt);
            }

            return list;
        }

        /// <summary>
        /// 有效链接统计图
        /// </summary>
        /// <param name="categoryId">项目及关键词分组ID,多个用;相连</param>
        /// <param name="prjId">项目ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="percent">显示百分比以上的节点</param>
        /// <param name="topNum">标记前多少位</param>
        /// <param name="sumNum">摘要显示多少个，0表示仅显示1个</param>
        /// <param name="timeInterval">坐标点时间间隔</param>
        /// <returns></returns>
        [HttpGet]
        public TimeLinkCountDto GetTimeLinkCount(string categoryId, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval)
        {
            DateTime tpStart = new DateTime();
            DateTime tpEnd = new DateTime();
            DateTime.TryParse(startTime, out tpStart);
            DateTime.TryParse(endTime, out tpEnd);

            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIds = categoryId.Split(';').Where(x => !string.IsNullOrEmpty(x)).Select(x => new ObjectId(x)).ToList();
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
            if (string.IsNullOrEmpty(prjId) && string.IsNullOrEmpty(categoryId))
            {
                return result;
            }
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var builder = Builders<Dnl_Link_Baidu>.Filter;

            //List<ObjectId> proIds = new List<ObjectId>();
            //if (!string.IsNullOrEmpty(categoryId))
            //{
            //    proIds = prjId.Split(';').Where(x => !string.IsNullOrEmpty(x)).Select(x => new ObjectId(x)).ToList();
            //}

            //获取关键词列表
            List<string> keywordList = new List<string>();      //关键词列表
            var keyToCate = new Dictionary<string, string>();   //关键词与词组和项目Id比对词典

            //从获取分组中关键词
            var groupFilter = groupBuilder.In(x => x.CategoryId, cateIds) & groupBuilder.Eq(x => x.IsDel, false);
            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            var taskList = groupCol.Find(groupFilter).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.CategoryId.ToString()
            }).ToList();
            foreach (var x in taskList)
            {
                if (!keywordList.Contains(x.KeywordId))
                    keywordList.Add(x.KeywordId);
                if (!keyToCate.ContainsKey(x.KeywordId))
                {
                    keyToCate.Add(x.KeywordId, x.CategoryId);
                }
                else
                {
                    //将一个关键词Id对应的多个分组ID拼接在一起
                    keyToCate[x.KeywordId] +=  ";" + x.CategoryId;
                }
            }

            //获取项目中关键词，因为词组Id和项目Id混在一起，所以要不规则对项目ID查询一遍
            var filterPro = groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & groupBuilder.In(x => x.ProjectId, cateIds) & groupBuilder.Eq(x => x.IsDel, false);
            var taskList2 = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterPro).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.ProjectId.ToString(),
                ProjectId = x.ProjectId.ToString()
            }).ToList();
            foreach (var x in taskList2)
            {
                if (!keywordList.Contains(x.KeywordId))        //如果词典中无该关键词Id，新增
                    keywordList.Add(x.KeywordId);
                if (!keyToCate.ContainsKey(x.KeywordId))
                {
                    keyToCate.Add(x.KeywordId, x.ProjectId);
                }
                else   //如果词典中已有该关键词Id，则在对应数据中追加
                {
                    keyToCate[x.KeywordId] += ";" + x.ProjectId;
                }
            }

            //获取发表时间
            var filter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");
            var queryDatas = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filter).Project(x => new
            {
                PublishTime = x.PublishTime,
                KeywordId = x.SearchkeywordId,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            //获取包含分组ID的链接发布时间信息
            List<LinkStatus> linkList = new List<LinkStatus>();
            foreach (var x in queryDatas)
            {

                DateTime tmpDt = new DateTime();
                DateTime.TryParse(x.PublishTime, out tmpDt);
                LinkStatus v = new LinkStatus();
                v.PublishTime = tmpDt;
                v.CategoryId = keyToCate[x.KeywordId];
                v.Title = x.Title;
                v.Description = x.Description;
                linkList.Add(v);
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
            List<CategoryList> categoryList = new List<CategoryList>(); //建立分组
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
            //同样再对项目名查询一遍
            var namefilter2 = Builders<IW2S_Project>.Filter.In(x => x._id, cateIds);
            var nameList2 = MongoDBHelper.Instance.GetIW2S_Projects().Find(namefilter2).Project(x => new
            {
                Name = x.Name,
                CategoryId = x._id.ToString()
            }).ToList();
            foreach (var x in nameList2)
            {
                CategoryList cat = categoryList.Find(s => s.CategoryId.Equals(x.CategoryId));
                cat.CategoryName = x.Name;
            }

            //获取各分组内数据的发布时间
            foreach (var cate in categoryList)
            {
                var links = linkList.FindAll(x => x.CategoryId.Contains(cate.CategoryId));     //因为链接可能有多个分组
                cate.PublishTime.AddRange(links.Select(x=>x.PublishTime));
            }

            List<LineData> lineData = new List<LineData>();

            //top数据
            List<TopData> topData = new List<TopData>();

            //遍历数组，获取不同分组的数据
            foreach (var categoryData in categoryList)
            {
                LineData line = new LineData();
                line.name = categoryData.CategoryName;

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

                line.LinkCount = linkCounts;
                lineData.Add(line);

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
            JiebaController jieba = new JiebaController();
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
        /// 获取时间标题数据列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="prjId"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_Timelevel1linkDto> GetTimeLinkList(string categoryId, string pubTime)
        {
            QueryResult<IW2S_Timelevel1linkDto> result = new QueryResult<IW2S_Timelevel1linkDto>();

            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
            }
            //判断是否为根分组
            bool cateIsRoot = false;
            if (cateIds.Count == 1 && cateIds[0].Equals(ObjectId.Empty))
            {
                cateIsRoot = true;
            }
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var filterMapping = groupBuilder.Eq(x => x.IsDel, false);
            var builder = Builders<Dnl_Link_Baidu>.Filter;

            var keywordList = new List<string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (!cateIsRoot)
            {
                //去除根结点
                cateIds.Remove(ObjectId.Empty);
                //获取所给节点ID下关键词ID
                filterMapping &= groupBuilder.In(x => x.CategoryId, cateIds);
            }
            else
            {
                filterMapping &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
            }
            keywordList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMapping).Project(x => x.KeywordId.ToString()).ToList();

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");

            var allQueryDatas = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(allTimeFilter).Project(x => new
            {
                PublishTime = x.PublishTime,
                Title = x.Title,
                LinkUrl = x.LinkUrl,
                Domain = x.Domain,
                Keywords = x.Keywords
            }).ToList();

            //将发布时间从string转为DateTime
            List<IW2S_Timelevel1linkDto> datas = new List<IW2S_Timelevel1linkDto>();
            foreach (var gr in allQueryDatas)
            {
                DateTime tpTime = new DateTime();
                DateTime.TryParse(gr.PublishTime, out tpTime);
                if (tpTime == DateTime.MinValue)
                {
                    continue;
                }
                IW2S_Timelevel1linkDto data = new IW2S_Timelevel1linkDto();
                data.PublishTime = tpTime;
                data.Title = gr.Title;
                data.LinkUrl = gr.LinkUrl;
                data.Domain = gr.Domain;
                data.Keywords = gr.Keywords;
                datas.Add(data);
            }
            DateTime pubTimedt = DateTime.MinValue;
            DateTime.TryParse(pubTime, out pubTimedt);
            if (pubTimedt != DateTime.MinValue)
            {
                //获取时间范围内数据
                DateTime start = pubTimedt.AddDays(-7);
                datas = datas.Where(x => x.PublishTime <= pubTimedt && x.PublishTime >= start).ToList();
                //删除其他词组内的关键词
            }
            datas = datas.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.Title).ToList();

            //去除标题重复的数据，并将同一标题对应的多个关键词合并到一起
            List<IW2S_Timelevel1linkDto> repeat = new List<IW2S_Timelevel1linkDto>();
            for (int i = 0; i < datas.Count; i++)
            {
                repeat = datas.FindAll(x => x.LinkUrl == datas[i].LinkUrl);
                for (int j = 1; j < repeat.Count; j++)
                {
                    repeat[0].Keywords += "；" + repeat[j].Keywords;
                    datas.Remove(repeat[j]);
                }
            }
            //格式化网址
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i].Domain = datas[i].Domain.Replace("http://", "");
                datas[i].Domain = datas[i].Domain.Replace("https://", "");
                int pos = datas[i].Domain.IndexOf('/');
                if (pos > 0)
                {
                    datas[i].Domain = datas[i].Domain.Substring(0, pos);
                }
            }
            datas = datas.OrderByDescending(x => x.PublishTime).ToList();
            result.Count = datas.Count;
            result.Result = datas;


            return result;
        }


        #region 辅助函数
        private List<string> GetIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").ToList();
        }

        private List<ObjectId> GetObjIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
        }
        #endregion
    }
}