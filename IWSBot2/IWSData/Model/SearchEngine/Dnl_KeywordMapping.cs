using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 私有关键词与公有关键词映射类
    /// </summary>
    public class Dnl_KeywordMapping
    {
        public ObjectId _id { get; set; }
        public ObjectId CategoryId { get; set; }
        /// <summary>
        /// 归属分组的父分组Id，用于在对已分出次级词组的词组再一次分组时获取已被分组关键词
        /// </summary>
        public ObjectId ParentCategoryId { get; set; }
        public ObjectId KeywordId { get; set; }
        public string Keyword { get; set; }
        public ObjectId UserId { get; set; }
        public bool IsDel { get; set; }
        public ObjectId ProjectId { get; set; }
        /// <summary>
        /// 关键词共现图计算使用
        /// </summary>
        public int GroupNumber { get; set; }
        /// <summary>
        /// 关键词共现图计算使用
        /// </summary>
        public int JisuanStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DelAt { get; set; }
    }

    /// <summary>
    /// 关键词分组类
    /// </summary>
    public class Dnl_KeywordCategory
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public bool IsDel { get; set; }
        public ObjectId ParentId { get; set; }
        public ObjectId UserId { get; set; }
        public int Weight { get; set; }
        public ObjectId InfriLawCode { get; set; }
        public ObjectId ProjectId { get; set; }
        public int GroupNumber { get; set; }
        /// <summary>
        /// 组内当前关键词数
        /// </summary>
        public int KeywordCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 关键词共现类
    /// </summary>
    public class Dnl_MappingCoPresent
    {
        public ObjectId _id { get; set; }
        public int source { get; set; }
        public int target { get; set; }
        public int value { get; set; }
        public ObjectId KeywordMappingId { get; set; }
        public ObjectId CategoryId { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class Dnl_KeywordMappingDto
    {
        public string _id { get; set; }
        public string CommendCategoryId { get; set; }
        public string ParentCategoryId { get; set; }
        public string BaiduCommendId { get; set; }
        public string BaiduCommend { get; set; }
    }

    /// <summary>
    /// 关键词组类
    /// </summary>
    public class Dnl_KeywordCategoryDto
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public int Weight { get; set; }
        public string InfriLawCode { get; set; }
        public string InfriLawCodeStr { get; set; }
        public int KeywordTotal { get; set; }
        public int BotStatus { get; set; }
        public bool isselected { get; set; }
        public int GroupNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
    
    /// <summary>
    /// 词组修改时关键词类
    /// </summary>
    public class KeywordMappingModelDto
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Dnl_KeywordDto> Selected { get; set; }
        public List<Dnl_KeywordDto> UnSelected { get; set; }
    }
}
