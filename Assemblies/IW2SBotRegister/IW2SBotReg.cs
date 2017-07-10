using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Timer = System.Timers.Timer;

namespace IW2SBotReg
{
    public class IW2SBotRegHelper
    {
        /// <summary>
        /// 注册bot的方法，仅限mongo数据库
        /// 放在main中启动该方法即可
        /// </summary>
        public void Register(BotType bt)
        {
            if(_hasRun) return;
            
            _hostName = Dns.GetHostName();
            _botType = bt;
            _ipAddress = BotHelper.Instance.GetInternetIpAddress();
            _processId = Process.GetCurrentProcess().Id;
            _botId = BotHelper.Instance.GenerateBotId(bt).ToString().Replace("-", "");

            var col = MongoDBHelper.Instance.GetIW2S_BotRegister();
            
            var botregTh = new Thread(() =>
            {
                RegBot(col);
                var timer = new Timer { Interval = 60 * 1000, Enabled = true };
                // var timer = new Timer { Interval = 1000, Enabled = true };
                timer.Elapsed += (s, e) => RegBot(col);
            });

            var waithStatus = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var builder = Builders<IW2S_BotRegister>.Filter;
                        var bot = col.Find(builder.Eq(x => x.BotId, _botId)).FirstOrDefault();
                        AutoResetEvent.WaitOne();  // 等到，直到设置状态。
                        if (bot == null) continue;
                        var updBot = new UpdateDocument
                    {
                        {"$set", new QueryDocument{{"Status", _botStatus}}}
                    };
                        col.UpdateOne(new QueryDocument { { "_id", bot._id } }, updBot);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                   
                }
            });

            botregTh.Start();
            waithStatus.Start();
            _hasRun = true;
        }

        ~IW2SBotRegHelper()
        {
            var builder = Builders<IW2S_BotRegister>.Filter;
            var filter = builder.Eq(x => x.BotId,_botId);
            MongoDBHelper.Instance.GetIW2S_BotRegister().DeleteOne(filter);
        }

        /// <summary>
        /// 设置Bot状态
        /// </summary>
        /// <param name="status">Bot状态 0：空闲， 1：忙碌// 2：脱机， 3：异常</param>
        public void SentStatus(int status)
        {
            _botStatus = status;
            AutoResetEvent.Set();
        }

        /// <summary>
        /// 获取当前BotId
        /// </summary>
        /// <returns></returns>
        public string GetBotId()
        {
            return _botId;
        }

        private void RegBot(IMongoCollection<IW2S_BotRegister> col)
        {
            try
            {
                // 在mongodb中注册Bot
                var bu = Builders<IW2S_BotRegister>.Filter;
                if (_botStatus != 3)
                    Console.WriteLine("regist bot:{0}", _botId);
                var bot = col.Find(bu.Eq(x => x.BotId, _botId)).FirstOrDefault();
                if (bot == null)
                {
                    col.InsertOne(new IW2S_BotRegister
                    {
                        BotId = _botId,
                        RegTime = DateTime.UtcNow, //e.SignalTime.ToUniversalTime(),
                        IpAddress = _ipAddress,
                        HostName = _hostName,
                        ProcessId = _processId.ToString(CultureInfo.InvariantCulture),
                        Status = 0,
                        BotType = _botType.ToString()
                    });
                }
                else
                {
                    var updateBot = new UpdateDocument
                        {
                            //{"$set", new QueryDocument {{"RegTime", e.SignalTime.ToUniversalTime()}}}
                            {"$set", new QueryDocument {{"RegTime", DateTime.UtcNow}}}
                        };
                    col.UpdateOne(new QueryDocument { { "_id", bot._id } }, updateBot);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool _hasRun;
        private int _botStatus;
        private readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        private string _hostName;
        private string _ipAddress;
        private int _processId;
        private BotType _botType;
        private string _botId;
    }

    public enum BotType
    {
        Baidu, Sogou, WeChat, Weibo, BaiduImg, Taobao,WeiXin
    }

    public class BotHelper
    {
        public static readonly BotHelper Instance = new BotHelper();

        /// <summary>
        /// 如果失败返回127.0.0.1
        /// </summary>
        /// <returns></returns>
        public string GetInternetIpAddress()
        {
            var errTime = 0;

            while (true)
            {
                try
                {
                    var webRequest = WebRequest.Create("http://1212.ip138.com/ic.asp");
                    webRequest.Timeout = 10*1000;
                    var stream = webRequest.GetResponse().GetResponseStream();
                    if (stream == null)
                    {
                        return "127.0.0.1";
                    }
                    var streamReader = new StreamReader(stream, Encoding.Default);
                    var all = streamReader.ReadToEnd();

                    var ip = all.Split('[')[1].Split(']')[0];

                    streamReader.Close();
                    stream.Close();
                    return ip;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errTime++;
                    if (errTime == 4)
                    {
                        return "127.0.0.1";
                    }
                    Console.WriteLine("Retry: {0}", errTime);
                }
            }
        }

        private int GetTimestamp()
        {
            var time = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var ts = (int)time.TotalSeconds;
            return ts;
        }

        public Guid GenerateBotId()
        {
            var hostName = Dns.GetHostName();
            var processId = Process.GetCurrentProcess().Id;
            var ts = GetTimestamp();
            var idStr = hostName + processId + ts;
            var botId = IDHelper.GetGuid(idStr);
            return botId;
        }

        public Guid GenerateBotId(BotType botType)
        {
            var hostName = Dns.GetHostName();
            var processId = Process.GetCurrentProcess().Id;
            var ts = GetTimestamp();
            Thread.Sleep(100);
            var ts2 = GetTimestamp();
            var idStr = hostName + processId + ts + botType + ts2;
            var botId = IDHelper.GetGuid(idStr);
            return botId;
        }
    }

    public static class IDHelper
    {
        [ThreadStatic]
        private static MD5CryptoServiceProvider md5Service;

        private static MD5CryptoServiceProvider MD5
        {
            get
            {
                if (md5Service == null)
                    md5Service = new MD5CryptoServiceProvider();
                return md5Service;
            }
        }

        public static byte[] ComputeMD5ForString(string input)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(input);
            return MD5.ComputeHash(bytes);
        }

        public static Guid GetGuid(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Guid.Empty;
            byte[] bytes = ComputeMD5ForString(input);
            return new Guid(bytes);
        }
    }
}
