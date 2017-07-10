using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class ipfiles
    {

        public ObjectId _id { get; set; }
        public byte[] bytes { get; set; }
        public string content_type { get; set; }
        public Nullable<int> size { get; set; }
        public string file_name { get; set; }

    }
}