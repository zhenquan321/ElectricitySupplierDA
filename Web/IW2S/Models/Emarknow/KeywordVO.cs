using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class KeywordVO
    {
        public string TimeKey { get; set; }
        public List<FreeTaskDto> TaskList { get; set; }
    }
}