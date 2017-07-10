using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iw2sDataAnalysis.Models
{
    public class IW2S_WX_BaiduCommend
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        public ObjectId KeywordId { get; set; }

        public string CommendKeyword { get; set; }

        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public ObjectId UsrId { get; set; }

        /// <summary>
        /// 任务间隔小时
        /// </summary>
        public int BotIntervalHours { get; set; }
        /// <summary>
        /// 下次搜索开始时间
        /// </summary>
        public DateTime NextBotStartAt { get; set; }

        /// <summary>
        /// 上次搜索开始时间
        /// </summary>
        public DateTime LastBotEndAt { get; set; }

        /// <summary>
        /// 1:搜索中,2:搜索完成
        /// </summary>
        public int BotStatus { get; set; }
        /// <summary>
        /// IP.ProcessId
        /// </summary>
        public string BotTag { get; set; }
        public string BotId { get; set; }
        public int Times { get; set; }

        //微信搜索状态
        public int WXStatus { get; set; }

        /// <summary>
        /// 0:百度热词，1:直搜百度
        /// </summary>
        public int SearchSource { get; set; }
        public ObjectId ProjectId { get; set; }
        public int ValLinkCount { get; set; }

        public int GroupNumber { get; set; }
        public int JisuanStatus { get; set; }

    }

    public class IW2S_WX_BaiduCommendDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        public string KeywordId { get; set; }

        public string CommendKeyword { get; set; }

        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public string UsrId { get; set; }

        public bool drag { get; set; }

        public int Times { get; set; }
        //微信搜索状态
        public int WXStatus { get; set; }

        public int BotStatus { get; set; }
        public string ProjectId { get; set; }
        public int ValLinkCount { get; set; }

        public bool isselected { get; set; }


        public int GroupNumber { get; set; }
        public int JisuanStatus { get; set; }

    }
}
