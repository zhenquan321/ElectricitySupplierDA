using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 网页关系图描述信息
    /// </summary>
    public class ReferChartDescMongo
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>
        public ObjectId ProjectId { get; set; }
        /// <summary>
        /// 信源类型
        /// </summary>
        public SourceType Type { get; set; }
        /// <summary>
        /// 描述信息列表
        /// </summary>
        public List<string> DescList { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
    }

    /// <summary>
    /// 网页关系图描述信息
    /// </summary>
    public class ReferChartDescDto
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 描述信息列表
        /// </summary>
        public List<string> DescList { get; set; }
    }
}
