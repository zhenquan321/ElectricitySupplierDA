using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2S_UrlQRCode
    {
        public ObjectId _id { get; set; }
        public string Url { get; set; }
        public string QRCodeUrl { get; set; }
    }

    public class IW2S_UrlQRCodeDto
    {
        public string _id { get; set; }
        public string Url { get; set; }
        public string QRCodeUrl { get; set; }
    }
}
