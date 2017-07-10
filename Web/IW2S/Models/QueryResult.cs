using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    
    public class QueryResult<T>
    {
        public List<T> Result { get; set; }
        public long Count { get; set; }
    }

    public class QueryResultView<T>
    {
        public List<T> Result { get; set; }
        public long Count { get; set; }
        public bool HasValue { get; set; }
        public string infriLawCode { get; set; }
    }

    public class QueryResultDomainCategory<T>
    {
        public List<T> Result { get; set; }
        public long Count { get; set; }
        public string DomainCategoryId { get; set; }
    }

    
}