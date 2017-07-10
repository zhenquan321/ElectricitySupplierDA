using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public partial class website
    {

        public ObjectId _id { get; set; }
        public string Domain { get; set; }
        public string WebsiteType { get; set; }
        public string IDCType { get; set; }
        public Nullable<int> IP { get; set; }
        public Nullable<int> PV { get; set; }
        public Nullable<int> UV { get; set; }
        public string CompanyName { get; set; }
        public string RegistrantName { get; set; }
        public string RegistrantEmail { get; set; }
        public string RegistrantAddress { get; set; }
        public string RegistrantPhone { get; set; }
        public string SponsoringRegistrar { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string AdminPhone { get; set; }
        public string AdminEmail { get; set; }
        public string DNS { get; set; }
        public string ICPLicense { get; set; }
        //public Nullable<long> BaiduPages { get; set; }
        public string Whois_txt { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Guid UsrId { get; set; }
        public int LinkCount { get; set; }
        public string BDV { get; set; }
        public long ScoreSum { get; set; }

        public int LinkCount_Ilegal { get; set; }
        public int LinkCount_FakeAD { get; set; }
        public int LinkCount_SaleFake { get; set; }
        
    }
}
