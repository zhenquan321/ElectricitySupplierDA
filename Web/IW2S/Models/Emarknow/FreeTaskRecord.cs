using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    public class FreeTaskRecord
    {
        public ObjectId _id { get; set; }

        public ObjectId Taskid { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> UsrId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> LastBotStartAt { get; set; }
        public Nullable<System.DateTime> LastBotEndAt { get; set; }
        public Nullable<bool> IsStarted { get; set; }
        public Nullable<bool> IsBot { get; set; }
        public string LanIP { get; set; }
        public int Dataquantity { get; set; }
        public string ServiceState { get; set; }
        public string Error { get; set; }

        public Nullable<System.Guid> SiteId { get; set; }
        public string SiteName { get; set; }
        public int LinksNum { get; set; }
        public int ShopsNum { get; set; }
        public ObjectId UId { get; set; }
        public double? MLP { get; set; }
        public ObjectId ProjectId { get; set; }
    }
}
