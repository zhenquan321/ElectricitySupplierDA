using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian.Models
{
    public class UsrDto
    {
        public Guid ID { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string NickName { get; set; }
        public string UserPhone { get; set; }
        public string HeadIcon { get; set; }
        public string RoleId { get; set; }
        public string UserEmail { get; set; }
        public int status { get; set; }
        public string Error { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Token { get; set; }
        public string YZM { get; set; }
    }


    public class CreatedAtCount
    {
        public int count { get; set; }
        public int yesterdayCount { get; set; }
        public int meCount { get; set; }

        public int loveCount { get; set; }

        public int ReplyerCount { get; set; }

        //点赞状态
        public int Love { get; set; }

    }



    public class IDNameVo
    {
        public int Id { get; set; }
        public string SeriesName { get; set; }
        public List<IDNameDto> Names { get; set; }

    }

    public class IDNameDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public string LoginName { get; set; }
        public int Rid { get; set; }
        public int Sid { get; set; }

    }
    public class WebpagesSeries
    {
        public int Id { get; set; }
        public string SeriesName { get; set; }

    }

}