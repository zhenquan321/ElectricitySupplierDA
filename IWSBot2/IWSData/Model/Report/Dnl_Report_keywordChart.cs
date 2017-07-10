using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 关键词图表类
    /// </summary>
    public class Dnl_Report_keywordChart
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 矩形树图
        /// </summary>
        public string Chart_RectangularTree { get; set; }
        /// <summary>
        /// 圆形d3图
        /// </summary>
        public string Chart_CategoryTree { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }

    /// <summary>
    /// 关键词图表类
    /// </summary>
    public class Dnl_Report_keywordChartDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 矩形树图
        /// </summary>
        public string Chart_RectangularTree { get; set; }
        /// <summary>
        /// 圆形d3图
        /// </summary>
        public string Chart_CategoryTree { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
