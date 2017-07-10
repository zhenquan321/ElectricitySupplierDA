using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class linksdto
    {
        public int source { get; set; }
        public int target { get; set; }
        public int value { get; set; }
        public Guid Gid { get; set; }

    }
}