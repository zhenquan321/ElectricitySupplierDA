using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian.Models
{
   


    public class ShareDto
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? Sender { get; set; }
        public string Label { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsDel { get; set; }
        public bool IsReaded { get; set; }
        public string Abstract { get; set; }

        public Guid ShareId { get; set; }

        public string SenderName { get; set; }
        public string SenderHead { get; set; }
        public string SenderGender { get; set; }

        public int ReplyerCount { get; set; }

        public int LoveCount { get; set; }

    }

}