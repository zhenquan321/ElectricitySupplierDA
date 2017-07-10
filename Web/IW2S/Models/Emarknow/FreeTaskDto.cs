using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S.Models
{
    public class FreeTaskDto
    {
       
        public string _id { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> UsrId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string TimeAt { get; set; }
        public Nullable<System.DateTime> LastBotStartAt { get; set; } 
        public Nullable<System.DateTime> LastBotEndAt { get; set; }
        public Nullable<bool> IsStarted { get; set; }
        public Nullable<bool> IsBot { get; set; }
        public Nullable<int> BotIntervalHours { get; set; }
        public Nullable<System.DateTime> NextBotStartAt { get; set; }
        public string Error { get; set; }
        public string UId { get; set; }

        public double? MLP { get; set; }
        public Nullable<int> recordNum { get; set; }

        public int LinksNum { get; set; }
        public int ShopsNum { get; set; }
        public string ProjectId { get; set; }

        public bool drag { get; set; }


    }

}
