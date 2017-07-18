using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IW2S.Models;
using IWSData.Model;
using System.IO;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using AISSystem;
using System.Text;
using IW2S.Helpers;

namespace IW2S.Controllers
{
    public class FileController : ApiController
    {

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="path">图片相对路径</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto ImgUpload(string path)
        {
             var result = new ResultDto();
            var imgId = ObjectId.GenerateNewId();       //唯一图片ID
            //var img = Request.Files[0];                //图片信息
            //string path = Server.MapPath("../UploadFile/"); //当前路径 
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/") + path; //当前路径 
            //string name = img.FileName;                         //图片名
            string exten = Path.GetExtension(path);               //扩展名
            string name = Guid.NewGuid().ToString() + exten;      //图片名
            //path += Guid.NewGuid().ToString() + exten;          //保存路径
            //保存图片
            //Request.Files[0].SaveAs(path);
            //调用保存图片
            string baseUrl = "http://43.240.138.233:9999";
            string upload_file_url = baseUrl + "/api/File/UploadFile?fileId=" + imgId + "&fileType=png&fileName=" + name;
            WebClient wc = new WebClient();
            byte[] sendData = System.Text.Encoding.UTF8.GetBytes(filePath);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("ContentLength", sendData.Length.ToString());
            byte[] recData = wc.UploadFile(upload_file_url, "POST", filePath);
            var success = (Encoding.GetEncoding("GB2312").GetString(recData)).ToBool();
            //删除原有图片
            System.IO.File.Delete(filePath);
            if (success.HasValue && success.Value)
            {
                //返回图片链接
                string imgUrl = baseUrl + "/api/File/DownloadFile?fileId=" + imgId;
                result.IsSuccess = true;
                result.Message = imgUrl;
            }
            return result;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="path">图片相对路径</param>
        /// <returns></returns>
        [HttpPost]
        public string ImgUpload2()
        {
            var imgId = ObjectId.GenerateNewId();
            //读取文件
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            byte[] bs = new byte[file.ContentLength];       //比特流
            file.InputStream.Read(bs, 0, bs.Length);        //获取文件流
            Dnl_File ff = new Dnl_File();
            ff.Bytes = bs;
            ff.FileType = "png";
            ff.FileName = file.FileName;
            ff._id = imgId;
            ff.Size = bs.Length;
            ff.CreateAt = DateTime.Now.AddHours(8);
            MongoDBHelper.Instance.GetDnl_File().InsertOne(ff);
            //返回图片链接
            string baseUrl = "http://43.240.138.233:9999";
            string imgUrl = baseUrl + "/api/File/DownloadFile?fileId=" + imgId;
            return imgUrl;
        }
    }
}