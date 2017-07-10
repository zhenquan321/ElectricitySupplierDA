using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iw2sDataAnalysis.Models
{
    public class IW2S_KeywordFilter
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        public ObjectId CommendKeywordId { get; set; }
                
        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public ObjectId UsrId { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class IW2S_KeywordFilterDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }

        public string CommendKeywordId { get; set; }

        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public string UsrId { get; set; }

        public string ProjectId { get; set; }
    }
}