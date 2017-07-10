using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class IW2SUser
    {
        public ObjectId _id { get; set; }
        public string LoginName { get; set; }
        public Nullable<System.Guid> LoginPwd { get; set; }
        
        public Nullable<System.Guid> UsrKey { get; set; }
        public string UsrEmail { get; set; }

        public Nullable<bool> IsEmailConfirmed { get; set; }
        public Nullable<bool> applicationState { get; set; }
        /// <summary>
        /// 登陆次数
        /// </summary>
        public int UsrNum { get; set; }
        /// <summary>
        /// 呢称
        /// </summary>
        public string Gender { get; set; }

        public string MobileNo { get; set; }
        /// <summary>
        /// 公司名 
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string PictureSrc { get; set; }
        /// <summary>
        /// 上次登陆时间
        /// </summary>
        public DateTime LastLoginAt { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserTypes UsrRole { get; set; }
        /// <summary>
        /// 第一次购买时间
        /// </summary>
        public DateTime PurchaseAt { get; set; }
        /// <summary>
        /// 限制项目数
        /// </summary>
        public int ProjectNum { get; set; }
        /// <summary>
        /// 限制关键词数
        /// </summary>
        public int KeywordNum { get; set; }
        /// <summary>
        /// 限制链接数
        /// </summary>
        public int LinkNum { get; set; }
        /// <summary>
        /// 限制简报数
        /// </summary>
        public int ReportNum { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 已签署合同
        /// </summary>
        public List<string> Contract { get; set; }
        /// <summary>
        /// 技术支持次数
        /// </summary>
        public int SupportNum { get; set; }
        /// <summary>
        /// 最后登陆时位置
        /// </summary>
        public string LastLoginLocation { get; set; }
        /// <summary>
        /// 数据分析次数
        /// </summary>
        public int DataAnalysisNum { get; set; }
    }

    /// <summary>
    /// 用户类型enum
    /// </summary>
    public enum UserTypes
    {
        
        /// <summary>
        /// 免费用户
        /// </summary>
        Free,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin,
        /// <summary>
        /// 工程师
        /// </summary>
        Engineer,
        /// <summary>
        /// 单项付费用户
        /// </summary>
        Single,
        /// <summary>
        /// 1级付费用户
        /// </summary>
        Vip1
    }
}
