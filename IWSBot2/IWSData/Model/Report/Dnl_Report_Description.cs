using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 报告描述
    /// </summary>
    public class Dnl_Report_Description
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 判断是开头描述还是末尾总结
        /// </summary>
        public bool IsStart { get; set; }
    }

    /// <summary>
    /// 报告描述
    /// </summary>
    public class Dnl_Report_DescriptionDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isHide { get; set; }
    }

    /// <summary>
    /// 报告描述因素
    /// </summary>
    public class DescriptionFactor
    {
        public string id { get; set; }
        public string reportId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 判断是开头描述还是末尾总结
        /// </summary>
        public bool isStart { get; set; }
    }
}
