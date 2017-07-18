using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderMongo
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
        /// 购买用户
        /// </summary>
        public ObjectId UserId { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public List<ProductInOrder> ProductList { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double TotalPrice { get; set; }
        /// <summary>
        /// 是否支付
        /// </summary>
        public bool IsPay { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayAt { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PayType Type { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DelAt { get; set; }
        /// <summary>
        /// 本地订单号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 微信支付链接
        /// </summary>
        public string WxPayUrl { get; set; }
        /// <summary>
        /// 微信商户订单号
        /// </summary>
        public string WxTradeNo { get; set; }
    }

    /// <summary>
    /// 订单中的产品信息
    /// </summary>
    public class ProductInOrder
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int Num { get; set; }
    }

    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public List<ProductInOrderDto> ProductList { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double TotalPrice { get; set; }
        /// <summary>
        /// 是否支付
        /// </summary>
        public bool IsPay { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayAt { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 订单中的产品信息
    /// </summary>
    public class ProductInOrderDto
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int Num { get; set; }
    }
}
