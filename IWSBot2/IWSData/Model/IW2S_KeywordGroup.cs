using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_KeywordGroup
    {
        public ObjectId _id { get; set; }
        public ObjectId CommendCategoryId { get; set; }
        public ObjectId ParentCategoryId { get; set; }
        public ObjectId BaiduCommendId { get; set; }
        public string BaiduCommend { get; set; }
        /// <summary>
        /// 1:百度直搜，2：百度热词
        /// </summary>
        public int GroupType { get; set; }

        public ObjectId UsrId { get; set; }

        public bool IsDel { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class IW2S_KeywordCategory
    {
        public ObjectId _id { get; set; }

        public string Name { get; set; }

        public bool IsDel { get; set; }

        public ObjectId ParentId { get; set; }

        /// <summary>
        /// 1:百度直搜，2：百度热词
        /// </summary>
        public int GroupType { get; set; }

        public ObjectId UsrId { get; set; }
        public int Weight { get; set; }
        public ObjectId KeywordId { get; set; }

        public ObjectId InfriLawCode { get; set; }

        public int KeywordTotal { get; set; }
        public ObjectId ProjectId { get; set; }
        public int ValLinkCount { get; set; }


        public int GroupNumber { get; set; }

    }

    public class IW2S_KeywordGroupDto
    {
        public string _id { get; set; }
        public string CommendCategoryId { get; set; }
        public string ParentCategoryId { get; set; }
        public string BaiduCommendId { get; set; }

        public string BaiduCommend { get; set; }
        /// <summary>
        /// 1:百度直搜，2：百度热词
        /// </summary>
        public int GroupType { get; set; }

        public string UsrId { get; set; }

        public bool IsDel { get; set; }

        public bool drag { get; set; }
        public string ProjectId { get; set; }
    }

    public class IW2S_KeywordCategoryDto
    {
        public string _id { get; set; }

        public string Name { get; set; }

        public bool IsDel { get; set; }

        public string ParentId { get; set; }

        public string ParentName { get; set; }

        /// <summary>
        /// 1:百度直搜，2：百度热词
        /// </summary>
        public int GroupType { get; set; }

        public string UsrId { get; set; }
        public int Weight { get; set; }
        public string KeywordId { get; set; }
        public string InfriLawCode { get; set; }
        public string InfriLawCodeStr { get; set; }

        public int KeywordTotal { get; set; }
        public string ProjectId { get; set; }

        public int BotStatus { get; set; }
        public int ValLinkCount { get; set; }
        public bool isselected { get; set; }
        public int GroupNumber { get; set; }
    }

    public class KeywordGroupModelDto
    {
        public List<IW2S_BaiduCommendDto> Selected { get; set; }
        public List<IW2S_BaiduCommendDto> UnSelected { get; set; }
    }
}
