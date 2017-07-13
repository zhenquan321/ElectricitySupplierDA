using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian.Models
{
    public class QueryResult<T>
    {

        public List<T> Result { get; set; }
        public long Count { get; set; }

    }
}