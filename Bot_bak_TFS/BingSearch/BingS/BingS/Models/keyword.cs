using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingS.Models
{
    public class keyword
    {

        public ObjectId _id;
        public string Txt { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Guid UsrId { get; set; }

        public byte BizType { get; set; }

        public int? Score { get; set; }

    }
}
