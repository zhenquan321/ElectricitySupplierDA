using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public class FreeBotItemVo
    {

        public string TimeKey { get; set; }
        public List<FreeBotItemDto> FreeBotItemList { get; set; }

    }


    public class FreeBotshopVo
    {
        public string TimeKey { get; set; }
        public List<FreebotKeyword> KeywordList { get; set; }
    }




    public class FreebotKeyword
    {
        public string KeywordId { get; set; }
        public List<FreeBotItemDto> FreeBotItemList { get; set; }
    }


}