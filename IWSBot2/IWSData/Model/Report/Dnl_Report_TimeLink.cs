using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 简报链接详情类
    /// </summary>
    public class Dnl_Report_TimeLink
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        /// <summary>
        /// 链接归属词组Id，多个用分号相连
        /// </summary>
        public string KeyCateId { get; set; }

        public string LinkUrl { get; set; }

        public string Title { get; set; }

        public string Domain { get; set; }
        /// <summary>
        /// 搜索关键词，多个用分号相连
        /// </summary>
        public string Keywords { get; set; }

        public DateTime PublishTime { get; set; }
    }

    /// <summary>
    /// 简报链接详情类
    /// </summary>
    public class Dnl_Report_TimeLinkDto
    {
        public string LinkUrl { get; set; }

        public string Title { get; set; }

        public string Domain { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }

        public DateTime PublishTime { get; set; }
        /// <summary>
        /// 链接归属词组Id，多个用分号相连
        /// </summary>
        public string KeyCateId { get; set; }
    }
}
