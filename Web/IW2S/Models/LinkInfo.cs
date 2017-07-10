using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    /// <summary>
    /// 链接信息类
    /// </summary>
    public class LinkInfo
    {
        public string 编号 { get; set; }
        public int 年 { get; set; }
        public int 月 { get; set; }
        public int 日 { get; set; }
        public string 链接标题 { get; set; }
        public string 域名 { get; set; }
        /// <summary>
        /// 链接命中关键词
        /// </summary>
        public List<string> 关键词 { get; set; }
        public int 命中关键词数 { get; set; }
        public int 网页关联数 { get; set; }
        public int 链接影响力 { get; set; }
        /// <summary>
        /// 链接归属域名分类
        /// </summary>
        public string 域名分组 { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int 评论数 { get; set; }
    }
}