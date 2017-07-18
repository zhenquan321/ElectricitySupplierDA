using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IW2S.Helpers;
using IW2S.Models;
using IWSData.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;

namespace WxPayAPI
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify:Notify
    {
        public ResultNotify(Page page):base(page)
        {
        }

        public override void ProcessNotify()
        {
            string folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string path = folder + "WxTest.txt";
            string text = "微信支付";
            if (File.Exists(path))
            {
                File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.WriteLine(text);
                sw.Close();
            }
            else
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.WriteLine(text);
                sw.Close();
            }
            WxPayData notifyData = GetNotifyData();

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                text = "transaction_id是否存在";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.WriteLine(text);
                sw.Close();
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();

            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                text = "订单查询失败";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.WriteLine(text);
                sw.Close();
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            //查询订单成功
            else
            {
                string path2 = folder + "WxTest.txt";
                string text2 = "微信支付成功";
                if (File.Exists(path))
                {
                    File.Delete(path);

                }
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.WriteLine(text);
                sw.Close();

                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());

                try
                {
                    //更新订单支付信息
                    string orderId = res.GetValue("nonce_str").ToString();
                    System.IO.StreamWriter sw3 = new System.IO.StreamWriter(path);
                    sw3.WriteLine(text + orderId);
                    sw3.Close();
                    var builder = Builders<OrderMongo>.Filter;
                    var filter = builder.Eq(x => x._id, new ObjectId(orderId));
                    var col = MongoDBHelper.Instance.GetOrder();
                    var order = col.Find(filter).FirstOrDefault();
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "IsPay", true }, { "PayAt", DateTime.Now.AddHours(8) }, { "Type", PayType.WeiXin } } } };
                    col.UpdateOne(filter, update);
                }
                catch(Exception ex)
                {
                    string path3 = folder + "WxError.txt";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    System.IO.StreamWriter sw2 = new System.IO.StreamWriter(path);
                    sw2.WriteLine(ex.Message);
                    sw2.Close();
                }
                

                page.Response.Write(res.ToXml());
                page.Response.End();
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}