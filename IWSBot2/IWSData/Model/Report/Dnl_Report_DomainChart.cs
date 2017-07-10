using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 域名分组统计图
    /// </summary>
    public class Dnl_Report_DomainChart
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public ObjectId CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 使用关键词组Id
        /// </summary>
        public string keyCateIds { get; set; }
        /// <summary>
        /// 域名分组信息
        /// </summary>
        public string DomainCategory { get; set; }
        /// <summary>
        /// 气泡图数据
        /// </summary>
        public string Chart_DomainCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
        public bool IsDel { get; set; }
    }
}
