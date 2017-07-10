using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 简报关键词类
    /// </summary>
    public class Dnl_Report_Keyword
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public ObjectId CategoryId { get; set; }
        public ObjectId BaiduCommendId { get; set; }
        public string CommendKeyword { get; set; }
        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 信源：0为百度
        /// </summary>
        public int SourceType { get; set; }
    }

    /// <summary>
    /// 简报关键词类
    /// </summary>
    public class Dnl_Report_KeywordDto
    {
        public String _id { get; set; }
        public String CategoryId { get; set; }
        public String BaiduCommendId { get; set; }
        /// <summary>
        /// 关键词名
        /// </summary>
        public string CommendKeyword { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 信源：0为百度
        /// </summary>
        public int SourceType { get; set; }
    }

    /// <summary>
    /// 简报关键词组类
    /// </summary>
    public class Dnl_Report_KeywordCategory
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public string Name { get; set; }
        public int KeywordCount { get; set; }
        /// <summary>
        /// 信源：0为百度
        /// </summary>
        public int SourceType { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 是否排除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }

    /// <summary>
    /// 简报关键词组类
    /// </summary>
    public class Dnl_Report_KeywordCategoryDto
    {
        public String _id { get; set; }
        public String UsrId { get; set; }
        public String ReportId { get; set; }
        public string Name { get; set; }
        public int KeywordCount { get; set; }
        /// <summary>
        /// 信源：0为百度
        /// </summary>
        public int SourceType { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 词组内关键词列表
        /// </summary>
        public List<Dnl_Report_KeywordDto> KewordList { get; set; }
        /// <summary>
        /// 判断该数据是否显示
        /// </summary>
        public bool IsHide { get; set; }
    }
}
