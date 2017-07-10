using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_DomainCategory
    {
        public ObjectId _id { get; set; }

        public string Name { get; set; }

        public bool IsDel { get; set; }

        public ObjectId ParentId { get; set; }
        public string ParentName { get; set; }
        public ObjectId UsrId { get; set; }
        
        //public ObjectId ProjectId { get; set; }        
    }

    public class IW2S_DomainCategoryData
    {
        public ObjectId _id { get; set; }

        public string DomainName { get; set; }

        public ObjectId DomainCategoryId { get; set; }        

        public ObjectId UsrId { get; set; }

        //public ObjectId ProjectId { get; set; }        
    }

    public class IW2S_DomainCategoryDto
    {
        public string _id { get; set; }

        public string Name { get; set; }

        public long Num { get; set; }

        public string ParentId { get; set; }
        
        public string UsrId { get; set; }

        public string ProjectId { get; set; }        
    }

    public class IW2S_DomainCategoryDataDto
    {
        public string _id { get; set; }

        public string DomainName { get; set; }

        public string DomainCategoryId { get; set; }

        public string UsrId { get; set; }
    }
}
