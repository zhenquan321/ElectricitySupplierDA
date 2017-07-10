using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{

    public class IW2S_Bing_BaiduKeyword
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public ObjectId UsrId { get; set; }


        public int BotStatus { get; set; }
        /// <summary>
        /// IP.ProcessId
        /// </summary>
        public string BotTag { get; set; }
        public string BotId { get; set; }
        public string ProjectName { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class IW2S_Bing_BaiduKeywordDto
    {
        public string _id { get; set; }

        public string CommendKeywordId { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public string UsrId { get; set; }


        public int BotStatus { get; set; }
        /// <summary>
        /// IP.ProcessId
        /// </summary>
        public string BotTag { get; set; }

        public List<IW2S_Bing_BaiduCommendDto> BaiduCommends { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }

        public int ValLinkCount { get; set; }
    }
}
