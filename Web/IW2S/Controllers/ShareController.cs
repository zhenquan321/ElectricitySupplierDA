using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AISSystem;
using System.IO;
using System.Text;

namespace IW2S.Controllers
{
    
    public class ShareController : ApiController
    {
        /// <summary>
        /// 新建分享项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="prjId"></param>
        /// <param name="usrEmails"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SendShareEmail(string usr_id,string prjId,string usrEmails,string content)
        {
            ResultDto result = new ResultDto();

            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            var usrEmailList = usrEmails.Split(';', '；').Where(x => !string.IsNullOrEmpty(x)).ToList();
            if(usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty || usrEmailList.Count == 0 || string.IsNullOrEmpty(content))
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<IW2S_ProjectShare>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ProjectShares();
            //var sharecode = CommonHelper.GenerateRandomNumber(5);

             var usrName = MongoDBHelper.Instance.Get_IW2SUser().Find(Builders<IW2SUser>.Filter.Eq(x => x._id, usrObjId)).Project(x => x.UsrEmail).FirstOrDefault();
            var prjName = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, prjObjId)).Project(x => x.Name).FirstOrDefault();

            Helpers.EmailHelper ems = new Helpers.EmailHelper();
            string CC = "";
            string Bcc = "";
            string Subject = "您收到“{0}”的一封分享项目“{1}”的邀请信".FormatStr(usrName, prjName);//主题
            System.Net.Mail.LinkedResource[] EmbeddedResources = null;//嵌入的外部资源
            System.Net.Mail.Attachment[] Attachments = null;//附件
            string From = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_acc");//发件人
            string UserName = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_name");//发件人用户名
            string Password = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_pwd");//密码
            string Server = AISSystem.AppSettingHelper.GetAppSetting("ems_smtp");//发件邮箱服务器
            int Port = int.Parse(AISSystem.AppSettingHelper.GetAppSetting("ems_smtp_port"));//邮箱端口
            bool UseSSL = false;//是否使用
            var body = @"<div>
                  <p>您好！</p>
                  <p>您收到来自点线云用户“{0}”的邀请，分享他（她）在点线云创建的监测项目：“{1}”。您可对分享的项目进行操作并与分享者或其他被分享点线云用户进行交流。</p>
                  <p>他给您留言：{3}</p> 
                  <p>如果您是点线云用户，请立即“<a href='{2}'>登录</a>”平台查看分享的项目，如果您还不是点线云用户，请立即“<a href='{2}'>注册</a>”成为点线云用户，然后登录平台就可查看分享的项目。注册时请注意：“邮箱”一栏请使用收到该邮件的邮箱地址。</p>
                  <p>如果您有什么问题，请联系我们的客服。祝您生活愉快！</p> 
                  </div>"
                .FormatStr(usrName, prjName,CommonHelper.iw2s_site,content);

            foreach(var usrEmail in usrEmailList)
            {
                var prjsh = col.Find(builder.Eq(x => x.ProjectId, prjObjId) & builder.Eq(x => x.SharedEmail, usrEmail)).Project(x => x._id).FirstOrDefault();
                if (prjsh == ObjectId.Empty)
                {
                    IW2S_ProjectShare share = new IW2S_ProjectShare();
                    share.Content = content;
                    share.ProjectId = prjObjId;
                    //share.ShareCode = sharecode;
                    share.SharedEmail = usrEmail;
                    share.UsrId = usrObjId;
                    share.CreatedAt = DateTime.Now.AddHours(8);
                    col.InsertOne(share);
                }

                ems.SendMail(usrEmail, From, Bcc, Subject, body,
                    EmbeddedResources, Attachments,  UserName, UserName, Password, "smtp.163.com", 25, UseSSL);
            }
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 取消分享项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="prjId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto CancelShareProject(string usr_id,string prjId)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty)
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<IW2S_ProjectShare>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ProjectShares();
            DateTime now = DateTime.Now.AddHours(8);
            col.UpdateMany(builder.Eq(x => x.ProjectId, prjObjId), new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", now } } } });
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 我分享的项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_ProjectDto> GetMyShareProjects(string usr_id, int page, int pagesize)
        {
            QueryResult<IW2S_ProjectDto> result = new QueryResult<IW2S_ProjectDto>();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            if (usrObjId == ObjectId.Empty)
            {
                return result;
            }

            //获取分享的项目id列表
            var sharebuilder = Builders<IW2S_ProjectShare>.Filter;
            var sharePrjIds = MongoDBHelper.Instance.GetIW2S_ProjectShares().Find(sharebuilder.Eq(x => x.UsrId, usrObjId) & sharebuilder.Eq(x => x.IsDel, false))
                .Project(x => new { ProjectId = x.ProjectId, SharedEmail = x.SharedEmail ,ShareId=x._id}).ToList();
            Dictionary<ObjectId, string> dicSharePrjEmails = new Dictionary<ObjectId, string>();    //项目Id和邮箱词典
            foreach (var sharePrjId in sharePrjIds)
            {
                if (dicSharePrjEmails.ContainsKey(sharePrjId.ProjectId))
                {
                    dicSharePrjEmails[sharePrjId.ProjectId] = dicSharePrjEmails[sharePrjId.ProjectId] + ";" + sharePrjId.SharedEmail;
                }
                else
                {
                    dicSharePrjEmails.Add(sharePrjId.ProjectId, sharePrjId.SharedEmail);
                }
            }

            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.In(x => x._id, dicSharePrjEmails.Keys.ToList());
            filter &= builder.Eq(x => x.IsDel, false) ;
            var query = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<IW2S_ProjectDto> data = new List<IW2S_ProjectDto>();

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
            var mappingBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var mappingFlter = mappingBuilder.In(x => x.ProjectId, proObjIds) & mappingBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & mappingBuilder.Eq(x => x.IsDel, false);
            var queryMapping = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mappingFlter).Project(x => new
            {
                Id = x._id.ToString(),
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
        /// 更新我分享的项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateMyShareProjects(string usr_id, string prjId, string usrEmails, string content)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty)
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<IW2S_ProjectShare>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ProjectShares();
            //删除原有分享项目
            col.DeleteMany(builder.Eq(x => x.ProjectId, prjObjId));
            //新建新的分享项目
            result = SendShareEmail(usr_id, prjId, usrEmails, content);

            return result;
        }

        /// <summary>
        /// 获取分享给我的项目
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_ProjectDto> GetShareToMeProjects(string usr_id, int page, int pagesize)
        {
            QueryResult<IW2S_ProjectDto> result = new QueryResult<IW2S_ProjectDto>();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            if (usrObjId == ObjectId.Empty)
            {
                return result;
            }
            //获取用户邮箱
            var userCol = MongoDBHelper.Instance.Get_IW2SUser();
            var email = userCol.Find(Builders<IW2SUser>.Filter.Eq(x => x._id, usrObjId)).Project(x => x.UsrEmail).FirstOrDefault();

            //获取分享给该邮箱的项目列表
            var sharebuilder = Builders<IW2S_ProjectShare>.Filter;
            var sharePrjs = MongoDBHelper.Instance.GetIW2S_ProjectShares().Find(sharebuilder.Eq(x => x.SharedEmail, email) & sharebuilder.Eq(x => x.IsDel, false)).ToList();
            var sharePrjIds = MongoDBHelper.Instance.GetIW2S_ProjectShares().Find(sharebuilder.Eq(x => x.SharedEmail, email) & sharebuilder.Eq(x => x.IsDel, false))
                .Project(x => new { ProjectId = x.ProjectId, IW2S_ProjectShare = x }).ToList().ToDictionary(x => x.ProjectId, y => y.IW2S_ProjectShare);

            //获取这些项目的信息
            var builder = Builders<IW2S_Project>.Filter;
            var filter = builder.In(x => x._id, sharePrjIds.Keys.ToList());
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetIW2S_Projects().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<IW2S_ProjectDto> data = new List<IW2S_ProjectDto>();

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
            var mappingBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var mappingFlter = mappingBuilder.In(x => x.ProjectId, proObjIds) & mappingBuilder.Eq(x => x.CategoryId, ObjectId.Empty) & mappingBuilder.Eq(x => x.IsDel, false);
            var queryMapping = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(mappingFlter).Project(x => new
            {
                Id = x._id.ToString(),
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
        /// 获取操作日志
        /// </summary>
        /// <param name="prjId"></param>
        /// <param name="opereateType"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_OperateLogDto> GetOperateLog(string prjId, string opereateType, int page, int pagesize, int siteSource)
        {
            QueryResult<IW2S_OperateLogDto> result = new QueryResult<IW2S_OperateLogDto>();
            
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (prjObjId == ObjectId.Empty)
            {                
                return result;
            }
            var builder = Builders<IW2S_OperateLog>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_OperateLogs();
            var opeareteTypes = CommonHelper.GetIdIntListFromStr(opereateType);

            var filter = builder.Eq(x => x.ProjectId, prjObjId) & builder.In(x => x.ShareOperateType, opeareteTypes) & builder.Eq(x => x.SiteSource, siteSource);
            var query = col.Find(filter).SortByDescending(x => x.CreatedAt);
            result.Count = query.Count();
            var datas = query.Skip(page * pagesize).Limit(pagesize).ToList();

            var usrIds = datas.Select(x => x.UserId).Distinct().ToList();
            var usrdatas = MongoDBHelper.Instance.Get_IW2SUser().Find(Builders<IW2SUser>.Filter.In(x => x._id, usrIds)).Project(x => new { Id = x._id.ToString(), User = x }).ToList().ToDictionary(x => x.Id, y => y.User);
            List<IW2S_OperateLogDto> logs = new List<IW2S_OperateLogDto>();
            foreach(var data in datas)
            {
                IW2S_OperateLogDto log = new IW2S_OperateLogDto();
                if(usrdatas.ContainsKey(data.UserId.ToString()))
                {
                    log.PictureSrc = usrdatas[data.UserId.ToString()].PictureSrc;
                    log.UserName = usrdatas[data.UserId.ToString()].LoginName;
                    log.CreatedAt = data.CreatedAt.AddHours(-8);
                }
                log.ShareOperateType = data.ShareOperateType;
                logs.Add(log);
            }
            result.Result = logs;
            
            return result;
        }
        /// <summary>
        /// 获取操作评论
        /// </summary>
        /// <param name="prjId"></param>
        /// <param name="opereateType"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_OperateCommentDto> GetOperateComment(string prjId, string opereateType, int page, int pagesize,int siteSource)
        {
            QueryResult<IW2S_OperateCommentDto> result = new QueryResult<IW2S_OperateCommentDto>();

            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (prjObjId == ObjectId.Empty)
            {
                return result;
            }
            var builder = Builders<IW2S_OperateComment>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_OperateComments();
            var opeareteTypes = CommonHelper.GetIdIntListFromStr(opereateType);

            var filter = builder.Eq(x => x.ProjectId, prjObjId) & builder.In(x => x.ShareOperateType, opeareteTypes) & builder.Eq(x => x.SiteSource, siteSource) & builder.Eq(x => x.IsDel, false);
            var query = col.Find(filter).SortByDescending(x=>x.CreatedAt);
            result.Count = query.Count();
            var datas = query.Skip(page * pagesize).Limit(pagesize).ToList();

            var usrIds = datas.Select(x => x.UserId).Distinct().ToList();
            var usrdatas = MongoDBHelper.Instance.Get_IW2SUser().Find(Builders<IW2SUser>.Filter.In(x => x._id, usrIds)).Project(x => new { Id = x._id.ToString(), User = x }).ToList().ToDictionary(x => x.Id, y => y.User);
            List<IW2S_OperateCommentDto> logs = new List<IW2S_OperateCommentDto>();
            foreach (var data in datas)
            {
                IW2S_OperateCommentDto log = new IW2S_OperateCommentDto();
                log._id = data._id.ToString();
                if (usrdatas.ContainsKey(data.UserId.ToString()))
                {
                    log.PictureSrc = usrdatas[data.UserId.ToString()].PictureSrc;
                    log.UserName = usrdatas[data.UserId.ToString()].LoginName;
                    log.CreatedAt = data.CreatedAt.AddHours(-8);
                }
                log.ShareOperateType = data.ShareOperateType;
                log.UserId = data.UserId.ToString();
                log.Comment = data.Comment;
                logs.Add(log);
            }
            result.Result = logs;

            return result;
        }
        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertComment(IW2S_OperateCommentDto data)
        {
            ResultDto result = new ResultDto();
            if(data == null || string.IsNullOrEmpty(data.Comment) || string.IsNullOrEmpty(data.ProjectId) || string.IsNullOrEmpty(data.UserId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_OperateComment>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_OperateComments();

            IW2S_OperateComment com = new IW2S_OperateComment();
            com.Comment = data.Comment;
            com.CreatedAt = DateTime.Now.AddHours(8);
            com.ProjectId = new ObjectId(data.ProjectId);
            com.ShareOperateType = data.ShareOperateType;
            com.SiteSource = data.SiteSource;
            com.UserId = new ObjectId(data.UserId);
            col.InsertOne(com);

            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelComment(string usr_id,string prjId,string commentId)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);
            
            if (usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty || string.IsNullOrEmpty(commentId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_OperateComment>.Filter;
            var filter = builder.In(x => x._id, CommonHelper.GetObjIdListFromStr(commentId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetIW2S_OperateComments().UpdateMany(filter,update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取分享评论
        /// </summary>
        /// <param name="prjId"></param>
        /// <param name="opereateType"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_ShareOutCommentDto> GetShareOutComment(string prjId, string opereateType, int page, int pagesize, int siteSource)
        {
            QueryResult<IW2S_ShareOutCommentDto> result = new QueryResult<IW2S_ShareOutCommentDto>();

            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (prjObjId == ObjectId.Empty)
            {
                return result;
            }
            var builder = Builders<IW2S_ShareOutComment>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ShareOutComments();
            var opeareteTypes = CommonHelper.GetIdIntListFromStr(opereateType);

            var filter = builder.Eq(x => x.ProjectId, prjObjId) & builder.In(x => x.ShareOperateType, opeareteTypes) & builder.Eq(x => x.SiteSource, siteSource) & builder.Eq(x => x.IsDel, false);
            var query = col.Find(filter).SortByDescending(x => x.CreatedAt);
            result.Count = query.Count();
            var datas = query.Skip(page * pagesize).Limit(pagesize).ToList();

            var usrIds = datas.Select(x => x.UserId).Distinct().ToList();
            var usrdatas = MongoDBHelper.Instance.Get_IW2SUser().Find(Builders<IW2SUser>.Filter.In(x => x._id, usrIds)).Project(x => new { Id = x._id.ToString(), User = x }).ToList().ToDictionary(x => x.Id, y => y.User);
            List<IW2S_ShareOutCommentDto> logs = new List<IW2S_ShareOutCommentDto>();
            foreach (var data in datas)
            {
                IW2S_ShareOutCommentDto log = new IW2S_ShareOutCommentDto();
                log._id = data._id.ToString();
                if (usrdatas.ContainsKey(data.UserId.ToString()))
                {
                    log.PictureSrc = usrdatas[data.UserId.ToString()].PictureSrc;
                    log.UserName = usrdatas[data.UserId.ToString()].LoginName;
                }
                log.CreatedAt = data.CreatedAt.AddHours(-8);
                log.ShareOperateType = data.ShareOperateType;
                log.UserId = data.UserId.ToString();
                log.Comment = data.Comment;
                logs.Add(log);
            }
            result.Result = logs;

            return result;
        }
        /// <summary>
        /// 新增分享评论
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertShareOutComment(IW2S_ShareOutCommentDto data)
        {
            ResultDto result = new ResultDto();
            if (data == null || string.IsNullOrEmpty(data.Comment) || string.IsNullOrEmpty(data.ProjectId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_ShareOutComment>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ShareOutComments();

            IW2S_ShareOutComment com = new IW2S_ShareOutComment();
            com.Comment = data.Comment;
            com.CreatedAt = DateTime.Now.AddHours(8);
            com.ProjectId = new ObjectId(data.ProjectId);
            com.ShareOperateType = data.ShareOperateType;
            com.SiteSource = data.SiteSource;
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(data.UserId, out usrObjId);
            if(usrObjId !=ObjectId.Empty)
            {
                com.UserId = usrObjId;
            }
            
            col.InsertOne(com);

            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 删除分享评论
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelShareOutComment(string usr_id, string prjId, string commentId)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty || string.IsNullOrEmpty(commentId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_ShareOutComment>.Filter;
            var filter = builder.In(x => x._id, CommonHelper.GetObjIdListFromStr(commentId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetIW2S_ShareOutComments().UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ResultDto GetQRCode(string url, string baseurl)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_UrlQRCode>.Filter;
            var filter = builder.Eq(x => x.Url, url);
            var col = MongoDBHelper.Instance.GetIW2S_UrlQRCodes();
            var qrcode = col.Find(filter).FirstOrDefault();
            if (qrcode == null)
            {
               // Guid qrcode_id = IDHelper.GetGuid(url);

                var qrcode_id = ObjectId.GenerateNewId();

                var bs = Helpers.ZXingQrcodeHelper.GetQrBitmap(url, null);
                string file = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + qrcode_id + ".png";
                File.WriteAllBytes(file, bs);
                //string upload_file_url = CommonHelper.file_sys_base_url + "/api/ipfile/upload_ipfile?md5=" + qrcode_id + "&content_type=png";
                //WebApiHelper.UploadFile(upload_file_url, file);
                //File.Delete(file);
                baseurl = "http://43.240.138.233:9999";
                string upload_file_url = baseurl + "/api/File/ImgUpload";
                // WebApiHelper.UploadFile(upload_file_url, path);
                WebClient wc = new WebClient();
                byte[] sendData = System.Text.Encoding.UTF8.GetBytes(file);
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                wc.Headers.Add("ContentLength", sendData.Length.ToString());
                byte[] recData = wc.UploadFile(upload_file_url, "POST", file);
                string stro = (Encoding.GetEncoding("GB2312").GetString(recData)).ToString();
                System.IO.File.Delete(file);
                //string downfile = baseurl + "/api/Account/download_ipfile?md5=" + qrcode_id;
                result.Message = stro;
            }
            else
            {
                result.Message = qrcode.QRCodeUrl;
            }

            result.IsSuccess = true;
            
            return result;
           
        }

        /// <summary>
        /// 获取分享到发现
        /// </summary>
        /// <param name="prjId"></param>
        /// <param name="opereateType"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<IW2S_ShareToDiscoverDto> GetShareToDiscover(string prjId, string opereateType, int page, int pagesize, int? siteSource,string prjusrname)
        {
            QueryResult<IW2S_ShareToDiscoverDto> result = new QueryResult<IW2S_ShareToDiscoverDto>();

            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            
            var builder = Builders<IW2S_ShareToDiscover>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ShareToDiscovers();
            var opeareteTypes = CommonHelper.GetIdIntListFromStr(opereateType);

            var usercol = MongoDBHelper.Instance.Get_IW2SUser();
            var prjcol = MongoDBHelper.Instance.GetIW2S_Projects();
            var searchusrdatas = new List<ObjectId>();
            var searchprjdatas = new List<ObjectId>();

            if (!string.IsNullOrEmpty(prjusrname))
            {
                searchusrdatas = usercol.Find(Builders<IW2SUser>.Filter.Regex(x => x.LoginName, new BsonRegularExpression("/.*" + prjusrname + ".*/"))).Project(x => x._id).ToList();
                searchprjdatas = prjcol.Find(Builders<IW2S_Project>.Filter.Regex(x => x.Name, new BsonRegularExpression("/.*" + prjusrname + ".*/"))).Project(x => x._id).ToList();
            }

            var filter = builder.Eq(x => x.IsDel, false);
            if (prjObjId != ObjectId.Empty)
            {
                filter &= builder.Eq(x => x.ProjectId, prjObjId);
            }
            if(opeareteTypes != null && opeareteTypes.Count > 0)
            {
                filter &= builder.In(x => x.ShareOperateType, opeareteTypes);
            }
            if(siteSource.HasValue)
            {
                filter &= builder.Eq(x => x.SiteSource, siteSource);
            }
            if(searchusrdatas!= null && searchusrdatas.Count > 0 && searchprjdatas != null && searchprjdatas.Count > 0)
            {
                filter &= builder.In(x => x.UserId, searchusrdatas) | builder.In(x => x.ProjectId, searchprjdatas);
            }
            else if (searchprjdatas != null && searchprjdatas.Count > 0)
            {
                filter &= builder.In(x => x.ProjectId, searchprjdatas);
            }
            else if (searchusrdatas != null && searchusrdatas.Count > 0)
            {
                filter &= builder.In(x => x.UserId, searchusrdatas);
            }
              
            var query = col.Find(filter).SortByDescending(x => x.CreatedAt);
            result.Count = query.Count();
            var datas = query.Skip(page * pagesize).Limit(pagesize).ToList();

            var usrIds = datas.Select(x => x.UserId).Distinct().ToList();
            var usrdatas = usercol.Find(Builders<IW2SUser>.Filter.In(x => x._id, usrIds)).Project(x => new { Id = x._id.ToString(), User = x }).ToList().ToDictionary(x => x.Id, y => y.User);

            var prjIds = datas.Select(x => x.ProjectId).Distinct().ToList();
            var prjdatas = prjcol.Find(Builders<IW2S_Project>.Filter.In(x => x._id, prjIds)).Project(x => new { Id = x._id.ToString(), ProjectName = x.Name }).ToList().ToDictionary(x => x.Id, y => y.ProjectName);

            List<IW2S_ShareToDiscoverDto> logs = new List<IW2S_ShareToDiscoverDto>();
            foreach (var data in datas)
            {
                IW2S_ShareToDiscoverDto log = new IW2S_ShareToDiscoverDto();
                log._id = data._id.ToString();
                if (usrdatas.ContainsKey(data.UserId.ToString()))
                {
                    log.PictureSrc = usrdatas[data.UserId.ToString()].PictureSrc;
                    log.UserName = usrdatas[data.UserId.ToString()].LoginName;
                    log.Gender = usrdatas[data.UserId.ToString()].Gender;

                }
                if (prjdatas.ContainsKey(data.ProjectId.ToString()))
                {
                    log.ProjectName = prjdatas[data.ProjectId.ToString()];
                }
                log.UserId = data.UserId.ToString();
                log.CreatedAt = data.CreatedAt.AddHours(-8);
                log.ShareOperateType = data.ShareOperateType;
                log.SiteSource = data.SiteSource;
                log.Content = data.Content;
                log.ProjectId = data.ProjectId.ToString();
                logs.Add(log);
            }
            result.Result = logs;

            return result;
        }
        /// <summary>
        /// 新增分享到发现
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertShareToDiscover(IW2S_ShareToDiscoverDto data)
        {
            ResultDto result = new ResultDto();
            if (data == null ||  string.IsNullOrEmpty(data.ProjectId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_ShareToDiscover>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ShareToDiscovers();

            IW2S_ShareToDiscover com = new IW2S_ShareToDiscover();
            com.Content = data.Content;
            com.CreatedAt = DateTime.Now.AddHours(8);
            com.ProjectId = new ObjectId(data.ProjectId);
            com.ShareOperateType = data.ShareOperateType;
            com.SiteSource = data.SiteSource;
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(data.UserId, out usrObjId);
            if (usrObjId != ObjectId.Empty)
            {
                com.UserId = usrObjId;
            }

            col.InsertOne(com);

            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 删除分享到发现
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelShareToDiscover(string usr_id, string prjId, string commentId)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var prjObjId = ObjectId.Empty;
            ObjectId.TryParse(prjId, out prjObjId);

            if (usrObjId == ObjectId.Empty || prjObjId == ObjectId.Empty || string.IsNullOrEmpty(commentId))
            {
                result.Message = "参数空异常";
                return result;
            }
            var builder = Builders<IW2S_ShareToDiscover>.Filter;
            var filter = builder.In(x => x._id, CommonHelper.GetObjIdListFromStr(commentId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetIW2S_ShareToDiscovers().UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 克隆项目到本地
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto CloneProject(string userId, string projectId)
        {
            //克隆项目信息
            var sourceProId = new ObjectId(projectId);      //原项目Id
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x._id, sourceProId);
            var colPro = MongoDBHelper.Instance.GetIW2S_Projects();
            var sourcePro = colPro.Find(filterPro).FirstOrDefault();
            var sourceUserId = sourcePro.UsrId;             //原用户Id
            var clonePro = sourcePro;
            clonePro._id = ObjectId.GenerateNewId();       //新项目Id
            clonePro.UsrId = new ObjectId(userId);         //新用户Id
            clonePro.Name += "_克隆";
            clonePro.CreatedAt = DateTime.Now.AddHours(8);
            
            colPro.InsertOne(clonePro);

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
                key.ProjectId = clonePro._id;
                key.UserId = clonePro.UsrId;
                cloneKeys.Add(key);
            }
            if (cloneKeys.Count > 0)
                colKey.InsertMany(cloneKeys);

            //克隆关键词分组
            BaiduBackupKeywordCate(ObjectId.Empty, ObjectId.Empty, sourceProId, clonePro._id, clonePro.UsrId);
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
                key.ProjectId = clonePro._id;
                key.UserId = clonePro.UsrId;
                WXcloneKeys.Add(key);
            }
            if (WXcloneKeys.Count > 0)
                WXcolKey.InsertMany(WXcloneKeys);

            //克隆关键词分组
            WeiXinBackupKeywordCate(ObjectId.Empty, ObjectId.Empty, sourceProId, clonePro._id, clonePro.UsrId);
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
                    chart.ProjectId = clonePro._id;
                    chart.UsrId = clonePro.UsrId;
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
                var cloneProLink = new List<IW2S_ProLinksCount>();
                foreach (var x in sourceProLink)
                {
                    var chart = x;
                    chart._id = ObjectId.GenerateNewId();
                    chart.ProjectId = clonePro._id;
                    chart.UsrId = clonePro.UsrId;
                    cloneProLink.Add(chart);
                }
                colProLink.InsertMany(cloneProLink);
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
        private void BaiduBackupKeywordCate(ObjectId soureCateObjId, ObjectId cloneCateObjId, ObjectId sourceProObjId, ObjectId cloneProObjId,ObjectId cloneUserObjId)
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
    }
}
