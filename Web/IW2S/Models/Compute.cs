using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;

namespace IW2S.Models
{
    /* 后台临时计算时使用类 */

    /// <summary>
    /// 关键词插入词组时其分组信息
    /// </summary>
    public class KeywordCateInfo
    {
        /// <summary>
        /// 分组Id
        /// </summary>
        public ObjectId CategoryId { get; set; }
        /// <summary>
        /// 归属分组的父分组Id，用于在对已分出次级词组的词组再一次分组时获取已被分组关键词
        /// </summary>
        public ObjectId ParentCategoryId { get; set; }
    }

    

    public class jsonFileUrlDto
    {
        public string Url { get; set; }
        public string Error { get; set; }

    }

    public class DomainKeywordDto
    {
        public string Domain { get; set; }
        public string KeywordId { get; set; }
    }

    public class DomainCategoryInfo
    {
        public List<string> Domain { get; set; }
        public List<string> DomainCategoryId { get; set; }
        public List<string> DomainCategoryName { get; set; }
    }
}