using AISSystem;
using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Text.RegularExpressions;

namespace IW2S.Controllers
{
    public class AccountController : ApiController
    {
        const string usr_key = "js;iprseefree^&$";


        string connCommonsStr = AISSystem.AppSettingHelper.GetAppSetting("commonsMySqlCon");


        //登陆
        [HttpGet]
        public IW2SUserDto Login(string uName, string uPwd,string ip)
        {
            if (string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(uPwd))
                return new IW2SUserDto { Error = "用户名和密码不能为空" };
            Guid md5 = EncryptHelper.GetEncryPwd(uPwd.ToLower());
            bool dd = System.Text.RegularExpressions.Regex.IsMatch(uName, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?");
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.LoginPwd, md5);
            if (dd)
            {
                filter &= builder.Eq(x => x.UsrEmail, uName);
            }
            else
            {
                filter &= builder.Eq(x => x.LoginName, uName);
            }
            string location = "";
            Regex reg = new Regex("市|自治区|自治州|自治县");
            if (!string.IsNullOrEmpty(ip))
            {
                string url = "http://ip.chinaz.com/";
                url += ip;
                string html = WebHelper.GetHtml(url);
                if (html != null)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    /* 解析源代码 */
                    var task = doc.DocumentNode.SelectSingleNode("//*[@id=\"leftinfo\"]/div[3]/div[2]/p[2]/span[4]");                      //搜索结果
                    string str = task.InnerText.Trim();
                    location = str.Split(' ')[0].Trim();
                    if (location.Contains("省"))
                    {
                        int j = location.IndexOf("省");
                        location = location.Substring(j + 1, location.Length - j - 1);
                    }
                    else if (location.Contains("自治区"))
                    {
                        int j = location.IndexOf("自治区");
                        location = location.Substring(j + 3, location.Length - j - 3);
                    }
                    else if (location.Contains("特别行政区"))
                    {
                        location = location.Replace("特别行政区", "");
                    }
                    location = reg.Replace(location, "");                    //去除后缀
                }
            }
            var dto = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (dto != null)
            {
                int uunm = dto.UsrNum + 1;
                DateTime dt = DateTime.Now.AddHours(8);
                var updateWebsiteCount = new UpdateDocument { { "$set", new QueryDocument { { "UsrNum", uunm }, { "LastLoginAt", dt }, { "LastLoginLocation", location } } } };
                var result = MongoDBHelper.Instance.Get_IW2SUser().UpdateOne(new QueryDocument { { "_id", dto._id } }, updateWebsiteCount);
                IW2SUserDto freDto = new IW2SUserDto();
                freDto._id = dto._id.ToString();
                freDto.LoginName = dto.LoginName;
                freDto.LoginPwd = dto.LoginPwd;
                freDto.UsrRole = dto.UsrRole;
                freDto.UsrKey = dto.UsrKey;
                freDto.UsrEmail = dto.UsrEmail;
                freDto.IsEmailConfirmed = dto.IsEmailConfirmed;
                freDto.applicationState = dto.applicationState;
                freDto.Gender = dto.Gender;
                freDto.PictureSrc = dto.PictureSrc;
                freDto.Remarks = dto.Remarks;
                freDto.MobileNo = dto.MobileNo;
                //freDto.Token = Helpers.IprAuthorizeAttribute.GetToken(dto.LoginName, dto.UsrRole);
                freDto.UsrNum = uunm;
                return freDto;
            }
            else
            {
                return new IW2SUserDto { Error = "用户名或密码不正确！" };
            }

        }

        //注册
        [HttpGet]
        public IW2SUserDto Regist(string uName, string uPwd1, string uPwd2, string email)
        {
            //var code = VerifyCodeClass.YzmCode;
            //if (YZM.ToLower() != code.ToLower())
            //{
            //    return new IW2SUserDto { Error = "验证码填写错误！" };
            //}
            if (string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(uPwd1))
                return new IW2SUserDto { Error = "用户名和密码不能为空" };
            if (!uPwd1.Equals(uPwd2))
                return new IW2SUserDto { Error = "密码不一致！" };
            if (string.IsNullOrEmpty(email))
                return new IW2SUserDto { Error = "邮箱不能为空！" };
            bool dd = System.Text.RegularExpressions.Regex.IsMatch(email, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?");
            if (dd == false)
                return new IW2SUserDto { Error = "邮箱格式不正确！" };
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.LoginName, uName);
            var _usr = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (_usr != null)
                return new IW2SUserDto { Error = "用户名已经存在" };
            //  var queryTask1 = new QueryDocument { { "UsrEmail", email } };
            filter = builder.Eq(x => x.UsrEmail, email);
            IW2SUser _usr1 = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (_usr1 != null)
                return new IW2SUserDto { Error = "该邮箱已经注册过，请换一个试试！" };
            var md5 = EncryptHelper.GetEncryPwd(uPwd1.ToLower());
            _usr = new IW2SUser()
            {
                _id = ObjectId.GenerateNewId(),
                LoginName = uName,
                LoginPwd = md5,
                UsrKey = IDHelper.GetGuid(uName + usr_key),
                applicationState = false,
                IsEmailConfirmed = false,
                UsrEmail = email,
                UsrRole = UserTypes.Free,
                UsrNum = 1,
                Gender = "",
                MobileNo = "",
                Remarks = "",
                PictureSrc = "",
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectNum = 2,
                KeywordNum = 20,
                ReportNum = 2,
                LinkNum = 2000
            };
            MongoDBHelper.Instance.Get_IW2SUser().InsertOne(_usr);
            IW2SUserDto freDto = new IW2SUserDto();
            freDto._id = _usr._id.ToString();
            freDto.LoginName = _usr.LoginName;
            freDto.LoginPwd = _usr.LoginPwd;
            freDto.UsrRole = _usr.UsrRole;
            freDto.UsrKey = _usr.UsrKey;
            freDto.UsrEmail = _usr.UsrEmail;
            freDto.IsEmailConfirmed = _usr.IsEmailConfirmed;
            freDto.applicationState = _usr.applicationState;
            freDto.UsrNum = _usr.UsrNum;
            //freDto.Token = Helpers.IprAuthorizeAttribute.GetToken(_usr.LoginName, _usr.UsrRole);
            return freDto;
        }

        //修改密码
        [HttpGet]
        public IW2SUserDto ChangePwd(string uName, string oldPwd, string newPwd)
        {
            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
                return new IW2SUserDto { Error = "原密码或新密码不能为空！" };
            var md5Old = EncryptHelper.GetEncryPwd(oldPwd.ToLower());//IDHelper.GetGuid(pwd1 + pwd_key);
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.LoginName, uName);
            filter &= builder.Eq(x => x.LoginPwd, md5Old);
            var dto = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (dto == null)
            {
                return new IW2SUserDto { Error = "原密码不正确！" };
            }
            var md5New = EncryptHelper.GetEncryPwd(newPwd.ToLower());
            var updateWebsiteCount = new UpdateDocument { { "$set", new QueryDocument { { "LoginPwd", md5New } } } };
            MongoDBHelper.Instance.Get_IW2SUser().UpdateOne(new QueryDocument { { "_id", dto._id } }, updateWebsiteCount);
            IW2SUserDto freDto = new IW2SUserDto();
            freDto._id = dto._id.ToString();
            freDto.LoginName = dto.LoginName;
            freDto.LoginPwd = md5New;
            freDto.UsrRole = dto.UsrRole;
            freDto.UsrKey = dto.UsrKey;
            freDto.UsrEmail = dto.UsrEmail;
            freDto.IsEmailConfirmed = dto.IsEmailConfirmed;
            freDto.applicationState = dto.applicationState;
            freDto.UsrNum = dto.UsrNum;
            //freDto.Token = Helpers.IprAuthorizeAttribute.GetToken(dto.LoginName, dto.UsrRole);
            return freDto;
        }

        //找回密码
        [HttpGet]
        public IW2SUserDto FindPwd(string email, string loginName)
        {
            if (string.IsNullOrEmpty(email))
                return new IW2SUserDto { Error = "邮箱不能为空" };
            bool dd = System.Text.RegularExpressions.Regex.IsMatch(email, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?");
            if (dd == false)
                return new IW2SUserDto { Error = "邮箱格式不正确！" };
            // MongoDBHelper<FreeUser> dbhelper = new MongoDBHelper<FreeUser>();
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.UsrEmail, email);

            filter &= builder.Eq(x => x.LoginName, loginName);

            var _usr = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (_usr != null)
            {

                int gg = new Random().Next(100000, 999999);

                Guid md5 = EncryptHelper.GetEncryPwd(gg.ToString());
                var updateWebsiteCount = new UpdateDocument { { "$set", new QueryDocument { { "LoginPwd", md5 } } } };
                // dbhelper.Update("FreeUser", new QueryDocument { { "_id", _usr._id } }, updateWebsiteCount);
                MongoDBHelper.Instance.Get_IW2SUser().UpdateOne(new QueryDocument { { "_id", _usr._id } }, updateWebsiteCount);
                Helpers.EmailHelper ems = new Helpers.EmailHelper();
                string CC = "";
                string Bcc = "";
                string Subject = "密码找回";//主题
                System.Net.Mail.LinkedResource[] EmbeddedResources = null;//嵌入的外部资源
                System.Net.Mail.Attachment[] Attachments = null;//附件
                string From = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_acc");//发件人
                string UserName = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_name");//发件人用户名
                string Password = AISSystem.AppSettingHelper.GetAppSetting("ems_usr_pwd");//密码
                string Server = AISSystem.AppSettingHelper.GetAppSetting("ems_smtp");//发件邮箱服务器
                int Port = int.Parse(AISSystem.AppSettingHelper.GetAppSetting("ems_smtp_port"));//邮箱端口
                bool UseSSL = false;//是否使用
                ems.SendMail(email, From, Bcc, Subject, "<div><p>" + _usr.LoginName + "，您好！</p><p>您的初始密码是：" + gg + "</p><p>登陆系统后，请自行修改密码！</p></div>",
                    EmbeddedResources, Attachments, UserName, UserName, "xiaofeng123", "smtp.163.com", 25, UseSSL);
                return new IW2SUserDto { Error = "密码已经发送到您的邮箱，请及时查看！" };
            }
            else
            {
                return new IW2SUserDto { Error = "您填写的邮箱与注册时填写不一致，请重试！" };
            }

        }

        //修改用户信息
        [HttpPost]
        public IW2SUserDto UpdateUser(string uName, IW2SUserDto user)
        {
            if (string.IsNullOrEmpty(uName))
                return new IW2SUserDto { Error = "用户名不能为空" };
            // MongoDBHelper<FreeUser> dbhelper = new MongoDBHelper<FreeUser>();
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.LoginName, uName);
            var _usr = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (_usr != null)
            {
                var updateUser = new UpdateDocument { { "$set", new QueryDocument { { "Gender", user.Gender }, { "MobileNo", user.MobileNo }, { "Remarks", user.Remarks }, 
                { "UsrEmail", user.UsrEmail }, { "PictureSrc", user.PictureSrc } } } };
                //  dbhelper.Update("FreeUser", new QueryDocument { { "_id", _usr._id } }, updateUser);
                MongoDBHelper.Instance.Get_IW2SUser().UpdateOne(new QueryDocument { { "_id", _usr._id } }, updateUser);
                IW2SUserDto freDto = new IW2SUserDto();
                freDto._id = _usr._id.ToString();
                freDto.LoginName = _usr.LoginName;
                freDto.LoginPwd = _usr.LoginPwd;
                freDto.UsrRole = _usr.UsrRole;
                freDto.UsrKey = _usr.UsrKey;
                freDto.UsrEmail = user.UsrEmail;
                freDto.Gender = user.Gender;
                freDto.MobileNo = user.MobileNo;
                freDto.Remarks = user.Remarks;
                freDto.IsEmailConfirmed = _usr.IsEmailConfirmed;
                freDto.applicationState = _usr.applicationState;
                freDto.UsrNum = _usr.UsrNum;
                freDto.PictureSrc = _usr.PictureSrc;
                //freDto.Token = Helpers.IprAuthorizeAttribute.GetToken(_usr.LoginName, _usr.UsrRole);
                return freDto;
            }
            else
            {
                return new IW2SUserDto { Error = "用户名不正确，请重试！" };
            }
        }



        //获取用户信息
        [HttpGet]
        public IW2SUserDto GetUser(string uName, string uPwd)
        {
            if (string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(uPwd))
                return new IW2SUserDto { Error = "用户名和密码不能为空" };
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Eq(x => x.LoginName, uName);
            bool dd = System.Text.RegularExpressions.Regex.IsMatch(uName, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?");
            if (dd)
            {
                builder.Eq(x => x.UsrEmail, uName);
            }
            var dto = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).FirstOrDefault();
            if (dto != null)
            {
                IW2SUserDto freDto = new IW2SUserDto();
                freDto._id = dto._id.ToString();
                freDto.LoginName = dto.LoginName;
                freDto.LoginPwd = dto.LoginPwd;
                freDto.UsrRole = dto.UsrRole;
                freDto.UsrKey = dto.UsrKey;
                freDto.UsrEmail = dto.UsrEmail;
                freDto.IsEmailConfirmed = dto.IsEmailConfirmed;
                freDto.applicationState = dto.applicationState;
                freDto.Gender = dto.Gender;
                freDto.MobileNo = dto.MobileNo;
                freDto.Remarks = dto.Remarks;
                freDto.PictureSrc = dto.PictureSrc;
                //freDto.Token = Helpers.IprAuthorizeAttribute.GetToken(dto.LoginName, dto.UsrRole);
                freDto.UsrNum = dto.UsrNum;
                return freDto;
            }
            else
            {
                return new IW2SUserDto { Error = "用户名或密码不正确！" };
            }

        }


        [HttpGet]
        public List<IW2SUserDto> GetUserList()
        {
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Where(x => x.UsrRole == 0);
            var dto = MongoDBHelper.Instance.Get_IW2SUser().Find(filter).SortByDescending(x => x.UsrNum).ToList();
            List<IW2SUserDto> list = new List<IW2SUserDto>();
            foreach (var item in dto)
            {
                IW2SUserDto freDto = new IW2SUserDto();
                freDto._id = item._id.ToString();
                freDto.LoginName = item.LoginName;
                list.Add(freDto);
            }
            return list;
        }

        #region 用户权限管理
        /// <summary>
        /// 获取用户数量统计
        /// </summary>
        /// <param name="extent">计算距现在多少天之内的数据</param>
        /// <returns></returns>
        [HttpGet]
        public UserCount GetUserCount(int extent)
        {
            var userCount = new UserCount();
            //获取所有用户信息
            var filterUser = Builders<IW2SUser>.Filter.Empty;
            var users = MongoDBHelper.Instance.Get_IW2SUser().Find(filterUser).ToList();

            DateTime dt = DateTime.Now.AddDays(-extent);
            userCount.ActiveUser = users.Count(x => x.LastLoginAt > dt);
            userCount.ActiveUserPercent = (double)userCount.ActiveUser / users.Count;
            userCount.NewFreeUser = users.Where(x => x.UsrRole == UserTypes.Free).Count(x => x.CreatedAt > dt);
            userCount.NewPurchaseUser = users.Where(x => x.UsrRole != UserTypes.Admin && x.UsrRole != UserTypes.Free && x.UsrRole != UserTypes.Engineer).Count(x => x.PurchaseAt > dt);
            userCount.PurchaseUser = users.Count(x => x.UsrRole != UserTypes.Admin && x.UsrRole != UserTypes.Free && x.UsrRole != UserTypes.Engineer);
            userCount.PurchaseeUserPercent = (double)userCount.PurchaseUser / users.Count;
            return userCount;
        }

        /// <summary>
        /// 用户变化趋势折线图
        /// </summary>
        /// <param name="extent">计算距现在多少天之内的数据</param>
        /// <param name="interval">时间坐标间隔，单位为天</param>
        /// <param name="userType">用户类型，0为所有用户，1为活跃用户，2为新增用户</param>
        /// <returns></returns>
        [HttpGet]
        public UserLineChart GetUserLineChart(int extent,int interval,int userType)
        {
            var line = new UserLineChart();
            //获取所有用户信息
            var filterUser = Builders<IW2SUser>.Filter.Empty;
            var users = MongoDBHelper.Instance.Get_IW2SUser().Find(filterUser).ToList();
            //建立时间坐标系
            DateTime end = DateTime.Now.AddDays(1).AddSeconds(-1);
            DateTime start = end.AddDays(-extent).Date;
            line.Time = new List<string>();
            while (start <= end)
            {
                line.Time.Add(end.Date.ToString("yyyy-MM-dd"));
                end = end.AddDays(-interval);
            }
            line.Time.Reverse();

            DateTime dt = DateTime.Now.AddDays(-7);      //活跃用户和新增用户判定时间

            switch (userType)
            {
                case 0:     //所有用户
                    {
                        //获取普通用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Free = new List<int>();
                        while (start <= end)
                        {
                            line.Free.Add(users.Where(x => x.CreatedAt < end && x.UsrRole == UserTypes.Free).Count());
                            end = end.AddDays(-interval);
                        }

                        //获取付费用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Purchase = new List<int>();
                        while (start <= end)
                        {
                            line.Purchase.Add(users.Where(x => x.CreatedAt < end && x.UsrRole == UserTypes.Vip1).Count());
                            end = end.AddDays(-interval);
                        }
                        break;
                    }
                case 1:     //活跃用户
                    {
                        //获取普通用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Free = new List<int>();
                        while (start <= end)
                        {
                            line.Free.Add(users.Where(x => x.CreatedAt < end && x.LastLoginAt > dt && x.UsrRole == UserTypes.Free).Count());
                            end = end.AddDays(-interval);
                        }

                        //获取付费用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Purchase = new List<int>();
                        while (start <= end)
                        {
                            line.Purchase.Add(users.Where(x => x.CreatedAt < end && x.LastLoginAt > dt && x.UsrRole == UserTypes.Vip1).Count());
                            end = end.AddDays(-interval);
                        }
                        break;
                    }
                case 2:     //新增用户
                    {
                        //获取普通用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Free = new List<int>();
                        while (start <= end)
                        {
                            line.Free.Add(users.Where(x => x.CreatedAt < end && x.CreatedAt > end.AddDays(-interval) && x.UsrRole == UserTypes.Free).Count());
                            end = end.AddDays(-interval);
                        }

                        //获取付费用户变化数
                        end = DateTime.Now.AddDays(1).AddSeconds(-1);
                        line.Purchase = new List<int>();
                        while (start <= end)
                        {
                            line.Purchase.Add(users.Where(x => x.CreatedAt < end && x.CreatedAt > end.AddDays(-interval) && x.UsrRole == UserTypes.Vip1).Count());
                            end = end.AddDays(-interval);
                        }
                        break;
                    }
            }
            line.Free.Reverse();
            line.Purchase.Reverse();
            return line;
        }

        /// <summary>
        /// 用户地理分布气泡图
        /// </summary>
        /// <param name="userType">用户类型，0为所有用户，1为活跃用户，2为付费用户，3为普通用户</param>
        /// <returns></returns>
        [HttpGet]
        public List<UserDistribution> GetUserDistribution(int userType)
        {
            var userDistributions = new List<UserDistribution>();
            var builder = Builders<IW2SUser>.Filter;
            var filter = builder.Ne(x => x.UsrRole, UserTypes.Admin) & builder.Ne(x => x.UsrRole, UserTypes.Engineer);
            var col = MongoDBHelper.Instance.Get_IW2SUser();
            DateTime dt = DateTime.Now.AddDays(-7).AddHours(8);      //活跃用户和新增用户判定时间
            switch (userType)
            {
                case 0:
                    break;
                case 1:
                    filter &= builder.Gte(x => x.LastLoginAt, dt);
                    break;
                case 2:
                    filter &= builder.Ne(x => x.UsrRole, UserTypes.Free);
                    break;
                case 3:
                    filter &= builder.Eq(x => x.UsrRole, UserTypes.Free);
                    break;
            }
            var query = col.Find(filter).ToList();
            //按地区分组并统计数量
            userDistributions = query.Where(x => !string.IsNullOrEmpty(x.LastLoginLocation)).GroupBy(x => x.LastLoginLocation).Select(x => new UserDistribution
            {
                name = x.Key,
                value = x.Count()
            }).ToList();
            return userDistributions;
        }

        /// <summary>
        /// 用户详细信息
        /// </summary>
        /// <param name="page">页数，从0开始</param>
        /// <param name="pagesize">1页多少数据</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<UserCountInfo> GetUserInfo(int page, int pagesize)
        {
            var result = new QueryResult<UserCountInfo>();
            //获取用户基础信息
            var builderUser = Builders<IW2SUser>.Filter;
            var filterUser = builderUser.Ne(x => x.UsrRole, UserTypes.Admin) & builderUser.Ne(x => x.UsrRole, UserTypes.Engineer);
            var colUser = MongoDBHelper.Instance.Get_IW2SUser();
            var queryUser = colUser.Find(filterUser).ToList();
            result.Count = queryUser.Count;
            queryUser = queryUser.Skip((page) * pagesize).Take(pagesize).ToList();
            var userObjIds = queryUser.Select(x => x._id).ToList();     //用户Id列表

            //获取项目信息
            var buiderPro = Builders<IW2S_Project>.Filter;
            var filterPro = buiderPro.In(x => x.UsrId, userObjIds) & buiderPro.Eq(x => x.IsDel, false);
            var queryPro = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).Project(x => new
            {
                Id = x._id,
                UserId = x.UsrId
            }).ToList();
            var proObjIds = queryPro.Select(x => x.Id).ToList();     //项目Id列表

            //获取关键词数
            var builderKeyMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterKeyMap = builderKeyMap.In(x => x.ProjectId, proObjIds) & builderKeyMap.Eq(x => x.IsDel, false) & builderKeyMap.Eq(x => x.CategoryId, ObjectId.Empty);
            var queryKeyMap = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterKeyMap).Project(x => new
            {
                KeywordId = x.KeywordId,
                UserId = x.UserId
            }).ToList();
            var keyObjIds = queryKeyMap.Select(x => x.KeywordId).Distinct().ToList();     //关键词Id列表

            //获取链接数量
            var builderKey = Builders<Dnl_Keyword>.Filter;
            var filterKey = builderKey.In(x => x._id, keyObjIds);
            var queryKey = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => new
            {
                Id = x._id,
                LinkNum=x.LinkCount_Baidu
            }).ToList();

            //获取报告信息
            var builderReport = Builders<Dnl_Report>.Filter;
            var filterReport = builderReport.In(x => x.UsrId, userObjIds) & builderReport.Eq(x => x.IsDel, false);
            var queryReport = MongoDBHelper.Instance.GetDnl_Report().Find(filterReport).Project(x => new
            {
                Id = x._id,
                UserId = x.UsrId
            }).ToList();

            //获取项目信息
            var userInfos = new List<UserCountInfo>();
            foreach (var x in queryUser)
            {
                var info = new UserCountInfo
                {
                    Id = x._id.ToString(),
                    Contract = x.Contract,
                    Balance = x.Balance,
                    UserName = x.LoginName,
                    UserType = x.UsrRole.ToString()
                };
                info.ProjectNum = queryPro.Count(s => s.UserId == x._id);
                var userKeyIds = queryKeyMap.Where(s => s.UserId == x._id).Select(s => s.KeywordId).ToList();
                info.KeywordNum = userKeyIds.Count;
                var keys = queryKey.FindAll(s => userKeyIds.Contains(s.Id));
                int linkNum = 0;
                foreach (var y in keys)
                {
                    linkNum += y.LinkNum;
                }
                info.LinkNum = linkNum;
                info.ReportNum = queryReport.Count(s => s.UserId == x._id);
                userInfos.Add(info);
            }
            result.Result = userInfos;
            return result;
        }
        #endregion


        [HttpGet]
        public UserInfo GetUserInfoById(string userId)
        {
            var userObjId = new ObjectId(userId);
            //获取用户基础信息
            var builderUser = Builders<IW2SUser>.Filter;
            var filterUser = builderUser.Eq(x => x._id, userObjId);
            var colUser = MongoDBHelper.Instance.Get_IW2SUser();
            var queryUser = colUser.Find(filterUser).FirstOrDefault();

            //获取项目信息
            var buiderPro = Builders<IW2S_Project>.Filter;
            var filterPro = buiderPro.Eq(x => x.UsrId, userObjId) & buiderPro.Eq(x => x.IsDel, false);
            var queryPro = MongoDBHelper.Instance.GetIW2S_Projects().Find(filterPro).Project(x => new
            {
                Id = x._id,
                UserId = x.UsrId
            }).ToList();
            var proObjIds = queryPro.Select(x => x.Id).ToList();     //项目Id列表

            //获取关键词数
            var builderKeyMap = Builders<Dnl_KeywordMapping>.Filter;
            var filterKeyMap = builderKeyMap.In(x => x.ProjectId, proObjIds) & builderKeyMap.Eq(x => x.IsDel, false) & builderKeyMap.Eq(x => x.CategoryId, ObjectId.Empty);
            var queryKeyMap = MongoDBHelper.Instance.GetDnl_KeywordMapping().Find(filterKeyMap).Project(x => new
            {
                KeywordId = x.KeywordId,
                UserId = x.UserId
            }).ToList();
            var keyIds = queryKeyMap.Select(x => x.KeywordId).Distinct().ToList();     //关键词Id列表

            //获取链接信息
            var builderKey = Builders<Dnl_Keyword>.Filter;
            var filterKey = builderKey.In(x => x._id, keyIds);
            var queryKey = MongoDBHelper.Instance.GetDnl_Keyword().Find(filterKey).Project(x => new
            {
                Id = x._id,
                LinkNum = x.LinkCount_Baidu
            }).ToList();

            //获取报告信息
            var builderReport = Builders<Dnl_Report>.Filter;
            var filterReport = builderReport.Eq(x => x.UsrId, userObjId) & builderReport.Eq(x => x.IsDel, false);
            var queryReport = MongoDBHelper.Instance.GetDnl_Report().Find(filterReport).Project(x => new
            {
                Id = x._id,
                UserId = x.UsrId
            }).ToList();

            //获取项目信息
            var info = new UserInfo
            {
                UserName = queryUser.LoginName,
                UserType = queryUser.UsrRole.ToString(),
                MaxDataAnalysisNum = queryUser.DataAnalysisNum,
                MaxKeywordNum = queryUser.KeywordNum,
                MaxLinkNum = queryUser.LinkNum,
                MaxProjectNum = queryUser.ProjectNum,
                MaxReportNum = queryUser.ReportNum,
                MaxSupportNum = queryUser.SupportNum,
                Id = queryUser._id.ToString(),
                Nickname = queryUser.Gender,
                PictureSrc = queryUser.PictureSrc,
                Balance=queryUser.Balance
            };
            if (queryUser.Contract != null)
            {
                info.ContractNum = queryUser.Contract.Count;
            }
            info.ProjectNum = queryPro.Count(s => s.UserId == queryUser._id);
            var userKeyIds = queryKeyMap.Where(s => s.UserId == queryUser._id).Select(s => s.KeywordId).ToList();
            info.KeywordNum = userKeyIds.Count;
            var keys = queryKey.FindAll(s => userKeyIds.Contains(s.Id));
            int linkNum = 0;
            foreach (var x in keys)
            {
                linkNum += x.LinkNum;
            }
            info.LinkNum = linkNum;
            info.ReportNum = queryReport.Count(s => s.UserId == queryUser._id);
            return info;
        }
    }
}