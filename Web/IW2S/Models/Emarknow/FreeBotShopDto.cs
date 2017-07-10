using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    public class FreeBotShopDto
    {

        public string _id { get; set; }
        public Nullable<System.Guid> Shop_id { get; set; }
        public Nullable<System.Guid> Siteid { get; set; }
        public string SiteName { get; set; }
        public string ShopName { get; set; }
        public string Location { get; set; }
        public string Credit { get; set; } 
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastUpdatedAt { get; set; }
        public string taobaoId { get; set; }
        public string ContactUrl { get; set; }
        public Nullable<bool> IsAuthorized { get; set; }
        public Nullable<bool> IsTmall { get; set; }
        public Nullable<float> GoodCommentsRate { get; set; }
        public Nullable<float> ServiceScore { get; set; }
        public Nullable<float> IsCertified { get; set; }
        public Nullable<float> ItemCount { get; set; }
        public Nullable<int> BotStatus { get; set; }
        public Nullable<System.DateTime> LastBotAt { get; set; }
        public Nullable<bool> IsImportant { get; set; }
        public Nullable<bool> IsClosed { get; set; }
        public Nullable<System.DateTime> ClosedAt { get; set; }
        public Nullable<int> TotalComments { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string QQ { get; set; }
        public string Error { get; set; }
        public string taskId { get; set; }
        public string taskName { get; set; }
        public int ArbitraryPriceCount { get; set; }
        public int ChuanhuoCount { get; set; }
        public int FakeCount { get; set; }
        public Nullable<int> usrId { get; set; }
        public string UId { get; set; }
        public string ProjectId { get; set; }
    }
}
