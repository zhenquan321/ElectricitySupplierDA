using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 简报文字树模块
    /// </summary>
    public class Dnl_Report_WordTree
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 文字树内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 当前词根
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 简报文字树模块
    /// </summary>
    public class Dnl_Report_WordTreeDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 文字树内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 当前词根
        /// </summary>
        public string Keyword { get; set; }
        public bool IsHide { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
