using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    
    public class IW2S_WB_level1link
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 搜索关键词Id
        /// </summary>
        public ObjectId SearchkeywordId { get; set; }
        
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        public string HeadIcon { get; set; }
        public string PostUrl { get; set; }
        public string PosterUrl { get; set; }
        public string PostImgUrl { get; set; }
        public bool IsBlueV { get; set; }

        public string Description { get; set; }
                
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }
        
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

        public ObjectId BizId { get; set; }
        
        public ObjectId ProjectId { get; set; }
        public ObjectId InfriLawCode { get; set; }

        public DateTime PublishTime { get; set; }
        public string AlternateFields { get; set; }
        public bool IsDel { get; set; }

        public int Rank { get; set; }
    }

    public class IW2S_WB_level1linkDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 搜索关键词Id
        /// </summary>
        public string SearchkeywordId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        public string HeadIcon { get; set; }
        public string PostUrl { get; set; }
        public string PosterUrl { get; set; }
        public string PostImgUrl { get; set; }
        public bool IsBlueV { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }

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
        public string UsrId { get; set; }
        /// <summary>
        /// 网页原文内容
        /// </summary>
        public string Html { get; set; }

        public string BizId { get; set; }

        public string ProjectId { get; set; }
        public string InfriLawCode { get; set; }
        public string InfriLawCodeStr { get; set; }
        public DateTime PublishTime { get; set; }
        public string AlternateFields { get; set; }
        public bool IsDel { get; set; }

        public int Rank { get; set; }

    }

    public class IW2S_WB_Timelevel1linkDto
    {

        /// <summary>
        /// 搜索关键词Id
        /// </summary>
        public string SearchkeywordId { get; set; }

        public string LinkUrl { get; set; }

        public string Title { get; set; }


        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keywords { get; set; }

        public DateTime PublishTime { get; set; }

    }
}
