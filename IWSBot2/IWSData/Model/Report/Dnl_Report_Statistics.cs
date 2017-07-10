using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 简报采集统计
    /// </summary>
    public class Dnl_Report_Statistics
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        /// <summary>
        /// 简报Id
        /// </summary>
        public ObjectId ReportId { get; set; }
        public string Description { get; set; }
        public int KeywordCount { get; set; }
        public int LinkCount { get; set; }
        /// <summary>
        /// 已搜索信源数
        /// </summary>
        public int SourceCount { get; set; }
        /// <summary>
        /// 监测开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 监测结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }

    /// <summary>
    /// 简报采集统计
    /// </summary>
    public class Dnl_Report_StatisticsDto
    {
        public String _id { get; set; }
        public String UsrId { get; set; }
        /// <summary>
        /// 简报Id
        /// </summary>
        public String ReportId { get; set; }
        public string Description { get; set; }
        public int KeywordCount { get; set; }
        public int LinkCount { get; set; }
        /// <summary>
        /// 已搜索信源数
        /// </summary>
        public int SourceCount { get; set; }
        /// <summary>
        /// 监测开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 监测结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }

    /// <summary>
    /// 链接发布时间
    /// </summary>
    public class LinkPublishTime
    {
        public string Id { get; set; }
        public DateTime PublishTime { get; set; }
    }
}
