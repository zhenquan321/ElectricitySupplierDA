using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_OperateLog
    {
        public ObjectId _id { get;set;}
        public ObjectId UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ShareOperateType { get; set; }
        public ObjectId ProjectId { get; set; }
        public int SiteSource { get; set; }
    }

    public class IW2S_OperateLogDto
    {
        public string _id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ShareOperateType { get; set; }
        public string ProjectId { get; set; }
        public string PictureSrc { get; set; }
        public string UserName { get; set; }
        public int SiteSource { get; set; }
    }

    
}
