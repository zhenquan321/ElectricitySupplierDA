using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class Dnl_EntityTreeMapping
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ObjectId EntityId { get; set; }
        public ObjectId ProjectId { get; set; }
    }

    public class Dnl_EntityTreeMappingDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EntityName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
