using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class Dnl_IgnoreDomain
    {
        public ObjectId _id;
        public string Keyword { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public ObjectId UsrId { get; set; }

        
    }
}
