using IW2S.Helpers;
using IW2S.Models;
using IW2S.Models.Emarknow;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
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
    public class EMNController : ApiController
    {
        //获取关键词
        [HttpGet]
        public List<FreeTaskDto> GetKeyword(string user_id, string projectId)
        {
            //设置筛选条件，以user_id为条件
            var builder = Builders<FreeTask>.Filter;
            var fliter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));

            //获取关键词，并以CreateAt列进行排序
            var TaskList = MongoDBHelper.Instance.Get_FreeTask().Find(fliter).SortByDescending(x => x.CreatedAt).ToList();
            //新建空白FreeTaskDto属性类
            List<FreeTaskDto> list = new List<FreeTaskDto>();
            //将获取的关键词存入list中
            foreach (var item in TaskList)
            {
                FreeTaskDto v = new FreeTaskDto();
                v._id = item._id.ToString();
                v.IsStarted = item.IsStarted;
                v.TaskName = item.TaskName;
                v.IsBot = item.IsBot;
                v.BotIntervalHours = item.BotIntervalHours;
                v.recordNum = item.recordNum;
                v.drag = true;
                list.Add(v);
            }
            return list;
        }

        //插入关键词
        [HttpGet]
        public ResultDto insertKeyword(string user_id, string keywords, double? MLP, int IntervalHours, string projectId)
        {
            //查询关键词是否存在
            var keywordList = keywords.Split(';', '；');
            ResultDto result = new ResultDto();
            var builder = Builders<FreeTask>.Filter;
            var col = MongoDBHelper.Instance.Get_FreeTask();

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId))).Project(x => x.UsrId).FirstOrDefault();

            foreach (var keyword in keywordList)
            {
                if (string.IsNullOrEmpty(keyword)) continue;
                var filter = builder.Eq(x => x.TaskName, keyword);
                filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
                var dto = MongoDBHelper.Instance.Get_FreeTask().Find(filter).FirstOrDefault();
                if (dto != null)
                {
                    result.Message += "关键词‘" + keyword + "’已经存在！";
                }
                //添加关键词
                FreeTask kw = new FreeTask
                {
                    _id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now.AddHours(8),
                    UId = usrObjId,
                    IsBot = false,
                    TaskName = keyword,
                    IsStarted = true,
                    MLP = MLP,
                    BotIntervalHours = IntervalHours,
                    recordNum = 0,
                    ProjectId = new ObjectId(projectId)
                };
                MongoDBHelper.Instance.Get_FreeTask().InsertOne(kw);
                ////将添加的关键词信息返回
                //FreeTaskDto fdto = new FreeTaskDto
                //{
                //    _id = kw._id.ToString(),
                //    CreatedAt = kw.CreatedAt,
                //    UId = kw.UId.ToString(),
                //    IsBot = kw.IsBot,
                //    TaskName = kw.TaskName,
                //    IsStarted = kw.IsStarted,
                //    MLP = kw.MLP,
                //    BotIntervalHours = kw.BotIntervalHours,
                //    recordNum = kw.recordNum,
                //    ProjectId = kw.ProjectId.ToString()
                //};
            }

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(projectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.emarketnow
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        //删除关键词
        [HttpGet]
        public string removeKeyword(string user_id, string keyid, string projectId)
        {
            //设置查询条件
            var builder = Builders<FreeTask>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(keyid));
            filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //删除关键词
            var result = MongoDBHelper.Instance.Get_FreeTask().DeleteOne(filter).DeletedCount;
            //??
            var queryTask = new QueryDocument { { "_id", new ObjectId(keyid) }, { "_UId", new ObjectId(user_id) } };
            var ssdfs = MongoDBHelper.Instance.Get_FreeBotItem().DeleteMany(queryTask).DeletedCount;
            return result > 0 ? "成功！" : "失败！";
        }

        //启动关键词搜索
        [HttpGet]
        public string startKeywords(string kids, string projectId)
        {
            var list = kids.Split(',');
            if (list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "IsStarted", true } } } };
                    MongoDBHelper.Instance.Get_FreeTask().UpdateOne(new QueryDocument { { "_id", new ObjectId(list[i]) } }, update);
                }
            }
            return "成功！";
        }

        //获取关键词，根据时间分组
        [HttpGet]
        public List<KeywordVO> GetkeywordByTime(string user_id, string projectId)
        {
            //查询关键词
            var builder = Builders<FreeTask>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var TaskList = MongoDBHelper.Instance.Get_FreeTask().Find(filter).ToList();
            //将关键词以CreateAt列升序排列
            List<FreeTaskDto> list = new List<FreeTaskDto>();
            foreach (var item in TaskList)
            {
                FreeTaskDto v = new FreeTaskDto();
                v._id = item._id.ToString();
                v.TaskName = item.TaskName;
                v.IsStarted = item.IsStarted;
                v.CreatedAt = item.CreatedAt;
                v.drag = true;
                v.TimeAt = Convert.ToDateTime(item.CreatedAt).ToString("yyyy/MM/dd");
                list.Add(v);
            }
            //将关键词列表转为KeywordVO形式
            List<KeywordVO> am = new List<KeywordVO>();
            foreach (var item in list.OrderBy(x => x.CreatedAt).GroupBy(x => x.TimeAt))
            {//CreateAt在list中无值？
                KeywordVO amm = new KeywordVO();
                amm.TimeKey = item.Key;
                amm.TaskList = item.OrderByDescending(x => x.CreatedAt).ToList();//item不是数组，使用排序命令是为为什么？
                am.Add(amm);
            }
            return am;
        }

        //获取关键词搜索记录
        [HttpGet]
        public List<FreeTaskRecordDto> GetKeywordRecord(string user_id, int? pagesize, string projectId)
        {
            var builder = Builders<FreeTaskRecord>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var TaskList = new List<FreeTaskRecord>();
            if (pagesize.HasValue)
            {
                TaskList = MongoDBHelper.Instance.Get_FreeTaskRecord().Find(filter).SortByDescending(x => x.CreatedAt).Limit(pagesize).ToList();
            }
            else
            {
                TaskList = MongoDBHelper.Instance.Get_FreeTaskRecord().Find(filter).SortByDescending(x => x.CreatedAt).ToList();
            }
            List<FreeTaskRecordDto> list = new List<FreeTaskRecordDto>();
            foreach (var item in TaskList)
            {
                FreeTaskRecordDto v = new FreeTaskRecordDto();
                v._id = item._id.ToString();
                v.IsStarted = item.IsStarted;
                v.TaskName = item.TaskName;
                v.CreatedAt = item.CreatedAt;
                v.Dataquantity = item.Dataquantity;
                v.LanIP = item.LanIP;
                v.UsrId = item.UsrId;
                v.Taskid = item.Taskid.ToString();
                v.ServiceState = item.ServiceState;
                v.LastBotStartAt = item.LastBotStartAt;
                v.LastBotEndAt = item.LastBotEndAt;
                v.IsBot = item.IsBot;
                v.LinksNum = item.LinksNum;
                v.ShopsNum = item.ShopsNum;
                v.SiteId = item.SiteId;
                v.SiteName = item.SiteName;
                v.ProjectId = item.ProjectId.ToString();
                list.Add(v);
            }
            return list;
        }



        //link获取
        [HttpGet]
        public List<linksdto> GetLinks(int source, string projectId)
        {
            var builder = Builders<links>.Filter;
            var filter = builder.Eq(x => x.source, source);
            filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var TaskList = MongoDBHelper.Instance.GetIW2S_links().Find(filter).Limit(100).ToList();
            List<linksdto> list = new List<linksdto>();
            foreach (var item in TaskList)
            {
                linksdto v = new linksdto();
                v.source = item.source;
                v.target = item.target;
                v.value = item.value;
                v.Gid = item.Gid;
                list.Add(v);
            }
            return list;
        }

        //link修改
        [HttpGet]
        public string ChangeLinks(int source, int target, int value, string projectId)
        {
            var builder = Builders<links>.Filter;
            var filter = builder.Eq(x => x.source, source);
            filter &= builder.Eq(x => x.target, target);
            filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var TaskList = MongoDBHelper.Instance.GetIW2S_links().Find(filter).FirstOrDefault();
            if (TaskList != null)
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "value", value } } } };
                var Query = new QueryDocument { { "source", source }, { "target", target } };
                MongoDBHelper.Instance.GetIW2S_links().UpdateOne(Query, update);
                return "成功！";
            }
            else
            {
                return "该目标不存在！";
            }

        }

        //link删除
        [HttpGet]
        public string Dellink(int soure, int target, string projectId)
        {
            var builder = Builders<links>.Filter;
            var filter = builder.Eq(x => x.source, soure);
            filter &= builder.Eq(x => x.target, target);
            filter &= builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            var TaskList = MongoDBHelper.Instance.GetIW2S_links().Find(filter).FirstOrDefault();
            if (TaskList == null)
            {
                return "该目标不存在!";
            }
            else
            {
                var result = MongoDBHelper.Instance.GetIW2S_links().DeleteOne(filter).DeletedCount;
                return result > 0 ? "成功！" : "失败！";
            }
        }






        /// <summary>
        /// 新增分组
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="groupName"></param>
        /// <param name="lawCode"></param>
        /// <param name="weight"></param>
        /// <param name="groupType">1:直搜</param>
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
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();
            var keywordGroupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();

            var categorybuilder = Builders<FreeKeywordCategory>.Filter;

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
                categoryDto = new FreeKeywordCategory
                {
                    _id = ObjectId.GenerateNewId(),
                    //GroupType = groupType,
                    Name = groupName,
                    //InfriLawCode = lawCode,
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
                var keywordBuilder = Builders<FreeTask>.Filter;
                var keywordCol = MongoDBHelper.Instance.Get_FreeTask();
                foreach (string keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {

                        var keywordFilter = keywordBuilder.Eq(x => x._id, new ObjectId(keyId));
                        var keyword = keywordCol.Find(keywordFilter).Project(x => new { CommendKeyword = x.TaskName}).FirstOrDefault();
                        if (keyword != null)
                        {
                            FreeKeywordGroup groupDto = new FreeKeywordGroup
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
                         //   valLinkCount += keyword.ValLinkCount;
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
                SiteSource = (int)SiteSource.emarketnow
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
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();
            var keywordGroupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();

            var categorybuilder = Builders<FreeKeywordCategory>.Filter;

            var categoryfilter = categorybuilder.Eq(x => x._id, new ObjectId(groupid));

            var categoryDto = categoryCol.Find(categoryfilter).FirstOrDefault();

            if (categoryDto == null)
            {
                result.Message = "修改的分组不能为空";
                return result;
            }

            var groupbuilder = Builders<FreeKeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, categoryDto._id);

            var alloldCommendIds = keywordGroupCol.Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            keywordGroupCol.DeleteMany(groupfilter);
            var keywordIdList = keywordIds.Split(';', '；', ',');

            int keywordTotal = 0;
            bool isLevel1 = categoryDto.ParentId.Equals(new ObjectId("000000000000000000000000")) ? true : false;
            if (!string.IsNullOrEmpty(keywordIds))
            {
                var builder = Builders<FreeTask>.Filter;
                foreach (var keyId in keywordIdList)
                {
                    if (!string.IsNullOrEmpty(keyId))
                    {
                        alloldCommendIds.Remove(new ObjectId(keyId));

                        FreeKeywordGroup groupDto = new FreeKeywordGroup
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
                        var keywordStr = MongoDBHelper.Instance.Get_FreeTask().Find(filter).Project(x => x.TaskName).FirstOrDefault();
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
                SiteSource = (int)SiteSource.emarketnow
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        private void UpdateLinksInfriType(string searchKeywordId, string lawCode)
        {
            var builder = Builders<FreeBotItem>.Filter;
            var filter = builder.Eq(x => x.taskId, new ObjectId(searchKeywordId));

            var update = new UpdateDocument { { "$set", new QueryDocument { { "InfriLawCode", new ObjectId(lawCode) } } } };

            MongoDBHelper.Instance.Get_FreeBotItem().UpdateMany(filter, update);
        }

        private void RecurseDelSubKeyword(FreeKeywordCategory categoryDto, List<ObjectId> alloldCommendIds)
        {
            var categorybuilder = Builders<FreeKeywordCategory>.Filter;
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();
            var groupbuilder = Builders<FreeKeywordGroup>.Filter;
            var keywordGroupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();

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
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();
            var keywordGroupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();

            var categorybuilder = Builders<FreeKeywordCategory>.Filter;

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
                SiteSource = (int)SiteSource.emarketnow
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public List<FreeKeywordCategoryDto> GetAllKeywordCategory(string user_id, string projectId)
        {
            List<FreeKeywordCategoryDto> result = new List<FreeKeywordCategoryDto>();

            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));

            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).SortBy(x => x.ParentId).ToList();

            var groupbuilder = Builders<FreeKeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));

            var keywordBuilder = Builders<FreeTask>.Filter;
            var keywordCol = MongoDBHelper.Instance.Get_FreeTask();
            FilterDefinition<FreeTask> keywordFilter = null;
            ObjectId nullId = new ObjectId("000000000000000000000000");

            foreach (var item in TaskList)
            {
                FreeKeywordCategoryDto v = new FreeKeywordCategoryDto();
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

                var selectedIdList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.IsBot, false);
                var unkeyword = keywordCol.Find(keywordFilter).Project(x => x._id).FirstOrDefault();

                keywordFilter = keywordBuilder.In(x => x._id, selectedIdList) & keywordBuilder.Eq(x => x.IsBot, true);
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
        public List<FreeKeywordCategoryDto> GetKeywordCategory(string user_id, string projectId, int groupType, string keywordId)
        {
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //if (!string.IsNullOrEmpty(keywordId))
            //{
            //    filter &= builder.Eq(x => x.KeywordId, new ObjectId(keywordId));
            //}
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).SortBy(x => x.ParentId).ToList();

            List<FreeKeywordCategoryDto> list = new List<FreeKeywordCategoryDto>();

            Dictionary<string, string> dicCategoryIDName = new Dictionary<string, string>();

            var commendbuilder = Builders<FreeTask>.Filter;

            var commendfilter = commendbuilder.Eq(x => x.ProjectId, new ObjectId(projectId)) ; 
            var keywordCount = MongoDBHelper.Instance.Get_FreeTask().Find(commendfilter).Project(x => x._id).Count();

            list.Add(new FreeKeywordCategoryDto
            {
                _id = "",
                Name = "所有词",
                KeywordTotal = (int)keywordCount
            });

            foreach (var item in TaskList)
            {
                FreeKeywordCategoryDto v = new FreeKeywordCategoryDto();
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
        /// <param name="groupType">1:直搜</param>
        /// <returns></returns>
        [HttpGet]
        public List<FreeTaskDto> GetKeywordGroup(string usr_id, string projectId, string groupid, string keywordId, int groupType)
        {

            groupType = 1;

            List<FreeTaskDto> list = new List<FreeTaskDto>();
            //if(string.IsNullOrEmpty( keywordId))
            //{
            //    return list;
            //}
            var groupbuilder = Builders<FreeKeywordGroup>.Filter;

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

            var selectedIdList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<FreeTaskDto> allKeywords = new List<FreeTaskDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                allKeywords = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter)
                    .Project(x => new FreeTaskDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                         TaskName = x.BaiduCommend,
                        drag = true
                    }
                    ).ToList();
            }

            else if (string.IsNullOrEmpty(groupid))
            {
                allKeywords = GetKeyword(usr_id, projectId);
            }

            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).ToList();
            return list;
        }



        [HttpGet]
        public Free_KeywordGroupModelDto GetEditKeywordGroup(string usr_id, string projectId, string groupid, string parentid, string keywordId, int groupType)
        {
            Free_KeywordGroupModelDto result = new Free_KeywordGroupModelDto();
            List<FreeTaskDto> list = new List<FreeTaskDto>();
            //if (string.IsNullOrEmpty(keywordId))
            //{
            //    return result;
            //}
            var groupbuilder = Builders<FreeKeywordGroup>.Filter;

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


            var selectedIdList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => x.BaiduCommendId).ToList();

            List<FreeTaskDto> allKeywords = new List<FreeTaskDto>();
            if (!string.IsNullOrEmpty(parentid) && parentid != "000000000000000000000000")
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(parentid));
                allKeywords = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter)
                    .Project(x => new FreeTaskDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                         TaskName = x.BaiduCommend,
                        drag = true
                    }
                    ).ToList();
            }
            else if (string.IsNullOrEmpty(parentid) || parentid == "000000000000000000000000")
            {
                allKeywords = GetKeyword(usr_id, projectId);
            }

            list = allKeywords.Where(x => !selectedIdList.Contains(new ObjectId(x._id))).ToList();

            List<FreeTaskDto> curSelectedList = new List<FreeTaskDto>();
            if (!string.IsNullOrEmpty(groupid))
            {
                groupfilter = groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                groupfilter &= groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(groupid));
                curSelectedList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter)
                    .Project(x => new FreeTaskDto
                    {
                        _id = x.BaiduCommendId.ToString(),
                         TaskName = x.BaiduCommend,
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
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, new ObjectId(categoryId));
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();

            var prjId = categoryCol.Find(builder.Eq(x => x._id, new ObjectId(categoryId))).Project(x => x.ProjectId).FirstOrDefault();

            categoryCol.DeleteOne(builder.Eq(x => x._id, new ObjectId(categoryId)));

            var groupBuilder = Builders<FreeKeywordGroup>.Filter;
            var groupFilter = groupBuilder.Eq(x => x.CommendCategoryId, new ObjectId(categoryId));
            var groupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();
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
                UserId = new ObjectId(user_id),
                SiteSource = (int)SiteSource.emarketnow
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            return result;
        }
        private void RecurseDelCategory(ObjectId categoryId)
        {

            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ParentId, categoryId);
            var categoryCol = MongoDBHelper.Instance.GetFreeKeywordCategory();

            var TaskList = categoryCol.Find(filter).Project(x => x._id).ToList();
            categoryCol.DeleteMany(builder.In(x => x._id, TaskList));

            var groupBuilder = Builders<FreeKeywordGroup>.Filter;

            var groupCol = MongoDBHelper.Instance.GetFreeKeywordGroup();
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
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<FreeKeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => new GroupTreeDto
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
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).Project(x => new GroupTreeDto
            {
                _id = x._id.ToString(),
                name = x.Name
            }).ToList();

            parent.children = new List<GroupTreeDto>();

            var groupbuilder = Builders<FreeKeywordGroup>.Filter;

            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);

            var keywordList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => new GroupTreeDto
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
                //string url = CreFile(folder, value, "SogouGroupTree.txt");
                string path = folder + "SogouGroupTree.txt";
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
                json.Url = "Scripts/app/data/" + "SogouGroupTree.txt";
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
                        var builder = Builders<FreeTask>.Filter;
                        var filter = builder.Eq(x => x._id, new ObjectId(v.id));
                        var task = MongoDBHelper.Instance.Get_FreeTask().Find(filter).FirstOrDefault();

                        GroupTree3Dto leaf = new GroupTree3Dto();
                        leaf.name = v.name; //v.name + "(" + task.ValLinkCount + ")";
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
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).Project(x => new GroupTree2Dto
            {
                id = x._id.ToString(),
                pId = x.ParentId.ToString(),
                name = x.Name
            }).ToList();

            //获取当前词组内关键词
            var groupbuilder = Builders<FreeKeywordGroup>.Filter;
            var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, parentId);
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
            var keywordList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => new GroupTree2Dto
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
            var builder = Builders<FreeKeywordCategory>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            //filter &= builder.Eq(x => x.GroupType, groupType);
            filter &= builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.ParentId, parentId);
            var TaskList = MongoDBHelper.Instance.GetFreeKeywordCategory().Find(filter).Project(x => new GroupTree2Dto
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
        public List<GroupKeywordsDto> GetFenleiKeywords(string usr_id, string projectId, string treeNodeId)
        {
            List<GroupKeywordsDto> keywordList = new List<GroupKeywordsDto>();
            var builder = Builders<FreeTask>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));
            
            //判断是否为根目录，是直接查询IW2S_BaiduCommends集合，否则先获取分类关键词再查询IW2S_BaiduCommends
            if (string.Equals(treeNodeId, "000000000000000000000000"))
            {
                keywordList = MongoDBHelper.Instance.Get_FreeTask().Find(filter).Project(x => new GroupKeywordsDto
                {
                    id = x._id.ToString(),
                    name = x.TaskName,
                    //ValLinkCount = x.ValLinkCount,
                    BotStatus = x.IsBot==false?0:1
                }).ToList();
            }
            else
            {
                var groupbuilder = Builders<FreeKeywordGroup>.Filter;
                var groupfilter = groupbuilder.Eq(x => x.CommendCategoryId, new ObjectId(treeNodeId));
                groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
                var TempList = MongoDBHelper.Instance.GetFreeKeywordGroup().Find(groupfilter).Project(x => new GroupKeywordsDto
                {
                    id = x.BaiduCommendId.ToString(),
                    name = x.BaiduCommend,
                    ValLinkCount = 0,
                    BotStatus = 0
                }).ToList();
                foreach (var v in TempList)
                {
                    var countfilter = builder.Eq(x => x._id, new ObjectId(v.id));
                    var temp = MongoDBHelper.Instance.Get_FreeTask().Find(countfilter).Project(x => new GroupKeywordsDto
                    {
                        id = x._id.ToString(),
                        name = x.TaskName,
                       // ValLinkCount = x.ValLinkCount,
                        BotStatus = x.IsBot == false ? 0 : 1
                    }).FirstOrDefault();
                    keywordList.Add(temp);
                }
            }
            return keywordList;
        }

        /// <summary>
        /// 排除关键词
        /// </summary>
        /// <param name="categoryId">关键词Id，用“;”隔开</param>
        /// <param name="status">true表示删除，false表示还原</param>
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

                    var result = MongoDBHelper.Instance.GetIW2S_SG_BaiduCommends().UpdateOne(new QueryDocument { { "_id", id } }, update).ModifiedCount;

                    var updategroup = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", status } } } };
                    MongoDBHelper.Instance.GetIW2S_SG_KeywordGroups().UpdateMany(new QueryDocument { { "BaiduCommendId", id } }, updategroup);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }



        private List<ObjectId> GetObjIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
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
        public ResultDto SetCommendEMNStatus(string user_id, string categoryId, byte? status, string prjId, bool isgroup)
        {
            ResultDto result = new ResultDto();
            var categoryBuilder = Builders<FreeKeywordGroup>.Filter;
            FilterDefinition<FreeKeywordGroup> categoryfilter = null;
            if (!string.IsNullOrEmpty(categoryId))
            {
                categoryfilter = categoryBuilder.In(x => x.CommendCategoryId,GetObjIdListFromStr(categoryId));

            }
            else if (!string.IsNullOrEmpty(prjId) && isgroup)
            {
                categoryfilter = categoryBuilder.Eq(x => x.ProjectId, new ObjectId(prjId));

            }
            var keywordBuilder = Builders<FreeTask>.Filter;
            var keywordcol = MongoDBHelper.Instance.Get_FreeTask();

            bool ssss = status == 0 ? false : true;

            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsBot", ssss } } } };
            var prjObjId = ObjectId.Empty;
            if (categoryfilter != null)
            {
                var categoryCol = MongoDBHelper.Instance.GetFreeKeywordGroup();
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
            result.IsSuccess = true;
            return result;
        }
   


    }
}