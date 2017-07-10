using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class RectangularTree
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ValLinkCount { get; set; }
        public string PId { get; set; }
        public bool IsNode { get; set; }
    }
}