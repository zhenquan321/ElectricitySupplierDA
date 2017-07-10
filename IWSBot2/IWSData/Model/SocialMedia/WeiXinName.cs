using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 微信公众号信息类
    /// </summary>
    public class WXNameMongo
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 清博数据中公众号对应Id
        /// </summary>
        public int GsId { get; set; }
        /// <summary>
        /// 公众号名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公众号呢称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 微信官方biz
        /// </summary>
        public string Biz { get; set; }
        /// <summary>
        /// 公众号类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 公众号二维码
        /// </summary>
        public string Qrcode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public string Vip { get; set; }
        /// <summary>
        /// 认证信息
        /// </summary>
        public string VipNote { get; set; }
        /// <summary>
        /// 所在国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 数据获取状态[1表示已获取，0表示未获取]
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 是否可用[1表示杉，0表示不可用]
        /// </summary>
        public int IsEnable { get; set; }
        /// <summary>
        /// 类目名称，未完善功能
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 更新状态，未完善功能
        /// </summary>
        public int UpdateStatus { get; set; }
        /// <summary>
        /// 未完善功能
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkName { get; set; }
        /// <summary>
        /// 联系人单位
        /// </summary>
        public string LinkUnit { get; set; }
        /// <summary>
        /// 联系人职位
        /// </summary>
        public string LinkPostion { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string LinkTel { get; set; }
        /// <summary>
        /// 联系人微信
        /// </summary>
        public string LinkWX { get; set; }
        /// <summary>
        /// 联系人QQ
        /// </summary>
        public string LinkQQ { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string LinkEmail { get; set; }
        /// <summary>
        /// 未完善功能
        /// </summary>
        public string Overseas { get; set; }
    }

    /// <summary>
    /// 微信公众号信息类
    /// </summary>
    public class WXNameDto
    {
        /// <summary>
        /// 公众号名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公众号呢称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 公众号二维码
        /// </summary>
        public string Qrcode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public string Vip { get; set; }
        /// <summary>
        /// 认证信息
        /// </summary>
        public string VipNote { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }
    }
}
