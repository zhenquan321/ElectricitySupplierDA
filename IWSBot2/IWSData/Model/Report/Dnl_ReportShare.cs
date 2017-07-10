using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class Dnl_ReportShare
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ReportId { get; set; }
        public string SharedEmail { get; set; }
        public string Content { get; set; }
        //public string ShareCode { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DelAt { get; set; }
    }
    public class Dnl_ReportShareDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ReportId { get; set; }
        public string SharedEmail { get; set; }
        public string Content { get; set; }
        //public string ShareCode { get; set; }
        public bool IsDel { get; set; }
    }
}
