using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 链接图表分组类
    /// </summary>
    public class Dnl_Report_LinkChartCategory
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public int ChartCount { get; set; }
        /// <summary>
        /// 列表中该类所处位置
        /// </summary>
        public int Index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }

    
}
