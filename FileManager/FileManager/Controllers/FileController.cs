using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Web.Http;
using System.Net.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using FileManager.Helpers;
using FileManager.Models;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace FileManager.Controllers
{
    public class FileController:ApiController
    {
        /// <summary>
        /// 根据md5上传文件
        /// </summary>
        /// <param name="fileId">文件唯一标识</param>
        /// <param name="content_type">文件类型</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        [HttpPost]
        public bool UploadFile(string fileId, string fileType,string fileName)
        {
            try
            {
                //读取文件
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                byte[] bs = new byte[file.ContentLength];       //比特流
                file.InputStream.Read(bs, 0, bs.Length);        //获取文件流
                Dnl_File ff = new Dnl_File();
                ff.Bytes = bs;
                ff.FileType = fileType;
                ff.FileName = fileName;
                ff._id = new ObjectId(fileId);
                ff.Size = bs.Length;
                ff.CreateAt = DateTime.Now.AddHours(8);
                MongoDBHelper.Instance.GetDnl_File().InsertOne(ff);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据md5下载文件
        /// </summary>
        /// <param name="fileId">文件唯一标识</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownloadFile(string fileId)
        {
            var builder = Builders<Dnl_File>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(fileId));
            var file = MongoDBHelper.Instance.GetDnl_File().Find(filter).FirstOrDefault();

            if (file != null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                MemoryStream stream = new MemoryStream(file.Bytes);
                response.Content = new StreamContent(stream);
                if (file.FileType == "png")
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                else
                {
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = file.FileName
                    };
                }

                return response;
            }
            return ControllerContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="path">图片相对路径</param>
        /// <returns></returns>
        [HttpPost]
        public string ImgUpload()
        {
            var imgId = ObjectId.GenerateNewId();
            //读取文件
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            byte[] bs = new byte[file.ContentLength];       //比特流
            file.InputStream.Read(bs, 0, bs.Length);        //获取文件流
            string exten = Path.GetExtension(file.FileName);               //扩展名
            string name = Guid.NewGuid().ToString() + exten;      //图片名
            Dnl_File ff = new Dnl_File();
            ff.Bytes = bs;
            ff.FileType = "png";
            ff.FileName = name;
            ff._id = imgId;
            ff.Size = bs.Length;
            ff.CreateAt = DateTime.Now.AddHours(8);
            MongoDBHelper.Instance.GetDnl_File().InsertOne(ff);
            //返回图片链接
            string baseUrl = "http://211.154.6.166:9999";
            string imgUrl = baseUrl + "/api/File/DownloadFile?fileId=" + imgId;
            return imgUrl;
        }
    }
}