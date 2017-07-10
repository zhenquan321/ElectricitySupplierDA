using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_Bing_PrjAnalysisItem
    {
        public ObjectId _id { get; set; }
        
        public ObjectId ProjectId { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        
        public ObjectId AnalysisItem { get; set; }
    }
    public class IW2S_Bing_AnalysisItem
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        public bool IsDefault { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class IW2S_Bing_AnalysisItemValue
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SeqNo { get; set; }
        
        public ObjectId IW2S_AnalysisItem { get; set; }
    }
    public class IW2S_Bing_PrjAnalysisItemDto
    {
        public string _id { get; set; }

        public string ProjectId { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public string UsrId { get; set; }

        public string AnalysisItem { get; set; }
    }

    public class IW2S_Bing_AnalysisItemDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public string UsrId { get; set; }
        public string ProjectId { get; set; }
        public bool IsDefault { get; set; }
        public List<IW2S_AnalysisItemValueDto> ItemValues { get;set; }
        
    }
    public class IW2S_Bing_AnalysisItemValueDto
    {
        public string _id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SeqNo { get; set; }

        public string IW2S_AnalysisItem { get; set; }
    }

    public class IW2S_Bing_PrjAnalysisItems
    {
        public ObjectId _id { get; set; }

        public ObjectId ProjectId { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public ObjectId UsrId { get; set; }

        public ObjectId AnalysisItem { get; set; }
    }

   

}
