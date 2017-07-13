using kuerjiudian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AISSystem;
using System.Data;
using kuerjiudian.DAL;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using MySql.Data.MySqlClient;


namespace kuerjiudian.Controllers
{
    public class AccountController : ApiController
    {
        const string usr_key = "js;myapps^&$";

        string connCommonsStr = AISSystem.AppSettingHelper.GetAppSetting("MAppEntitiesPOCO");

        [HttpGet]
        public UsrDto Login(string usr_name, string pwd)
        {
            if (string.IsNullOrEmpty(usr_name) || string.IsNullOrEmpty(pwd))
                return new UsrDto { Error = "用户名和密码不能为空" };
            Guid md5 = EncryptHelper.GetEncryPwd(pwd);
            //var _usr = comm.usrs.FirstOrDefault(x => x.UsrName == usr_name && x.PwdMD5 == md5);
            //if (_usr == null)
            //    return new UsrDto { Error = "用户名或密码不对" };
            string selsql = @"select ID,LoginName,LoginPwd,NickName,UserPhone,HeadIcon,RoleId,UserEmail,status,CreatedAt from huangguan_user where LoginName=N'{0}' and LoginPwd='{1}'"
                .FormatStr(usr_name, md5);
            DataTable dtq = MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
            if (dtq.Rows.Count > 0)
            {
                UsrDto dto = new UsrDto();
                dto.ID = Guid.Parse(dtq.Rows[0]["ID"].ToString());
                dto.LoginName = dtq.Rows[0]["LoginName"].ToString();
                dto.LoginPwd = dtq.Rows[0]["LoginPwd"].ToString();
                dto.NickName = dtq.Rows[0]["NickName"].ToString();
                dto.UserPhone = dtq.Rows[0]["UserPhone"].ToString();
                dto.HeadIcon = dtq.Rows[0]["HeadIcon"].ToString();
                dto.RoleId = dtq.Rows[0]["RoleId"].ToString();
                dto.UserEmail = dtq.Rows[0]["UserEmail"].ToString();
                dto.status = Convert.ToInt32(dtq.Rows[0]["status"].ToString());
                dto.CreatedAt = Convert.ToDateTime(dtq.Rows[0]["CreatedAt"].ToString());
                dto.Token = IprAuthorizeAttribute.GetToken(dto.LoginName, 0);
                return dto;
            }
            else
            {
                return new UsrDto { Error = "用户名或密码不对" };
            }
        }

        [HttpPost]
        public UsrDto Regsit(UsrDto dddd)
        {
            var code = VerifyCodeClass.YzmCode;
            if (dddd.YZM.ToLower() != code.ToLower())
            {
                return new UsrDto { Error = "验证码填写错误！" };
            }
            return RegisterSub(dddd);
        }


        public UsrDto RegisterSub(UsrDto dddd)
        {
            if (string.IsNullOrEmpty(dddd.LoginName) || string.IsNullOrEmpty(dddd.LoginPwd))
                return new UsrDto { Error = "用户名和密码不能为空" };
            string selsql = @"select ID from huangguan_user where LoginName=N'{0}'"
               .FormatStr(dddd.LoginName);
            DataTable dtq = MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
            if (dtq.Rows.Count > 0)
            {
                return new UsrDto { Error = "用户名已经存在" };
            }
            else
            {
                var md5 = EncryptHelper.GetEncryPwd(dddd.LoginPwd);
                Guid id = Guid.NewGuid();
                string insertsql = @"insert into huangguan_user(ID,LoginName,LoginPwd,NickName,UserPhone,RoleId,UserEmail,status,CreatedAt) 
values('{0}',N'{1}','{2}',N'{3}','{4}','{5}','{6}','{7}','{8}')".FormatStr(id, dddd.LoginName, md5, dddd.LoginName, dddd.UserPhone, 2, dddd.UserEmail, 0, DateTime.Now);
                int resultrow = MySqlDbHelper.ExecuteSql(connCommonsStr, insertsql);
                if (resultrow > 0)
                {
                    UsrDto dto = new UsrDto();
                    dto.ID = id;
                    dto.LoginName = dddd.LoginName;
                    dto.LoginPwd = md5.ToString();
                    dto.NickName = dddd.LoginName;
                    dto.UserPhone = dddd.UserPhone;
                    dto.HeadIcon = "";
                    dto.RoleId = "2";
                    dto.UserEmail = dddd.UserEmail;
                    dto.status = 0;
                    dto.CreatedAt = DateTime.Now;
                    dto.Token = IprAuthorizeAttribute.GetToken(dto.LoginName, 0);
                    return dto;
                }
                else
                {
                    return new UsrDto { Error = "注册失败，请重新填写！" };
                }
            }
        }



        [HttpGet]
        //获取总注册用户 和 当月注册用户
        public UserNum GetUserNum()
        {
            DateTime ddddddd = DateTime.Now;
            var dfdf = ddddddd.Year + "-" + ddddddd.Month + "-" + "1";
            var ggggg = DateTime.Parse(dfdf);
            string sql1 = "SELECT COUNT(ID) num FROM huangguan_user ";
            string sql2 = "SELECT COUNT(ID) num FROM huangguan_user WHERE CreatedAt >='{0}' ".FormatStr(ggggg);
            DataTable dt1 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql1);
            DataTable dt2 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql2);
            UserNum cr = new UserNum();
            cr.TotalNum = Convert.ToInt32(dt1.Rows[0]["num"]);
            cr.MonthNum = Convert.ToInt32(dt2.Rows[0]["num"]);
            return cr;
        }




        [HttpGet]
        //找回密码
        public ResultInfo FindPwd(string usr, string email)
        {
            ResultInfo result = new ResultInfo();
            if (string.IsNullOrEmpty(usr))
            {
                result.IsSuccess = false;
                result.Message = "用户名不能为空";
                return result;
            }
            string selsql = "select ID from huangguan_user where LoginName=N'{0}'".FormatStr(usr);
            DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
            if (dt.Rows.Count > 0)
            {
                string selemail = "select ID from huangguan_user where LoginName=N'{0}' and UserEmail='{1}'";
                DataTable dtrow = MySqlDbHelper.ExecuteQuery(connCommonsStr, selemail);
                if (dtrow.Rows.Count <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "填写的邮箱与该用户注册邮箱不一致！";
                    return result;
                }
                int n = new Random().Next(100000, 999999);
                var md5 = EncryptHelper.GetEncryPwd(n.ToString());
                string updatesql = @"update huangguan_user set LoginPwd='{0}' where LoginName='{1}'".FormatStr(md5, usr);
                int countrow = MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
                if (countrow > 0)
                {
                    EmailHelper ems = new EmailHelper();
                    ems.SendMail(email, "", null, "用户找回密码", "<div><p>您的初始密码是：" + n + "</p><p>登陆系统后，请自行修改密码！</p></div>",
                        null, null,
                        //"sinofaith_ips@163.com", "sinofaith_ips@163.com", "sino_ips", 
                        "sharabei@163.com", "sharabei@163.com", "ttttt33333",
                        "smtp.163.com", 25, false);
                    result.Message = "密码已经发送到您注册邮箱，请查收邮件！";
                    result.IsSuccess = true;
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "找回密码失败";
                    return result;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "用户名不存在";
                return result;
            }
        }

        [HttpGet]
        //修改密码
        public ResultInfo ChangePwd(string usr, string pwd1, string pwd2)
        {
            ResultInfo result = new ResultInfo();
            if (string.IsNullOrEmpty(usr) || string.IsNullOrEmpty(pwd2))
            {
                result.IsSuccess = false;
                result.Message = "用户名和密码不能为空";
                return result;
            }
            Guid md5 = EncryptHelper.GetEncryPwd(pwd1);
            string selsql = @"select ID,LoginName,LoginPwd,NickName,UserPhone,HeadIcon,RoleId,UserEmail,status,CreatedAt from huangguan_user where LoginName=N'{0}' and LoginPwd='{1}'"
                .FormatStr(usr, md5);
            DataTable dtq = MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
            if (dtq.Rows.Count < 0)
            {
                result.Message = "原始密码不正确";
                result.IsSuccess = false;
                return result;
            }
            var Newmd5 = EncryptHelper.GetEncryPwd(pwd2);
            string updatesql = @"update huangguan_user set LoginPwd='{0}' where LoginName='{1}'".FormatStr(Newmd5, usr);
            int countrow = MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
            if (countrow > 0)
            {
                result.Message = "密码修改成功";
                result.IsSuccess = true;
                return result;
            }
            else
            {
                result.Message = "密码修改失败";
                result.IsSuccess = false;
                return result;
            }

        }

        //修改用户信息
        [HttpPost]
        public string UpdateUser(string loginName, UsrDto usrdata)
        {
            string sql = "select ID from huangguan_user where NickName =N'{0}'".FormatStr(usrdata.NickName);
            DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql);
            if (dt.Rows.Count > 0)
            {
                return "用户昵称已经存在了，请换一个！";
            }
            string updatesql = @"update huangguan_user set NickName=N'{0}',UserPhone='{1}',UserEmail='{2}' 
                                where LoginName=N'{3}'".FormatStr(usrdata.NickName, usrdata.UserPhone, usrdata.UserEmail, loginName);
            int count = MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
            return count > 0 ? "成功！" : "失败！";
        }
        /// <summary>
        /// 保存用户头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultInfo SaveUserHeadIcon(Guid id, string url, string baseurl)
        {
            ResultInfo r = new ResultInfo();
            string sql = "select LoginName from huangguan_user where ID='{0}'".FormatStr(id);
            DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql);
            if (dt.Rows.Count > 0)
            {
                string imgId = Guid.NewGuid().ToString();
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "UploadFile\\XiuXiuUpload\\"; //当前路径 
                int index = url.LastIndexOf('/');
                if (index >= 0 && url.Length > index + 1)
                {
                    path += url.Substring(index + 1);
                    if (!string.IsNullOrEmpty(path))
                    {

                        string upload_file_url = baseurl + "/api/Account/upload_ipfile?md5=" + imgId + "&content_type=png";
                        // WebApiHelper.UploadFile(upload_file_url, path);

                        WebClient wc = new WebClient();
                        byte[] sendData = System.Text.Encoding.UTF8.GetBytes(path);
                        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        wc.Headers.Add("ContentLength", sendData.Length.ToString());
                        byte[] recData = wc.UploadFile(upload_file_url, "POST", path);
                        string stro = (Encoding.GetEncoding("GB2312").GetString(recData)).ToString();
                        System.IO.File.Delete(path);
                    }
                    string headUrl = "/api/Account/download_ipfile?md5=" + imgId;
                    //  MongoDBHelper.Instance.GetUserInfo().UpdateOne(filter, new UpdateDocument { { "$set", new QueryDocument { { "PicSrc", headUrl } } } });
                    string delcustAcc = "update huangguan_user set HeadIcon='{0}' where ID='{1}'".FormatStr(headUrl, id);
                    int row = MySqlDbHelper.ExecuteSql(connCommonsStr, delcustAcc);
                    r.Message = headUrl;
                    r.IsSuccess = true;
                }
                return r;
            }
            else
            {
                r.Message = "用户信息为空异常";
                return r;
            }

        }



        /// <summary>
        /// 根据md5上传文件
        /// </summary>
        /// <param name="md5">md5url</param>
        /// <returns></returns>
        [HttpPost]
        public bool upload_ipfile(string md5, string content_type)
        {
            try
            {
                Guid id = new Guid(md5);
                kuerhotelsEntities mm = new kuerhotelsEntities();
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                byte[] bs = new byte[file.ContentLength];
                file.InputStream.Read(bs, 0, bs.Length);

                //var f = mm.ipfiles.FirstOrDefault(x => x.Id == id);
                //if (f == null)
                //{
                ipfiles ff = new ipfiles();
                ff.bytes = bs;
                ff.content_type = content_type;
                ff.file_name = md5.ReplaceWith("-", "") + "." + content_type;
                ff.Id = id;
                ff.size = bs.Length;
                //mm.ipfiles.Add(ff);
                //mm.SaveChanges();
               
                string sql = "insert into ipfiles(Id,bytes,content_type,size,file_name) values('{0}',@bytes,N'{1}','{2}',N'{3}')".FormatStr(ff.Id, ff.content_type, ff.size, ff.file_name);
                var paraList = new List<MySqlParameter>
            {
                new MySqlParameter("@bytes", ff.bytes)             
            };
                MySqlDbHelper.ExecuteSql(connCommonsStr, sql, paraList);

                //string updatesql = "update ipfiles set bytes='{0}' where ID='{1}' ".FormatStr(ff.bytes, ff.Id);
                //MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
                //}
                //else
                //{
                //    f.bytes = bs;
                //    f.size = bs.Length;
                //    mm.Entry(f).State = System.Data.EntityState.Modified;
                //    mm.SaveChanges();
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpGet]
        public HttpResponseMessage download_ipfile(string md5)
        {

            var context = new kuerhotelsEntities();

            Guid id = new Guid(md5);
            var f = context.ipfiles.FirstOrDefault(x => x.Id == id);
            if (f != null && f.bytes.Length > 0)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                MemoryStream stream = new MemoryStream(f.bytes);
                response.Content = new StreamContent(stream);
                if (f.content_type == "png")
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                else
                {
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = f.file_name
                    };
                }

                return response;
            }
            return ControllerContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
        }




    }
}