using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot2.Models
{
    /// <summary>
    /// 气泡图结果
    /// </summary>
    public class DomainStatisDto
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 有效链接数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 关键词数
        /// </summary>
        public int KeywordTotal { get; set; }
        /// <summary>
        /// 域名收录量
        /// </summary>
        public long DomainColNum { get; set; }
        /// <summary>
        /// 含发布时间比
        /// </summary>
        public string PublishRatio { get; set; }
        /// <summary>
        /// 域名分组Id
        /// </summary>
        public string DomainCategoryId { get; set; }
        /// <summary>
        /// 域名分组名
        /// </summary>
        public string DomainCategoryName { get; set; }
    }

    /// <summary>
    /// 折线图及饼图数据
    /// </summary>
    public class TimeLinkCountDto
    {
        /// <summary>
        /// 时间坐标
        /// </summary>
        public List<DateTime> Times { get; set; }
        /// <summary>
        /// 折线图数据，每个对象为一条线
        /// </summary>

        public List<LineData> LineDataList { get; set; }
        /// <summary>
        /// 精简版自动摘要，用于在折线图生成提示及生成饼图
        /// </summary>
        public List<SumData> Sum { get; set; }
    }

    /// <summary>
    /// 微信气泡图结果
    /// </summary>
    public class WXDomainStatisDto
    {
        /// <summary>
        /// 公众号名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 链接数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 关键词数
        /// </summary>
        public int KeywordTotal { get; set; }
        /// <summary>
        /// 热度
        /// </summary>
        public int HotNum { get; set; }
        /// <summary>
        /// 域名分组Id
        /// </summary>
        public string DomainCategoryId { get; set; }
        /// <summary>
        /// 域名分组名
        /// </summary>
        public string DomainCategoryName { get; set; }
        /// <summary>
        /// 含发布时间比
        /// </summary>
        public float PublishRatio { get; set; }
    }

    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    #region 网页关系图
    /// <summary>
    /// 链接关系词组
    /// </summary>
    public class LinkRefer_Cate
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
    /// 链接关系图计算所需链接信息
    /// </summary>
    public class LinkRefer_Info
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 搜索关键词Id
        /// </summary>
        public string KeywordId { get; set; }
        /// <summary>
        /// 微信文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 微信文章正文摘要
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 微信文章url地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
    }

    /// <summary>
    /// 链接关系节点
    /// </summary>
    public class LinkRefer_Node
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
        public LinkRefer_Node()
        {

        }
        public LinkRefer_Node(LinkRefer_Node source)
        {
            name = source.name;
            describe = source.describe;
            value = source.value;
            keyWordCount = source.keyWordCount;
            keyWordList = source.keyWordList;
            category = source.category;
            publishTime = source.publishTime;
            linkUrl = source.linkUrl;
        }
    }

    /// <summary>
    /// 两个链接结点间关系
    /// </summary>
    public class LinkRefer_Refer
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
        public List<LinkRefer> ReferList { get; set; }
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
    public class LinkRefer
    {
        public string Json { get; set; }
        /// <summary>
        /// 集群核心点
        /// </summary>
        public List<LinkRefer_Node> TopData { get; set; }
        /// <summary>
        /// 包括最多关键词的点
        /// </summary>
        public List<LinkRefer_Node> TopKeyData { get; set; }
        /// <summary>
        /// 包含最多组外链接数的点
        /// </summary>
        public List<LinkRefer_Node> TopCateData { get; set; }
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
    #endregion

    #region 公众号热度分析
    /// <summary>
    /// 公众号热度分析
    /// </summary>
    public class NameStatisticDto
    {
        /// <summary>
        /// 公众号名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公众号Id
        /// </summary>
        public string NameId { get; set; }
        /// <summary>
        /// 发布文章数
        /// </summary>
        public int LinkNum { get; set; }
        ///// <summary>
        ///// 文章信息列表
        ///// </summary>
        //public List<WXLinkInfo> LinkInfoList { get; set; }
        /// <summary>
        /// 累计评论数
        /// </summary>
        public int CommentNum { get; set; }
        /// <summary>
        /// 累计点赞数
        /// </summary>
        public int LikeNum { get; set; }
        /// <summary>
        /// 累计阅读数
        /// </summary>
        public int ReadNum { get; set; }
        /// <summary>
        /// 命中关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        ///// <summary>
        ///// 关键词信息列表
        ///// </summary>
        //public List<WXKeywordInfo> KeyInfoList { get; set; }
        /// <summary>
        /// 影响力指数
        /// </summary>
        public int InfluenceNum { get; set; }
    }

    /// <summary>
    /// 微信文章信息
    /// </summary>
    public class WXLinkInfo
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentNum { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeNum { get; set; }
        /// <summary>
        /// 阅读数
        /// </summary>
        public int ReadNum { get; set; }
        /// <summary>
        /// 命中关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        /// <summary>
        /// 影响力指数
        /// </summary>
        public int InfluenceNum { get; set; }
    }

    /// <summary>
    /// 微信关键词信息
    /// </summary>
    public class WXKeywordInfo
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 命中次数
        /// </summary>
        public int MatchNum { get; set; }
        /// <summary>
        /// 链接信息
        /// </summary>
        public List<LinkTitleAUrl> LinkList { get; set; }
        /// <summary>
        /// 影响力指数
        /// </summary>
        public int InfluenceNum { get; set; }
    }

    public class LinkTitleAUrl
    {
        /// <summary>
        /// 链接地址，排重使用
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
    #endregion

    /// <summary>
    /// 列表数据及总数类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class QueryResult<T>
    {
        public List<T> Result { get; set; }
        public long Count { get; set; }
    }
}
