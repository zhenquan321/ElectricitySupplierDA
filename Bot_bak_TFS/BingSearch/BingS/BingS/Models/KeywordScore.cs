using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingS.Models
{
    public class KeywordScore
    {
        public string Keyword { get; set; }
        public int? Score { get; set; }

        public byte BizType { get; set; }
    }
}
