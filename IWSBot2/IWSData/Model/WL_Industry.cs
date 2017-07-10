using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    public class WL_Industry
    {
        public ObjectId _id { get; set; }
        
        /// <summary>
        /// 企业数据更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 法院诉讼统计数
        /// </summary>
        public int lawsuitCount { get; set; }
        /// <summary>
        /// 法院公告统计数
        /// </summary>
        public string ctaCount { get; set; }
        /// <summary>
        /// 企业证书条目统计数
        /// </summary>
        public string certificateCount { get; set; }
        /// <summary>
        /// 企业著作权统计数
        /// </summary>
        public string copyrightCount { get; set; }
        /// <summary>
        /// 包含:被执行、年报、商标、失信等条目统计数
        /// </summary>
        public string CountInfo { get; set; }
        /// <summary>
        /// 投资人
        /// </summary>
        public string Partners { get; set; }
        /// <summary>
        /// 工商变更信息
        /// </summary>
        public string ChangeRecords { get; set; }
        /// <summary>
        /// 法定代表
        /// </summary>
        public string OperName { get; set; }
        /// <summary>
        /// 成立日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 营业期限时间起点
        /// </summary>
        public string TermStart { get; set; }
        /// <summary>
        /// 信息更新时间
        /// </summary>
        public string UpdatedDate { get; set; }
        /// <summary>
        /// 注册资本
        /// </summary>
        public string RegistCapi { get; set; }
        /// <summary>
        /// 公司曾用名
        /// </summary>
        public string OriginalName { get; set; }
        /// <summary>
        /// 业务范围
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 企业注册状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 工商注册号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 营业期限时间终点
        /// </summary>
        public string TeamEnd { get; set; }
        /// <summary>
        /// 企业性质
        /// </summary>
        public string EconKind { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 所属区域信息
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 企业联系方式
        /// </summary>
        public string ContactInfo { get; set; }
        /// <summary>
        /// 主要人员
        /// </summary>
        public string Employees { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 分支机构
        /// </summary>
        public string Branches { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string OrgNo { get; set; }
        /// <summary>
        /// 登记机关
        /// </summary>
        public string BelongOrg { get; set; }
        /// <summary>
        /// 发证日期
        /// </summary>
        public string CheckDate { get; set; }
        /// <summary>
        /// 数据库存储企业对应唯一键值
        /// </summary>
        public string Unique { get; set; }
    }
}
