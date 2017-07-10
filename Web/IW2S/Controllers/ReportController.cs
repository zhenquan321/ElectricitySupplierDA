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
    public class ReportController : ApiController
    {
        /// <summary>
        /// 创建监测简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="name">简报名</param>
        /// <param name="description">简报描述</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="iconUrl">图标地址</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertReport(string usr_id, string name, string description, string projectId, string iconUrl)
        {
            ResultDto result = new ResultDto();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(usr_id))
            {
                return result;
            }

            var builder = Builders<Dnl_Report>.Filter;
            var col = MongoDBHelper.Instance.GetDnl_Report();
            var filter = builder.Eq(x => x.Name, name);
            filter &= builder.Eq(x => x.UsrId, new ObjectId(usr_id));
            filter &= builder.Eq(x => x.IsDel, false);

            var dto = col.Find(filter).FirstOrDefault();

            if (dto != null)
            {
                result.Message = "简报名‘" + name + "’已经存在！";
                return result;

            }
            if (iconUrl == null)
                iconUrl = "";

            //获取项目名称
            var filterPro = Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(projectId));
            var proName = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).Project(x => x.Name).FirstOrDefault();

            Dnl_Report rep = new Dnl_Report
            {
                UsrId = new ObjectId(usr_id),
                Name = name,
                CreatedAt = DateTime.Now.AddHours(8),
                IsDel = false,
                ProjectId = projectId,
                Description = description,
                IconUrl = iconUrl,
                ProjectName = proName
            };

            col.InsertOne(rep);
            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 获取自己简报列表
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="page">第几页</param>
        /// <param name="pagesize">一页数据量</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_ReportDto> GetMyReport(string usr_id, int page, int pagesize)
        {
            QueryResult<Dnl_ReportDto> result = new QueryResult<Dnl_ReportDto>();
            if (string.IsNullOrEmpty(usr_id))
            {
                return result;
            }
            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(usr_id));
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_Report().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();

            List<Dnl_ReportDto> data = new List<Dnl_ReportDto>();       //要返回的数据
            foreach (var item in TaskList)
            {
                Dnl_ReportDto v = new Dnl_ReportDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.CreatedAt = item.CreatedAt;
                v.Description = item.Description;
                v.IconUrl = item.IconUrl;
                v.ProjectId = item.ProjectId;
                v.UsrId = item.UsrId.ToString();
                v.ProjectName = item.ProjectName;
                data.Add(v);
            }

            result.Result = data;
            result.Count = totalCount;
            return result;
        }

        /// <summary>
        /// 删除简报
        /// </summary>
        /// <param name="filterIds">以;隔开</param>
        /// <returns></returns>
        [HttpGet]
        public string DelReport(string ids)
        {
            var filterlist = ids.Split(';', '；');
            List<ObjectId> obIds = new List<ObjectId>();

            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.In(x => x._id, obIds);
            foreach (var filterId in filterlist)
            {
                if (!string.IsNullOrEmpty(filterId))
                {
                    obIds.Add(new ObjectId(filterId));
                }
            }
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetDnl_Report().UpdateMany(filter, update);

            //删除分享简报
            var shareBuilder = Builders<Dnl_ReportShare>.Filter;
            var shareCol = MongoDBHelper.Instance.GetDnl_ReportShare();
            DateTime now = DateTime.Now.AddHours(8);
            shareCol.UpdateMany(shareBuilder.In(x => x.ReportId, obIds), new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", now } } } });

            return "成功！";
        }

        /// <summary>
        /// 更新简报
        /// </summary>
        /// <param name="reportId">简报Id</param>
        /// <param name="name">简报名</param>
        /// <param name="description">简报描述</param>
        /// <param name="iconUrl">图标地址</param>
        [HttpGet]
        public ResultDto UpdateReport(string reportId, string name, string description, string iconUrl)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(reportId));
            if (string.IsNullOrEmpty(name))
            {
                result.Message = "简报名不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "简报描述不能为空";
                return result;
            }
            if (iconUrl == null)
                iconUrl = "";

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", name }, { "Description", description }, { "IconUrl", iconUrl } } } };
            MongoDBHelper.Instance.GetDnl_Report().UpdateOne(filter, update);

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取简报创建日志（最后十次创建内容）
        /// </summary>
        /// <param name="usr_id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_ReportDto> GetReportLog(string usr_id)
        {
            List<Dnl_ReportDto> result = new List<Dnl_ReportDto>();
            if (string.IsNullOrEmpty(usr_id))
            {
                return null;
            }
            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.Eq(x => x.UsrId, new ObjectId(usr_id));
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_Report().Find(filter);
            var reportList = query.SortByDescending(x => x.CreatedAt).ToList().Take(10);
            foreach (var x in reportList)
            {
                var log = new Dnl_ReportDto();
                log._id = x._id.ToString();
                log.Name = x.Name;
                log.CreatedAt = x.CreatedAt;
                log.Description = x.Description;
                log.IconUrl = x.IconUrl;
                log.ProjectId = x.ProjectId;
                log.UsrId = x.UsrId.ToString();
                log.ProjectName = x.ProjectName;
                result.Add(log);
            }

            return result;
        }

        /// <summary>
        /// 新建分享简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="reportId">简报Id</param>
        /// <param name="usrEmails">要分享的用户邮箱，多个用分号隔开</param>
        /// <param name="content">邮件正文</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto SendShareEmail(string usr_id, string reportId, string usrEmails, string content)
        {
            ResultDto result = new ResultDto();

            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            var usrEmailList = usrEmails.Split(';', '；').Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (usrObjId == ObjectId.Empty || repObjId == ObjectId.Empty || usrEmailList.Count == 0 || string.IsNullOrEmpty(content))
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<Dnl_ReportShare>.Filter;
            var col = MongoDBHelper.Instance.GetDnl_ReportShare();
            //var sharecode = CommonHelper.GenerateRandomNumber(5);

            var usrName = MongoDBHelper.Instance.Get_IW2SUser().Find(Builders<IW2SUser>.Filter.Eq(x => x._id, usrObjId)).Project(x => x.UsrEmail).FirstOrDefault();
            var reportName = MongoDBHelper.Instance.GetDnl_Report().Find(Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId)).Project(x => x.Name).FirstOrDefault();

            Helpers.EmailHelper ems = new Helpers.EmailHelper();
            string CC = "";
            string Bcc = "";
            string Subject = "您收到“{0}”的一封分享数据简报“{1}”的邀请信".FormatStr(usrName, reportName);//主题
            System.Net.Mail.LinkedResource[] EmbeddedResources = null;//嵌入的外部资源
            System.Net.Mail.Attachment[] Attachments = null;//附件
            string From = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_acc");//发件人
            string UserName = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_name");//发件人用户名
            string Password = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_pwd");//密码
            string Server = AISSystem.AppSettingHelper.GetAppSetting("ems_smtp");//发件邮箱服务器
            int Port = int.Parse(AISSystem.AppSettingHelper.GetAppSetting("ems_smtp_port"));//邮箱端口
            bool UseSSL = false;//是否使用
            var body = @"<div><p>您好！您收到“{0}”的一封分享数据简报“{1}”的邀请信</p> <p> 您的朋友说：“{3}”</p> <p>如果您不是DataWeco用户，请先以该邮箱<a href='{2}'>注册</a>，<a href='{2}'>登录</a>后，在进入数据简报页面，点击“我收到的分享”，即可看到收到的数据简报，点击您需要的报告，便可以查看。同时Dataweco推荐您创建自己的监测项目。</p>
                        <p>如果您有任何问题，欢迎随时回复邮件，我们将第一时间为您解答！</p><p>DataWeco服务团队祝您生活愉快！</p></div>"
                .FormatStr(usrName, reportName, CommonHelper.iw2s_site, content);

            foreach (var usrEmail in usrEmailList)
            {
                var prjsh = col.Find(builder.Eq(x => x.ReportId, repObjId) & builder.Eq(x => x.SharedEmail, usrEmail)).Project(x => x._id).FirstOrDefault();
                if (prjsh == ObjectId.Empty)
                {
                    Dnl_ReportShare share = new Dnl_ReportShare();
                    share.Content = content;
                    share.ReportId = repObjId;
                    //share.ShareCode = sharecode;
                    share.SharedEmail = usrEmail;
                    share.UsrId = usrObjId;
                    share.CreatedAt = DateTime.Now.AddHours(8);
                    col.InsertOne(share);
                }

                ems.SendMail(usrEmail, CC, Bcc, Subject, body,
                    EmbeddedResources, Attachments, UserName, UserName, Password, "smtp.163.com", 25, UseSSL);
            }
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 取消分享简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="page">第几页</param>
        /// <param name="pagesize">一页数据量</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto CancelShareReport(string usr_id, string reportId)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            if (usrObjId == ObjectId.Empty || repObjId == ObjectId.Empty)
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<Dnl_ReportShare>.Filter;
            var col = MongoDBHelper.Instance.GetDnl_ReportShare();
            DateTime now = DateTime.Now.AddHours(8);
            col.UpdateMany(builder.Eq(x => x.ReportId, repObjId), new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", now } } } });
            result.IsSuccess = true;

            return result;
        }
        /// <summary>
        /// 我分享的简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="page">第几页</param>
        /// <param name="pagesize">一页数据量</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_ReportDto> GetMyShareReport(string usr_id, int page, int pagesize)
        {
            QueryResult<Dnl_ReportDto> result = new QueryResult<Dnl_ReportDto>();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            if (usrObjId == ObjectId.Empty)
            {
                return result;
            }

            //获取分享的简报id列表
            var sharebuilder = Builders<Dnl_ReportShare>.Filter;
            var sharePrjIds = MongoDBHelper.Instance.GetDnl_ReportShare().Find(sharebuilder.Eq(x => x.UsrId, usrObjId) & sharebuilder.Eq(x => x.IsDel, false))
                .Project(x => new { ReportId = x.ReportId, SharedEmail = x.SharedEmail, ShareId = x._id, ShareTime = x.CreatedAt }).ToList();
            Dictionary<ObjectId, string> dicSharePrjEmails = new Dictionary<ObjectId, string>();    //简报Id和邮箱词典
            foreach (var sharePrjId in sharePrjIds)
            {
                if (dicSharePrjEmails.ContainsKey(sharePrjId.ReportId))
                {
                    dicSharePrjEmails[sharePrjId.ReportId] = dicSharePrjEmails[sharePrjId.ReportId] + ";" + sharePrjId.SharedEmail;
                }
                else
                {
                    dicSharePrjEmails.Add(sharePrjId.ReportId, sharePrjId.SharedEmail);
                }
            }

            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.In(x => x._id, dicSharePrjEmails.Keys.ToList());
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_Report().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<Dnl_ReportDto> data = new List<Dnl_ReportDto>();

            foreach (var item in TaskList)
            {
                Dnl_ReportDto v = new Dnl_ReportDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.CreatedAt = item.CreatedAt;
                v.Description = item.Description;
                v.IconUrl = item.IconUrl;
                v.ProjectId = item.ProjectId;
                v.UsrId = item.UsrId.ToString();
                v.ProjectName = item.ProjectName;
                if (dicSharePrjEmails.ContainsKey(item._id))
                {
                    v.SharerEmail = dicSharePrjEmails[item._id].Split(';', '；').ToList();
                    v.ShareTime = sharePrjIds.Find(x => x.ReportId.Equals(item._id)).ShareTime;
                }
                data.Add(v);
            }


            result.Result = data;
            result.Count = totalCount;
            return result;
        }

        /// <summary>
        /// 更新我分享的简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="reportId">简报Id</param>
        /// <param name="usrEmails">要分享的用户邮箱，多个用分号隔开</param>
        /// <param name="content">邮件正文</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateMyShareReport(string usr_id, string reportId, string usrEmails, string content)
        {
            ResultDto result = new ResultDto();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            if (usrObjId == ObjectId.Empty || repObjId == ObjectId.Empty)
            {
                result.Message = "参数空错误";
                return result;
            }
            var builder = Builders<Dnl_ReportShare>.Filter;
            var col = MongoDBHelper.Instance.GetDnl_ReportShare();
            //删除原有分享简报
            col.DeleteOne(builder.Eq(x => x.ReportId, repObjId));
            //新建新的分享简报
            result = SendShareEmail(usr_id, reportId, usrEmails, content);

            return result;
        }

        /// <summary>
        /// 获取分享给我的简报
        /// </summary>
        /// <param name="usr_id">用户Id</param>
        /// <param name="page">第几页</param>
        /// <param name="pagesize">一页数据量</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_ReportDto> GetShareToMeReport(string usr_id, int page, int pagesize)
        {
            QueryResult<Dnl_ReportDto> result = new QueryResult<Dnl_ReportDto>();
            var usrObjId = ObjectId.Empty;
            ObjectId.TryParse(usr_id, out usrObjId);
            if (usrObjId == ObjectId.Empty)
            {
                return result;
            }
            //获取用户邮箱
            var userCol = MongoDBHelper.Instance.Get_IW2SUser();
            var email = userCol.Find(Builders<IW2SUser>.Filter.Eq(x => x._id, usrObjId)).Project(x => x.UsrEmail).FirstOrDefault();

            //获取分享给该邮箱的简报列表
            var sharebuilder = Builders<Dnl_ReportShare>.Filter;
            var sharePrjs = MongoDBHelper.Instance.GetDnl_ReportShare().Find(sharebuilder.Eq(x => x.SharedEmail, email) & sharebuilder.Eq(x => x.IsDel, false)).ToList();
            var sharePrjIds = MongoDBHelper.Instance.GetDnl_ReportShare().Find(sharebuilder.Eq(x => x.SharedEmail, email) & sharebuilder.Eq(x => x.IsDel, false))
                .Project(x => new { ReportId = x.ReportId, Dnl_ReportShare = x }).ToList().ToDictionary(x => x.ReportId, y => y.Dnl_ReportShare);

            //获取这些简报的信息
            var builder = Builders<Dnl_Report>.Filter;
            var filter = builder.In(x => x._id, sharePrjIds.Keys.ToList());
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetDnl_Report().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<Dnl_ReportDto> data = new List<Dnl_ReportDto>();



            foreach (var item in TaskList)
            {
                Dnl_ReportDto v = new Dnl_ReportDto();
                v._id = item._id.ToString();
                v.Name = item.Name;
                v.IconUrl = item.IconUrl;
                v.ProjectId = item.ProjectId;
                v.UsrId = item.UsrId.ToString();
                v.ProjectName = item.ProjectName;
                v.SharerEmail = new List<string>();
                if (sharePrjIds.ContainsKey(item._id))
                {
                    v.ShareTime = sharePrjIds[item._id].CreatedAt;
                    string usrEmail = userCol.Find(Builders<IW2SUser>.Filter.Eq(x => x._id, sharePrjIds[item._id].UsrId)).Project(x => x.UsrEmail).FirstOrDefault();
                    v.SharerEmail.Add(usrEmail);
                }
                v.CreatedAt = item.CreatedAt;
                v.Description = item.Description;
                data.Add(v);
            }
            result.Result = data;
            result.Count = totalCount;
            return result;
        }

        /// <summary>
        /// 插入简报描述
        /// </summary>
        /// <param name="factor">报告描述因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertDescription(DescriptionFactor factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(factor.reportId, out repObjId);

            //获取用户Id
            var filterRep = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
            var userId = MongoDBHelper.Instance.GetDnl_Report().Find(filterRep).Project(x => x.UsrId).FirstOrDefault();

            //写入数据
            var data = new Dnl_Report_Description
            {
                Description = factor.description,
                IsDel = false,
                IsHide = false,
                ReportId = repObjId,
                UsrId = userId,
                Title = factor.title,
                CreatedAt=DateTime.Now.AddHours(8),
                IsStart = factor.isStart
            };
            MongoDBHelper.Instance.GetDnl_Report_Description().InsertOne(data);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取简报描述
        /// </summary>
        /// <param name="reportId">简报Id</param>
        /// <param name="isHide">是否显示已隐藏数据</param>
        /// <param name="isStart">true返回描述，false返回总结</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_Report_DescriptionDto> GetDescription(string reportId, bool isHide, bool isStart)
        {
            var result = new QueryResult<Dnl_Report_DescriptionDto>();
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            //获取数据
            var cateObjIds = new List<ObjectId>();
            var builder = Builders<Dnl_Report_Description>.Filter;
            var filter = builder.Eq(x => x.ReportId, repObjId) & builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.IsStart, isStart);
            //判断是否获取设置为不显示的数据
            if (isHide)
            {
                filter &= builder.Eq(x => x.IsHide, false);
            }
            var query = MongoDBHelper.Instance.GetDnl_Report_Description().Find(filter).Project(x => new Dnl_Report_DescriptionDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                ReportId = x.ReportId.ToString(),
                Title = x.Title,
                Description = x.Description,
                CreatedAt=x.CreatedAt
            }).ToList();
            result.Result = query;
            result.Count = query.Count;
            return result;
        }

        /// <summary>
        /// 获取简报总结
        /// </summary>
        /// <param name="reportId">简报Id</param>
        /// <param name="isHide">是否显示已隐藏数据</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_Report_DescriptionDto> GetSummary(string reportId, bool isHide)
        {
            var result = new QueryResult<Dnl_Report_DescriptionDto>();
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            //获取数据
            var cateObjIds = new List<ObjectId>();
            var builder = Builders<Dnl_Report_Description>.Filter;
            var filter = builder.Eq(x => x.ReportId, repObjId) & builder.Eq(x => x.IsDel, false);
            filter &= builder.Eq(x => x.IsStart, false);
            //判断是否获取设置为不显示的数据
            if (isHide)
            {
                filter &= builder.Eq(x => x.IsHide, false);
            }
            var query = MongoDBHelper.Instance.GetDnl_Report_Description().Find(filter).Project(x => new Dnl_Report_DescriptionDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                ReportId = x.ReportId.ToString(),
                Title = x.Title,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                isHide = x.IsHide
            }).ToList();
            result.Result = query;
            result.Count = query.Count;
            return result;
        }

        /// <summary>
        /// 修改简报描述
        /// </summary>
        /// <param name="factor">报告描述因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateDescription(DescriptionFactor factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            var filter = Builders<Dnl_Report_Description>.Filter.Eq(x => x._id, new ObjectId(factor.id));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", factor.title }, { "Description", factor.description } } } };
            MongoDBHelper.Instance.GetDnl_Report_Description().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 删除简报描述
        /// </summary>
        /// <param name="descId">描述Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelDescription(string descId)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_Description>.Filter.Eq(x => x._id, new ObjectId(descId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetDnl_Report_Description().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置简报描述显示状态
        /// </summary>
        /// <param name="descId">描述Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideDescription(string descId,bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_Description>.Filter.Eq(x => x._id, new ObjectId(descId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_Description().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取简报关键词
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_Report_KeywordCategoryDto> GetKeywordCate(string reportId, bool isReRead, bool isHide)
        {
            QueryResult<Dnl_Report_KeywordCategoryDto> result = new QueryResult<Dnl_Report_KeywordCategoryDto>();
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */

            var cateObjIds = new List<ObjectId>();
            var builderKey = Builders<Dnl_Report_Keyword>.Filter;
            var filterKey = builderKey.Eq(x => x.IsDel, false);
            var colKey = MongoDBHelper.Instance.GetDnl_Report_Keyword();
            
            var builderCate = Builders<Dnl_Report_KeywordCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ReportId, repObjId);
            filterCate &= builderCate.Eq(x => x.IsDel, false);
            var colCate = MongoDBHelper.Instance.GetDnl_Report_KeywordCategory();
            var queryCate = colCate.Find(filterCate).Project(x => new Dnl_Report_KeywordCategoryDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                ReportId = x.ReportId.ToString(),
                Name = x.Name,
                SourceType = x.SourceType,
                KeywordCount = x.KeywordCount,
                KewordList = new List<Dnl_Report_KeywordDto>(),
                IsHide = x.IsHide
            }).ToList();
            Dictionary<string, bool> nameToHide = new Dictionary<string, bool>();       //关键词组名与隐藏状态的辞典
            //判断是否重新获取数据
            if (isReRead && queryCate.Count>0)
            {
                colCate.DeleteMany(filterCate);
                var delCateObjIds = queryCate.Select(x => new ObjectId(x._id)).ToList();
                var delFilterKey = builderKey.Eq(x => x.ReportId, repObjId);
                delFilterKey = builderKey.In(x => x.CategoryId, delCateObjIds);
                colKey.DeleteMany(delFilterKey);
                //生成字典
                foreach (var cate in queryCate)
                {
                    nameToHide.Add(cate.Name, cate.IsHide);
                }
                queryCate.Clear();
            }
            if (queryCate.Count == 0)   //判断是否有存储好的数据
            {
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var proObjId = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new ObjectId(x.ProjectId)).FirstOrDefault();
                //查询项目数据获取第一级分组
                //var parentObjId = new ObjectId("000000000000000000000000");          //parentId是此值时为第一级分组
                var builderProCate = Builders<Dnl_KeywordCategory>.Filter;
                var fiterProCate = builderProCate.Eq(x => x.ProjectId, proObjId) & builderProCate.Eq(x => x.IsDel, false) & builderProCate.Eq(x => x.ParentId, ObjectId.Empty);
                var queryProCate = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(fiterProCate).ToList();
                //获取分组内关键词
                var proCateObjIds = queryProCate.Select(x => x._id).ToList();
                var builderProKey = Builders<Dnl_KeywordMapping>.Filter;
                var filterProKey = builderProKey.In(x => x.CategoryId, proCateObjIds) & builderProKey.Eq(x => x.IsDel, false);
                var queryProKey = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterProKey).ToList();

                //将这些信息写入简报相关表中
                //写入分组表
                var cateList = new List<Dnl_Report_KeywordCategory>();
                foreach (var pro in queryProCate)
                {
                    var cate = new Dnl_Report_KeywordCategory
                    {
                        UsrId = pro.UserId,
                        ReportId = repObjId,
                        Name = pro.Name,
                        IsDel = false,
                        KeywordCount = pro.KeywordCount,
                        CreatedAt = DateTime.Now.AddHours(8),
                        SourceType = 0,
                        IsHide = false
                    };
                    //继承原有词组的状态
                    if (nameToHide.ContainsKey(pro.Name))
                    {
                        cate.IsHide = nameToHide[pro.Name];
                    }
                    cateList.Add(cate);
                }
                colCate.InsertMany(cateList);
                //获取刚刚插入的分组数据
                queryCate = colCate.Find(filterCate).Project(x => new Dnl_Report_KeywordCategoryDto
                {
                    _id = x._id.ToString(),
                    UsrId = x.UsrId.ToString(),
                    ReportId = x.ReportId.ToString(),
                    Name = x.Name,
                    SourceType = x.SourceType,
                    KeywordCount = x.KeywordCount,
                    IsHide = x.IsHide
                }).ToList();
                //获取分组Id，设置关键词查询条件
                cateObjIds = queryCate.Select(x => new ObjectId(x._id)).ToList();
                filterKey &= builderKey.In(x => x.CategoryId, cateObjIds);
                //写入各分组关键词
                var keyList = new List<Dnl_Report_Keyword>();
                foreach (var pro in queryProKey)
                {
                    var proCate = queryProCate.Find(x => x._id == pro.CategoryId);
                    if (proCate != null)
                    {
                        var cate = queryCate.Find(x => x.Name==proCate.Name);
                        var key = new Dnl_Report_Keyword
                        {
                            UsrId = new ObjectId(cate.UsrId),
                            BaiduCommendId = pro.KeywordId,
                            CommendKeyword = pro.Keyword,
                            IsDel = false,
                            CategoryId = new ObjectId(cate._id),
                            CreatedAt = DateTime.Now.AddHours(8),
                            SourceType = 0,
                            ReportId = repObjId
                        };
                        keyList.Add(key);
                    } 
                }
                colKey.InsertMany(keyList);
            }
            //判断是否获取设置为不显示的数据
            if (isHide)
            {
                queryCate = queryCate.Where(x => x.IsHide == false).ToList();
            }
            //获取刚刚插入的关键词数据
            var queryKey = colKey.Find(filterKey).Project(x => new Dnl_Report_KeywordDto
            {
                _id = x._id.ToString(),
                CategoryId = x.CategoryId.ToString(),
                BaiduCommendId = x.BaiduCommendId.ToString(),
                CommendKeyword = x.CommendKeyword,
                CreatedAt = x.CreatedAt,
                SourceType = x.SourceType
            }).ToList();
            //为每个词组创建单独的关键词列表
            for (int i = 0; i < queryCate.Count; i++)
            {
                queryCate[i].KewordList = new List<Dnl_Report_KeywordDto>();
            }
            //将关键词分配到各组中去
            foreach (var key in queryKey)
            {
                var cate = queryCate.Find(x => x._id.Equals(key.CategoryId));
                if (cate != null)
                {
                    cate.KewordList.Add(key);
                }
            }
            result.Result = queryCate;
            result.Count = queryCate.Count;
            return result;
        }

        /// <summary>
        /// 设置简报关键词显示状态
        /// </summary>
        /// <param name="kewordIds">所有词组Id，用分号拼接</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideKeywordCate(string keyCateIds, bool isHide)
        {
            var cateIdList = keyCateIds.Split(';').Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_KeywordCategory>.Filter.In(x => x._id, cateIdList);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_KeywordCategory().UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_StatisticsDto GetStatistics(string reportId, bool isReRead, bool isHide)
        {
            Dnl_Report_StatisticsDto result = new Dnl_Report_StatisticsDto();
            string description = null;        //用户描述，重新获取时通过该参数保留用户描述内容
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderSta = Builders<Dnl_Report_Statistics>.Filter;
            var filterSta = builderSta.Eq(x => x.ReportId, repObjId);
            var colSta = MongoDBHelper.Instance.GetDnl_Report_Statistics();
            var querySta = colSta.Find(filterSta).Project(x => new Dnl_Report_StatisticsDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                ReportId = x.ReportId.ToString(),
                KeywordCount = x.KeywordCount,
                CreatedAt = x.CreatedAt,
                Description = x.Description,
                EndTime = x.EndTime,
                LinkCount = x.LinkCount,
                SourceCount = x.SourceCount,
                StartTime = x.StartTime,
                IsHide=x.IsHide
            }).FirstOrDefault();
            bool hideState = false;        //隐藏状态
            //判断是否重新获取数据
            if (isReRead && querySta!=null)
            {
                hideState = querySta.IsHide;
                colSta.DeleteOne(filterSta);
                description = querySta.Description;
                querySta = null;
            }
            if (querySta == null)   //判断是否有存储好的数据
            {
                var statistics = new Dnl_Report_Statistics();
                statistics.Description = description;
                statistics.CreatedAt = DateTime.Now.AddHours(8);
                statistics.ReportId = repObjId;
                statistics.SourceCount = 1;
                statistics.IsHide = hideState;
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
                {
                    Id = new ObjectId(x.ProjectId),
                    UsrId = x.UsrId
                }).FirstOrDefault();
                statistics.UsrId = pro.UsrId;
                //获取已被分组的关键词ID并去重

                var builderProKey = Builders<Dnl_KeywordMapping>.Filter;
                var filterProKey = builderProKey.Eq(x => x.ProjectId, pro.Id) & builderProKey.Eq(x => x.IsDel, false) & builderProKey.Ne(x => x.CategoryId, ObjectId.Empty);
                var queryProKey = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterProKey).Project(x => x.KeywordId.ToString()).ToList().Distinct().ToList();
                statistics.KeywordCount = queryProKey.Count;
                /* 获取已搜索链接信息
                 * 数量
                 * 起止时间*/
                var builderProLink = Builders<Dnl_Link_Baidu>.Filter;
                var filterProLink = builderProLink.In(x => x.SearchkeywordId, queryProKey) & builderProLink.Ne(x => x.PublishTime, "");
                var queryProLink = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filterProLink).Project(x => new
                {
                    Id = x._id.ToString(),
                    PublishTime = x.PublishTime
                }).ToList();
                //将发布时间转化为时间格式
                var linkTimes = new List<LinkPublishTime>();
                foreach (var link in queryProLink)
                {
                    DateTime tmpDt = new DateTime();
                    DateTime.TryParse(link.PublishTime,out tmpDt);
                    var time = new LinkPublishTime
                    {
                        Id = link.Id,
                        PublishTime = tmpDt
                    };
                    linkTimes.Add(time);
                }
                //删除异常时间 如0001-01-01与2063-23-12等时间，并排序
                linkTimes = linkTimes.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.PublishTime).ToList();
                statistics.LinkCount = linkTimes.Count;
                if (queryProLink.Count > 0)
                {
                    statistics.StartTime = linkTimes[linkTimes.Count - 1].PublishTime;
                    statistics.EndTime = linkTimes[0].PublishTime;
                }

                //写入统计信息
                colSta.InsertOne(statistics);
                //获取刚刚写入的统计信息
                querySta = colSta.Find(filterSta).Project(x => new Dnl_Report_StatisticsDto
                {
                    _id = x._id.ToString(),
                    UsrId = x.UsrId.ToString(),
                    ReportId = x.ReportId.ToString(),
                    KeywordCount = x.KeywordCount,
                    CreatedAt = x.CreatedAt,
                    Description = x.Description,
                    EndTime = x.EndTime,
                    LinkCount = x.LinkCount,
                    SourceCount = x.SourceCount,
                    StartTime = x.StartTime,
                    IsHide=x.IsHide
                }).FirstOrDefault();
            }
            //判断是否隐藏数据
            if (isHide && querySta.IsHide)
            {
                return null;
            }
            result = querySta;
            return result;
        }

        /// <summary>
        /// 修改统计信息描述
        /// </summary>
        /// <param name="staId">统计信息Id</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateStatistics(string staId, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_Report_Statistics>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(staId));
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Description", description } } } };
            MongoDBHelper.Instance.GetDnl_Report_Statistics().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置统计信息描述显示状态
        /// </summary>
        /// <param name="kewordIds">所有词组Id，用分号拼接</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideStatistics(string staId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_Statistics>.Filter.Eq(x => x._id, new ObjectId(staId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_Statistics().UpdateMany(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取关键词分析图表
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_keywordChartDto GetKeywordChart(string reportId, bool isReRead, bool isHide)
        {
            Dnl_Report_keywordChartDto result = new Dnl_Report_keywordChartDto();
            result.ReportId = reportId;
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderChart = Builders<Dnl_Report_keywordChart>.Filter;
            var filterChart = builderChart.Eq(x => x.ReportId, repObjId);
            var colChart = MongoDBHelper.Instance.GetDnl_Report_keywordChart();
            var queryChart = colChart.Find(filterChart).FirstOrDefault();
            string description = "";
            bool hideState = false;        //隐藏状态
            //判断是否重新获取数据
            if (isReRead && queryChart!=null)
            {
                description = queryChart.Description;
                hideState = queryChart.IsHide;
                colChart.DeleteOne(filterChart);
                queryChart = null;
            }
            if (queryChart == null)   //判断是否有存储好的数据
            {
                var keyChart = new Dnl_Report_keywordChart();
                keyChart.CreatedAt = DateTime.Now.AddHours(8);
                keyChart.ReportId = repObjId;
                keyChart.IsHide = hideState;
                keyChart.Description = description;
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
                {
                    Id = x.ProjectId,
                    UsrId = x.UsrId
                }).FirstOrDefault();
                keyChart.UsrId = pro.UsrId;
                //获取矩形树图json字符串
                string rectanglarTree = GetRectangularTreeStr(pro.Id);
                keyChart.Chart_RectangularTree = rectanglarTree;
                
                //获取分组树图json字符串
                string allGroupTree = GetAllGroupTreeStr(pro.Id);
                keyChart.Chart_CategoryTree = allGroupTree;

                //写入数据库
                colChart.InsertOne(keyChart);
                //从数据库中再获取数据
                queryChart = colChart.Find(filterChart).FirstOrDefault();
            }
            //判断是否隐藏数据
            if (isHide && queryChart.IsHide)
            {
                return null;
            }

            result._id = queryChart._id.ToString();
            result.CreatedAt = queryChart.CreatedAt;
            result.UsrId = queryChart.UsrId.ToString();
            result.Chart_RectangularTree = queryChart.Chart_RectangularTree;
            result.Chart_CategoryTree = queryChart.Chart_CategoryTree;
            result.Description = queryChart.Description;

            return result;
        }

        /// <summary>
        /// 修改关键词分析图表描述
        /// </summary>
        /// <param name="keyChartId">关键词图表Id</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateKeywordChart(string keyChartId, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_Report_keywordChart>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(keyChartId));
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Description", description } } } };
            MongoDBHelper.Instance.GetDnl_Report_keywordChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置关键词分析图表显示状态
        /// </summary>
        /// <param name="keyChartId">关键词图表Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideKeywordChart(string keyChartId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_keywordChart>.Filter.Eq(x => x._id, new ObjectId(keyChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_keywordChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        #region 关键词图表计算函数
        /// <summary>
        /// 文件写入
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="directory">目录</param>
        /// <param name="fileName">文件名</param>
        private void FileWirte(string text, string directory, string fileName)
        {
            if (directory.Last() != '\\')
            {
                directory += "\\";
            }
            string path = directory + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.Write(text);
                sw.Close();
            }
            else
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, Encoding.Unicode);
                sw.Write(text);
                //sw.WriteLine(value);
                sw.Close();
            }
        }

        /// <summary>
        /// 获取矩形树图字符串
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [HttpGet]
        private string GetRectangularTreeStr(string projectId)
        {
            //获取该项目下所有分组信息
            var tree = GetRectangularTree(projectId, new ObjectId("000000000000000000000000"), new List<RectangularTree>());

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

            return text;
        }

        //获取词组及关键词列表
        private List<RectangularTree> GetRectangularTree(string projectId, ObjectId parentId, List<RectangularTree> list)
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
            groupfilter &= groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
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
                GetRectangularTree(projectId, new ObjectId(treedata.Id), list);
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
        /// 获组分组树，返回json文件路径
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        private string GetAllGroupTreeStr(string projectId)
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
            var listGT = GetCategoryTree3(projectId, new ObjectId("000000000000000000000000"), list);

            //将分组信息转为树形
            GroupTree3Dto GroupTree = new GroupTree3Dto();
            GroupTree = GetCategoryTreeNode("000000000000000000000000", list);

            //将类转化成Json字符串
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string value = Serializer.Serialize(GroupTree);
            //删除无用字符串
            value = value.Replace(@",""children"":null", "");
            value = value.Replace(@",""size"":null", "");

            return value;
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
        /// 获取分组目录（含叶结点）数组
        /// </summary>
        /// <param name="usr_id"></param>
        /// <param name="projectId"></param>
        /// <param name="parentId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<GroupTree2Dto> GetCategoryTree3(string projectId, ObjectId parentId, List<GroupTree2Dto> list)
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
            groupfilter &= groupbuilder.Eq(x => x.ProjectId, new ObjectId(projectId));
            groupfilter &= groupbuilder.Eq(x => x.IsDel, false);
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
            if (TaskList.Count == 0) return null;

            //递归调用GetCategoryTree2()，获取次级词组名和当前组内关键词
            foreach (GroupTree2Dto treedata in TaskList)
            {
                list.Add(treedata);
                GetCategoryTree3(projectId, new ObjectId(treedata.id), list);
                // parent.children.Add(treedata);
            }

            return list;
        }
        #endregion

        /// <summary>
        /// 获取链接检测概况图
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_LinkOverviewDto GetLinkOverview(string reportId, bool isReRead, bool isHide)
        {
            Dnl_Report_LinkOverviewDto result = new Dnl_Report_LinkOverviewDto();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            result.ReportId = reportId;
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderView = Builders<Dnl_Report_LinkChart>.Filter;
            var filterView = builderView.Eq(x => x.ReportId, repObjId);
            filterView &= builderView.Eq(x => x.CategoryId, ObjectId.Empty);        //分组Id为空的是概况图
            var colView = MongoDBHelper.Instance.GetDnl_Report_LinkChart();
            var queryView = colView.Find(filterView).FirstOrDefault();
            string title = "";
            string description = "";
            bool hideState = false;        //隐藏状态


            //判断是否重新获取数据
            if (isReRead && queryView != null)
            {
                title = queryView.Title;
                description = queryView.Description;
                hideState = queryView.IsHide;
                colView.DeleteOne(filterView);
                queryView = null;
            }
            if (queryView == null)   //判断是否有存储好的数据
            {
                var overview = new Dnl_Report_LinkChart();
                overview.CreatedAt = DateTime.Now.AddHours(8);
                overview.ReportId = repObjId;
                overview.Title = title;
                overview.Description = description;
                overview.IsHide = hideState;
                overview.IsDel = false;
                overview.CategoryId = ObjectId.Empty;
                overview.Index = 0;
                overview.CharyType = 0;
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
                {
                    Id = x.ProjectId,
                    UsrId = x.UsrId
                }).FirstOrDefault();
                overview.UsrId = pro.UsrId;

                string cateIds = null;
                //获取项目内分组Id
                var filterCate = Builders<Dnl_KeywordCategory>.Filter.Eq(x => x.ProjectId, new ObjectId(pro.Id));
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    cateIds = string.Join(";", cateIdList);
                }
                overview.KeyCateIds = cateIds;
                //获取图表数据
                var chartData = GetTimeLinkCount(cateIds, pro.Id, null,null, 25, 25,7,0);
                //简化坐标节点
                overview.StartTime = chartData.Times[0];
                overview.EndTime = chartData.Times[chartData.Times.Count - 1];
                overview.TimeInterval = 7;
                //剔除分组统计中无数据节点
                var simpleCateList = new List<Report_LinkInfo>();         //折线图数据
                foreach (var cate in chartData.LineDataList)
                {
                    var simpleCate = new Report_LinkInfo();
                    simpleCate.CategoryId = cate.CategoryId;
                    simpleCate.Name = cate.name;
                    simpleCate.topData = cate.topData;
                    simpleCate.IndexList = new List<int>();
                    simpleCate.LinkCountList = new List<int>();
                    for (int i = 0; i < cate.LinkCount.Count; i++)
                    {
                        if (cate.LinkCount[i] > 0)
                        {
                            simpleCate.IndexList.Add(i);
                            simpleCate.LinkCountList.Add(cate.LinkCount[i]);
                        }
                    }
                    simpleCateList.Add(simpleCate);
                }
                //Json序列化对象
                var lineChart = Serializer.Serialize(simpleCateList);
                overview.LineChart = lineChart;
                var pieChart = Serializer.Serialize(chartData.Sum);
                overview.PieChart = pieChart;

                //写入信息
                colView.InsertOne(overview);
                //重获取信息
                queryView = colView.Find(filterView).FirstOrDefault();
            }
            //判断是否隐藏数据
            if (isHide && queryView.IsHide)
            {
                return null;
            }
            result._id = queryView._id.ToString();
            result.CreatedAt = queryView.CreatedAt;
            result.Title = queryView.Title;
            result.Description = queryView.Description;
            result.ReportId = queryView.ReportId.ToString();
            result.UsrId = queryView.UsrId.ToString();
            result.CategoryId = queryView.CategoryId.ToString();
            result.Index = queryView.Index;
            //重计算坐标节点
            var times = new List<DateTime>();
            DateTime tpStart = queryView.StartTime;
            DateTime tpEnd = queryView.EndTime;
            while (tpStart <= tpEnd)
            {
                times.Add(tpStart);
                tpStart = tpStart.AddDays(queryView.TimeInterval);
            }
            result.Times = times;
            //反序列化对象，解析数据
            var repLine = Serializer.Deserialize<List<Report_LinkInfo>>(queryView.LineChart);
            var repSum = Serializer.Deserialize<List<SumData>>(queryView.PieChart);
            result.PieChartData = repSum;
            //重整理分组数据中坐标节点
            result.LineChartData = new List<LineData>();
            foreach (var rep in repLine)
            {
                var newLine = new LineData();               //完整的一条折线图数据
                newLine.CategoryId = rep.CategoryId;
                newLine.name = rep.Name;
                newLine.topData = rep.topData;
                var linkCount = new List<int>();            //完整坐标节点
                for (int i = 0, j = 0; i < times.Count; i++)
                {
                    /* 判断该节点是否有数据
                     * 无数据添加0
                     * 有数据则添加对应节点数据 */
                    if (j < rep.IndexList.Count - 1)
                    {
                        if (i < rep.IndexList[j])
                        {
                            linkCount.Add(0);
                        }
                        else
                        {
                            linkCount.Add(rep.LinkCountList[j]);
                            j++;
                        }
                    }
                    else
                    {
                        linkCount.Add(0);
                    }
                }
                newLine.LinkCount = linkCount;
                result.LineChartData.Add(newLine);
            }
            return result;
        }

        /// <summary>
        /// 修改链接检测概况图
        /// </summary>
        /// <param name="overviewId">链接概览Id</param>
        /// <param name="description">标题</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateLinkOverview(string overviewId,string title, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_Report_LinkChart>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(overviewId));
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", title }, { "Description", description } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置链接检测概况图显示状态
        /// </summary>
        /// <param name="overviewId">链接概览Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideLinkOverview(string overviewId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_LinkChart>.Filter.Eq(x => x._id, new ObjectId(overviewId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        #region 折线图和饼图计算函数
        /// <summary>
        /// 有效链接统计图
        /// </summary>
        /// <param name="categoryId">关键词组ID,多个用;相连</param>
        /// <param name="prjId">项目Id</param>
        /// <param name="startTime">开始时间，为null或空时表示计算所有</param>
        /// <param name="endTime">结束时间，为null或空时表示计算所有</param>
        /// <param name="topNum">标记前多少位</param>
        /// <param name="sumNum">摘要显示多少个，0表示仅显示1个</param>
        /// <param name="timeInterval">坐标点时间间隔</param>
        /// <returns></returns>
        private TimeLinkCountDto GetTimeLinkCount(string categoryId, string prjId, string startTime, string endTime, int topNum, int sumNum, int timeInterval, int percent)
        {
            DateTime tpStart = new DateTime();
            DateTime tpEnd = new DateTime();
            DateTime.TryParse(startTime, out tpStart);
            DateTime.TryParse(endTime, out tpEnd);

            //判断是否有分组
            bool cateIsHave = false;
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIsHave = true;
            }

            TimeLinkCountDto result = new TimeLinkCountDto();
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var builder = Builders<Dnl_Link_Baidu>.Filter;
            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
            }

            //获取关键词列表
            List<string> keywordList = new List<string>();      //关键词列表
            var keyToCate = new Dictionary<string, string>();
            /* 判断是否有分组
             * 有则使用原有分组信息
             * 无则仅建立所有词一组数据 */
            var groupFilter =groupBuilder.Eq(x => x.ProjectId, new ObjectId(prjId)) & groupBuilder.Eq(x => x.IsDel, false);
            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();
            if (cateIsHave)
            {
                //从分组中获取所有关键词Id
                groupFilter &= groupBuilder.In(x => x.CategoryId, cateIds);
                var TaskList = groupCol.Find(groupFilter).Project(x => new
                {
                    BaiduCommendId = x.KeywordId.ToString(),
                    CategoryId = x.CategoryId.ToString()
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
            else
            {
                //直接获取项目中所有关键词Id
                groupFilter &= groupBuilder.Eq(x => x.CategoryId, ObjectId.Empty);
                keywordList = groupCol.Find(groupFilter).Project(x => x.KeywordId.ToString()).ToList();
            }

            //获取发表时间
            var filter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");
            var queryDatas = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(filter).Project(x => new
            {
                PublishTime = x.PublishTime,
                BaiduCommendId = x.SearchkeywordId,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            //获取包含分组ID的发布时间信息
            List<LinkStatus> categoryTime = new List<LinkStatus>();
            foreach (var x in queryDatas)
            {

                DateTime tmpDt = new DateTime();
                DateTime.TryParse(x.PublishTime, out tmpDt);
                int i = keywordList.IndexOf(x.BaiduCommendId);
                while (i != -1)
                {
                    LinkStatus v = new LinkStatus();
                    v.PublishTime = tmpDt;
                    if (cateIsHave)
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
                    categoryTime.Add(v);
                    i = keywordList.IndexOf(x.BaiduCommendId, i + 1);
                }
            }

            //删除异常时间 如0001-01-01与2063-23-12等时间，并排序
            categoryTime = categoryTime.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.PublishTime).ToList();

            //建立时间坐标
            List<DateTime> xCoordinate = new List<DateTime>();
            //int i = 1;
            if (categoryTime.Count > 0)
            {
                DateTime now = new DateTime();
                DateTime end = new DateTime();
                if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    now = categoryTime[0].PublishTime;
                    end = categoryTime[categoryTime.Count - 1].PublishTime;
                }
                else{
                    now = tpEnd;
                    end = tpStart;
                }
                while (now >= end)
                {
                    xCoordinate.Add(now);
                    now = now.AddDays(-timeInterval);
                }
            }
            xCoordinate.Reverse();
            result.Times = xCoordinate;

            //将发布时间依分组拆分
            List<CategoryList> categoryList = new List<CategoryList>();
            if (cateIsHave)
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
            foreach (var x in categoryTime)
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
                link.CategoryId = categoryData.CategoryId;

                List<int> linkCounts = new List<int>();
                if (categoryData.PublishTime.Count > 0)
                {
                    DateTime now = new DateTime();
                    DateTime end = new DateTime();
                    if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                    {
                        now = categoryTime[0].PublishTime;
                        end = categoryTime[categoryTime.Count - 1].PublishTime;
                    }
                    else
                    {
                        now = tpEnd;
                        end = tpStart;
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
            topData = topData.Where(x => x.X > categoryTime[0].PublishTime.AddYears(-1)).ToList().OrderByDescending(x => x.Y).ToList<TopData>();

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
                foreach (var x in categoryTime)
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
        #endregion


        /// <summary>
        /// 插入一组链接图表
        /// </summary>
        /// <param name="reportId">简报Id</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="index">列表中该类所处位置</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertLinkChartCate(string reportId, string title, string description,int index)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            //获取用户Id
            var filterRep = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
            var userId = MongoDBHelper.Instance.GetDnl_Report().Find(filterRep).Project(x => x.UsrId).FirstOrDefault();

            //写入数据
            var data = new Dnl_Report_LinkChartCategory
            {
                Description = description,
                IsDel = false,
                IsHide = false,
                ReportId = repObjId,
                UsrId = userId,
                Title = title,
                ChartCount = 0,
                CreatedAt = DateTime.Now.AddHours(8),
                Index = index
            };
            MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory().InsertOne(data);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取链接分析图表
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_Report_LinkChartCategoryDto> GetLinkChartCate(string reportId, bool isReRead, bool isHide)
        {
            var result = new List<Dnl_Report_LinkChartCategoryDto>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderCate = Builders<Dnl_Report_LinkChartCategory>.Filter;
            var filterCate = builderCate.Eq(x => x.ReportId, repObjId);
            filterCate &= builderCate.Eq(x => x.IsDel, false);
            var colCate = MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory();
            var queryCate = colCate.Find(filterCate).ToList().OrderBy(x => x.Index).ToList();
            if (queryCate.Count == 0)
            {
                return null;
            }
            //获取组内链接分析图表信息
            var cateIds = queryCate.Select(x => x._id).ToList();
            var builderLinkChart = Builders<Dnl_Report_LinkChart>.Filter;
            var filterLinkChart = builderLinkChart.In(x => x.CategoryId, cateIds);
            filterLinkChart &= builderLinkChart.Eq(x => x.IsDel, false);
            var colLinkChart = MongoDBHelper.Instance.GetDnl_Report_LinkChart();
            var queryLinkChart = colLinkChart.Find(filterLinkChart).ToList();
            //判断是否重新获取数据
            if (isReRead && queryLinkChart.Count > 0)   //判断是否有存储好的数据
            {
                //重计算并写入数据
                foreach (var chart in queryLinkChart)
                {
                    //反序列化对象，解析数据
                    var repLine = Serializer.Deserialize<List<Report_LinkInfo>>(chart.LineChart);
                    int topNum = 0;
                    foreach (var line in repLine)
                    {
                        topNum += line.topData.Count;
                    }
                    var repSum = Serializer.Deserialize<List<SumData>>(chart.PieChart);
                    var factor = new LinkChartFactor
                    {
                        id=chart._id.ToString(),
                        description = chart.Description,
                        index = chart.Index,
                        startTime = chart.StartTime.ToString("yyyy-MM-dd"),
                        endTime = chart.EndTime.ToString("yyyy-MM-dd"),
                        chartType = chart.CharyType,
                        lChartCateId = chart.CategoryId.ToString(),
                        keyCateIds = chart.KeyCateIds,
                        reportId = chart.ReportId.ToString(),
                        sumNum = repSum.Count,
                        timeInterval = chart.TimeInterval,
                        title = chart.Title,
                        topNum = topNum,
                        isHide=chart.IsHide
                    };
                    InsertLinkChart(factor);
                }
                //删除原有数据
                var chartIds = queryLinkChart.Select(x => x._id).ToList();
                var filterDelChart = builderLinkChart.In(x => x._id, chartIds);
                colLinkChart.DeleteMany(filterDelChart);
                //重获取数据
                queryLinkChart = colLinkChart.Find(filterLinkChart).ToList().OrderBy(x => x.Index).ToList();
            }
             //获取组内链接分析图表信息
            var builderDomainChart = Builders<Dnl_Report_DomainChart>.Filter;
            var filterDomainChart = builderDomainChart.In(x => x.CategoryId, cateIds);
            filterDomainChart &= builderDomainChart.Eq(x => x.IsDel, false);
            var colDomainChart = MongoDBHelper.Instance.GetDnl_Report_DomainChart();
            var queryDomainChart = colDomainChart.Find(filterDomainChart).ToList();
            //判断是否重新获取数据
            if (isReRead && queryDomainChart.Count > 0)   //判断是否有存储好的数据
            {
                //重计算并更新数据
                foreach (var chart in queryDomainChart)
                {
                    //获取项目Id
                    var filterUser = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                    var userId = MongoDBHelper.Instance.GetDnl_Report().Find(filterUser).Project(x => x.UsrId.ToString()).FirstOrDefault();
                    var domainCategory = Serializer.Serialize(GetAllDomainCategory(userId));
                    var chart_DomainCategory = Serializer.Serialize(GetDomainStatis(chart.keyCateIds, userId));
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "Chart_DomainCategory", chart_DomainCategory }, { "DomainCategory", domainCategory } } } };
                    var filterDomainUpdate = builderDomainChart.Eq(x => x._id, chart._id);
                    colDomainChart.UpdateOne(filterDomainUpdate, update);
                }
                //重获取数据
                queryDomainChart = colDomainChart.Find(filterDomainChart).ToList();
            }
            var cateList = new List<Dnl_Report_LinkChartCategoryDto>();     //所有分组数据
            foreach (var old in queryCate)
            {
                if(isHide&&old.IsHide){
                    continue;
                }
                var cateDto = new Dnl_Report_LinkChartCategoryDto
                {
                    _id = old._id.ToString(),
                    ReportId = old.ReportId.ToString(),
                    UsrId = old.UsrId.ToString(),
                    ChartCount = old.ChartCount,
                    CreatedAt = old.CreatedAt,
                    Description = old.Description,
                    Title = old.Title,
                    Index = old.Index
                };
                cateDto.LinkChartList = new List<Dnl_Report_LinkChartDto>();
                var LinkChartList = queryLinkChart.Where(x => x.CategoryId == old._id).OrderBy(x => x.Index).ToList();
                foreach (var oldChart in LinkChartList)
                {
                    var chartDto = new Dnl_Report_LinkChartDto();
                    //判断是否隐藏数据
                    if (isHide && oldChart.IsHide)
                    {
                        continue;
                    }
                    chartDto._id = oldChart._id.ToString();
                    chartDto.CreatedAt = oldChart.CreatedAt;
                    chartDto.Title = oldChart.Title;
                    chartDto.Description = oldChart.Description;
                    chartDto.ReportId = oldChart.ReportId.ToString();
                    chartDto.UsrId = oldChart.UsrId.ToString();
                    chartDto.CategoryId = oldChart.CategoryId.ToString();
                    chartDto.Index = oldChart.Index;
                    chartDto.CharyType = oldChart.CharyType;
                    //重计算坐标节点
                    var times = new List<DateTime>();
                    DateTime tpStart = oldChart.StartTime;
                    DateTime tpEnd = oldChart.EndTime;
                    while (tpStart <= tpEnd)
                    {
                        times.Add(tpStart);
                        tpStart = tpStart.AddDays(oldChart.TimeInterval);
                    }
                    chartDto.Times = times;
                    //反序列化对象，解析数据
                    var repLine = Serializer.Deserialize<List<Report_LinkInfo>>(oldChart.LineChart);
                    var repSum = Serializer.Deserialize<List<SumData>>(oldChart.PieChart);
                    chartDto.PieChartData = repSum;
                    //重整理分组数据中坐标节点
                    chartDto.LineChartData = new List<LineData>();
                    foreach (var rep in repLine)
                    {
                        var newLine = new LineData();               //完整的一条折线图数据
                        newLine.CategoryId = rep.CategoryId;
                        newLine.name = rep.Name;
                        newLine.topData = rep.topData;
                        var linkCount = new List<int>();            //完整坐标节点
                        for (int i = 0, j = 0; i < times.Count; i++)
                        {
                            /* 判断该节点是否有数据
                             * 无数据添加0
                             * 有数据则添加对应节点数据 */
                            if (j < rep.IndexList.Count - 1)
                            {
                                if (i < rep.IndexList[j])
                                {
                                    linkCount.Add(0);
                                }
                                else
                                {
                                    linkCount.Add(rep.LinkCountList[j]);
                                    j++;
                                }
                            }
                            else
                            {
                                linkCount.Add(0);
                            }
                        }
                        newLine.LinkCount = linkCount;
                        chartDto.LineChartData.Add(newLine);
                    }
                    cateDto.LinkChartList.Add(chartDto);
                }
                cateDto.DomainChartList = new List<Dnl_Report_DomainChartDto>();
                //获取气泡图数据
                var domainChartList = queryDomainChart.Where(x => x.CategoryId == old._id).OrderBy(x => x.Index).ToList();
                foreach (var oldChart in domainChartList)
                {
                    var chartDto = new Dnl_Report_DomainChartDto();
                    //判断是否隐藏数据
                    if (isHide && oldChart.IsHide)
                    {
                        continue;
                    }
                    chartDto._id = oldChart._id.ToString();
                    chartDto.CreatedAt = oldChart.CreatedAt;
                    chartDto.Title = oldChart.Title;
                    chartDto.Description = oldChart.Description;
                    chartDto.ReportId = oldChart.ReportId.ToString();
                    chartDto.UsrId = oldChart.UsrId.ToString();
                    chartDto.CategoryId = oldChart.CategoryId.ToString();
                    chartDto.Index = oldChart.Index;
                    chartDto.CharyType = 3;
                    chartDto.DomainCategory = Serializer.Deserialize<List<IW2S_DomainCategoryDto>>(oldChart.DomainCategory);
                    chartDto.Chart_DomainCategory = Serializer.Deserialize<List<DomainStatisDto>>(oldChart.Chart_DomainCategory);
                    cateDto.DomainChartList.Add(chartDto);
                }
                cateList.Add(cateDto);
            }
           

            result = cateList;
            return result;
        }

        /// <summary>
        /// 修改简报链接图表组
        /// </summary>
        /// <param name="lChartCateId">链接图表组Id</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateLinkChartCate(string lChartCateId, string title, string description)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            var filter = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, new ObjectId(lChartCateId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", title }, { "Description", description } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 删除简报链接图表组
        /// </summary>
        /// <param name="lChartCateId">链接图表组Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelLinkChartCate(string lChartCateId)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, new ObjectId(lChartCateId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory().UpdateOne(filter, update);
            //删除组内数据
            var filterChart = Builders<Dnl_Report_LinkChart>.Filter.Eq(x => x.CategoryId, new ObjectId(lChartCateId));
            MongoDBHelper.Instance.GetDnl_Report_LinkChart().UpdateMany(filterChart, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置简报链接图表组显示状态
        /// </summary>
        /// <param name="lChartCateId">链接图表组Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideLinkChartCate(string lChartCateId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, new ObjectId(lChartCateId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 插入简报链接图表
        /// </summary>
        /// <param name="factor">链接图表设置因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertLinkChart(LinkChartFactor factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(factor.reportId, out repObjId);
            //var keyCateIdList = keyCateIds.Split(';').Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();

            //获取数据
            var linkChart = new Dnl_Report_LinkChart();
            linkChart.CreatedAt = DateTime.Now.AddHours(8);
            linkChart.ReportId = repObjId;
            linkChart.Title = factor.title;
            linkChart.Description = factor.description;
            linkChart.IsHide = factor.isHide;
            linkChart.IsDel = false;
            linkChart.CategoryId = new ObjectId(factor.lChartCateId);
            linkChart.Index = factor.index;
            linkChart.KeyCateIds = factor.keyCateIds;
            linkChart.CharyType = factor.chartType;
            //获取项目Id
            var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
            var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
            {
                Id = x.ProjectId,
                UsrId = x.UsrId
            }).FirstOrDefault();
            linkChart.UsrId = pro.UsrId;
            if (string.IsNullOrEmpty(factor.keyCateIds))
            {
                //获取项目内分组Id
                var builderCate = Builders<Dnl_KeywordCategory>.Filter;
                var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(pro.Id)) & builderCate.Eq(x => x.ParentId, ObjectId.Empty);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    factor.keyCateIds = string.Join(";", cateIdList);
                }
            }
            //获取图表数据
            var chartData = GetTimeLinkCount(factor.keyCateIds, pro.Id,factor.startTime,factor.endTime,factor.topNum,factor.sumNum,factor.timeInterval,factor.percent);
            //判断表内是否有数据
            if (chartData.Times.Count == 0)
            {
                result.IsSuccess = false;
                result.Message = "图表内无数据";
            }
            //简化坐标节点
            linkChart.StartTime = chartData.Times[0];
            linkChart.EndTime = chartData.Times[chartData.Times.Count - 1];
            linkChart.TimeInterval = factor.timeInterval;
            //剔除分组统计中无数据节点
            var simpleCateList = new List<Report_LinkInfo>();         //折线图数据
            foreach (var cate in chartData.LineDataList)
            {
                var simpleCate = new Report_LinkInfo();
                simpleCate.CategoryId = cate.CategoryId;
                simpleCate.Name = cate.name;
                simpleCate.topData = cate.topData;
                simpleCate.IndexList = new List<int>();
                simpleCate.LinkCountList = new List<int>();
                for (int i = 0; i < cate.LinkCount.Count; i++)
                {
                    if (cate.LinkCount[i] > 0)
                    {
                        simpleCate.IndexList.Add(i);
                        simpleCate.LinkCountList.Add(cate.LinkCount[i]);
                    }
                }
                simpleCateList.Add(simpleCate);
            }
            //Json序列化对象
            var lineChart = Serializer.Serialize(simpleCateList);
            linkChart.LineChart = lineChart;
            var pieChart = Serializer.Serialize(chartData.Sum);
            linkChart.PieChart = pieChart;

            //写入图表
            MongoDBHelper.Instance.GetDnl_Report_LinkChart().InsertOne(linkChart);
            if (string.IsNullOrEmpty(factor.id))
            {
                //更新图表分组信息
                var filterCate = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, new ObjectId(factor.lChartCateId));
                var col = MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory();
                var chartCount = col.Find(filterCate).Project(x => x.ChartCount).FirstOrDefault();
                chartCount++;
                var update = new UpdateDocument { { "$set", new QueryDocument { { "ChartCount", chartCount } } } };
                col.UpdateOne(filterCate, update);
            }
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 修改简报链接图表
        /// </summary>
        /// <param name="factor">链接图表设置因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateLinkChart(LinkChartFactor factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            var filter = Builders<Dnl_Report_LinkChart>.Filter.Eq(x => x._id, new ObjectId(factor.lChartCateId));
            var col=MongoDBHelper.Instance.GetDnl_Report_LinkChart();
            //判断仅更新描述还是重新获取数据
            if (string.IsNullOrEmpty(factor.keyCateIds))
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", factor.title }, { "Description", factor.description } } } };
                col.UpdateOne(filter, update);
            }
            else
            {
                InsertLinkChart(factor);
                col.DeleteOne(filter);
            }
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 删除简报链接图表
        /// </summary>
        /// <param name="lChartCateId">链接图表Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelLinkChart(string lChartId)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_LinkChart>.Filter.Eq(x => x._id, new ObjectId(lChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            var colChart=MongoDBHelper.Instance.GetDnl_Report_LinkChart();
            colChart.UpdateOne(filter, update);
            var cateId = colChart.Find(filter).Project(x => x.CategoryId).FirstOrDefault();
            //更新图表分组信息
            var filterCate = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, cateId);
            var colCate = MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory();
            var chartCount = colCate.Find(filterCate).Project(x => x.ChartCount).FirstOrDefault();
            chartCount--;
            var updateDel = new UpdateDocument { { "$set", new QueryDocument { { "ChartCount", chartCount } } } };
            colCate.UpdateOne(filterCate, updateDel);

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置简报链接图表显示状态
        /// </summary>
        /// <param name="lChartCateId">链接图表Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideLinkChart(string lChartId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_LinkChart>.Filter.Eq(x => x._id, new ObjectId(lChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_LinkChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 获取所有域名分类
        /// </summary>
        /// <param name="usrId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public List<IW2S_DomainCategoryDto> GetAllDomainCategory(string usrId)
        {
            var usrObjId = new ObjectId(usrId);
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
            for (int i = 0; i < result.Count; i++)
            {
                var builderNum = Builders<IW2S_DomainCategoryData>.Filter;
                var filterNum = builderNum.Eq(x => x.DomainCategoryId, new ObjectId(result[i]._id));
                result[i].Num = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(filterNum).Count();
            }
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

        /// <summary>
        /// 获取域名分组统计图概览
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_DomainChartDto GetDomainOverview(string reportId, bool isReRead, bool isHide)
        {
            var result = new Dnl_Report_DomainChartDto();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderChart = Builders<Dnl_Report_DomainChart>.Filter;
            var filterChart = builderChart.Eq(x => x.ReportId, repObjId);
            var colChart = MongoDBHelper.Instance.GetDnl_Report_DomainChart();
            var queryChart = colChart.Find(filterChart).ToList().FirstOrDefault();
            string title = "";
            string description = "";
            bool hideState = false;        //隐藏状态
            //判断是否重新获取数据
            if (isReRead && queryChart != null)
            {
                title = queryChart.Title;
                description = queryChart.Description;
                hideState = queryChart.IsHide;
                colChart.DeleteOne(filterChart);
                queryChart = null;
            }
            if (queryChart == null)   //判断是否有存储好的数据
            {
                var domainChart = new Dnl_Report_DomainChart();
                domainChart.CreatedAt = DateTime.Now.AddHours(8);
                domainChart.Title = title;
                domainChart.Description = description;
                domainChart.ReportId = repObjId;
                domainChart.IsHide = hideState;
                domainChart.IsDel = false;
                domainChart.CategoryId = ObjectId.Empty;
                domainChart.Index = 0;
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
                {
                    Id = new ObjectId(x.ProjectId),
                    UsrId = x.UsrId
                }).FirstOrDefault();
                domainChart.UsrId = pro.UsrId;
                //查询项目数据获取分组
                var domainCate = GetAllDomainCategory(pro.UsrId.ToString());
                /* 判断用户是否有预设分组
                 * 有则直接使用
                 * 无则获取通用预设分组 */
                if (domainCate.Count == 0)
                {
                    domainCate = GetAllDomainCategory("000000000000000000000000");
                }
                domainChart.DomainCategory = Serializer.Serialize(domainCate);
                //查询项目数据获取分组
                var builderProCate = Builders<Dnl_KeywordCategory>.Filter;
                var fiterProCate = builderProCate.Eq(x => x.ProjectId, pro.Id) & builderProCate.Eq(x => x.IsDel, false);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(fiterProCate).Project(x=>x._id.ToString()).ToList();
                //获取分组图数据
                var cateIds = string.Join(";", cateIdList);
                domainChart.keyCateIds = cateIds;
                var chartData = GetDomainStatis(cateIds, pro.UsrId.ToString());
                domainChart.Chart_DomainCategory = Serializer.Serialize(chartData);
                //写入并重新获取数据
                colChart.InsertOne(domainChart);
                queryChart = colChart.Find(filterChart).FirstOrDefault();
            }
            if (isHide && queryChart.IsHide)
            {
                return null;
            }
            //Json反序列化
            result.Chart_DomainCategory = Serializer.Deserialize<List<DomainStatisDto>>(queryChart.Chart_DomainCategory);
            result.DomainCategory = Serializer.Deserialize<List<IW2S_DomainCategoryDto>>(queryChart.DomainCategory);
            result._id = queryChart._id.ToString();
            result.CreatedAt = queryChart.CreatedAt;
            result.Description = queryChart.Description;
            result.ReportId = queryChart.ReportId.ToString();
            result.UsrId = queryChart.UsrId.ToString();
            result.CharyType = 3;
            result.Title = queryChart.Title;
            return result;
        }

        /// <summary>
        /// 修改域名分组统计图描述
        /// </summary>
        /// <param name="domainChartId">关键词图表Id</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateDomainOverview(string domainChartId, string description)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<Dnl_Report_DomainChart>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(domainChartId));
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }

            var update = new UpdateDocument { { "$set", new QueryDocument { { "Description", description } } } };
            MongoDBHelper.Instance.GetDnl_Report_DomainChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置域名分组统计图显示状态
        /// </summary>
        /// <param name="domainChartId">关键词图表Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideDomainOverview(string domainChartId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_DomainChart>.Filter.Eq(x => x._id, new ObjectId(domainChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_DomainChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 插入简报域名气泡图
        /// </summary>
        /// <param name="factor">链接图表设置因素</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto InsertDomainChart(string reportId, string title, string description, int index, string categoryId, string keyCateIds)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            var domainChart = new Dnl_Report_DomainChart();
            domainChart.CreatedAt = DateTime.Now.AddHours(8);
            domainChart.Title = title;
            domainChart.Description = description;
            domainChart.ReportId = repObjId;
            domainChart.IsHide = false;
            domainChart.IsDel = false;
            domainChart.keyCateIds = keyCateIds;
            domainChart.Index = index;
            domainChart.CategoryId = new ObjectId(categoryId);
            //获取项目Id
            var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
            var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
            {
                Id = new ObjectId(x.ProjectId),
                UsrId = x.UsrId
            }).FirstOrDefault();
            domainChart.UsrId = pro.UsrId;
            //查询项目数据获取分组
            var domainCate = GetAllDomainCategory(pro.UsrId.ToString());
            /* 判断用户是否有预设分组
             * 有则直接使用
             * 无则获取通用预设分组 */
            if (domainCate.Count == 0)
            {
                domainCate = GetAllDomainCategory("000000000000000000000000");
            }
            domainChart.DomainCategory = Serializer.Serialize(domainCate);
            if (string.IsNullOrEmpty(keyCateIds))
            {
                //获取项目内分组Id
                var filterProCate = Builders<Dnl_KeywordCategory>.Filter.Eq(x => x.ProjectId, pro.Id);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterProCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    keyCateIds = string.Join(";", cateIdList);
                }
            }
            //获取分组图数据
            var chartData = GetDomainStatis(keyCateIds, pro.UsrId.ToString());
            domainChart.Chart_DomainCategory = Serializer.Serialize(chartData);
            //写入并重新获取数据
            var colChart = MongoDBHelper.Instance.GetDnl_Report_DomainChart();
            colChart.InsertOne(domainChart);
            //更新图表分组信息
            var filterCate = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, new ObjectId(categoryId));
            var col = MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory();
            var chartCount = col.Find(filterCate).Project(x => x.ChartCount).FirstOrDefault();
            chartCount++;
            var update = new UpdateDocument { { "$set", new QueryDocument { { "ChartCount", chartCount } } } };
            col.UpdateOne(filterCate, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 修改简报域名气泡图
        /// </summary>
        /// <param name="factor">链接图表设置因素</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto UpdateDomainChart(string domainChartId,string title, string description)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            var filter = Builders<Dnl_Report_DomainChart>.Filter.Eq(x => x._id, new ObjectId(domainChartId));
            var col = MongoDBHelper.Instance.GetDnl_Report_DomainChart();
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", title }, { "Description", description } } } };
                col.UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 删除简报域名气泡图
        /// </summary>
        /// <param name="domainChartId">气泡图Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelDomainChart(string domainChartId)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_DomainChart>.Filter.Eq(x => x._id, new ObjectId(domainChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            var colChart = MongoDBHelper.Instance.GetDnl_Report_DomainChart();
            colChart.UpdateOne(filter, update);
            var cateId = colChart.Find(filter).Project(x => x.CategoryId).FirstOrDefault();
            //更新图表分组信息
            var filterCate = Builders<Dnl_Report_LinkChartCategory>.Filter.Eq(x => x._id, cateId);
            var colCate = MongoDBHelper.Instance.GetDnl_Report_LinkChartCategory();
            var chartCount = colCate.Find(filterCate).Project(x => x.ChartCount).FirstOrDefault();
            chartCount--;
            var updateDel = new UpdateDocument { { "$set", new QueryDocument { { "ChartCount", chartCount } } } };
            colCate.UpdateOne(filterCate, updateDel);

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置简报域名气泡图显示状态
        /// </summary>
        /// <param name="domainChartId">气泡图Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideDomainChart(string domainChartId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_DomainChart>.Filter.Eq(x => x._id, new ObjectId(domainChartId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_DomainChart().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取临时链接分析图表
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_LinkChartDto GetTempLinkChart(string keyCateIds, string reportId, string startTime, string endTime, int topNum, int sumNum, int timeInterval, int percent)
        {
            var result = new Dnl_Report_LinkChartDto();
            //获取项目Id
            var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, new ObjectId(reportId));
            var proId = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x =>x.ProjectId).FirstOrDefault();
            if(string.IsNullOrEmpty(keyCateIds))
            {
                //获取项目内分组Id
                var builderCate = Builders<Dnl_KeywordCategory>.Filter;
                var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(proId)) & builderCate.Eq(x => x.IsDel, false);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    keyCateIds = string.Join(";", cateIdList);
                }
            }
            //获取图表数据
            var chartData =GetTimeLinkCount(keyCateIds, proId, startTime, endTime, topNum, sumNum, timeInterval, percent);
            
            result.Times = chartData.Times;
            result.LineChartData = chartData.LineDataList;
            result.PieChartData = chartData.Sum;
            return result;
        }

        /// <summary>
        /// 获取临时域名散点气泡图
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [HttpGet]
        public Dnl_Report_DomainChartDto GetTempDomainChart(string keyCateIds, string reportId)
        {
            var result = new Dnl_Report_DomainChartDto();
            //获取项目Id
             var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, new ObjectId(reportId));
            var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
            {
                Id = new ObjectId(x.ProjectId),
                UsrId = x.UsrId.ToString()
            }).FirstOrDefault();
            //获取关键词组Id
            if (string.IsNullOrEmpty(keyCateIds))
            {
                //获取项目内分组Id
                var filterCate = Builders<Dnl_KeywordCategory>.Filter.Eq(x => x.ProjectId, pro.Id);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    keyCateIds = string.Join(";", cateIdList);
                }
            }
            //获取图表数据
            var domainCategory = GetAllDomainCategory(pro.UsrId);
            ////获取关键词组Id
            //if (string.IsNullOrEmpty(keyCateIds))
            //{
            //    //获取项目内分组Id
            //    var filterProCate = Builders<IW2S_KeywordCategory>.Filter.Eq(x => x.ProjectId, pro.Id);
            //    var cateIdList = MongoDBHelper.Instance.GetIW2S_KeywordCategorys().Find(filterProCate).Project(x => x._id.ToString()).ToList();
            //    //判断是否有分组
            //    if (cateIdList.Count > 0)
            //    {
            //        keyCateIds = string.Join(";", cateIdList);
            //    }
            //}
            var chart_Domain = GetDomainStatis(keyCateIds, pro.UsrId);
            result.DomainCategory = domainCategory;
            result.Chart_DomainCategory = chart_Domain;
            return result;
        }

        #region 域名分组气泡图计算函数
        /// <summary>
        /// 命中关键词域名分布图
        /// </summary>
        /// <param name="categoryId">使用关键词组</param>
        /// <param name="usrId">用户Id</param>
        /// <returns></returns>
        public List<DomainStatisDto> GetDomainStatis(string categoryId, string usrId)
        {
            List<DomainStatisDto> result = new List<DomainStatisDto>();
            if (string.IsNullOrEmpty(categoryId))
            {
                return result;
            }
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
            var groupFilter = groupBuilder.In(x => x.CategoryId, cateIds);


            var groupCol = MongoDBHelper.Instance.GetDnl_KeywordMapping();

            var TaskList = groupCol.Find(groupFilter).Project(x => x.KeywordId).ToList().Select(x => x.ToString()).ToList();

            var builder = Builders<Dnl_Link_Baidu>.Filter;
            var filter = builder.In(x => x.SearchkeywordId, TaskList);
            var col = MongoDBHelper.Instance.GetDnl_Link_Baidu();
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

            var agreresult3 = col.Aggregate().Match(builder.In(x => x.SearchkeywordId, TaskList) & builder.Ne(x => x.PublishTime, ""))
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
                //ds.RankTotal = ag["RankTotal"].ToInt32() / ds.Count;
                result.Add(ds);
            }
            if (result == null || result.Count == 0)
            {
                return result;
            }
            List<string> domainNameList = result.Select(x => x.Domain).Distinct().ToList();

            var domainCatBuilder = Builders<IW2S_DomainCategoryData>.Filter;
            var domainCatFilter = domainCatBuilder.Eq(x => x.UsrId, new ObjectId(usrId)) & domainCatBuilder.In(x => x.DomainName, domainNameList);
            var domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(domainCatFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
            if (domainCategoryDatas.Count == 0)
            {
                //new ObjectId("000000000000000000000000")
                var commonFilter = domainCatBuilder.Eq(x => x.UsrId, new ObjectId("000000000000000000000000")) & domainCatBuilder.In(x => x.DomainName, domainNameList);
                domainCategoryDatas = MongoDBHelper.Instance.GetIW2S_DomainCategoryDatas().Find(commonFilter).Project(x => new { DomainCategoryId = x.DomainCategoryId, DomainName = x.DomainName }).ToList().DistinctBy(x => x.DomainName);
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
        #endregion


        /// <summary>
        /// 保存图表
        /// </summary>
        /// <param name="categoryId">关键词分组ID,多个用;相连</param>
        /// <param name="reportId">简报ID</param>
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
        public ResultDto SaveChart(string categoryId, string reportId, string startTime, string endTime, int percent, int topNum, int sumNum, int timeInterval, string sourceType, string name, string user_id)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<IW2S_ChartConfig>.Filter;
            //获取项目Id
            var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, new ObjectId(reportId));
            var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
            {
                Id = new ObjectId(x.ProjectId),
                UsrId = x.UsrId.ToString()
            }).FirstOrDefault();
            //确定该项目该信源下该设置名称是否已经存在
            var filter = builder.Eq(x => x.ProjectId, pro.Id);
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
                ProjectId = pro.Id,
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
        public List<IW2S_ChartConfigDto> GetChart(string user_id, string reportId, string sourceType)
        {
            var builder = Builders<IW2S_ChartConfig>.Filter;
            //获取项目Id
            var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, new ObjectId(reportId));
            var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
            {
                Id = new ObjectId(x.ProjectId),
                UsrId = x.UsrId.ToString()
            }).FirstOrDefault();
            //获取该项目该信源下所有保存的设置
            var filter = builder.Eq(x => x.ProjectId, pro.Id);
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

        /// <summary>
        /// 插入简报文字树
        /// </summary>
        /// <param name="factor">报告文字树因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertWordTree(ReportWordTreeInfo factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "描述不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.text))
            {
                result.Message = "文字树内容不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.keyword))
            {
                result.Message = "文字树关键词不能为空";
                return result;
            }
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(factor.reportId, out repObjId);

            //获取用户Id
            var filterRep = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
            var userId = MongoDBHelper.Instance.GetDnl_Report().Find(filterRep).Project(x => x.UsrId).FirstOrDefault();

            //写入数据
            var data = new Dnl_Report_WordTree
            {
                Description = factor.description,
                IsDel = false,
                IsHide = false,
                ReportId = repObjId,
                UsrId = userId,
                Title = factor.title,
                Keyword=factor.keyword,
                Text=factor.text,
                CreatedAt = DateTime.Now.AddHours(8)
            };
            MongoDBHelper.Instance.GetDnl_Report_WordTree().InsertOne(data);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取简报文字树
        /// </summary>
        /// <param name="reportId">简报Id</param>
        /// <param name="isHide">是否显示已隐藏数据</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<Dnl_Report_WordTreeDto> GetWordTree(string reportId, bool isHide)
        {
            var result = new QueryResult<Dnl_Report_WordTreeDto>();
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);

            //获取数据
            var cateObjIds = new List<ObjectId>();
            var builder = Builders<Dnl_Report_WordTree>.Filter;
            var filter = builder.Eq(x => x.ReportId, repObjId) & builder.Eq(x => x.IsDel, false);
            //判断是否获取设置为不显示的数据
            if (isHide)
            {
                filter &= builder.Eq(x => x.IsHide, false);
            }
            var query = MongoDBHelper.Instance.GetDnl_Report_WordTree().Find(filter).Project(x => new Dnl_Report_WordTreeDto
            {
                _id = x._id.ToString(),
                UsrId = x.UsrId.ToString(),
                ReportId = x.ReportId.ToString(),
                Title = x.Title,
                Description = x.Description,
                Text = x.Text,
                Keyword = x.Keyword,
                CreatedAt = x.CreatedAt,
                IsHide = x.IsHide
            }).ToList();
            result.Count = query.Count;
            result.Result = query;
            return result;
        }

        /// <summary>
        /// 修改简报文字树
        /// </summary>
        /// <param name="factor">报告文字树因素</param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateWordTree(ReportWordTreeInfo factor)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(factor.title))
            {
                result.Message = "标题不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.description))
            {
                result.Message = "文字树不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.text))
            {
                result.Message = "文字树内容不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(factor.keyword))
            {
                result.Message = "文字树关键词不能为空";
                return result;
            }
            var filter = Builders<Dnl_Report_WordTree>.Filter.Eq(x => x._id, new ObjectId(factor.id));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "Title", factor.title }, { "Description", factor.description }, { "Text", factor.text }, { "Keyword", factor.keyword } } } };
            MongoDBHelper.Instance.GetDnl_Report_WordTree().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 删除简报文字树
        /// </summary>
        /// <param name="treeId">文字树Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelWordTree(string treeId)
        {
            var objIds = GetObjIdListFromStr(treeId);
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_WordTree>.Filter.In(x => x._id, objIds);
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };
            MongoDBHelper.Instance.GetDnl_Report_WordTree().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 设置简报文字树显示状态
        /// </summary>
        /// <param name="descId">文字树Id</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto HideWordTree(string treeId, bool isHide)
        {
            var result = new ResultDto();
            var filter = Builders<Dnl_Report_WordTree>.Filter.Eq(x => x._id, new ObjectId(treeId));
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsHide", isHide } } } };
            MongoDBHelper.Instance.GetDnl_Report_WordTree().UpdateOne(filter, update);
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取链接详情
        /// </summary>
        /// <param name="reportId">简报ID</param>
        /// <param name="isReRead">判断是否重新获取数据</param>
        /// <param name="isHide">数据是否显示</param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_Report_TimeLinkDto> GetTimeLinkDetails(string reportId, bool isReRead, bool isHide)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();   //Json序列化与反序列化
            //将ID从string转为ObjectId
            var repObjId = ObjectId.Empty;
            ObjectId.TryParse(reportId, out repObjId);
            /* 查询是否有保存数据
             * 有则读取保存数据
             * 无则查询项目内对应数据表获取信息 */
            var builderView = Builders<Dnl_Report_LinkChart>.Filter;
            var filterView = builderView.Eq(x => x.ReportId, repObjId);
            filterView &= builderView.Eq(x => x.CategoryId, ObjectId.Empty);        //分组Id为空的是概况图
            var colView = MongoDBHelper.Instance.GetDnl_Report_LinkChart();
            var hideStateView = colView.Find(filterView).Project(x=>x.IsHide).FirstOrDefault();

            //链接详情部分查询
            var builderLink = Builders<Dnl_Report_TimeLink>.Filter;
            var filterLink = builderLink.Eq(x => x.ReportId, repObjId);
            var colLink = MongoDBHelper.Instance.GetDnl_Report_TimeLink();
            var queryLink = colLink.Find(filterLink).Project(x => new Dnl_Report_TimeLinkDto
            {
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                PublishTime = x.PublishTime,
                Title = x.Title
            }).ToList();

            //判断是否重新获取数据
            if (isReRead && queryLink.Count!=0)
            {
                colLink.DeleteMany(filterLink);
                queryLink.Clear();
            }
            if (queryLink.Count == 0)   //判断是否有存储好的数据
            {
                //获取项目Id
                var filterPro = Builders<Dnl_Report>.Filter.Eq(x => x._id, repObjId);
                var pro = MongoDBHelper.Instance.GetDnl_Report().Find(filterPro).Project(x => new
                {
                    Id = x.ProjectId,
                    UsrId = x.UsrId
                }).FirstOrDefault();

                string cateIds = null;
                //获取项目内分组Id
                var builderCate = Builders<Dnl_KeywordCategory>.Filter;
                var filterCate = builderCate.Eq(x => x.ProjectId, new ObjectId(pro.Id)) & builderCate.Eq(x => x.IsDel, false);
                var cateIdList = MongoDBHelper.Instance.GetDnl_KeywordCategory().Find(filterCate).Project(x => x._id.ToString()).ToList();
                //判断是否有分组
                if (cateIdList.Count > 0)
                {
                    cateIds = string.Join(";", cateIdList);
                }

                /*获取并写入链接详情*/
                //从绑定项目中获取数据
                var oldTimeLink = GetTimeLinkList(cateIds);
                //将数据写入到简报信息中去
                var timeLinkList = new List<Dnl_Report_TimeLink>();
                foreach (var x in oldTimeLink)
                {
                    var link = new Dnl_Report_TimeLink
                    {
                        Domain = x.Domain,
                        KeyCateId = x.KeyCateId,
                        Keywords = x.Keywords,
                        LinkUrl = x.LinkUrl,
                        PublishTime = x.PublishTime,
                        ReportId = repObjId,
                        UsrId = pro.UsrId,
                        Title = x.Title
                    };
                    timeLinkList.Add(link);
                }
                colLink.InsertMany(timeLinkList);
                queryLink = colLink.Find(filterLink).Project(x => new Dnl_Report_TimeLinkDto
                {
                    Keywords = x.Keywords,
                    Domain = x.Domain,
                    LinkUrl = x.LinkUrl,
                    PublishTime = x.PublishTime,
                    Title = x.Title
                }).ToList();
            }
            //判断是否隐藏数据
            if (isHide && hideStateView)
            {
                return null;
            }

            return queryLink;
        }

        /// <summary>
        /// 获取当前绑定项目的全部链接详情数据
        /// </summary>
        /// <param name="categoryId">关键词组Id，多个用分号相连</param>
        /// <returns></returns>
        private List<Dnl_Report_TimeLinkDto> GetTimeLinkList(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return null; 
            }
            var groupBuilder = Builders<Dnl_KeywordMapping>.Filter;
            var builder = Builders<Dnl_Link_Baidu>.Filter;
            List<ObjectId> cateIds = new List<ObjectId>();
            //拆分categoryId，转为ObjectId数组
            if (!string.IsNullOrEmpty(categoryId))
            {
                cateIds = categoryId.Split(';').Select(x => new ObjectId(x)).ToList();
            }
            var keywordList = new List<string>();
            //获取所给节点ID下关键词ID
            var keywordFilter = groupBuilder.In(x => x.CategoryId, cateIds) & groupBuilder.Eq(x => x.IsDel, false);
            var keywordQuery = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(keywordFilter).Project(x => new{
                Id=x.KeywordId.ToString(),
                CateId=x.CategoryId.ToString()}).ToList();
            keywordList = keywordQuery.Select(x => x.Id).ToList();

            //获取关键词链接发表时间
            var allTimeFilter = builder.In(x => x.SearchkeywordId, keywordList) & builder.Ne(x => x.PublishTime, "");

            var allQueryDatas = MongoDBHelper.Instance.GetDnl_Link_Baidu().Find(allTimeFilter).Project(x => new
            {
                PublishTime = x.PublishTime,
                Title = x.Title,
                LinkUrl = x.LinkUrl,
                Domain = x.Domain,
                Keywords = x.Keywords,
                KeywordId=x.SearchkeywordId
            }).ToList();

            //将发布时间从string转为DateTime
            DateTime tpTime = new DateTime();
            List<Dnl_Report_TimeLinkDto> datas = new List<Dnl_Report_TimeLinkDto>();
            foreach (var gr in allQueryDatas)
            {
                DateTime.TryParse(gr.PublishTime, out tpTime);
                Dnl_Report_TimeLinkDto data = new Dnl_Report_TimeLinkDto();
                data.PublishTime = tpTime.Date;
                data.Title = gr.Title;
                data.LinkUrl = gr.LinkUrl;
                data.Domain = gr.Domain;
                data.Keywords = gr.Keywords;
                //获取链接归属的所有分组
                var belongCateIds = keywordQuery.FindAll(x => x.Id == gr.KeywordId).Select(x=>x.CateId);
                data.KeyCateId = string.Join(";", belongCateIds);
                datas.Add(data);
            }
            
            datas = datas.Where(x => x.PublishTime > new DateTime(1753, 1, 09)).Where(x => x.PublishTime <= DateTime.Now).OrderByDescending(x => x.Title).ToList();

            //去除标题重复的数据，并将同一标题对应的多个关键词合并到一起
            List<Dnl_Report_TimeLinkDto> repeat = new List<Dnl_Report_TimeLinkDto>();
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
            return datas;
        }

        /// <summary>
        /// 折线图点击对应链接详情
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="reportId"></param>
        /// <param name="pubTime"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Dnl_Report_TimeLinkDto> GetCateTimeLink(string categoryId, string reportId, string pubTime, int timeInterval)
        {
            DateTime pubTimedt = DateTime.MinValue;
            DateTime.TryParse(pubTime, out pubTimedt);
            if (pubTimedt == DateTime.MinValue)
            {
                return null;
            }
            var builderLink = Builders<Dnl_Report_TimeLink>.Filter;
            var filterLink = builderLink.Eq(x => x.ReportId, new ObjectId(reportId));
            if (timeInterval == 1)
            {
                filterLink &= builderLink.Eq(x => x.PublishTime, pubTimedt);
            }
            else
            {
                DateTime start = pubTimedt.AddDays(-timeInterval);
                filterLink &= builderLink.Lte(x => x.PublishTime, pubTimedt);
                filterLink &= builderLink.Gt(x => x.PublishTime, start);
            }
            var colLink = MongoDBHelper.Instance.GetDnl_Report_TimeLink();
            var queryLink = colLink.Find(filterLink).Project(x => new Dnl_Report_TimeLinkDto
            {
                Keywords = x.Keywords,
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                PublishTime = x.PublishTime,
                Title = x.Title,
                KeyCateId = x.KeyCateId
            }).ToList();

            queryLink = queryLink.FindAll(x => x.KeyCateId.Contains(categoryId));
            return queryLink;
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


    #region 前后端通信类
    /// <summary>
    /// 链接图表分组类
    /// </summary>
    public class Dnl_Report_LinkChartCategoryDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public int ChartCount { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 组内图表
        /// </summary>
        public List<Dnl_Report_LinkChartDto> LinkChartList { get; set; }
        public List<Dnl_Report_DomainChartDto> DomainChartList { get; set; }
    }

    /// <summary>
    /// 域名分组散点气泡图输出类
    /// </summary>
    public class Dnl_Report_DomainChartDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public int Index { get; set; }
        /// <summary>
        /// 域名分组信息
        /// </summary>
        public List<IW2S_DomainCategoryDto> DomainCategory { get; set; }
        /// <summary>
        /// 统计图数据
        /// </summary>
        public List<DomainStatisDto> Chart_DomainCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 图表类型，1为折线图，2为饼图，3为气泡图
        /// </summary>
        public int CharyType { get; set; }
    }

    /// <summary>
    /// 文字树前端往后台传递信息时使用类
    /// </summary>
    public class ReportWordTreeInfo
    {
        public string id { get; set; }
        public string reportId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string text { get; set; }
        public string keyword { get; set; }
    }
    #endregion
    
}