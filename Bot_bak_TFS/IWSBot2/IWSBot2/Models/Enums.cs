using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSBot.Models
{
    //public enum BotStatus
    //{
    //    Blocked,
    //    StructureChanged,
    //    ServerError,
    //    Removed,
    //    Ok,
    //}
     

    //public enum MatchType
    //{
    //    Title,
    //    Author,
    //    FingerPrint,
    //    BussinessKeyword,
    //}

    //public enum DataCleanStatus
    //{
    //    /// <summary>
    //    /// 有嫌疑
    //    /// </summary>
    //    Suspected,
    //    /// <summary>
    //    /// 排除嫌疑
    //    /// </summary>
    //    Excluded,
    //    /// <summary>
    //    /// 确认侵权
    //    /// </summary>
    //    Confirmed,
    //}

    //public enum FilterUseFor
    //{
    //    Query,
    //    Exclude,
    //    SetToConfirmed,
    //    SetScore,
    //} 




    public enum Roles
    {
        /// <summary>
        /// 客户
        /// </summary>
        Client,
        /// <summary>
        /// 客户经理
        /// </summary>
        ClientMng,
        /// <summary>
        /// 数据工程师
        /// </summary>
        DataEngineer,
        /// <summary>
        /// 系统管理员
        /// </summary>
        Admin,
    }

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
        /// 特征关键词
        /// </summary>
        Fingerprint,
    }
}
