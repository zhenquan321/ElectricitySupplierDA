using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace IW2SBotReg
{
    /// <summary>
    /// 供Bot注册使用，每分钟会注册一次，超过3分钟未注册
    /// 则会认为该Bot离线，超过10分钟则认为该Bot失联
    /// </summary>


    public class IW2S_BotRegister
    {
        public ObjectId _id { get; set; }

        public string BotId { get; set; }
        public DateTime RegTime { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string ProcessId { get; set; }

        /// <summary>
        /// Bot状态 0：空闲， 1：忙碌， 2：脱机， 3：异常
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Bot类型：Baidu, Sogou, WeChat, Weibo, BaiduImg, Taobao
        /// </summary>
        public string BotType { set; get; }
    }

    public class IW2S_BotRegisterDto
    {
        public string _id { get; set; }

        public string BotId { get; set; }
        public DateTime RegTime { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string ProcessId { get; set; }

        /// <summary>
        /// Bot状态 0：空闲， 1：忙碌， 2：脱机， 3：异常
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Bot类型：Baidu, Sogou, WeChat, Weibo, BaiduImg, Taobao
        /// </summary>
        public string BotType { set; get; }
    }
}
