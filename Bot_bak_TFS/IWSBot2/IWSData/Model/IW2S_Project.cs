using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_Project
    {
        public ObjectId _id { get; set; }

        public ObjectId UsrId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsDel { get; set; }
        public int KeywordCount { get; set; }
    }

    public class IW2S_ProjectDto
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int KeywordCount { get; set; }
    }
}
