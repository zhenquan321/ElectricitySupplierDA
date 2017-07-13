using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian.Models
{
    public class ShareUsersDto
    {
        public Guid ID { get; set; }
        public Guid SharedUser { get; set; }
        public Guid ShareId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReaded { get; set; }
    }
}