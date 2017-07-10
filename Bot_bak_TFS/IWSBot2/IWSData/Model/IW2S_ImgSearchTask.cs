using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_ImgSearchTask
    {
        public ObjectId _id { get; set; }
        public string Src { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
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
        public ObjectId ProjectId { get; set; }
    }

    public class IW2S_ImgSearchLink
    {
        public ObjectId _id { get; set; }
        public ObjectId IW2S_ImgSearchTaskId { get; set; }
        public string Src { get; set; }
        public string LinkUrl { get; set; }
        public string BaiduImgUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Domain { get;set; }
        public string TopDomain { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string PublishTime { get; set; }
        public ObjectId ProjectId { get; set; }
        public bool IsDel { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId BizId { get; set; }
        public int Rank { get; set; }
        /// <summary>
        /// 数据清洗状态：1，收藏;2,排除
        /// </summary>
        public Nullable<byte> DataCleanStatus { get; set; }
    }

    public class IW2S_ImgSearchTaskDto
    {
        public string _id { get; set; }
        public string Src { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public string UsrId { get; set; }

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
        public string ProjectId { get; set; }
        
    }

    public class IW2S_ImgSearchLinkDto
    {
        public string _id { get; set; }
        public string IW2S_ImgSearchTaskId { get; set; }
        public string Src { get; set; }
        public string LinkUrl { get; set; }
        public string BaiduImgUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string TopDomain { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string PublishTime { get; set; }
        public string ProjectId { get; set; }
        public bool IsDel { get; set; }
        public string UsrId { get; set; }
        public string BizId { get; set; }
        public int Rank { get; set; }
        /// <summary>
        /// 数据清洗状态：1，收藏;2,排除
        /// </summary>
        public Nullable<byte> DataCleanStatus { get; set; }
    }
}
