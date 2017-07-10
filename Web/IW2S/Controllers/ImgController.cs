using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace IW2S.Controllers
{
    public class ImgController : ApiController
    {
        [HttpPost]
        public ResultDto InsertImgSearchTask(IW2S_ImgSearchTaskDto data)
        {
            ResultDto result = new ResultDto();

            var builder = Builders<IW2S_ImgSearchTask>.Filter;
            var col = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks();

            var usrObjId = MongoDBHelper.Instance.GetIW2S_Projects().Find(Builders<IW2S_Project>.Filter.Eq(x => x._id, new ObjectId(data.ProjectId))).Project(x => x.UsrId).FirstOrDefault();

            var filter =  builder.Eq(x => x.ProjectId, new ObjectId(data.ProjectId)) & builder.Eq(x => x.Src, data.Src) & builder.Eq(x => x.IsDel, false);
            var task = col.Find(filter).FirstOrDefault();
            if(task != null)
            {
                result.Message = "已经上传成功了";
                return result;
            }
            IW2S_ImgSearchTask kw = new IW2S_ImgSearchTask
            {
                _id = ObjectId.GenerateNewId(),
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(data.ProjectId),
                UsrId = usrObjId,
                BotStatus = 0,
                Src = data.Src,
                IsDel = false
            };
            
            col.InsertOne(kw);

            IW2S_OperateLog log = new IW2S_OperateLog
            {
                CreatedAt = DateTime.Now.AddHours(8),
                ProjectId = new ObjectId(data.ProjectId),
                ShareOperateType = (int)ShareOperateType.AddKeyword,
                UserId = new ObjectId(data.UsrId),
                SiteSource = (int)SiteSource.BaiduImg
            };
            MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);

            result.IsSuccess = true;
            return result;
        }

        [HttpGet]
        public QueryResult<IW2S_ImgSearchTaskDto> GetImgSearchTasks(string usr_id,string prjId, int page, int pagesize)
        {
            QueryResult<IW2S_ImgSearchTaskDto> result = new QueryResult<IW2S_ImgSearchTaskDto>();
            if (string.IsNullOrEmpty(prjId))
            {
                return result;
            }
            var builder = Builders<IW2S_ImgSearchTask>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(prjId));
            filter &= builder.Eq(x => x.IsDel, false);
            var query = MongoDBHelper.Instance.GetIW2S_ImgSearchTasks().Find(filter);
            var totalCount = query.Count();
            var TaskList = query.SortByDescending(x => x.CreatedAt).Skip(page * pagesize).Limit(pagesize).ToList();
            List<IW2S_ImgSearchTaskDto> data = new List<IW2S_ImgSearchTaskDto>();
            foreach (var item in TaskList)
            {
                IW2S_ImgSearchTaskDto v = new IW2S_ImgSearchTaskDto();
                v._id = item._id.ToString();
                v.Src = item.Src;
                
                v.CreatedAt = item.CreatedAt.AddHours(8);
                v.BotStatus = item.BotStatus;
                data.Add(v);
            }
            result.Result = data;
            result.Count = totalCount;
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filterIds">以;隔开</param>
        /// <returns></returns>
        [HttpGet]
        public string DelImgSearchTask(string ids)
        {
            var filterlist = ids.Split(';', '；');
            List<ObjectId> obIds = new List<ObjectId>();

            var builder = Builders<IW2S_ImgSearchTask>.Filter;
            var filter = builder.In(x => x._id, obIds);
            foreach (var filterId in filterlist)
            {
                if (!string.IsNullOrEmpty(filterId))
                {
                    obIds.Add(new ObjectId(filterId));
                }
            }
            var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true } } } };

            MongoDBHelper.Instance.GetIW2S_ImgSearchTasks().UpdateMany(filter, update);
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
        public QueryResult<IW2S_ImgSearchLinkDto> GetImgSearchLinks(string user_id, string projectId, string searchTaskId, byte? status, int page, int pagesize)
        {

            var builder = Builders<IW2S_ImgSearchLink>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId));

            if (!string.IsNullOrEmpty(searchTaskId))
            {
                filter &= builder.Eq(x => x.IW2S_ImgSearchTaskId, new ObjectId(searchTaskId));
            }
            if (status.HasValue)
            {
                filter &= builder.Eq(x => x.DataCleanStatus, status.Value);
            }
            else
            {
                filter &= !builder.Eq(x => x.DataCleanStatus, (byte)2);
            }

            var col = MongoDBHelper.Instance.GetIW2S_ImgSearchLinks();

            var query = col.Find(filter).Project(x => new IW2S_ImgSearchLinkDto
            {
                _id = x._id.ToString(),
                Title = x.Title,
                Description = x.Description,
                Src = x.Src,
                
                Domain = x.Domain,
                LinkUrl = x.LinkUrl,
                DataCleanStatus = x.DataCleanStatus,
                CreatedAt = x.CreatedAt,
                
                PublishTime = x.PublishTime
            });

            var count = query.Count();
            var TaskList = query.Skip((page) * pagesize).Limit(pagesize).ToList();


            return new QueryResult<IW2S_ImgSearchLinkDto> { Count = count, Result = TaskList };
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

            var builder = Builders<IW2S_ImgSearchLink>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(id));

            var col = MongoDBHelper.Instance.GetIW2S_ImgSearchLinks();
            IW2S_ImgSearchLink linkUrlPrj = col.Find(filter).Project(x => new IW2S_ImgSearchLink
            {
                LinkUrl = x.LinkUrl,
                ProjectId = x.ProjectId
            }).FirstOrDefault();
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
                    UserId = new ObjectId(user_id),
                    SiteSource = (int)SiteSource.BaiduImg
                };
                MongoDBHelper.Instance.GetIW2S_OperateLogs().InsertOne(log);
            }

            result.IsSuccess = true;
            return result;
        }
        [HttpGet]
        public string ExportImgSearchLinks(string user_id, string projectId)
        {

            var builder = Builders<IW2S_ImgSearchLink>.Filter;
            var filter = builder.Eq(x => x.ProjectId, new ObjectId(projectId)) & !builder.Ne(x => x.DataCleanStatus, (byte)2);
            var col = MongoDBHelper.Instance.GetIW2S_ImgSearchLinks();
            int page = 0, pageSize = 100;
            List<IW2S_ImgSearchLink> keywords = new List<IW2S_ImgSearchLink>();
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
            
            RowHead.CreateCell(1).SetCellValue("BizId");
            RowHead.CreateCell(2).SetCellValue("CreatedAt");
            RowHead.CreateCell(3).SetCellValue("DataCleanStatus");
            RowHead.CreateCell(4).SetCellValue("Description");
            RowHead.CreateCell(5).SetCellValue("Domain");

            RowHead.CreateCell(6).SetCellValue("Src");
            RowHead.CreateCell(7).SetCellValue("LinkUrl");
            RowHead.CreateCell(8).SetCellValue("PublishTime");

            RowHead.CreateCell(9).SetCellValue("IW2S_ImgSearchTaskId");
            RowHead.CreateCell(10).SetCellValue("Title");
            RowHead.CreateCell(11).SetCellValue("TopDomain");

            int count = 0;
            foreach (var keyword in keywords)
            {
                IRow row = sheet.CreateRow(count + 1);
                row.CreateCell(0).SetCellValue(keyword._id.ToString());
                
                row.CreateCell(1).SetCellValue(keyword.BizId.ToString());
                row.CreateCell(2).SetCellValue(keyword.CreatedAt);
                row.CreateCell(3).SetCellValue(keyword.DataCleanStatus ?? 0);
                row.CreateCell(4).SetCellValue(keyword.Description);
                row.CreateCell(5).SetCellValue(keyword.Domain);
                
                row.CreateCell(6).SetCellValue(keyword.Src);
                row.CreateCell(7).SetCellValue(keyword.LinkUrl);
                row.CreateCell(8).SetCellValue(keyword.PublishTime);
                
                row.CreateCell(9).SetCellValue(keyword.IW2S_ImgSearchTaskId.ToString());
                row.CreateCell(10).SetCellValue(keyword.Title);
                row.CreateCell(11).SetCellValue(keyword.TopDomain);
                count = count + 1;
            }




            string baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
            string filename = "图片监测结果" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".xls";
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
    }
}
