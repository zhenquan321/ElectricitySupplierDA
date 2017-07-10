using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    public class FreeBotItem
    {
        public ObjectId _id { get; set; }
        public Nullable<System.Guid> ItemId { get; set; }
        public Nullable<System.Guid> ProductID { get; set; }
        public string ProductName { get; set; } 
        public Nullable<System.Guid> ShopID { get; set; }
        public string ShopName { get; set; }
        public string ItemName { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastUpdatedAt { get; set; }
        public Nullable<int> TotalBotPackages { get; set; }
        public string taobaoID { get; set; }
        public Nullable<int> Recent30DaysSoldNum { get; set; }
        public double? Price { get; set; }
        public string DetailUrl { get; set; }
        public Nullable<System.Guid> SiteId { get; set; }
        public string SiteName { get; set; }
        public Nullable<System.DateTime> LastBotAt { get; set; }
        public Nullable<int> QuantityInStock { get; set; }
        public Nullable<int> BotStatus { get; set; }
        public Nullable<int> TotalComments { get; set; }
        public Nullable<int> BadComments { get; set; }
        public Nullable<int> Reviews { get; set; }
        public Nullable<bool> IsExcluded { get; set; }
        public Nullable<System.DateTime> ClosedAt { get; set; }
        public string BotBrand { get; set; }
        public Nullable<int> Recent30DaysOrderedNum { get; set; }
        public Nullable<bool> IsTmall { get; set; }
        public Nullable<float> ItemGoodCommentPercent { get; set; }
        public string pic_url { get; set; }
        public ObjectId taskId { get; set; }
        public string taskName { get; set; }
        public string Error { get; set; }
        public double salesAmount { get; set; }

        public int DataCleaningState { get; set; }

        public Nullable<int> usrId { get; set; }

        public ObjectId UId { get; set; }

        public string Location { get; set; }
        public Nullable<int> PageNum { get; set; }
        public Nullable<int> Position { get; set; }

        public ObjectId ProjectId { get; set; }


    }
}
