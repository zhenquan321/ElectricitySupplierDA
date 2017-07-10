using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class KwywordLinksVO
    {
        public List<nodes> nodes { get; set; }
        public List<linksdto> links { get; set; }
    }

    /// <summary>
    /// 统计项目链接时计算类
    /// </summary>
    public class ProLinkKey
    {
        public string Id { get; set; }
        public DateTime UpdateTime { get; set; }
        public string ProjectId { get; set; }
        /// <summary>
        /// 链接数
        /// </summary>
        public int LinkCount { get; set; }
    }
}