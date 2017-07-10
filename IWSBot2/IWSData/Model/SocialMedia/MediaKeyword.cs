using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 公用关键词库类
    /// </summary>
    public class MediaKeywordMongo
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 任务间隔小时
        /// </summary>
        public int BotIntervalHours { get; set; }
        /// <summary>
        /// 微信上次搜索时间
        /// </summary>
        public DateTime WXLastBotAt { get; set; }
        /// <summary>
        /// 微信链接数
        /// </summary>
        public int WXLinkNum { get; set; }
        /// <summary>
        /// 微信搜索状态
        /// </summary>
        public int WXBotStatus { get; set; }
        ///// <summary>
        ///// 微信搜索起始时间
        ///// </summary>
        //public DateTime WXStartTime { get; set; }
        ///// <summary>
        ///// 微信搜索结束时间
        ///// </summary>
        //public DateTime WXEndTime { get; set; }
    }
}
