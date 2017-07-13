using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace IW2S.Models
{
    #region 话语提取
    /// <summary>
    /// 文本抽取类
    /// </summary>
    public class TextExtract
    {
        /// <summary>
        /// 命名实体
        /// </summary>
        public IWSData.Model.Dnl_EntityTreeDto Entity { get; set; }
        /// <summary>
        /// 抽取出的话语
        /// </summary>
        public List<TextInfo> Text { get; set; }
        public List<ObjectId> LinkIds { get; set; }
        public List<string> regStrs { get; set; }
    }

    public class TextInfo
    {
        public string Content { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
    #endregion

    #region 用户管理
    /// <summary>
    /// 用户数量统计
    /// </summary>
    public class UserCount
    {
        /// <summary>
        /// 活跃用户
        /// </summary>
        public int ActiveUser { get; set; }
        /// <summary>
        /// 活跃用户百分比
        /// </summary>
        public double ActiveUserPercent { get; set; }
        /// <summary>
        /// 付费用户
        /// </summary>
        public int PurchaseUser { get; set; }
        /// <summary>
        /// 付费用户百分比
        /// </summary>
        public double PurchaseeUserPercent { get; set; }
        /// <summary>
        /// 新增付费用户
        /// </summary>
        public int NewPurchaseUser { get; set; }
        /// <summary>
        /// 新增普通用户
        /// </summary>
        public int NewFreeUser { get; set; }
    }

    /// <summary>
    /// 用户变化数据
    /// </summary>
    public class UserLineChart
    {
        /// <summary>
        /// 时间坐标
        /// </summary>
        public List<string> Time { get; set; }
        /// <summary>
        /// 免费用户
        /// </summary>
        public List<int> Free { get; set; }
        /// <summary>
        /// 付费用户
        /// </summary>
        public List<int> Purchase { get; set; }
    }

    /// <summary>
    /// 用户地理分布
    /// </summary>
    public class UserDistribution
    {
        /// <summary>
        /// 城市名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 用户数
        /// </summary>
        public int value { get; set; }
    }

    /// <summary>
    /// 用户统计信息
    /// </summary>
    public class UserCountInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户级别
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 已付费用
        /// </summary>
        public double PurchaseMoney { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 已签署合同
        /// </summary>
        public List<string> Contract { get; set; }
        /// <summary>
        /// 项目数
        /// </summary>
        public int ProjectNum { get; set; }
        /// <summary>
        /// 关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        /// <summary>
        /// 链接数
        /// </summary>
        public int LinkNum { get; set; }
        /// <summary>
        /// 简报数
        /// </summary>
        public int ReportNum { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户呢称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string PictureSrc { get; set; }
        /// <summary>
        /// 用户级别
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 已签署合同数
        /// </summary>
        public int ContractNum { get; set; }
        /// <summary>
        /// 项目数
        /// </summary>
        public int ProjectNum { get; set; }
        /// <summary>
        /// 关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        /// <summary>
        /// 链接数
        /// </summary>
        public int LinkNum { get; set; }
        /// <summary>
        /// 简报数
        /// </summary>
        public int ReportNum { get; set; }
        /// <summary>
        /// 最大项目数
        /// </summary>
        public int MaxProjectNum { get; set; }
        /// <summary>
        /// 最大关键词数
        /// </summary>
        public int MaxKeywordNum { get; set; }
        /// <summary>
        /// 最大链接数
        /// </summary>
        public int MaxLinkNum { get; set; }
        /// <summary>
        /// 最大简报数
        /// </summary>
        public int MaxReportNum { get; set; }
        /// <summary>
        /// 技术支持次数
        /// </summary>
        public int SupportNum { get; set; }
        /// <summary>
        /// 数据分析次数
        /// </summary>
        public int DataAnalysisNum { get; set; }
        /// <summary>
        /// 最大技术支持次数
        /// </summary>
        public int MaxSupportNum { get; set; }
        /// <summary>
        /// 最大数据分析次数
        /// </summary>
        public int MaxDataAnalysisNum { get; set; }

    }
    #endregion

    /// <summary>
    /// 百度气泡图结果
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

    /// <summary>
    /// 微信时间标题数据
    /// </summary>
    public class WeiXinTimelinkDto
    {
        /// <summary>
        /// 链接Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 微信公众号呢称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
    }

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
    /// 链接信息类
    /// </summary>
    public class WXLinkInfoDto
    {
        public string 编号 { get; set; }
        public int 年 { get; set; }
        public int 月 { get; set; }
        public int 日 { get; set; }
        public string 文章标题 { get; set; }
        public string 公众号 { get; set; }
        /// <summary>
        /// 链接命中关键词
        /// </summary>
        public List<string> 关键词 { get; set; }
        public int 命中关键词数 { get; set; }
        public int 相关文章数 { get; set; }
        public int 阅读数 { get; set; }
        public int 点赞数 { get; set; }
        public int 文章影响力 { get; set; }
        public int 评论数 { get; set; }
    }

    /// <summary>
    /// 网页关系图统计信息
    /// </summary>
    public class LinkReferCount
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProName { get; set; }
        /// <summary>
        /// 项目描述
        /// </summary>
        public string ProDesc { get; set; }
        /// <summary>
        /// 关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        /// <summary>
        /// 分组数
        /// </summary>
        public int CateNum { get; set; }
        /// <summary>
        /// 链接数
        /// </summary>
        public int LinkNum { get; set; }
        /// <summary>
        /// 网站数
        /// </summary>
        public int SiteNum { get; set; }
    }

    /// <summary>
    /// 插入生成的订单信息
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public IWSData.Model.OrderDto order { get; set; }
        /// <summary>
        /// 二维码
        /// </summary>
<<<<<<< HEAD
        public System.Net.Http.HttpResponseMessage qrcode { get; set; }
=======
        public string qrcode { get; set; }
>>>>>>> c26f92d240a523a1903a8e87db204683ad299860
    }
}