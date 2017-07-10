using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iw2sDataAnalysis.Models
{
    public class IW2S_Data
    {
        public ObjectId _id { get; set; }
        public Guid WeChatId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicNoName { get; set; }
        public string PublicNo { get; set; }
        public string Function { get; set; }
        public string QrCode { get; set; }
        public string DetailUrl { get; set; }
        public string SourceLink { get; set; }
        public ObjectId companyID { get; set; }
        public int DataCleanstatus { get; set; }
        public DateTime? CreateAt { get; set; }
        public ObjectId keywordId { get; set; }
        public string keywordName { get; set; }
        public byte? MatchAt { get; set; }
        public string MatchType { get; set; }
        public int? Score { get; set; }
        public int TagType { get; set; }
        public string HtmlText { get; set; }
        public string QrCodeIcon { get; set; }
        public string ImgIcon { get; set; }
        public string TitleImg { get; set; }
        public string NameValue1 { get; set; }
        public string NameValue2 { get; set; }
        public ObjectId publicNoId { get; set; }

    }

    public class IW2S_SG_Data
    {
        public ObjectId _id { get; set; }
        public Guid WeChatId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicNoName { get; set; }
        public string PublicNo { get; set; }
        public string Function { get; set; }
        public string QrCode { get; set; }
        public string DetailUrl { get; set; }
        public string SourceLink { get; set; }
        public ObjectId companyID { get; set; }
        public int DataCleanstatus { get; set; }
        public DateTime? CreateAt { get; set; }
        public ObjectId keywordId { get; set; }
        public string keywordName { get; set; }
        public byte? MatchAt { get; set; }
        public string MatchType { get; set; }
        public int? Score { get; set; }
        public int TagType { get; set; }
        public string HtmlText { get; set; }
        public string QrCodeIcon { get; set; }
        public string ImgIcon { get; set; }
        public string TitleImg { get; set; }
        public string NameValue1 { get; set; }
        public string NameValue2 { get; set; }
        public ObjectId publicNoId { get; set; }

    }

    public class IW2S_WX_Data
    {
        public ObjectId _id { get; set; }
        public Guid WeChatId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicNoName { get; set; }
        public string PublicNo { get; set; }
        public string Function { get; set; }
        public string QrCode { get; set; }
        public string DetailUrl { get; set; }
        public string SourceLink { get; set; }
        public ObjectId companyID { get; set; }
        public int DataCleanstatus { get; set; }
        public DateTime? CreateAt { get; set; }
        public ObjectId keywordId { get; set; }
        public string keywordName { get; set; }
        public byte? MatchAt { get; set; }
        public string MatchType { get; set; }
        public int? Score { get; set; }
        public int TagType { get; set; }
        public string HtmlText { get; set; }
        public string QrCodeIcon { get; set; }
        public string ImgIcon { get; set; }
        public string TitleImg { get; set; }
        public string NameValue1 { get; set; }
        public string NameValue2 { get; set; }
        public ObjectId publicNoId { get; set; }

    }
}
