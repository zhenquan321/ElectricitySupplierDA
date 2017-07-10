using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_Report
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IconUrl { get; set; }
        public bool IsDel { get; set; }
    }

    public class IW2S_ReportDto
    {
        public string _id { get; set; }
        public string UsrId { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> SharerEmail { get; set; }
        public DateTime ShareTime { get; set; }
        public string IconUrl { get; set; }
    }
}
