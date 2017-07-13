using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{

    public enum KeywordType
    {
        /// <summary>
        /// 业务关键词
        /// </summary>
        Bussiness,
        /// <summary>
        /// 排除关键词
        /// </summary>
        Excluding,
        /// <summary>
        /// 搜索关键词
        /// </summary>
        Search,
    }

    public enum AppType
    {
        /// <summary>
        /// 仿冒监测
        /// </summary>
        Fake,
        /// <summary>
        /// 域名监测
        /// </summary>
        Domain,

    }

    public enum BaiduItemPart
    {
        /// <summary>
        /// 不相关
        /// </summary>
        None,
        /// <summary>
        /// 全文
        /// </summary>
        All,
        /// <summary>
        /// 标题
        /// </summary>
        Title,
        /// <summary>
        /// 摘要
        /// </summary>
        Abstract,
        /// <summary>
        /// 标题和摘要
        /// </summary>
        TitleAbstract,
        /// <summary>
        /// URL
        /// </summary>
        Url,
    }

    public enum MatchType
    {
        /// <summary>
        /// 业务关键词
        /// </summary>
        BussinessKeyword,
        /// <summary>
        /// 搜索关键词
        /// </summary>
        Searchkeyword,
    }

    public enum DataCleanStatus
    {
        /// <summary>
        /// 有嫌疑
        /// </summary>
        Suspected,
        /// <summary>
        /// 排除嫌疑
        /// </summary>
        Excluded,
        /// <summary>
        /// 确认侵权
        /// </summary>
        Confirmed,

    }

    public enum BotStatus
    {
        Blocked = 0,
        StructureChanged = 1,
        ServerError = 2,
        Removed = 3,
        Ok = 4,
    }

    public enum ShareOperateType
    {
        AddKeyword = 0,//添加搜索词
        DelKeyword = 1,//删除搜索词
        FilterConfig = 2,//过滤设置
        CollectConfig = 3,//收藏设置
        ManageGroup = 4,//创建和管理词组
        ManageAnalysisItem = 5,//分析指向管理
        SearchResultConfig = 6,//监测结果设置
        SetLinkAnalysisItem = 7,//设置连接分析指项
        ImportKeywordGroup = 8,//导入分组
        ReSearchKeyword = 9,//重新搜索关键词
        SaveDomainCategory = 10,//保存域名分类
        DelDomainCategory = 11,//保存域名分类
    }

    public enum SiteSource
    {
        Baidu = 0,//百度
        BaiduWeibo = 1,//百度微博
        Weichat = 2,//微信
        Sogou = 3,//搜狗
        BaiduImg = 4,//百度图片
        emarketnow = 5,//市场快照
    }
    public enum ShareOutOperateType
    {
        ValidLinkStatis = 0,//有效链接统计图
    }

    /// <summary>
    /// 图表类型
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// 折线图和饼图
        /// </summary>
        Line,
        /// <summary>
        /// 气泡图
        /// </summary>
        Bubble,
        /// <summary>
        /// 词云图
        /// </summary>
        WordCloud,
        /// <summary>
        /// 词频图
        /// </summary>
        WordFrequence,
        /// <summary>
        /// 网页关系图
        /// </summary>
        LinkReference,
        /// <summary>
        /// 公众号热度排行分析表
        /// </summary>
        NameStatis,
    }

    /// <summary>
    /// 信源类型
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// 搜索引擎
        /// </summary>
        Enginee,
        /// <summary>
        /// 社交媒体
        /// </summary>
        Media
    }

    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 微信
        /// </summary>
        WeiXin,
    }
}
