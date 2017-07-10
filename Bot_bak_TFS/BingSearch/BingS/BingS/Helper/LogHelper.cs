using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BingS
{
    public class LogerHelper
    {
       
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

}
