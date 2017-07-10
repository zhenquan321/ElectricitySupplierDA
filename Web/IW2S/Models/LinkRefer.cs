using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    /// <summary>
    /// 链接关系词组
    /// </summary>
    public class LinkReferCate
    {
        /// <summary>
        /// 词组简称
        /// </summary>
        public string name { get; set; }
        public string keyword { get; set; }
        /// <summary>
        /// 词组全称，转换到Json时字段名应改为base
        /// </summary>
        public string baseName { get; set; }
        public string id { get; set; }
    }

    /// <summary>
    /// 关键词及其搜索节点
    /// </summary>
    public class LinkReferByKey
    {
        /// <summary>
        /// 关键词Id
        /// </summary>
        public string KeywordId { get; set; }
        /// <summary>
        /// 关键词对应节点
        /// </summary>
        public List<LinkReferNode> NodeList { get; set; }
    }

    
    /// <summary>
    /// 链接关系节点
    /// </summary>
    public class LinkReferNode
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 备用属性，暂为1
        /// </summary>
        public int value { get; set; }
        /// <summary>
        /// 所含关键词数
        /// </summary>
        public int keyWordCount { get; set; }
        /// <summary>
        /// 包含关键词列表
        /// </summary>
        public List<string> keyWordList { get; set; }
        /// <summary>
        /// 包含关键词Id列表
        /// </summary>
        public List<string> keyWordIdList { get; set; }
        /// <summary>
        /// 归属关键词组序号
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime publishTime { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string linkUrl { get; set; }
        /// <summary>
        /// 排序使用的数据（选取核心点时使用）
        /// </summary>
        public long SortNum { get; set; }
        /// <summary>
        /// 列表中位置（计算关系时使用）
        /// </summary>
        public int Index { get; set; }
        public LinkReferNode()
        {

        }
        public LinkReferNode(LinkReferNode source)
        {
            name = source.name;
            describe = source.describe;
            value = source.value;
            keyWordCount = source.keyWordCount;
            keyWordList = source.keyWordList;
            category = source.category;
            publishTime = source.publishTime;
            linkUrl = source.linkUrl;
            SortNum = source.SortNum;
        }
    }

    /// <summary>
    /// 链接关系节点
    /// </summary>
    public class NameReferNode
    {
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime publishTime { get; set; }
        /// <summary>
        /// 公众号呢称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 备用属性，暂为1
        /// </summary>
        public int value { get; set; }
        /// <summary>
        /// 所含关键词数
        /// </summary>
        public int keyWordCount { get; set; }
        /// <summary>
        /// 包含关键词列表
        /// </summary>
        public List<string> keyWordList { get; set; }
        /// <summary>
        /// 包含关键词Id列表
        /// </summary>
        public List<string> keyWordIdList { get; set; }
        /// <summary>
        /// 归属关键词组序号
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 公众号微信名
        /// </summary>
        public string WXName { get; set; }
        /// <summary>
        /// 排序使用的数据（选取核心点时使用）
        /// </summary>
        public long SortNum { get; set; }
        /// <summary>
        /// 列表中位置（计算关系时使用）
        /// </summary>
        public int Index { get; set; }
        public NameReferNode()
        {

        }
        public NameReferNode(NameReferNode source)
        {
            nickname = source.nickname;
            describe = source.describe;
            value = source.value;
            keyWordCount = source.keyWordCount;
            keyWordList = source.keyWordList;
            category = source.category;
            WXName = source.WXName;
            SortNum = source.SortNum;
        }
    }

    /// <summary>
    /// 关键词及其搜索节点
    /// </summary>
    public class NameReferByKey
    {
        /// <summary>
        /// 关键词Id
        /// </summary>
        public string KeywordId { get; set; }
        /// <summary>
        /// 关键词对应节点
        /// </summary>
        public List<NameReferNode> NodeList { get; set; }
    }


    /// <summary>
    /// 链接分析结果及Top数据
    /// </summary>
    public class NameReferDto
    {
        public string Json { get; set; }
        /// <summary>
        /// 集群核心点
        /// </summary>
        public List<NameReferNode> TopData { get; set; }
        /// <summary>
        /// 包括最多关键词的点
        /// </summary>
        public List<NameReferNode> TopKeyData { get; set; }
        /// <summary>
        /// 包含最多组外链接数的点
        /// </summary>
        public List<NameReferNode> TopCateData { get; set; }
        /// <summary>
        /// 分组权重
        /// </summary>
        public List<CategoryWeight> CateWeights { get; set; }
    }

    /// <summary>
    /// 不同时间刻度下的链接关系图数据
    /// </summary>
    public class TimeNameRefer
    {
        public List<DateTime> DateTimeList { get; set; }
        public List<NameReferDto> ReferList { get; set; }
    }

    /// <summary>
    /// 两个链接结点间关系
    /// </summary>
    public class LinkReferIn2Node
    {
        /// <summary>
        /// 起点序号
        /// </summary>
        public int source { get; set; }
        /// <summary>
        /// 终点序号
        /// </summary>
        public int target { get; set; }
    }

    /// <summary>
    /// 不同时间刻度下的链接关系图数据
    /// </summary>
    public class TimeLinkRefer
    {
        public List<DateTime> DateTimeList { get; set; }
        public List<LinkReferDto> ReferList { get; set; }
    }

    /// <summary>
    /// 分组权重
    /// </summary>
    public class CategoryWeight
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 链接分析结果及Top数据
    /// </summary>
    public class LinkReferDto
    {
        public string Json { get; set; }
        /// <summary>
        /// 集群核心点
        /// </summary>
        public List<LinkReferNode> TopData { get; set; }
        /// <summary>
        /// 包括最多关键词的点
        /// </summary>
        public List<LinkReferNode> TopKeyData { get; set; }
        /// <summary>
        /// 包含最多组外链接数的点
        /// </summary>
        public List<LinkReferNode> TopCateData { get; set; }
        /// <summary>
        /// 分组权重
        /// </summary>
        public List<CategoryWeight> CateWeights { get; set; }
    }

    /// <summary>
    /// 统计组外链接数
    /// </summary>
    public class LinkGroupCount
    {
        /// <summary>
        /// 链接在链接列表中位置
        /// </summary>
        public int Pos { get; set; }
        /// <summary>
        /// 组外链接数
        /// </summary>
        public int Count { get; set; }
    }
}