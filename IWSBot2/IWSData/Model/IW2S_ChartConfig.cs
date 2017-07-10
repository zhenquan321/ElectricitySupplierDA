using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_ChartConfig
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ProjectId { get; set; }
        public string Name { get; set; }
        public string Configuration { get; set; }
        public string SourceType { get; set; }
        public bool IsDel { get; set; }
        public int ChartType { get; set; }
    }

    public class IW2S_ChartConfigDto
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; }
        public string categoryId { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string percent { get; set; }
        public string topNum { get; set; }
        public string sumNum { get; set; }
        public string timeInterval { get; set; }
    }
}
