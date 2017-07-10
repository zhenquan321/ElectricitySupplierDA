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
    public class WXSourceController : ApiController
    {


        /// <summary>
        /// 插入微信热词搜索关键词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="keywords">多个词以;隔开</param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto insertKeyword(string user_id, string keywords, string projectId)
        {
            var keywordList = keywords.Split(';', '；');
            var builder = Builders<IW2S_WX_BaiduKeyword>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
            ResultDto result = new ResultDto();

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();

            var builderCommend = Builders<IW2S_WX_BaiduCommend>.Filter;

            foreach (var keyword in keywordList)
            {
                if (string.IsNullOrEmpty(keyword)) continue;

                var filter = builder.Eq(x => x.Keyword, keywords);
                
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
                IW2S_WX_BaiduKeyword kw = new IW2S_WX_BaiduKeyword
                {
                    _id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = new ObjectId(projectId),
                    BotStatus = 0,
                    Keyword = keyword,
                    ProjectName = keyword,
                    UsrId = usrObjId
                };
                
                col.InsertOne(kw);


                var filterCommend = builderCommend.Eq(x => x.KeywordId, kw._id);
                
                if (!string.IsNullOrEmpty(projectId))
                {
                    filterCommend &= builderCommend.Eq(x => x.ProjectId, new ObjectId(projectId));
                }

                filterCommend &= builderCommend.Eq(x => x.IsRemoved, false);
                var colCommend = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
                var dtoCommend = colCommend.Find(filterCommend).FirstOrDefault();

                if (dtoCommend != null)
                {

                    result.IsSuccess = true;
                    return result;

                }
                IW2S_WX_BaiduCommend kwCommend = new IW2S_WX_BaiduCommend
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
                if (!string.IsNullOrEmpty(user_id))
                {
                    kwCommend.UsrId = new ObjectId(user_id);
                }
                colCommend.InsertOne(kwCommend);
            }
            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;

            return result;
        }

        [HttpGet]
        //插入微信搜索关键词
        public ResultDto insertBaiduSearchKeyword(string user_id, string keywords, string projectId)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.CommendKeyword, keywords);

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            filter &= builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
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
            IW2S_WX_BaiduCommend kw = new IW2S_WX_BaiduCommend
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
            if (!string.IsNullOrEmpty(user_id))
            {
                kw.UsrId = new ObjectId(user_id);
            }
            col.InsertOne(kw);
            result.IsSuccess = true;

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

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
            var builder = Builders<IW2S_WX_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(keyword_id));
            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "ProjectName", keywords } } } };
            col.UpdateOne(filter, update);


            result.IsSuccess = true;
            return result;

        }

        //获取微信推荐关键词
        [HttpGet]
        public List<IW2S_WX_BaiduCommendDto> GetBaiduCommendKeyword(string user_id, string keywordId, string projectId)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, false);
            if (!string.IsNullOrEmpty(keywordId))
            {
                filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            }
            
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            List<string> keyNameList = new List<string>();
            foreach (var item in TaskList)
            {
                if (keyNameList.Contains(item.CommendKeyword)) continue;
                IW2S_WX_BaiduCommendDto v = new IW2S_WX_BaiduCommendDto();
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

        //获取直搜微信的关键词
        [HttpGet]
        public List<IW2S_WX_BaiduCommendDto> GetBaiduSearchKeyword(string user_id)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            foreach (var item in TaskList)
            {
                IW2S_WX_BaiduCommendDto v = new IW2S_WX_BaiduCommendDto();
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

        //获取排除的微信推荐关键词
        [HttpGet]
        public List<IW2S_WX_BaiduCommendDto> GetExcludeBaiduCommend(string user_id, string keyword, string projectId)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, true);
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            
            filter &= builder.Eq(x => x.Keyword, keyword);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).SortByDescending(x => x.Times).ToList();
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            foreach (var item in TaskList)
            {
                IW2S_WX_BaiduCommendDto v = new IW2S_WX_BaiduCommendDto();
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
        //排除微信推荐关键词
        public string ExcludeKeyword(List<IW2S_WX_BaiduCommendDto> lists)
        {
            foreach (var list in lists)
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", list.IsRemoved } } } };

                var result = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().UpdateOne(new QueryDocument { { "_id", new ObjectId(list._id) } }, update).ModifiedCount;

                var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", list.IsRemoved } } } };
                MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().UpdateMany(new QueryDocument { { "BaiduCommendId", new ObjectId(list._id) } }, updategroup);

                //排除关键词搜索出来的链接
                var updateLinks = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", 2 } } } };
                MongoDBHelper.Instance.GetIW2S_WX_level1links().UpdateMany(new QueryDocument { { "SearchkeywordId", list._id } }, updateLinks);
            }
            return "成功！";
        }

        [HttpPost]
        //排除微信关键词
        public string ExcludeBaiduKeyword(List<IW2S_WX_BaiduKeywordDto> lists)
        {
            foreach (var list in lists)
            {

                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", list.IsRemoved } } } };
                var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
                var result = col.UpdateOne(new QueryDocument { { "_id", new ObjectId(list._id) } }, update).ModifiedCount;

                //IW2S_WX_KeywordCategory
                var updatecategory = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", list.IsRemoved } } } };
                MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().UpdateMany(new QueryDocument { { "KeywordId", new ObjectId(list._id) } }, updatecategory);

                var commendIds = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(new QueryDocument { { "KeywordId", new ObjectId(list._id) } }).Project(x => new IW2S_WX_BaiduCommendDto { IsRemoved = true, _id = x._id.ToString() }).ToList();

                //排除关键词搜索出来的链接
                var updateLinks = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", 2 } } } };
                MongoDBHelper.Instance.GetIW2S_WX_level1links().UpdateMany(new QueryDocument { { "SearchkeywordId", list._id } }, updateLinks);
                
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
        public QueryResult<IW2S_WX_level1linkDto> GetLevelLinks(string user_id, string projectId, string categoryId, string keywordId, string Title, string domain, string infriLawCode, byte? status, int page, int pagesize)
        {
            //if (string.IsNullOrEmpty(keywordId) && string.IsNullOrEmpty(categoryId))
            //{
            //    return new QueryResult<IW2S_WX_level1linkDto> { Count = 0, Result = new List<IW2S_WX_level1linkDto>() };
            //}
            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            if (!string.IsNullOrEmpty(Title))
            {
                filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + Title + ".*/"));
            }

            if (!string.IsNullOrEmpty(infriLawCode))
            {
                List<ObjectId> InfriList = GetObjIdListFromStr(infriLawCode);
                filter = filter & builder.In(x => x.InfriLawCode, InfriList);
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
                    var buildGroup = Builders<IW2S_WX_KeywordGroup>.Filter;
                    var filterGroup = buildGroup.In(x => x.CommendCategoryId, GetObjIdListFromStr(categoryId));
                    var keywordIdsGroup = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(filterGroup).Project(x => x.BaiduCommendId.ToString()).ToList();
                    if (keywordIdsGroup.Count > 0)
                    {
                        commendIds.AddRange(keywordIdsGroup);
                    }
                }
            }
            if (commendIds.Count > 0)
            {
                filter &= builder.In(x => x.SearchkeywordId, commendIds);
            }

            if (!string.IsNullOrEmpty(domain))
            {
                filter = filter & builder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + domain + ".*/"));
            }
            if (status.HasValue)
            {
                filter &= builder.Eq(x => x.DataCleanStatus, status.Value);
            }
            else
            {
                filter &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
            }

            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();

            var query = col.Find(filter).Project(x => new IW2S_WX_level1linkDto
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
                Function = x.Function,
                SourceLink = x.SourceLink,
                QrCode = x.QrCode,
                ImgIcon = x.ImgIcon,
                TitleImg = x.TitleImg,
                PublishTime = x.PublishTime,
                 
            });

            var count = query.Count();
            var TaskList = query.Skip((page) * pagesize).Limit(pagesize).ToList();


            return new QueryResult<IW2S_WX_level1linkDto> { Count = count, Result = TaskList };
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

            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(id));

            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();
            IW2S_WB_level1link linkUrlPrj = col.Find(filter).Project(x => new IW2S_WB_level1link
            {
                PostUrl = x.LinkUrl,
                ProjectId = x.ProjectId
            }).FirstOrDefault();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", status } } } };
            col.UpdateOne(filter, update);

            if (status == 1)
            {
                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = linkUrlPrj.ProjectId,
                    ShareOperateType = (int)ShareOperateType.CollectConfig,
                    UserId = new ObjectId(user_id),
                    SiteSource = (int)SiteSource.Weichat
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 设置链接链接标签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="infriType"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetLinkInfriType(string user_id, string id, int infriType)
        {
            ResultDto result = new ResultDto();

            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(id));

            var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", infriType } } } };
            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();
            col.UpdateOne(filter, update);

            var filter1 = builder.Eq(x => x._id, new ObjectId(id));

            var prjId = col.Find(filter1).Project(x => x.ProjectId).FirstOrDefault();
            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = prjId,
                ShareOperateType = (int)ShareOperateType.SetLinkAnalysisItem,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        private void RefreshValidLinkCount(IW2S_WX_BaiduCommend p)
        {
            var update = new UpdateDocument { { "$set", new QueryDocument { {"LastBotEndAt",DateTime.UtcNow.AddHours(8)},
                {"BotStatus",2}} }};
            var commendCol = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
            var result = commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);

            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.UsrId, p.UsrId);
            filter &= builder.Eq(x => x.SearchkeywordId, p._id.ToString());
            filter &= builder.Ne(x => x.DataCleanStatus, (byte)2);
            filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/")) |
        builder.Regex(x => x.Description, new BsonRegularExpression("/.*" + p.CommendKeyword + ".*/"));

            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();
            var agreresult = col.Aggregate().Match(filter)
                .Group(new BsonDocument { { "_id", "$_id" }, { "Count", new BsonDocument("$sum", 1) } })
                .ToListAsync()
                .Result;

            var vallinkCount = agreresult.Count;
            update = new UpdateDocument { { "$set", new QueryDocument { { "ValLinkCount", vallinkCount } } } };

            commendCol.UpdateOne(new QueryDocument { { "_id", p._id } }, update);
        }

        [HttpGet]
        public List<IW2S_WX_BaiduKeywordDto> GetBaiduKeyword(string user_id, string projectId, int page, int pagesize)
        {

            var builder = Builders<IW2S_WX_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsRemoved, false);

            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
            var TaskList = col.Find(filter).SortByDescending(x => x.CreatedAt).Skip((page) * pagesize).Limit(pagesize).ToList();

            List<IW2S_WX_BaiduKeywordDto> list = new List<IW2S_WX_BaiduKeywordDto>();
            var builder1 = Builders<IW2S_WX_BaiduCommend>.Filter;
            foreach (var item in TaskList)
            {
                IW2S_WX_BaiduKeywordDto v = new IW2S_WX_BaiduKeywordDto();
                v._id = item._id.ToString();
                v.Keyword = item.Keyword;
                v.ProjectName = item.ProjectName;
                if (string.IsNullOrEmpty(v.ProjectName))
                {
                    v.ProjectName = item.Keyword;
                }

                var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));
                filter1 &= builder1.Eq(x => x.KeywordId, item._id);
                filter1 &= builder1.Eq(x => x.SearchSource, 1);
                filter1 &= builder1.Eq(x => x.IsRemoved, false);
                var searchData = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter1)
                    .Project(x => new IW2S_WX_BaiduCommendDto { _id = x._id.ToString(), ValLinkCount = x.ValLinkCount, BotStatus = x.BotStatus }).FirstOrDefault();
                if (searchData != null)
                {
                    v.ValLinkCount = searchData.ValLinkCount;
                    v.CommendKeywordId = searchData._id;
                    v.BotStatus = searchData.BotStatus;
                }

                list.Add(v);


                filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));
                filter1 &= builder1.Eq(x => x.KeywordId, item._id);
                filter1 &= builder1.Eq(x => x.SearchSource, 0);
                filter1 &= builder1.Eq(x => x.IsRemoved, false);
                var TaskList1 = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter1).SortByDescending(x => x.Times).ToList();
                List<IW2S_WX_BaiduCommendDto> list1 = new List<IW2S_WX_BaiduCommendDto>();
                foreach (var item1 in TaskList1)
                {
                    IW2S_WX_BaiduCommendDto v1 = new IW2S_WX_BaiduCommendDto();
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
        /// <param name="groupType">1:微信直搜，2：微信热词</param>
        /// <param name="keywordIds">多个Id以,隔开</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertKeywordGroup(string usr_id, string projectId, string groupName, string lawCode, int weight, int groupType, string parentGroupId, string keywordId, string keywordIds)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(groupName))
            {
                result.Message = "分组名不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            var categorybuilder = Builders<IW2S_WX_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x.ProjectId, new ObjectId(projectId)); ;
            //categoryfilter &= categorybuilder.Eq(x => x.GroupType, groupType);
            categoryfilter &= categorybuilder.Eq(x => x.Name, groupName);

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            var keywordIdList = keywordIds.Split(';', '；', ',');
            if (categoryDto != null)
            {
                result.Message = "分组已存在";
                return result;
            }
            else
            {
                categoryDto = new IW2S_WX_KeywordCategory
                {
                    _id = ObjectId.GenerateNewId(),
                    //GroupType = groupType,
                    Name = groupName,
                    //InfriLawCode = new ObjectId(lawCode),
                    IsDel = false,
                    UsrId = usrObjId,
                    Weight = weight,
                    ProjectId = new ObjectId(projectId),
                    GroupNumber = 0
                };
                if (!string.IsNullOrEmpty(lawCode))
                {
                    ObjectId lawCodeId = ObjectId.Empty;
                    ObjectId.TryParse(lawCode, out lawCodeId);
                    categoryDto.InfriLawCode = lawCodeId;
                }
                if (!string.IsNullOrEmpty(parentGroupId))
                {
                    categoryDto.ParentId = new ObjectId(parentGroupId);
                }
                if (!string.IsNullOrEmpty(keywordId))
                {
                    categoryDto.KeywordId = new ObjectId(keywordId);
                }
                categoryDto.KeywordTotal = keywordIdList.Length;

                categoryCol.InsertOne(categoryDto);
            }
            if (!string.IsNullOrEmpty(keywordIds))
            {
                int keywordTotal = 0, valLinkCount = 0;
                var keywordBuilder = Builders<IW2S_WX_BaiduCommend>.Filter;
                var keywordCol = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
                foreach (string keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {

                        var keywordFilter = keywordBuilder.Eq(x => x._id, new ObjectId(keyId));
                        var keyword = keywordCol.Find(keywordFilter).Project(x => new { CommendKeyword = x.CommendKeyword, ValLinkCount = x.ValLinkCount }).FirstOrDefault();
                        if (keyword != null)
                        {
                            IW2S_WX_KeywordGroup groupDto = new IW2S_WX_KeywordGroup
                            {
                                //GroupType = groupType,
                                BaiduCommendId = new ObjectId(keyId),
                                BaiduCommend = keyword.CommendKeyword,
                                CommendCategoryId = categoryDto._id,
                                ParentCategoryId = categoryDto.ParentId,
                                IsDel = false,
                                UsrId = usrObjId,
                                ProjectId = new ObjectId(projectId)
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
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(usr_id),
                SiteSource = (int)SiteSource.Weichat
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
        [HttpGet]
        public ResultDto UpdateKeywordGroup(string user_id, string groupid, string groupName, string lawCode, int weight, string keywordIds)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(groupid))
            {
                result.Message = "修改的分组不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            var categorybuilder = Builders<IW2S_WX_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x._id, new ObjectId(groupid));

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();

            if (categoryDto == null)
            {
                result.Message = "修改的分组不能为空";
                return result;
            }

            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, categoryDto._id);

            var alloldCommendIds = keywordGroupCol.Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            keywordGroupCol.DeleteMany(groupfilter);
            var keywordIdList = keywordIds.Split(';', '；', ',');

            int keywordTotal = 0;
            bool isLevel1 = categoryDto.ParentId.Equals(new ObjectId("000000000000000000000000")) ? true : false;
            if (!string.IsNullOrEmpty(keywordIds))
            {
                var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
                foreach (var keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {
                        alloldCommendIds.Remove(new ObjectId(keyId));

                        IW2S_WX_KeywordGroup groupDto = new IW2S_WX_KeywordGroup
                        {
                            //GroupType = categoryDto.GroupType,
                            BaiduCommendId = new ObjectId(keyId),
                            ParentCategoryId = categoryDto.ParentId,
                            CommendCategoryId = categoryDto._id,
                            IsDel = false,
                            UsrId = categoryDto.UsrId,
                            ProjectId = categoryDto.ProjectId
                        };

                        var filter = builder.Eq(x => x._id, new ObjectId(keyId));
                        var keywordStr = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).Project(x => x.CommendKeyword).FirstOrDefault();
                        if (string.IsNullOrEmpty(keywordStr))
                        {
                            continue;
                        }
                        groupDto.BaiduCommend = keywordStr;

                        keywordGroupCol.InsertOne(groupDto);
                        keywordTotal++;
                        if (isLevel1)
                        {
                            UpdateLinksInfriType(keyId, lawCode);
                        }
                    }
                }
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", groupName }, { "InfriLawCode", lawCode }, { "Weight", weight }, { "KeywordTotal", keywordTotal } } } };
            categoryCol.UpdateOne(categoryfilter, update);

            if (alloldCommendIds.Count > 0)
            {
                RecurseDelSubKeyword(categoryDto, alloldCommendIds);
                if (isLevel1)
                {
                    foreach (var alloldCommendId in alloldCommendIds)
                    {
                        UpdateLinksInfriType(alloldCommendId.ToString(), "000000000000000000000000");
                    }
                }
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = categoryDto.ProjectId,
                ShareOperateType = (int)ShareOperateType.ManageGroup,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        private void UpdateLinksInfriType(string searchKeywordId, string lawCode)
        {
            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.SearchkeywordId, searchKeywordId);

            var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", new ObjectId(lawCode) } } } };

            MongoDBHelper.Instance.GetIW2S_WX_level1links().UpdateMany(filter, update);
        }

        private void RecurseDelSubKeyword(IW2S_WX_KeywordCategory categoryDto, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            var categoryfilter = categorybuilder.Eq(x => x.ParentId, categoryDto._id);

            var subcategoryDtos = categoryCol.Find(categoryfilter).ToList();
            if (subcategoryDtos.Count > 0)
            {
                long delCount = 0;
                foreach (var subcategoryDto in subcategoryDtos)
                {
                    var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, subcategoryDto._id) & groupbuilder.In(x => x.BaiduCommendId, alloldCommendIds);

                    keywordGroupCol.DeleteMany(groupfilter);

                    groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, subcategoryDto._id);
                    delCount = keywordGroupCol.Find(groupfilter).Project(x => x._id).Count();

                    var updateca = new UpdateDocument { { "$set", new QueryDocument { { "KeywordTotal", delCount } } } };
                    categoryfilter = categorybuilder.Eq(x => x._id, subcategoryDto._id);
                    categoryCol.UpdateOne(categoryfilter, updateca);
                    //
                    RecurseDelSubKeyword(subcategoryDto, alloldCommendIds);
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
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            var keywordGroupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            var categorybuilder = Builders<IW2S_WX_KeywordCategory>.Filter;

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
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public List<IW2S_WX_KeywordCategoryDto> GetAllKeywordCategory(string user_id, string projectId)
        {
            List<IW2S_WX_KeywordCategoryDto> result = new List<IW2S_WX_KeywordCategoryDto>();

            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));

            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).SortBy(x => x.ParentId).ToList();

            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));

            var keywordBuilder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var keywordCol = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
            FilterDefinition<IW2S_WX_BaiduCommend> keywordFilter = null;
            ObjectId nullId = new ObjectId("000000000000000000000000");

            foreach (var item in TaskList)
            {
                IW2S_WX_KeywordCategoryDto v = new IW2S_WX_KeywordCategoryDto();
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

                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, item._id);

                var selectedIdList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

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
        /// <param name="groupType">1:微信直搜，2：微信热词</param>
        /// <param name="keywordId">关键词ID</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_WX_KeywordCategoryDto> GetKeywordCategory(string user_id, string projectId, int groupType, string keywordId)
        {
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //if (!string.IsNullOrEmpty(keywordId))
            //{
            //    filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            //}
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).SortBy(x => x.ParentId).ToList();

            List<IW2S_WX_KeywordCategoryDto> list = new List<IW2S_WX_KeywordCategoryDto>();

            Dictionary<string, string> dicCategoryIDName = new Dictionary<string, string>();

            var commendbuilder = Builders<IW2S_WX_BaiduCommend>.Filter;

            var commendfilter = commendbuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & commendbuilder.Eq(x => x.IsRemoved, false); ;
            var keywordCount = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(commendfilter).Project(x => x._id).Count();

            list.Add(new IW2S_WX_KeywordCategoryDto
            {
                _id = "",
                Name = "所有词",
                KeywordTotal = (int)keywordCount
            });

            foreach (var item in TaskList)
            {
                IW2S_WX_KeywordCategoryDto v = new IW2S_WX_KeywordCategoryDto();
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
        /// <param name="groupType">1:微信直搜，2：微信热词</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_WX_BaiduCommendDto> GetKeywordGroup(string usr_id, string projectId, string groupid, string keywordId, int groupType)
        {
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            //if(string.IsNullOrEmpty( keywordId))
            //{
            //    return list;
            //}
            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId(groupid));
            }
            else
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId("000000000000000000000000"));
            }

            var selectedIdList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<IW2S_WX_BaiduCommendDto> allKeywords = new List<IW2S_WX_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                allKeywords = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_WX_BaiduCommendDto
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
        public WX_KeywordGroupModelDto GetEditKeywordGroup(string usr_id, string projectId, string groupid, string parentid, string keywordId, int groupType)
        {
            WX_KeywordGroupModelDto result = new WX_KeywordGroupModelDto();
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            //if (string.IsNullOrEmpty(keywordId))
            //{
            //    return result;
            //}
            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId(parentid));
            }
            else
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId("000000000000000000000000"));
            }


            var selectedIdList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<IW2S_WX_BaiduCommendDto> allKeywords = new List<IW2S_WX_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(parentid));
                allKeywords = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_WX_BaiduCommendDto
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

            List<IW2S_WX_BaiduCommendDto> curSelectedList = new List<IW2S_WX_BaiduCommendDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                curSelectedList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter)
                    .Project(x => new IW2S_WX_BaiduCommendDto
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
        public ResultDto DelKeywordCategory(string categoryId)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(categoryId))
            {
                return result;
            }
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, new ObjectId(categoryId));
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();

            categoryCol.DeleteOne(builder.Eq(x => x._id, new ObjectId(categoryId)));

            var groupBuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.CommendCategoryId, new ObjectId(categoryId));
            var groupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();
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

            return result;
        }
        private void RecurseDelCategory(ObjectId categoryId)
        {

            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, categoryId);
            var categoryCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();

            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            categoryCol.DeleteMany(builder.In(x => x._id, TaskList));

            var groupBuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();
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
            //child2.name = "微信热词";
            //result.children.Add(child2);

            //GetCategoryTree(usr_id, new ObjectId("000000000000000000000000"), child1, 1);
            GetCategoryTree(usr_id, projectId, new ObjectId("000000000000000000000000"), result, 2);

            return result;
        }

        private void GetHotCategoryTree(string usr_id, string projectId, ObjectId parentId, GroupTreeDto parent, int groupType)
        {
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => new GroupTreeDto
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
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => new GroupTreeDto
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
            if (string.IsNullOrEmpty(commendKeywordId))
            {
                return result;
            }
            var builder = Builders<IW2S_WX_KeywordFilter>.Filter;

            var keywordList = keywords.Split(';', '；');

            var col = MongoDBHelper.Instance.GetIW2S_WX_KeywordFilters();
            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            foreach (var keyword in keywordList)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                    filter &= builder.Eq(x => x.Keyword, keyword);
                    filter &= builder.Eq(x => x.CommendKeywordId, new ObjectId(commendKeywordId));

                    filter &= builder.Eq(x => x.IsDel, false);

                    var dto = col.Find(filter).FirstOrDefault();

                    if (dto != null)
                    {
                        result.Message = "关键词‘" + keyword + "’已经存在！";
                        return result;

                    }

                    IW2S_WX_KeywordFilter kw = new IW2S_WX_KeywordFilter
                    {
                        _id = ObjectId.GenerateNewId(),
                        CreatedAt = DateTime.Now.AddHours(8),
                        CommendKeywordId = new ObjectId(commendKeywordId),
                        IsDel = false,
                        Keyword = keyword,
                        ProjectId = new ObjectId(projectId),
                        UsrId = usrObjId
                    };
                    col.InsertOne(kw);

                    var linkBuilder = Builders<IW2S_WX_level1link>.Filter;
                    var linkFilter = linkBuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & linkBuilder.Eq(x => x.SearchkeywordId, commendKeywordId);
                    linkFilter = linkBuilder.Ne(x => x.DataCleanStatus, (byte)1);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keyword + ".*/")));

                    var linkcol = MongoDBHelper.Instance.GetIW2S_WX_level1links();
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", (byte)2 } } } };
                    linkcol.UpdateMany(linkFilter, update);
                }
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.FilterConfig,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.Weichat
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
        public List<IW2S_WX_KeywordFilterDto> GetKeywordFilter(string user_id, string projectId, string keywordId)
        {
            var builder = Builders<IW2S_WX_KeywordFilter>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Eq(x => x.CommendKeywordId, new ObjectId(keywordId));
            filter &= builder.Eq(x => x.IsDel, false);
           
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordFilters().Find(filter).SortByDescending(x => x.CreatedAt).ToList();
            List<IW2S_WX_KeywordFilterDto> list = new List<IW2S_WX_KeywordFilterDto>();
            foreach (var item in TaskList)
            {
                IW2S_WX_KeywordFilterDto v = new IW2S_WX_KeywordFilterDto();
                v._id = item._id.ToString();
                v.CommendKeywordId = item.CommendKeywordId.ToString();
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
        public string DelKeywordFilter(string filterIds)
        {
            var filterlist = filterIds.Split(';', '；');
            foreach (var filterId in filterlist)
            {
                if (!string.IsNullOrEmpty(filterId))
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
                    var col = MongoDBHelper.Instance.GetIW2S_WX_KeywordFilters();
                    var keywordFilter = col.Find(new QueryDocument { { "_id", new ObjectId(filterId) } }).Project(x => new IW2S_WX_KeywordFilterDto
                    {
                        CommendKeywordId = x.CommendKeywordId.ToString(),
                        Keyword = x.Keyword
                    }).FirstOrDefault();

                    col.UpdateOne(new QueryDocument { { "_id", new ObjectId(filterId) } }, update);

                    var linkBuilder = Builders<IW2S_WX_level1link>.Filter;
                    var linkFilter = linkBuilder.Eq(x => x.SearchkeywordId, keywordFilter.CommendKeywordId);
                    linkFilter = linkBuilder.Eq(x => x.DataCleanStatus, (byte)2);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")));

                    var linkcol = MongoDBHelper.Instance.GetIW2S_WX_level1links();
                    var linkupdate = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", (byte)0 } } } };
                    linkcol.UpdateMany(linkFilter, update);
                }

            }
            return "成功！";
        }


        //获取微信推荐关键词
        [HttpGet]
        public List<IW2S_WX_BaiduCommendDto> GetBaiduCommendKeywordPage(string user_id, string projectId, int page, int pagesize)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, false);

            
            if (!string.IsNullOrEmpty(projectId))
            {
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            }
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).SortByDescending(x => x.Times).Skip(page * pagesize).Limit(pagesize).ToList();
            List<IW2S_WX_BaiduCommendDto> list = new List<IW2S_WX_BaiduCommendDto>();
            List<string> keyNameList = new List<string>();
            foreach (var item in TaskList)
            {
                if (keyNameList.Contains(item.CommendKeyword)) continue;
                IW2S_WX_BaiduCommendDto v = new IW2S_WX_BaiduCommendDto();
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
            var filter = builder.Eq(x => x._id, new ObjectId(projectId));
            var task = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter).Project(x => new IW2S_ProjectDto { Name = x.Name }).FirstOrDefault();
            result.name = task.Name;
            //根目录ID默认为"000000000000000000000000"
            result.id = "000000000000000000000000";
            result.pId = "0";
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
            //叶结点数
            int count2 = 0;

            foreach (var v in list)
            {
                if (v.id == id) { GroupTree.name = v.name; }
                if (v.pId == id)
                {
                    //判断x是否为叶结点
                    foreach (var x2 in list)
                    {
                        if (x2.pId == v.id)
                        {
                            count2++;
                        }
                    }
                    //count2==0说明x为叶结点,否则为普通结点，依结点类型不同判断是否继续递归
                    if (count2 == 0)
                    {
                        //获取有效链接数
                        var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
                        var filter = builder.Eq(x => x._id, new ObjectId(v.id));
                        var task = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).FirstOrDefault();

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
                        count2 = 0;
                    }
                }
            }
            //判断是否已查询到叶子结点，是则子树为叶结点集合，否则为普通结点集合
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
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => new GroupTree2Dto
            {
                id = x.BaiduCommendId.ToString(),
                pId = x.CommendCategoryId.ToString(),
                name = x.BaiduCommend
            }).ToList();

            //判断关键词在list是否已存在，存在修改其pId，不存在则将其添加至list中
            foreach (var item in keywordList)
            {
                bool isHas = false;
                foreach (var item2 in list)
                {
                    if (item2.name == item.name)
                    {
                        item2.pId = item.pId;
                        isHas = true;
                        continue;
                    }
                }
                if (!isHas)
                {
                    GroupTree2Dto gt = new GroupTree2Dto();
                    gt.id = item.id;
                    gt.pId = item.pId;
                    gt.name = item.name;
                    list.Add(gt);
                }
            }

            //若次级词组不存在，返回null，中断递归
            if (TaskList.Count == 0)
                return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (var treedata in TaskList)
            {
                GetCategoryTree3(usr_id, projectId, new ObjectId(treedata.id), list);
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
            var builder = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys().Find(filter).Project(x => new GroupTree2Dto
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
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsRemoved, status);
            //判断是否为根目录，是直接查询IW2S_BaiduCommends集合，否则先获取分类关键词再查询IW2S_BaiduCommends
            if (string.Equals(treeNodeId, "000000000000000000000000"))
            {
                keywordList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).Project(x => new GroupKeywordsDto
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
                var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
                var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(treeNodeId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, status);
                var TempList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();
                //获取关键词名、Bot状态和有效链接数
                var countfilter = builder.In(x => x._id, TempList) & builder.Eq(x => x.IsRemoved, status);
                var temp = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(countfilter).Project(x => new GroupKeywordsDto
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

                    var result = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().UpdateOne(new QueryDocument { { "_id", id } }, update).ModifiedCount;

                    var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", status } } } };
                    MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().UpdateMany(new QueryDocument { { "BaiduCommendId", id } }, updategroup);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //获取关键词 分组和链接
        [HttpGet]
        public jsonFileUrlDto GetKeywordBygroup(string user_id, string projectId)
        {
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.SearchSource, 1);
            filter &= builder.Eq(x => x.IsRemoved, false);
            var TaskList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).Limit(60).ToList();
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
            var builder2 = Builders<WX_links>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter2 &= builder2.Lte(x => x.source, 59);
            filter2 &= builder2.Lte(x => x.target, 59);
            filter2 &= builder2.In(x => x.KeywordId, lobj);
            var TaskList2 = MongoDBHelper.Instance.GetIW2S_WX_links().Find(filter2).SortByDescending(x => x.value).Limit(1000).ToList();
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

            var builder = Builders<IW2S_WX_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Eq(x => x.IsRemoved, false);
            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
            var keywords = col.Find(filter).ToList();
            if (keywords == null || keywords.Count == 0)
            {
                return "没有要导出的数据";
            }

            HSSFWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet("IW2S_WX_BaiduKeyword");
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

            var builder1 = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder1.Eq(x => x.IsRemoved, false);
            var col1 = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
            var keywords1 = col1.Find(filter1).ToList();

            ISheet sheet1 = workBook.CreateSheet("IW2S_WX_BaiduCommend");
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

            var builder2 = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder2.Eq(x => x.IsDel, false);
            var col2 = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            var keywords2 = col2.Find(filter2).ToList();

            ISheet sheet2 = workBook.CreateSheet("IW2S_WX_KeywordCategory");
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

            var builder3 = Builders<IW2S_WX_KeywordGroup>.Filter;
            var filter3 = builder3.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder3.Eq(x => x.IsDel, false);
            var col3 = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();
            var keywords3 = col3.Find(filter3).ToList();

            ISheet sheet3 = workBook.CreateSheet("IW2S_WX_KeywordGroup");
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

            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & !builder.Ne(x => x.DataCleanStatus, (byte)2);
            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();
            int page = 0, pageSize = 100;
            List<IW2S_WX_level1link> keywords = new List<IW2S_WX_level1link>();
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
            ISheet sheet = workBook.CreateSheet("IW2S_WX_level1link");
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
            List<IW2S_WX_BaiduKeyword> keywords = new List<IW2S_WX_BaiduKeyword>();
            List<IW2S_WX_BaiduCommend> commendKeywords = new List<IW2S_WX_BaiduCommend>();
            List<IW2S_WX_KeywordCategory> keywordCategories = new List<IW2S_WX_KeywordCategory>();
            List<IW2S_WX_KeywordGroup> groups = new List<IW2S_WX_KeywordGroup>();

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
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_WX_BaiduKeyword keyword = new IW2S_WX_BaiduKeyword();
                        keyword._id = new ObjectId(row.GetCell(0).StringCellValue);
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
                        IW2S_WX_BaiduCommend commendKeyword = new IW2S_WX_BaiduCommend();
                        commendKeyword._id = new ObjectId(row.GetCell(0).StringCellValue);
                        commendKeyword.CreatedAt = DateTime.Now.AddHours(8);
                        commendKeyword.IsRemoved = false;
                        commendKeyword.Keyword = row.GetCell(3).StringCellValue;
                        commendKeyword.Times = (int)row.GetCell(5).NumericCellValue;
                        commendKeyword.CommendKeyword = row.GetCell(1).StringCellValue;
                        commendKeyword.KeywordId = new ObjectId(row.GetCell(4).StringCellValue);
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
                        IW2S_WX_KeywordCategory keywordCategory = new IW2S_WX_KeywordCategory();
                        keywordCategory._id = new ObjectId(row.GetCell(0).StringCellValue);
                        keywordCategory.IsDel = false;
                        keywordCategory.GroupNumber = (int)row.GetCell(1).NumericCellValue;
                        keywordCategory.GroupType = (int)row.GetCell(2).NumericCellValue;
                        keywordCategory.InfriLawCode = new ObjectId(row.GetCell(3).StringCellValue);
                        keywordCategory.KeywordId = new ObjectId(row.GetCell(4).StringCellValue);
                        keywordCategory.KeywordTotal = (int)row.GetCell(5).NumericCellValue;
                        keywordCategory.Name = row.GetCell(6).StringCellValue;
                        keywordCategory.ParentId = new ObjectId(row.GetCell(7).StringCellValue);
                        keywordCategory.Name = row.GetCell(6).StringCellValue;
                        keywordCategory.ProjectId = new ObjectId(projectId);
                        keywordCategory.UsrId = usrObjId;

                        keywordCategories.Add(keywordCategory);
                    }

                    HSSFSheet sheet3 = workbook.GetSheetAt(3) as HSSFSheet;

                    for (int i = 1; i <= sheet3.LastRowNum; i++)
                    {
                        HSSFRow row = sheet3.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }
                        IW2S_WX_KeywordGroup group = new IW2S_WX_KeywordGroup();
                        group._id = new ObjectId(row.GetCell(0).StringCellValue);
                        group.IsDel = false;
                        group.BaiduCommend = row.GetCell(1).StringCellValue;
                        group.BaiduCommendId = new ObjectId(row.GetCell(2).StringCellValue);
                        group.CommendCategoryId = new ObjectId(row.GetCell(3).StringCellValue);
                        group.GroupType = (int)row.GetCell(4).NumericCellValue;
                        group.ParentCategoryId = new ObjectId(row.GetCell(5).StringCellValue);
                        group.ProjectId = new ObjectId(projectId);
                        group.UsrId = usrObjId;

                        groups.Add(group);
                    }
                }


            }
            catch (Exception ex)
            {
                message = "导入数据失败";
            }
            var builder = Builders<IW2S_WX_BaiduKeyword>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col = MongoDBHelper.Instance.GetIW2S_WX_BaiduKeywords();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsRemoved", true } } } };
            foreach (var keyword in keywords)
            {
                //col.UpdateMany(filter & builder.Eq(x => x.Keyword, keyword.Keyword), update);
                col.DeleteMany(filter & builder.Eq(x => x.Keyword, keyword.Keyword));
                col.InsertOne(keyword);
            }

            var builder1 = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter1 = builder1.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col1 = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends();
            foreach (var commendKeyword in commendKeywords)
            {
                //col1.UpdateMany(filter1 & builder1.Eq(x => x.CommendKeyword, commendKeyword.CommendKeyword) & builder1.Eq(x=>x.KeywordId,commendKeyword.KeywordId), update);
                col1.DeleteMany(filter1 & builder1.Eq(x => x.CommendKeyword, commendKeyword.CommendKeyword) & builder1.Eq(x => x.KeywordId, commendKeyword.KeywordId));
                col1.InsertOne(commendKeyword);
            }

            var builder2 = Builders<IW2S_WX_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col2 = MongoDBHelper.Instance.GetIW2S_WX_KeywordCategorys();
            update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            foreach (var keywordCategory in keywordCategories)
            {
                //col2.UpdateMany(filter2 & builder2.Eq(x => x.Name, keywordCategory.Name), update);
                col2.DeleteMany(filter2 & builder2.Eq(x => x.Name, keywordCategory.Name));
                col2.InsertOne(keywordCategory);
            }

            var builder3 = Builders<IW2S_WX_KeywordGroup>.Filter;
            var filter3 = builder3.Eq(x => x.ProjectId, new ObjectId(projectId));
            var col3 = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            foreach (var group in groups)
            {
                //col3.UpdateMany(filter3 & builder3.Eq(x => x.BaiduCommendId, group.BaiduCommendId)&builder3.Eq(x=>x.CommendCategoryId,group.CommendCategoryId), update);
                col3.DeleteMany(filter3 & builder3.Eq(x => x.BaiduCommendId, group.BaiduCommendId) & builder3.Eq(x => x.CommendCategoryId, group.CommendCategoryId));
                col3.InsertOne(group);
            }

            message = "导入成功";

            return message;
        }

        [HttpGet]
        public string ImportLevelLinks(string user_id, string projectId, string excelFilePath)
        {
            string message = "";
            string Path = System.Web.Hosting.HostingEnvironment.MapPath("~/ExportFiles/ImportExcel/"); //当前路径 
            List<IW2S_WX_level1link> keywords = new List<IW2S_WX_level1link>();
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
                        IW2S_WX_level1link keyword = new IW2S_WX_level1link();
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
            catch (Exception ex)
            {
                message = "导入数据失败";
            }
            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Ne(x => x.DataCleanStatus, (byte)2);
            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();

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
        public List<DomainStatisDto> GetDomainStatis(string categoryId, string prjId)
        {
            List<DomainStatisDto> result = new List<DomainStatisDto>();
            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var groupBuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
            if (!string.IsNullOrEmpty(categoryId))
            {
                var cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
                groupFilter &= groupBuilder.In(x => x.CommendCategoryId, cateIds);
            }


            var groupCol = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups();

            var TaskList = groupCol.Find(groupFilter).Project(x => x.BaiduCommendId).ToList().Select(x => x.ToString()).ToList();

            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.In(x => x.SearchkeywordId, TaskList);
            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();
            var agreresult = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, TaskList))
                .Group(new BsonDocument { { "_id", "$Domain" }, { "Count", new BsonDocument("$sum", 1) }, { "RankTotal", new BsonDocument("$sum", "$Rank") }, { "KeywordTotal", new BsonDocument("$sum", 1) } })
                        .ToListAsync()
                        .Result;
            var agreresult2 = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, TaskList))
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

            foreach (var ag in agreresult)
            {
                DomainStatisDto ds = new DomainStatisDto();
                ds.Domain = ag["_id"].ToString();
                ds.Count = ag["Count"].ToInt32();
                if (domainKeywords.ContainsKey(ds.Domain))
                {
                    ds.KeywordTotal = domainKeywords[ds.Domain];
                }
                ds.RankTotal = ag["RankTotal"].ToInt32() / ds.Count;
                result.Add(ds);
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
            var builder = Builders<IW2S_WX_PrjAnalysisItems>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(prjId));

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(prjId))).Project(x => x.UsrId).FirstOrDefault();

            var col = MongoDBHelper.Instance.GetIW2S_WX_PrjAnalysisItems();
            col.DeleteMany(filter);

            var prjItem = new IW2S_WX_PrjAnalysisItems();
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
                UserId = new ObjectId(usr_id),
                SiteSource = (int)SiteSource.Weichat
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
            var builder = Builders<IW2S_WX_PrjAnalysisItems>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(prjId));

            var col = MongoDBHelper.Instance.GetIW2S_WX_PrjAnalysisItems();
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
        public List<IW2S_AnalysisItemDto> GetAnalysisItem(string usr_id)
        {
            List<IW2S_AnalysisItemDto> results = new List<IW2S_AnalysisItemDto>();
            var builder = Builders<IW2S_AnalysisItem>.Filter;
            var filter = builder.Eq(x => x.IsRemoved, false);

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
            var filter = builder.Eq(x => x.UsrId, new ObjectId(anaItem.UsrId)) & builder.Eq(x => x.IsRemoved, false) & builder.Eq(x => x.Name, anaItem.Name);
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
            result.Message = "分析指项保存成功";

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(anaItem.ProjectId),
                ShareOperateType = (int)ShareOperateType.ManageAnalysisItem,
                UserId = new ObjectId(anaItem.UsrId),
                SiteSource = (int)SiteSource.Weichat
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

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
                    UserId = new ObjectId(user_id),
                    SiteSource = (int)SiteSource.Weichat
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
        public QueryResult<IW2S_WX_Timelevel1linkDto> GetTimeLinkList(string categoryId, string prjId, string pubTime)
        {
            QueryResult<IW2S_WX_Timelevel1linkDto> result = new QueryResult<IW2S_WX_Timelevel1linkDto>();

            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var groupBuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
            var builder = Builders<IW2S_WX_level1link>.Filter;
            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
            }
            else
            {
                return result;
            }

            //var allFilter = Builders<IW2S_BaiduCommend>.Filter.Eq(x => x.ProjectId, new ObjectId(prjId)) & Builders<IW2S_BaiduCommend>.Filter.Eq(x => x.IsRemoved, false);
            //var allTaskList = MongoDBHelper.Instance.GetIW2S_BaiduCommends().Find(allFilter).Project(x => x._id).ToList().Select(x => x.ToString()).ToList();

            //获取所给节点ID下关键词ID
            var keywordFilter = groupBuilder.In(x => x.CommendCategoryId, cateIds) & groupBuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(keywordFilter).Project(x => x.BaiduCommendId).ToList().Select(x => x.ToString()).ToList();

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");

            var allQueryDatas = MongoDBHelper.Instance.GetIW2S_WX_level1links().Find(allTimeFilter).Project(x => new
            {
                PublishTime = x.PublishTime,
                Title = x.Title,
                LinkUrl = x.LinkUrl,
                Keywords = x.Keywords
            }).ToList();
            DateTime tpTime = new DateTime();
            List<IW2S_WX_Timelevel1linkDto> datas = new List<IW2S_WX_Timelevel1linkDto>();
            foreach (var gr in allQueryDatas)
            {
                DateTime.TryParse(gr.PublishTime, out tpTime);
                IW2S_WX_Timelevel1linkDto data = new IW2S_WX_Timelevel1linkDto();


                data.PublishTime = tpTime;
                data.Title = gr.Title;
                data.LinkUrl = gr.LinkUrl;
                data.Keywords = gr.Keywords;
                datas.Add(data);
            }
            DateTime pubTimedt = DateTime.MinValue;
            DateTime.TryParse(pubTime, out pubTimedt);
            if (pubTimedt != DateTime.MinValue)
            {
                datas = datas.Where(x => x.PublishTime == pubTimedt).ToList();
                //if(datas.Count==0)return new QueryResult<IW2S_Timelevel1linkDto> {Count=0};
            }
            datas = datas.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.Title).ToList();

            //去除标题重复的数据，并将同一标题对应的多个关键词合并到一起
            bool isRepeat = false;
            int j = 0;
            List<IW2S_WX_Timelevel1linkDto> repeat = new List<IW2S_WX_Timelevel1linkDto>();
            for (int i = 1; i < datas.Count; i++)
            {
                if (datas[j].Title.Equals(datas[i].Title))
                {
                    repeat.Add(datas[i]);
                    datas[j].Keywords += "；" + datas[i].Keywords;
                    isRepeat = true;
                }
                else
                {
                    isRepeat = false;
                }
                if (!isRepeat)
                {
                    j = i;
                }
            }
            foreach (var x in repeat)
            {
                datas.Remove(x);
            }
            datas = datas.OrderByDescending(x => x.PublishTime).ToList();

            result.Count = datas.Count;
            result.Result = datas;


            return result;
        }

        //监测结果页面获取多个分类关键词
        [HttpGet]
        public List<GroupKeywordsDto> GetFenleiKeywordsView(string usr_id, string projectId, string categoryId)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            var builder = Builders<IW2S_WX_BaiduCommend>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &

            //判断是否为根目录，是直接查询IW2S_BaiduCommends集合，否则先获取分类关键词再查询IW2S_BaiduCommends
            if (string.Equals(categoryId, "000000000000000000000000") || string.IsNullOrEmpty(categoryId))
            {
                filter &= builder.Eq(x => x.IsRemoved, false);
                keywordList = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(filter).Project(x => new GroupKeywordsDto
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
                var groupbuilder = Builders<IW2S_WX_KeywordGroup>.Filter;
                var groupfilter = groupbuilder.In(x => x.CommendCategoryId, categoryIdList) & groupbuilder.Eq(x => x.IsDel, false);
                var TempList = MongoDBHelper.Instance.GetIW2S_WX_KeywordGroups().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();
                //获取关键词名、Bot状态和有效链接数
                var countfilter = builder.In(x => x._id, TempList) & builder.Eq(x => x.IsRemoved, false);
                var temp = MongoDBHelper.Instance.GetIW2S_WX_BaiduCommends().Find(countfilter).Project(x => new GroupKeywordsDto
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

        //获取监测结果页面搜索链接数据
        [HttpPost]
        public QueryResultView<IW2S_WX_level1linkDto> GetLevelLinksView(LinkSearchData linkSearchData)
        {
            //if (string.IsNullOrEmpty(keywordId) && string.IsNullOrEmpty(categoryId))
            //{
            //    return new QueryResultView<IW2S_level1linkDto> { Count = 0, Result = new List<IW2S_level1linkDto>() };
            //}
            var builder = Builders<IW2S_WX_level1link>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id))
            //单独查询最后一个关键词
            var filterLast = builder.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) 
            if (!string.IsNullOrEmpty(linkSearchData.Title))
            {
                //模糊查询
                filter &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + linkSearchData.Title + ".*/"));
                filterLast &= builder.Regex(x => x.Title, new BsonRegularExpression("/.*" + linkSearchData.Title + ".*/"));
            }

            //若infriLawCode为空，则查找未设置链接标签数据，否则拆分后查找数据
            if (!string.IsNullOrEmpty(linkSearchData.infriLawCode))
            {
                //ObjectId lawCodeId = ObjectId.Empty;
                //ObjectId.TryParse(infriLawCode, out lawCodeId);
                if (linkSearchData.infriLawCode.Equals("000000000000000000000000"))
                {
                    filter = filter & builder.Eq(x => x.InfriLawCode, new ObjectId("000000000000000000000000"));
                    filterLast = filter & builder.Eq(x => x.InfriLawCode, new ObjectId("000000000000000000000000"));
                }
                else
                {
                    List<ObjectId> InfriList = GetObjIdListFromStr(linkSearchData.infriLawCode);
                    filter = filter & builder.In(x => x.InfriLawCode, InfriList);
                    filterLast = filter & builder.In(x => x.InfriLawCode, InfriList);
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
            }

            //域名筛选
            if (!string.IsNullOrEmpty(linkSearchData.domain))
            {
                filter = filter & builder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + linkSearchData.domain + ".*/"));
                filterLast = filter & builder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + linkSearchData.domain + ".*/"));
            }

            //值筛选
            if (linkSearchData.status.HasValue)
            {
                filter &= builder.Eq(x => x.DataCleanStatus, linkSearchData.status.Value);
                filterLast &= builder.Eq(x => x.DataCleanStatus, linkSearchData.status.Value);
            }
            else
            {
                filter &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
                filterLast &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
            }

            var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();

            var query = col.Find(filter).Project(x => new IW2S_WX_level1linkDto
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
                InfriLawCodeStr = null,
                PublishTime = x.PublishTime
            });

            var count = query.Count();
            var TaskList = query.Skip((linkSearchData.page) * linkSearchData.pagesize).Limit(linkSearchData.pagesize).ToList();
            if (!linkSearchData.infriLawCode.Equals("000000000000000000000000"))
            {
                foreach (var list in TaskList)
                {
                    var builderInfri = Builders<IW2S_AnalysisItemValue>.Filter.Eq(x => x._id, new ObjectId(list.InfriLawCode));
                    string InfriName = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues().Find(builderInfri).Project(x => x.Name).FirstOrDefault();
                    list.InfriLawCodeStr = InfriName;
                }
            }

            bool hasvalue = true;
            //判断最后一个关键词是否有数据
            if (!string.IsNullOrEmpty(lastKeyword))
            {
                filterLast &= builder.Eq(x => x.SearchkeywordId, lastKeyword);
                long LastCount = col.Find(filterLast).Count();
                if (LastCount <= 0)
                {
                    hasvalue = false;
                }
            }


            return new QueryResultView<IW2S_WX_level1linkDto> { Count = count, Result = TaskList, HasValue = hasvalue, infriLawCode = linkSearchData.infriLawCode };
        }

        /// <summary>
        /// 设置链接链接标签
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


                var builder = Builders<IW2S_WX_level1link>.Filter;
                var col = MongoDBHelper.Instance.GetIW2S_WX_level1links();

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
                        UserId = new ObjectId(user_id),
                        SiteSource = (int)SiteSource.Weichat
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
    }
}