using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iw2sDataAnalysis.Models
{
     public class links
    {
         public ObjectId _id { get; set; }
         public int source { get; set; }
         public int target { get; set; }
         public int value { get; set; }
         public ObjectId KeywordId { get; set; }
         public ObjectId CommendCategoryId { get; set; }
         public ObjectId ProjectId { get; set; }

         public Guid Gid { get; set; }


    }


     public class WX_links
     {
         public ObjectId _id { get; set; }
         public int source { get; set; }
         public int target { get; set; }
         public int value { get; set; }

         public ObjectId KeywordId { get; set; }
         public ObjectId CommendCategoryId { get; set; }
         public ObjectId ProjectId { get; set; }
         public Guid Gid { get; set; }

     }
     public class SG_links
     {
         public ObjectId _id { get; set; }
         public int source { get; set; }
         public int target { get; set; }
         public int value { get; set; }

         public ObjectId KeywordId { get; set; }
         public ObjectId CommendCategoryId { get; set; }
         public ObjectId ProjectId { get; set; }
         public Guid Gid { get; set; }

     }
}
