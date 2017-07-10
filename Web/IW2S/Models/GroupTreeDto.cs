using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    /// <summary>
    /// 横向动态关键词分组树
    /// </summary>
    public class GroupTreeDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 分组名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 子结点
        /// </summary>
        public List<GroupTreeDto> children { get; set; }
    }

    /// <summary>
    /// 分组树（圆形d3图）
    /// </summary>
    public class GroupTree2Dto
    {
        public string id { get; set; }
        /// <summary>
        /// 父结点Id
        /// </summary>
        public string pId { get; set; }
        /// <summary>
        /// 结点名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否为非叶子结点
        /// </summary>
        public bool isNode { get; set; }
        
    }

    public class GroupKeywordsDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public int ValLinkCount { get; set; }
        public int BotStatus { get; set; }
    }

    public class GroupTree3Dto
    {
        public string name { get; set; }
        public string size { get; set; }
        public List<GroupTree3Dto> children { get; set; }

    }


}