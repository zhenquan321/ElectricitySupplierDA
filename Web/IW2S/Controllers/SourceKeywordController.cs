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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IW2S.Controllers
{
    public class SourceKeywordController : ApiController
    {
        /// <summary>
        /// 插入百度热词搜索关键词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="keywords">多个词以;隔开</param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto insertKeyword(string user_id, string keywords, string projectId)
        {
            var keywordList = keywords.Split(';', '；');
            var builder = Builders<IW2S_BaiduKeyword>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
            ResultDto result = new ResultDto();

            var builderCommend = Builders<IW2S_BaiduCommend>.Filter;

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();

            foreach (var keyword in keywordList)
            {
                if (string.IsNullOrEmpty(keyword)) continue;

                var filter = builder.Eq(x => x.Keyword, keywords);
                //if (!string.IsNullOrEmpty(user_id))
                //{
                //    filter &= builder.Eq(x => x.UsrId, new ObjectId(user_id));
                //}
                if (!string.IsNullOrEmpty(projectId))
                {
                    filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                }
                filter &= builder.Eq(x => x.IsRemoved, false);

                var dto = col.Find(filter).FirstOrDefault();

                if (dto != null)
                {
                    if (!string.IsNullOrEmpty(user_id))
                    {
                        result.Message += "关键词‘" + keywords + "’已经存在！";
                        continue;
                    }
                    else
                    {
                        result.IsSuccess = true;
                        continue;
                    }
                }
                IW2S_BaiduKeyword kw = new IW2S_BaiduKeyword
                {
                    _id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = new ObjectId(projectId),
                    BotStatus = 0,
                    Keyword = keyword,
                    ProjectName = keyword,
                    UsrId = usrObjId
                };
                //if (!string.IsNullOrEmpty(user_id))
                //{
                //    kw.UsrId = new ObjectId(user_id);
                //}
                col.InsertOne(kw);


                var filterCommend = builderCommend.Eq(x => x.KeywordId, kw._id);
                filterCommend &= builderCommend.Eq(x => x.UsrId, usrObjId);

                if (!string.IsNullOrEmpty(projectId))
                {
                    filterCommend &= builderCommend.Eq(x => x.ProjectId, new ObjectId(projectId));
                }

                filterCommend &= builderCommend.Eq(x => x.IsRemoved, false);
                var colCommend = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
                var dtoCommend = colCommend.Find(filterCommend).FirstOrDefault();

                if (dtoCommend != null)
                {

                    result.IsSuccess = true;
                    return result;

                }
                IW2S_BaiduCommend kwCommend = new IW2S_BaiduCommend
                {
                    _id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = new ObjectId(projectId),
                    BotStatus = 0,
                    CommendKeyword = keyword,
                    SearchSource = 1,
                    BotIntervalHours = 7 * 24,
                    IsRemoved = false,
                    Keyword = keyword,
                    KeywordId = kw._id,
                    GroupNumber = 0,
                    JisuanStatus = 0,
                    UsrId = usrObjId
                };
                //if (!string.IsNullOrEmpty(user_id))
                //{
                //    kwCommend.UsrId = new ObjectId(user_id);
                //}
                colCommend.InsertOne(kwCommend);
            }
            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;

            return result;
        }

        [HttpGet]
        //插入百度搜索关键词
        public ResultDto insertBaiduSearchKeyword(string user_id, string keywords, string projectId)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.CommendKeyword, keywords);
            //if (!string.IsNullOrEmpty(user_id))
            //{
            //    filter &= builder.Eq(x => x.UsrId, new ObjectId(user_id));
            //}
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            filter &= builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var col = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var dto = col.Find(filter).FirstOrDefault();
            ResultDto result = new ResultDto();
            if (dto != null)
            {
                if (!string.IsNullOrEmpty(user_id))
                {
                    result.Message = "关键词‘" + keywords + "’已经存在！";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    return result;
                }
            }
            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            IW2S_BaiduCommend kw = new IW2S_BaiduCommend
            {
                _id = ObjectId.GenerateNewId(),
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                BotStatus = 0,
                CommendKeyword = keywords,
                SearchSource = 1,
                BotIntervalHours = 7 * 24,
                IsRemoved = false,
                UsrId = usrObjId
            };
            //if (!string.IsNullOrEmpty(user_id))
            //{
            //    kw.UsrId = new ObjectId(user_id);
            //}
            col.InsertOne(kw);

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;

            return result;
        }

        [HttpGet]
        //修改搜索关键词名称
        public ResultDto UpdateKeyword(string keyword_id, string keywords)
        {
            ResultDto result = new ResultDto();
            if (string.IsNullOrEmpty(keywords))
            {
                result.Message = "关键词不能为空";
                return result;
            }
            var builder = Builders<IW2S_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(keyword_id));
            var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "ProjectName", keywords } } } };
            col.UpdateOne(filter, update);

            result.IsSuccess = true;
            return result;

        }

        //获取百度推荐关键词
        [HttpGet]
        public List<IW2S_BaiduCommendDto> GetBaiduCommendKeyword(string user_id, string keywordId, string projectId)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, false);
            if (!string.IsNullOrEmpty(keywordId))
            {
                filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            }
            //if (!string.IsNullOrEmpty(user_id))
            //{
            //    filter &= builder.Eq(x => x.UsrId, new ObjectId(user_id));
            //}
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            var TaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            List<string> keyNameList = new List<string>();
            foreach (var item in TaskList)
            {
                if (keyNameList.Contains(item.CommendKeyword)) continue;
                IW2S_BaiduCommendDto v = new IW2S_BaiduCommendDto();
                v._id = item._id.ToString();
                v.CommendKeyword = item.CommendKeyword;
                v.Keyword = item.Keyword;
                v.drag = true;
                v.Times = item.Times + 1;
                v.KeywordId = item.KeywordId.ToString();
                v.BotStatus = item.BotStatus;
                v.ProjectId = item.ProjectId.ToString();

                keyNameList.Add(item.CommendKeyword);
                list.Add(v);
            }
            return list;
        }

        //获取直搜百度的关键词
        [HttpGet]
        public List<IW2S_BaiduCommendDto> GetBaiduSearchKeyword(string user_id)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(user_id));
            filter &= builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            foreach (var item in TaskList)
            {
                IW2S_BaiduCommendDto v = new IW2S_BaiduCommendDto();
                v._id = item._id.ToString();
                v.CommendKeyword = item.CommendKeyword;
                v.Keyword = item.Keyword;
                v.drag = true;
                v.Times = item.Times;
                v.BotStatus = item.BotStatus;

                list.Add(v);
            }
            return list;
        }

        //获取排除的百度推荐关键词
        [HttpGet]
        public List<IW2S_BaiduCommendDto> GetExcludeBaiduCommend(string user_id, string keyword, string projectId)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.Keyword, keyword);//builder.Eq(x => x.UsrId, new ObjectId(user_id));
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            filter &= builder.Eq(x => x.IsRemoved, true);

            var TaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            foreach (var item in TaskList)
            {
                IW2S_BaiduCommendDto v = new IW2S_BaiduCommendDto();
                v._id = item._id.ToString();
                v.CommendKeyword = item.CommendKeyword;
                v.Keyword = item.Keyword;
                v.Times = item.Times;
                v.drag = true;
                v.ProjectId = item.ProjectId.ToString();
                list.Add(v);
            }
            return list;
        }

        [HttpPost]
        //排除百度推荐关键词
        public string ExcludeKeyword(List<IW2S_BaiduCommendDto> lists)
        {
            foreach (var list in lists)
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", list.IsRemoved } } } };

                var result = MongoDBHelper.Instance.GetIW2S_BaiduCommends().UpdateOne(new QueryDocument { { "_id", new ObjectId(list._id) } }, update).ModifiedCount;

                var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", list.IsRemoved } } } };
                MongoDBHelper.Instance.GetIW2S_KeywordGroups().UpdateMany(new QueryDocument { { "BaiduCommendId", new ObjectId(list._id) } }, updategroup);

                //排除关键词搜索出来的链接
                var updateLinks = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", 2 } } } };
                MongoDBHelper.Instance.GetIW2S_level1links().UpdateMany(new QueryDocument { { "SearchkeywordId", list._id } }, updateLinks);

                //排队谷歌关键词

            }
            return "成功！";
        }

        [HttpPost]
        //排除百度关键词
        public string ExcludeBaiduKeyword(List<IW2S_BaiduKeywordDto> lists)
        {
            foreach (var list in lists)
            {

                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", list.IsRemoved } } } };
                var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
                var result = col.UpdateOne(new QueryDocument { { "_id", new ObjectId(list._id) } }, update).ModifiedCount;

                //IW2S_KeywordCategory
                var updatecategory = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", list.IsRemoved } } } };
                MongoDBHelper.Instance.GetIW2S_KeywordCategorys().UpdateMany(new QueryDocument { { "KeywordId", new ObjectId(list._id) } }, updatecategory);

                var commendIds = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(new QueryDocument { { "KeywordId", new ObjectId(list._id) } }).Project(x => new IW2S_BaiduCommendDto { IsRemoved = true, _id = x._id.ToString() }).ToList();

                //排除关键词搜索出来的链接
                var updateLinks = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", 2 } } } };
                MongoDBHelper.Instance.GetIW2S_level1links().UpdateMany(new QueryDocument { { "SearchkeywordId", list._id } }, updateLinks);

                ExcludeKeyword(commendIds);
            }
            return "成功！";
        }

        /// <summary>
        /// 获取搜索链接数据
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="projectId"></param>
        /// <param name="categoryId">多个Id以;分开</param>
        /// <param name="keywordId">多个Id以;分开</param>
        /// <param name="Title"></param>
        /// <param name="domain"></param>
        /// <param name="infriLawCode"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_level1linkDto> GetLevelLinks(string user_id, string projectId, string categoryId, string keywordId, string Title, string domain, string infriLawCode, byte? status, int page, int pagesize)
        {
            //if (string.IsNullOrEmpty(keywordId) && string.IsNullOrEmpty(categoryId))
            //{
            //    return new QueryResult<IW2S_level1linkDto> { Count = 0, Result = new List<IW2S_level1linkDto>() };
            //}
            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id))
            //谷歌数据
            var builderGl = Builders<Dnl_Google_level1link>.Filter;
            var filterGl = builderGl.Eq(x => x.ProjectId, new ObjectId(projectId));
            if (!string.IsNullOrEmpty(Title))
            {
                filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + Title + ".*/"));
                filterGl &= builderGl.Regex(x => x.Title, new BsonRegularExpression("/.*" + Title + ".*/"));
            }

            //若infriLawCode为空，则查找所有数据
            if (!string.IsNullOrEmpty(infriLawCode))
            {
                //ObjectId lawCodeId = ObjectId.Empty;
                //ObjectId.TryParse(infriLawCode, out lawCodeId);
                //if (infriLawCode.Equals("000000000000000000000000"))
                //{
                //    filter = filter & builder.Eq(x => x.InfriLawCode, new ObjectId("000000000000000000000000"));
                //}
                //else
                //{
                List<ObjectId> InfriList = GetObjIdListFromStr(infriLawCode);
                filter = filter & builder.In(x => x.InfriLawCode, InfriList);
                filterGl &= builderGl.In(x => x.InfriLawCode, InfriList);
                //}
            }

            List<string> commendIds = new List<string>();
            if (!string.IsNullOrEmpty(keywordId))
            {
                var keyIds = GetIdListFromStr(keywordId);
                if (keyIds.Count > 0)
                {
                    commendIds.AddRange(keyIds);
                }
            }
            if (!string.IsNullOrEmpty(categoryId))
            {
                var keyIds = GetObjIdListFromStr(categoryId);
                if (keyIds.Count > 0)
                {
                    var buildGroup = Builders<IW2S_KeywordGroup>.Filter;
                    var filterGroup = buildGroup.In(x => x.CommendCategoryId, GetObjIdListFromStr(categoryId));
                    var keywordIdsGroup = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(filterGroup).Project(x => x.BaiduCommendId.ToString()).ToList();
                    if (keywordIdsGroup.Count > 0)
                    {
                        commendIds.AddRange(keywordIdsGroup);
                    }
                }
            }
            if (commendIds.Count > 0)
            {
                filter &= builder.In(x => x.SearchkeywordId, commendIds);
                filterGl &= builderGl.In(x => x.SearchkeywordId, commendIds);
            }

            if (!string.IsNullOrEmpty(domain))
            {
                filter = filter & builder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + domain + ".*/"));
                filterGl &= builderGl.Regex(x => x.Domain, new BsonRegularExpression("/.*" + domain + ".*/"));
            }
            if (status.HasValue)
            {
                filter &= builder.Eq(x => x.DataCleanStatus, status.Value);
                filterGl &= builderGl.Eq(x => x.DataCleanStatus, status.Value);
            }
            else
            {
                filter &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
                filterGl &= builderGl.Eq(x => x.DataCleanStatus, (byte)2);
            }

            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            var colGl = MongoDBHelper.Instance.GetDnl_Google_level1links();

            var query = col.Find(filter).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Abstract = x.Abstract,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                DataCleanStatus = x.DataCleanStatus,
                CreatedAt = x.CreatedAt,
                AppType = x.AppType,
                MatchAt = x.MatchAt,
                MatchType = x.MatchType,
                Score = x.Score,
                InfriLawCode = x.InfriLawCode.ToString(),
                InfriLawCodeStr = "",
                PublishTime = x.PublishTime
            }).ToList();

            var queryGl = colGl.Find(filterGl).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Abstract = x.Abstract,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                DataCleanStatus = x.DataCleanStatus,
                CreatedAt = x.CreatedAt,
                AppType = x.AppType,
                MatchAt = x.MatchAt,
                MatchType = x.MatchType,
                Score = x.Score,
                InfriLawCode = x.InfriLawCode.ToString(),
                InfriLawCodeStr = "",
                PublishTime = x.PublishTime
            }).ToList();
            query.AddRange(queryGl);

            var count = query.Count();
            var TaskList = query.Skip((page) * pagesize).Take(pagesize).ToList();
            var linkList = new List<IW2S_level1linkDto>();      //最后输出的链接列表
            //转换时间格式
            foreach (var x in TaskList)
            {
                DateTime tpt = new DateTime();    //临时时间，转换格式时使用
                var link = new IW2S_level1linkDto
                {
                    _id = x._id,
                    Title = x.Title,
                    Description = x.Description,
                    Abstract = x.Abstract,
                    Keywords = x.Keywords,
                    Domain = x.Domain,
                    LinkUrl = x.LinkUrl,
                    DataCleanStatus = x.DataCleanStatus,
                    CreatedAt = x.CreatedAt,
                    AppType = x.AppType,
                    MatchAt = x.MatchAt,
                    MatchType = x.MatchType,
                    Score = x.Score,
                    InfriLawCode = x.InfriLawCode,
                    InfriLawCodeStr = null,
                };
                DateTime.TryParse(x.PublishTime, out tpt);
                link.PublishTime = tpt;
                linkList.Add(link);
            }
            linkList = linkList.OrderByDescending(x => x.PublishTime).ToList();
            //if (!string.IsNullOrEmpty(infriLawCode))
            //{
            //    foreach (var list in TaskList)
            //    {
            //        var builderInfri = Builders<IW2S_AnalysisItemValue>.Filter.Eq(x => x._id, new ObjectId(list.InfriLawCode));
            //        string InfriName = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues().Find(builderInfri).Project(x => x.Name).FirstOrDefault();
            //        list.InfriLawCodeStr = InfriName;
            //    }
            //}


            return new QueryResult<IW2S_level1linkDto> { Count = count, Result = linkList };
        }

        //获取监测结果页面搜索链接数据
        [HttpPost]
        public QueryResultView<IW2S_level1linkDto> GetLevelLinksView(LinkSearchData linkSearchData)
        {
            //if (string.IsNullOrEmpty(keywordId) && string.IsNullOrEmpty(categoryId))
            //{
            //    return new QueryResultView<IW2S_level1linkDto> { Count = 0, Result = new List<IW2S_level1linkDto>() };
            //}
            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id))
            var builderGl = Builders<Dnl_Google_level1link>.Filter;
            var filterGl = builderGl.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));
            //单独查询最后一个关键词
            var filterLast = builder.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) 
            var filterLastGl = builderGl.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));
            if (!string.IsNullOrEmpty(linkSearchData.Title))
            {
                //模糊查询
                filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + linkSearchData.Title + ".*/"));

                filterGl &= builderGl.Regex(x => x.Title, new BsonRegularExpression("/.*" + linkSearchData.Title + ".*/"));
            }

            //若infriLawCode为空，则查找未设置侵权类型数据，否则拆分后查找数据
            if (!string.IsNullOrEmpty(linkSearchData.infriLawCode))
            {
                //ObjectId lawCodeId = ObjectId.Empty;
                //ObjectId.TryParse(infriLawCode, out lawCodeId);
                if (linkSearchData.infriLawCode.Equals("000000000000000000000000"))
                {
                    filter = filter & builder.Eq(x => x.InfriLawCode, new ObjectId("000000000000000000000000"));

                    filterGl = filterGl & builderGl.Eq(x => x.InfriLawCode, new ObjectId("000000000000000000000000"));
                }
                else
                {
                    List<ObjectId> InfriList = GetObjIdListFromStr(linkSearchData.infriLawCode);
                    filter = filter & builder.In(x => x.InfriLawCode, InfriList);

                    filterGl &= builderGl.In(x => x.InfriLawCode, InfriList);
                }
            }

            //获取关键词ID列表
            List<string> commendIds = new List<string>();
            string lastKeyword = "";
            if (!string.IsNullOrEmpty(linkSearchData.keywordId))
            {
                var keyIds = GetIdListFromStr(linkSearchData.keywordId);
                lastKeyword = keyIds[keyIds.Count - 1];
                if (keyIds.Count > 0)
                {
                    commendIds.AddRange(keyIds);
                }
            }

            if (commendIds.Count > 0)
            {
                filter &= builder.In(x => x.SearchkeywordId, commendIds);

                filterGl &= builderGl.In(x => x.SearchkeywordId, commendIds);
            }

            //域名筛选
            if (!string.IsNullOrEmpty(linkSearchData.domain))
            {
                filter = filter & builder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + linkSearchData.domain + ".*/"));

                filterGl &= builderGl.Regex(x => x.Domain, new BsonRegularExpression("/.*" + linkSearchData.domain + ".*/"));
            }

            //值筛选
            if (linkSearchData.status.HasValue)
            {
                filter &= builder.Eq(x => x.DataCleanStatus, linkSearchData.status.Value);
                filterGl &= builderGl.Eq(x => x.DataCleanStatus, linkSearchData.status.Value);
            }
            else
            {
                filter &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
                filterGl &= !builderGl.Eq(x => x.DataCleanStatus, (byte)2);
            }

            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            var colGl = MongoDBHelper.Instance.GetDnl_Google_level1links();

            var query = col.Find(filter).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Abstract = x.Abstract,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                DataCleanStatus = x.DataCleanStatus,
                CreatedAt = x.CreatedAt,
                AppType = x.AppType,
                MatchAt = x.MatchAt,
                MatchType = x.MatchType,
                Score = x.Score,
                InfriLawCode = x.InfriLawCode.ToString(),
                //InfriLawCodeStr = null,
                PublishTime = x.PublishTime
            }).ToList();
            //获取谷歌数据
            var queryGl = colGl.Find(filterGl).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Abstract = x.Abstract,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                DataCleanStatus = x.DataCleanStatus,
                CreatedAt = x.CreatedAt,
                AppType = x.AppType,
                MatchAt = x.MatchAt,
                MatchType = x.MatchType,
                Score = x.Score,
                InfriLawCode = x.InfriLawCode.ToString(),
                //InfriLawCodeStr = null,
                PublishTime = x.PublishTime
            }).ToList();

            query.AddRange(queryGl);

            var count = query.Count();
            var TaskList = query.Skip((linkSearchData.page) * linkSearchData.pagesize).Take(linkSearchData.pagesize).ToList();

            var linkList = new List<IW2S_level1linkDto>();      //最后输出的链接列表
            //转换时间格式
            foreach (var x in TaskList)
            {
                DateTime tpt = new DateTime();    //临时时间，转换格式时使用
                var link = new IW2S_level1linkDto
                {
                    _id = x._id,
                    Title = x.Title,
                    Description = x.Description,
                    Abstract = x.Abstract,
                    Keywords = x.Keywords,
                    Domain = x.Domain,
                    LinkUrl = x.LinkUrl,
                    DataCleanStatus = x.DataCleanStatus,
                    CreatedAt = x.CreatedAt,
                    AppType = x.AppType,
                    MatchAt = x.MatchAt,
                    MatchType = x.MatchType,
                    Score = x.Score,
                    InfriLawCode = x.InfriLawCode,
                    InfriLawCodeStr = null,
                };
                DateTime.TryParse(x.PublishTime, out tpt);
                link.PublishTime = tpt;
                linkList.Add(link);
            }
            if (!linkSearchData.infriLawCode.Equals("000000000000000000000000"))
            {
                foreach (var list in linkList)
                {
                    var builderInfri = Builders<IW2S_AnalysisItemValue>.Filter.Eq(x => x._id, new ObjectId(list.InfriLawCode));
                    string InfriName = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues().Find(builderInfri).Project(x => x.Name).FirstOrDefault();
                    list.InfriLawCodeStr = InfriName;
                }
            }
            linkList = linkList.OrderByDescending(x => x.PublishTime).ToList();

            bool hasvalue = true;
            //判断最后一个关键词是否有数据
            if (!string.IsNullOrEmpty(lastKeyword))
            {
                filterLast = filter;
                filterLast &= builder.Eq(x => x.SearchkeywordId, lastKeyword);

                filterLastGl = filterGl;
                filterLastGl &= builderGl.Eq(x => x.SearchkeywordId, lastKeyword);

                long LastCount = col.Find(filterLast).Count() + colGl.Find(filterGl).Count();
                if (LastCount <= 0)
                {
                    hasvalue = false;
                }
            }


            return new QueryResultView<IW2S_level1linkDto> { Count = count, Result = linkList, HasValue = hasvalue, infriLawCode = linkSearchData.infriLawCode };
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">1,收藏;2,排除</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetLinkStatus(string user_id, string id, byte? status)
        {
            ResultDto result = new ResultDto();

            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(id));
            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            IW2S_level1link linkUrlPrj = col.Find(filter).Project(x => new IW2S_level1link
            {
                LinkUrl = x.LinkUrl,
                ProjectId = x.ProjectId
            }).FirstOrDefault();
            //如果百度无数据则检查谷歌搜索结果
            var updfilter = builder.Eq(x => x.LinkUrl, linkUrlPrj.LinkUrl) & builder.Eq(x => x.ProjectId, linkUrlPrj.ProjectId);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", status } } } };
            col.UpdateMany(updfilter, update);

            if (status == 1)
            {
                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = linkUrlPrj.ProjectId,
                    ShareOperateType = (int)ShareOperateType.CollectConfig,
                    UserId = new ObjectId(user_id)
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 设置链接侵权类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="infriType"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetLinkInfriType(string user_id, string id, string infriType)
        {
            ResultDto result = new ResultDto();
            try
            {


                var builder = Builders<IW2S_level1link>.Filter;
                var col = MongoDBHelper.Instance.GetIW2S_level1links();

                UpdateDocument update = null;
                ObjectId lawCodeId = ObjectId.Empty;
                ObjectId.TryParse(infriType, out lawCodeId);
                update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", lawCodeId } } } };

                var idList = id.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList();
                foreach (var idstr in idList)
                {
                    var filter = builder.Eq(x => x._id, new ObjectId(idstr));
                    col.UpdateOne(filter, update);
                }
                if (idList.Count > 0)
                {
                    var filter = builder.Eq(x => x._id, new ObjectId(idList[0]));

                    var prjId = col.Find(filter).Project(x => x.ProjectId).FirstOrDefault();
                    IW2S_OperateLog log = new IW2S_OperateLog
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        ProjectId = prjId,
                        ShareOperateType = (int)ShareOperateType.SetLinkAnalysisItem,
                        UserId = new ObjectId(user_id)
                    };
                    MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
                }

                result.IsSuccess = true;
            }
            catch
            {
                result.IsSuccess = false;
                result.Message = "服务器连接出错！";
            }
            return result;
        }

        private void RefreshValidLinkCount(IW2S_BaiduCommend p)
        {
            var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt",DateTime.UtcNow.AddHours(8)},
                {"BotStatus",2}} }};
            var commendCol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);

            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.UsrId, p.UsrId);
            filter &= builder.Eq(x => x.SearchkeywordId, p._id.ToString());
            filter &= builder.Ne(x => x.DataCleanStatus, (byte)2);
            filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/")) |
        builder.Regex(x => x.Description, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/"));

            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            var agreresult = col.Aggregate().Match(filter)
                .Group(new BsonDocument { { "_id", "$_id" }, { "Count", new BsonDocument("$sum", 1) } })
                .ToListAsync()
                .Result;

            var vallinkCount = agreresult.Count;
            update = new UpdateDocument { { "$set", new QueryDocument { { "ValLinkCount", vallinkCount } } } };

            commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
        }

        [HttpGet]
        public List<IW2S_BaiduKeywordDto> GetBaiduKeyword(string user_id, string projectId, int page, int pagesize)
        {

            var builder = Builders<IW2S_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) 
            filter &= builder.Eq(x => x.IsRemoved, false);

            var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
            var TaskList = col.Find(filter).SortByDescending(x => x.CreatedAt).Skip((page) * pagesize).Limit(pagesize).ToList();

            List<IW2S_BaiduKeywordDto> list = new List<IW2S_BaiduKeywordDto>();
            var builder1 = Builders<IW2S_BaiduCommend>.Filter;
            foreach (var item in TaskList)
            {
                IW2S_BaiduKeywordDto v = new IW2S_BaiduKeywordDto();
                v._id = item._id.ToString();
                v.Keyword = item.Keyword;
                v.ProjectName = item.ProjectName;
                if (string.IsNullOrEmpty(v.ProjectName))
                {
                    v.ProjectName = item.Keyword;
                }

                var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));//builder1.Eq(x => x.UsrId, new ObjectId(user_id))
                filter1 &= builder1.Eq(x => x.KeywordId, item._id);
                filter1 &= builder1.Eq(x => x.SearchSource, 1);
                filter1 &= builder1.Eq(x => x.IsRemoved, false);
                var searchData = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter1)
                    .Project(x => new IW2S_BaiduCommendDto { _id = x._id.ToString(), ValLinkCount = x.ValLinkCount, BotStatus = x.BotStatus }).FirstOrDefault();
                if (searchData != null)
                {
                    v.ValLinkCount = searchData.ValLinkCount;
                    v.CommendKeywordId = searchData._id;
                    v.BotStatus = searchData.BotStatus;
                }

                list.Add(v);


                filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));//builder1.Eq(x => x.UsrId, new ObjectId(user_id))
                filter1 &= builder1.Eq(x => x.KeywordId, item._id);
                filter1 &= builder1.Eq(x => x.SearchSource, 0);
                filter1 &= builder1.Eq(x => x.IsRemoved, false);
                var TaskList1 = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter1).SortByDescending(x => x.Times).ToList();
                List<IW2S_BaiduCommendDto> list1 = new List<IW2S_BaiduCommendDto>();
                foreach (var item1 in TaskList1)
                {
                    IW2S_BaiduCommendDto v1 = new IW2S_BaiduCommendDto();
                    v1._id = item1._id.ToString();
                    v1.CommendKeyword = item1.CommendKeyword;
                    v1.Keyword = item.Keyword;
                    v1.drag = true;
                    v1.Times = item1.Times + 1;
                    v1.BotStatus = item1.BotStatus;
                    v1.ValLinkCount = item1.ValLinkCount;
                    v1.isselected = false;
                    list1.Add(v1);
                }
                v.BaiduCommends = list1;
            }
            return list;

        }

        /// <summary>
        /// 新增分组
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="groupName"></param>
        /// <param name="lawCode"></param>
        /// <param name="weight"></param>
        /// <param name="groupType">1:百度直搜，2：百度热词</param>
        /// <param name="keywordIds">多个Id以,隔开</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertKeywordGroup(kgvo sss)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(sss.groupName))
            {
                result.Message = "分组名不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x.ProjectId, new ObjectId(sss.projectId)); //categorybuilder.Eq(x => x.UsrId, new ObjectId(usr_id))
            //categoryfilter &= categorybuilder.Eq(x => x.GroupType, groupType);
            categoryfilter &= categorybuilder.Eq(x => x.Name, sss.groupName) & categorybuilder.Eq(x => x.IsDel, false);

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(sss.projectId))).Project(x => x.UsrId).FirstOrDefault();

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            var keywordIdList = sss.keywordIds.Split(';', '；', ',');
            if (categoryDto != null)
            {
                result.Message = "分组已存在";
                return result;
            }
            else
            {
                categoryDto = new IW2S_KeywordCategory
                {
                    _id = ObjectId.GenerateNewId(),
                    //GroupType = groupType,
                    Name = sss.groupName,
                    //InfriLawCode = lawCode,
                    IsDel = false,
                    UsrId = usrObjId,
                    Weight = sss.weight,
                    ProjectId = new ObjectId(sss.projectId),
                    GroupNumber = 0
                };
                if (!string.IsNullOrEmpty(sss.lawCode))
                {
                    ObjectId lawCodeId = ObjectId.Empty;
                    ObjectId.TryParse(sss.lawCode, out lawCodeId);
                    categoryDto.InfriLawCode = lawCodeId;
                }
                if (!string.IsNullOrEmpty(sss.parentGroupId))
                {
                    categoryDto.ParentId = new ObjectId(sss.parentGroupId);
                }
                if (!string.IsNullOrEmpty(sss.keywordId))
                {
                    categoryDto.KeywordId = new ObjectId(sss.keywordId);
                }
                categoryDto.KeywordTotal = keywordIdList.Length;

                categoryCol.InsertOne(categoryDto);
            }
            if (!string.IsNullOrEmpty(sss.keywordIds))
            {
                int keywordTotal = 0, valLinkCount = 0;
                var keywordBuilder = Builders<IW2S_BaiduCommend>.Filter;
                var keywordCol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
                foreach (string keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {

                        var keywordFilter = keywordBuilder.Eq(x => x._id, new ObjectId(keyId));
                        var keyword = keywordCol.Find(keywordFilter).Project(x => new { CommendKeyword = x.CommendKeyword, ValLinkCount = x.ValLinkCount }).FirstOrDefault();
                        if (keyword != null)
                        {
                            IW2S_KeywordGroup groupDto = new IW2S_KeywordGroup
                            {
                                //GroupType = groupType,
                                BaiduCommendId = new ObjectId(keyId),
                                BaiduCommend = keyword.CommendKeyword,
                                CommendCategoryId = categoryDto._id,
                                ParentCategoryId = categoryDto.ParentId,
                                IsDel = false,
                                UsrId = usrObjId,
                                ProjectId = new ObjectId(sss.projectId)
                            };
                            keywordGroupCol.InsertOne(groupDto);
                            valLinkCount += keyword.ValLinkCount;
                            keywordTotal++;
                        }
                    }
                }
                var update = new UpdateDocument { { "$set", new QueryDocument { { "ValLinkCount", valLinkCount }, { "KeywordTotal", keywordTotal } } } };
                categoryCol.UpdateOne(categoryfilter, update);
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(sss.projectId),
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(sss.usr_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 修改分组名称，父分组Id，关键词
        /// </summary>
        /// <param name="groupid">分组Id</param>
        /// <param name="groupName">分组名</param>
        /// <param name="parentGroupId">父分组Id</param>
        /// <param name="keywordIds">多个Id以,隔开</param>
        /// <returns></returns>
        [HttpPost]
        //string user_id,string groupid, string groupName, string lawCode, int weight, string keywordIds,string parentGroupId
        public ResultDto UpdateKeywordGroup(kgvo sss)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(sss.groupid))
            {
                result.Message = "修改的分组不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x._id, new ObjectId(sss.groupid));

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();

            if (categoryDto == null)
            {
                result.Message = "修改的分组不能为空";
                return result;
            }

            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, categoryDto._id);

            ObjectId parentId = new ObjectId("000000000000000000000000");
            if (!string.IsNullOrEmpty(sss.parentGroupId))
            {
                parentId = new ObjectId(sss.parentGroupId);
            }

            bool isChangeParent = parentId != categoryDto.ParentId;
            var alloldCommendIds = keywordGroupCol.Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            keywordGroupCol.DeleteMany(groupfilter);
            var keywordIdList = sss.keywordIds.Split(';', '；', ',');

            int keywordTotal = 0;
            bool isLevel1 = categoryDto.ParentId.Equals(new ObjectId("000000000000000000000000")) ? true : false;
            if (!string.IsNullOrEmpty(sss.keywordIds))
            {
                var builder = Builders<IW2S_BaiduCommend>.Filter;
                foreach (var keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {
                        //alloldCommendIds.Remove(new ObjectId(keyId));

                        IW2S_KeywordGroup groupDto = new IW2S_KeywordGroup
                        {
                            //GroupType = categoryDto.GroupType,
                            BaiduCommendId = new ObjectId(keyId),
                            ParentCategoryId = parentId,//categoryDto.ParentId,
                            CommendCategoryId = categoryDto._id,
                            IsDel = false,
                            UsrId = categoryDto.UsrId,
                            ProjectId = categoryDto.ProjectId
                        };

                        var filter = builder.Eq(x => x._id, new ObjectId(keyId));
                        var keywordStr = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Project(x => x.CommendKeyword).FirstOrDefault();
                        if (string.IsNullOrEmpty(keywordStr))
                        {
                            continue;
                        }
                        groupDto.BaiduCommend = keywordStr;

                        keywordGroupCol.InsertOne(groupDto);
                        keywordTotal++;
                        if (isLevel1)
                        {
                            UpdateLinksInfriType(keyId, sss.lawCode);
                        }
                    }
                }
            }
            var infriLawCode = new ObjectId(sss.lawCode);
            if (string.IsNullOrEmpty(sss.lawCode))
            {
                infriLawCode = new ObjectId("000000000000000000000000");
            }
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", sss.groupName }, { "ParentId", parentId }, { "InfriLawCode", infriLawCode }, { "Weight", sss.weight }, { "KeywordTotal", keywordTotal } } } };
            categoryCol.UpdateOne(categoryfilter, update);

            if (alloldCommendIds.Count > 0)
            {
                if (isChangeParent)
                {
                    RecurseDelParentKeyword(categoryDto, alloldCommendIds);
                    RecurseAddParentKeyword(parentId, alloldCommendIds);
                }
                if (isLevel1)
                {
                    foreach (var alloldCommendId in alloldCommendIds)
                    {
                        if (!keywordIdList.Contains(alloldCommendId.ToString()))
                        {
                            UpdateLinksInfriType(alloldCommendId.ToString(), "000000000000000000000000");
                        }
                    }
                }
            }
            if (isChangeParent)
            {
                categoryfilter = categorybuilder.Eq(x => x._id, parentId);
                infriLawCode = categoryCol.Find(categoryfilter).Project(x => x.InfriLawCode).FirstOrDefault();
                categoryCol.UpdateOne(categorybuilder.Eq(x => x._id, categoryDto._id), new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", infriLawCode } } } });

                RecurseUpdateSubInfriType(categoryDto._id, infriLawCode);
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = categoryDto.ProjectId,
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(sss.user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }
        private void RecurseUpdateSubInfriType(ObjectId parentId, ObjectId lawCode)
        {
            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;
            var categoryfilter = categorybuilder.Eq(x => x.ParentId, parentId);
            var updateca = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", lawCode } } } };
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            categoryCol.UpdateOne(categoryfilter, updateca);

            categoryfilter = categorybuilder.Eq(x => x.ParentId, parentId);

            var parentcategoryDtos = categoryCol.Find(categoryfilter).ToList();
            foreach (var parentcategoryDto in parentcategoryDtos)
            {
                RecurseUpdateSubInfriType(parentcategoryDto._id, lawCode);
            }
        }

        private void UpdateLinksInfriType(string searchKeywordId, string lawCode)
        {
            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.SearchkeywordId, searchKeywordId);

            ObjectId lawCodeId = ObjectId.Empty;
            ObjectId.TryParse(lawCode, out lawCodeId);

            var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", lawCodeId } } } };

            MongoDBHelper.Instance.GetIW2S_level1links().UpdateMany(filter, update);
        }

        private void RecurseDelParentKeyword(IW2S_KeywordCategory categoryDto, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            var categoryfilter = categorybuilder.Eq(x => x._id, categoryDto.ParentId);

            var parentcategoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            if (parentcategoryDto != null)
            {
                long delCount = 0;

                var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentcategoryDto._id) & groupbuilder.In(x => x.BaiduCommendId, alloldCommendIds);

                keywordGroupCol.DeleteMany(groupfilter);

                groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentcategoryDto._id);
                delCount = keywordGroupCol.Find(groupfilter).Project(x => x._id).Count();

                var updateca = new UpdateDocument { { "$set", new QueryDocument { { "KeywordTotal", delCount } } } };
                categoryfilter = categorybuilder.Eq(x => x._id, parentcategoryDto._id);
                categoryCol.UpdateOne(categoryfilter, updateca);
                //
                RecurseDelParentKeyword(parentcategoryDto, alloldCommendIds);

            }
            else
            {
                return;
            }
        }
        private void RecurseAddParentKeyword(ObjectId parentId, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            var categoryfilter = categorybuilder.Eq(x => x._id, parentId);

            var parentcategoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            if (parentcategoryDto != null)
            {
                var builder = Builders<IW2S_BaiduCommend>.Filter;
                foreach (var keyId in alloldCommendIds)
                {

                    //alloldCommendIds.Remove(keyId);

                    IW2S_KeywordGroup groupDto = new IW2S_KeywordGroup
                    {
                        //GroupType = categoryDto.GroupType,
                        BaiduCommendId = keyId,
                        ParentCategoryId = parentcategoryDto.ParentId,
                        CommendCategoryId = parentId,
                        IsDel = false,
                        UsrId = parentcategoryDto.UsrId,
                        ProjectId = parentcategoryDto.ProjectId
                    };

                    var filter = builder.Eq(x => x._id, keyId);
                    var keywordStr = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Project(x => x.CommendKeyword).FirstOrDefault();
                    if (string.IsNullOrEmpty(keywordStr))
                    {
                        continue;
                    }
                    groupDto.BaiduCommend = keywordStr;

                    keywordGroupCol.InsertOne(groupDto);
                }

                var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentcategoryDto._id);
                var delCount = keywordGroupCol.Find(groupfilter).Project(x => x._id).Count();

                var updateca = new UpdateDocument { { "$set", new QueryDocument { { "KeywordTotal", delCount } } } };
                categoryfilter = categorybuilder.Eq(x => x._id, parentcategoryDto._id);
                categoryCol.UpdateOne(categoryfilter, updateca);

                if (parentcategoryDto.ParentId != ObjectId.Empty)
                {
                    RecurseAddParentKeyword(parentcategoryDto.ParentId, alloldCommendIds);
                }

            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 修改分组名称
        /// </summary>
        /// <param name="groupid">分组Id</param>
        /// <param name="groupName">分组名</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateKeywordGroupName(string user_id, string groupid, string groupName)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(groupid))
            {
                result.Message = "修改的分组不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            var categorybuilder = Builders<IW2S_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x._id, new ObjectId(groupid));

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();

            if (categoryDto == null)
            {
                result.Message = "修改的分组不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", groupName } } } };
            categoryCol.UpdateOne(categoryfilter, update);

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = categoryDto.ProjectId,
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public List<IW2S_KeywordCategoryDto> GetAllKeywordCategory(string user_id, string projectId)
        {
            List<IW2S_KeywordCategoryDto> result = new List<IW2S_KeywordCategoryDto>();

            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id))

            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).SortBy(x => x.ParentId).ToList();

            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(user_id));

            var keywordBuilder = Builders<IW2S_BaiduCommend>.Filter;
            var keywordCol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            FilterDefinition<IW2S_BaiduCommend> keywordFilter = null;
            ObjectId nullId = new ObjectId("000000000000000000000000");

            foreach (var item in TaskList)
            {
                IW2S_KeywordCategoryDto v = new IW2S_KeywordCategoryDto();
                v._id = item._id.ToString();
                v.KeywordId = item.KeywordId.ToString();
                v.Name = item.Name;
                v.ParentId = item.ParentId.ToString();
                v.Weight = item.Weight;
                v.UsrId = item.UsrId.ToString();
                v.InfriLawCode = item.InfriLawCode.ToString();
                //v.InfriLawCodeStr = CommonHelper.GetLawCodeStr(v.InfriLawCode);
                v.KeywordTotal = item.KeywordTotal;
                v.ValLinkCount = item.ValLinkCount;

                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(user_id)) &
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, item._id);

                var selectedIdList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.BotStatus, 0);
                var unkeyword = keywordCol.Find(keywordFilter).Project(x => x._id).FirstOrDefault();

                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.BotStatus, 1);
                var prokeyword = keywordCol.Find(keywordFilter).Project(x => x._id).FirstOrDefault();


                if (unkeyword == nullId && prokeyword == nullId)
                {
                    v.BotStatus = 2;
                }
                else if (prokeyword != nullId)
                {
                    v.BotStatus = 1;
                }
                else
                {
                    v.BotStatus = 0;
                }
                v.isselected = false;
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        /// 获取分组
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="groupType">1:百度直搜，2：百度热词</param>
        /// <param name="keywordId">关键词ID</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_KeywordCategoryDto> GetKeywordCategory(string user_id, string projectId, int groupType, string keywordId)
        {
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) &
            //if (!string.IsNullOrEmpty(keywordId))
            //{
            //    filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            //}
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).SortBy(x => x.ParentId).ToList();

            List<IW2S_KeywordCategoryDto> list = new List<IW2S_KeywordCategoryDto>();

            Dictionary<string, string> dicCategoryIDName = new Dictionary<string, string>();

            var commendbuilder = Builders<IW2S_BaiduCommend>.Filter;

            var commendfilter = commendbuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & commendbuilder.Eq(x => x.IsRemoved, false); //commendbuilder.Eq(x => x.UsrId, new ObjectId(user_id)) & 
            var keywordCount = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(commendfilter).Project(x => x._id).Count();

            list.Add(new IW2S_KeywordCategoryDto
            {
                _id = "",
                Name = "所有词",
                KeywordTotal = (int)keywordCount
            });

            foreach (var item in TaskList)
            {
                IW2S_KeywordCategoryDto v = new IW2S_KeywordCategoryDto();
                v._id = item._id.ToString();
                v.KeywordId = item.KeywordId.ToString();
                v.Name = item.Name;
                v.ParentId = item.ParentId.ToString();
                v.Weight = item.Weight;
                v.UsrId = item.UsrId.ToString();
                v.InfriLawCode = item.InfriLawCode.ToString();
                //v.InfriLawCodeStr = CommonHelper.GetLawCodeStr(v.InfriLawCode);
                v.KeywordTotal = item.KeywordTotal;

                if (v.ParentId != "000000000000000000000000")
                {
                    if (dicCategoryIDName.ContainsKey(v.ParentId))
                    {
                        v.ParentName = dicCategoryIDName[v.ParentId];
                    }
                    else
                    {
                        v.ParentName = TaskList.Where(x => x._id == item.ParentId).Select(x => x.Name).FirstOrDefault();
                        dicCategoryIDName.Add(v.ParentId, v.ParentName);
                    }
                }
                list.Add(v);


            }
            return list;
        }
        /// <summary>
        /// 新增分组获取可用关键词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="groupid"></param>
        /// <param name="keywordId"></param>
        /// <param name="groupType">1:百度直搜，2：百度热词</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_BaiduCommendDto> GetKeywordGroup(string usr_id, string projectId, string groupid)
        {
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            //if(string.IsNullOrEmpty( keywordId))
            //{
            //    return list;
            //}
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId(groupid));
            }
            else
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId("000000000000000000000000"));
            }

            var selectedIdList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<IW2S_BaiduCommendDto> allKeywords = new List<IW2S_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                allKeywords = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_BaiduCommendDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                        CommendKeyword = x.BaiduCommend,
                        drag = true
                    }
                    ).ToList();
            }

            else if (string.IsNullOrEmpty(groupid))
            {
                allKeywords = GetBaiduCommendKeyword(usr_id, null, projectId);
            }

            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).ToList();
            return list;
        }


        [HttpGet]
        public KeywordGroupModelDto GetEditKeywordGroup(string usr_id, string projectId, string groupid, string parentid, string keywordId, int groupType)
        {
            KeywordGroupModelDto result = new KeywordGroupModelDto();
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            //if (string.IsNullOrEmpty(keywordId))
            //{
            //    return result;
            //}
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId(parentid));
            }
            else
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId("000000000000000000000000"));
            }


            var selectedIdList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<IW2S_BaiduCommendDto> allKeywords = new List<IW2S_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(parentid));
                allKeywords = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_BaiduCommendDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                        CommendKeyword = x.BaiduCommend,
                        drag = true
                    }
                    ).ToList();
            }
            else if (string.IsNullOrEmpty(parentid) || parentid == "000000000000000000000000")
            {
                allKeywords = GetBaiduCommendKeyword(usr_id, null, projectId);
            }

            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).ToList();

            List<IW2S_BaiduCommendDto> curSelectedList = new List<IW2S_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                curSelectedList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_BaiduCommendDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                        CommendKeyword = x.BaiduCommend,
                        drag = true
                    }
                    ).ToList();
            }

            result.Selected = curSelectedList;
            result.UnSelected = list;
            return result;
        }

        [HttpGet]
        public ResultDto DelKeywordCategory(string user_id, string categoryId)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(categoryId))
            {
                return result;
            }
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, new ObjectId(categoryId));
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();

            var prjId = categoryCol.Find(builder.Eq(x => x._id, new ObjectId(categoryId))).Project(x => x.ProjectId).FirstOrDefault();

            categoryCol.DeleteOne(builder.Eq(x => x._id, new ObjectId(categoryId)));

            var groupBuilder = Builders<IW2S_KeywordGroup>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.CommendCategoryId, new ObjectId(categoryId));
            var groupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
            groupCol.DeleteMany(groupFilter);

            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            if (TaskList.Count > 0)
            {
                categoryCol.DeleteMany(builder.In(x => x._id, TaskList));
                groupFilter = groupBuilder.In(x => x.CommendCategoryId, TaskList);

                groupCol.DeleteMany(groupFilter);
                foreach (ObjectId delId in TaskList)
                {
                    RecurseDelCategory(delId);
                }
            }
            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = prjId,
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;

            return result;
        }
        private void RecurseDelCategory(ObjectId categoryId)
        {

            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, categoryId);
            var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();

            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            categoryCol.DeleteMany(builder.In(x => x._id, TaskList));

            var groupBuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
            var groupFilter = groupBuilder.In(x => x.CommendCategoryId, TaskList);

            groupCol.DeleteMany(groupFilter);
            if (TaskList.Count == 0)
                return;
            foreach (ObjectId delId in TaskList)
            {
                RecurseDelCategory(delId);
            }

        }
        [HttpGet]
        public GroupTreeDto GetAllGroupTree(string usr_id, string projectId)
        {
            GroupTreeDto result = new GroupTreeDto();

            result.name = "根节点";
            result._id = "11";
            result.children = new List<GroupTreeDto>();

            //GroupTreeDto child1 = new GroupTreeDto();
            //child1._id = "22";
            //child1.name = "直搜";
            //result.children.Add(child1);

            //GroupTreeDto child2 = new GroupTreeDto();
            //child2._id = "33";
            //child2.name = "百度热词";
            //result.children.Add(child2);

            //GetCategoryTree(usr_id, new ObjectId("000000000000000000000000"), child1, 1);
            GetCategoryTree(usr_id, projectId, new ObjectId("000000000000000000000000"), result, 2);

            return result;
        }

        private void GetHotCategoryTree(string usr_id, string projectId, ObjectId parentId, GroupTreeDto parent, int groupType)
        {
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.BaiduCommend
            }).ToList();

            parent.children.AddRange(keywordList);

            if (TaskList.Count == 0)
                return;

            foreach (var treedata in TaskList)
            {
                GetCategoryTree(usr_id, projectId, new ObjectId(treedata._id), treedata, 1);
                parent.children.Add(treedata);
            }


        }

        private void GetCategoryTree(string usr_id, string projectId, ObjectId parentId, GroupTreeDto parent, int groupType)
        {
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.BaiduCommend
            }).ToList();

            parent.children.AddRange(keywordList);

            if (TaskList.Count == 0)
                return;

            foreach (var treedata in TaskList)
            {
                GetCategoryTree(usr_id, projectId, new ObjectId(treedata._id), treedata, 1);
                parent.children.Add(treedata);
            }


        }

        /// <summary>
        /// 获组分组树，返回json文件路径
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public jsonFileUrlDto GetAllGroupTreeUrl(string usr_id, string projectId)
        {
            //获取该项目下所有分组信息
            List<GroupTree2Dto> list = new List<GroupTree2Dto>();
            GroupTree2Dto result = new GroupTree2Dto();
            //获取该项目名称
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 
            var task = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter).Project(x => new IW2S_ProjectDto { Name = x.Name }).FirstOrDefault();
            result.name = task.Name;
            //根目录ID默认为"000000000000000000000000"
            result.id = "000000000000000000000000";
            result.pId = "0";
            result.isNode = true;
            list.Add(result);
            var listGT = GetCategoryTree3(usr_id, projectId, new ObjectId("000000000000000000000000"), list);

            //将分组信息转为树形
            GroupTree3Dto GroupTree = new GroupTree3Dto();
            GroupTree = GetCategoryTreeNode("000000000000000000000000", list);

            //输出到json格式文件中
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();

                string value = Serializer.Serialize(GroupTree);
                //删除无用字符串
                value = value.Replace(@",""children"":null", "");
                value = value.Replace(@",""size"":null", "");

                //写入json格式文件
                string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
                //DelFile(folder);
                //string url = CreFile(folder, value, "AllGroupTree.txt");
                string path = folder + "AllGroupTree.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                    sw.WriteLine(value);
                    sw.Close();
                }
                else
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                    sw.WriteLine(value);
                    sw.Close();
                }

                jsonFileUrlDto json = new jsonFileUrlDto();
                json.Url = "Scripts/app/data/" + "AllGroupTree.txt";
                return json;
            }
            catch (Exception ex)
            {
                return new jsonFileUrlDto { Error = "json文件生成失败！" + ex.Message };
            }
        }

        /// <summary>
        /// 将分组数组转化为分组树
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="list">分组数组</param>
        /// <returns>分组数</returns>
        private GroupTree3Dto GetCategoryTreeNode(string id, List<GroupTree2Dto> list)
        {
            GroupTree3Dto GroupTree = new GroupTree3Dto();
            //子结点集合
            List<GroupTree3Dto> children = new List<GroupTree3Dto>();

            foreach (var v in list)
            {
                if (v.id == id) { GroupTree.name = v.name; }
                if (v.pId == id)
                {

                    //判断结点类型，依结点类型不同判断是否继续递归
                    if (!v.isNode)
                    {
                        //获取有效链接数
                        var builder = Builders<IW2S_BaiduCommend>.Filter;
                        var filter = builder.Eq(x => x._id, new ObjectId(v.id));
                        var task = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).FirstOrDefault();

                        GroupTree3Dto leaf = new GroupTree3Dto();
                        leaf.name = v.name + "(" + task.ValLinkCount + ")";
                        leaf.size = Convert.ToString(100);
                        children.Add(leaf);
                    }
                    else
                    {
                        GroupTree3Dto node = new GroupTree3Dto();
                        node = GetCategoryTreeNode(v.id, list);
                        children.Add(node);
                    }
                }
            }

            GroupTree.children = children;
            return GroupTree;
        }

        /// <summary>
        /// 获取分组目录（含叶结点）数组
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="projectId"></param>
        /// <param name="parentId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<GroupTree2Dto> GetCategoryTree3(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name,
                isNode = true
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => new GroupTree2Dto
            {
                id = x.BaiduCommendId.ToString(),
                pId = x.CommendCategoryId.ToString(),
                name = x.BaiduCommend,
                isNode = false
            }).ToList();

            //判断关键词在list是否已存在，存在修改其pId，不存在则将其添加至list中
            foreach (GroupTree2Dto item in keywordList)
            {
                bool isHas = false;
                foreach (var item2 in list)
                {
                    if (item2.name == item.name & !item2.isNode)
                    {
                        item2.pId = item.pId;
                        isHas = true;
                        continue;
                    }
                }
                if (!isHas)
                {
                    list.Add(item);
                }
            }

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0) return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (GroupTree2Dto treedata in TaskList)
            {
                list.Add(treedata);
                GetCategoryTree3(usr_id, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
            }

            return list;
        }



        /// <summary>
        /// 获取该项目下所有分组
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <returns>关键词数组</returns>
        [HttpGet]
        public List<GroupTree2Dto> GetAllFenZhu(string usr_id, string projectId)
        {

            List<GroupTree2Dto> list = new List<GroupTree2Dto>();

            GroupTree2Dto result = new GroupTree2Dto();
            result.name = "所有词";
            //根目录ID默认为"000000000000000000000000"
            result.id = "000000000000000000000000";
            result.pId = "0";
            list.Add(result);


            var listGT = GetCategoryTree2(usr_id, projectId, new ObjectId("000000000000000000000000"), list);
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
        private List<GroupTree2Dto> GetCategoryTree2(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name
            }).ToList();

            ////获取当前词组内关键词
            //var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            //var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            //groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            //var keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => new GroupTree2Dto
            //{
            //    id = x._id.ToString(),
            //    pId = x.CommendCategoryId.ToString(),
            //    name = x.BaiduCommend
            //}).ToList();

            ////判断关键词在list是否已存在，存在修改其pId，不存在则将其添加至list中
            //foreach (var item in keywordList)
            //{
            //    bool isHas = false;
            //    foreach (var item2 in list)
            //    {
            //        if (item2.name == item.name)
            //        {
            //            item2.pId = item.pId;
            //            isHas = true;
            //            continue;
            //        }
            //    }
            //    if (!isHas)
            //    {
            //        GroupTree2Dto gt = new GroupTree2Dto();
            //        gt.id = item.id;
            //        gt.pId = item.pId;
            //        gt.name = item.name;
            //        list.Add(gt);
            //    }
            //}

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0)
                return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (var treedata in TaskList)
            {
                GetCategoryTree2(usr_id, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
                GroupTree2Dto gt = new GroupTree2Dto();
                gt.id = treedata.id;
                gt.pId = treedata.pId;
                gt.name = treedata.name;
                list.Add(gt);
            }

            return list;
        }

        /// <summary>
        /// 获取分类关键词
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="treeNodeId">分类结点ID</param>
        /// <returns>分类关键词数组</returns>
        [HttpGet]
        public List<GroupKeywordsDto> GetFenleiKeywords(string usr_id, string projectId, string treeNodeId, bool status)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 
            filter &= builder.Eq(x => x.IsRemoved, status);
            //判断是否为根目录，是直接查询IW2S_BaiduCommends集合，否则先获取分类关键词再查询IW2S_BaiduCommends
            if (string.Equals(treeNodeId, "000000000000000000000000"))
            {
                keywordList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Project(x => new GroupKeywordsDto
                {
                    id = x._id.ToString(),
                    name = x.CommendKeyword,
                    ValLinkCount = x.ValLinkCount,
                    BotStatus = x.BotStatus
                }).ToList();
            }
            else
            {
                //获取关键词ID列表
                var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
                var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(treeNodeId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, status);
                var TempList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();
                //获取关键词名、Bot状态和有效链接数
                var countfilter = builder.In(x => x._id, TempList) & builder.Eq(x => x.IsRemoved, status);
                var temp = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(countfilter).Project(x => new GroupKeywordsDto
                {
                    id = x._id.ToString(),
                    name = x.CommendKeyword,
                    ValLinkCount = x.ValLinkCount,
                    BotStatus = x.BotStatus
                }).ToList();
                keywordList.AddRange(temp);

            }
            return keywordList;
        }

        //监测结果页面获取多个分类关键词
        [HttpGet]
        public List<GroupKeywordsDto> GetFenleiKeywordsView(string usr_id, string projectId, string categoryId)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &

            //判断是否为根目录，是直接查询IW2S_BaiduCommends集合，否则先获取分类关键词再查询IW2S_BaiduCommends
            if (string.Equals(categoryId, "000000000000000000000000") || string.IsNullOrEmpty(categoryId))
            {
                filter &= builder.Eq(x => x.IsRemoved, false);
                keywordList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Project(x => new GroupKeywordsDto
                {
                    id = x._id.ToString(),
                    name = x.CommendKeyword,
                    ValLinkCount = x.ValLinkCount,
                    BotStatus = x.BotStatus
                }).ToList();
            }
            else
            {
                //获取关键词ID列表
                var categoryIdList = GetObjIdListFromStr(categoryId);
                var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
                var groupfilter = groupbuilder.In(x => x.CommendCategoryId, categoryIdList) & groupbuilder.Eq(x => x.IsDel, false);
                var TempList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();
                //获取关键词名、Bot状态和有效链接数
                var countfilter = builder.In(x => x._id, TempList) & builder.Eq(x => x.IsRemoved, false);
                var temp = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(countfilter).Project(x => new GroupKeywordsDto
                {
                    id = x._id.ToString(),
                    name = x.CommendKeyword,
                    ValLinkCount = x.ValLinkCount,
                    BotStatus = x.BotStatus
                }).ToList();
                keywordList.AddRange(temp);

            }
            return keywordList;
        }

        /// <summary>
        /// 排除关键词
        /// </summary>
        /// <param name="categoryId">多个ID用";"隔开</param>
        /// <param name="status">true表示排除，false表示还原</param>
        /// <returns></returns>
        [HttpGet]
        public bool SetKeywordStatus(string categoryId, bool status)
        {
            try
            {
                var idArray = categoryId.Split(';');
                var idList = idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();

                foreach (var id in idList)
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", status } } } };

                    var result = MongoDBHelper.Instance.GetIW2S_BaiduCommends().UpdateOne(new QueryDocument { { "_id", id } }, update).ModifiedCount;

                    var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", status } } } };
                    MongoDBHelper.Instance.GetIW2S_KeywordGroups().UpdateMany(new QueryDocument { { "BaiduCommendId", id } }, updategroup);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 插入推荐词的搜索过滤词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="commendKeywordId"></param>
        /// <param name="keywords">多个以;隔开</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertKeywordFilter(string user_id, string projectId, string commendKeywordId, string keywords)
        {
            ResultDto result = new ResultDto();
            //if (string.IsNullOrEmpty(commendKeywordId))
            //{
            //    return result;
            //}
            var builder = Builders<IW2S_KeywordFilter>.Filter;

            var keywordList = keywords.Split(';', '；');

            var col = MongoDBHelper.Instance.GetIW2S_KeywordFilters();
            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();

            foreach (var keyword in keywordList)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) &
                    filter &= builder.Eq(x => x.Keyword, keyword);
                    //filter &= builder.Eq(x => x.CommendKeywordId, new ObjectId(commendKeywordId));

                    filter &= builder.Eq(x => x.IsDel, false);

                    var dto = col.Find(filter).FirstOrDefault();

                    if (dto != null)
                    {
                        result.Message = "关键词‘" + keyword + "’已经存在！";
                        return result;

                    }

                    IW2S_KeywordFilter kw = new IW2S_KeywordFilter
                    {
                        _id = ObjectId.GenerateNewId(),
                        CreatedAt = DateTime.Now.AddHours(8),
                        //CommendKeywordId = new ObjectId(commendKeywordId),
                        IsDel = false,
                        Keyword = keyword,
                        ProjectId = new ObjectId(projectId)
                    };
                    kw.UsrId = usrObjId;
                    col.InsertOne(kw);

                    var linkBuilder = Builders<IW2S_level1link>.Filter;
                    var linkFilter = linkBuilder.Eq(x => x.ProjectId, new ObjectId(projectId));// linkBuilder.Eq(x => x.UsrId, new ObjectId(user_id)) & 
                    linkFilter &= linkBuilder.Ne(x => x.DataCleanStatus, (byte)1);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keyword + ".*/")));

                    var linkcol = MongoDBHelper.Instance.GetIW2S_level1links();
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", (byte)2 } } } };
                    linkcol.UpdateMany(linkFilter, update);
                }
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.FilterConfig,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 获取推荐词的搜索过滤词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="keywordId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_KeywordFilterDto> GetKeywordFilter(string user_id, string projectId, string keywordId)
        {
            var builder = Builders<IW2S_KeywordFilter>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));// & builder.Eq(x => x.CommendKeywordId, new ObjectId(keywordId));
            filter &= builder.Eq(x => x.IsDel, false);
            //if (!string.IsNullOrEmpty(user_id))
            //{
            //    filter &= builder.Eq(x => x.UsrId, new ObjectId(user_id));
            //}
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordFilters().Find(filter).SortByDescending(x => x.CreatedAt).ToList();
            List<IW2S_KeywordFilterDto> list = new List<IW2S_KeywordFilterDto>();
            foreach (var item in TaskList)
            {
                IW2S_KeywordFilterDto v = new IW2S_KeywordFilterDto();
                v._id = item._id.ToString();
                //v.CommendKeywordId = item.CommendKeywordId.ToString();
                v.Keyword = item.Keyword;

                list.Add(v);
            }
            return list;
        }

        /// <summary>
        /// 删除推荐词的搜索过滤词
        /// </summary>
        /// <param name="filterIds">以;隔开</param>
        /// <returns></returns>
        [HttpGet]
        public string DelKeywordFilter(string user_id, string filterIds)
        {
            var filterlist = filterIds.Split(';', '；');
            var col = MongoDBHelper.Instance.GetIW2S_KeywordFilters();
            foreach (var filterId in filterlist)
            {
                if (!string.IsNullOrEmpty(filterId))
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };

                    var keywordFilter = col.Find(new QueryDocument { { "_id", new ObjectId(filterId) } }).Project(x => new IW2S_KeywordFilterDto
                    {
                        ProjectId = x.ProjectId.ToString(),
                        CommendKeywordId = x.CommendKeywordId.ToString(),
                        Keyword = x.Keyword
                    }).FirstOrDefault();

                    col.UpdateOne(new QueryDocument { { "_id", new ObjectId(filterId) } }, update);

                    var linkBuilder = Builders<IW2S_level1link>.Filter;
                    //var linkFilter = linkBuilder.Eq(x => x.SearchkeywordId, keywordFilter.CommendKeywordId);
                    var linkFilter = linkBuilder.Eq(x => x.ProjectId, new ObjectId(keywordFilter.ProjectId)) & linkBuilder.Eq(x => x.DataCleanStatus, (byte)2);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")));

                    var linkcol = MongoDBHelper.Instance.GetIW2S_level1links();
                    var linkupdate = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", (byte)0 } } } };
                    linkcol.UpdateMany(linkFilter, update);
                }

            }
            if (filterlist.Length > 0)
            {
                var prjId = col.Find(new QueryDocument { { "_id", new ObjectId(filterlist[0]) } }).Project(x => x.ProjectId).FirstOrDefault();
                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = prjId,
                    ShareOperateType = (int)ShareOperateType.FilterConfig,
                    UserId = new ObjectId(user_id)
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }
            return "成功！";
        }


        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="name">项目名</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertProject(string usr_id, string name, string description)
        {
            ResultDto result = new ResultDto();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(usr_id))
            {
                return result;
            }

            var builder = Builders<IW2S_Project>.Filter;

            var col = MongoDBHelper.Instance.GetIW2S_Projects();

            var filter = builder.Eq(x => x.Name, name);
            filter &= builder.Eq(x => x.UsrId, new ObjectId(usr_id));

            filter &= builder.Eq(x => x.IsDel, false);

            var dto = col.Find(filter).FirstOrDefault();

            if (dto != null)
            {
                result.Message = "项目名‘" + name + "’已经存在！";
                return result;

            }

            IW2S_Project prj = new IW2S_Project
            {
                UsrId = new ObjectId(usr_id),
                Name = name,
                CreatedAt = DateTime.Now.AddHours(8),
                IsDel = false,
                Description = description,
            };

            col.InsertOne(prj);
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="usr_id"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_ProjectDto> GetProjects(string usr_id, int page, int pagesize)
        {
            QueryResult<IW2S_ProjectDto> result = new QueryResult<IW2S_ProjectDto>();
            if (string.IsNullOrEmpty(usr_id))
            {
                return result;
            }
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(usr_id));
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();

            List<IW2S_ProjectDto> data = new List<IW2S_ProjectDto>();       //要返回的数据
            var proObjIds = TaskList.Select(x => x._id).ToList();
            //获取项目内链接数变化数组
            var countBuilder = Builders<IW2S_ProLinksCount>.Filter;
            var countFilter = countBuilder.In(x => x.ProjectId, proObjIds);
            var countCol = MongoDBHelper.Instance.GetIW2S_ProLinksCount();
            var countQuery = countCol.Find(countFilter).SortBy(x => x.CreatedAt).Project(x => new
            {
                ProjectId = x.ProjectId.ToString(),
                LinkCount = x.LinksCount,
                CreateAt = x.CreatedAt
            }).ToList();

            //项目更新时间，查询最新的关键词
            var keywordBuilder = Builders<IW2S_BaiduCommend>.Filter;
            var keywordCol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var keywordFilter = keywordBuilder.In(x => x.ProjectId, proObjIds) & keywordBuilder.Eq(x => x.IsRemoved, false);
            var keywordQuery = keywordCol.Find(keywordFilter).SortByDescending(x => x.LastBotEndAt).Project(x => new
            {
                BaiduCommandId = x._id.ToString(),
                UpdateTime = x.LastBotEndAt,
                ProjectId = x.ProjectId.ToString(),
                ValLinkCount = x.ValLinkCount
            }).ToList();
            var keywordGroup = keywordQuery.GroupBy(x => x.ProjectId).ToList();

            foreach (var item in TaskList)
            {
                IW2S_ProjectDto v = new IW2S_ProjectDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.CreatedAt = item.CreatedAt.AddHours(-8);
                v.Description = item.Description;
                //获取项目最近更新的关键词，获取最近更新时间
                if (keywordQuery.Count > 0)
                {
                    var lastKey = keywordQuery.Find(x => x.ProjectId == v._id);
                    if (lastKey != null)
                        v.UpdateTime = lastKey.UpdateTime;
                }
                //获取当前项目有效链接数
                int valLinkCount = 0;
                var tempKeyList = keywordGroup.Find(x => x.Key.Equals(v._id));
                if (tempKeyList != null)
                {
                    foreach (var x in tempKeyList)
                    {
                        valLinkCount += x.ValLinkCount;
                    }
                }
                v.LinkCount = valLinkCount;
                //根据项目Id获取过去链接数变化数组
                v.CountList = new List<int>();
                v.CountList.Add(0);
                var countList = countQuery.Where(x => x.ProjectId.Equals(v._id)).ToList();
                if (countList.Count > 0)
                {
                    v.CountList.AddRange(countList.Select(x => x.LinkCount).ToList());
                    //判断当前链接数和最新一次的记录是否相同
                    if (countList[countList.Count - 1].LinkCount != v.LinkCount)
                    {
                        v.CountList.Add(v.LinkCount);
                        IW2S_ProLinksCount temp = new IW2S_ProLinksCount();
                        temp.CreatedAt = DateTime.Now.AddHours(8);
                        temp.ProjectId = item._id;
                        temp.LinksCount = v.LinkCount;
                        temp.UsrId = new ObjectId(usr_id);
                        countCol.InsertOne(temp);
                    }
                }
                else if (v.LinkCount != 0)
                {
                    v.CountList.Add(v.LinkCount);
                    IW2S_ProLinksCount temp = new IW2S_ProLinksCount();
                    temp.CreatedAt = DateTime.Now.AddHours(8);
                    temp.ProjectId = item._id;
                    temp.LinksCount = v.LinkCount;
                    temp.UsrId = new ObjectId(usr_id);
                    countCol.InsertOne(temp);
                }
                else
                {
                    v.CountList.Add(0);
                }
                data.Add(v);
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
        public string DelProject(string ids)
        {
            var filterlist = ids.Split(';', '；');
            List<ObjectId> obIds = new List<ObjectId>();

            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.In(x => x._id, obIds);
            foreach (var filterId in filterlist)
            {
                if (!string.IsNullOrEmpty(filterId))
                {
                    obIds.Add(new ObjectId(filterId));
                }
            }
            DateTime now = DateTime.Now.AddHours(8);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", now } } } };
            MongoDBHelper.Instance.GetIW2S_Projects().UpdateMany(filter, update);

            //获取归属项目组并修改项目组中项目数字段
            var builderGroup = Builders<Dnl_ProjectGroup>.Filter;
            var colGroup = MongoDBHelper.Instance.GetDnl_ProjectGroup();
            var categoryIdList = new List<ObjectId>();
            foreach (var proId in obIds)
            {
                var filterGroup2 = builderGroup.Eq(x => x.ProjectId, proId);
                var tempList = colGroup.Find(filterGroup2).Project(x => x.ProjectCategoryId).ToList();
                categoryIdList.AddRange(tempList);
            }
            var categoryIds = categoryIdList.GroupBy(x => x).Select(x => new { Key = x.Key, Count = x.Count() }).ToList();  //去重并统计重复数量
            var builderCate = Builders<Dnl_ProjectCategory>.Filter;
            var colCate = MongoDBHelper.Instance.GetDnl_ProjectCategory();
            foreach (var cateId in categoryIds)
            {
                var filterCate = builderCate.Eq(x => x._id, cateId.Key);
                var num = colCate.Find(filterCate).Project(x => x.ProjectCount).FirstOrDefault();
                var updateCate = new UpdateDocument { { "$set", new QueryDocument { { "ProjectCount", num - cateId.Count } } } };
                colCate.UpdateOne(filterCate, updateCate);
            }

            //删除Group表内数据
            var filterGroup = builderGroup.In(x => x.ProjectId, obIds);
            colGroup.DeleteMany(filterGroup);
            //
            return "成功！";
        }

        [HttpGet]
        public ResultDto UpdateProject(string prjId, string name, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(prjId));
            if (string.IsNullOrEmpty(name))
            {
                result.Message = "项目名不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "项目描述不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", name }, { "Description", description } } } };
            MongoDBHelper.Instance.GetIW2S_Projects().UpdateOne(filter, update);

            //更新Group表内数据
            var filterGroup = Builders<Dnl_ProjectGroup>.Filter.Eq(x => x.ProjectId, new ObjectId(prjId));
            MongoDBHelper.Instance.GetDnl_ProjectGroup().UpdateOne(filterGroup, update);
            result.IsSuccess = true;
            return result;
        }

        //获取百度推荐关键词
        [HttpGet]
        public List<IW2S_BaiduCommendDto> GetBaiduCommendKeywordPage(string user_id, string projectId, int page, int pagesize)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, false);

            //if (!string.IsNullOrEmpty(user_id))
            //{
            //    filter &= builder.Eq(x => x.UsrId, new ObjectId(user_id));
            //}
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            var TaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).SortByDescending(x => x.Times).Skip(page * pagesize).Limit(pagesize).ToList();
            List<IW2S_BaiduCommendDto> list = new List<IW2S_BaiduCommendDto>();
            List<string> keyNameList = new List<string>();
            foreach (var item in TaskList)
            {
                if (keyNameList.Contains(item.CommendKeyword)) continue;
                IW2S_BaiduCommendDto v = new IW2S_BaiduCommendDto();
                v._id = item._id.ToString();
                v.CommendKeyword = item.CommendKeyword;
                v.Keyword = item.Keyword;
                v.drag = true;
                v.Times = item.Times + 1;
                v.KeywordId = item.KeywordId.ToString();
                v.BotStatus = item.BotStatus;
                v.ProjectId = item.ProjectId.ToString();
                v.ValLinkCount = item.ValLinkCount;

                keyNameList.Add(item.CommendKeyword);
                list.Add(v);
            }
            return list;
        }



        //获取关键词 分组和链接
        [HttpGet]
        public jsonFileUrlDto GetKeywordBygroup(string user_id, string projectId)
        {
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Limit(60).ToList();
            List<KwywordLinksVO> list = new List<KwywordLinksVO>();
            List<nodes> nodesList = new List<nodes>();
            List<linksdto> linkList = new List<linksdto>();

            List<ObjectId> lobj = new List<ObjectId>();

            foreach (var item in TaskList)
            {

                lobj.Add(item._id);
                nodes v = new nodes();
                v.name = item.CommendKeyword.ToString();
                v.group = item.GroupNumber.ToString();

                nodesList.Add(v);
            }
            var builder2 = Builders<links>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter2 &= builder2.Lte(x => x.source, 59);
            filter2 &= builder2.Lte(x => x.target, 59);
            filter2 &= builder2.In(x => x.KeywordId, lobj);
            var TaskList2 = MongoDBHelper.Instance.GetIW2S_links().Find(filter2).SortByDescending(x => x.value).Limit(1000).ToList();
            foreach (var item in TaskList2)
            {
                linksdto lk = new linksdto();
                lk.source = item.source;
                lk.target = item.target;
                lk.value = item.value;
                linkList.Add(lk);
            }
            KwywordLinksVO vo = new KwywordLinksVO();
            vo.nodes = nodesList;
            vo.links = linkList;
            list.Add(vo);
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                string value = Serializer.Serialize(list);
                string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
                // DelFile(folder);
                string url = CreFile(folder, value, "miserables.txt");
                jsonFileUrlDto json = new jsonFileUrlDto();
                json.Url = "Scripts/app/data/" + url;
                return json;
            }
            catch (Exception ex)
            {
                return new jsonFileUrlDto { Error = "json文件生成失败！" + ex.Message };
            }
            //return list;
        }

        //删除临时json文件
        void DelFile(string path)
        {
            try
            {
                var files = IOHelper.GetFiles(path);
                int now = DateTime.Now.ToDateKey();
                foreach (var f in files)
                {
                    int? dk = f.SubAfter("dk_").ExInt();
                    if (dk.HasValue && (DateTime.Now - dk.Value.ToDateTime().Value).TotalDays > 1)
                        File.Delete(path + f);
                }
            }
            catch
            {
            }
        }

        //创建临时json文件
        string CreFile(string folder, string value, string tmp)
        {
            // string tmp = "dk_maps_file.txt";
            int sd = value.Length - 2;
            value = value.Substring(1, sd);
            string path = folder + tmp;
            if (File.Exists(path))
            {
                File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.WriteLine(value);
                sw.Close();
            }
            else
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.WriteLine(value);
                sw.Close();
            }
            return tmp;

        }

        [HttpGet]
        public string ExportKeywordGroup(string user_id, string projectId)
        {

            var builder = Builders<IW2S_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Eq(x => x.IsRemoved, false);
            var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
            var keywords = col.Find(filter).ToList();
            if (keywords == null || keywords.Count == 0)
            {
                return "没有要导出的数据";
            }

            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("IW2S_BaiduKeyword");
            IRow RowHead = sheet.CreateRow(0);
            RowHead.CreateCell(0).SetCellValue("_id");
            RowHead.CreateCell(1).SetCellValue("Keyword");

            int count = 0;
            foreach (var keyword in keywords)
            {
                IRow row = sheet.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.Keyword);
                count = count + 1;
            }

            var builder1 = Builders<IW2S_BaiduCommend>.Filter;
            var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder1.Eq(x => x.IsRemoved, false);
            var col1 = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var keywords1 = col1.Find(filter1).ToList();

            ISheet sheet1 = workBook.CreateSheet("IW2S_BaiduCommend");
            IRow RowHead1 = sheet1.CreateRow(0);
            RowHead1.CreateCell(0).SetCellValue("_id");
            RowHead1.CreateCell(1).SetCellValue("CommendKeyword");
            RowHead1.CreateCell(2).SetCellValue("GroupNumber");
            RowHead1.CreateCell(3).SetCellValue("Keyword");
            RowHead1.CreateCell(4).SetCellValue("KeywordId");
            RowHead1.CreateCell(5).SetCellValue("Times");

            count = 0;
            foreach (var keyword in keywords1)
            {
                IRow row = sheet1.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.CommendKeyword);
                row.CreateCell(2).SetCellValue(keyword.GroupNumber);
                row.CreateCell(3).SetCellValue(keyword.Keyword);
                row.CreateCell(4).SetCellValue(keyword.KeywordId.ToString());
                row.CreateCell(5).SetCellValue(keyword.Times);
                count = count + 1;
            }

            var builder2 = Builders<IW2S_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder2.Eq(x => x.IsDel, false);
            var col2 = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var keywords2 = col2.Find(filter2).ToList();

            ISheet sheet2 = workBook.CreateSheet("IW2S_KeywordCategory");
            IRow RowHead2 = sheet2.CreateRow(0);
            RowHead2.CreateCell(0).SetCellValue("_id");
            RowHead2.CreateCell(1).SetCellValue("GroupNumber");
            RowHead2.CreateCell(2).SetCellValue("GroupType");
            RowHead2.CreateCell(3).SetCellValue("InfriLawCode");
            RowHead2.CreateCell(4).SetCellValue("KeywordId");
            RowHead2.CreateCell(5).SetCellValue("KeywordTotal");
            RowHead2.CreateCell(6).SetCellValue("Name");
            RowHead2.CreateCell(7).SetCellValue("ParentId");
            RowHead2.CreateCell(8).SetCellValue("Weight");

            count = 0;
            foreach (var keyword in keywords2)
            {
                IRow row = sheet2.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.GroupNumber);
                row.CreateCell(2).SetCellValue(keyword.GroupType);
                row.CreateCell(3).SetCellValue(keyword.InfriLawCode.ToString());
                row.CreateCell(4).SetCellValue(keyword.KeywordId.ToString());
                row.CreateCell(5).SetCellValue(keyword.KeywordTotal);
                row.CreateCell(6).SetCellValue(keyword.Name);
                row.CreateCell(7).SetCellValue(keyword.ParentId.ToString());
                row.CreateCell(8).SetCellValue(keyword.Weight);
                count = count + 1;
            }

            var builder3 = Builders<IW2S_KeywordGroup>.Filter;
            var filter3 = builder3.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder3.Eq(x => x.IsDel, false);
            var col3 = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
            var keywords3 = col3.Find(filter3).ToList();

            ISheet sheet3 = workBook.CreateSheet("IW2S_KeywordGroup");
            IRow RowHead3 = sheet3.CreateRow(0);
            RowHead3.CreateCell(0).SetCellValue("_id");
            RowHead3.CreateCell(1).SetCellValue("BaiduCommend");
            RowHead3.CreateCell(2).SetCellValue("BaiduCommendId");
            RowHead3.CreateCell(3).SetCellValue("CommendCategoryId");
            RowHead3.CreateCell(4).SetCellValue("GroupType");
            RowHead3.CreateCell(5).SetCellValue("ParentCategoryId");

            count = 0;
            foreach (var keyword in keywords3)
            {
                IRow row = sheet3.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.BaiduCommend);
                row.CreateCell(2).SetCellValue(keyword.BaiduCommendId.ToString());
                row.CreateCell(3).SetCellValue(keyword.CommendCategoryId.ToString());
                row.CreateCell(4).SetCellValue(keyword.GroupType);
                row.CreateCell(5).SetCellValue(keyword.ParentCategoryId.ToString());
                count = count + 1;
            }


            string baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
            string filename = "词组关系" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".xls";
            string path = baseUrl + @"\ExportFiles\" + filename;
            foreach (string file in Directory.GetFiles(baseUrl + @"\ExportFiles\"))
            {
                File.Delete(file);
            }

            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                workBook.Write(file);　　//创建Excel文件。
                file.Close();
            }
            return path;
        }

        [HttpGet]
        public string ExportLevelLinks(string user_id, string projectId)
        {

            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & !builder.Ne(x => x.DataCleanStatus, (byte)2);
            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            int page = 0, pageSize = 100;
            List<IW2S_level1link> keywords = new List<IW2S_level1link>();
            while (page >= 0)
            {
                var keywords1 = col.Find(filter).Skip((page) * pageSize).Limit(pageSize).ToList();
                if (keywords1 == null || keywords1.Count == 0)
                {
                    break;
                }
                keywords.AddRange(keywords1);
                page++;
            }
            if (keywords == null || keywords.Count == 0)
            {
                return "没有要导出的数据";
            }

            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("IW2S_level1link");
            IRow RowHead = sheet.CreateRow(0);
            RowHead.CreateCell(0).SetCellValue("_id");
            RowHead.CreateCell(1).SetCellValue("Abstract");
            RowHead.CreateCell(2).SetCellValue("AppType");
            RowHead.CreateCell(3).SetCellValue("BaiduVStar");
            RowHead.CreateCell(4).SetCellValue("BizId");
            RowHead.CreateCell(5).SetCellValue("CreatedAt");
            RowHead.CreateCell(6).SetCellValue("DataCleanStatus");
            RowHead.CreateCell(7).SetCellValue("Description");
            RowHead.CreateCell(8).SetCellValue("Domain");
            //RowHead.CreateCell(9).SetCellValue("Html");
            RowHead.CreateCell(9).SetCellValue("InfriLawCode");
            RowHead.CreateCell(10).SetCellValue("IsMarket");
            RowHead.CreateCell(11).SetCellValue("Keywords");
            RowHead.CreateCell(12).SetCellValue("LinkUrl");
            RowHead.CreateCell(13).SetCellValue("PublishTime");
            RowHead.CreateCell(14).SetCellValue("Score");
            RowHead.CreateCell(15).SetCellValue("SearchkeywordId");
            RowHead.CreateCell(16).SetCellValue("Title");
            RowHead.CreateCell(17).SetCellValue("TopDomain");

            int count = 0;
            foreach (var keyword in keywords)
            {
                IRow row = sheet.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.Abstract);
                row.CreateCell(2).SetCellValue(keyword.AppType);
                row.CreateCell(3).SetCellValue(keyword.BaiduVStar);
                row.CreateCell(4).SetCellValue(keyword.BizId.ToString());
                row.CreateCell(5).SetCellValue(keyword.CreatedAt);
                row.CreateCell(6).SetCellValue(keyword.DataCleanStatus ?? 0);
                row.CreateCell(7).SetCellValue(keyword.Description);
                row.CreateCell(8).SetCellValue(keyword.Domain);
                //row.CreateCell(9).SetCellValue(keyword.Html);
                row.CreateCell(9).SetCellValue(keyword.InfriLawCode.ToString());
                row.CreateCell(10).SetCellValue(keyword.IsMarket);
                row.CreateCell(11).SetCellValue(keyword.Keywords);
                row.CreateCell(12).SetCellValue(keyword.LinkUrl);
                row.CreateCell(13).SetCellValue(keyword.PublishTime);
                row.CreateCell(14).SetCellValue(keyword.Score ?? 0);
                row.CreateCell(15).SetCellValue(keyword.SearchkeywordId.ToString());
                row.CreateCell(16).SetCellValue(keyword.Title);
                row.CreateCell(17).SetCellValue(keyword.TopDomain);
                count = count + 1;
            }




            string baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
            string filename = "监测结果" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".xls";
            string path = baseUrl + @"\ExportFiles\" + filename;
            foreach (string file in Directory.GetFiles(baseUrl + @"\ExportFiles\"))
            {
                File.Delete(file);
            }

            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                workBook.Write(file);　　//创建Excel文件。
                file.Close();
            }
            return path;
        }

        [HttpGet]
        public string ImportKeywordGroup(string user_id, string projectId, string excelFilePath)
        {
            string message = "";
            string Path = System.Web.Hosting.HostingEnvironment.MapPath("~/ExportFiles/ImportExcel/"); //当前路径 
            List<IW2S_BaiduKeyword> keywords = new List<IW2S_BaiduKeyword>();
            List<IW2S_BaiduCommend> commendKeywords = new List<IW2S_BaiduCommend>();
            List<IW2S_KeywordCategory> keywordCategories = new List<IW2S_KeywordCategory>();
            List<IW2S_KeywordGroup> groups = new List<IW2S_KeywordGroup>();

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            try
            {

                using (Stream stream = new FileStream(Path + excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(stream);
                    HSSFSheet sheet = workbook.GetSheetAt(0) as HSSFSheet;
                    //Execel第一行是标题，不是要导入数据库的数据
                    Dictionary<string, string> dickeywords = new Dictionary<string, string>();
                    Dictionary<string, string> diccommendkeywords = new Dictionary<string, string>();
                    Dictionary<string, string> diccategorys = new Dictionary<string, string>();
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        HSSFRow row = sheet.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_BaiduKeyword keyword = new IW2S_BaiduKeyword();
                        keyword._id = ObjectId.GenerateNewId();

                        dickeywords.Add(row.GetCell(0).StringCellValue, keyword._id.ToString());

                        keyword.CreatedAt = DateTime.Now.AddHours(8);
                        keyword.IsRemoved = false;
                        keyword.Keyword = row.GetCell(1).StringCellValue;
                        keyword.ProjectId = new ObjectId(projectId);
                        keyword.UsrId = usrObjId;

                        keywords.Add(keyword);
                    }

                    HSSFSheet sheet1 = workbook.GetSheetAt(1) as HSSFSheet;

                    for (int i = 1; i <= sheet1.LastRowNum; i++)
                    {
                        HSSFRow row = sheet1.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_BaiduCommend commendKeyword = new IW2S_BaiduCommend();

                        commendKeyword._id = ObjectId.GenerateNewId();

                        diccommendkeywords.Add(row.GetCell(0).StringCellValue, commendKeyword._id.ToString());

                        commendKeyword.CreatedAt = DateTime.Now.AddHours(8);
                        commendKeyword.IsRemoved = false;
                        commendKeyword.Keyword = row.GetCell(3).StringCellValue;
                        commendKeyword.Times = (int)row.GetCell(5).NumericCellValue;
                        commendKeyword.CommendKeyword = row.GetCell(1).StringCellValue;
                        if (dickeywords.ContainsKey(row.GetCell(4).StringCellValue))
                        {
                            commendKeyword.KeywordId = new ObjectId(dickeywords[row.GetCell(4).StringCellValue]);
                        }
                        else
                        {
                            commendKeyword.KeywordId = new ObjectId(row.GetCell(4).StringCellValue);
                        }
                        commendKeyword.GroupNumber = (int)row.GetCell(2).NumericCellValue;
                        commendKeyword.Keyword = row.GetCell(3).StringCellValue;
                        commendKeyword.ProjectId = new ObjectId(projectId);
                        commendKeyword.UsrId = usrObjId;

                        commendKeywords.Add(commendKeyword);
                    }
                    HSSFSheet sheet2 = workbook.GetSheetAt(2) as HSSFSheet;

                    for (int i = 1; i <= sheet2.LastRowNum; i++)
                    {
                        HSSFRow row = sheet2.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(6) == null || string.IsNullOrEmpty(row.GetCell(6).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_KeywordCategory keywordCategory = new IW2S_KeywordCategory();

                        keywordCategory._id = ObjectId.GenerateNewId();
                        diccategorys.Add(row.GetCell(0).StringCellValue, keywordCategory._id.ToString());

                        keywordCategory.IsDel = false;
                        keywordCategory.GroupNumber = (int)row.GetCell(1).NumericCellValue;
                        keywordCategory.GroupType = (int)row.GetCell(2).NumericCellValue;
                        keywordCategory.InfriLawCode = new ObjectId(row.GetCell(3).StringCellValue);

                        if (diccommendkeywords.ContainsKey(row.GetCell(4).StringCellValue))
                        {
                            keywordCategory.KeywordId = new ObjectId(diccommendkeywords[row.GetCell(4).StringCellValue]);
                        }
                        else
                        {
                            keywordCategory.KeywordId = new ObjectId(row.GetCell(4).StringCellValue);
                        }

                        keywordCategory.KeywordTotal = (int)row.GetCell(5).NumericCellValue;
                        keywordCategory.Name = row.GetCell(6).StringCellValue;
                        keywordCategory.ParentId = new ObjectId(row.GetCell(7).StringCellValue);
                        keywordCategory.Name = row.GetCell(6).StringCellValue;
                        keywordCategory.ProjectId = new ObjectId(projectId);
                        keywordCategory.UsrId = usrObjId;

                        keywordCategories.Add(keywordCategory);
                    }
                    foreach (var keywordCategorie in keywordCategories)
                    {
                        keywordCategorie.ParentId = diccategorys.ContainsKey(keywordCategorie.ParentId.ToString()) ? new ObjectId(diccategorys[keywordCategorie.ParentId.ToString()]) : keywordCategorie.ParentId;
                    }

                    HSSFSheet sheet3 = workbook.GetSheetAt(3) as HSSFSheet;

                    for (int i = 1; i <= sheet3.LastRowNum; i++)
                    {
                        HSSFRow row = sheet3.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_KeywordGroup group = new IW2S_KeywordGroup();
                        group._id = ObjectId.GenerateNewId();
                        group.IsDel = false;
                        group.BaiduCommend = row.GetCell(1).StringCellValue;

                        if (diccommendkeywords.ContainsKey(row.GetCell(2).StringCellValue))
                        {
                            group.BaiduCommendId = new ObjectId(diccommendkeywords[row.GetCell(2).StringCellValue]);
                        }
                        else
                        {
                            group.BaiduCommendId = new ObjectId(row.GetCell(2).StringCellValue);
                        }

                        if (diccategorys.ContainsKey(row.GetCell(3).StringCellValue))
                        {
                            group.CommendCategoryId = new ObjectId(diccategorys[row.GetCell(3).StringCellValue]);
                        }
                        else
                        {
                            group.CommendCategoryId = new ObjectId(row.GetCell(3).StringCellValue);
                        }
                        if (diccategorys.ContainsKey(row.GetCell(5).StringCellValue))
                        {
                            group.ParentCategoryId = new ObjectId(diccategorys[row.GetCell(5).StringCellValue]);
                        }
                        else
                        {
                            group.ParentCategoryId = new ObjectId(row.GetCell(5).StringCellValue);
                        }

                        group.GroupType = (int)row.GetCell(4).NumericCellValue;

                        group.ProjectId = new ObjectId(projectId);
                        group.UsrId = usrObjId;

                        groups.Add(group);
                    }
                }


            }
            catch (Exception)
            {
                message = "导入数据失败";
            }
            var builder = Builders<IW2S_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col = MongoDBHelper.Instance.GetIW2S_BaiduKeywords();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", true } } } };
            foreach (var keyword in keywords)
            {
                //col.UpdateMany(filter & builder.Eq(x => x.Keyword, keyword.Keyword), update);
                col.DeleteMany(filter & builder.Eq(x => x.Keyword, keyword.Keyword));
                col.InsertOne(keyword);
            }

            var builder1 = Builders<IW2S_BaiduCommend>.Filter;
            var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col1 = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            foreach (var commendKeyword in commendKeywords)
            {
                //col1.UpdateMany(filter1 & builder1.Eq(x => x.CommendKeyword, commendKeyword.CommendKeyword) & builder1.Eq(x=>x.KeywordId,commendKeyword.KeywordId), update);
                col1.DeleteMany(filter1 & builder1.Eq(x => x.CommendKeyword, commendKeyword.CommendKeyword) & builder1.Eq(x => x.KeywordId, commendKeyword.KeywordId));
                col1.InsertOne(commendKeyword);
            }

            var builder2 = Builders<IW2S_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col2 = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            foreach (var keywordCategory in keywordCategories)
            {
                //col2.UpdateMany(filter2 & builder2.Eq(x => x.Name, keywordCategory.Name), update);
                col2.DeleteMany(filter2 & builder2.Eq(x => x.Name, keywordCategory.Name));
                col2.InsertOne(keywordCategory);
            }

            var builder3 = Builders<IW2S_KeywordGroup>.Filter;
            var filter3 = builder3.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col3 = MongoDBHelper.Instance.GetIW2S_KeywordGroups();

            foreach (var group in groups)
            {
                //col3.UpdateMany(filter3 & builder3.Eq(x => x.BaiduCommendId, group.BaiduCommendId)&builder3.Eq(x=>x.CommendCategoryId,group.CommendCategoryId), update);
                col3.DeleteMany(filter3 & builder3.Eq(x => x.BaiduCommendId, group.BaiduCommendId) & builder3.Eq(x => x.CommendCategoryId, group.CommendCategoryId));
                col3.InsertOne(group);
            }

            message = "导入成功";

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.ImportKeywordGroup,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            return message;
        }

        [HttpGet]
        public string ImportLevelLinks(string user_id, string projectId, string excelFilePath)
        {
            string message = "";
            string Path = System.Web.Hosting.HostingEnvironment.MapPath("~/ExportFiles/ImportExcel/"); //当前路径 
            List<IW2S_level1link> keywords = new List<IW2S_level1link>();
            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            try
            {

                using (Stream stream = new FileStream(Path + excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(stream);
                    HSSFSheet sheet = workbook.GetSheetAt(0) as HSSFSheet;
                    //Execel第一行是标题，不是要导入数据库的数据

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        HSSFRow row = sheet.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(13) == null || string.IsNullOrEmpty(row.GetCell(13).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_level1link keyword = new IW2S_level1link();
                        keyword._id = new ObjectId(row.GetCell(0).StringCellValue);
                        keyword.Abstract = row.GetCell(1).StringCellValue;
                        keyword.AppType = (byte)row.GetCell(2).NumericCellValue;
                        keyword.BaiduVStar = (int)row.GetCell(3).NumericCellValue;
                        keyword.BizId = Guid.Parse(row.GetCell(4).StringCellValue);
                        if (!string.IsNullOrEmpty(row.GetCell(5).StringCellValue))
                        {
                            keyword.CreatedAt = row.GetCell(5).DateCellValue;
                        }
                        keyword.DataCleanStatus = (byte)0;
                        keyword.Description = row.GetCell(7).StringCellValue;
                        keyword.Domain = row.GetCell(8).StringCellValue;
                        //keyword.Html = row.GetCell(9).StringCellValue;
                        keyword.InfriLawCode = new ObjectId(row.GetCell(9).StringCellValue);
                        keyword.IsMarket = row.GetCell(10).BooleanCellValue;
                        keyword.Keywords = row.GetCell(11).StringCellValue;
                        keyword.LinkUrl = row.GetCell(12).StringCellValue;
                        keyword.PublishTime = row.GetCell(13).StringCellValue;
                        keyword.Score = (int)row.GetCell(14).NumericCellValue;
                        keyword.SearchkeywordId = row.GetCell(15).StringCellValue;
                        keyword.Title = row.GetCell(16).StringCellValue;
                        keyword.TopDomain = row.GetCell(17).StringCellValue;
                        keyword.ProjectId = new ObjectId(projectId);
                        keyword.UsrId = usrObjId;

                        keywords.Add(keyword);
                    }
                }
            }
            catch (Exception)
            {
                message = "导入数据失败";
            }
            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Ne(x => x.DataCleanStatus, (byte)2);
            var col = MongoDBHelper.Instance.GetIW2S_level1links();

            var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", (byte)2 } } } };
            foreach (var keyword in keywords)
            {
                col.UpdateMany(filter & builder.Eq(x => x.LinkUrl, keyword.LinkUrl) & builder.Eq(x => x.SearchkeywordId, keyword.SearchkeywordId), update);
                col.InsertOne(keyword);
            }

            message = "导入成功";

            return message;
        }

        [HttpGet]
        public string ExportProject(string projectId)
        {
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(projectId));
            var col = MongoDBHelper.Instance.GetIW2S_Projects();

            var keyword = col.Find(filter).FirstOrDefault();

            if (keyword == null)
            {
                return "没有要导出的数据";
            }

            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("IW2S_Project");
            IRow RowHead = sheet.CreateRow(0);
            RowHead.CreateCell(0).SetCellValue("_id");
            RowHead.CreateCell(1).SetCellValue("Description");
            RowHead.CreateCell(2).SetCellValue("Name");

            int count = 0;

            IRow row = sheet.CreateRow(count + 1);
            row.CreateCell(0).SetCellValue(keyword._id.ToString());
            row.CreateCell(1).SetCellValue(keyword.Description);
            row.CreateCell(2).SetCellValue(keyword.Name);

            string baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
            string filename = "项目描述" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".xls";
            string path = baseUrl + @"\ExportFiles\" + filename;
            foreach (string file in Directory.GetFiles(baseUrl + @"\ExportFiles\"))
            {
                File.Delete(file);
            }

            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                workBook.Write(file);　　//创建Excel文件。
                file.Close();
            }
            return path;
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
                cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
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
            var groupBuilder = Builders<IW2S_KeywordGroup>.Filter;
            var builder = Builders<IW2S_level1link>.Filter;

            //获取关键词列表
            List<string> keywordList = new List<string>();      //关键词列表
            var keyToCate = new Dictionary<string, string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (cateIsRoot)
            {
                //直接获取项目中所有关键词Id
                var builderPro = Builders<IW2S_BaiduCommend>.Filter;
                var filterPro = builderPro.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderPro.Eq(x => x.IsRemoved, false);
                keywordList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filterPro).Project(x => x._id.ToString()).ToList();
            }
            else
            {
                //从分组中获取所有关键词Id
                var groupFilter = groupBuilder.In(x => x.CommendCategoryId, cateIds) & groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId)) & groupBuilder.Eq(x => x.IsDel, false);
                var groupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
                var TaskList = groupCol.Find(groupFilter).Project(x => new
                {
                    BaiduCommendId = x.BaiduCommendId.ToString(),
                    CategoryId = x.CommendCategoryId.ToString()
                }).ToList();
                foreach (var x in TaskList)
                {
                    if (!keywordList.Contains(x.BaiduCommendId) && !keyToCate.ContainsKey(x.BaiduCommendId))
                    {
                        keywordList.Add(x.BaiduCommendId);
                        keyToCate.Add(x.BaiduCommendId, x.CategoryId);
                    }
                }
            }

            //获取发表时间
            var filter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");
            var queryDatas = MongoDBHelper.Instance.GetIW2S_level1links().Find(filter).Project(x => new
            {
                PublishTime = x.PublishTime,
                BaiduCommendId = x.SearchkeywordId,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            //获取包含分组ID的发布时间信息
            List<LinkStatus> linkList = new List<LinkStatus>();
            foreach (var x in queryDatas)
            {

                DateTime tmpDt = new DateTime();
                DateTime.TryParse(x.PublishTime, out tmpDt);
                int i = keywordList.IndexOf(x.BaiduCommendId);
                while (i != -1)
                {
                    LinkStatus v = new LinkStatus();
                    v.PublishTime = tmpDt;
                    if (!cateIsRoot)
                    {
                        v.CategoryId = keyToCate[x.BaiduCommendId];
                    }
                    else
                    {
                        //无分组以空定义为所有分组
                        v.CategoryId = "000000000000000000000000";
                    }
                    v.Title = x.Title;
                    v.Description = x.Description;
                    linkList.Add(v);
                    i = keywordList.IndexOf(x.BaiduCommendId, i + 1);
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
                var namefilter = Builders<IW2S_KeywordCategory>.Filter.In(x => x._id, cateIds);
                var nameList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(namefilter).Project(x => new
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

        //命中关键词域名分布图
        [HttpGet]
        public List<DomainStatisDto> GetDomainStatis(string categoryId, string prjId)
        {
            List<DomainStatisDto> result = new List<DomainStatisDto>();
            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(prjId))
            {
                return result;
            }

            var keyIds = new List<string>();
            var usrId = ObjectId.Empty;
            var groupBuilder = Builders<IW2S_KeywordGroup>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));

            if (!string.IsNullOrEmpty(categoryId))
            {
                var cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
                //判断是否分组
                if (cateIds.Count == 1 && cateIds[0].Equals(ObjectId.Empty))
                {
                    var builderKey = Builders<IW2S_BaiduCommend>.Filter;
                    var filterKey = builderKey.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderKey.Eq(x => x.IsRemoved, false);
                    keyIds = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filterKey).Project(x => x._id.ToString()).ToList();
                    usrId = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filterKey).Project(x => x.UsrId).FirstOrDefault();
                }
                else
                {
                    //去除根结点
                    cateIds.Remove(ObjectId.Empty);
                    groupFilter &= groupBuilder.In(x => x.CommendCategoryId, cateIds);
                    var groupCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
                    keyIds = groupCol.Find(groupFilter).Project(x => x.BaiduCommendId).ToList().Select(x => x.ToString()).ToList();
                    usrId = groupCol.Find(groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId))).Project(x => x.UsrId).FirstOrDefault();
                }
            }



            var builder = Builders<IW2S_level1link>.Filter;
            var filter = builder.In(x => x.SearchkeywordId, keyIds);
            var col = MongoDBHelper.Instance.GetIW2S_level1links();
            var agreresult = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, keyIds))
                .Group(new BsonDocument { { "_id", "$Domain" }, { "Count", new BsonDocument("$sum", 1) }, { "RankTotal", new BsonDocument("$sum", "$Rank") }, { "KeywordTotal", new BsonDocument("$sum", 1) } })
                        .ToListAsync()
                        .Result;
            var agreresult2 = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, keyIds))
                .Group(new BsonDocument { { "_id", new BsonDocument { { "Domain", "$Domain" }, { "SearchkeywordId", "$SearchkeywordId" } } }, { "Count", new BsonDocument("$sum", 1) } })
                        .ToListAsync()
                        .Result;
            List<DomainKeywordDto> dks = new List<DomainKeywordDto>();
            foreach (var age in agreresult2)
            {
                DomainKeywordDto dk = new DomainKeywordDto();
                dk.Domain = age["_id"]["Domain"].ToString();
                dk.KeywordId = age["_id"]["SearchkeywordId"].ToString();
                dks.Add(dk);
            }
            var dkgroups = dks.GroupBy(x => x.Domain).Select(x => new { Key = x.Key, Count = x.Count() }).ToList();
            Dictionary<string, int> domainKeywords = new Dictionary<string, int>();
            foreach (var key in dkgroups)
            {
                if (!domainKeywords.ContainsKey(key.Key))
                {
                    domainKeywords.Add(key.Key, key.Count);
                }
            }

            var agreresult3 = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, keyIds) & builder.Ne(x => x.PublishTime, ""))
                .Group(new BsonDocument { { "_id", "$Domain" }, { "Count", new BsonDocument("$sum", 1) } })
                        .ToListAsync()
                        .Result;
            Dictionary<string, int> domainCounts = new Dictionary<string, int>();
            foreach (var ag in agreresult3)
            {
                domainCounts.Add(ag["_id"].ToString(), ag["Count"].ToInt32());
            }

            foreach (var ag in agreresult)
            {
                DomainStatisDto ds = new DomainStatisDto();
                ds.Domain = ag["_id"].ToString();
                ds.Count = ag["Count"].ToInt32();
                if (domainKeywords.ContainsKey(ds.Domain))
                {
                    ds.KeywordTotal = domainKeywords[ds.Domain];
                }
                if (domainCounts.ContainsKey(ds.Domain))
                {
                    ds.PublishRatio = domainCounts[ds.Domain] / (float)ds.Count * 100 + "%";
                }
                else
                {
                    ds.PublishRatio = "0%";
                }
                ds.RankTotal = ag["RankTotal"].ToInt32() / ds.Count;
                result.Add(ds);
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
        [HttpGet]
        public ResultDto SetPrjAnalysisItem(string usr_id, string prjId, string anaItemId)
        {
            ResultDto result = new ResultDto();
            if (string.IsNullOrEmpty(prjId) || string.IsNullOrEmpty(anaItemId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_PrjAnalysisItem>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(prjId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 

            var col = MongoDBHelper.Instance.GetIW2S_PrjAnalysisItems();
            col.DeleteMany(filter);

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(prjId))).Project(x => x.UsrId).FirstOrDefault();

            var prjItem = new IW2S_PrjAnalysisItem();
            prjItem.AnalysisItem = new ObjectId(anaItemId);
            prjItem.ProjectId = new ObjectId(prjId);
            prjItem.UsrId = usrObjId;
            col.InsertOne(prjItem);

            result.Message = "设置项目的分析指项成功";

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(prjId),
                ShareOperateType = (int)ShareOperateType.ManageAnalysisItem,
                UserId = new ObjectId(usr_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }
        [HttpGet]
        public IW2S_AnalysisItemDto GetPrjAnalysisItem(string usr_id, string prjId)
        {
            IW2S_AnalysisItemDto result = new IW2S_AnalysisItemDto();
            if (string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var builder = Builders<IW2S_PrjAnalysisItem>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(prjId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 

            var col = MongoDBHelper.Instance.GetIW2S_PrjAnalysisItems();
            var itemId = col.Find(filter).Project(x => x.AnalysisItem).FirstOrDefault();

            if (itemId == ObjectId.Empty)
            {
                return result;
            }
            var aitembuilder = Builders<IW2S_AnalysisItem>.Filter;
            var aitemfilter = aitembuilder.Eq(x => x._id, itemId);

            var aitemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItems();
            var anaItem = aitemcol.Find(aitemfilter).FirstOrDefault();

            if (anaItem == null)
            {
                return result;
            }
            result._id = anaItem._id.ToString();
            result.CreatedAt = anaItem.CreatedAt;
            result.IsDefault = anaItem.IsDefault;
            result.Name = anaItem.Name;
            result.ItemValues = new List<IW2S_AnalysisItemValueDto>();

            var itembuilder = Builders<IW2S_AnalysisItemValue>.Filter;
            var itemfilter = itembuilder.Eq(x => x.IW2S_AnalysisItem, anaItem._id);
            var itemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues();

            var usrItemValues = itemcol.Find(itemfilter).ToList();
            foreach (var usrItemValue in usrItemValues)
            {
                IW2S_AnalysisItemValueDto item = new IW2S_AnalysisItemValueDto();
                item._id = usrItemValue._id.ToString();
                item.Name = usrItemValue.Name;
                item.SeqNo = usrItemValue.SeqNo;

                result.ItemValues.Add(item);
            }
            return result;
        }

        [HttpGet]
        public List<IW2S_AnalysisItemDto> GetAnalysisItem(string prj_id)
        {
            List<IW2S_AnalysisItemDto> results = new List<IW2S_AnalysisItemDto>();
            var builder = Builders<IW2S_AnalysisItem>.Filter;

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(prj_id))).Project(x => x.UsrId).FirstOrDefault();

            var filter = builder.Eq(x => x.UsrId, usrObjId) & builder.Eq(x => x.IsRemoved, false);

            var col = MongoDBHelper.Instance.GetIW2S_AnalysisItems();

            var usrItems = col.Find(filter).ToList();
            filter = builder.Eq(x => x.UsrId, new ObjectId("000000000000000000000000")) & builder.Eq(x => x.IsRemoved, false);
            usrItems.AddRange(col.Find(filter).ToList());

            if (usrItems != null)
            {
                foreach (var anaItem in usrItems)
                {
                    IW2S_AnalysisItemDto result = new IW2S_AnalysisItemDto();
                    result._id = anaItem._id.ToString();
                    result.CreatedAt = anaItem.CreatedAt;
                    result.IsDefault = anaItem.IsDefault;
                    result.Name = anaItem.Name;
                    result.UsrId = anaItem.UsrId.ToString();
                    result.ItemValues = new List<IW2S_AnalysisItemValueDto>();

                    var itembuilder = Builders<IW2S_AnalysisItemValue>.Filter;
                    var itemfilter = itembuilder.Eq(x => x.IW2S_AnalysisItem, anaItem._id);
                    var itemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues();

                    var usrItemValues = itemcol.Find(itemfilter).ToList();
                    foreach (var usrItemValue in usrItemValues)
                    {
                        IW2S_AnalysisItemValueDto item = new IW2S_AnalysisItemValueDto();
                        item._id = usrItemValue._id.ToString();
                        item.Name = usrItemValue.Name;
                        item.SeqNo = usrItemValue.SeqNo;

                        result.ItemValues.Add(item);
                    }
                    results.Add(result);
                }
            }
            return results;
        }

        [HttpPost]
        public ResultDto InsertAnalysisItem(IW2S_AnalysisItemDto anaItem)
        {
            ResultDto result = new ResultDto();

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(anaItem.ProjectId))).Project(x => x.UsrId).FirstOrDefault();
            var builder = Builders<IW2S_AnalysisItem>.Filter;
            var filter = builder.Eq(x => x.UsrId, usrObjId) & builder.Eq(x => x.IsRemoved, false) & builder.Eq(x => x.Name, anaItem.Name);
            var col = MongoDBHelper.Instance.GetIW2S_AnalysisItems();

            var itembuilder = Builders<IW2S_AnalysisItemValue>.Filter;
            var itemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues();

            if (!string.IsNullOrEmpty(anaItem._id))
            {
                col.DeleteMany(filter);
                itemcol.DeleteMany(itembuilder.Eq(x => x.IW2S_AnalysisItem, new ObjectId(anaItem._id)));
            }

            var usrItem = col.Find(filter).FirstOrDefault();
            if (usrItem != null)
            {
                result.Message = "该分析指项名称已经存在";
                return result;
            }
            usrItem = new IW2S_AnalysisItem();
            if (!string.IsNullOrEmpty(anaItem._id))
            {
                usrItem._id = new ObjectId(anaItem._id);
            }
            else
            {
                usrItem._id = ObjectId.GenerateNewId();
            }
            usrItem.CreatedAt = DateTime.Now.AddHours(8);
            usrItem.IsDefault = true;
            usrItem.IsRemoved = false;
            usrItem.Name = anaItem.Name;
            usrItem.UsrId = usrObjId;

            col.InsertOne(usrItem);


            foreach (var itemDto in anaItem.ItemValues)
            {
                var item = new IW2S_AnalysisItemValue();
                if (!string.IsNullOrEmpty(itemDto._id))
                {
                    item._id = new ObjectId(itemDto._id);
                }
                else
                {
                    item._id = ObjectId.GenerateNewId();
                }
                item.Name = itemDto.Name;
                item.SeqNo = itemDto.SeqNo;
                item.IW2S_AnalysisItem = usrItem._id;
                itemcol.InsertOne(item);
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(anaItem.ProjectId),
                ShareOperateType = (int)ShareOperateType.ManageAnalysisItem,
                UserId = new ObjectId(anaItem.UsrId)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.Message = "分析指项保存成功";

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public ResultDto RemoveAnalysisItem(string user_id, string anaItemId)
        {
            ResultDto result = new ResultDto();

            var builder = Builders<IW2S_AnalysisItem>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_AnalysisItems();

            var itembuilder = Builders<IW2S_AnalysisItemValue>.Filter;
            var itemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues();

            var prjbuilder = Builders<IW2S_PrjAnalysisItem>.Filter;
            var prjfilter = prjbuilder.Eq(x => x.AnalysisItem, new ObjectId(anaItemId));
            var prjcol = MongoDBHelper.Instance.GetIW2S_PrjAnalysisItems();


            if (!string.IsNullOrEmpty(anaItemId))
            {
                var filter = builder.Eq(x => x._id, new ObjectId(anaItemId));

                var prjId = col.Find(filter).Project(x => x.ProjectId).FirstOrDefault();

                col.DeleteMany(filter);
                itemcol.DeleteMany(itembuilder.Eq(x => x.IW2S_AnalysisItem, new ObjectId(anaItemId)));
                prjcol.DeleteMany(prjfilter);


                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = prjId,
                    ShareOperateType = (int)ShareOperateType.ManageAnalysisItem,
                    UserId = new ObjectId(user_id)
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }



            result.Message = "分析指项删除成功！";
            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 获取时间标题数据列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="prjId"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_Timelevel1linkDto> GetTimeLinkList(string categoryId, string prjId, string pubTime)
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

            if (string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var groupBuilder = Builders<IW2S_KeywordGroup>.Filter;
            var builder = Builders<IW2S_level1link>.Filter;

            var keywordList = new List<string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            if (!cateIsRoot)
            {
                //去除根结点
                cateIds.Remove(ObjectId.Empty);
                //获取所给节点ID下关键词ID
                var keywordFilter = groupBuilder.In(x => x.CommendCategoryId, cateIds) & groupBuilder.Eq(x => x.IsDel, false);
                keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(keywordFilter).Project(x => x.BaiduCommendId.ToString()).ToList();
            }
            else
            {
                //直接获取项目中所有关键词Id
                var builderPro = Builders<IW2S_BaiduCommend>.Filter;
                var filterPro = builderPro.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderPro.Eq(x => x.IsRemoved, false);
                keywordList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filterPro).Project(x => x._id.ToString()).ToList();
            }

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");

            var allQueryDatas = MongoDBHelper.Instance.GetIW2S_level1links().Find(allTimeFilter).Project(x => new
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
        /// <summary>
        /// 根据分类Id设置分类Id下的所有关键词的状态
        /// </summary>
        /// <param name="categoryId">多个以;隔开</param>
        /// <param name="status"></param>
        /// <param name="prjId"></param>
        /// <param name="isgroup">是否已分组</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetCommendKeywordsStatus(string user_id, string categoryId, byte? status, string prjId, bool isgroup)
        {
            ResultDto result = new ResultDto();
            var categoryBuilder = Builders<IW2S_KeywordGroup>.Filter;
            FilterDefinition<IW2S_KeywordGroup> categoryfilter = null;

            if (!string.IsNullOrEmpty(categoryId))
            {
                categoryfilter = categoryBuilder.In(x => x.CommendCategoryId, GetObjIdListFromStr(categoryId));
            }
            else if (!string.IsNullOrEmpty(prjId) && isgroup)
            {
                categoryfilter = categoryBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
            }
            var keywordBuilder = Builders<IW2S_BaiduCommend>.Filter;
            var keywordcol = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "BotStatus", 0 } } } };

            var prjObjId = ObjectId.Empty;
            if (categoryfilter != null)
            {
                var categoryCol = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
                var keywordIds = categoryCol.Find(categoryfilter).Project(x => x.BaiduCommendId).ToList();

                var keywordFilter = keywordBuilder.In(x => x._id, keywordIds);
                keywordcol.UpdateMany(keywordFilter, update);

                prjObjId = categoryCol.Find(categoryfilter).Project(x => x.ProjectId).FirstOrDefault();
            }
            else
            {
                if (!string.IsNullOrEmpty(prjId) && !isgroup)
                {
                    var keywordFilter = keywordBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
                    keywordcol.UpdateMany(keywordFilter, update);

                    prjObjId = new ObjectId(prjId);
                }
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = prjObjId,
                ShareOperateType = (int)ShareOperateType.ReSearchKeyword,
                UserId = new ObjectId(user_id)
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 保存域名分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDtoCategory SaveDomainCategory([FromBody]IW2S_DomainCategoryDto data)
        {
            ResultDtoCategory result = new ResultDtoCategory();
            var col = MongoDBHelper.Instance.GetIW2S_DomainCategorys();
            var builder = Builders<IW2S_DomainCategory>.Filter;
            if (string.IsNullOrEmpty(data.Name))
            {
                result.Message = "分类名称为空";
                return result;
            }
            else if (string.IsNullOrEmpty(data.UsrId))
            {
                result.Message = "用户Id为空";
                return result;
            }
            //var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(data.ProjectId))).Project(x => x.UsrId).FirstOrDefault();

            var filter = builder.Eq(x => x.UsrId, new ObjectId(data.UsrId)) & builder.Eq(x => x.Name, data.Name) & builder.Eq(x => x.IsDel, false);
            if (string.IsNullOrEmpty(data._id))
            {
                var r = col.Find(filter).Project(x => x._id).FirstOrDefault();
                if (r != ObjectId.Empty)
                {
                    result.Message = "分类已经存在";
                    return result;
                }
                IW2S_DomainCategory cat = new IW2S_DomainCategory();
                cat._id = ObjectId.GenerateNewId();
                result.NewId = cat._id.ToString();
                cat.IsDel = false;
                cat.Name = data.Name;

                //如果ParentsId为空，则父Id设为根目录
                if (string.IsNullOrEmpty(data.ParentId))
                {
                    cat.ParentId = new ObjectId("000000000000000000000000");
                }
                else
                {
                    cat.ParentId = new ObjectId(data.ParentId);
                }
                cat.UsrId = new ObjectId(data.UsrId);
                //cat.ParentName = data.ParentName;
                col.InsertOne(cat);
            }
            else
            {
                filter = builder.Eq(x => x._id, new ObjectId(data._id));
                col.UpdateOne(filter, new UpdateDocument { { "$set", new QueryDocument { { "Name", data.Name } } } });
            }

            //IW2S_OperateLog log = new IW2S_OperateLog
            //{
            //    CreatedAt = DateTime.Now.AddHours(8),
            //    ProjectId = data.ProjectId,
            //    ShareOperateType = (int)ShareOperateType.ReSearchKeyword,
            //    UserId = new ObjectId(user_id)
            //};
            //MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 删除域名分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelDomainCategory(string id)
        {
            ResultDto result = new ResultDto();
            RecurseDelDomainCategory(new ObjectId(id));

            result.IsSuccess = true;
            return result;
        }

        private void RecurseDelDomainCategory(ObjectId id)
        {
            var col = MongoDBHelper.Instance.GetIW2S_DomainCategorys();
            var builder = Builders<IW2S_DomainCategory>.Filter;
            var filter = builder.Eq(x => x._id, id);

            col.UpdateOne(filter, new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } });

            var builder1 = Builders<IW2S_DomainCategoryData>.Filter;
            var filter1 = builder1.Eq(x => x.DomainCategoryId, id);
            MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().DeleteMany(filter1);

            //将子目录一并删除
            var filterChildren = builder.Eq(x => x.ParentId, id);
            var cat = col.Find(filterChildren).Project(x => x._id).ToList();
            if (cat.Count > 0)
            {
                foreach (var x in cat)
                {
                    RecurseDelDomainCategory(x);
                }
            }
        }
        /// <summary>
        /// 获取所有域名分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_DomainCategoryDto> GetAllDomainCategory(string prjId)
        {
            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(prjId))).Project(x => x.UsrId).FirstOrDefault();
            List<IW2S_DomainCategoryDto> result = new List<IW2S_DomainCategoryDto>();
            var col = MongoDBHelper.Instance.GetIW2S_DomainCategorys();
            var builder = Builders<IW2S_DomainCategory>.Filter;
            var filter = builder.Eq(x => x.UsrId, usrObjId) & builder.Eq(x => x.IsDel, false);
            result = col.Find(filter).Project(x => new IW2S_DomainCategoryDto
            {
                _id = x._id.ToString(),
                Name = x.Name,
                ParentId = x.ParentId.ToString(),
                //ParentName = x.ParentName
            }).ToList();
            if (result.Count == 0)
            {
                filter = builder.Eq(x => x.UsrId, ObjectId.Empty) & builder.Eq(x => x.IsDel, false);
                result = col.Find(filter).Project(x => new IW2S_DomainCategoryDto
                {
                    _id = x._id.ToString(),
                    Name = x.Name,
                    ParentId = x.ParentId.ToString(),
                    //ParentName = x.ParentName
                }).ToList();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var builderNum = Builders<IW2S_DomainCategoryData>.Filter;
                var filterNum = builderNum.Eq(x => x.DomainCategoryId, new ObjectId(result[i]._id));
                result[i].Num = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(filterNum).Count();
            }
            return result;
        }

        /// <summary>
        /// 设置域名的分类
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="categoryId"></param>
        /// <param name="usrId"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto SaveDomainCategoryData([FromBody]List<IW2S_DomainCategoryDataDto> data)
        {
            ResultDto result = new ResultDto();
            try
            {
                foreach (var category in data)
                {
                    List<string> domainList = new List<string>();
                    var col = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas();
                    var builder = Builders<IW2S_DomainCategoryData>.Filter;
                    if (string.IsNullOrEmpty(category.DomainName))
                    {
                        continue;
                    }
                    var catId = ObjectId.Empty;
                    ObjectId.TryParse(category.DomainCategoryId, out catId);
                    //var usrObjId = ObjectId.Empty;
                    //ObjectId.TryParse(category.UsrId, out usrObjId);
                    //var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(category.ProjectId))).Project(x => x.UsrId).FirstOrDefault();
                    //if (usrObjId == ObjectId.Empty)
                    //{
                    //    continue;
                    //}
                    if (string.IsNullOrEmpty(category.UsrId))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(category._id))
                    {
                        IW2S_DomainCategoryData cat = new IW2S_DomainCategoryData();
                        cat.DomainCategoryId = catId;
                        cat.DomainName = category.DomainName;
                        cat.UsrId = new ObjectId(category.UsrId);
                        col.InsertOne(cat);
                    }
                    else
                    {
                        var filter = builder.Eq(x => x._id, new ObjectId(category._id));
                        col.UpdateOne(filter, new UpdateDocument { { "$set", new QueryDocument { { "DomainName", category.DomainName }, { "DomainCategoryId", new ObjectId(category.DomainCategoryId) } } } });
                    }
                }
                result.IsSuccess = true;
            }
            catch
            {
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// 获取分类下的域名
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="usrId"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResultDomainCategory<IW2S_DomainCategoryDataDto> GetDomainCategoryData(string categoryId, string usrId, int page, int pagesize)
        {
            QueryResultDomainCategory<IW2S_DomainCategoryDataDto> query = new QueryResultDomainCategory<IW2S_DomainCategoryDataDto>();
            query.DomainCategoryId = categoryId;
            List<IW2S_DomainCategoryDataDto> result = new List<IW2S_DomainCategoryDataDto>();
            if (string.IsNullOrEmpty(categoryId))
            {
                return query;
            }
            var categoryIdList = GetIdListFromStr(categoryId);
            var builder = Builders<IW2S_DomainCategoryData>.Filter;
            var filter = builder.Eq(x => x.DomainCategoryId, new ObjectId(categoryId));
            var col = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas();
            query.Count = col.Find(filter).Count();
            result = col.Find(filter).Skip((page) * pagesize).Limit(pagesize).Project(x => new IW2S_DomainCategoryDataDto
            {
                _id = x._id.ToString(),
                DomainCategoryId = x.DomainCategoryId.ToString(),
                DomainName = x.DomainName,
                UsrId = x.UsrId.ToString()
            }).ToList();
            query.Result = result;
            return query;
        }

        /// <summary>
        /// 删除域名
        /// </summary>
        /// <param name="DomainId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelDomainCategoryData(string DomainId)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_DomainCategoryData>.Filter;
            var domainIdList = GetObjIdListFromStr(DomainId);
            var filter = builder.In(x => x._id, domainIdList);
            try
            {
                MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().DeleteMany(filter);
                result.IsSuccess = true;
            }
            catch
            {
                result.IsSuccess = false;
            }
            return result;
        }

        //设置矩形树图
        [HttpGet]
        public jsonFileUrlDto GetRectangularTreeUrl(string usr_id, string projectId)
        {
            //获取该项目下所有分组信息
            var tree = GetRectangularTree(usr_id, projectId, new ObjectId("000000000000000000000000"), new List<RectangularTree>());

            //获取所有关键词对应有效链接数
            List<ObjectId> keywordList = new List<ObjectId>();
            foreach (var x in tree)
            {
                if (x.IsNode) continue;
                keywordList.Add(new ObjectId(x.Id));
            }
            var builder = Builders<IW2S_BaiduCommend>.Filter;
            var filter = builder.In(x => x._id, keywordList);
            var taskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filter).Project(x => new
            {
                Id = x._id.ToString(),
                ValLinkCount = x.ValLinkCount
            }).ToList();
            foreach (var x in taskList)
            {
                for (int i = 0; i < tree.Count; i++)
                {
                    if (tree[i].Id.Equals(x.Id))
                    {
                        tree[i].ValLinkCount = x.ValLinkCount;
                    }
                }
            }
            ////获取所有词组关键词有效链接数
            //foreach (var x in tree)
            //{
            //    if (!x.IsNode) continue;
            //    int num = 0;
            //    bool HasNode = false;
            //    foreach (var v in tree)
            //    {
            //        if (x.Id.Equals(v.PId))
            //        {
            //            num += v.ValLinkCount;
            //            if (v.IsNode) HasNode = true;
            //        }
            //    }
            //    if (HasNode) x.ValLinkCount = num;
            //}
            RectangularTree root = new RectangularTree();
            root.Id = "000000000000000000000000";
            string text = GetRectanglarTreeList(tree, root);
            text = text.Substring(0, text.Length - 20);
            text += System.Environment.NewLine + "}";
            string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
            string path = folder + "RectangularTree.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.WriteLine(text);
                sw.Close();
            }
            else
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.WriteLine(text);
                sw.Close();
            }

            jsonFileUrlDto json = new jsonFileUrlDto();
            json.Url = "Scripts/app/data/" + "RectangularTree.txt";
            return json;
        }

        //获取词组及关键词列表
        private List<RectangularTree> GetRectangularTree(string usr_id, string projectId, ObjectId parentId, List<RectangularTree> list)
        {
            //获取次级词组名
            var builder = Builders<IW2S_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filter).Project(x => new RectangularTree
            {
                Id = x._id.ToString(),
                PId = x.ParentId.ToString(),
                Name = x.Name,
                IsNode = true,
                ValLinkCount = 0,
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<IW2S_KeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(groupfilter).Project(x => new RectangularTree
            {
                Id = x.BaiduCommendId.ToString(),
                PId = x.CommendCategoryId.ToString(),
                Name = x.BaiduCommend,
                IsNode = false,
                ValLinkCount = 0,
            }).ToList();

            //判断关键词在list是否已存在，存在修改其pId，不存在则将其添加至list中
            foreach (RectangularTree item in keywordList)
            {
                bool isHas = false;
                foreach (var item2 in list)
                {
                    if (item2.Name == item.Name & !item2.IsNode)
                    {
                        item2.PId = item.PId;
                        isHas = true;
                        continue;
                    }
                }
                if (!isHas)
                {
                    list.Add(item);
                }
            }

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0) return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (RectangularTree treedata in TaskList)
            {
                list.Add(treedata);
                GetRectangularTree(usr_id, projectId, new ObjectId(treedata.Id), list);
                // parent.children.Add(treedata);
            }

            return list;
        }

        //将列表转为字符串
        private string GetRectanglarTreeList(List<RectangularTree> list, RectangularTree parent)
        {
            string text = "{" + System.Environment.NewLine;
            List<RectangularTree> temp = new List<RectangularTree>();
            foreach (var v in list)
            {
                if (parent.Id.Equals(v.PId)) temp.Add(v);
            }
            foreach (var v in temp)
            {
                text += "\"" + v.Name + "\": ";
                if (v.IsNode)
                {
                    string text2 = GetRectanglarTreeList(list, v);
                    text += text2;
                }
                else
                {
                    text += "{" + System.Environment.NewLine + "\"$count\": " + v.ValLinkCount + System.Environment.NewLine + "}," + System.Environment.NewLine;
                }
            }
            text += "\"$count\": " + parent.ValLinkCount + System.Environment.NewLine + "}," + System.Environment.NewLine;
            return text;
        }

        /// <summary>
        /// 保存图表
        /// </summary>
        /// <param name="categoryId">关键词分组ID,多个用;相连</param>
        /// <param name="prjId">项目ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="percent">显示百分比以上的节点</param>
        /// <param name="topNum">标记前多少位</param>
        /// <param name="sumNum">摘要显示多少个，0表示仅显示1个</param>
        /// <param name="timeInterval">坐标点时间间隔</param>
        /// <param name="sourceType">信源类型</param>
        /// <param name="name">保存设置名称</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SaveChart(string categoryId, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval, string sourceType, string name, string user_id)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_ChartConfig>.Filter;
            var proOId = new ObjectId(prjId);
            //确定该项目该信源下该设置名称是否已经存在
            var filter = builder.Eq(x => x.ProjectId, proOId);
            filter &= builder.Eq(x => x.SourceType, sourceType);
            filter &= builder.Eq(x => x.Name, name);
            filter &= builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetIW2S_ChartConfig();
            var query = col.Find(filter).FirstOrDefault();
            if (query != null)
            {
                result.IsSuccess = false;
                result.Message = "该名称已存在！";
                return result;
            }
            //将所有设置信息合并为一条信息
            string config = categoryId;
            config += "|" + startTime;
            config += "|" + endTime;
            config += "|" + percent;
            config += "|" + topNum;
            config += "|" + sumNum;
            config += "|" + timeInterval;
            //创建新图表，
            IW2S_ChartConfig chart = new IW2S_ChartConfig
            {
                UsrId = new ObjectId(user_id),
                ProjectId = proOId,
                Name = name,
                Configuration = config,
                SourceType = sourceType,
                IsDel = false,
                ChartType = 0
            };
            col.InsertOne(chart);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取该项目该信源下所有保存的设置
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="prjId"></param>
        /// <param name="sourceType">信源类型</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_ChartConfigDto> GetChart(string user_id, string prjId, string sourceType)
        {
            var builder = Builders<IW2S_ChartConfig>.Filter;
            var proOId = new ObjectId(prjId);
            //获取该项目该信源下所有保存的设置
            var filter = builder.Eq(x => x.ProjectId, proOId);
            filter &= builder.Eq(x => x.SourceType, sourceType);
            filter &= builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetIW2S_ChartConfig();
            var query = col.Find(filter).ToList();
            //转换配置信息
            var configList = new List<IW2S_ChartConfigDto>();
            foreach (var x in query)
            {
                //拆分后依次是categoryId,startTime,endTime,percent,topNum,sumNum,timeInterval
                var configInfo = x.Configuration.Split('|').ToList();
                IW2S_ChartConfigDto config = new IW2S_ChartConfigDto
                {
                    _id = x._id.ToString(),
                    Name = x.Name,
                    SourceType = x.SourceType,
                    categoryId = configInfo[0],
                    startTime = configInfo[1],
                    endTime = configInfo[2],
                    percent = configInfo[3],
                    topNum = configInfo[4],
                    sumNum = configInfo[5],
                    timeInterval = configInfo[6]
                };
                configList.Add(config);
            }
            return configList;
        }

        /// <summary>
        /// 删除保存的设置
        /// </summary>
        /// <param name="ids">id列表，多个id用“;”隔开</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelChart(string ids)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_ChartConfig>.Filter;
            var oIdList = GetObjIdListFromStr(ids);
            var filter = builder.In(x => x._id, oIdList);
            var col = MongoDBHelper.Instance.GetIW2S_ChartConfig();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            col.UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public ResultDto CodeUserId(string user_id)
        {
            ResultDto result = new ResultDto();
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\views\wordtree\wordtree.html";
            string text = "";
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
                int index = text.IndexOf("userId") + 8;
                char a = text[index];
                if (a != '<')
                {
                    int end = text.IndexOf("<", index);
                    string temp = text.Substring(index, end = index);
                    text = text.Replace(temp, user_id);
                }
                else
                {
                    text = text.Insert(index, user_id);
                }
            }
            File.WriteAllText(path, text);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 保存文字树信息
        /// </summary>
        /// <param name="wordTree"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto SaveWordTree(WordTreeInfo wordTree)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_WordTree>.Filter;
            var filter = builder.Eq(x => x.Name, wordTree.Name);
            var col = MongoDBHelper.Instance.GetDnl_WordTree();
            var query = col.Find(filter).FirstOrDefault();
            if (query != null)
            {
                result.IsSuccess = false;
                result.Message = "该名称已存在";
            }
            var tree = new Dnl_WordTree
            {
                UsrId = new ObjectId(wordTree.UserId),
                Keyword = wordTree.Keyword,
                Text = wordTree.Text,
                Name = wordTree.Name,
                CreatedAt = DateTime.Now.AddHours(8)
            };
            col.InsertOne(tree);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取保存的文字树列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_WordTreeDto> GetWordTreeList(string userId)
        {
            var result = new QueryResult<Dnl_WordTreeDto>();
            var builder = Builders<Dnl_WordTree>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(userId)) & builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_WordTree().Find(filter).Project(x => new Dnl_WordTreeDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                Name = x.Name,
                CreatedAt = x.CreatedAt
            }).ToList();
            result.Result = query;
            result.Count = query.Count;
            return result;
        }

        /// <summary>
        /// 返回文字树内容
        /// </summary>
        /// <param name="treeId"></param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_WordTreeDto GetWordTree(string treeId)
        {
            var builder = Builders<Dnl_WordTree>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(treeId));
            var query = MongoDBHelper.Instance.GetDnl_WordTree().Find(filter).FirstOrDefault();
            if (query != null)
            {
                var wordtree = new Dnl_WordTreeDto
                {
                    _id = query._id.ToString(),
                    Keyword = query.Keyword,
                    Name = query.Name,
                    Text = query.Text,
                    UsrId = query.UsrId.ToString(),
                    CreatedAt = query.CreatedAt
                };
                return wordtree;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除文字树
        /// </summary>
        /// <param name="treeId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelWordTree(string treeId)
        {
            ResultDto result = new ResultDto();
            var objIds = GetObjIdListFromStr(treeId);
            var filter = Builders<Dnl_WordTree>.Filter.In(x => x._id, objIds);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetDnl_WordTree().UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 获取链接关系图谱
        /// </summary>
        /// <param name="prjId">项目Id</param>
        /// <param name="timeInterval">时间间隔，0为全部，1为月，2为季，3为年</param>
        /// <returns></returns>
        [HttpGet]
        public TimeLinkRefer GetLinkReference(string prjId, int timeInterval)
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
            var builderCate = Builders<IW2S_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(prjId));
            filterCate &= builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
            var queryCate = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filterCate).Project(x => new
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
            var builderGroup = Builders<IW2S_KeywordGroup>.Filter;
            var filterGroup = builderGroup.In(x => x.CommendCategoryId, cateObjIds) & builderGroup.Eq(x => x.IsDel, false);
            var queryKey = MongoDBHelper.Instance.GetIW2S_KeywordGroups().Find(filterGroup).Project(x => new
            {
                KeywordId = x.BaiduCommendId.ToString(),
                CategoryId = x.CommendCategoryId,
                Keyword = x.BaiduCommend,
            }).ToList();
            var keyIds = queryKey.Select(x => x.KeywordId).ToList();    //关键词ObjectId列表

            //建议关键词与词组字典
            var keyToCate = new Dictionary<string, string>();
            //var keyGroup = queryKey.GroupBy(x => x.KeywordId).ToList();
            queryKey = queryKey.DistinctBy(x => x.KeywordId).ToList();
            foreach (var key in queryKey)
            {
                var cateId = queryCate.Find(x => x.Id.Equals(key.CategoryId)).Id.ToString();
                keyToCate.Add(key.KeywordId, cateId);
            }

            //获取链接信息
            var builderLink = Builders<IW2S_level1link>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds) & builderLink.Eq(x => x.IsDel, false);
            var queryLink = MongoDBHelper.Instance.GetIW2S_level1links().Find(filterLink).Project(x => new
            {
                Title = x.Title,
                Description = x.Description,
                Keword = x.Keywords,
                KeywordId = x.SearchkeywordId,
                PublissTime = x.PublishTime,
                LinkUrl = x.LinkUrl
            }).ToList();

            //建立节点信息
            var linkNodes = new List<LinkRefer_Node>();         //节点信息
            for (int i = 0; i < queryLink.Count; i++)
            {
                //未存在发布信息时跳过该链接
                DateTime tpdt = new DateTime();
                DateTime.TryParse(queryLink[i].PublissTime, out tpdt);
                if (tpdt < new DateTime(1753, 1, 09) || tpdt > DateTime.Now)
                {
                    continue;
                }

                var link = new LinkRefer_Node();
                link.publishTime = tpdt;
                link.linkUrl = queryLink[i].LinkUrl;
                if (queryLink[i].Title != null && queryLink[i].Title.Length > 20)
                    link.name = queryLink[i].Title.Substring(0, 19) + "…";
                else
                    link.name = queryLink[i].Title;
                if (queryLink[i].Description != null && queryLink[i].Description.Length > 50)
                    link.describe = queryLink[i].Description.Substring(0, 49) + "…";
                else
                    link.describe = queryLink[i].Description;
                link.value = 1;

                //获取链接所含关键词及数量
                var repeat = queryLink.FindAll(s => s.Title == queryLink[i].Title).DistinctBy(s => s.KeywordId);

                link.keyWordCount = repeat.Count;
                link.keyWordList = new List<string>();
                link.keyWordIdList = new List<string>();
                foreach (var y in repeat)
                {
                    link.keyWordList.Add(y.Keword);
                    link.keyWordIdList.Add(y.KeywordId);
                    if (queryLink[i].KeywordId != y.KeywordId)
                    {
                        queryLink.Remove(y);
                    }
                }
                //获取归属组序号
                var cateId = keyToCate[queryLink[i].KeywordId];
                var cateIndex = cateInfo.FindIndex(s => s.id == cateId);
                link.category = cateIndex;
                link.describe = queryLink[i].Description;
                linkNodes.Add(link);
            }

            //建立时间坐标并将链接信息按时间分组
            DateTime startTime = linkNodes.Min(x => x.publishTime);
            DateTime endTime = linkNodes.Max(x => x.publishTime);
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

            //清理重复集群关系
            var tempGroup = linkRefers.GroupBy(x => x.source).ToList();              //统计获取集群中心点
            var tempCount = tempGroup.Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();               //统计获取集群中心点
            var usedIndex = new List<int>();    //已清理节点
            foreach (var x in tempCount)
            {
                if (usedIndex.Contains(x.Key))
                {
                    continue;
                }
                var centerNode = linkNodes[x.Key];      //集群中心点
                var referNodeList = tempGroup.Find(s => s.Key == x.Key);
                var referIndexs = referNodeList.Select(s => s.target).ToList();      //集群内所有相关点位置
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
                    }
                    usedIndex.Add(y.target);
                }
            }

            //获取Top10的集群中心点
            var topIndex = linkRefers.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
            result.TopData = new List<LinkRefer_Node>();
            foreach (var x in topIndex)
            {
                result.TopData.Add(linkNodes[x.Key]);
            }
            if (result.TopData.Count < 10)
            {
                foreach (var x in topIndex)
                {
                    //获取集群内其他点
                    var temp = linkRefers.FindAll(s => s.source == x.Key);
                    foreach (var y in temp)
                    {
                        result.TopData.Add(linkNodes[y.target]);
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
                        result.TopData.Add(x);
                }
            }

            //获取包含关键词最多的10个节点
            result.TopKeyData = linkNodes.OrderByDescending(x => x.keyWordCount).Take(10).ToList();

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
                result.TopCateData.Add(linkNodes[x.Pos]);
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

            return result;
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="post">实体post传输数据</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertEntity(Post_Entity post)
        {
            ResultDto result = new ResultDto();
            //检验传递过来的数据是否有误
            if (string.IsNullOrEmpty(post.Entity.Key))
            {
                result.Message = "实体名不能为空";
                return result;
            }
            //查询是否已存在该实体
            var builder = Builders<Dnl_EntityTree>.Filter;
            var filter = builder.Eq(x => x.Entity.Key, post.Entity.Key) & builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetDnl_EntityTree();
            var query = col.Find(filter).FirstOrDefault();
            if (query != null)
            {
                result.Message = "实体已存在";
                return result;
            }
            var entity = new Dnl_EntityTree
            {
                Entity = post.Entity,
                Attributes = post.Attributes,
                IsDel = false,
            };
            //判断是否写入Id
            if (!string.IsNullOrEmpty(post.Id))
            {
                entity._id = new ObjectId(post.Id);
            }
            //判断是公有还是私有
            if (!string.IsNullOrEmpty(post.UsrId))
            {
                entity.UsrId = new ObjectId(post.UsrId);
            }
            //判断是否为根结点
            if (!string.IsNullOrEmpty(post.ParentId))
            {
                entity.ParentId = new ObjectId(post.ParentId);
            }
            //判断是否重新记录时间
            if (post.CreatedAt == DateTime.MinValue)
            {
                entity.CreatedAt = DateTime.Now.AddHours(8);
            }
            else
            {
                entity.CreatedAt = post.CreatedAt;
            }
            col.InsertOne(entity);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userId">用户Id，为空时获取公用实体树</param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_EntityTreeDto> GetEntity(string userId)
        {
            var userObjId = ObjectId.Empty;
            //判断获取公有还是私有
            if (!string.IsNullOrEmpty(userId))
            {
                userObjId = new ObjectId(userId);
            }
            //获取所有实体树
            var builder = Builders<Dnl_EntityTree>.Filter;
            var filter = builder.Eq(x => x.UsrId, userObjId) & builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_EntityTree().Find(filter).Project(x => new Dnl_EntityTreeDto
            {
                Id = x._id.ToString(),
                ParentId = x.ParentId.ToString(),
                Entity = x.Entity,
                Attributes = x.Attributes,
                CreatedAt = x.CreatedAt
            }).ToList();

            //获取所有树的根结点
            var entityTrees = query.FindAll(x => x.ParentId == ObjectId.Empty.ToString());
            //获取各树的子树
            for (int i = 0; i < entityTrees.Count; i++)
            {
                entityTrees[i].Children = EntityToTree(query, entityTrees[i].Id);
            }
            return entityTrees;
        }

        /// <summary>
        /// 将实体结点列表转化为实体树
        /// </summary>
        /// <param name="entities">实体结点列表</param>
        /// <param name="parentId">父结点</param>
        /// <returns></returns>
        private List<Dnl_EntityTreeDto> EntityToTree(List<Dnl_EntityTreeDto> entities, string parentId)
        {
            var children = entities.FindAll(x => x.ParentId == parentId);
            foreach (var child in children)
            {
                child.Children = EntityToTree(entities, child.Id);
            }
            return children;
        }

        /// <summary>
        /// 获取某个实体的子树
        /// </summary>
        /// <param name="soure">源数据</param>
        /// <param name="parentId">当前实体Id</param>
        /// <returns></returns>
        private List<Dnl_EntityTreeDto> GetEntitySubtree(List<Dnl_EntityTreeDto> soure, string parentId)
        {
            var result = new List<Dnl_EntityTreeDto>();
            //获取次级子结点
            var trees = soure.FindAll(x => x.ParentId == parentId);
            result.AddRange(trees);
            //循环获取次次级子结点，直到叶结点为止
            foreach (var tree in trees)
            {
                var subtrees = GetEntitySubtree(soure, tree.Id);
                result.AddRange(subtrees);
            }
            return result;
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="post">实体post传输数据</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateEntity(Post_Entity post)
        {
            var result = new ResultDto();
            var builder = Builders<Dnl_EntityTree>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(post.UsrId));
            MongoDBHelper.Instance.GetDnl_EntityTree().DeleteOne(filter);
            InsertEntity(post);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelEntity(string id)
        {
            var objId = new ObjectId(id);
            var result = new ResultDto();
            var builder = Builders<Dnl_EntityTree>.Filter;
            var filter = builder.Eq(x => x._id, objId);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            var col = MongoDBHelper.Instance.GetDnl_EntityTree();
            var parentId = col.Find(filter).Project(x => x.ParentId).FirstOrDefault();
            col.UpdateOne(filter, update);
            //修改子树结点归属
            var filterChild = builder.Eq(x => x.ParentId, objId) & builder.Eq(x => x.IsDel, false);
            var updateChild = new UpdateDocument { { "$set", new QueryDocument { { "ParentId", parentId } } } };
            col.UpdateMany(filterChild, updateChild);

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取链接报表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<LinkInfo> GetLinkReport(string userId, string projectId)
        {
            var userObjId = new ObjectId(userId);
            //获取域名分组信息
            var domainToCate = new Dictionary<string, string>();        //域名与归属词组字典
            var builderCate = Builders<IW2S_DomainCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.UsrId, new ObjectId(userId));
            var queryCate = MongoDBHelper.Instance.GetIW2S_DomainCategorys().Find(filterCate).Project(x => new
            {
                Id = x._id.ToString(),
                Name = x.Name
            }).ToList();
            //如果用户无分组，则使用公有分组
            if (queryCate.Count == 0)
            {
                userObjId = ObjectId.Empty;
                filterCate = builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.UsrId, ObjectId.Empty);
                queryCate = MongoDBHelper.Instance.GetIW2S_DomainCategorys().Find(filterCate).Project(x => new
                {
                    Id = x._id.ToString(),
                    Name = x.Name
                }).ToList();
            }
            var cateObjIds = queryCate.Select(x => new ObjectId(x.Id)).ToList();
            var builderDomain = Builders<IW2S_DomainCategoryData>.Filter;
            var filterDomain = builderDomain.In(x => x.DomainCategoryId, cateObjIds);
            var queryDomain = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(filterDomain).Project(x => new
            {
                Id = x._id.ToString(),
                CateId = x.DomainCategoryId.ToString(),
                Domain = x.DomainName
            }).ToList();
            queryDomain = queryDomain.DistinctBy(x => x.Id).ToList();
            foreach (var x in queryDomain)
            {
                if (!domainToCate.ContainsKey(x.Domain))
                {
                    var cateName = queryCate.Find(s => s.Id == x.CateId).Name;
                    domainToCate.Add(x.Domain, cateName);
                }
            }

            //获取链接信息
            var builderKey = Builders<IW2S_BaiduCommend>.Filter;
            var filterKey = builderKey.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderKey.Eq(x => x.IsRemoved, false);
            var keyIds = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(filterKey).Project(x => x._id.ToString()).ToList();
            var builderLink = Builders<IW2S_level1link>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds) & builderLink.Eq(x => x.IsDel, false);
            var queryLink = MongoDBHelper.Instance.GetIW2S_level1links().Find(filterLink).Project(x => new
            {
                Id = x._id.ToString(),
                Domain = x.Domain,
                Title = x.Title,
                LinkUrl = x.LinkUrl,
                PublishTime = x.PublishTime,
                Keyword = x.Keywords
            }).ToList();

            //转换发布时间
            var linkInfos = new List<LinkInfo>();
            foreach (var x in queryLink)
            {
                DateTime dt = new DateTime();
                DateTime.TryParse(x.PublishTime, out dt);
                string title = x.Title;
                if (title.Length > 20)
                {
                    title = title.Substring(0, 19) + "...";
                }
                LinkInfo link = new LinkInfo
                {
                    编号 = x.Id,
                    域名 = x.Domain,
                    链接地址 = x.LinkUrl,
                    关键词 = new List<string>(),
                    年 = dt.Year,
                    月 = dt.Month,
                    日 = dt.Day,
                    标题 = title,
                    标题2 = "<a href=\"" + x.LinkUrl + "\">" + title + "</a>",
                    评论数 = 0
                };
                link.关键词.Add(x.Keyword);
                linkInfos.Add(link);
            }

            //去重并获取域名分组
            for (int i = 0; i < linkInfos.Count; i++)
            {
                var repeats = linkInfos.FindAll(x => x.链接地址 == linkInfos[i].链接地址);
                if (repeats.Count > 1)
                {
                    repeats.Remove(linkInfos[i]);
                    foreach (var x in repeats)
                    {
                        linkInfos[i].关键词.AddRange(x.关键词);
                        linkInfos.Remove(x);
                    }
                }
                if (domainToCate.ContainsKey(linkInfos[i].域名))
                {
                    var cateName = domainToCate[linkInfos[i].域名];
                    linkInfos[i].域名分组 = cateName;
                }
                else
                {
                    linkInfos[i].域名分组 = "未分组";
                }
            }

            return linkInfos;
        }

        [HttpGet]
        public ResultDto BackupProject(string projectId)
        {
            //备份项目信息
            var sourceProId = new ObjectId(projectId);
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x._id, sourceProId);
            var colPro = MongoDBHelper.Instance.GetIW2S_Projects();
            var sourcePro = colPro.Find(filterPro).FirstOrDefault();
            var backupPro = sourcePro;
            backupPro._id = ObjectId.GenerateNewId();
            backupPro.Name += "_备份";
            backupPro.CreatedAt = DateTime.Now.AddHours(8);
            colPro.InsertOne(backupPro);

            //备份关键词分组
            BackupKeywordCate(ObjectId.Empty, ObjectId.Empty, sourceProId, backupPro._id);

            //备份折线图设置
            var builderChart = Builders<IW2S_ChartConfig>.Filter;
            var filterChart = builderChart.Eq(x => x.IsDel, false) & builderChart.Eq(x => x.ChartType, 0);
            filterChart &= builderChart.Eq(x => x.ProjectId, sourceProId);
            var colChart = MongoDBHelper.Instance.GetIW2S_ChartConfig();
            var sourceCharts = colChart.Find(filterChart).ToList();
            if (sourceCharts.Count > 0)
            {
                var backupCharts = new List<IW2S_ChartConfig>();
                foreach (var x in sourceCharts)
                {
                    var chart = x;
                    chart._id = ObjectId.GenerateNewId();
                    chart.ProjectId = backupPro._id;
                    backupCharts.Add(chart);
                }
                colChart.InsertMany(backupCharts);
            }

            //备份项目链接数变化数组
            var builderProLink = Builders<IW2S_ProLinksCount>.Filter;
            var filterProLink = builderProLink.Eq(x => x.ProjectId, sourceProId);
            var colProLink = MongoDBHelper.Instance.GetIW2S_ProLinksCount();
            var sourceProLink = colProLink.Find(filterProLink).ToList();
            if (sourceProLink.Count > 0)
            {
                var backupProLink = new List<IW2S_ProLinksCount>();
                foreach (var x in sourceProLink)
                {
                    var chart = x;
                    chart._id = ObjectId.GenerateNewId();
                    chart.ProjectId = backupPro._id;
                    backupProLink.Add(chart);
                }
                colProLink.InsertMany(backupProLink);
            }


            ResultDto result = new ResultDto();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 迭代备份关键词组及组内关键词
        /// </summary>
        /// <param name="soureCateObjId">原父分组Id</param>
        /// <param name="BackupCateObjId">备份的父分组Id</param>
        /// <param name="sourceProObjId">原项目Id</param>
        /// <param name="BackupProObjId">备份的项目Id</param>
        private void BackupKeywordCate(ObjectId soureCateObjId, ObjectId BackupCateObjId, ObjectId sourceProObjId, ObjectId BackupProObjId)
        {
            //获取原子分组信息
            var builderCate = Builders<IW2S_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ProjectId, sourceProObjId);
            filterCate &= builderCate.Eq(x => x.ParentId, soureCateObjId);
            var colCate = MongoDBHelper.Instance.GetIW2S_KeywordCategorys();
            var sourceCates = colCate.Find(filterCate).ToList();

            var builderKey = Builders<IW2S_KeywordGroup>.Filter;
            var colKey = MongoDBHelper.Instance.GetIW2S_KeywordGroups();
            foreach (var x in sourceCates)
            {
                var sourceId = x._id;
                //备份子分组信息
                var backupCate = x;
                backupCate._id = ObjectId.GenerateNewId();
                backupCate.ProjectId = BackupProObjId;
                backupCate.ParentId = BackupCateObjId;
                colCate.InsertOne(backupCate);
                //备份子分组内关键词
                var filterKey = builderKey.Eq(s => s.CommendCategoryId, sourceId);
                var soureKeys = colKey.Find(filterKey).ToList();
                var backupKeys = new List<IW2S_KeywordGroup>();
                foreach (var y in soureKeys)
                {
                    var key = y;
                    key._id = ObjectId.GenerateNewId();
                    key.CommendCategoryId = backupCate._id;
                    key.ParentCategoryId = BackupCateObjId;
                    key.ProjectId = BackupProObjId;
                    backupKeys.Add(key);
                }
                colKey.InsertMany(backupKeys);

                //迭代备份孙分组
                BackupKeywordCate(sourceId, backupCate._id, sourceProObjId, BackupProObjId);
            }

        }
    }

}
