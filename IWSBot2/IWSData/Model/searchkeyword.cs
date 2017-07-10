using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class searchkeyword
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 应用类型：仿冒监测，域名监测
        /// </summary>
        public byte AppType { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 保护域名
        /// </summary>
        public string ProtectDomainName { get; set; }
        /// <summary>
        /// 是否启动搜索
        /// </summary>
        public bool IsStarted { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 任务间隔小时
        /// </summary>
        public int BotIntervalHours { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public Guid UsrId { get; set; }
        /// <summary>
        /// 下次搜索开始时间
        /// </summary>
        public DateTime NextBotStartAt { get; set; }

        /// <summary>
        /// 上次搜索开始时间
        /// </summary>
        public DateTime LastBotEndAt { get; set; }

        public bool IsSelected { get; set; }

        public bool IsBot { get; set; }
        /// <summary>
        /// IP.ProcessId
        /// </summary>
        public string BotTag { get; set; }

    }
}
