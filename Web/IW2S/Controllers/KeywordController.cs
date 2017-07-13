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
using System.Text.RegularExpressions;

namespace IW2S.Controllers
{
    public class KeywordController : ApiController
    {
        #region 关键词
        /// <summary>
        /// 插入百度热词搜索关键词
        /// </summary>
        /// <param name="keyInfo">关键词插入POST类</param>
        /// <returns></returns>
        [HttpPost]
        public ResultKeywordDto insertKeyword(KeywordPost keyInfo)
        {
            var result = new ResultKeywordDto();
            try
            {
                var keywordList = keyInfo.keywords.Split(';', '；');
                var builderKey = Builders<Dnl_Keyword>.Filter;
                var colKey = MongoDBHelper.Instance.GetDnl_Keyword();
                var builderMapping = Builders<Dnl_KeywordMapping>.Filter;
                var colMapping = MongoDBHelper.Instance.GetDnl_KeywordMapping();
                var proObjId = new ObjectId(keyInfo.projectId);

                //获取用户信息
                var userObjId = new ObjectId(keyInfo.user_id);
                var filterUser = Builders<IW2SUser>.Filter.Eq(x => x._id, userObjId);
                var colUser = MongoDBHelper.Instance.Get_IW2SUser();
                var user = colUser.Find(filterUser).FirstOrDefault();

                List<string> commendKeys = new List<string>();      //推荐词列表
                foreach (var keyword in keywordList)
                {
                    if (string.IsNullOrEmpty(keyword)) continue;
                    var filterMapping = builderMapping.Eq(x => x.ProjectId, proObjId) & builderMapping.Eq(x => x.IsDel, false);

                    //if (user.UserType != UserTypes.Engineer)
                    //{
                    //    //查询用户已有关键词映射数，判断其是否已达到限制
                    //    var keyIds = colMapping.Find(filterMapping).Project(x => x.KeywordId.ToString()).ToList();
                    //    if (keyIds.Count >= user.Limit_KeywordNum)
                    //    {
                    //        //判断本次添加词只有一个还是多个
                    //        if (keywordList.Count() == 1)
                    //        {
                    //            result.Message = "关键词数已达上限！";
                    //            return result;
                    //        }
                    //        break;
                    //    }
                    //    //查询用户已有链接数，判断其是否已达到限制
                    //    //获取项目内已删除的链接Id
                    //    var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
                    //    var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
                    //    var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

                    //    //获取链接信息
                    //    var builderLink = Builders<Dnl_Link_Baidu>.Filter;
                    //    var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
                    //    filterLink &= builderLink.Nin(x => x._id, exLinkObjIds);
                    //    var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => x._id).ToList();
                    //    if (queryLink.Count >= user.Limit_KeywordNum)
                    //    {
                    //        //判断本次添加词只有一个还是多个
                    //        if (keywordList.Count() == 1)
                    //        {
                    //            result.Message = "链接数已达上限！";
                    //            return result;
                    //        }
                    //        break;
                    //    }
                    //}

                    //查询关键词是否已添加
                    filterMapping &= builderMapping.Eq(x => x.Keyword, keyword);
                    var queryMapping = colMapping.Find(filterMapping).FirstOrDefault();
                    if (queryMapping != null)
                    {
                        //判断本次添加词只有一个还是多个
                        if (keywordList.Count() == 1)
                        {
                            result.Message = "关键词：" + keyword + " 已存在！";
                            return result;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //查询关键词是否已在关键词库中
                    var filter = builderKey.Eq(x => x.Keyword, keyword);
                    var query = colKey.Find(filter).FirstOrDefault();
                    ObjectId keyObjId = ObjectId.GenerateNewId();
                    /* 不存在则新增一个关键词并添加映射
                     * 存在新增一个指向该关键词的映射 */
                    if (query == null)
                    {
                        Dnl_Keyword key = new Dnl_Keyword
                        {
                            _id = keyObjId,
                            CreatedAt = DateTime.Now.AddHours(8),
                            Keyword = keyword,
                            BotIntervalHours = 7 * 24
                        };
                        colKey.InsertOne(key);
                    }
                    else
                    {
                        keyObjId = query._id;
                    }
                    Dnl_KeywordMapping mapping = new Dnl_KeywordMapping
                    {
                        Keyword = keyword,
                        KeywordId = keyObjId,
                        ProjectId = proObjId,
                        CreatedAt = DateTime.Now.AddHours(8),
                        UserId = userObjId
                    };
                    colMapping.InsertOne(mapping);
                    //判断是否插入到词组
                    if (!string.IsNullOrEmpty(keyInfo.cateId))
                    {
                        //获取所有要插入的分组信息
                        var cateInfos = new List<KeywordCateInfo>();
                        GetAllParentCate(new ObjectId(keyInfo.cateId), cateInfos);
                        //生成所有要插入的映射
                        var maps = new List<Dnl_KeywordMapping>();
                        foreach (var x in cateInfos)
                        {
                            Dnl_KeywordMapping temp = new Dnl_KeywordMapping
                            {
                                Keyword = keyword,
                                KeywordId = keyObjId,
                                ProjectId = new ObjectId(keyInfo.projectId),
                                UserId = userObjId,
                                CategoryId = x.CategoryId,
                                CreatedAt = DateTime.Now.AddHours(8),
                                ParentCategoryId = x.ParentCategoryId
                            };
                            maps.Add(temp);
                        }
                        colMapping.InsertMany(maps);
                    }

                    //判断是否获取百度推荐词
                    if (!keyInfo.isCommend)
                    {
                        try
                        {
                            commendKeys.AddRange(GetBaiduCommend(keyword));
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                result.KeywordList = commendKeys;

                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = proObjId,
                    ShareOperateType = (int)ShareOperateType.AddKeyword,
                    UserId = new ObjectId(keyInfo.user_id)
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 循环获取所有父分组Id
        /// </summary>
        /// <param name="nowCateId">当前分组Id</param>
        /// <param name="cateInfos">所有分组Id信息</param>
        /// <returns></returns>
        private void GetAllParentCate(ObjectId nowCateId,List<KeywordCateInfo> cateInfos)
        {
            //获取该分组的父分组Id
            var filter = Builders<Dnl_KeywordCategory>.Filter.Eq(x => x._id, nowCateId);
            var parentId = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => x.ParentId).FirstOrDefault();
            var info = new KeywordCateInfo();
            info.CategoryId = nowCateId;
            info.ParentCategoryId = parentId;
            cateInfos.Add(info);
            //在父分组不为空的时候循环获取上一级父分组Id
            if(!parentId.Equals(ObjectId.Empty))
                GetAllParentCate(parentId, cateInfos);
        }

        #region 获取百度推荐词
        /// <summary>
        /// 获取百度推荐词
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        private List<string> GetBaiduCommend(string keyword)
        {
            List<string> commendKeys = new List<string>();
            //生成搜索网页地址
            string baiduUrlFormat = "http://www.baidu.com/s?ie=utf-8&wd={0}";
            string url = baiduUrlFormat.FormatStr(keyword.GetUrlEncodedString("utf-8"));

            //抓取网页源码，最多尝试5次
            string html = null;
            int num = 0;
            int maxTry = 5;
            while (num < maxTry)
            {
                html = GetFastHtml(url);
                if (!string.IsNullOrEmpty(html))
                    break;
                else
                    num++;
            }

            //获取百度推荐词部分源码并分组
            var tags = html.SubAfter("相关搜索</div>").SubBefore("id=\"page\"").SplitWith("<a");
            if (tags == null || tags.Length == 0)
            {
                return null;
            }
            //循环提取百度推荐词
            foreach (var a in tags)
            {
                string commend = GetTxtFromHtml(a);
                if (!string.IsNullOrEmpty(commend))
                    commendKeys.Add(commend.Trim());
            }
            return commendKeys;
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="charset">编码格式</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        private string GetFastHtml(string url, string charset = "utf-8", int timeout = 5000)
        {

            try
            {
                var uri = new Uri(url);
                var req = WebRequest.CreateDefault(uri);
                //var wp = new WebProxy(ip, port) { BypassProxyOnLocal = true };
                //req.Proxy = wp;
                req.Timeout = timeout;

                var res = req.GetResponse();
                var resStream = res.GetResponseStream();

                var realUrl = res.ResponseUri.AbsoluteUri;

                if (resStream != null)
                {
                    var reader = new StreamReader(resStream, System.Text.Encoding.GetEncoding(charset));
                    resStream.Flush();
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        //去掉 <xxxxx>  包括<xxxx>,</xxxx>
        //  href="a">abcd <para>xs</para>   处理完得到 abcdxs
        public string GetTxtFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";
            int indexOfLeft = html.GetIndex("<");
            int indexOfRight = html.GetIndex(">");
            //aaaa
            if (indexOfLeft < 0 && indexOfRight < 0)
                return html.ReplaceWith("&nbsp;", "");
            // href='''>aba
            if (indexOfLeft < 0)
            {
                if (indexOfRight >= html.Length - 1)
                    return "";
                return html.Substring(indexOfRight + 1);
            }
            // abc <b
            if (indexOfRight < 0)
            {
                if (indexOfLeft == 0)
                    return "";
                return html.Substring(0, indexOfLeft);
            }
            //abc<a>cd<
            if (indexOfLeft < indexOfRight)
            {
                string a = "";
                if (indexOfLeft > 0)
                    a = html.Substring(0, indexOfLeft);
                if (indexOfRight >= html.Length - 1)
                    return a;
                string b = html.Substring(indexOfRight + 1);
                if (string.IsNullOrEmpty(b))
                    return a;
                html = (a + b);
            }
            if (indexOfLeft > indexOfRight)
            {
                html = html.Substring(indexOfRight + 1);
            }
            return GetTxtFromHtml(html);
        }
        #endregion

        /// <summary>
        /// 删除关键词映射
        /// </summary>
        /// <param name="keywordIds">关键词Id，多个用分号拼接</param>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto ExcludeKeyword(string keywordIds, string projectId)
        {
            var result = new ResultDto();
            var keyObjIds = GetObjIdListFromStr(keywordIds);
            //循环删除项目内该关键词Id的所有映射
            var builderMap = Builders<Dnl_KeywordMapping>.Filter;
            var colMap = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            foreach (var keyId in keyObjIds)
            {
                //删除关键词映射
                var filterMap = builderMap.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderMap.Eq(x => x.KeywordId, keyId) & builderMap.Eq(x => x.IsDel, false);
                var cateObjIds = colMap.Find(filterMap).Project(x => x.CategoryId).ToList();
                var updateDel = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true },{ "DelAt", DateTime.Now.AddHours(8) } } } };
                colMap.UpdateMany(filterMap, updateDel);
                //更新词组的关键词数
                cateObjIds.Remove(ObjectId.Empty);      //移除所有词分组Id，该分组不计数
                var builderCate = Builders<Dnl_KeywordCategory>.Filter;
                var colCate = MongoDBHelper.Instance.GetDnl_KeywordCategory();
                foreach (var cateId in cateObjIds)
                {
                    var filterCate = builderCate.Eq(x => x._id, cateId) & builderCate.Eq(x => x.IsDel, false);
                    var queryCate = colCate.Find(filterCate).FirstOrDefault();
                    if (queryCate != null)
                    {
                        var count = queryCate.KeywordCount;
                        count--;
                        var updateCount = new UpdateDocument { { "$set", new QueryDocument { { "KeywordCount", count } } } };
                        colCate.UpdateOne(filterCate, updateCount);
                    }
                }
            }
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取搜索关键词
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="projectId"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_KeywordDto> GetBaiduKeyword(string user_id, string projectId, int page, int pagesize)
        {
            //获取该项目所有关键词映射
            var builderMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterMap = builderMap.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderMap.Eq(x => x.IsDel, false);
            var queryMap = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).ToList();
            //获取该项目所有关键词
            var keyObjIds = queryMap.Select(x => x.KeywordId).ToList();
            var filterKey = Builders<Dnl_Keyword>.Filter.In(x => x._id, keyObjIds);
            var queryKey = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).ToList();
            var keywords = new List<Dnl_KeywordDto>();
            foreach (var x in queryKey)
            {
                var key = new Dnl_KeywordDto
                {
                    _id = x._id.ToString(),
                    BotStatus = x.BotStatus_Baidu,
                    CreatedAt = x.CreatedAt,
                    Keyword = x.Keyword,
                    ValLinkCount = x.ValLinkCount_Baidu
                };
                var map = queryMap.Find(s => s.KeywordId.Equals(x._id));
                if (map != null)
                {
                    key.GroupNumber = map.GroupNumber;
                    key.JisuanStatus = map.JisuanStatus;
                    if (map.CreatedAt > DateTime.MinValue)
                        key.CreatedAt = map.CreatedAt;
                    break;
                }
                keywords.Add(key);
            }
            //按添加时间降序排列
            keywords = keywords.OrderByDescending(x => x.CreatedAt).ToList();
            return keywords;
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
            if (string.IsNullOrEmpty(keywords))
            {
                result.Message = "过滤词不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(commendKeywordId))
            {
                result.Message = "关键词不能为空";
                return result;
            }
            var builder = Builders<IW2S_KeywordFilter>.Filter;      //字符串转数组

            var keywordList = GetIdListFromStr(keywords);
            var col = MongoDBHelper.Instance.GetIW2S_KeywordFilters();
            var usrObjId = new ObjectId(user_id);

            foreach (var keyword in keywordList)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                    filter &= builder.Eq(x => x.Keyword, keyword);
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
                        CommendKeywordId = new ObjectId(commendKeywordId),
                        IsDel = false,
                        Keyword = keyword,
                        ProjectId = new ObjectId(projectId)
                    };
                    kw.UsrId = usrObjId;
                    col.InsertOne(kw);

                    //检查该关键词数据，获取筛选出的链接Id
                    var linkBuilder = Builders<Dnl_Link_Baidu>.Filter;
                    var linkFilter = linkBuilder.Eq(x => x.SearchkeywordId, commendKeywordId);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keyword + ".*/")));
                    var linkObjIds = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(linkFilter).Project(x => x._id).ToList();

                    //为这些链接添加映射
                    foreach (var x in linkObjIds)
                    {
                        SetLinkStatus(projectId, x.ToString(), 2);
                    }
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

                    //检查该关键词数据，获取筛选出的链接Id
                    var linkBuilder = Builders<Dnl_Link_Baidu>.Filter;
                    var linkFilter = linkBuilder.Eq(x => x.SearchkeywordId, keywordFilter.CommendKeywordId);
                    linkFilter &= (linkBuilder.Regex(x => x.Title, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Domain, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")) |
                        linkBuilder.Regex(x => x.Description, new BsonRegularExpression("/.*" + keywordFilter.Keyword + ".*/")));
                    var linkObjIds = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(linkFilter).Project(x => x._id).ToList();

                    //为这些链接添加映射
                    foreach (var x in linkObjIds)
                    {
                        SetLinkStatus(keywordFilter.ProjectId, x.ToString(), 0);
                    }

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

        #endregion

        #region 链接
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
        public QueryResult<Dnl_Link_BaiduDto> GetLevelLinks(string user_id, string projectId, string categoryId, string keywordId, string Title, string domain, string infriLawCode, byte? status, int page, int pagesize)
        {
            var result = new QueryResult<Dnl_Link_BaiduDto>();

            //获取要查询所有关键词Id
            var keyIds = new List<string>();
            //判断是否有选定关键词
            if (!string.IsNullOrEmpty(keywordId))
            {
                keyIds = GetIdListFromStr(keywordId);
            }
            else
            {
                var builderMap = Builders<Dnl_KeywordMapping>.Filter;
                var filterMap = builderMap.Eq(x => x.IsDel, false);
                var colMap = MongoDBHelper.Instance.GetDnl_KeywordMapping();
                //判断是否有选定分组
                if (!string.IsNullOrEmpty(categoryId))
                {
                    //获取所有选定分组内关键词Id
                    var cateObjIds = GetObjIdListFromStr(categoryId);
                    filterMap &= builderMap.In(x => x.CategoryId, cateObjIds);
                }
                else
                {
                    if (!string.IsNullOrEmpty(projectId))
                    {
                        //获取当前项目内关键词
                        filterMap &= builderMap.Eq(x => x.ProjectId, new ObjectId(projectId));
                    }
                }
                keyIds = colMap.Find(filterMap).Project(x => x.KeywordId.ToString()).ToList().Distinct().ToList();
            }
            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
            if (!string.IsNullOrEmpty(Title))
            {
                filterLink &= builderLink.Regex(x => x.Title, new BsonRegularExpression("/.*" + Title + ".*/"));
            }

            if (!string.IsNullOrEmpty(domain))
            {
                filterLink &= builderLink.Regex(x => x.Domain, new BsonRegularExpression("/.*" + domain + ".*/"));
            }

            //获取链接映射信息，判断清洗状态和数据类型
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(projectId));
            var colLinkMap = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu();
            var queryLinkMap = colLinkMap.Find(filterLinkMap).Project(x => new
            {
                LinkId = x.LinkId.ToString(),
                DataStatus = x.DataCleanStatus,
                InfriLawCode = x.InfriLawCode.ToString()
            }).ToList();        //项目下所有链接映射
            var linkObjIds = new List<ObjectId>();          //需要获取的链接Id
            var excludeObjIds = new List<ObjectId>();          //需要排除的链接Id
            //判断是否获取某侵权类型链接
            if (!string.IsNullOrEmpty(infriLawCode))
            {
                var infriIds = GetIdListFromStr(infriLawCode);
                //判断是否获取某种清洗状态的链接
                if (status.HasValue && status.Value > 0)
                {
                    linkObjIds = queryLinkMap.Where(x => x.DataStatus == status && infriIds.Contains(x.InfriLawCode)).Select(x => new ObjectId(x.LinkId)).ToList();
                }
                else
                {
                    linkObjIds = queryLinkMap.Where(x => infriIds.Contains(x.InfriLawCode)).Select(x => new ObjectId(x.LinkId)).ToList();
                    //获取未被删除的链接
                    excludeObjIds = queryLinkMap.Where(x => x.DataStatus == 2).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.Nin(x => x._id, excludeObjIds);
                }
                filterLink &= builderLink.In(x => x._id, linkObjIds);
            }
            else
            {
                //判断是否获取收藏或排除的链接
                if (!status.HasValue || (status.HasValue && status.Value == 0))
                {
                    //获取未被删除的链接
                    excludeObjIds = queryLinkMap.Where(x => x.DataStatus == 2).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.Nin(x => x._id, excludeObjIds);
                }
                else
                {
                    //只获取该清洗状态的数据
                    linkObjIds = queryLinkMap.Where(x => x.DataStatus == status).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.In(x => x._id, linkObjIds);
                }
            }

            var colLink = MongoDBHelper.Instance.GetDnl_Link_Baidu();
            var queryLink = colLink.Find(filterLink).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                CreatedAt = x.CreatedAt,
                InfriLawCodeStr = "",
                PublishTime = x.PublishTime
            }).ToList();

            var linkList = new List<Dnl_Link_BaiduDto>();      //最后输出的链接列表
            foreach (var x in queryLink)
            {
                DateTime tpt = new DateTime();    //临时时间，转换格式时使用
                var link = new Dnl_Link_BaiduDto
                {
                    _id = x._id,
                    Title = x.Title,
                    Description = x.Description,
                    Keywords = x.Keywords,
                    Domain = x.Domain,
                    LinkUrl = x.LinkUrl,
                    CreatedAt = x.CreatedAt,
                    InfriLawCodeStr = null,
                };
                var map = queryLinkMap.Find(s => s.LinkId == x._id);
                if (map != null)
                {
                    link.DataCleanStatus = map.DataStatus;
                    link.InfriLawCode = map.InfriLawCode;
                }

                DateTime.TryParse(x.PublishTime, out tpt);
                link.PublishTime = tpt;
                linkList.Add(link);
            }
            linkList = linkList.OrderByDescending(x => x.PublishTime).ToList();
            result.Count = linkList.Count;
            linkList = linkList.Skip((page) * pagesize).Take(pagesize).ToList();
            result.Result = linkList;
            return result;
        }

        //获取监测结果页面搜索链接数据
        [HttpPost]
        public QueryResultView<Dnl_Link_BaiduDto> GetLevelLinksView(LinkSearchData linkSearchData)
        {
            var result = new QueryResultView<Dnl_Link_BaiduDto>();
            //获取要查询所有关键词Id
            var keyIds = new List<string>();
            string lastKeyword = "";        //单独查询最后一个关键词(如果有）
            //判断是否有选定关键词
            if (!string.IsNullOrEmpty(linkSearchData.keywordId))
            {
                keyIds = GetIdListFromStr(linkSearchData.keywordId);
                lastKeyword = keyIds.Last();
            }
            else
            {
                var builderMap = Builders<Dnl_KeywordMapping>.Filter;
                var filterMap = builderMap.Eq(x => x.IsDel, false);
                var colMap = MongoDBHelper.Instance.GetDnl_KeywordMapping();
                if (!string.IsNullOrEmpty(linkSearchData.projectId))
                {
                    //获取当前项目内关键词
                    filterMap &= builderMap.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));
                }
                keyIds = colMap.Find(filterMap).Project(x => x.KeywordId.ToString()).ToList().Distinct().ToList();
            }

            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
            if (!string.IsNullOrEmpty(linkSearchData.Title))
            {
                //模糊查询
                filterLink &= builderLink.Regex(x => x.Title, new BsonRegularExpression("/.*" + linkSearchData.Title + ".*/"));
            }

            if (!string.IsNullOrEmpty(linkSearchData.domain))
            {
                filterLink &= builderLink.Regex(x => x.Domain, new BsonRegularExpression("/.*" + linkSearchData.domain + ".*/"));
            }

            //获取链接映射信息，判断清洗状态和数据类型
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(linkSearchData.projectId));
            var colLinkMap = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu();
            var queryLinkMap = colLinkMap.Find(filterLinkMap).Project(x => new
            {
                LinkId = x.LinkId.ToString(),
                DataStatus = x.DataCleanStatus,
                InfriLawCode = x.InfriLawCode.ToString()
            }).ToList();
            var linkObjIds = new List<ObjectId>();          //需要获取的链接Id
            var excludeObjIds = new List<ObjectId>();          //需要排除的链接Id
            //判断是否获取某侵权类型链接
            if (!string.IsNullOrEmpty(linkSearchData.infriLawCode))
            {
                var infriIds = GetIdListFromStr(linkSearchData.infriLawCode);
                //判断是否获取某种清洗状态的链接
                if (linkSearchData.status.HasValue && linkSearchData.status.Value > 0)
                {
                    linkObjIds = queryLinkMap.Where(x => x.DataStatus == linkSearchData.status && infriIds.Contains(x.InfriLawCode)).Select(x => new ObjectId(x.LinkId)).ToList();
                }
                else
                {
                    linkObjIds = queryLinkMap.Where(x => infriIds.Contains(x.InfriLawCode)).Select(x => new ObjectId(x.LinkId)).ToList();
                    //获取未被删除的链接
                    excludeObjIds = queryLinkMap.Where(x => x.DataStatus == 2).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.Nin(x => x._id, excludeObjIds);
                }
                filterLink &= builderLink.In(x => x._id, linkObjIds);
            }
            else
            {
                //判断是否获取收藏或排除的链接
                if (!linkSearchData.status.HasValue || (linkSearchData.status.HasValue && linkSearchData.status.Value == 0))
                {
                    //获取未被删除的链接
                    excludeObjIds = queryLinkMap.Where(x => x.DataStatus == 2).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.Nin(x => x._id, excludeObjIds);
                }
                else
                {
                    //只获取该清洗状态的数据
                    linkObjIds = queryLinkMap.Where(x => x.DataStatus == linkSearchData.status).Select(x => new ObjectId(x.LinkId)).ToList();
                    filterLink &= builderLink.In(x => x._id, linkObjIds);
                }
            }

            var colLink = MongoDBHelper.Instance.GetDnl_Link_Baidu();
            var queryLink = colLink.Find(filterLink).Project(x => new
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                CreatedAt = x.CreatedAt,
                InfriLawCodeStr = "",
                PublishTime = x.PublishTime
            }).ToList();

            var linkList = new List<Dnl_Link_BaiduDto>();      //最后输出的链接列表
            foreach (var x in queryLink)
            {
                DateTime tpt = new DateTime();    //临时时间，转换格式时使用
                var link = new Dnl_Link_BaiduDto
                {
                    _id = x._id,
                    Title = x.Title,
                    Description = x.Description,
                    Keywords = x.Keywords,
                    Domain = x.Domain,
                    LinkUrl = x.LinkUrl,
                    CreatedAt = x.CreatedAt,
                    InfriLawCodeStr = null,
                };
                var map = queryLinkMap.Find(s => s.LinkId == x._id);
                if (map != null)
                {
                    link.DataCleanStatus = map.DataStatus;
                    link.InfriLawCode = map.InfriLawCode;
                }

                DateTime.TryParse(x.PublishTime, out tpt);
                link.PublishTime = tpt;
                linkList.Add(link);
            }
            linkList = linkList.OrderByDescending(x => x.PublishTime).ToList();
            result.Count = linkList.Count;
            linkList = linkList.Skip((linkSearchData.page) * linkSearchData.pagesize).Take(linkSearchData.pagesize).ToList();

            //如果类型存在，则查询类型名称
            if (!string.IsNullOrEmpty(linkSearchData.infriLawCode)&&!linkSearchData.infriLawCode.Equals("000000000000000000000000"))
            {
                foreach (var list in linkList)
                {
                    var builderInfri = Builders<IW2S_AnalysisItemValue>.Filter.Eq(x => x._id, new ObjectId(list.InfriLawCode));
                    string InfriName = MongoDBHelper.Instance.GetIW2S_AnalysisItemValues().Find(builderInfri).Project(x => x.Name).FirstOrDefault();
                    list.InfriLawCodeStr = InfriName;
                }
            }

            result.Result = linkList;

            bool hasvalue = true;
            //判断最后一个关键词是否有数据
            if (!string.IsNullOrEmpty(lastKeyword))
            {
                var filterLast = filterLink;
                filterLast &= builderLink.Eq(x => x.SearchkeywordId, lastKeyword);

                long LastCount = colLink.Find(filterLast).Count();
                if (LastCount <= 0)
                {
                    hasvalue = false;
                }
            }

            result.HasValue = hasvalue;
            result.infriLawCode = linkSearchData.infriLawCode;
            return result;
        }

        /// <summary>
        /// 设置链接状态
        /// </summary>
        /// <param name="projectId">用户id</param>
        /// <param name="id"></param>
        /// <param name="status">1,收藏;2,排除</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetLinkStatus(string projectId, string id, byte? status)
        {
            ResultDto result = new ResultDto();
            var proObjId = new ObjectId(projectId);
            var filterUser = Builders<IW2S_Project>.Filter.Eq(x => x._id, proObjId);
            var userObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterUser).Project(x => x.UsrId).FirstOrDefault();
            
            var linkObjIds = GetObjIdListFromStr(id);

            //查询是否已经存在这些链接Id的映射
            var builder = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filter = builder.In(x => x.LinkId, linkObjIds) & builder.Eq(x => x.ProjectId, proObjId);
            var col = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu();
            var query = col.Find(filter).Project(x => x.LinkId).ToList();
            foreach (var objId in linkObjIds)
            {
                /* 存在则更新记录
                 * 不存在则创建记录 */
                if (query.Contains(objId))
                {
                    filter = builder.Eq(x => x.LinkId, objId) & builder.Eq(x => x.ProjectId, proObjId);
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "DataCleanStatus", status } } } };
                    col.UpdateOne(filter, update);
                }
                else
                {
                    var linkMap = new Dnl_LinkMapping_Baidu
                    {
                        LinkId = objId,
                        DataCleanStatus = status,
                        ProjectId = proObjId,
                        UserId = userObjId
                    };
                    col.InsertOne(linkMap);
                }
            }


            if (status == 1)
            {
                IW2S_OperateLog log = new IW2S_OperateLog
                {
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProjectId = proObjId,
                    ShareOperateType = (int)ShareOperateType.CollectConfig,
                    UserId = userObjId
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }

            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 设置链接类型
        /// </summary>
        /// <param name="projectId">项目id</param>
        /// <param name="id">链接Id，多个时用分号相连</param>
        /// <param name="infriType">链接类型</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SetLinkInfriType(string projectId, string id, string infriType)
        {
            ResultDto result = new ResultDto();
            try
            {
                var proObjId = new ObjectId(projectId);
                var filterUser = Builders<IW2S_Project>.Filter.Eq(x => x._id, proObjId);
                var userObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterUser).Project(x => x.UsrId).FirstOrDefault();
                ObjectId lawCodeId = ObjectId.Empty;
                ObjectId.TryParse(infriType, out lawCodeId);
                var objIds = GetObjIdListFromStr(id);

                //查询已经存在的链接Id的映射
                var builder = Builders<Dnl_LinkMapping_Baidu>.Filter;
                var filter = builder.In(x => x.LinkId, objIds);
                var col = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu();
                var query = col.Find(filter).Project(x => x.LinkId).ToList();
                foreach (var x in objIds)
                {
                    /* 存在则更新记录
                     * 不存在则创建记录 */
                    if (query.Contains(x))
                    {
                        var filterUpdate = builder.Eq(s => s.LinkId, x);
                        var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", lawCodeId } } } };
                        col.UpdateOne(filterUpdate, update);
                    }
                    else
                    {
                        var linkMap = new Dnl_LinkMapping_Baidu
                        {
                            LinkId = x,
                            InfriLawCode = lawCodeId,
                            ProjectId = proObjId,
                            UserId = userObjId,
                            Source = SourceType.Enginee,
                        };
                        col.InsertOne(linkMap);
                    }
                }

                if (objIds.Count > 0)
                {
                    IW2S_OperateLog log = new IW2S_OperateLog
                    {
                        CreatedAt = DateTime.Now.AddHours(8),
                        ProjectId = proObjId,
                        ShareOperateType = (int)ShareOperateType.SetLinkAnalysisItem,
                        UserId = userObjId
                    };
                    MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
                }

                result.IsSuccess = true;
            }
            catch
            {
                result.IsSuccess = false;
                result.Message = "网络打盹了，请稍后。。。！";
            }
            return result;
        }

        /// <summary>
        /// 获取链接正文
        /// </summary>
        /// <param name="linkId">链接Id</param>
        /// <returns></returns>
        public string GetLinkContent(string linkId)
        {
            var builder = Builders<Dnl_Link_Baidu>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(linkId));
            var col = MongoDBHelper.Instance.GetDnl_Link_Baidu();
            var wordTree = new TextTree();
            string content = "";
            try
            {
                var query = col.Find(filter).FirstOrDefault();
                if (!string.IsNullOrEmpty(query.Content))
                {
                    wordTree.TreeValues= query.Content;
                }
                else
                {
                    wordTree.TreeValues = GetMainContentHelper.GetMainContent(query.Html);
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            JiebaController jieba = new JiebaController();
            content = jieba.WordSegemete(wordTree).TreeValues;
            return content;
        }
        #endregion

        #region 关键词组
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
            var categoryBuilder = Builders<Dnl_KeywordMapping>.Filter;
            FilterDefinition<Dnl_KeywordMapping> categoryfilter = null;

            if (!string.IsNullOrEmpty(categoryId))
            {
                categoryfilter = categoryBuilder.In(x => x.CategoryId, GetObjIdListFromStr(categoryId));
            }
            else if (!string.IsNullOrEmpty(prjId) && isgroup)
            {
                categoryfilter = categoryBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
            }
            var keywordBuilder = Builders<Dnl_Keyword>.Filter;
            var keywordcol = MongoDBHelper.Instance.GetDnl_Keyword();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "BotStatus_Baidu", 0 } } } };

            var prjObjId = ObjectId.Empty;
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            if (categoryfilter != null)
            {
                var keywordIds = categoryCol.Find(categoryfilter).Project(x => x.KeywordId).ToList();

                var keywordFilter = keywordBuilder.In(x => x._id, keywordIds);
                keywordcol.UpdateMany(keywordFilter, update);

                prjObjId = categoryCol.Find(categoryfilter).Project(x => x.ProjectId).FirstOrDefault();
            }
            else
            {
                if (!string.IsNullOrEmpty(prjId) && !isgroup)
                {
                    categoryfilter = categoryBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
                    var keywordIds = categoryCol.Find(categoryfilter).Project(x => x.KeywordId).ToList();
                    var keywordFilter = keywordBuilder.In(x => x._id, keywordIds);
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
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var keywordMapCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;

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
                categoryDto = new Dnl_KeywordCategory
                {
                    _id = ObjectId.GenerateNewId(),
                    //GroupType = groupType,
                    Name = sss.groupName,
                    //InfriLawCode = lawCode,
                    IsDel = false,
                    UserId = usrObjId,
                    Weight = sss.weight,
                    ProjectId = new ObjectId(sss.projectId),
                    GroupNumber = 0,
                    CreatedAt=DateTime.Now.AddHours(8),
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

                categoryCol.InsertOne(categoryDto);
            }
            if (!string.IsNullOrEmpty(sss.keywordIds))
            {
                int keywordCount = 0;
                var keywordBuilder = Builders<Dnl_Keyword>.Filter;
                var keywordCol = MongoDBHelper.Instance.GetDnl_Keyword();
                foreach (string keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {

                        var keywordFilter = keywordBuilder.Eq(x => x._id, new ObjectId(keyId));
                        var keyword = keywordCol.Find(keywordFilter).Project(x => x.Keyword).FirstOrDefault();
                        if (keyword != null)
                        {
                            var map = new Dnl_KeywordMapping
                            {
                                //GroupType = groupType,
                                KeywordId = new ObjectId(keyId),
                                Keyword = keyword,
                                CategoryId = categoryDto._id,
                                ParentCategoryId = categoryDto.ParentId,
                                IsDel = false,
                                UserId = usrObjId,
                                ProjectId = new ObjectId(sss.projectId),
                                CreatedAt = DateTime.Now.AddHours(8),
                            };
                            keywordMapCol.InsertOne(map);
                            keywordCount++;
                        }
                    }
                }
                var update = new UpdateDocument { { "$set", new QueryDocument { { "KeywordCount", keywordCount } } } };
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
        public ResultDto UpdateKeywordGroup(kgvo sss)
        {
            ResultDto result = new ResultDto();

            if (string.IsNullOrEmpty(sss.groupid))
            {
                result.Message = "修改的分组不能为空";
                return result;
            }
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var keywordMapCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x._id, new ObjectId(sss.groupid));

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();

            if (categoryDto == null)
            {
                result.Message = "修改的分组不能为空";
                return result;
            }

            var mapbuilder = Builders<Dnl_KeywordMapping>.Filter;

            var mapfilter = mapbuilder.Eq(x => x.CategoryId, categoryDto._id);

            ObjectId parentId = new ObjectId("000000000000000000000000");
            if (!string.IsNullOrEmpty(sss.parentGroupId))
            {
                parentId = new ObjectId(sss.parentGroupId);
            }

            bool isChangeParent = parentId != categoryDto.ParentId;     //是否变更分组归属
            var alloldCommendIds = keywordMapCol.Find(mapfilter).Project(x => x.KeywordId).ToList();        //分组内原有关键词Id列表
            //清除分组内原有关键词映射
            keywordMapCol.DeleteMany(mapfilter);
            var keywordIdList = sss.keywordIds.Split(';', '；', ',');

            int keywordCount = 0;
            bool isLevel1 = categoryDto.ParentId.Equals(new ObjectId("000000000000000000000000")) ? true : false;
            if (!string.IsNullOrEmpty(sss.keywordIds))
            {
                var builderKey = Builders<Dnl_Keyword>.Filter;
                foreach (var keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {
                        //alloldCommendIds.Remove(new ObjectId(keyId));
                        //插入新关键词映射
                        var map = new Dnl_KeywordMapping
                        {
                            //GroupType = categoryDto.GroupType,
                            KeywordId = new ObjectId(keyId),
                            ParentCategoryId = parentId,//categoryDto.ParentId,
                            CategoryId = categoryDto._id,
                            IsDel = false,
                            UserId = categoryDto.UserId,
                            ProjectId = categoryDto.ProjectId,
                            CreatedAt = DateTime.Now.AddHours(8),
                        };

                        var filterKey = builderKey.Eq(x => x._id, new ObjectId(keyId));
                        var keywordStr = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => x.Keyword).FirstOrDefault();
                        if (string.IsNullOrEmpty(keywordStr))
                        {
                            continue;
                        }
                        map.Keyword = keywordStr;

                        keywordMapCol.InsertOne(map);
                        keywordCount++;
                        if (isLevel1)
                        {
                            UpdateLinksInfriType(keyId, sss.lawCode, categoryDto.UserId.ToString(),categoryDto.ProjectId.ToString());
                        }
                    }
                }
            }
            //更新分组信息
            var infriLawCode = new ObjectId(sss.lawCode);
            if (string.IsNullOrEmpty(sss.lawCode))
            {
                infriLawCode = new ObjectId("000000000000000000000000");
            }
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", sss.groupName }, { "ParentId", parentId }, { "InfriLawCode", infriLawCode }, { "Weight", sss.weight }, { "KeywordCount", keywordCount } } } };
            categoryCol.UpdateOne(categoryfilter, update);

            if (alloldCommendIds.Count > 0)
            {
                //父组变更时将原有所有父组内删除原有关键词，并往新父组内添加这些关键词
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
                            UpdateLinksInfriType(alloldCommendId.ToString(), "000000000000000000000000", categoryDto.UserId.ToString(), categoryDto.ProjectId.ToString());
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
            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;
            var categoryfilter = categorybuilder.Eq(x => x.ParentId, parentId);
            var updateca = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", lawCode } } } };
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            categoryCol.UpdateOne(categoryfilter, updateca);

            categoryfilter = categorybuilder.Eq(x => x.ParentId, parentId);

            var parentcategoryDtos = categoryCol.Find(categoryfilter).ToList();
            foreach (var parentcategoryDto in parentcategoryDtos)
            {
                RecurseUpdateSubInfriType(parentcategoryDto._id, lawCode);
            }
        }

        private void UpdateLinksInfriType(string searchKeywordId, string lawCode,string userId,string projectId)
        {
            //获取关键词对应的链接ID
            var filterLink = Builders<Dnl_Link_Baidu>.Filter.Eq(x => x.SearchkeywordId, searchKeywordId);
            var linkObjIds = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => x._id).ToList();
            //查询已有链接映射
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var colLinkMap = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu();
            foreach (var id in linkObjIds)
            {
                var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(projectId));
                filterLinkMap &= builderLinkMap.Eq(x => x.LinkId, id);
                var query = colLinkMap.Find(filterLinkMap).FirstOrDefault();
                if (query != null)
                {
                    //更新链接映射
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", new ObjectId(lawCode) } } } };
                    colLinkMap.UpdateOne(filterLinkMap, update);
                }
                else
                {
                    //如果侵权类型为空，跳过
                    if(lawCode=="000000000000000000000000")
                        continue;
                    //插入新链接映射
                    var linkMap = new Dnl_LinkMapping_Baidu
                    {
                        LinkId = id,
                        UserId = new ObjectId(userId),
                        ProjectId = new ObjectId(projectId),
                        InfriLawCode=new ObjectId(lawCode)
                    };
                    colLinkMap.InsertOne(linkMap);
                }
            }
        }

        /// <summary>
        /// 循环删除父分组内原有关键词
        /// </summary>
        /// <param name="categoryDto">当前分组信息</param>
        /// <param name="alloldCommendIds">要删除的关键词Id列表</param>
        private void RecurseDelParentKeyword(Dnl_KeywordCategory categoryDto, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var mapbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var keywordMapCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            var categoryfilter = categorybuilder.Eq(x => x._id, categoryDto.ParentId);

            var parentcategoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            if (parentcategoryDto != null)
            {
                //删除父分组中本分组内所有原关键词
                long delCount = 0;
                var mapfilter = mapbuilder.Eq(x => x.CategoryId, parentcategoryDto._id) & mapbuilder.In(x => x.KeywordId, alloldCommendIds);
                keywordMapCol.DeleteMany(mapfilter);
                //获取当前父分组内关键词数
                mapfilter = mapbuilder.Eq(x => x.CategoryId, parentcategoryDto._id);
                delCount = keywordMapCol.Find(mapfilter).Project(x => x._id).Count();

                var updateca = new UpdateDocument { { "$set", new QueryDocument { { "KeywordCount", delCount } } } };
                categoryfilter = categorybuilder.Eq(x => x._id, parentcategoryDto._id);
                categoryCol.UpdateOne(categoryfilter, updateca);
                //循环删除父词组内相关信息，直到根目录
                RecurseDelParentKeyword(parentcategoryDto, alloldCommendIds);

            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 循环往新父组内添加关键词
        /// </summary>
        /// <param name="parentId">父组Id</param>
        /// <param name="alloldCommendIds">要添加的关键词Id列表</param>
        private void RecurseAddParentKeyword(ObjectId parentId, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var mapbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var keywordGroupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            var categoryfilter = categorybuilder.Eq(x => x._id, parentId);
            //获取父组信息
            var parentcategoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();
            if (parentcategoryDto != null)
            {
                var builderKey = Builders<Dnl_Keyword>.Filter;
                foreach (var keyId in alloldCommendIds)
                {

                    //alloldCommendIds.Remove(keyId);

                    var mapDto = new Dnl_KeywordMapping
                    {
                        //GroupType = categoryDto.GroupType,
                        KeywordId = keyId,
                        ParentCategoryId = parentcategoryDto.ParentId,
                        CategoryId = parentId,
                        IsDel = false,
                        UserId = parentcategoryDto.UserId,
                        ProjectId = parentcategoryDto.ProjectId,
                        CreatedAt = DateTime.Now.AddHours(8),
                    };

                    var filterKey = builderKey.Eq(x => x._id, keyId);
                    var keywordStr = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => x.Keyword).FirstOrDefault();
                    if (string.IsNullOrEmpty(keywordStr))
                    {
                        continue;
                    }
                    mapDto.Keyword = keywordStr;

                    keywordGroupCol.InsertOne(mapDto);
                }
                //获取父组当前关键词数
                var mapfilter = mapbuilder.Eq(x => x.CategoryId, parentcategoryDto._id);
                var delCount = keywordGroupCol.Find(mapfilter).Project(x => x._id).Count();

                var updateca = new UpdateDocument { { "$set", new QueryDocument { { "KeywordCount", delCount } } } };
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
        /// <param name="user_id">分组Id</param>
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
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var categorybuilder = Builders<Dnl_KeywordCategory>.Filter;
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

        /// <summary>
        /// 获取所有关键词组（不包含所有词）
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_KeywordCategoryDto> GetAllKeywordCategory(string user_id, string projectId)
        {
            List<Dnl_KeywordCategoryDto> result = new List<Dnl_KeywordCategoryDto>();
            //获取所有分组
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id))
            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).SortBy(x => x.ParentId).ToList();

            var mapbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var mapfilter = mapbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//mapbuilder.Eq(x => x.UsrId, new ObjectId(user_id));

            var keywordBuilder = Builders<Dnl_Keyword>.Filter;
            var keywordCol = MongoDBHelper.Instance.GetDnl_Keyword();
            FilterDefinition<Dnl_Keyword> keywordFilter = null;
            ObjectId nullId = new ObjectId("000000000000000000000000");

            foreach (var item in TaskList)
            {
                var v = new Dnl_KeywordCategoryDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.ParentId = item.ParentId.ToString();
                v.Weight = item.Weight;
                v.InfriLawCode = item.InfriLawCode.ToString();
                //v.InfriLawCodeStr = CommonHelper.GetLawCodeStr(v.InfriLawCode);
                v.KeywordTotal = item.KeywordCount;
                //获取分组内所有关键词ID
                mapfilter &= mapbuilder.Eq(x => x.IsDel, false);
                mapfilter &= mapbuilder.Eq(x => x.CategoryId, item._id);
                var selectedIdList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mapfilter).Project(x => x.KeywordId).ToList();
                //获取还未搜索的关键词
                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.BotStatus_Baidu, 0);
                var unkeyword = keywordCol.Find(keywordFilter).Project(x => x._id).FirstOrDefault();
                //获取正在搜索的关键词
                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.BotStatus_Baidu, 1);
                var prokeyword = keywordCol.Find(keywordFilter).Project(x => x._id).FirstOrDefault();
                //如果所有关键词都搜索完成，则认为词组已搜索完成
                if (unkeyword == nullId && prokeyword == nullId)
                {
                    v.BotStatus = 2;
                }
                else if (prokeyword != nullId)  //如果有关键词正在搜索，则认为词组也正在搜索
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
        /// 获取关键词分组(包含所有词）
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="groupType">1:百度直搜，2：百度热词</param>
        /// <param name="keywordId">关键词ID</param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_KeywordCategoryDto> GetKeywordCategory(string user_id, string projectId, int groupType, string keywordId)
        {
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(user_id)) &
            //if (!string.IsNullOrEmpty(keywordId))
            //{
            //    filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            //}
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).SortBy(x => x.ParentId).ToList();

            List<Dnl_KeywordCategoryDto> list = new List<Dnl_KeywordCategoryDto>();

            Dictionary<string, string> dicCategoryIDName = new Dictionary<string, string>();
            //获取所有关键词数
            var commendbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var commendfilter = commendbuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) & commendbuilder.Eq(x => x.IsDel, false); //commendbuilder.Eq(x => x.UsrId, new ObjectId(user_id)) & 
            commendfilter &= commendbuilder.Eq(x => x.CategoryId, ObjectId.Empty);
            var keywordCount = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(commendfilter).Project(x => x._id).Count();
            //添加所有词信息
            list.Add(new Dnl_KeywordCategoryDto
            {
                _id = "",
                Name = "所有词",
                KeywordTotal = (int)keywordCount
            });
            //添加其余关键词分组信息
            foreach (var item in TaskList)
            {
                Dnl_KeywordCategoryDto v = new Dnl_KeywordCategoryDto();
                v._id = item._id.ToString();
                //v.KeywordId = item.KeywordId.ToString();
                v.Name = item.Name;
                v.ParentId = item.ParentId.ToString();
                v.Weight = item.Weight;
                v.InfriLawCode = item.InfriLawCode.ToString();
                //v.InfriLawCodeStr = CommonHelper.GetLawCodeStr(v.InfriLawCode);
                v.KeywordTotal = item.KeywordCount;

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
        /// <param name="projectId"></param>
        /// <param name="groupid">父组Id，为空时从所有词获取可用关键词</param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_KeywordDto> GetKeywordGroup(string usr_id, string projectId, string groupid)
        {
            List<Dnl_KeywordDto> list = new List<Dnl_KeywordDto>();

            var mapbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var mapfilter = mapbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//mapbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            mapfilter &= mapbuilder.Eq(x => x.IsDel, false);
            /* groupid不为空时表示从已有分组进行进一步分组
             * groupid为空时表示从所有词中进行分组 */
            if (!string.IsNullOrEmpty(groupid))
            {
                //获取当前分组的次级分组的关键词
                mapfilter &= mapbuilder.Eq(x => x.ParentCategoryId, new ObjectId(groupid));
            }
            else
            {
                //获取所有第一级分组内的关键词
                var builderCate = Builders<Dnl_KeywordCategory>.Filter;
                var filterCate = builderCate.Eq(x => x.ParentId, ObjectId.Empty) & builderCate.Eq(x => x.ProjectId, new ObjectId(projectId));
                var cateObjIds = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id).ToList();
                mapfilter &= mapbuilder.In(x => x.CategoryId, cateObjIds);
            }
            //获取已被次级分组使用的关键词ID
            var selectedIdList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mapfilter).Project(x => x.KeywordId).ToList();
            //去重
            selectedIdList = selectedIdList.Distinct().ToList();
            //获取分组下所有关键词
            List<Dnl_KeywordDto> allKeywords = new List<Dnl_KeywordDto>();
            mapfilter = mapbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//mapbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            mapfilter &= mapbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(groupid))
            {
                //获取分组内所有关键词
                mapfilter &= mapbuilder.Eq(x => x.CategoryId, new ObjectId(groupid));
            }
            else
            {
                //如果分组id为空，则获取所有词
                mapfilter &= mapbuilder.Eq(x => x.CategoryId, ObjectId.Empty);
            }
            allKeywords = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mapfilter)
                    .Project(x => new Dnl_KeywordDto
                    {
                        _id = x.KeywordId.ToString(),
                        Keyword = x.Keyword,
                        CreatedAt = x.CreatedAt,
                        drag = true
                    }
                    ).ToList();
            //去除已被使用关键词
            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).OrderByDescending(x=>x.CreatedAt).ToList();
            return list;
        }

        /// <summary>
        /// 获取修改分组时当前分组和父分组关键词
        /// </summary>
        /// <param name="usr_id">用户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="groupid">要修改的分组id</param>
        /// <param name="parentid">要修改的分组id的父组id</param>
        /// <param name="keywordId">废弃</param>
        /// <param name="groupType">废弃</param>
        /// <returns></returns>
        [HttpGet]
        public KeywordMappingModelDto GetEditKeywordGroup(string usr_id, string projectId, string groupid, string parentid, string keywordId, int groupType)
        {
            KeywordMappingModelDto result = new KeywordMappingModelDto();
            List<Dnl_KeywordDto> list = new List<Dnl_KeywordDto>();     //可用关键词列表
            var groupbuilder = Builders<Dnl_KeywordMapping>.Filter;

            //获取当前项目下父分组内已被使用关键词id列表
            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter &= groupbuilder.Eq(x => x.ParentCategoryId, new ObjectId(parentid));
            }
            else
            {
                //获取所有第一级分组内的关键词
                var filterCate = Builders<Dnl_KeywordCategory>.Filter.Eq(x => x.ParentId, ObjectId.Empty);
                var cateObjIds = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id).ToList();
                groupfilter &= groupbuilder.In(x => x.CategoryId, cateObjIds);
            }
            //获取已被次级分组使用的关键词ID
            var selectedIdList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter).Project(x => x.KeywordId).ToList();
            //去重
            selectedIdList = selectedIdList.Distinct().ToList();

            List<Dnl_KeywordDto> allKeywords = new List<Dnl_KeywordDto>();      //父分组内所有关键词列表
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                //如果父分组不为空，从分组表获取数据
                groupfilter &= groupbuilder.Eq(x => x.CategoryId, new ObjectId(parentid));
            }
            else if (string.IsNullOrEmpty(parentid) || parentid == "000000000000000000000000")
            {
                //获取项目下所有关键词
                groupfilter =  groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId))&groupbuilder.Eq(x => x.IsDel, false)&groupbuilder.Eq(x => x.CategoryId, ObjectId.Empty);
                
            }
            allKeywords = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter)
                    .Project(x => new Dnl_KeywordDto
                    {
                        _id = x.KeywordId.ToString(),
                        Keyword = x.Keyword,
                        CreatedAt = x.CreatedAt,
                        drag = true
                    }
                    ).ToList();

            //删除已被分配关键词
            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).ToList();

            List<Dnl_KeywordDto> curSelectedList = new List<Dnl_KeywordDto>();
            //如果不是新建分组，获取原分组内已被使用关键词
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));//groupbuilder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CategoryId, new ObjectId(groupid));
                curSelectedList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter)
                    .Project(x => new Dnl_KeywordDto
                    {
                        _id = x.KeywordId.ToString(),
                        Keyword = x.Keyword,
                        CreatedAt = x.CreatedAt,
                        drag = true
                    }
                    ).ToList();
            }

            result.Selected = curSelectedList.OrderByDescending(x => x.CreatedAt).ToList();
            result.UnSelected = list.OrderByDescending(x => x.CreatedAt).ToList();
            return result;
        }

        /// <summary>
        /// 删除关键词组
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelKeywordCategory(string user_id,string categoryId)
        {
            ResultDto result = new ResultDto();
            //删除关键词组
            if (string.IsNullOrEmpty(categoryId))
            {
                return result;
            }
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var prjId = categoryCol.Find(builder.Eq(x => x._id, new ObjectId(categoryId))).Project(x => x.ProjectId).FirstOrDefault();
            categoryCol.DeleteOne(builder.Eq(x => x._id, new ObjectId(categoryId)));            
            //删除词组内关键词
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.CategoryId, new ObjectId(categoryId));
            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            groupCol.DeleteMany(groupFilter);
            //获取子分组Id
            var filter = builder.Eq(x => x.ParentId, new ObjectId(categoryId));
            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            if (TaskList.Count > 0)
            {
                //删除子分组
                categoryCol.DeleteMany(builder.In(x => x._id, TaskList));
                groupFilter = groupBuilder.In(x => x.CategoryId, TaskList);
                //删除子分组关键词
                groupCol.DeleteMany(groupFilter);
                //循环删除子分组
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
        /// <summary>
        /// 循环删除子分组
        /// </summary>
        /// <param name="categoryId">分组Id</param>
        private void RecurseDelCategory(ObjectId categoryId)
        {
            //删除子分组
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, categoryId);
            var categoryCol = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            categoryCol.DeleteMany(builder.In(x => x._id, TaskList));
            //删除子分组关键词
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            var groupFilter = groupBuilder.In(x => x.CategoryId, TaskList);
            groupCol.DeleteMany(groupFilter);

            if (TaskList.Count == 0)
                return;
            foreach (ObjectId delId in TaskList)
            {
                RecurseDelCategory(delId);
            }

        }

        /// <summary>
        /// 获取单个词组内未被删除或已被删除关键词
        /// </summary>
        /// <param name="usr_id">用户ID</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="treeNodeId">分类结点ID</param>
        /// <returns>分类关键词数组</returns>
        [HttpGet]
        public List<GroupKeywordsDto> GetFenleiKeywords(string usr_id, string projectId, string treeNodeId, bool status)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            //获取关键词ID列表
            var builderMapping = Builders<Dnl_KeywordMapping>.Filter;
            var filterMapping = builderMapping.Eq(x => x.ProjectId, new ObjectId(projectId));
            filterMapping &= builderMapping.Eq(x => x.IsDel, status);
            filterMapping &= builderMapping.Eq(x => x.CategoryId, new ObjectId(treeNodeId));
            var keyObjIds = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMapping).Project(x => x.KeywordId).ToList();
            //获取关键词名、Bot状态和有效链接数
            var filterKey = Builders<Dnl_Keyword>.Filter.In(x => x._id, keyObjIds);
            var temp = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => new GroupKeywordsDto
            {
                id = x._id.ToString(),
                name = x.Keyword,
                ValLinkCount = x.ValLinkCount_Baidu,
                BotStatus = x.BotStatus_Baidu
            }).ToList();
            keywordList.AddRange(temp);
            return keywordList;
        }

        //监测结果页面获取多个词组下关键词
        [HttpGet]
        public List<GroupKeywordsDto> GetFenleiKeywordsView(string usr_id, string projectId, string categoryId)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            var cateObjIds = GetObjIdListFromStr(categoryId);
            //获取关键词ID列表
            var builderMapping = Builders<Dnl_KeywordMapping>.Filter;
            var filterMapping = builderMapping.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderMapping.Eq(x => x.IsDel, false);
            filterMapping &= builderMapping.In(x => x.CategoryId, cateObjIds);
            var keyObjIds = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMapping).Project(x => x.KeywordId).ToList();
            //获取关键词名、Bot状态和有效链接数
            var filterKey = Builders<Dnl_Keyword>.Filter.In(x => x._id, keyObjIds);
            var temp = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => new GroupKeywordsDto
            {
                id = x._id.ToString(),
                name = x.Keyword,
                ValLinkCount = x.ValLinkCount_Baidu,
                BotStatus = x.BotStatus_Baidu
            }).ToList();
            keywordList.AddRange(temp);

            return keywordList;
        }

        /// <summary>
        /// 排除关键词组内所有关键词映射
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
                    var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", status }, { "DelAt", DateTime.Now.AddHours(8) } } } };
                    MongoDBHelper.Instance.GetDnl_KeywordMapping().UpdateMany(new QueryDocument { { "KeywordId", id } }, updategroup);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 图表
        //设置矩形树图
        [HttpGet]
        public JsonResultDto GetRectangularTreeUrl(string usr_id, string projectId)
        {
            var result = new JsonResultDto();
            try
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
                var builder = Builders<Dnl_Keyword>.Filter;
                var filter = builder.In(x => x._id, keywordList);
                var taskList = MongoDBHelper.Instance.GetDnl_Keyword().Find(filter).Project(x => new
                {
                    Id = x._id.ToString(),
                    ValLinkCount = x.ValLinkCount_Baidu
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
                RectangularTree root = new RectangularTree();
                root.Id = "000000000000000000000000";
                string text = GetRectanglarTreeList(tree, root);
                text = text.Substring(0, text.Length - 20);
                text += System.Environment.NewLine + "}";
                result.IsSuccess = true;
                result.Json = text;
                //string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
                //string path = folder + "RectangularTree.txt";
                //if (File.Exists(path))
                //{
                //    File.Delete(path);
                //    System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                //    sw.WriteLine(text);
                //    sw.Close();
                //}
                //else
                //{
                //    System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                //    sw.WriteLine(text);
                //    sw.Close();
                //}

                //jsonFileUrlDto json = new jsonFileUrlDto();
                //json.Url = "Scripts/app/data/" + "RectangularTree.txt";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        //获取词组及关键词列表
        private List<RectangularTree> GetRectangularTree(string usr_id, string projectId, ObjectId parentId, List<RectangularTree> list)
        {
            //获取次级词组名
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new RectangularTree
            {
                Id = x._id.ToString(),
                PId = x.ParentId.ToString(),
                Name = x.Name,
                IsNode = true,
                ValLinkCount = 0,
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            groupfilter &= groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var keywordList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter).Project(x => new RectangularTree
            {
                Id = x.KeywordId.ToString(),
                PId = x.CategoryId.ToString(),
                Name = x.Keyword,
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
                filterMapping &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));
            }
            keywordList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMapping).Project(x => x.KeywordId.ToString()).ToList();

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");
            allTimeFilter &= builder.Nin(x => x._id, exLinkObjIds);
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
        public TimeLinkCountDto GetTimeLinkCount(string categoryIds, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval)
        {
            DateTime tpStart = new DateTime();
            DateTime tpEnd = new DateTime();
            DateTime.TryParse(startTime, out tpStart);
            DateTime.TryParse(endTime, out tpEnd);

            TimeLinkCountDto result = new TimeLinkCountDto();
            if (string.IsNullOrEmpty(prjId))
            {
                return null;
            }
            var proObjId = new ObjectId(prjId);

            var cateIds = new List<string>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateIds = CommonHelper.GetIdListFromStr(categoryIds);
                cateIds.Remove(ObjectId.Empty.ToString());
                cateIds.Sort();
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();       //Json序列化与反序列化
            //生成参数Json
            JObject lineFactorJson = new JObject();
            lineFactorJson.Add(new JProperty("categoryIds", string.Join(";", cateIds)));
            lineFactorJson.Add(new JProperty("startTime", startTime));
            lineFactorJson.Add(new JProperty("endTime", endTime));
            lineFactorJson.Add(new JProperty("percent", percent));
            lineFactorJson.Add(new JProperty("topNum", topNum));
            lineFactorJson.Add(new JProperty("sumNum", sumNum));
            lineFactorJson.Add(new JProperty("timeInterval", timeInterval));

            //获取图表数据
            var builderChart = Builders<PojectChartMongo>.Filter;
            var filterChart = builderChart.Eq(x => x.ProjectId, proObjId) & builderChart.Eq(x => x.Type, ChartType.Line);
            filterChart &= builderChart.Eq(x => x.Source, SourceType.Enginee) & builderChart.Eq(x => x.Name, "默认");
            var colChart = MongoDBHelper.Instance.GetPojectChart();
            var queryChart = colChart.Find(filterChart).FirstOrDefault();

            if (queryChart != null && queryChart.FactorJson == lineFactorJson.ToString())
            {
                result = serializer.Deserialize<TimeLinkCountDto>(queryChart.DataJson);
            }
            else
            {
                //判断是否为根分组
                bool cateIsRoot = false;
                var cateObjIds = CommonHelper.GetObjIdListFromStr(categoryIds);
                if (cateObjIds.Count == 1 && cateObjIds[0].Equals(ObjectId.Empty))
                {
                    cateIsRoot = true;
                }
                //多个分组时剔除根分组
                cateObjIds.Remove(ObjectId.Empty);

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

                var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
                var builder = Builders<Dnl_Link_Baidu>.Filter;

                //获取关键词列表
                List<string> keywordList = new List<string>();      //关键词列表
                var groupFilter = groupBuilder.Eq(x => x.ProjectId, proObjId) & groupBuilder.Eq(x => x.IsDel, false);
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
                    groupFilter &= groupBuilder.In(x => x.CategoryId, cateObjIds);
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
                var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, proObjId) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
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
                    foreach (var x in cateObjIds)
                    {
                        CategoryList v = new CategoryList();
                        v.PublishTime = new List<DateTime>();
                        v.CategoryId = x.ToString();
                        categoryList.Add(v);
                    }

                    //获取分组名并分配到数据中去
                    var namefilter = Builders<Dnl_KeywordCategory>.Filter.In(x => x._id, cateObjIds);
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
            }
            
            return result;
        }

        //命中关键词域名分布图
        [HttpGet]
        public List<DomainStatisDto> GetDomainStatis(string categoryIds, string projectId)
        {
            List<DomainStatisDto> result = new List<DomainStatisDto>();
            if (string.IsNullOrEmpty(categoryIds) || string.IsNullOrEmpty(projectId))
            {
                return result;
            }

            ObjectId proObjId=new ObjectId(projectId);
            JavaScriptSerializer serializer = new JavaScriptSerializer();       //Json序列化与反序列化

            var cateIds = new List<string>();
            if (!string.IsNullOrEmpty(categoryIds))
            {
                cateIds = GetIdListFromStr(categoryIds);
                cateIds.Remove(ObjectId.Empty.ToString());
                cateIds.Sort();
            }
            
            //生成参数Json
            JObject factorJson = new JObject();
            factorJson.Add(new JProperty("categoryIds", string.Join(";",cateIds)));

            //获取图表数据
            var builderChart = Builders<PojectChartMongo>.Filter;
            var filterChart = builderChart.Eq(x => x.ProjectId, proObjId) & builderChart.Eq(x => x.Type, ChartType.Bubble);
            filterChart &= builderChart.Eq(x => x.Source, SourceType.Enginee) & builderChart.Eq(x => x.Name, "默认");
            var colChart = MongoDBHelper.Instance.GetPojectChart();
            var queryChart = colChart.Find(filterChart).FirstOrDefault();   

            /* 查询本设置对应的图表是否已存在 */
            //判断是否不刷新数据且图表数据存在并参数相同
            if (queryChart != null && queryChart.FactorJson == factorJson.ToString())
            {
                //反序列化图表数据
                result = serializer.Deserialize<List<DomainStatisDto>>(queryChart.DataJson);
            }
            else
            {
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
            }
            
            return result;
        }

        /// <summary>
        /// 关键词共现图
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResultDto GetKeywordBygroup(string user_id, string projectId)
        {
            var result = new JsonResultDto();
            try
            {
                //获取所有关键词映射
                var builder = Builders<Dnl_KeywordMapping>.Filter;
                var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                filter &= builder.Eq(x => x.IsDel, false);
                filter &= builder.Eq(x => x.CategoryId, ObjectId.Empty);
                var TaskList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filter).Limit(60).ToList();
                List<KwywordLinksVO> list = new List<KwywordLinksVO>();
                List<nodes> nodesList = new List<nodes>();
                List<linksdto> linkList = new List<linksdto>();

                List<ObjectId> lobj = new List<ObjectId>();         //所有映射Id

                foreach (var item in TaskList)
                {

                    lobj.Add(item._id);
                    nodes v = new nodes();
                    v.name = item.Keyword.ToString();
                    v.group = item.GroupNumber.ToString();

                    nodesList.Add(v);
                }
                var builder2 = Builders<Dnl_MappingCoPresent>.Filter;
                var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId));
                filter2 &= builder2.Lte(x => x.source, 59);
                filter2 &= builder2.Lte(x => x.target, 59);
                filter2 &= builder2.In(x => x.KeywordMappingId, lobj);
                var TaskList2 = MongoDBHelper.Instance.GetDnl_MappingCoPresent().Find(filter2).SortByDescending(x => x.value).Limit(1000).ToList();
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

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                string json = Serializer.Serialize(list);
                //string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
                // DelFile(folder);
                //string url = CreFile(folder, value, "miserables.txt");
                //jsonFileUrlDto json = new jsonFileUrlDto();
                //json.Url = url;
                result.IsSuccess = true;
                result.Json = json;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                //return new jsonFileUrlDto { Error = "json文件生成失败！" + ex.Message };
            }
            return result;
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

        /// <summary>
        /// 获取横向动态词树
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [HttpGet]
        public GroupTreeDto GetAllGroupTree(string usr_id, string projectId)
        {
            GroupTreeDto result = new GroupTreeDto();
            //添加根结点
            result.name = "根节点";
            result._id = "11";
            result.children = new List<GroupTreeDto>();

            GetCategoryTree(usr_id, projectId, new ObjectId("000000000000000000000000"), result);

            return result;
        }

        /// <summary>
        /// 获取横向动态词树子树
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="parentId">父结点Id</param>
        /// <param name="parent">父结点</param>
        private void GetCategoryTree(string usr_id, string projectId, ObjectId parentId, GroupTreeDto parent)
        {
            //获取子分组
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();
            //获取子分组内关键词
            var groupbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            groupfilter &= groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var keywordList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter).Project(x => new GroupTreeDto
            {
                _id = x.KeywordId.ToString(),
                name = x.Keyword
            }).ToList();
            parent.children.AddRange(keywordList);

            if (TaskList.Count == 0)
                return;
            //循环获取
            foreach (var treedata in TaskList)
            {
                GetCategoryTree(usr_id, projectId, new ObjectId(treedata._id), treedata);
                parent.children.Add(treedata);
            }


        }

        /// <summary>
        /// 获组分组树，返回json文件路径（圆形d3图）
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResultDto GetAllGroupTreeUrl(string usr_id, string projectId)
        {
            var result = new JsonResultDto();
            try
            {
                //获取该项目下所有分组信息
                List<GroupTree2Dto> list = new List<GroupTree2Dto>();
                GroupTree2Dto root = new GroupTree2Dto();
                //获取该项目名称
                var builder = Builders<IW2S_Project>.Filter;
                var filter = builder.Eq(x => x._id, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) & 
                var task = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter).Project(x => new IW2S_ProjectDto { Name = x.Name }).FirstOrDefault();
                root.name = task.Name;
                //根目录ID默认为"000000000000000000000000"
                root.id = "000000000000000000000000";
                root.pId = "0";
                root.isNode = true;
                list.Add(root);
                //获取子树信息
                GetCategoryTree3(usr_id, projectId, new ObjectId("000000000000000000000000"), list);

                //将分组信息转为树形
                GroupTree3Dto GroupTree = new GroupTree3Dto();
                GroupTree = GetCategoryTreeNode("000000000000000000000000", list);

                //输出到json格式文件中

                JavaScriptSerializer Serializer = new JavaScriptSerializer();

                string value = Serializer.Serialize(GroupTree);
                //删除无用字符串
                value = value.Replace(@",""children"":null", "");
                value = value.Replace(@",""size"":null", "");

                result.IsSuccess = true;
                result.Json = value;

                ////写入json格式文件
                //string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Scripts\app\data\";
                ////DelFile(folder);
                ////string url = CreFile(folder, value, "AllGroupTree.txt");
                //string path = folder + "AllGroupTree.txt";
                //if (File.Exists(path))
                //{
                //    File.Delete(path);
                //}
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                //sw.WriteLine(value);
                //sw.Close();

                //jsonFileUrlDto json = new jsonFileUrlDto();
                //json.Url = "Scripts/app/data/" + "AllGroupTree.txt";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                //return new jsonFileUrlDto { Error = "json文件生成失败！" + ex.Message };
            }
            return result;
        }

        /// <summary>
        /// 获取分组目录（含叶结点）数组
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="parentId">父结点Id</param>
        /// <param name="list">结点列表</param>
        /// <returns></returns>
        private void GetCategoryTree3(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));//builder.Eq(x => x.UsrId, new ObjectId(usr_id)) &
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name,
                isNode = true
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<Dnl_KeywordMapping>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            groupfilter &= groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var keywordList = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(groupfilter).Project(x => new GroupTree2Dto
            {
                id = x.KeywordId.ToString(),
                pId = x.CategoryId.ToString(),
                name = x.Keyword,
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
            if (TaskList.Count == 0) 
                return;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (GroupTree2Dto treedata in TaskList)
            {
                list.Add(treedata);
                GetCategoryTree3(usr_id, projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
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
                        var builder = Builders<Dnl_Keyword>.Filter;
                        var filter = builder.Eq(x => x._id, new ObjectId(v.id));
                        var task = MongoDBHelper.Instance.GetDnl_Keyword().Find(filter).FirstOrDefault();

                        GroupTree3Dto leaf = new GroupTree3Dto();
                        leaf.name = v.name + "(" + task.ValLinkCount_Baidu + ")";
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
        /// 获取该项目下所有分组(词组文件夹列表)
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


           var listGT=  GetCategoryTree2(usr_id, projectId, new ObjectId("000000000000000000000000"), list);
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
        private  List<GroupTree2Dto> GetCategoryTree2(string usr_id, string projectId, ObjectId parentId, List<GroupTree2Dto> list)
        {
            //获取次级词组名
            var builder = Builders<Dnl_KeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId =x.ParentId.ToString(),
                name = x.Name
            }).ToList();

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
        #endregion

        #region 项目
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
            var mappingBuilder=Builders<Dnl_KeywordMapping>.Filter;
            var mappingFlter = mappingBuilder.In(x => x.ProjectId, proObjIds) & mappingBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & mappingBuilder.Eq(x => x.IsDel, false);
            var queryMapping = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mappingFlter).Project(x => new
            {
                Id=x._id.ToString(),
                KeywordId = x.KeywordId.ToString(),
                ProjectId = x.ProjectId.ToString()
            }).ToList();
            var keyObjIds = queryMapping.Select(x => new ObjectId(x.KeywordId)).ToList();
            //建立关键词映射与项目对应词典
            var keyToPoject = new Dictionary<string, string>();

            foreach (var x in queryMapping)
            {
                if (keyToPoject.ContainsKey(x.KeywordId))
                    keyToPoject[x.KeywordId] += ";" + x.ProjectId;      //一个关键词可能对应多个项目
                else
                    keyToPoject.Add(x.KeywordId, x.ProjectId);
            }
            //获取关键词信息
            var keywordBuilder = Builders<Dnl_Keyword>.Filter;
            var keywordFilter = keywordBuilder.In(x => x._id, keyObjIds);
            var keywordCol = MongoDBHelper.Instance.GetDnl_Keyword();
            var keywordQuery = keywordCol.Find(keywordFilter).SortByDescending(x => x.LastBotEndAt_Baidu).Project(x => new ProLinkKey
            {
                Id = x._id.ToString(),
                UpdateTime = x.LastBotEndAt_Baidu,
                ProjectId = keyToPoject[x._id.ToString()],
                LinkCount = x.LinkCount_Baidu
            }).ToList();

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
                int linkCount = 0;
                var tempKeyList = keywordQuery.Where(x => x.ProjectId.Contains(v._id));
                if (tempKeyList != null)
                {
                    foreach (var x in tempKeyList)
                    {
                        linkCount += x.LinkCount;
                    }
                }
                v.LinkCount = linkCount;
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
            var colGroup=MongoDBHelper.Instance.GetDnl_ProjectGroup();
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
            
            return "成功！";
        }

        [HttpGet]
        public ResultDto UpdateProject(string prjId, string name, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(prjId));
            if(string.IsNullOrEmpty(name))
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
        #endregion

        #region 导入导出
        /// <summary>
        /// 导出关键词分组信息
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public string ExportKeywordGroup(string user_id, string projectId)
        {
            HSSFWorkbook workBook = new HSSFWorkbook();     //Excel表格

            //获取所有关键词信息
            var builder = Builders<Dnl_KeywordMapping>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder.Eq(x=>x.IsDel,false);
            var col = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            var keywords = col.Find(filter).ToList();
            if(keywords == null  || keywords.Count ==0)
            {
                return "没有要导出的数据";
            }
            //导出关键词信息
            ISheet sheet1 = workBook.CreateSheet("Dnl_Keyword");
            IRow RowHead1 = sheet1.CreateRow(0);
            RowHead1.CreateCell(0).SetCellValue("_id");
            RowHead1.CreateCell(1).SetCellValue("Keyword");
            RowHead1.CreateCell(2).SetCellValue("KeywordId");
            RowHead1.CreateCell(3).SetCellValue("CategoryId");
            RowHead1.CreateCell(4).SetCellValue("ParentCategoryId");
            RowHead1.CreateCell(5).SetCellValue("GroupNumber");

            int count = 0;
            foreach (var keyword in keywords)
            {
                IRow row = sheet1.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword.KeywordId.ToString());
                row.CreateCell(1).SetCellValue(keyword.Keyword);
                row.CreateCell(2).SetCellValue(keyword.KeywordId.ToString());
                row.CreateCell(3).SetCellValue(keyword.CategoryId.ToString());
                row.CreateCell(4).SetCellValue(keyword.ParentCategoryId.ToString());
                row.CreateCell(5).SetCellValue(keyword.GroupNumber);
                count = count + 1;
            }

            //获取所有分组信息
            var builder2 = Builders<Dnl_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, new ObjectId(projectId)) & builder2.Eq(x => x.IsDel, false);
            var col2 = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var keywords2 = col2.Find(filter2).ToList();

            ISheet sheet2 = workBook.CreateSheet("Dnl_KeywordCategory");
            IRow RowHead2 = sheet2.CreateRow(0);
            RowHead2.CreateCell(0).SetCellValue("_id");
            RowHead2.CreateCell(1).SetCellValue("GroupNumber");
            RowHead2.CreateCell(2).SetCellValue("InfriLawCode");
            RowHead2.CreateCell(3).SetCellValue("KeywordCount");
            RowHead2.CreateCell(4).SetCellValue("Name");
            RowHead2.CreateCell(5).SetCellValue("ParentId");
            RowHead2.CreateCell(6).SetCellValue("Weight");
            //导出所有分组信息
            count = 0;
            foreach (var keyword in keywords2)
            {
                IRow row = sheet2.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.GroupNumber);
                row.CreateCell(2).SetCellValue(keyword.InfriLawCode.ToString());
                row.CreateCell(3).SetCellValue(keyword.KeywordCount);
                row.CreateCell(4).SetCellValue(keyword.Name);
                row.CreateCell(5).SetCellValue(keyword.ParentId.ToString());
                row.CreateCell(6).SetCellValue(keyword.Weight);
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
            int page = 0, pageSize = 100;
            List<Dnl_Link_BaiduDto> keywords = new List<Dnl_Link_BaiduDto>();
            while (page >= 0)
            {
                var keywords1 = GetLevelLinks(user_id, projectId, null, null, null, null, null, null, page, pageSize).Result;
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
            RowHead.CreateCell(2).SetCellValue("BaiduVStar");
            RowHead.CreateCell(3).SetCellValue("CreatedAt");
            RowHead.CreateCell(4).SetCellValue("DataCleanStatus");
            RowHead.CreateCell(5).SetCellValue("Description");
            RowHead.CreateCell(6).SetCellValue("Domain");
            RowHead.CreateCell(7).SetCellValue("InfriLawCode");
            RowHead.CreateCell(8).SetCellValue("Keywords");
            RowHead.CreateCell(9).SetCellValue("LinkUrl");
            RowHead.CreateCell(10).SetCellValue("PublishTime");
            RowHead.CreateCell(11).SetCellValue("SearchkeywordId");
            RowHead.CreateCell(12).SetCellValue("Title");
            RowHead.CreateCell(13).SetCellValue("TopDomain");

            int count = 0;
            foreach (var keyword in keywords)
            {
                IRow row = sheet.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                row.CreateCell(1).SetCellValue(keyword.Abstract);
                row.CreateCell(2).SetCellValue(keyword.BaiduVStar);
                row.CreateCell(3).SetCellValue(keyword.CreatedAt);
                row.CreateCell(4).SetCellValue(keyword.DataCleanStatus??0);
                row.CreateCell(4).SetCellValue(keyword.Description);
                row.CreateCell(5).SetCellValue(keyword.Domain);
                row.CreateCell(7).SetCellValue(keyword.InfriLawCode.ToString());
                row.CreateCell(8).SetCellValue(keyword.Keywords);
                row.CreateCell(9).SetCellValue(keyword.LinkUrl);
                row.CreateCell(10).SetCellValue(keyword.PublishTime);
                row.CreateCell(11).SetCellValue(keyword.SearchkeywordId.ToString());
                row.CreateCell(12).SetCellValue(keyword.Title);
                row.CreateCell(13).SetCellValue(keyword.TopDomain);
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
            List<Dnl_KeywordCategory> keywordCategories = new List<Dnl_KeywordCategory>();
            List<Dnl_KeywordMapping> mappings = new List<Dnl_KeywordMapping>();

            ObjectId proObjId=new ObjectId(projectId);

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();
            try
            {

                using (Stream stream = new FileStream(Path + excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(stream);
                    //Execel第一行是标题，不是要导入数据库的数据
                    Dictionary<string, string> diccategorys = new Dictionary<string, string>();

                    //导入关键词分组信息
                    HSSFSheet sheet2 = workbook.GetSheetAt(1) as HSSFSheet;
                    for (int i = 1; i <= sheet2.LastRowNum; i++)
                    {
                        HSSFRow row = sheet2.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(5) == null || string.IsNullOrEmpty(row.GetCell(5).StringCellValue))
                        {
                            continue;
                        }
                        Dnl_KeywordCategory keywordCategory = new Dnl_KeywordCategory();

                        keywordCategory._id = ObjectId.GenerateNewId();
                        diccategorys.Add(row.GetCell(0).StringCellValue, keywordCategory._id.ToString());

                        keywordCategory.IsDel = false;
                        keywordCategory.GroupNumber = (int)row.GetCell(1).NumericCellValue;
                        keywordCategory.InfriLawCode = new ObjectId(row.GetCell(2).StringCellValue);
                        keywordCategory.KeywordCount = (int)row.GetCell(3).NumericCellValue;
                        keywordCategory.Name = row.GetCell(4).StringCellValue;
                        keywordCategory.ParentId = new ObjectId(row.GetCell(5).StringCellValue);
                        keywordCategory.Weight = (int)row.GetCell(6).NumericCellValue;
                        keywordCategory.ProjectId = proObjId;
                        keywordCategory.UserId = usrObjId;
                        keywordCategory.CreatedAt = DateTime.Now.AddHours(8);

                        keywordCategories.Add(keywordCategory);
                    }
                    string empty = ObjectId.Empty.ToString();
                    if (!diccategorys.ContainsKey(empty))
                    {
                        diccategorys.Add(empty, empty);
                    }
                    foreach (var keywordCategorie in keywordCategories)
                    {
                        keywordCategorie.ParentId = diccategorys.ContainsKey(keywordCategorie.ParentId.ToString()) ? new ObjectId(diccategorys[keywordCategorie.ParentId.ToString()]) : keywordCategorie.ParentId;
                    }

                    //导入词组内关键词信息
                    HSSFSheet sheet = workbook.GetSheetAt(0) as HSSFSheet;

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        HSSFRow row = sheet.GetRow(i) as HSSFRow;
                        if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue) || row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).StringCellValue))
                        {
                            continue;
                        }

                        Dnl_KeywordMapping mapping = new Dnl_KeywordMapping();
                        mapping.CreatedAt = DateTime.Now.AddHours(8);
                        mapping.ProjectId = new ObjectId(projectId);
                        mapping.UserId = usrObjId;
                        mapping._id = ObjectId.GenerateNewId();

                        mapping.Keyword = row.GetCell(1).StringCellValue;
                        mapping.KeywordId = new ObjectId(row.GetCell(2).StringCellValue);
                        mapping.CategoryId = new ObjectId(diccategorys[row.GetCell(3).StringCellValue]);
                        mapping.ParentCategoryId = new ObjectId(diccategorys[row.GetCell(4).StringCellValue]);
                        mapping.GroupNumber = (int)row.GetCell(5).NumericCellValue;

                        mappings.Add(mapping);
                    }

                }
                
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            var builder2 = Builders<Dnl_KeywordCategory>.Filter;
            var filter2 = builder2.Eq(x => x.ProjectId, proObjId);
            var col2 = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            col2.DeleteMany(filter2);
            col2.InsertMany(keywordCategories);

            var builder3 = Builders<Dnl_KeywordMapping>.Filter;
            var filter3 = builder3.Eq(x => x.ProjectId, proObjId);
            var col3 = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            col3.DeleteMany(filter3);
            col3.InsertMany(mappings);

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
        #endregion

        #region 分析指向
        [HttpGet]
        public ResultDto SetPrjAnalysisItem(string usr_id, string prjId, string anaItemId)
        {
            ResultDto result = new ResultDto();
            if(string.IsNullOrEmpty(prjId) || string.IsNullOrEmpty(anaItemId))
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
            var itemId = col.Find(filter).Project(x=>x.AnalysisItem).FirstOrDefault();

            if(itemId == ObjectId.Empty)
            {
                return result;
            }
            var aitembuilder = Builders<IW2S_AnalysisItem>.Filter;
            var aitemfilter = aitembuilder.Eq(x => x._id, itemId);

            var aitemcol = MongoDBHelper.Instance.GetIW2S_AnalysisItems();
            var anaItem = aitemcol.Find(aitemfilter).FirstOrDefault();

            if(anaItem == null)
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
            usrItems.AddRange( col.Find(filter).ToList());
            
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
        public ResultDto RemoveAnalysisItem(string user_id,string anaItemId)
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
        #endregion

        #region 域名分类
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
            if(string.IsNullOrEmpty(data.Name))
            {
                result.Message="分类名称为空";
                return result;
            }
            else if(string.IsNullOrEmpty(data.UsrId))
            {
                result.Message="用户Id为空";
                return result;
            }
            //var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(data.ProjectId))).Project(x => x.UsrId).FirstOrDefault();

            var filter = builder.Eq(x => x.UsrId, new ObjectId(data.UsrId)) & builder.Eq(x => x.Name, data.Name) & builder.Eq(x => x.IsDel, false);
            if(string.IsNullOrEmpty(data._id))
            {
                var r = col.Find(filter).Project(x => x._id).FirstOrDefault();
                if(r != ObjectId.Empty)
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
                    cat.ParentId=new ObjectId("000000000000000000000000");
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
            if(cat.Count>0)
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
                filter = builder.Eq(x => x.UsrId,ObjectId.Empty) & builder.Eq(x => x.IsDel, false);
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
                        col.UpdateOne(filter, new UpdateDocument { { "$set", new QueryDocument {{ "DomainName", category.DomainName },{"DomainCategoryId",new ObjectId(category.DomainCategoryId)} } } });
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
        #endregion

        #region 折线图及饼图保存
        /// <summary>
        /// 保存折线图及饼图
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
        public ResultDto SaveChart(string categoryId, string prjId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval, string sourceType,string name,string user_id)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_ChartConfig>.Filter;
            var proOId=new ObjectId(prjId);
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
        #endregion

        #region 文字树
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
                Name=wordTree.Name,
                CreatedAt=DateTime.Now.AddHours(8)
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
                CreatedAt=x.CreatedAt
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
                    CreatedAt=query.CreatedAt
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
        #endregion

        #region 链接关系图
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
            var builderCate=Builders<Dnl_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(prjId));
            filterCate &= builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
            var queryCate = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => new
            {
                Id = x._id,
                Name = x.Name
            }).ToList();
            var cateObjIdList = queryCate.Select(x => x.Id).ToList();      //词组ObjectId列表
            //建立分组信息
            var cateInfo = new List<LinkReferCate>();
            foreach (var x in queryCate)
            {
                var cate = new LinkReferCate();
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
            var builderMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterMap = builderMap.In(x => x.CategoryId, cateObjIdList) & builderMap.Eq(x => x.IsDel, false);
            var queryKey = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.CategoryId,
                Keyword = x.Keyword,
            }).ToList();
            var keyIdList = queryKey.Select(x => x.KeywordId).ToList();    //关键词ObjectId列表

            //建议关键词与词组字典
            var keyToCate = new Dictionary<string, string>();
            //var keyGroup = queryKey.GroupBy(x => x.KeywordId).ToList();
            queryKey = queryKey.DistinctBy(x => x.KeywordId).ToList();
            foreach (var key in queryKey)
            {
                var cateId = queryCate.Find(x => x.Id.Equals(key.CategoryId)).Id.ToString();
                keyToCate.Add(key.KeywordId, cateId);
            }

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIdList = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取链接信息
            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIdList);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIdList);
            var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Title = x.Title,
                Description = x.Description,
                Keword = x.Keywords,
                KeywordId = x.SearchkeywordId,
                PublishTime = x.PublishTime,
                LinkUrl = x.LinkUrl,
                DCNum = x.DCNum
            }).ToList();

            //建立节点信息
            var linkNodeList = new List<LinkReferNode>();         //节点信息
            for (int i = 0; i < queryLink.Count;i++ )
            {
                //未存在发布信息时跳过该链接
                DateTime tpdt = new DateTime();
                DateTime.TryParse(queryLink[i].PublishTime, out tpdt);
                if (tpdt < new DateTime(1753, 1, 09) || tpdt > DateTime.Now)
                {
                    continue;
                }
                //获取链接信息
                var link = new LinkReferNode
                {
                    publishTime = tpdt,
                    linkUrl = queryLink[i].LinkUrl,
                    value = 1,
                    SortNum = queryLink[i].DCNum,
                };
                if (queryLink[i].Title != null && queryLink[i].Title.Length > 20)
                    link.name = queryLink[i].Title.Substring(0, 19) + "…";
                else
                    link.name = queryLink[i].Title;
                if (queryLink[i].Description != null && queryLink[i].Description.Length > 50)
                    link.describe = queryLink[i].Description.Substring(0, 49) + "…";
                else
                    link.describe = queryLink[i].Description;

                //获取链接所含关键词及数量
                var repeat = queryLink.FindAll(s => s.Title == queryLink[i].Title).DistinctBy(s => s.KeywordId);
                
                link.keyWordCount = repeat.Count;
                link.keyWordList = new List<string>();
                link.keyWordIdList = new List<string>();
                //移除重复的链接
                foreach (var y in repeat)
                {
                    link.keyWordList.Add(y.Keword);
                    link.keyWordIdList.Add(y.KeywordId);
                    if (i == queryLink.Count)
                        i--;
                    if (queryLink[i].KeywordId != y.KeywordId)
                    {
                        queryLink.Remove(y);
                    }
                }
                //获取归属组序号
                var cateId = keyToCate[queryLink[i].KeywordId];
                var cateIndex = cateInfo.FindIndex(s => s.id == cateId);
                link.category = cateIndex;
                linkNodeList.Add(link);
            }

            //建立时间坐标并将链接信息按时间分组
            DateTime startTime = linkNodeList.Min(x => x.publishTime);
            DateTime endTime = linkNodeList.Max(x => x.publishTime);
            result.DateTimeList = new List<DateTime>();
            int year = startTime.Year;
            int month = startTime.Month;
            int season = (month-1) / 3;
            var timeLinkNodeList = new List<List<LinkReferNode>>();        //按时间分组的链接节点信息
            switch (timeInterval)
            {
                case 0:     //一次返回所有时间段
                    result.DateTimeList.Add(startTime);
                    timeLinkNodeList.Add(linkNodeList);
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
                            var tempNode = new List<LinkReferNode>();
                            tempNode = linkNodeList.Where(x => x.publishTime >= time && x.publishTime < time.AddMonths(1)).ToList();
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
                            var tempNode = new List<LinkReferNode>();
                            tempNode = linkNodeList.Where(x => x.publishTime >= time && x.publishTime < time.AddMonths(3)).ToList();
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
                            var tempNode = new List<LinkReferNode>();
                            tempNode = linkNodeList.Where(x => x.publishTime >= time && x.publishTime < time.AddYears(1)).ToList();
                            timeLinkNodeList.Add(tempNode);
                        }
                        break;
                    }
                default:
                    return null;
            }
            result.ReferList = new List<LinkReferDto>();
            for (int i = 0; i < result.DateTimeList.Count;i++ )
            {
                //重标记结点的顺序
                for (int j = 0; j < timeLinkNodeList[i].Count; j++)
                {
                    timeLinkNodeList[i][j].Index = j;
                }
                var referData = ComputerLinkRefer(timeLinkNodeList[i], cateInfo,keyIdList);
                result.ReferList.Add(referData);
            }
            
            sw.Stop();
            return result;
            
        }

        /// <summary>
        /// 计算节点关系
        /// </summary>
        /// <param name="linkNodeList">节点信息</param>
        /// <param name="cateInfo">分组信息</param>
        /// <param name="keyIdList">关键词Id列表</param>
        /// <returns></returns>
        LinkReferDto ComputerLinkRefer(List<LinkReferNode> linkNodeList, List<LinkReferCate> cateInfo,List<string> keyIdList)
        {
            LinkReferDto result = new LinkReferDto();
            //建立节点关系
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var linkReferList = new List<LinkReferIn2Node>();        //节点间关系
            //按关键词分组生成小集群列表
            var linkByKeyList = new List<LinkReferByKey>();         //关键词链接信息列表
            //var centerNodeList = new List<LinkReferNode>();         //核心节点列表
            foreach (var keyId in keyIdList)
            {
                var nodeList = linkNodeList.FindAll(x => x.keyWordIdList.Contains(keyId));
                if (nodeList.Count > 0)
                {
                    var linkByKey = new LinkReferByKey
                    {
                        KeywordId = keyId,
                        NodeList = nodeList
                    };
                    linkByKeyList.Add(linkByKey);
                }
            }
            //遍历计算关系
            for (int i = 0; i < linkByKeyList.Count; i++)
            {
                string keyId = linkByKeyList[i].KeywordId;
                var nodeList = linkByKeyList[i].NodeList;
                //计算集群内部关系
                var center = nodeList.OrderByDescending(x => x.SortNum).FirstOrDefault();     //选取核心点
                //centerNodeList.Add(center);
                foreach (var node in nodeList)
                {
                    if (node.linkUrl == center.linkUrl)
                        continue;
                    var refer = new LinkReferIn2Node
                    {
                        source = center.Index,
                        target = node.Index
                    };
                    linkReferList.Add(refer);
                }
            }

            ////遍历所有节点，计算有复数关键词之间节点关系
            //var mutilNodeList = linkNodeList.FindAll(x => x.keyWordCount > 1);      //所有复数关键词节点
            //for (int i = 0; i < mutilNodeList.Count; i++)
            //{
            //    var node = mutilNodeList[i];        //源节点
            //    //只与核心结点对比
            //    for (int j = i + 1; j < centerNodeList.Count; j++)
            //    {
            //        var cpNode = centerNodeList[j];  //对比节点
            //        //判断两个节点是否相关
            //        bool isRefer = false;
            //        foreach (var keyId in node.keyWordIdList)
            //        {
            //            if (isRefer)
            //            {
            //                break;
            //            }
            //            if (cpNode.keyWordIdList.Contains(keyId))
            //            {
            //                //判断关系是否已存在
            //                var rep = linkReferList.Find(x => x.source == node.Index && x.target == cpNode.Index);
            //                if (rep != null)
            //                    break;
            //                var rep2 = linkReferList.Find(x => x.source == cpNode.Index && x.target == node.Index);
            //                if (rep2 != null)
            //                    break;
            //                var refer = new LinkReferIn2Node
            //                {
            //                    source = node.Index,
            //                    target = cpNode.Index
            //                };
            //                linkReferList.Add(refer);
            //                isRefer = true;
            //            }
            //        }
            //    }
            //}

            #region 旧节点关系计算优化方法（废弃）
            //for (int i = 0; i < linkNodeList.Count; i++)
            //{
            //    for (int j = i + 1; j < linkNodeList.Count; j++)
            //    {
            //        //判断i和j两个节点是否有关联
            //        bool isRefer = false;
            //        foreach (var keyId in linkNodeList[i].keyWordIdList)
            //        {
            //            if (isRefer)
            //            {
            //                break;
            //            }
            //            if (linkNodeList[j].keyWordIdList.Contains(keyId))
            //            {
            //                var refer = new LinkReferIn2Node
            //                {
            //                    source = i,
            //                    target = j
            //                };
            //                linkReferList.Add(refer);
            //                isRefer = true;
            //            }
            //        }
            //    }
            //}
            ////清理重复集群关系
            //var tempGroup = linkReferList.GroupBy(x => x.source).ToList();              //统计获取集群中心点
            //var tempCount = tempGroup.Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();               //统计获取集群中心点
            //var usedIndex = new List<int>();    //已清理节点
            //foreach (var x in tempCount)
            //{
            //    if (usedIndex.Contains(x.Key))
            //    {
            //        continue;
            //    }
            //    var centerNode = linkNodeList[x.Key];      //集群中心点
            //    var referNodeList = tempGroup.Find(s => s.Key == x.Key);
            //    var referIndexs = referNodeList.Select(s => s.target).ToList();      //集群内所有相关点位置
            //    foreach (var y in referNodeList)            //遍历集群相关点
            //    {
            //        var referNode = linkNodeList[y.target];    //单个相关点
            //        if (centerNode.category != referNode.category)
            //        {
            //            continue;
            //        }
            //        //清除相关点与集群内其他点的关系
            //        var referInfos = linkReferList.FindAll(s => s.source == y.target);
            //        var tempIndexs = referInfos.Select(s => s.target).ToList();     //相关点其所有关系节点位置
            //        foreach (var z in referInfos)
            //        {
            //            if (referIndexs.Contains(z.target))
            //            {
            //                var targetRefer = linkReferList.FindAll(s => s.source == z.target).Select(s => s.target).ToList();     //当前对应相关点的关系节点其自身所有关系节点位置
            //                //判断关系节点是否在同一集群
            //                if (targetRefer.All(s => referIndexs.Contains(s)))
            //                {
            //                    linkReferList.Remove(z);
            //                }
            //            }
            //        }
            //        usedIndex.Add(y.target);
            //    }
            //}
            #endregion

            sw.Stop();
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
            foreach (var x in linkNodeList)
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
            foreach (var x in linkReferList)
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
            var topIndex = linkReferList.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
            result.TopData = new List<LinkReferNode>();
            foreach (var x in topIndex)
            {
                var data = new LinkReferNode(linkNodeList[x.Key]);
                data.value = x.Count;
                result.TopData.Add(data);
            }
            if (result.TopData.Count < 10)
            {
                foreach (var x in topIndex)
                {
                    //获取集群内其他点
                    var temp = linkReferList.FindAll(s => s.source == x.Key);
                    foreach (var y in temp)
                    {
                        var data = new LinkReferNode(linkNodeList[y.target]);
                        data.value = linkReferList.Count(s => s.source == y.target);
                        result.TopData.Add(data);
                        if (result.TopData.Count == 10)
                            break;
                    }
                    if (result.TopData.Count == 10)
                        break;
                }
                //获取孤立结点
                foreach (var x in linkNodeList)
                {
                    if (result.TopData.Count == 10)
                        break;
                    if (!result.TopData.Contains(x))
                    {
                        var data = new LinkReferNode(x);
                        data.value = 1;
                        result.TopData.Add(data);
                    }
                }
            }
            

            //获取包含关键词最多的10个节点
            result.TopKeyData = new List<LinkReferNode>();
            var tpData = linkNodeList.OrderByDescending(x => x.keyWordCount).Take(10).ToList();
            for (int i = 0; i < tpData.Count; i++)
            {
                var data = new LinkReferNode(tpData[i]);
                data.value = data.keyWordCount;
                result.TopKeyData.Add(data);
            }

            //获取组外链接数最多的10个节点
            var linkGroupCount = new List<LinkGroupCount>();
            for(int i=0;i<linkNodeList.Count;i++)
            {
                int count = 0;
                //获取该结点所有相关节点
                var referIndexs = linkReferList.FindAll(x => x.source == i).Select(x => x.target).ToList();
                referIndexs.AddRange(linkReferList.FindAll(x => x.target == i).Select(x => x.source).ToList());
                referIndexs = referIndexs.Distinct().ToList();
                foreach (var x in referIndexs)
                {
                    if (linkNodeList[i].category != linkNodeList[x].category)
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
            result.TopCateData = new List<LinkReferNode>();
            foreach (var x in linkGroupCount)
            {
                var data = new LinkReferNode(linkNodeList[x.Pos]);
                data.value = x.Count;
                result.TopCateData.Add(data);
            }

            //获取词组影响力
            result.CateWeights = new List<CategoryWeight>();
            for (int i = 0; i < cateInfo.Count; i++)
            {
                var cateWeight = new CategoryWeight();
                cateWeight.Category = cateInfo[i].name;
                topIndex = linkReferList.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
                int referNum = 0;
                int keywordNum = 0;
                for (int j = 0; j < linkNodeList.Count; j++)
                {
                    if (linkNodeList[j].category == i)
                    {
                        referNum += linkReferList.Where(x => x.source == j || x.target == j).Count();
                        keywordNum += linkNodeList[j].keyWordCount;
                    }
                }
                cateWeight.Weight = keywordNum * 99 + referNum * 12;
                result.CateWeights.Add(cateWeight);
            }
            result.CateWeights = result.CateWeights.OrderByDescending(x => x.Weight).ToList();

                return result;
        }

        /// <summary>
        /// 获取链接关系统计
        /// </summary>
        /// <param name="prjId">项目Id</param>
        /// <returns></returns>
        public LinkReferCount GetLinkReferCount(string projectId)
        {
            var result = new LinkReferCount();
            if (string.IsNullOrEmpty(projectId))
            {
                return result;
            }
            //获取项目信息
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId));
            var pro = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).Project(x => new
            {
                Id = x._id,
                Name = x.Name,
                Description = x.Description
            }).FirstOrDefault();
            result.ProName = pro.Name;
            result.ProDesc = pro.Description;
            //获取第1级关键词组
            List<ObjectId> cateIds = new List<ObjectId>();
            var builderCate = Builders<Dnl_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ProjectId, pro.Id);
            filterCate &= builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
            var queryCate = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => new
            {
                Id = x._id,
                Name = x.Name
            }).ToList();
            result.CateNum = queryCate.Count;
            var cateObjIds = queryCate.Select(x => x.Id).ToList();      //词组ObjectId列表
            //建立分组信息
            var cateInfo = new List<LinkReferCate>();
            foreach (var x in queryCate)
            {
                var cate = new LinkReferCate();
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
            var builderGroup = Builders<Dnl_KeywordMapping>.Filter;
            var filterGroup = builderGroup.In(x => x.CategoryId, cateObjIds) & builderGroup.Eq(x => x.IsDel, false);
            var queryKey = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterGroup).Project(x => new
            {
                KeywordId = x.KeywordId,
                CategoryId = x.CategoryId,
                Keyword = x.Keyword,
            }).ToList();
            var keyIds = queryKey.Select(x => x.KeywordId.ToString()).ToList();    //关键词ObjectId列表
            keyIds = keyIds.Distinct().ToList();
            result.KeywordNum = keyIds.Count;

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
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, pro.Id) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            filterLinkMap &= builderLinkMap.Eq(x => x.Source, SourceType.Enginee);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取链接总数
            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIds);
            var colLink = MongoDBHelper.Instance.GetDnl_Link_Baidu();
            var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Title = x.Title,
                Keword = x.Keywords,
                KeywordId = x.SearchkeywordId,
                PublishTime = x.PublishTime,
                LinkUrl = x.LinkUrl,
                DCNum = x.DCNum,
                Domain = x.Domain
            }).ToList();

            //建立节点信息
            var linkNodeList = new List<LinkReferNode>();         //节点信息
            int index = 0;
            for (int i = 0; i < queryLink.Count; i++)
            {
                //未存在发布信息时跳过该链接
                DateTime tpdt = new DateTime();
                DateTime.TryParse(queryLink[i].PublishTime, out tpdt);
                if (tpdt < new DateTime(1753, 1, 09) || tpdt > DateTime.Now)
                {
                    continue;
                }
                //获取链接信息
                var link = new LinkReferNode
                {
                    publishTime = tpdt,
                    linkUrl = queryLink[i].LinkUrl,
                    value = 1,
                    SortNum = queryLink[i].DCNum,
                    Index = index++
                };
                if (queryLink[i].Title != null && queryLink[i].Title.Length > 20)
                    link.name = queryLink[i].Title.Substring(0, 19) + "…";
                else
                    link.name = queryLink[i].Title;

                //获取链接所含关键词及数量
                var repeat = queryLink.FindAll(s => s.Title == queryLink[i].Title).DistinctBy(s => s.KeywordId);

                link.keyWordCount = repeat.Count;
                link.keyWordList = new List<string>();
                link.keyWordIdList = new List<string>();
                //移除重复的链接
                foreach (var y in repeat)
                {
                    link.keyWordList.Add(y.Keword);
                    link.keyWordIdList.Add(y.KeywordId);
                    if (i == queryLink.Count)
                        i--;
                    if (queryLink[i].KeywordId != y.KeywordId)
                    {
                        queryLink.Remove(y);
                    }
                }
                //获取归属组序号
                var cateId = keyToCate[queryLink[i].KeywordId];
                var cateIndex = cateInfo.FindIndex(s => s.id == cateId);
                link.category = cateIndex;
                linkNodeList.Add(link);
            }

            result.LinkNum = linkNodeList.Count;
            result.SiteNum = queryLink.Select(x => x.Domain).Distinct().Count();

            return result;
        }

        /// <summary>
        /// 插入描述信息
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertReferChartDesc(ReferChartPost post)
        {
            ResultDto result = new ResultDto();
            ObjectId proObjId = new ObjectId(post.projectId);
            var builder = Builders<ReferChartDescMongo>.Filter;
            var filter = builder.Eq(x => x.ProjectId, proObjId);
            filter &= builder.Eq(x => x.Type, SourceType.Enginee) & builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetReferChartDesc();
            try
            {
                //检查是否已保存与该描述相同的记录
                var query = col.Find(filter).FirstOrDefault();
                if (query != null)
                {
                    result.Message = "该描述已存在！";
                    return result;
                }
                var desc = new ReferChartDescMongo
                {
                    ProjectId = proObjId,
                    CreatedAt = DateTime.Now.AddHours(8),
                    DescList = post.descList,
                    Type = SourceType.Enginee
                };
                col.InsertOne(desc);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 修改描述
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateReferChartDesc(ReferChartPost post)
        {
            ResultDto result = new ResultDto();
            ObjectId proObjId = new ObjectId(post.projectId);
            var builder = Builders<ReferChartDescMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(post.descId));
            filter &= builder.Eq(x => x.Type, SourceType.Enginee) & builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetReferChartDesc();
            try
            {
                //删除原有记录，写入新记录
                col.DeleteOne(filter);
                InsertReferChartDesc(post);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns></returns>
        [HttpGet]
        public ReferChartDescDto GetReferChartDesc(string projectId)
        {
            ObjectId proObjId = new ObjectId(projectId);
            var builder = Builders<ReferChartDescMongo>.Filter;
            var filter = builder.Eq(x => x.ProjectId, proObjId);
            filter &= builder.Eq(x => x.Type, SourceType.Enginee) & builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetReferChartDesc();
            try
            {
                //检查是否已保存与该描述相同的记录
                var query = col.Find(filter).Project(x => new ReferChartDescDto
                {
                    Id = x._id.ToString(),
                    DescList = x.DescList
                }).FirstOrDefault();
                return query;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 删除描述信息
        /// </summary>
        /// <param name="descId">描述Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelReferChartDesc(string descId)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<ReferChartDescMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(descId));
            filter &= builder.Eq(x => x.Type, SourceType.Enginee) & builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetReferChartDesc();
            try
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
                col.UpdateOne(filter, update);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region 域名关系图
        /// <summary>
        /// 获取域名关系图谱
        /// </summary>
        /// <param name="prjId">项目Id</param>
        /// <param name="timeInterval">时间间隔，0为全部，1为月，2为季，3为年</param>
        /// <returns></returns>
        [HttpGet]
        public TimeLinkRefer GetDomainReference(string prjId, int timeInterval)
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
            var builderCate = Builders<Dnl_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(prjId));
            filterCate &= builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
            var queryCate = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => new
            {
                Id = x._id,
                Name = x.Name
            }).ToList();
            var cateObjIdList = queryCate.Select(x => x.Id).ToList();      //词组ObjectId列表
            //建立分组信息
            var cateInfo = new List<LinkReferCate>();
            foreach (var x in queryCate)
            {
                var cate = new LinkReferCate();
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
            var builderMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterMap = builderMap.In(x => x.CategoryId, cateObjIdList) & builderMap.Eq(x => x.IsDel, false);
            var queryKey = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).Project(x => new
            {
                KeywordId = x.KeywordId.ToString(),
                CategoryId = x.CategoryId,
                Keyword = x.Keyword,
            }).ToList();
            var keyIdList = queryKey.Select(x => x.KeywordId).ToList();    //关键词ObjectId列表

            //建议关键词与词组字典
            var keyToCate = new Dictionary<string, string>();
            //var keyGroup = queryKey.GroupBy(x => x.KeywordId).ToList();
            queryKey = queryKey.DistinctBy(x => x.KeywordId).ToList();
            foreach (var key in queryKey)
            {
                var cateId = queryCate.Find(x => x.Id.Equals(key.CategoryId)).Id.ToString();
                keyToCate.Add(key.KeywordId, cateId);
            }

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(prjId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIdList = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            //获取链接信息
            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIdList);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIdList);
            var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Title = x.Title,
                Keyword = x.Keywords,
                KeywordId = x.SearchkeywordId,
                DCNum = x.DCNum,
                Domain = x.Domain
            }).ToList();

            //建立节点信息
            var linkNodes = new List<LinkReferNode>();         //节点信息
            var linkByDomain = queryLink.GroupBy(x => x.Domain).ToList();
            int index = 0;
            for (int i = 0; i < linkByDomain.Count; i++)
            {
                //获取链接信息
                var links = linkByDomain[i].ToList();
                var domain = new LinkReferNode
                {
                    value = 1,
                    SortNum = queryLink[i].DCNum,
                    Index = index++,
                    name = linkByDomain[i].Key,
                };

                var tempLinks = links.DistinctBy(x => x.KeywordId);
                domain.keyWordCount = tempLinks.Count;
                domain.keyWordList = tempLinks.Select(x => x.Keyword).ToList();
                domain.keyWordIdList = tempLinks.Select(x => x.KeywordId).ToList();

                //获取归属组序号
                var cateId = keyToCate[queryLink[i].KeywordId];
                var cateIndex = cateInfo.FindIndex(s => s.id == cateId);
                domain.category = cateIndex;
                linkNodes.Add(domain);
            }

            //建立时间坐标并将链接信息按时间分组
            DateTime startTime = linkNodes.Min(x => x.publishTime);
            DateTime endTime = linkNodes.Max(x => x.publishTime);
            result.DateTimeList = new List<DateTime>();
            int year = startTime.Year;
            int month = startTime.Month;
            int season = (month - 1) / 3;
            var timeLinkNodeList = new List<List<LinkReferNode>>();        //按时间分组的链接节点信息
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
                            var tempNode = new List<LinkReferNode>();
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
                            var tempNode = new List<LinkReferNode>();
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
                            var tempNode = new List<LinkReferNode>();
                            tempNode = linkNodes.Where(x => x.publishTime >= time && x.publishTime < time.AddYears(1)).ToList();
                            timeLinkNodeList.Add(tempNode);
                        }
                        break;
                    }
                default:
                    return null;
            }
            result.ReferList = new List<LinkReferDto>();
            for (int i = 0; i < result.DateTimeList.Count; i++)
            {
                var referData = ComputerDomainRefer(timeLinkNodeList[i], cateInfo, keyIdList);
                result.ReferList.Add(referData);
            }

            sw.Stop();
            return result;

        }

        /// <summary>
        /// 计算节点关系
        /// </summary>
        /// <param name="linkNodeList">节点信息</param>
        /// <param name="cateInfo">分组信息</param>
        /// <param name="keyIdList">关键词Id列表</param>
        /// <returns></returns>
        [HttpGet]
        LinkReferDto ComputerDomainRefer(List<LinkReferNode> linkNodeList, List<LinkReferCate> cateInfo, List<string> keyIdList)
        {
            LinkReferDto result = new LinkReferDto();
            //建立节点关系
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var linkReferList = new List<LinkReferIn2Node>();        //节点间关系
            //按关键词分组生成小集群列表
            var linkByKeyList = new List<LinkReferByKey>();         //关键词链接信息列表
            //var centerNodeList = new List<LinkReferNode>();         //核心节点列表
            foreach (var keyId in keyIdList)
            {
                var nodeList = linkNodeList.FindAll(x => x.keyWordIdList.Contains(keyId));
                if (nodeList.Count > 0)
                {
                    var linkByKey = new LinkReferByKey
                    {
                        KeywordId = keyId,
                        NodeList = nodeList
                    };
                    linkByKeyList.Add(linkByKey);
                }
            }
            //遍历计算关系
            for (int i = 0; i < linkByKeyList.Count; i++)
            {
                string keyId = linkByKeyList[i].KeywordId;
                var nodeList = linkByKeyList[i].NodeList;
                //计算集群内部关系
                var center = nodeList.OrderByDescending(x => x.SortNum).FirstOrDefault();     //选取核心点
                //centerNodeList.Add(center);
                foreach (var node in nodeList)
                {
                    if (node.name == center.name)
                        continue;
                    var refer = new LinkReferIn2Node
                    {
                        source = center.Index,
                        target = node.Index
                    };
                    linkReferList.Add(refer);
                }
            }

            #region 旧节点关系计算优化方法（废弃）
            //for (int i = 0; i < linkNodeList.Count; i++)
            //{
            //    for (int j = i + 1; j < linkNodeList.Count; j++)
            //    {
            //        //判断i和j两个节点是否有关联
            //        bool isRefer = false;
            //        foreach (var keyId in linkNodeList[i].keyWordIdList)
            //        {
            //            if (isRefer)
            //            {
            //                break;
            //            }
            //            if (linkNodeList[j].keyWordIdList.Contains(keyId))
            //            {
            //                var refer = new LinkReferIn2Node
            //                {
            //                    source = i,
            //                    target = j
            //                };
            //                linkReferList.Add(refer);
            //                isRefer = true;
            //            }
            //        }
            //    }
            //}
            ////清理重复集群关系
            //var tempGroup = linkReferList.GroupBy(x => x.source).ToList();              //统计获取集群中心点
            //var tempCount = tempGroup.Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();               //统计获取集群中心点
            //var usedIndex = new List<int>();    //已清理节点
            //foreach (var x in tempCount)
            //{
            //    if (usedIndex.Contains(x.Key))
            //    {
            //        continue;
            //    }
            //    var centerNode = linkNodeList[x.Key];      //集群中心点
            //    var referNodeList = tempGroup.Find(s => s.Key == x.Key);
            //    var referIndexs = referNodeList.Select(s => s.target).ToList();      //集群内所有相关点位置
            //    foreach (var y in referNodeList)            //遍历集群相关点
            //    {
            //        var referNode = linkNodeList[y.target];    //单个相关点
            //        if (centerNode.category != referNode.category)
            //        {
            //            continue;
            //        }
            //        //清除相关点与集群内其他点的关系
            //        var referInfos = linkReferList.FindAll(s => s.source == y.target);
            //        var tempIndexs = referInfos.Select(s => s.target).ToList();     //相关点其所有关系节点位置
            //        foreach (var z in referInfos)
            //        {
            //            if (referIndexs.Contains(z.target))
            //            {
            //                var targetRefer = linkReferList.FindAll(s => s.source == z.target).Select(s => s.target).ToList();     //当前对应相关点的关系节点其自身所有关系节点位置
            //                //判断关系节点是否在同一集群
            //                if (targetRefer.All(s => referIndexs.Contains(s)))
            //                {
            //                    linkReferList.Remove(z);
            //                }
            //            }
            //        }
            //        usedIndex.Add(y.target);
            //    }
            //}
            #endregion

            sw.Stop();
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
            foreach (var x in linkNodeList)
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
            foreach (var x in linkReferList)
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
            var topIndex = linkReferList.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
            result.TopData = new List<LinkReferNode>();
            foreach (var x in topIndex)
            {
                var data = new LinkReferNode(linkNodeList[x.Key]);
                data.value = x.Count;
                result.TopData.Add(data);
            }
            if (result.TopData.Count < 10)
            {
                foreach (var x in topIndex)
                {
                    //获取集群内其他点
                    var temp = linkReferList.FindAll(s => s.source == x.Key);
                    foreach (var y in temp)
                    {
                        var data = new LinkReferNode(linkNodeList[y.target]);
                        data.value = linkReferList.Count(s => s.source == y.target);
                        result.TopData.Add(data);
                        if (result.TopData.Count == 10)
                            break;
                    }
                    if (result.TopData.Count == 10)
                        break;
                }
                //获取孤立结点
                foreach (var x in linkNodeList)
                {
                    if (result.TopData.Count == 10)
                        break;
                    if (!result.TopData.Contains(x))
                    {
                        var data = new LinkReferNode(x);
                        data.value = 1;
                        result.TopData.Add(data);
                    }
                }
            }


            //获取包含关键词最多的10个节点
            result.TopKeyData = new List<LinkReferNode>();
            var tpData = linkNodeList.OrderByDescending(x => x.keyWordCount).Take(10).ToList();
            for (int i = 0; i < tpData.Count; i++)
            {
                var data = new LinkReferNode(tpData[i]);
                data.value = data.keyWordCount;
                result.TopKeyData.Add(data);
            }

            //获取组外链接数最多的10个节点
            var linkGroupCount = new List<LinkGroupCount>();
            for (int i = 0; i < linkNodeList.Count; i++)
            {
                int count = 0;
                //获取该结点所有相关节点
                var referIndexs = linkReferList.FindAll(x => x.source == i).Select(x => x.target).ToList();
                referIndexs.AddRange(linkReferList.FindAll(x => x.target == i).Select(x => x.source).ToList());
                referIndexs = referIndexs.Distinct().ToList();
                foreach (var x in referIndexs)
                {
                    if (linkNodeList[i].category != linkNodeList[x].category)
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
            result.TopCateData = new List<LinkReferNode>();
            foreach (var x in linkGroupCount)
            {
                var data = new LinkReferNode(linkNodeList[x.Pos]);
                data.value = x.Count;
                result.TopCateData.Add(data);
            }

            //获取词组影响力
            result.CateWeights = new List<CategoryWeight>();
            for (int i = 0; i < cateInfo.Count; i++)
            {
                var cateWeight = new CategoryWeight();
                cateWeight.Category = cateInfo[i].name;
                topIndex = linkReferList.GroupBy(x => x.source).Select(x => new { Key = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();
                int referNum = 0;
                int keywordNum = 0;
                for (int j = 0; j < linkNodeList.Count; j++)
                {
                    if (linkNodeList[j].category == i)
                    {
                        referNum += linkReferList.Where(x => x.source == j || x.target == j).Count();
                        keywordNum += linkNodeList[j].keyWordCount;
                    }
                }
                cateWeight.Weight = keywordNum * 99 + referNum * 12;
                result.CateWeights.Add(cateWeight);
            }
            result.CateWeights = result.CateWeights.OrderByDescending(x => x.Weight).ToList();

            return result;
        }
        #endregion

        #region 命名实体
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
                PicUrl = post.PicUrl,
                IsDel = false
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
                CreatedAt = x.CreatedAt,
                PicUrl=x.PicUrl
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
        /// 获取某个实体的子孙结点列表
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
        /// 新建实体树与项目映射
        /// </summary>
        /// <param name="entityId">实体树Id</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertEntityMapping(string entityId, string projectId, string userId)
        {
            var result = new ResultDto();
            try
            {
                var enObjIds = GetObjIdListFromStr(entityId);
                var proObjId=new ObjectId(projectId);
                var userObjId = new ObjectId(userId);
                //判断该实体树和项目的对应关系是否已存在
                var builder=Builders<Dnl_EntityTreeMapping>.Filter;
                var col = MongoDBHelper.Instance.GetDnl_EntityTreeMapping();
                foreach (var enObjId in enObjIds)
                {
                    var filter = builder.Eq(x => x.ProjectId, proObjId) & builder.Eq(x => x.EntityId, enObjId) & builder.Eq(x => x.UsrId, userObjId);
                    var query = col.Find(filter).FirstOrDefault();
                    if (query != null)
                    {
                        if (enObjIds.Count == 1)
                        {
                            result.Message = "该项目与该实体已绑定！";
                            return result;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var map = new Dnl_EntityTreeMapping
                        {
                            UsrId = userObjId,
                            ProjectId = proObjId,
                            EntityId = enObjId,
                            CreatedAt = DateTime.Now.AddHours(8)
                        };
                        col.InsertOne(map);
                    }
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpGet]
        public List<Dnl_EntityTreeMappingDto> GetEntityMapping(string projectId)
        {
            var mappings = new List<Dnl_EntityTreeMappingDto>();
            var proObjId=new ObjectId(projectId);
            var filterMap = Builders<Dnl_EntityTreeMapping>.Filter.Eq(x => x.ProjectId, proObjId);
            var queryMap = MongoDBHelper.Instance.GetDnl_EntityTreeMapping().Find(filterMap).ToList();

            var entityObjIds = queryMap.Select(x => x.EntityId).ToList();
            var filterEntity = Builders<Dnl_EntityTree>.Filter.In(x=>x._id,entityObjIds);
            var queryEntity = MongoDBHelper.Instance.GetDnl_EntityTree().Find(filterEntity).ToList();
            var project = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, proObjId)).FirstOrDefault();
            foreach (var x in queryMap)
            {
                var map = new Dnl_EntityTreeMappingDto
                {
                    CreatedAt = x.CreatedAt,
                    Id = x._id.ToString(),
                    ProjectId = projectId,
                    ProjectName = project.Name
                };
                map.EntityName=queryEntity.Find(s=>s._id==x.EntityId).Entity.Key;
                mappings.Add(map);
            }
            return mappings;
        }

        /// <summary>
        /// 删除实体树与项目映射
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelEntityMapping(string mappingId)
        {
            var result = new ResultDto();
            try
            {
                var filter = Builders<Dnl_EntityTreeMapping>.Filter.Eq(x => x._id, new ObjectId(mappingId));
                MongoDBHelper.Instance.GetDnl_EntityTreeMapping().DeleteOne(filter);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 话语抽取
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<TextExtract> TextExtractByEntity(string userId, string projectId)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var userObjId = new ObjectId(userId);
            var proObjId = new ObjectId(projectId);
            //获取该项目绑定的实体树ID
            var builderMap = Builders<Dnl_EntityTreeMapping>.Filter;
            var filterMap = builderMap.Eq(x => x.ProjectId, proObjId);
            var entityIds = MongoDBHelper.Instance.GetDnl_EntityTreeMapping().Find(filterMap).Project(x=>x.EntityId.ToString()).ToList();
            //获取该用户的所有实体树信息
            var filterEn = Builders<Dnl_EntityTree>.Filter.Eq(x => x.UsrId, userObjId);
            var queryEn = MongoDBHelper.Instance.GetDnl_EntityTree().Find(filterEn).Project(x => new Dnl_EntityTreeDto
                    {
                        Id = x._id.ToString(),
                        Attributes = x.Attributes,
                        CreatedAt = x.CreatedAt,
                        Entity = x.Entity,
                        ParentId = x.ParentId.ToString(),
                        PicUrl = x.PicUrl
                    }).ToList();
            //获取该项目绑定实体树内的所有实体结点
            var entitys = new List<Dnl_EntityTreeDto>();
            var temp = queryEn.FindAll(x => entityIds.Contains(x.Id));
            entitys.AddRange(temp);
            //foreach (var x in temp)
            //{
            //    var children = GetEntitySubtree(queryEn, x.Id);
            //    entitys.AddRange(children);
            //}
            entitys = entitys.DistinctBy(x => x.Id);

            /* 获取Top20网页关联数的链接 */
            //获取链接信息
            var builderKeyMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterKeyMap = builderKeyMap.Eq(x => x.ProjectId, proObjId) & builderKeyMap.Eq(x => x.IsDel, false);
            var keyIds = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterKeyMap).Project(x => x.KeywordId.ToString()).ToList();

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIds);
            var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Id=x._id,
                Title = x.Title,
                Description = x.Description,
                Keword = x.Keywords,
                KeywordId = x.SearchkeywordId,
                PublishTime = x.PublishTime,
                //Html=x.Html,
                LinkUrl = x.LinkUrl
            }).ToList();

            //建立节点信息
            var linkNodeList = new List<LinkReferNode>();         //节点信息
            for (int i = 0; i < queryLink.Count; i++)
            {
                //未存在发布信息时跳过该链接
                DateTime tpdt = new DateTime();
                DateTime.TryParse(queryLink[i].PublishTime, out tpdt);
                if (tpdt < new DateTime(1753, 1, 09) || tpdt > DateTime.Now)
                {
                    continue;
                }
                //获取链接信息
                var link = new LinkReferNode();
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
                var repeat = queryLink.FindAll(s => s.LinkUrl == queryLink[i].LinkUrl).DistinctBy(s => s.KeywordId);

                link.keyWordCount = repeat.Count;
                link.keyWordList = new List<string>();
                link.keyWordIdList = new List<string>();
                //移除重复的链接
                foreach (var y in repeat)
                {
                    link.keyWordList.Add(y.Keword);
                    link.keyWordIdList.Add(y.KeywordId);
                    if (i == queryLink.Count)
                        i--;
                    if (queryLink[i].KeywordId != y.KeywordId)
                    {
                        queryLink.Remove(y);
                    }
                }
                link.describe = queryLink[i].Description;
                linkNodeList.Add(link);
            }

            //计算网页关联数
            for (int i = 0; i < linkNodeList.Count; i++)
            {
                for (int j = i + 1; j < linkNodeList.Count; j++)
                {
                    //判断i和j两个节点是否有关联
                    bool isRefer = false;
                    foreach (var keyId in linkNodeList[i].keyWordIdList)
                    {
                        if (isRefer)
                        {
                            break;
                        }
                        if (linkNodeList[j].keyWordIdList.Contains(keyId))
                        {
                            linkNodeList[i].value++;
                            isRefer = true;
                        }
                    }
                }
            }
            linkNodeList = linkNodeList.OrderByDescending(x => x.value).Take(50).ToList();

            sw.Stop();
            //匹配抽取文本信息
            var extracts = new List<TextExtract>();
            var linkObjIds = new List<ObjectId>();
            foreach (var entity in entitys)
            {
                var ex = new TextExtract();
                ex.Entity = entity;
                //string regStr = "";         //匹配上的实体名称或变体
                var htmls = new List<string>();
                ex.LinkIds = new List<ObjectId>();
                ex.regStrs = new List<string>();
                foreach (var link in linkNodeList)
                {
                    if ((link.name != null && link.name.Contains(entity.Entity.Key)) || (link.describe != null && link.describe.Contains(entity.Entity.Key)))
                    {
                        linkObjIds.Add(queryLink.Find(x => x.LinkUrl == link.linkUrl).Id);
                        ex.LinkIds.Add(queryLink.Find(x => x.LinkUrl == link.linkUrl).Id);
                        string regStr = entity.Entity.Key;
                        ex.regStrs.Add(regStr);
                    }
                    else
                    {
                        //匹配实体名称变体
                        foreach (var varient in entity.Entity.Varients)
                        {
                            if ((link.name != null && link.name.Contains(varient)) || (link.describe!=null&&link.describe.Contains(varient)))
                            {
                                linkObjIds.Add(queryLink.Find(x => x.LinkUrl == link.linkUrl).Id);
                                ex.LinkIds.Add(queryLink.Find(x => x.LinkUrl == link.linkUrl).Id);
                                string regStr = varient;
                                ex.regStrs.Add(regStr);
                                break;
                            }
                        }
                    }
                }
                if (ex.LinkIds.Count == 0)
                    continue;
                extracts.Add(ex);
            }
            /* 话语提取 */
            //获取链接信息
            filterLink = builderLink.In(x => x._id, linkObjIds);
            var linkInfo = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Id = x._id,
                Html = x.Html,
                Title=x.Title,
                Url=x.LinkUrl
            }).ToList();
            foreach (var ex in extracts)
            {
                ex.Text = new List<TextInfo>();
                var htmls = linkInfo.FindAll(x => ex.LinkIds.Contains(x.Id));       //获取链接内容
                foreach (var html in htmls)
                {
                    //一个实体最多匹配10个链接
                    if (ex.Text.Count == 10)
                        break;

                    int pos = ex.LinkIds.IndexOf(html.Id);
                    string text = LinkTextExtract(html.Html, ex.regStrs[pos]);
                    if (!string.IsNullOrEmpty(text))
                    {
                        //去重
                        int i = ex.Text.FindIndex(x => x.Content == text);
                        if (i != -1)
                            continue;

                        var info = new TextInfo();
                        info.Title = html.Title;
                        info.Url = html.Url;
                        info.Content = text;
                        ex.Text.Add(info);
                    }
                }
            }
            extracts.RemoveAll(x => x.Text.Count == 0);
            return extracts;
        }


        /// <summary>
        /// 话语提取
        /// </summary>
        /// <param name="html">链接地址</param>
        /// <param name="regStr">匹配词</param>
        /// <returns></returns>
        [HttpGet]
        public string LinkTextExtract(string html, string regStr)
        {
            string result="";
            //string html = GetMainContentHelper.getDataFromUrl(url);
            if (html != string.Empty)
            {
                string text = GetMainContentHelper.GetMainContent(html);
                //将文本按句拆分开
                var sentences = text.Split('.', '?', '。', '？');
                foreach (var x in sentences)
                {
                    if (result.Length > 100)
                        break;
                    if (x.Contains(regStr))
                    {
                        result += x + "。";
                    }
                }
            }
            return result;
        }
        #endregion

        #region 链接报表
        /// <summary>
        /// 获取链接报表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<LinkInfo> GetLinkReport(string userId,string projectId)
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
            var cateObjIds=queryCate.Select(x=>new ObjectId(x.Id)).ToList();
            var builderDomain = Builders<IW2S_DomainCategoryData>.Filter;
            var filterDomain = builderDomain.In(x => x.DomainCategoryId, cateObjIds);
            var queryDomain = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(filterDomain).Project(x=>new{
                Id=x._id.ToString(),
                CateId=x.DomainCategoryId.ToString(),
                Domain=x.DomainName
            }).ToList();
            queryDomain=queryDomain.DistinctBy(x=>x.Id).ToList();
            foreach (var x in queryDomain)
            {
                if (!domainToCate.ContainsKey(x.Domain))
                {
                    var cateName = queryCate.Find(s => s.Id == x.CateId).Name;
                    domainToCate.Add(x.Domain, cateName);
                }
            }

            //获取链接信息
            var builderMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterMap = builderMap.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderMap.Eq(x => x.IsDel, false);
            var keyIds = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterMap).Project(x => x.KeywordId.ToString()).ToList();

            //获取项目内已删除的链接Id
            var builderLinkMap = Builders<Dnl_LinkMapping_Baidu>.Filter;
            var filterLinkMap = builderLinkMap.Eq(x => x.ProjectId, new ObjectId(projectId)) & builderLinkMap.Eq(x => x.DataCleanStatus, (byte)2);
            var exLinkObjIds = MongoDBHelper.Instance.GetDnl_LinkMapping_Baidu().Find(filterLinkMap).Project(x => x.LinkId).ToList();       //项目中已删除的链接ID列表

            var builderLink = Builders<Dnl_Link_Baidu>.Filter;
            var filterLink = builderLink.In(x => x.SearchkeywordId, keyIds);
            filterLink &= builderLink.Nin(x => x._id, exLinkObjIds);
            var queryLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterLink).Project(x => new
            {
                Id = x._id.ToString(),
                Domain = x.Domain,
                Title = x.Title,
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
                    关键词 = new List<string>(),
                    年 = dt.Year,
                    月 = dt.Month,
                    日 = dt.Day,
                    链接标题 = title,
                    评论数 = 0
                };
                link.关键词.Add(x.Keyword);
                linkInfos.Add(link);
            }

            //去重并获取域名分组
            for (int i = 0; i < linkInfos.Count; i++)
            {
                var repeats = linkInfos.FindAll(x => x.链接标题 == linkInfos[i].链接标题);
                if (repeats.Count > 1)
                {
                    repeats.Remove(linkInfos[i]);
                    foreach (var x in repeats)
                    {
                        linkInfos[i].关键词.AddRange(x.关键词);
                        linkInfos.Remove(x);
                    }
                }
                //统计关键词数
                linkInfos[i].命中关键词数 = linkInfos[i].关键词.Count;
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

            //计算网页关联数
            for (int i = 0; i < linkInfos.Count; i++)
            {
                int num = 0;
                for (int j = i + 1; j < linkInfos.Count; j++)
                {
                    //判断i和j两个节点是否有关联
                    bool isRefer = false;
                    foreach (var keyword in linkInfos[i].关键词)
                    {
                        if (isRefer)
                        {
                            break;
                        }
                        if (linkInfos[j].关键词.Contains(keyword))
                        {
                            num++;
                            isRefer = true;
                        }
                    }
                }
                linkInfos[i].网页关联数 = num;
                linkInfos[i].链接影响力 = num + linkInfos[i].命中关键词数 * 5;
            }

            return linkInfos;
        }
        #endregion

        #region 备份
        /// <summary>
        /// 备份项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
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

            #region 百度
            /* 克隆所有关键词 */
            //获取原有所有关键词映射
            var builderKey = Builders<Dnl_KeywordMapping>.Filter;
            var colKey = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            var filterKey = builderKey.Eq(s => s.CategoryId, ObjectId.Empty);
            filterKey &= builderKey.Eq(x => x.ProjectId, sourceProId);
            filterKey &= builderKey.Eq(x => x.IsDel, false);
            var soureKeys = colKey.Find(filterKey).ToList();
            //克隆所有关键词映射
            var cloneKeys = new List<Dnl_KeywordMapping>();
            foreach (var y in soureKeys)
            {
                var key = y;
                key._id = ObjectId.GenerateNewId();
                key.ProjectId = backupPro._id;
                key.UserId = backupPro.UsrId;
                cloneKeys.Add(key);
            }
            if (cloneKeys.Count > 0)
                colKey.InsertMany(cloneKeys);

            //克隆关键词分组
            BaiduBackupKeywordCate(ObjectId.Empty, ObjectId.Empty, sourceProId, backupPro._id, backupPro.UsrId);
            #endregion

            #region 微信
            /* 克隆所有关键词 */
            //获取原有所有关键词映射
            var WXbuilderKey = Builders<MediaKeywordMappingMongo>.Filter;
            var WXcolKey = MongoDBHelper.Instance.GetMediaKeywordMapping();
            var WXfilterKey = WXbuilderKey.Eq(s => s.CategoryId, ObjectId.Empty);
            WXfilterKey &= WXbuilderKey.Eq(x => x.ProjectId, sourceProId);
            WXfilterKey &= WXbuilderKey.Eq(x => x.IsDel, false);
            var WXsoureKeys = WXcolKey.Find(WXfilterKey).ToList();
            //克隆所有关键词映射
            var WXcloneKeys = new List<MediaKeywordMappingMongo>();
            foreach (var y in WXsoureKeys)
            {
                var key = y;
                key._id = ObjectId.GenerateNewId();
                key.ProjectId = backupPro._id;
                key.UserId = backupPro.UsrId;
                WXcloneKeys.Add(key);
            }
            if (WXcloneKeys.Count > 0)
                WXcolKey.InsertMany(WXcloneKeys);

            //克隆关键词分组
            WeiXinBackupKeywordCate(ObjectId.Empty, ObjectId.Empty, sourceProId, backupPro._id, backupPro.UsrId);
            #endregion

            //克隆折线图设置
            var builderChart = Builders<IW2S_ChartConfig>.Filter;
            var filterChart = builderChart.Eq(x => x.IsDel, false) & builderChart.Eq(x => x.ChartType, 0);
            filterChart &= builderChart.Eq(x => x.ProjectId, sourceProId);
            var colChart = MongoDBHelper.Instance.GetIW2S_ChartConfig();
            var sourceCharts = colChart.Find(filterChart).ToList();
            if (sourceCharts.Count > 0)
            {
                var cloneCharts = new List<IW2S_ChartConfig>();
                foreach (var x in sourceCharts)
                {
                    var chart = x;
                    chart._id = ObjectId.GenerateNewId();
                    chart.ProjectId = backupPro._id;
                    chart.UsrId = backupPro.UsrId;
                    cloneCharts.Add(chart);
                }
                colChart.InsertMany(cloneCharts);
            }

            //克隆项目链接数变化数组
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
                    chart.UsrId = backupPro.UsrId;
                    backupProLink.Add(chart);
                }
                colProLink.InsertMany(backupProLink);
            }

            ResultDto result = new ResultDto();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 迭代克隆百度关键词组及组内关键词
        /// </summary>
        /// <param name="soureCateObjId">原父分组Id</param>
        /// <param name="cloneCateObjId">克隆的父分组Id</param>
        /// <param name="sourceProObjId">原项目Id</param>
        /// <param name="cloneProObjId">克隆的项目Id</param>
        /// <param name="cloneUserObjId">克隆的用户Id</param>
        private void BaiduBackupKeywordCate(ObjectId soureCateObjId, ObjectId cloneCateObjId, ObjectId sourceProObjId, ObjectId cloneProObjId, ObjectId cloneUserObjId)
        {
            //获取原子分组信息
            var builderCate = Builders<Dnl_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ProjectId, sourceProObjId);
            filterCate &= builderCate.Eq(x => x.ParentId, soureCateObjId);
            var colCate = MongoDBHelper.Instance.GetDnl_KeywordCategory();
            var sourceCates = colCate.Find(filterCate).ToList();

            var builderKey = Builders<Dnl_KeywordMapping>.Filter;
            var colKey = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            foreach (var x in sourceCates)
            {
                var sourceId = x._id;
                //克隆子分组信息
                var cloneCate = x;
                cloneCate._id = ObjectId.GenerateNewId();
                cloneCate.ProjectId = cloneProObjId;
                cloneCate.ParentId = cloneCateObjId;
                cloneCate.UserId = cloneUserObjId;
                colCate.InsertOne(cloneCate);
                //克隆子分组内关键词
                var filterKey = builderKey.Eq(s => s.CategoryId, sourceId);
                var soureKeys = colKey.Find(filterKey).ToList();
                var cloneKeys = new List<Dnl_KeywordMapping>();
                foreach (var y in soureKeys)
                {
                    var key = y;
                    key._id = ObjectId.GenerateNewId();
                    key.CategoryId = cloneCate._id;
                    key.ParentCategoryId = cloneCateObjId;
                    key.ProjectId = cloneProObjId;
                    key.UserId = cloneUserObjId;
                    cloneKeys.Add(key);
                }
                colKey.InsertMany(cloneKeys);

                //迭代克隆孙分组
                BaiduBackupKeywordCate(sourceId, cloneCate._id, sourceProObjId, cloneProObjId, cloneUserObjId);
            }

        }

        /// <summary>
        /// 迭代克隆微信关键词组及组内关键词
        /// </summary>
        /// <param name="soureCateObjId">原父分组Id</param>
        /// <param name="cloneCateObjId">克隆的父分组Id</param>
        /// <param name="sourceProObjId">原项目Id</param>
        /// <param name="cloneProObjId">克隆的项目Id</param>
        /// <param name="cloneUserObjId">克隆的用户Id</param>
        private void WeiXinBackupKeywordCate(ObjectId soureCateObjId, ObjectId cloneCateObjId, ObjectId sourceProObjId, ObjectId cloneProObjId, ObjectId cloneUserObjId)
        {
            //获取原子分组信息
            var builderCate = Builders<MediaKeywordCategoryMongo>.Filter;
            var filterCate = builderCate.Eq(x => x.IsDel, false) & builderCate.Eq(x => x.ProjectId, sourceProObjId);
            filterCate &= builderCate.Eq(x => x.ParentId, soureCateObjId);
            var colCate = MongoDBHelper.Instance.GetMediaKeywordCategory();
            var sourceCates = colCate.Find(filterCate).ToList();

            var builderKey = Builders<MediaKeywordMappingMongo>.Filter;
            var colKey = MongoDBHelper.Instance.GetMediaKeywordMapping();
            foreach (var x in sourceCates)
            {
                var sourceId = x._id;
                //克隆子分组信息
                var cloneCate = x;
                cloneCate._id = ObjectId.GenerateNewId();
                cloneCate.ProjectId = cloneProObjId;
                cloneCate.ParentId = cloneCateObjId;
                cloneCate.UserId = cloneUserObjId;
                colCate.InsertOne(cloneCate);
                //克隆子分组内关键词
                var filterKey = builderKey.Eq(s => s.CategoryId, sourceId);
                var soureKeys = colKey.Find(filterKey).ToList();
                var cloneKeys = new List<MediaKeywordMappingMongo>();
                foreach (var y in soureKeys)
                {
                    var key = y;
                    key._id = ObjectId.GenerateNewId();
                    key.CategoryId = cloneCate._id;
                    key.ParentCategoryId = cloneCateObjId;
                    key.ProjectId = cloneProObjId;
                    key.UserId = cloneUserObjId;
                    cloneKeys.Add(key);
                }
                colKey.InsertMany(cloneKeys);

                //迭代克隆孙分组
                WeiXinBackupKeywordCate(sourceId, cloneCate._id, sourceProObjId, cloneProObjId, cloneUserObjId);
            }

        }
        #endregion      

        #region ID拆分
        /// <summary>
        /// 拆分为字符串Id列表
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        private List<string> GetIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").ToList();
        }

        /// <summary>
        /// 拆分为ObjectId列表
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        private List<ObjectId> GetObjIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
        }
        #endregion

    }
    

}
