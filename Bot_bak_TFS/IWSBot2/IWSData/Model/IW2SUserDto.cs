using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2SUserDto
    {
        public string _id { get; set; }
        public string LoginName { get; set; }
        public Nullable<System.Guid> LoginPwd { get; set; }
        public Nullable<int> UsrRole { get; set; }
        public Nullable<System.Guid> UsrKey { get; set; }
        public string UsrEmail { get; set; }

        public Nullable<bool> IsEmailConfirmed { get; set; }
        public Nullable<bool> applicationState { get; set; }

        public string Token { get; set; }

        public int UsrNum { get; set; }
        public string Error { get; set; }


        public string Gender { get; set; }

        public string MobileNo { get; set; }

        public string Remarks { get; set; }

        public string PictureSrc { get; set; }

        public Nullable<DateTime> CreatedAt { get; set; }

    }
}
