using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iw2sDataAnalysis.Models
{
    /// <summary>
    /// 公用关键词库类
    /// </summary>
    public class Dnl_Keyword
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
        /// 百度上次搜索时间
        /// </summary>
        public DateTime LastBotEndAt_Baidu { get; set; }

        /// <summary>
        /// 百度搜索状态：0:未搜索,1:搜索中,2:搜索完成
        /// </summary>
        public int BotStatus_Baidu { get; set; }
        /// <summary>
        /// 百度有效链接数
        /// </summary>
        public int ValLinkCount_Baidu { get; set; }
        /// <summary>
        /// 百度链接数
        /// </summary>
        public int LinkCount_Baidu { get; set; }

    }

    /// <summary>
    /// 关键词类
    /// </summary>
    public class Dnl_KeywordDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public int BotStatus { get; set; }
        public int ValLinkCount { get; set; }
        public bool isselected { get; set; }

        public int GroupNumber { get; set; }
        public int JisuanStatus { get; set; }
        public bool drag { get; set; }

    }
}
