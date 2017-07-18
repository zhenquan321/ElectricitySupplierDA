using AISSystem;
using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Text.RegularExpressions;
using System.Linq;
using WxPayAPI;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IW2S.Controllers
{
    public class PayController : ApiController
    {
        #region 产品管理

        /// <summary>
        /// 插入产品信息
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto InsertProduct(ProductPost post)
        {
            ResultDto result = new ResultDto();
            //判断该产品是否已添加
            var builder = Builders<ProductMongo>.Filter;
            var filter = builder.Eq(x => x.Name, post.name);
            filter &= builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetProduct();
            try
            {
                var query = col.Find(filter).FirstOrDefault();
                if (query != null)
                {
                    result.Message = "该产品已存在！";
                    return result;
                }
                var product = new ProductMongo
                {
                    Name = post.name,
                    Description = post.description,
                    Price = post.price,
                    CreatedAt = DateTime.Now.AddHours(8),
                };
                col.InsertOne(product);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto UpdateProduct(ProductPost post)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<ProductMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(post.id));
            filter &=builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetProduct();
            try
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "Name", post.name }, { "Description", post.description }, { "Price", post.price } } } };
                col.UpdateOne(filter, update);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="page">页数，从零开始</param>
        /// <param name="pagesize">一页内数据量</param>
        /// <returns></returns>
        [HttpGet]
        public List<ProductDto> GetProduct(int page,int pagesize)
        {
            var builder = Builders<ProductMongo>.Filter;
            var filter = builder.Eq(x => x.IsDel, false);
            var col = MongoDBHelper.Instance.GetProduct();
            try
            {
                var query = col.Find(filter).Skip(page * pagesize).Limit(pagesize).Project(x => new ProductDto
                {
                    Id = x._id.ToString(),
                    Description = x.Description,
                    Name = x.Name,
                    Price = x.Price
                }).ToList();
                return query;

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 删除产品信息
        /// </summary>
        /// <param name="productId">产品Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelProduct(string productId)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<ProductMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(productId));
            var col = MongoDBHelper.Instance.GetProduct();
            try
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", DateTime.Now.AddHours(8) } } } };
                col.UpdateOne(filter, update);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region 订单管理
        /// <summary>
        /// 插入订单信息
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public OrderInfo InsertOrder(OrderPost post)
        {
            var result = new OrderInfo();
            var userObjId = new ObjectId(post.userId);
            //获取最近一个订单信息
            var builderOrder = Builders<OrderMongo>.Filter;
            var filterOrder = builderOrder.Eq(x => x.UserId, userObjId);
            filterOrder &= builderOrder.Eq(x => x.IsDel, false);
            var colOrder = MongoDBHelper.Instance.GetOrder();
            try
            {
                //判断两个订单时间间隔，避免短时间频繁提交订单
                var queryOrder = colOrder.Find(filterOrder).SortByDescending(x => x.CreatedAt).FirstOrDefault();
                if (queryOrder != null)
                {
                    TimeSpan interval = DateTime.Now - queryOrder.CreatedAt;
                    if (interval.TotalSeconds < 10)
                        return null;
                }
                //根据产品Id获取产品信息
                var productObjIdList = post.productList.Select(x => new ObjectId(x.id)).ToList();
                var buiderProduct = Builders<ProductMongo>.Filter;
                var filterProduct = buiderProduct.In(x => x._id, productObjIdList);
                filterProduct &= buiderProduct.Eq(x => x.IsDel, false);
                var queryProduct = MongoDBHelper.Instance.GetProduct().Find(filterProduct).ToList();

                var productList = new List<ProductInOrder>();       //订单内产品信息列表
                var proDtoList = new List<ProductInOrderDto>();     //返回给前端的订单内产品信息列表
                foreach (var x in queryProduct)
                {
                    var product = new ProductInOrder
                    {
                        _id = x._id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price
                    };
                    product.Num = post.productList.Find(s => s.id == x._id.ToString()).Num;
                    productList.Add(product);

                    var proDto = new ProductInOrderDto
                    {
                        Id = x._id.ToString(),
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        Num=product.Num,
                    };
                    proDtoList.Add(proDto);
                }
                var ran = new Random();
                string tradeNo = string.Format("DT{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(9999));
                var order = new OrderMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now.AddHours(8),
                    ProductList = productList,
                    UserId = userObjId,
                    TradeNo = tradeNo
                };
                order.TotalPrice = productList.Select(x => x.Num * x.Price).Sum();
                colOrder.InsertOne(order);

                order.ProductList = productList;
                var orderDto = new OrderDto
                {
                    Id = order._id.ToString(),
                    ProductList = proDtoList,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    TradeNo = order.TradeNo
                };
                result.order = orderDto;

                //获取二维码
                result.qrcode = "/api/Pay/GetWxPayQcode?orderId={0}".FormatStr(orderDto.Id);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页数，从零开始</param>
        /// <param name="pagesize">一页内数据量</param>
        /// <param name="type">要查询的订单类型，0为所有，1为未完成，2为已完成</param>
        /// <returns></returns>
        [HttpGet]
        public QueryResult<OrderDto> GetOrder(string userId, int page, int pagesize,int type)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var result = new QueryResult<OrderDto>();
            var builder = Builders<OrderMongo>.Filter;
            var filter = builder.Eq(x => x.UserId, new ObjectId(userId));
            filter&=builder.Eq(x=>x.IsDel,false);
            //判断要查询的订单类型
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    filter &= builder.Eq(x => x.IsPay, false);
                    break;
                case 2:
                    filter &= builder.Eq(x => x.IsPay, true);
                    break;
                default:
                    break;
            }

            var col = MongoDBHelper.Instance.GetOrder();
            try
            {
                result.Count = col.Find(filter).Count();

                var query = col.Find(filter).Skip(page * pagesize).Limit(pagesize).ToList();
                var orderList = new List<OrderDto>();
                foreach (var x in query)
                {
                    var order = new OrderDto
                    {
                        Id = x._id.ToString(),
                        IsPay = x.IsPay,
                        PayAt = x.PayAt,
                        TotalPrice = x.TotalPrice,
                        TradeNo = x.TradeNo
                    };
                    var productList = new List<ProductInOrderDto>();
                    foreach (var y in x.ProductList)
                    {
                        var product = new ProductInOrderDto
                        {
                            Id = y._id.ToString(),
                            Description = y.Description,
                            Name = y.Name,
                            Price = y.Price,
                            Num = y.Num
                        };
                        productList.Add(product);
                    }
                    order.ProductList = productList;
                    orderList.Add(order);
                }

                result.Result = orderList;
                return result;
            }
            catch
            {
                return null;
            }
        }

        public bool GetOrderStatus(string orderId)
        {
            var builder = Builders<OrderMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(orderId));
            var col = MongoDBHelper.Instance.GetOrder();
            var order = col.Find(filter).FirstOrDefault();
            if (order == null)
            {
                return false;
            }
            return order.IsPay;
        }

        /// <summary>
        /// 删除订单信息
        /// </summary>
        /// <param name="productId">产品Id</param>
        /// <returns></returns>
        [HttpGet]
        public ResultDto DelOrder(string orderId)
        {
            ResultDto result = new ResultDto();
            var builder = Builders<OrderMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(orderId));
            var col = MongoDBHelper.Instance.GetOrder();
            try
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "IsDel", true }, { "DelAt", DateTime.Now.AddHours(8) } } } };
                col.UpdateOne(filter, update);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region 支付管理
        /// <summary>
        /// 获取微信支付二维码
        /// </summary>
        /// <param name="orderId">订单Id</param>
        [HttpGet]
        public HttpResponseMessage GetWxPayQcode(string orderId)
        {
            //获取订单信息
            var builder = Builders<OrderMongo>.Filter;
            var filter = builder.Eq(x => x._id, new ObjectId(orderId));
            filter &= builder.Eq(x => x.IsPay, false);
            var col = MongoDBHelper.Instance.GetOrder();
            var order = col.Find(filter).FirstOrDefault();
            if (order == null)
            {
                return null;
            }

            DateTime payAt = order.PayAt;
            TimeSpan ts = DateTime.Now - payAt;     //上次支付到当前时间间隔
            string payUrl = order.WxPayUrl;
            string wxTradeNo = WxPayApi.GenerateOutTradeNo();

            //判断是使用原有的支付链接还是需要重新生成支付链接
            if (string.IsNullOrEmpty(order.WxPayUrl) || ts.TotalMinutes > 10)
            {
                //生成微信支付链接
                Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");

                double fee = order.TotalPrice * 100;

                WxPayData data = new WxPayData();
                data.SetValue("body", "DWC服务费");//商品描述
                data.SetValue("attach", "北京");//附加数据
                data.SetValue("out_trade_no", wxTradeNo);//商户订单号
                data.SetValue("total_fee", fee.ToString());//总金额
                data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
                // data.SetValue("goods_tag", "jjj");//商品标记
                data.SetValue("trade_type", "NATIVE");//交易类型
                data.SetValue("product_id", orderId);//商品ID

                WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
                payUrl = result.GetValue("code_url").ToString();//获得统一下单接口返回的支付链接
                Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + payUrl);
            }

            //生成二维码
            var qrcode = ZXingQrcodeHelper.GetQrBitmap(payUrl, null);
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            MemoryStream stream = new MemoryStream(qrcode);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

            //更新订单中信息
            var update = new UpdateDocument { { "$set", new QueryDocument { { "WxTradeNo", wxTradeNo }, { "WxPayUrl", payUrl }, { "PayAt", DateTime.Now.AddHours(8) } } } };
            col.UpdateOne(filter, update);

            return response;
        }

        [HttpGet]
        public string Run(string out_trade_no)
        {
            Log.Info("OrderQuery", "OrderQuery is processing...");

            WxPayData data = new WxPayData();
                data.SetValue("out_trade_no", out_trade_no);

            WxPayData result = WxPayApi.OrderQuery(data);//提交订单查询请求给API，接收返回数据

            Log.Info("OrderQuery", "OrderQuery process complete, result : " + result.ToXml());
            return result.ToPrintStr();
        }
        #endregion
    }
}