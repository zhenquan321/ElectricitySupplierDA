using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw2swcsserch.Models
{
    public class IW2S_WX_level1link
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词Id
        /// </summary>
        public string SearchkeywordId { get; set; }
        /// <summary>
        /// 应用类型：仿冒监测，域名监测
        /// </summary>
        public byte AppType { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 主域名
        /// </summary>
        public string TopDomain { get; set; }
        public string LinkUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 保护域名
        /// </summary>
        public string ProtectDomainName { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }
        public Nullable<byte> MatchAt { get; set; }
        /// <summary>
        /// 匹配类型：搜索关键词、业务关键词
        /// </summary>
        public Nullable<byte> MatchType { get; set; }
        /// <summary>
        /// 打分结果
        /// </summary>
        public Nullable<int> Score { get; set; }
        /// <summary>
        /// 数据清洗状态：1，收藏;2,排除
        /// </summary>
        public Nullable<byte> DataCleanStatus { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public System.DateTime CreatedAt { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        /// <summary>
        /// 网页原文内容
        /// </summary>
        public string Html { get; set; }

        public string WebsiteId { get; set; }

        public Guid BizId { get; set; }

        public string Abstract { get; set; }
        public ObjectId UsrIdSiteId { get; set; }
        public int BaiduVStar { get; set; }
        public bool IsMarket { get; set; }
        public ObjectId ProjectId { get; set; }
        public ObjectId InfriLawCode { get; set; }

        public string PublishTime { get; set; }
        public string AlternateFields { get; set; }

        public string PublicNo { get; set; }

        public string Function { get; set; }
        public string QrCode { get; set; }
        public string SourceLink { get; set; }
        public int TagType { get; set; }
        public string ImgIcon { get; set; }

        public string TitleImg { get; set; }
        public string QrCodeIcon { get; set; }
    }
}
