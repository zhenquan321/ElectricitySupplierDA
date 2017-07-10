using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    public class FreeShopTimeline
    {

        public ObjectId _id { get; set; }
        public Nullable<System.Guid> SId { get; set; }
        public string ShopName { get; set; }

        public int? Position { get; set; }
        public int? TotalComments { get; set; }
        public int? Recent30DaysSoldNum { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedAt2 { get; set; }
        public ObjectId UId { get; set; }
        public ObjectId taskId { get; set; }
        public ObjectId ProjectId { get; set; }

    }
}
