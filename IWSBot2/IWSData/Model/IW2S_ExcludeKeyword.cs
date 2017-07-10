using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_ExcludeKeyword
    {
        public ObjectId _id;
        public string Keyword { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public ObjectId UsrId { get; set; }

        
    }
}
