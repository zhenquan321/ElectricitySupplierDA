using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DBHelper.Models.MongoDB;
using DBHelper;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Threading;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;



namespace WolongWeibo
{
    class TaskManager
    {
        /// <summary>
        /// 从卧龙接口取回任务数据
        /// </summary>
        public void Start()
        {
            var skip = 0;
            while (true)
            {
                try
                {
                    var wltask = GetWolongWeiboTasks(1, skip);
                    if (wltask == null)
                    {
                        WolongApiSecurity.Log("没有未完成的微博任务！");
                        skip = 0;
                        Thread.Sleep(2000);
                        continue;
                    }

                    //if (!wltask.CreatedAt.HasValue)
                    //{
                    //    WolongApiSecurity.Log("任务时间错误！");
                    //    Thread.Sleep(2000);
                    //    continue;
                    //}

                    double delay;
                    var utcNow = DateTime.UtcNow.AddHours(8.0);
                    if (!wltask.LastUpdateAt.HasValue)
                    {
                        var hours = (utcNow - wltask.CreatedAt).TotalHours;
                        delay = hours;
                    }
                    else
                    {
                        var hours = (utcNow - wltask.LastUpdateAt.Value).TotalHours;
                        delay = hours;
                    }


                    if (delay <= 1)
                    {
                        WolongApiSecurity.Log("没有需要更新的微博任务！");
                        skip++;
                        Thread.Sleep(2000);
                        continue;
                    }

                    // 开始更新微博任务数据
                    // 注意：卧龙接口使用UTC+8时间，注意时区转换！！
                    // 格式：2017-03-27 14:45:00
                    int page = 1;
                    while (true)
                    {
                        try
                        {
                            var para = new Dictionary<string, object>
                            {
                                {"task_id", wltask.TaskId},
                                {"count", 50},
                                {"page", page},
                                {"endtime", utcNow.ToString("yyyy-MM-dd hh:mm:ss")}
                            };

                            if (!wltask.LastUpdateAt.HasValue)
                            {
                                para.Add("starttime", wltask.CreatedAt.ToString("yyyy-MM-dd hh:mm:ss"));
                            }
                            else
                            {
                                para.Add("starttime", wltask.LastUpdateAt.Value.ToString("yyyy-MM-dd hh:mm:ss"));
                            }
                          
                            var url = WolongApiSecurity.FormatWolongUrl(para, null);
                            var wc = new WebClient();
                            var html = wc.DownloadString(url);
                            //var html = temp_readjson();                

                            var woLongData = (JObject)JsonConvert.DeserializeObject(html);
                            if (woLongData == null || woLongData["code"] == null)
                            {
                                WolongApiSecurity.Log("未返回数据");
                                break;
                            }
                            else if ((int)woLongData["code"] != 1200)
                            {
                                WolongApiSecurity.Log(woLongData["code"] + ":" + woLongData["data"]);
                                Thread.Sleep(2000);
                                continue;
                            }
                            SaveWolongWeiboData.SaveData(woLongData, wltask);
                            WolongApiSecurity.Log(string.Format("任务{0},{1}已保存{2}微博数据，Page {3}/{4}",
                                wltask.TaskId, wltask.Keyword, woLongData["data"]["item_list"].Count(), page, woLongData["data"]["total_page"].Value<int>()));
                            
                            if (!woLongData["data"]["item_list"].Any())
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            WolongApiSecurity.Log(ex.Message);
                        }
                        finally
                        {
                            page++;
                        }
                    }


                    var updatedate = DateTime.UtcNow.AddHours(13.0);

                    UpdateWLTaskTime(utcNow, wltask._id);
                    WolongApiSecurity.Log(string.Format("已完成任务：{0}，关键词：{1}", wltask.TaskId, wltask.Keyword));
                }
                catch (Exception ex)
                {
                    WolongApiSecurity.Log(ex.Message);
                    Thread.Sleep(2000);
                }
            }
        }

        public string temp_readjson()
        {
            var sr = new StreamReader("wb.txt");
            var s = sr.ReadToEnd();
            return s;
        }

        /// <summary>
        /// 向卧龙接口提交任务
        /// </summary>
        public void CommintTask()
        {
            while (true)
            {
                try
                {
                    var wltask = GetWolongWeiboTasks(0);
                    if (wltask == null)
                    {
                        WolongApiSecurity.Log("没有需要向卧龙提交的微博搜索任务！");
                        Thread.Sleep(2000);
                        continue;
                    }

                    var para = new Dictionary<string, object>();
                    para.Add("task_id", wltask.TaskId);
                    para.Add("keyword", wltask.Keyword);
                    var url = WolongApiSecurity.FormatWolongUrl(para, "add");

                    var wc = new WebClient();
                    var html = wc.DownloadString(url);
                    var woLongData = (JObject)JsonConvert.DeserializeObject(html);
                    if (woLongData == null || woLongData["code"] == null)
                    {
                        WolongApiSecurity.Log("未返回数据");
                        Thread.Sleep(2000);
                        continue;
                    }
                    else if ((int)woLongData["code"] != 1200)
                    {
                        WolongApiSecurity.Log(woLongData["code"] + ":" + woLongData["data"]);
                        Thread.Sleep(2000);
                        continue;
                    }

                    UpdateWolongWeiboTaskStatus(1, wltask._id);
                    WolongApiSecurity.Log(string.Format("已提交任务：{0}，任务：{1}，关键词：{2}", wltask.TaskId, wltask.Keyword, wltask.Keyword));
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    WolongApiSecurity.Log(ex.Message);
                    Thread.Sleep(2000);
                }
            }

        }

        /// <summary>
        /// 获取卧龙微博的任务
        /// </summary>        
        private IW2S_WB_BaiduCommend GetWolongWeiboTasks(int stauts, int? skip = null)
        {
            //var dbCol = MongoDBHelper.Instance.GetWolongWeiboTask();
            //if (skip.HasValue)
            //{
            //    var wltask = dbCol.Find(Builders<WolongWeiboTask>.Filter.Eq(x => x.Status, stauts)).SortBy(x => x.CreateAt).Skip(skip).FirstOrDefault();
            //    return wltask;
            //}
            //else
            //{
            //    var wltask = dbCol.Find(Builders<WolongWeiboTask>.Filter.Eq(x => x.Status, stauts)).FirstOrDefault();
            //    return wltask;
            //}


            try
            {

                var dt = DateTime.UtcNow.AddHours(8);

                if (skip.HasValue)
                {
                    var builder = Builders<IW2S_WB_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.IsRemoved, false);
                    filter &= builder.Eq(x => x.BotStatus, stauts);

                    var col = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
                    var result = col.Find(filter).SortBy(x => x.CreatedAt).Skip(skip).FirstOrDefault();
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}" + result.Keyword);
                    }

                    return result;
                }
                else
                {
                    //var wltask = dbCol.Find(Builders<WolongWeiboTask>.Filter.Eq(x => x.Status, stauts)).FirstOrDefault();
                    //return wltask;


                    var builder = Builders<IW2S_WB_BaiduCommend>.Filter;
                    var filter = builder.Eq(x => x.IsRemoved, false);
                    filter &= builder.Eq(x => x.BotStatus, stauts);

                    var col = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
                    var result = col.Find(filter).FirstOrDefault();
                    if (result != null)
                    {
                        Console.WriteLine("start to search {0}" + result.Keyword);
                    }

                    return result;



                }


              
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get searchkeyword error: {0}" + ex.Message);
            }

            return null;



        }

        private void UpdateWolongWeiboTaskStatus(int status, ObjectId id)
        {
            var dbCol = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
            dbCol.UpdateOne(Builders<IW2S_WB_BaiduCommend>.Filter.Eq(x => x._id, id),
                new UpdateDocument { { "$set", new QueryDocument { { "BotStatus", status } } } });
        }

        private void UpdateWLTaskTime(DateTime utcDate, ObjectId id)
        {
            var dbCol = MongoDBHelper.Instance.GetIW2S_WB_BaiduCommends();
            dbCol.UpdateOne(Builders<IW2S_WB_BaiduCommend>.Filter.Eq(x => x._id, id),
                new UpdateDocument { { "$set", new QueryDocument { { "LastUpdateAt", utcDate } } } });
        }
    }

    internal class WolongApiSecurity
    {
        private const string Secret = "nzvkrv85zx5fbe1wsqpvjuq8rqjvj0wx";   //卧龙提供Secret
        private const string AppKey = "3851133252"; //卧龙提供AppKey


        public static string FormatWolongUrl(Dictionary<string, object> paras, string resource)
        {
            paras.Add("app_key", AppKey);
            paras.Add("t", GetTimestamp());

            var sb = new StringBuilder();
            sb.Append(ConfigurationManager.AppSettings["wolong_weibo_api"].ToString());
            sb.Append(resource);

            string url;
            string sign = GenerateWolongSign(paras, Secret, out url);
            sb.Append(url);
            sb.Append("&sign=");
            sb.Append(sign);

            return sb.ToString();
        }

        /// <summary>
        /// 返回参数签名
        /// </summary>
        public static string GenerateWolongSign(Dictionary<string, object> param, string secret, out string url)
        {
            var paraStr = GetParaUrl(param, out url);
            var sign = Md5Encoding(secret + paraStr + secret);
            return sign;
        }

        /// <summary>
        /// 拼接参数
        /// </summary>
        public static string GetParaUrl(Dictionary<string, object> param, out string url)
        {
            var orderParas = param.OrderBy(x => x.Key);
            var sb = new StringBuilder();   // 带URL中的链接符
            var sb1 = new StringBuilder();  // 首尾相接，不带连接符
            var n = 0;

            foreach (var p in orderParas)
            {
                sb1.Append(UrlEncoding(p.Key)).Append(UrlEncoding(p.Value.ToString()));
                if (n == 0)
                {
                    sb.Append("?").Append(UrlEncoding(p.Key)).Append("=").Append(UrlEncoding(p.Value.ToString()));
                }
                else
                {
                    sb.Append("&").Append(UrlEncoding(p.Key)).Append("=").Append(UrlEncoding(p.Value.ToString()));
                }
                n++;
            }

            url = sb.ToString();
            return sb1.ToString();
        }

        /// <summary>
        /// URL 编码
        /// </summary>
        public static string UrlEncoding(string para)
        {
            var str = HttpUtility.UrlEncode(para);
            if (string.IsNullOrEmpty(str)) return null;
            str = Regex.Replace(str, @"\+", "%20");

            if (para.Contains("/"))
            {
                str = str.Replace("%2f", "/");
            }

            return !para.Equals(str) ? str.ToUpper() : str;
        }

        /// <summary>
        /// 计算参数拼接后的MD5值
        /// </summary>
        public static string Md5Encoding(string para)
        {
            var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(para);
            var md5Bytes = md5.ComputeHash(bytes);
            // var s = Convert.ToBase64String(md5Bytes);
            var s = string.Empty;
            foreach (var b in md5Bytes)
            {
                s += b.ToString("X2");
            }

            return s.ToLower();
        }

        public static int GetTimestamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            var cstName = (int)timeSpan.TotalSeconds;
            return cstName;
        }

        public static void Log(string message)
        {
            Console.WriteLine(DateTime.Now + ": " + message);
        }
    }
}
