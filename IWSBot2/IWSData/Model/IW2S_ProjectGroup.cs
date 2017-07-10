using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_ProjectCategory
    {
        public ObjectId _id { get; set; }

        public ObjectId UsrId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDel { get; set; }
        public int ProjectCount { get; set; }
    }

    public class IW2S_ProjectCategoryDto
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<IW2S_ProjectGroupDto> ProjectList { get; set; }
    }

    public class IW2S_ProjectGroup
    {
        public ObjectId _id { get; set; }
        public ObjectId UsrId { get; set; }
        public ObjectId ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObjectId ProjectCategoryId { get; set; }
        public bool IsDel { get; set; }
    }

    public class IW2S_ProjectGroupDto
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectCategoryId { get; set; }
    }
}
