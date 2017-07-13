using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian.Models
{
    public class ShareReplyDto
    {

        public Guid ID { get; set; }
        public Guid ShareId { get; set; }
        public Guid? Replyer { get; set; }

        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public bool IsDel { get; set; }
        public int Love { get; set; }


        public string ReplyerName { get; set; }

        public string ReplyerHead {get;set;}
        public string ReplyerGender { get; set; }


    }
}