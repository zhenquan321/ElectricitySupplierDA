using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace IWSData.Model
{
    /// <summary>
    /// 图表存储
    /// </summary>
    public class PojectChartMongo
    {
        /// <summary>
        /// MongoDB唯一标识Id
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        public ObjectId ProjectId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 图片设置名，对只有一种设置的图表该值为空
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图表参数的Json字符串
        /// </summary>
        public string FactorJson { get; set; }
        /// <summary>
        /// 图表数据的Json字符串
        /// </summary>
        public string DataJson { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 图表类型
        /// </summary>
        public ChartType Type { get; set; }
        /// <summary>
        /// 信源类型
        /// </summary>
        public SourceType Source { get; set; }
    }
}
