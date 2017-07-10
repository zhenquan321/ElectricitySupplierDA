using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Controllers
{
    public class FreeWebSite
    {

        public string _id { get; set; }
        public string SiteName { get; set; }

        public ObjectId ProjectId { get; set; }

    }
}