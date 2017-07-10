using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_ProjectShare
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ProjectId { get; set; }
        public string SharedEmail { get; set; }
        public string Content { get; set; }
        //public string ShareCode { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DelAt { get; set; }
    }
    public class IW2S_ProjectShareDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ProjectId { get; set; }
        public string SharedEmail { get; set; }
        public string Content { get; set; }
        //public string ShareCode { get; set; }
        public bool IsDel { get; set; }
    }
}
