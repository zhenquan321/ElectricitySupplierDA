using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Models
{
    /// <summary>
    /// 文件类，用于保存图片
    /// </summary>
    public class Dnl_File
    {

        public ObjectId _id { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        public byte[] Bytes { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public Nullable<int> Size { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public System.DateTime CreateAt { get; set; }
    }
}