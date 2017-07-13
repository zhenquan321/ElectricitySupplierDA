using kuerjiudian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AISSystem;
using ProxyLib;
using System.Data;
using kuerjiudian.DAL;

namespace kuerjiudian.Controllers
{
    public class ShareController : ApiController
    {

        string connCommonsStr = AISSystem.AppSettingHelper.GetAppSetting("MAppEntitiesPOCO");

        /// <summary>
        /// 保存分享主题信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultInfo SaveShare(ShareDto dto)
        {
            ResultInfo r = new ResultInfo();
            if (dto.ID == Guid.Empty)
            {
                dto.CreatedAt = DateTime.Now;
                dto.ID = Guid.NewGuid();
                dto.IsDel = false;
                dto.IsReaded = false;
                if (string.IsNullOrEmpty(dto.Title) || dto.Sender == Guid.Empty)
                {
                    r.IsSuccess = false;
                    r.Message = "参数空异常";
                    return r;
                }
                if (!string.IsNullOrEmpty(dto.Description))
                {
                    dto.Abstract = dto.Description.Substring(0, System.Math.Min(dto.Description.Length, 1000)).GetTxtFromHtml2().Replace("&nbsp;", "");
                }
                string inserttab = @"insert into huangguan_share(ID,Title,Description,Sender,Label,CreatedAt,IsDel,IsReaded,Abstract) 
                                values('{0}',N'{1}',N'{2}','{3}',N'{4}','{5}',{6},{7},N'{8}')"
                    .FormatStr(dto.ID, dto.Title, dto.Description, dto.Sender, dto.Label, dto.CreatedAt, 0, 0, dto.Abstract);
                int rowcount = MySqlDbHelper.ExecuteSql(connCommonsStr, inserttab);
                r.IsSuccess = true;
                r.Message = dto.ID.ToString();
                return r;
            }
            else
            {
                string selsql = @"select ID from huangguan_share where ID='{0}'".FormatStr(dto.ID);
                DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
                if (dt.Rows.Count <= 0)
                {
                    r.IsSuccess = false;
                    r.Message = "分享主题信息为空异常";
                    return r;
                }
                if (!string.IsNullOrEmpty(dto.Description))
                {
                    dto.Abstract = dto.Description.Substring(0, System.Math.Min(dto.Description.Length, 1000)).GetTxtFromHtml2().Replace("&nbsp;", "");
                }
                string updatesql = @"update huangguan_share set Description=N'{0}',Label=N'{1}',Title=N'{2}',Abstract=N'{3}' where ID='{4}'".FormatStr(dto.Description, dto.Label, dto.Title, dto.Abstract, dto.ID);
                int cccc = MySqlDbHelper.ExecuteSql(connCommonsStr, updatesql);
                r.IsSuccess = true;
                r.Message = dto.ID.ToString();
                return r;
            }
        }












        /// <summary>
        /// 获取分享主题信息
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="titlelabel"></param>
        /// <param name="type">3：所有；1:我分享的，2：分享给我的</param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<ShareDto> GetShare(Guid? usrId, string titlelabel, int? type, int page, int pagesize)
        {
            QueryResult<ShareDto> r = new QueryResult<ShareDto>();
            kuerhotelsEntities m = new kuerhotelsEntities();
            var q = from s in m.huangguan_share
                    where s.IsDel == false
                    orderby s.CreatedAt descending
                    select new ShareDto
                    {
                        CreatedAt = s.CreatedAt,
                        Abstract = s.Abstract,
                        //Description = s.Description,
                        ID = s.ID,
                        Label = s.Label,
                        Sender = s.Sender,
                        Title = s.Title,
                    };

            Dictionary<Guid?, bool?> shareIdDatas = new Dictionary<Guid?, bool?>();
            List<Guid?> shareIds = new List<Guid?>();
            if (type == 2)
            {

                string sql1 = @"select s.Id, s.Title, s.Sender, s.Label, s.CreatedAt, s.IsDel, su.IsReaded, s.Abstract,su.ShareId from huangguan_share s join huangguan_shareusers su on s.Id = su.ShareId
                               where 1=1 and (s.IsDel = 0 and su.SharedUser = {0}) order by s.CreatedAt desc".FormatStr(usrId);
                DataTable dt1 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql1);
                List<ShareDto> list1 = new List<ShareDto>();
                foreach (DataRow item in dt1.Rows)
                {
                    ShareDto sd = new ShareDto();
                    sd.CreatedAt = Convert.ToDateTime(item["CreatedAt"].ToString());
                    sd.Abstract = item["Abstract"].ToString();
                    sd.ID = Guid.Parse(item["Id"].ToString());
                    sd.Label = item["Label"].ToString();
                    sd.Sender = Guid.Parse(item["Sender"].ToString());
                    sd.Title = item["Title"].ToString();
                    sd.IsReaded = bool.Parse(item["IsReaded"].ToString());
                    sd.ShareId = Guid.Parse(item["ShareId"].ToString());
                    list1.Add(sd);


                }
                q = list1.AsQueryable();


            }
            else if (type == 3)
            {

                string selsql = @"select s.Id, s.Title, s.Sender, s.Label, s.CreatedAt, s.IsDel,  s.Abstract from huangguan_share s 
                                where 1=1 and s.IsDel = 0 order by s.CreatedAt desc".FormatStr(usrId, usrId);

                DataTable dt =MySqlDbHelper.ExecuteQuery(connCommonsStr, selsql);
                List<ShareDto> list2 = new List<ShareDto>();
                foreach (DataRow item in dt.Rows)
                {
                    ShareDto sd = new ShareDto();
                    sd.CreatedAt = Convert.ToDateTime(item["CreatedAt"].ToString());
                    //Description = s.Description,
                    sd.Abstract = item["Abstract"].ToString();
                    sd.ID = Guid.Parse(item["Id"].ToString());
                    sd.Label = item["Label"].ToString();
                    sd.Sender = Guid.Parse(item["Sender"].ToString());
                    sd.Title = item["Title"].ToString();
                   
                    list2.Add(sd);
                }

                q = list2.AsQueryable();

            }
            else if (type == 1)
            {
                if (usrId != Guid.Empty)
                {
                    q = q.Where(x => x.Sender == usrId);
                    r.Count = q.ToList().Count;
                    r.Result = q.ToList();
                    return r;
                }
            }
            else if (!type.HasValue)
            {
                shareIdDatas = m.huangguan_shareusers.Where(x => x.SharedUser == usrId).ToDictionary(x => x.ShareId, y => y.IsReaded);
                shareIds = shareIdDatas.Keys.ToList();
                if (shareIds.Count > 0)
                {
                    q = q.Where(x => shareIds.Contains(x.ID) || x.Sender == usrId);
                }
            }
            if (!string.IsNullOrEmpty(titlelabel))
            {
                q = q.Where(x => x.Title.Contains(titlelabel) || x.Label.Contains(titlelabel));
            }
            if (q.ToList().Count > 0)
            {
                var List = q.ToList();
                foreach (var item in List)
                {
                    if (item.Sender != Guid.Empty)
                    {
                        string touxing = "select LoginName,NickName,HeadIcon,Gender from huangguan_user where ID='{0}'".FormatStr(item.Sender);
                        var dataicon = MySqlDbHelper.ExecuteQuery(connCommonsStr, touxing);
                        if (dataicon.Rows.Count <= 0)
                        {
                            return r;
                        }


                        item.SenderName = string.IsNullOrEmpty(dataicon.Rows[0]["NickName"].ToString()) ? "无昵称" : dataicon.Rows[0]["NickName"].ToString();
                        item.SenderHead = dataicon.Rows[0]["HeadIcon"].ToString();
                        item.SenderGender = dataicon.Rows[0]["Gender"].ToString();

                        string RCount = "select * from huangguan_sharereply where IsDel = 0 and ShareId = '{0}'".FormatStr(item.ID);
                        string LCount = "select * from huangguan_sharereply where  ` Love` = 1 and  ShareId = '{0}'".FormatStr(item.ID);
                        item.ReplyerCount = MySqlDbHelper.ExecuteQuery(connCommonsStr, RCount).Rows.Count;
                        item.LoveCount = MySqlDbHelper.ExecuteQuery(connCommonsStr, LCount).Rows.Count;
                    }


                }
                r.Count = List.Count();
                r.Result = List.Skip(page * pagesize).Take(pagesize).ToList();

                return r;
            }
            else
            {
                r.Count = 0;
                r.Result = q.ToList();
                return r;
            }

        }



        /// <summary>
        /// 获取分享主题信息详情
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="titlelabel"></param>
        /// <param name="type">1:我分享的，2：分享给我的</param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<ShareDto> GetShareDetails(Guid id)
        {
            QueryResult<ShareDto> r = new QueryResult<ShareDto>();

            kuerhotelsEntities m = new kuerhotelsEntities();
            if (id != null)
            {
                var s = m.huangguan_share.Where(x => x.ID == id && x.IsDel == false).Select(x => new ShareDto
                {
                    CreatedAt = x.CreatedAt,
                    //Abstract = x.Abstract,
                    Description = x.Description,
                    ID = x.ID,
                    Label = x.Label,
                    Sender = x.Sender,
                    Title = x.Title,

                });
                r.Count = s.Count();
                r.Result = s.OrderByDescending(x=>x.CreatedAt).ToList();
                var usrId = r.Result.Select(x => x.Sender).Distinct().ToList();
                if (usrId.Count > 0)
                {
                    foreach (var item in r.Result)
                    {
                        if (item.Sender != Guid.Empty)
                        {
                            string touxing = "select NickName,HeadIcon,Gender from huangguan_user where ID='{0}'".FormatStr(item.Sender);
                            var dataicon = MySqlDbHelper.ExecuteQuery(connCommonsStr, touxing);
                            item.SenderName = string.IsNullOrEmpty(dataicon.Rows[0]["NickName"].ToString()) ? "无昵称" : dataicon.Rows[0]["NickName"].ToString();
                            item.SenderHead = dataicon.Rows[0]["HeadIcon"].ToString();
                            item.SenderGender = dataicon.Rows[0]["Gender"].ToString();
                        }
                    }
                }
                return r;
            }
            else
            {
                return null;

            }


        }



        /// <summary>
        /// 删除分享主题信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultInfo DelShare(Guid? id, bool isDel)
        {
            ResultInfo r = new ResultInfo();

            kuerhotelsEntities m = new kuerhotelsEntities();
            if (id.HasValue && id != Guid.Empty)
            {
                var s = m.huangguan_share.FirstOrDefault(x => x.ID == id);
                if (s == null)
                {
                    r.Message = "分享主题信息为空异常";
                    return r;
                }

                s.IsDel = isDel;
                m.SaveChanges();
            }
            else
            {
                return r;
            }

            r.IsSuccess = true;
            return r;
        }
        /// <summary>
        /// 获取分享的评论
        /// </summary>
        /// <param name="shareId"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public QueryResult<ShareReplyDto> GetShareReply(Guid? shareId, int page, int pagesize)
        {
            QueryResult<ShareReplyDto> r = new QueryResult<ShareReplyDto>();

            kuerhotelsEntities m = new kuerhotelsEntities();

            if (!shareId.HasValue || shareId == Guid.Empty)
            {
                return r;
            }
            var q = m.huangguan_sharereply.Where(x => x.ShareId == shareId && x.IsDel == false).Select(x => new ShareReplyDto
            {
                CreatedAt = x.CreatedAt,
                Description = x.Description,
                ID = x.ID,
                Replyer = x.Replyer,
                ShareId = x.ShareId
            });
            r.Count = q.Count();
            r.Result = q.OrderByDescending(x => x.CreatedAt).Skip(page * pagesize).Take(pagesize).ToList();
            var usrIds = r.Result.Select(x => x.Replyer).Distinct().ToList();
            if (usrIds.Count > 0)
            {
                foreach (var data in r.Result)
                {
                    if (data.Replyer != Guid.Empty && data.Replyer !=null)
                    {
                        string touxing = "select NickName,HeadIcon,Gender from huangguan_user where Id='{0}'".FormatStr(data.Replyer);
                        var dataicon = MySqlDbHelper.ExecuteQuery(connCommonsStr, touxing);
                        data.ReplyerName = string.IsNullOrEmpty(dataicon.Rows[0]["NickName"].ToString()) ? "无昵称" : dataicon.Rows[0]["NickName"].ToString();
                        data.ReplyerHead = dataicon.Rows[0]["HeadIcon"].ToString();
                        data.ReplyerGender = dataicon.Rows[0]["Gender"].ToString();
                    }
                }

            }

            return r;
        }

        /// <summary>
        /// 要求用户参与分享讨论
        /// </summary>
        /// <param name="shareId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultInfo ShareToUsers(Guid shareId, string userIds)
        {
            ResultInfo r = new ResultInfo();

            kuerhotelsEntities m = new kuerhotelsEntities();
            if (shareId == Guid.Empty || string.IsNullOrEmpty(userIds))
            {
                r.Message = "参数空异常";
                return r;
            }
            var userIdList = userIds.Split(';');

            foreach (var usrid in userIdList)
            {

                Guid dfdf = Guid.Parse(usrid);

                var s = m.huangguan_shareusers.FirstOrDefault(x => x.ShareId == shareId && x.SharedUser == dfdf);
                if (s == null)
                {
                    s = new  huangguan_shareusers();
                    s.CreatedAt = DateTime.Now;
                    s.ID = Guid.NewGuid();
                    s.SharedUser = dfdf;
                    s.ShareId = shareId;
                    m.huangguan_shareusers.Add(s);
                }


            }
            m.SaveChanges();
            r.IsSuccess = true;
            return r;
        }
        /// <summary>
        /// 保存分享的评论
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultInfo SaveShareReply(huangguan_sharereply dto)
        {
            ResultInfo r = new ResultInfo();

            kuerhotelsEntities m = new kuerhotelsEntities();
            if (dto.ID == Guid.Empty)
            {
                dto.CreatedAt = DateTime.Now;
                dto.ID = Guid.NewGuid();
                dto.IsDel = false;
                if (string.IsNullOrEmpty(dto.Description) || dto.Replyer ==Guid.Empty || dto.ShareId == Guid.Empty)
                {
                    r.Message = "参数空异常";
                    return r;
                }
                m.huangguan_sharereply.Add(dto);
            }
            else
            {
                var s = m.huangguan_sharereply.FirstOrDefault(x => x.ID == dto.ID);
                if (s == null)
                {
                    r.Message = "分享评论为空异常";
                    return r;
                }
                s.Description = dto.Description;
                s.IsDel = dto.IsDel;
                s.Replyer = dto.Replyer;
                s.ShareId = dto.ShareId;
                s.C_Love = 0;
            }

            m.SaveChanges();
            r.IsSuccess = true;
            return r;
        }


        /// <summary>
        /// 保存点赞
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultInfo LoveShareReply(huangguan_sharereply dto)
        {
            ResultInfo r = new ResultInfo();
            kuerhotelsEntities m = new kuerhotelsEntities();
            var q = m.huangguan_sharereply.Where(x => x.ShareId == dto.ShareId && x.Replyer == dto.Replyer && x.C_Love != null).Select(x => new ShareReplyDto
            {
                ID = x.ID,
            });
            if (dto.ID == Guid.Empty && q.ToList().Count == 0)
            {
                dto.CreatedAt = DateTime.Now;
                dto.ID = Guid.NewGuid();
                if (dto.C_Love == null || dto.Replyer ==Guid.Empty || dto.ShareId == Guid.Empty)
                {
                    r.Message = "参数空异常";
                    return r;
                }
                m.huangguan_sharereply.Add(dto);
            }
            else
            {
                var s = m.huangguan_sharereply.FirstOrDefault(x => x.ShareId == dto.ShareId && x.Replyer == dto.Replyer && x.C_Love != null);
                if (s == null)
                {
                    r.Message = "分享评论为空异常";
                    return r;
                }
                s.C_Love = dto.C_Love;
            }
            m.SaveChanges();
            r.IsSuccess = true;
            return r;


        }


        /// <summary>
        /// 获取点赞的状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        public CreatedAtCount LoveState(Guid shareId, Guid Replyer)
        {
            string sql1 = "SELECT ` Love` FROM huangguan_sharereply WHERE ` Love` IS NOT NULL and ShareId = '{0}' and Replyer ='{1}'".FormatStr(shareId, Replyer);
            DataTable dt1 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql1);
            CreatedAtCount cr = new CreatedAtCount();
            if (dt1.Rows.Count > 0)
            {
                cr.Love = Convert.ToInt32(dt1.Rows[0][" Love"]);
                return cr;
            }
            else
            {
                cr.Love = 0;
                return cr;
            }
        }


        /// <summary>
        /// 设置邀请用户信息已读
        /// </summary>
        /// <param name="id">shareId</param>
        /// <param name="usrId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultInfo SetShareUserRead(Guid? id, Guid usrId)
        {
            ResultInfo r = new ResultInfo();

            kuerhotelsEntities m = new kuerhotelsEntities();
            if (id.HasValue && id != Guid.Empty)
            {
                var s = m.huangguan_shareusers.FirstOrDefault(x => x.ShareId == id && x.SharedUser == usrId);
                if (s == null)
                {
                    r.Message = "邀请用户信息为空异常";
                    return r;
                }

                s.IsReaded = true;
                m.SaveChanges();
            }
            else
            {
                return r;
            }

            r.IsSuccess = true;
            return r;
        }



        //获取所有用户的 id、昵称、登陆用户名
        [HttpGet]
        public List<IDNameDto> GetAllIdName()
        {
            string sql = "select ID,LoginName,NickName from huangguan_user where status=0";
            DataTable dt = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql);
            List<IDNameDto> list = new List<IDNameDto>();
            foreach (DataRow item in dt.Rows)
            {
                IDNameDto dto = new IDNameDto();
                dto.ID = Guid.Parse(item["ID"].ToString());
                dto.LoginName = item["LoginName"].ToString();
                dto.Name = item["NickName"].ToString();
               
                list.Add(dto);
            }


            return list;
        }



       
        //新增数、总数
        [HttpGet]
        public CreatedAtCount GetCreatedAtCount(Guid usrId)
        {
            DateTime ddddddd = DateTime.Now;
            var dfdf = ddddddd.Year + "-" + ddddddd.Month + "-" + "1";
            var ggggg = DateTime.Parse(dfdf);
            string sql1 = "SELECT COUNT(*) num FROM huangguan_share where IsDel =0";
            string sql2 = "SELECT COUNT(CreatedAt) num FROM huangguan_share WHERE CreatedAt >='{0}' and IsDel = 0".FormatStr(ggggg);
            DataTable dt1 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql1);
            DataTable dt2 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql2);
            CreatedAtCount cr = new CreatedAtCount();
            cr.count = Convert.ToInt32(dt1.Rows[0]["num"]);
            cr.yesterdayCount = Convert.ToInt32(dt2.Rows[0]["num"]);
            return cr;
        }


        //获取点赞数、评论数
        [HttpGet]
        public CreatedAtCount GetLoveCount(Guid? shareId)
        {
            string sql1 = "  SELECT COUNT(Id) num FROM huangguan_sharereply WHERE  IsDel = 0 and ShareId = '{0}'".FormatStr(shareId);
            string sql2 = "  SELECT COUNT(` Love`) num FROM huangguan_sharereply WHERE ` Love` = 1  and  ShareId = '{0}'".FormatStr(shareId);
            DataTable dt1 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql1);
            DataTable dt2 = MySqlDbHelper.ExecuteQuery(connCommonsStr, sql2);
            CreatedAtCount cr = new CreatedAtCount();
            cr.ReplyerCount = Convert.ToInt32(dt1.Rows[0]["num"]);
            cr.loveCount = Convert.ToInt32(dt2.Rows[0]["num"]);
            return cr;
        }




    }
}