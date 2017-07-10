using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class FreeShopTimelineDto
    {

        public string _id { get; set; }
        public Nullable<System.Guid> SId { get; set; }
        public string ShopName { get; set; }
        public int? Position { get; set; }
        public int? TotalComments { get; set; }
        public int? Recent30DaysSoldNum { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedAt2 { get; set; }
        public string UId { get; set; }
        public string taskId { get; set; }
        public string ProjectId { get; set; }


    }
}