using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    
        public enum KeywordType
        {
            /// <summary>
            /// 业务关键词
            /// </summary>
            Bussiness,
            /// <summary>
            /// 排除关键词
            /// </summary>
            Excluding,
            /// <summary>
            /// 搜索关键词
            /// </summary>
            Search,
        }

        public enum AppType
        {
            /// <summary>
            /// 仿冒监测
            /// </summary>
            Fake,
            /// <summary>
            /// 域名监测
            /// </summary>
            Domain,

        }

        public enum BaiduItemPart
        {
            /// <summary>
            /// 不相关
            /// </summary>
            None,
            /// <summary>
            /// 全文
            /// </summary>
            All,
            /// <summary>
            /// 标题
            /// </summary>
            Title,
            /// <summary>
            /// 摘要
            /// </summary>
            Abstract,
            /// <summary>
            /// 标题和摘要
            /// </summary>
            TitleAbstract,
            /// <summary>
            /// URL
            /// </summary>
            Url,
        }

        public enum MatchType
        {
            /// <summary>
            /// 业务关键词
            /// </summary>
            BussinessKeyword,
            /// <summary>
            /// 搜索关键词
            /// </summary>
            Searchkeyword,
        }

        public enum DataCleanStatus
        {
            /// <summary>
            /// 有嫌疑
            /// </summary>
            Suspected,
            /// <summary>
            /// 排除嫌疑
            /// </summary>
            Excluded,
            /// <summary>
            /// 确认侵权
            /// </summary>
            Confirmed,

        }

        public enum BotStatus
        {
            Blocked = 0,
            StructureChanged = 1,
            ServerError = 2,
            Removed = 3,
            Ok = 4,
        }

   
    
}
