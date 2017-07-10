using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot2.Models
{
    /// <summary>
    /// 域名分类信息
    /// </summary>
    public class DomainCategoryInfo
    {
        public List<string> Domain { get; set; }
        public List<string> DomainCategoryId { get; set; }
        public List<string> DomainCategoryName { get; set; }
    }


    /// <summary>
    /// 折线图单条线数据
    /// </summary>
    public class LineData
    {
        public string name { get; set; }
        public List<int> LinkCount { get; set; }
        public List<TopData> topData { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// Top数据
    /// </summary>
    public class TopData
    {
        public DateTime X { get; set; }
        public int Y { get; set; }
        public string name { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// 摘要数据
    /// </summary>
    public class SumData
    {
        public DateTime X { get; set; }
        public int Y { get; set; }
        public string Summary { get; set; }
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }

    /// <summary>
    /// 链接信息
    /// </summary>
    public class LinkStatus
    {
        public DateTime PublishTime { get; set; }
        public string CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectId { get; set; }
    }

    /// <summary>
    /// 链接分组信息
    /// </summary>
    public class CategoryList
    {
        public List<DateTime> PublishTime { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
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
}
