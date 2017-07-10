using AISSystem;
using IPRWorx.XiuXiuPost;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace IPRWorx
{
    /// <summary>
    /// Summary description for post
    /// </summary>
    public class post : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string name = null;
            if (context.Request.Files.Count > 0)
            {
                //config 配置节点可以将图片保存至指定目录，未配置将保存至 /XiuXiuUpload
                //<appSettings>
                //  <add key="XiuXiuImageSavePath" value="/upload"/>
                //</appSettings>
                XiuXiuPostImage img = new XiuXiuPostImage(context);
                name = img.Save();
            }
            else
            {
                name = "非法访问";
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(name);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    namespace XiuXiuPost
    {

        /// <summary>
        /// 上传抽象类
        /// </summary>
        public abstract class XiuXiuImage
        {
            /// <summary>
            /// 基类保存
            /// </summary>
            /// <returns>返回保存路径+文件名</returns>
            public virtual string Save()
            {
                string fileName = this.GetFileName();
                if (null == fileName) return null;

                string root = HttpContext.Current.Server.MapPath(path);

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                this.FileName = Path.Combine(root, fileName);
                string[] paths = { path, fileName };
                return string.Join("/", paths);
            }

            public XiuXiuImage()
            {
                path = path == null ? "/UploadFile/XiuXiuUpload" : path;
            }

            /// <summary>
            /// 确定上传类型
            /// </summary>
            protected bool IsUplodType
            {
                get
                {
                    string extension = this.GetExtension();
                    return ".jpg .gif .png .icon .bmp .tiff .wmf .emf .exif".IndexOf(extension) >= 0 ? true : false;
                }
            }

            private string _fileName = null;
            /// <summary>
            /// 最终保存路径
            /// </summary>
            protected string FileName
            {
                set { _fileName = value; }
                get { return _fileName; }
            }

            /// <summary>
            /// 配置文件路径 无配置上传到XiuXiuUpload
            /// </summary>
            protected string path = ConfigurationManager.AppSettings["XiuXiuImageSavePath"];

            /// <summary>
            /// 获取拓展名
            /// </summary>
            /// <returns></returns>
            protected abstract string GetExtension();

            /// <summary>
            /// 获取最终保存文件名
            /// </summary>
            /// <returns></returns>
            protected string GetFileName()
            {
                string extension = this.GetExtension();
                if (null == extension) return null;
                else
                {
                    string name = this.GetName();
                    string[] imgName = { "XiuXiu", name, extension };
                    return string.Join("", imgName);
                }
            }
            /// <summary>
            /// 获取保存文件名
            /// </summary>
            /// <returns></returns>
            private string GetName()
            {
                DateTime uploadTime = DateTime.Now;
                string[] times = { uploadTime.Year.ToString(), uploadTime.Month.ToString(), uploadTime.Day.ToString(),
                                 uploadTime.Hour.ToString(), uploadTime.Millisecond.ToString(), uploadTime.Second.ToString() };
                return string.Join("", times);
            }
        }
        /// <summary>
        /// POST接收
        /// </summary>
        public sealed class XiuXiuPostImage : XiuXiuImage
        {
            private HttpFileCollection xiuxiuFiles = null;

            public XiuXiuPostImage(HttpContext context)
            {
                this.xiuxiuFiles = context.Request.Files;
            }
            /// <summary>
            /// 上传文件个数
            /// </summary>
            public int Count
            {
                get { return xiuxiuFiles.Count; }
            }
            /// <summary>
            /// 保存图片,成功返回文件路径,失败null
            /// 非图片格式返回错误信息
            /// </summary>
            /// <returns></returns>
            public override string Save()
            {
                if (!this.IsUplodType)
                {
                    return "Only allowed to upload pictures.";
                }
                string returnName = base.Save();
                if (this.FileName != null)
                {
                    this.File.SaveAs(this.FileName);

                    return returnName;
                }
                return null;
            }

            private HttpPostedFile File
            {
                get { return this.Count >= 1 ? xiuxiuFiles[0] : null; }
            }

            protected override string GetExtension()
            {
                return null == this.File ? null : Path.GetExtension(this.File.FileName);
            }


        }
    }
}