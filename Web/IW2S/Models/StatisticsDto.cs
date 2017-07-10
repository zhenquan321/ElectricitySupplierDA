using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    /// <summary>
    /// 项目和简报数量统计类
    /// </summary>
    public class StatisticsDto
    {
        /// <summary>
        /// 时间坐标
        /// </summary>
        public List<DateTime> Times { get; set; }
        /// <summary>
        /// 我创建的
        /// </summary>
        public List<int> MyCreate { get; set; }
        /// <summary>
        /// 我分享的
        /// </summary>
        public List<int> MyShare { get; set; }
        /// <summary>
        /// 分享给我的
        /// </summary>
        public List<int> ShowToMe { get; set; }
    }

    /// <summary>
    /// 单个结点信息
    /// </summary>
    public class TimeToCount
    {
        /// <summary>
        /// 时间坐标
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 坐标对应数量
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 项目或简报的时间信息
    /// </summary>
    public class ItemDateInfo
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? DelAt { get; set; }
        public bool IsDel { get; set; }
    }

    ///// <summary>
    ///// 项目有效链接数统计时关键词类
    ///// </summary>
    //public class ProLinkCountKey
    //{
    //    public string Id { get; set; }
    //    /// <summary>
    //    /// 最后更新时间
    //    /// </summary>
    //    public DateTime UpdateTime { get; set; }
    //    /// <summary>
    //    /// 项目Id
    //    /// </summary>
    //    public string ProjectId { get; set; }
    //    /// <summary>
    //    /// 有效链接数
    //    /// </summary>
    //    public int ValLinkCount { get; set; }
        
    //}
}