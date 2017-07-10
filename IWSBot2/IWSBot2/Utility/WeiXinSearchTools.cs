using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLib;
using AISSystem;
using System.Threading;
using IWSData.Model;
using IWSBot.Utility;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using IWSBot2.Helper;
using GSDataAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IWSBot.Utility
{
    public class WeiXinSearchTools
    {
        public WeiXinSearchTools()
        {
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
        }
        /// <summary>
        /// 网页相关类
        /// </summary>
        WebHelperNoCookieProxy Proxy = new WebHelperNoCookieProxy();

        /// <summary>
        /// 搜索开始日期
        /// </summary>
        public DateTime StartDate;
        /// <summary>
        /// 搜索结束日期
        /// </summary>
        public DateTime EndDate;

        public delegate void UpdateStatus();
        /// <summary>
        /// 设置Bot为工作状态，供控制台查看
        /// </summary>
        public event UpdateStatus SetBusy;
        /// <summary>
        /// 设置Bot为空闲状态，供控制台查看
        /// </summary>
        public event UpdateStatus SetReady;
        /// <summary>
        /// 设置Bot为错误状态，供控制台查看
        /// </summary>
        public event UpdateStatus SetError;

        public void Run()
        {
            while (true)
            {
                Random r = new Random();
                //获取搜索关键词信息
                var task = GetSearchTask();
                //若没有需要搜索的关键词，进行休眠
                if (task == null)
                {
                    SetReady();
                    //Commons.Log("无搜索关键词！休眠一段时间");
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }

                int linkNum = 0;        //搜索链接数
                try
                {
                    SetBusy();
                    var botId = Utility.GenerateBotId().ToString().Replace("-", "");                //获取BotId
                    int botInterval = task.BotIntervalHours == 0 ? 7 * 24 : task.BotIntervalHours;        //获取搜索间隔
                    //更新关键词状态为正在搜索
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "WXBotStatus", 1 } } } };
                    var result = MongoDBHelper.Instance.GetMediaKeyword().UpdateOne(new QueryDocument { { "_id", task._id } }, update);
                    //搜索关键词
                    linkNum = QueryLink(task);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Commons.Log("微信搜索错误信息:{0},堆:{1}".FormatStr(ex.Message, ex.StackTrace));
                        ex = ex.InnerException;
                        SetError();
                        Console.ReadLine();
                    }
                }
                try
                {
                    //更新关键词状态为已完成
                    var update = new UpdateDocument { { "$set", new QueryDocument { { "WXLastBotAt", DateTime.UtcNow.AddHours(8) }, { "WXBotStatus", 2 } } } };
                    var colKey = MongoDBHelper.Instance.GetMediaKeyword();
                    var result = colKey.UpdateOne(new QueryDocument { { "_id", task._id } }, update);
                    //更新关键词搜索链接数
                    linkNum += task.WXLinkNum;
                    update = new UpdateDocument { { "$set", new QueryDocument { { "WXLinkNum", linkNum } } } };

                    colKey.UpdateOne(new QueryDocument { { "_id", task._id } }, update);
                }
                catch (Exception ex)
                {
                    Commons.Log("关键词信息更新错误 - " + ex.Message);
                    SetError();
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// 获取要搜索的关键词信息
        /// </summary>
        /// <returns></returns>
        MediaKeywordMongo GetSearchTask()
        {
            try
            {
                //查询未被搜索的关键词
                var dtNow = DateTime.UtcNow.AddHours(8);
                var builder = Builders<MediaKeywordMongo>.Filter; ;
                var filter = builder.Eq(x => x.WXBotStatus, 0);

                var col = MongoDBHelper.Instance.GetMediaKeyword();
                var result = col.Find(filter).SortByDescending(x => x.CreatedAt).FirstOrDefault();

                //如果没有未被搜索的关键词，则查询需要重新搜索的关键词
                if (result == null)
                {
                    ////获取最早被搜索的关键词
                    //filter = builder.Eq(x => x.BotStatus_Baidu, 2);
                    //var keyword = col.Find(filter).SortBy(x => x.LastBotEndAt_Baidu).FirstOrDefault();
                    ////判断是否已到需被重新搜索的时间
                    //DateTime nextBot = keyword.LastBotEndAt_Baidu.AddHours(keyword.BotIntervalHours);
                    //if (nextBot < dtNow)
                    //{
                    //    Console.WriteLine("开始搜索： {0}".FormatStr(keyword.Keyword));
                    //    return keyword;
                    //}
                }
                if (result != null)
                {
                    Console.WriteLine("开始搜索： {0}".FormatStr(result.Keyword));
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取搜索关键词出错： {0}".FormatStr(ex.Message));
            }

            return null;

        }

        /// <summary>
        /// 微信搜索关键词
        /// </summary>
        /// <param name="task">关键词</param>
        public int QueryLink(MediaKeywordMongo task)
        {
            //搜索关键词
            string baseUrl = "http://open.gsdata.cn/";
            string linkUrl = baseUrl + "api/wx/opensearchapi/content_keyword_search";       //关键词获取文章接口地址
            string appid = "JVEvKn7ghegw984neooX";
            string appkey = "n0TWOaX9gta1dpfVF07hpkKr2";
            GSDataSDK api = new GSDataSDK(appid, appkey);           //接口函数
            int computeNum = 0;
            int linkNum = 0;        //微信已搜索链接数
            int topNum = 0;         //微信链接总数
            while (true)
            {
                if (computeNum >= 2000)
                {
                    Commons.Log("防止访问过于频繁，休眠10分钟！");
                    computeNum = 0;
                    Thread.Sleep(10 * 60 * 1000);
                }
                Dictionary<string, object> postData = new Dictionary<string, object>();                 //post参数
                postData.Add("keyword", task.Keyword);
                postData.Add("start", linkNum);
                postData.Add("num", 10);
                postData.Add("startdate", StartDate.ToString("yyyy-MM-dd"));
                postData.Add("enddate", EndDate.ToString("yyyy-MM-dd"));
                postData.Add("sortname", "likenum");
                postData.Add("sort", "desc");

                string LinkStr = api.Call(postData, linkUrl);       //调用接口，获取返回数据
                int testNum = 0;
                //如果返回值为空，重试3次
                while (LinkStr == null)
                {
                    if (testNum == 3)
                        break;
                    LinkStr = api.Call(postData, linkUrl);
                    testNum++;
                }

                //解析Json字符串
                JObject linkJson = new JObject();
                try
                {
                    linkJson = JObject.Parse(LinkStr);
                }
                catch(Exception ex)
                {
                    SetError();
                    Commons.Log("微信搜索出错：" + ex.Message);
                    Commons.Log("是否继续重试？（Y/N）");
                    string code = Console.ReadLine().ToUpper();
                    if (code != "Y" && code != "N")
                    {
                        continue;
                    }
                    else
                    {
                        if (code == "Y")
                            continue;
                        else
                            break;
                    }
                }
                if (linkJson.Property("returnCode") != null && linkJson.Property("returnCode").Value.ToString() == "1001")
                {
                    Commons.Log("链接" + linkJson.Property("returnMsg").Value);
                    JObject returnData = (JObject)linkJson["returnData"];
                    topNum = Convert.ToInt32(returnData.Property("total").Value);         //链接总数
                    JArray items = (JArray)returnData["items"];
                    Commons.Log("当前搜索状态 - [{0}/{1}]".FormatStr(linkNum, topNum));
                    computeNum += items.Count;
                    for (int i = 0; i < items.Count; i++)
                    {
                        //获取常用链接信息
                        JObject item = (JObject)items[i];
                        if (item == null)
                            break;
                        var linkMain = new WXLinkMainMongo()
                        {
                            _id = ObjectId.GenerateNewId(),
                            CreatedAt = DateTime.Now.AddHours(8),
                            Keyword = task.Keyword,
                            KeywordId = task._id.ToString(),
                        };
                        if (item.Property("name") != null)
                            linkMain.Nickname = item.Property("name").Value.ToString();
                        if (item.Property("wx_name") != null)
                            linkMain.Name = item.Property("wx_name").Value.ToString();
                        if (item.Property("title") != null)
                            linkMain.Title = item.Property("title").Value.ToString();
                        if (item.Property("content") != null)
                            linkMain.Description = item.Property("content").Value.ToString();
                        if (item.Property("url") != null)
                            linkMain.Url = item.Property("url").Value.ToString();
                        if (item.Property("picurl") != null)
                            linkMain.PicUrl = item.Property("picurl").Value.ToString();

                        int readNum = 0;
                        if (item.Property("readnum") != null)
                            readNum = Convert.ToInt32(item.Property("readnum").Value);
                        int realReadNum = 0;
                        if (item.Property("realreadnum") != null)
                            realReadNum = Convert.ToInt32(item.Property("realreadnum").Value);
                        int likeNum = 0;
                        if (item.Property("likenum") != null)
                            likeNum = Convert.ToInt32(item.Property("likenum").Value);
                        int readNumPM = 0;
                        if (item.Property("readnum_pm") != null)
                            readNumPM = Convert.ToInt32(item.Property("readnum_pm").Value);
                        int realReadNumPM = 0;
                        if (item.Property("realreadnum_pm") != null)
                            realReadNumPM = Convert.ToInt32(item.Property("realreadnum_pm").Value);
                        int likeNumPM = 0;
                        if (item.Property("likenum_pm") != null)
                            likeNumPM = Convert.ToInt32(item.Property("likenum_pm").Value);
                        int readNumWeek = 0;
                        if (item.Property("readnum_week") != null)
                            readNumWeek = Convert.ToInt32(item.Property("readnum_week").Value);
                        int realReadNumWeek = 0;
                        if (item.Property("realreadnum_week") != null)
                            realReadNumWeek = Convert.ToInt32(item.Property("realreadnum_week").Value);
                        int likeNumWeek = 0;
                        if (item.Property("likenum_week") != null)
                            likeNumWeek = Convert.ToInt32(item.Property("likenum_week").Value);
                        int readNumNewest = 0;
                        if (item.Property("readnum_newest") != null)
                            readNumNewest = Convert.ToInt32(item.Property("readnum_newest").Value);
                        int likeNumNewest = 0;
                        if (item.Property("likenum_newest") != null)
                            likeNumNewest = Convert.ToInt32(item.Property("likenum_newest").Value);
                        //比较大小，获取最大的阅读数和点赞数
                        var readNums = new List<int> { readNum, readNumPM, readNumWeek, readNumNewest, realReadNum, realReadNumPM, realReadNumWeek };
                        linkMain.ReadNum = readNums.Max();
                        var likeNums = new List<int> { likeNum, likeNumPM, likeNumWeek, likeNumNewest };
                        linkMain.LikeNum = likeNums.Max();

                        DateTime dt = new DateTime();
                        if (item.Property("posttime") != null)
                            DateTime.TryParse(item.Property("posttime").Value.ToString(), out dt);
                        linkMain.PostTime = dt.AddHours(8);
                        dt = DateTime.MinValue;

                        //获取非常用链接信息
                        var linkOther = new WXLinkOtherMongo
                        {
                            LinkId = linkMain._id,
                            KeywordId = linkMain.KeywordId
                        };
                        if (item.Property("status") != null)
                            linkOther.Status = Convert.ToInt32(item.Property("status").Value);
                        if (item.Property("top") != null)
                            linkOther.Top = Convert.ToInt32(item.Property("top").Value);
                        if (item.Property("type") != null)
                            linkOther.Type = Convert.ToInt32(item.Property("type").Value);
                        
                        if (item.Property("sourceurl") != null)
                            linkOther.SourceUrl = item.Property("sourceurl").Value.ToString();
                        if (item.Property("author") != null)
                            linkOther.Author = item.Property("author").Value.ToString();
                        if (item.Property("copyright") != null)
                            linkOther.Copyright = item.Property("copyright").Value.ToString();

                        if (item.Property("add_time") != null)
                            DateTime.TryParse(item.Property("add_time").Value.ToString(), out dt);
                        linkOther.AddTime = dt.AddHours(8);
                        dt = DateTime.MinValue;
                        DateTime getTime = new DateTime();
                        if (item.Property("get_time") != null)
                            DateTime.TryParse(item.Property("get_time").Value.ToString(), out getTime);
                        DateTime getTimePM = new DateTime();
                        if (item.Property("get_time_pm") != null)
                            DateTime.TryParse(item.Property("get_time_pm").Value.ToString(), out getTimePM);
                        DateTime getTimeNewest = new DateTime();
                        if (item.Property("get_time_newest") != null)
                            DateTime.TryParse(item.Property("get_time_newest").Value.ToString(), out getTimeNewest);
                        //比较大小，获取最新的获取时间
                        var getTimes = new List<DateTime> { getTime, getTimeNewest, getTimePM };
                        linkOther.GetTime = getTimes.Max().AddHours(8);

                        #region 获取正文
                        var linkContent = new WXLinkContentMongo
                        {
                            LinkId = linkMain._id,
                            KeywordId = linkMain.KeywordId
                        };
                        string html = Proxy.GetFastHtmlWithProxyIpAndARE(linkMain.Url, "utf-8");
                        if (!string.IsNullOrEmpty(html))
                        {
                            linkContent.Html = html;
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(html);
                            HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@id=\"js_content\"]");     //定位正文位置
                            if (node != null)
                            {
                                string content =  Commons.RemoveTextTag(node.InnerHtml);
                                linkContent.Content = content;
                                linkMain.ContentLen = content.Length;
                            }
                            else
                            {
                                linkContent.Content = "";
                                //if (html.Contains("该内容已被发布者删除") || html.Contains("该公众号已迁移"))
                                //{
                                    linkMain.IsDelByAu = true;
                                //}
                            }
                        }
                        #endregion

                        #region 公众号信息获取
                        var builderName = Builders<WXNameMongo>.Filter;
                        var colName = MongoDBHelper.Instance.GetWXName();
                        //检查本公众号是否已查询过
                        var filterName = builderName.Eq(x=>x.Name,linkMain.Name);
                        var queryName = colName.Find(filterName).FirstOrDefault();
                        ObjectId nameObjId = new ObjectId();
                        if (queryName != null)
                        {
                            nameObjId = queryName._id;
                        }
                        else
                        {
                            string nameUrl = baseUrl + "api/wx/wxapi/nickname_one";       //公众号信息接口
                            postData.Clear();
                            postData.Add("wx_name", linkMain.Name);
                            string nameStr = api.Call(postData, nameUrl);       //调用接口，获取返回数据
                            testNum = 0;
                            //如果返回值为空，重试3次
                            while (nameStr == null)
                            {
                                if (testNum == 3)
                                    break;
                                nameStr = api.Call(postData, nameUrl);
                                testNum++;
                            }

                            //解析Json字符串
                            JObject nameJson = new JObject();
                            try
                            {
                                nameJson = JObject.Parse(nameStr);
                            }
                            catch (Exception ex)
                            {
                                Commons.Log("微信搜索出错：" + ex.Message);
                                Commons.Log("是否继续重试？（Y/N）");
                                string code = Console.ReadLine().ToUpper();
                                if (code != "Y" && code != "N")
                                {
                                    continue;
                                }
                                else
                                {
                                    if (code == "Y")
                                        continue;
                                    else
                                        break;
                                }
                            }
                            if (nameJson.Property("returnCode") != null && nameJson.Property("returnCode").Value.ToString() == "1001")
                            {
                                Commons.Log("公众号信息" + nameJson.Property("returnMsg").Value);
                                JObject nameReturn = (JObject)nameJson["returnData"];
                                //获取常用链接信息
                                if (nameReturn == null)
                                {
                                    computeNum++;
                                    continue;
                                }
                                var nameInfo = new WXNameMongo()
                                {
                                    _id = ObjectId.GenerateNewId(),
                                    CreatedAt = DateTime.Now.AddHours(8),
                                };
                                nameObjId = nameInfo._id;
                                if (nameReturn.Property("id") != null)
                                    nameInfo.GsId = Convert.ToInt32(nameReturn.Property("id").Value);
                                if (nameReturn.Property("wx_name") != null)
                                    nameInfo.Name = nameReturn.Property("wx_name").Value.ToString();
                                if (nameReturn.Property("wx_nickname") != null)
                                    nameInfo.Nickname = nameReturn.Property("wx_nickname").Value.ToString();
                                if (nameReturn.Property("wx_type") != null)
                                    nameInfo.Type = nameReturn.Property("wx_type").Value.ToString();
                                if (nameReturn.Property("wx_biz") != null)
                                    nameInfo.Biz = nameReturn.Property("wx_biz").Value.ToString();
                                if (nameReturn.Property("wx_qrcode") != null)
                                    nameInfo.Qrcode = nameReturn.Property("wx_qrcode").Value.ToString();
                                if (nameReturn.Property("wx_note") != null)
                                    nameInfo.Description = nameReturn.Property("wx_note").Value.ToString();
                                if (nameReturn.Property("wx_vip") != null)
                                    nameInfo.Vip = nameReturn.Property("wx_vip").Value.ToString();
                                if (nameReturn.Property("wx_vip_note") != null)
                                    nameInfo.VipNote = nameReturn.Property("wx_vip_note").Value.ToString();
                                if (nameReturn.Property("wx_country") != null)
                                    nameInfo.Country = nameReturn.Property("wx_country").Value.ToString();
                                if (nameReturn.Property("wx_province") != null)
                                    nameInfo.Province = nameReturn.Property("wx_province").Value.ToString();
                                if (nameReturn.Property("wx_city") != null)
                                    nameInfo.City = nameReturn.Property("wx_city").Value.ToString();
                                if (nameReturn.Property("status") != null)
                                    nameInfo.Status = Convert.ToInt32(nameReturn.Property("status").Value);
                                if (nameReturn.Property("isenable") != null)
                                    nameInfo.IsEnable = Convert.ToInt32(nameReturn.Property("isenable").Value);
                                if (nameReturn.Property("category_id") != null)
                                    nameInfo.CategoryId = nameReturn.Property("category_id").Value.ToString();
                                if (nameReturn.Property("update_status") != null)
                                    nameInfo.UpdateStatus = Convert.ToInt32(nameReturn.Property("update_status").Value);
                                if (nameReturn.Property("wx_district") != null)
                                    nameInfo.District = nameReturn.Property("wx_district").Value.ToString();
                                if (nameReturn.Property("overseas") != null)
                                    nameInfo.Overseas = nameReturn.Property("overseas").Value.ToString();
                                if (nameReturn.Property("link_name") != null)
                                    nameInfo.LinkName = nameReturn.Property("link_name").Value.ToString();
                                if (nameReturn.Property("link_unit") != null)
                                    nameInfo.LinkUnit = nameReturn.Property("link_unit").Value.ToString();
                                if (nameReturn.Property("link_position") != null)
                                    nameInfo.LinkPostion = nameReturn.Property("link_position").Value.ToString();
                                if (nameReturn.Property("link_tel") != null)
                                    nameInfo.LinkTel = nameReturn.Property("link_tel").Value.ToString();
                                if (nameReturn.Property("link_wx") != null)
                                    nameInfo.LinkWX = nameReturn.Property("link_wx").Value.ToString();
                                if (nameReturn.Property("link_qq") != null)
                                    nameInfo.LinkQQ = nameReturn.Property("link_qq").Value.ToString();
                                if (nameReturn.Property("link_email") != null)
                                    nameInfo.LinkEmail = nameReturn.Property("link_email").Value.ToString();

                                try
                                {
                                    colName.InsertOne(nameInfo);
                                    Commons.Log("成功计算公众号 - " + nameInfo.Nickname);
                                }
                                catch (Exception ex)
                                {
                                    Commons.Log(ex.Message);
                                    Console.ReadLine();
                                }

                            }
                        }
                        linkMain.NameId = nameObjId;

                        #endregion


                        #region 获取评论
                        ////获取文章评论
                        //string commentUrl = baseUrl + "api/wx/wxapi2/wx_comment_by_url";          //获取评论网页源码接口
                        //postData.Clear();
                        //postData.Add("url", link.Url);
                        //string commentStr = api.Call(postData, commentUrl);       //调用接口，获取返回数据

                        ////解析Json字符串
                        //JObject commentJson = JObject.Parse(commentStr);
                        //var linkComments = new List<WeiXinLinkCommentMongo>();
                        //if (commentJson.Property("returnCode") != null && commentJson.Property("returnCode").Value.ToString() == "1001")
                        //{
                        //    Commons.Log("评论获取状态 - " + commentJson.Property("returnMsg").Value.ToString());
                        //    returnData = (JObject)commentJson["returnData"];
                        //    JArray comments = (JArray)returnData["data"];
                        //    for (int j = 0; j < comments.Count; j++)
                        //    {
                        //        JObject commment = (JObject)comments[j];
                        //        //新建评论
                        //        var linkComment = new WeiXinLinkCommentMongo
                        //        {
                        //            LinkId = link._id,
                        //            CreateAt = DateTime.Now.AddHours(8),
                        //            Content = commment.Property("content").Value.ToString(),
                        //            NickName = commment.Property("nick_name").Value.ToString(),
                        //            LogoUrl = commment.Property("logo_url").Value.ToString(),
                        //            LikeNum = Convert.ToInt32(commment.Property("like_num").Value.ToString()),
                        //            CommentId = Convert.ToInt32(commment.Property("id").Value.ToString()),
                        //        };
                        //        DateTime.TryParse(commment.Property("create_time").Value.ToString(), out dt);
                        //        linkComment.CreateTime = dt.AddHours(8);
                        //        dt = DateTime.MinValue;
                        //        DateTime.TryParse(commment.Property("get_time").Value.ToString(), out dt);
                        //        linkComment.GetTime = dt.AddHours(8);
                        //        dt = DateTime.MinValue;

                        //        //获取用户回复
                        //        linkComment.AuReplys = new List<AuthorReply>();
                        //        JArray authorReplys = (JArray)commment["reply"]["reply_list"];
                        //        for(int k=0;k<authorReplys.Count;k++)
                        //        {
                        //            JObject replyJson = (JObject)authorReplys[k];
                        //            var reply = new AuthorReply
                        //            {
                        //                Content = replyJson.Property("content").Value.ToString(),
                        //                ReplyId = Convert.ToInt32(replyJson.Property("reply_id").Value.ToString())
                        //            };
                        //            DateTime.TryParse(replyJson.Property("create_time").Value.ToString(), out dt);
                        //            reply.CreateTime = dt.AddHours(8);
                        //            dt = DateTime.MinValue;
                        //            linkComment.AuReplys.Add(reply);
                        //        }
                        //        linkComments.Add(linkComment);
                        //    }

                            
                        //}
                        #endregion
                        try
                        {
                            //检查本条链接是否已经保存过
                            var builderLink = Builders<WXLinkMainMongo>.Filter;
                            var filterLink = builderLink.Eq(x => x.KeywordId, linkMain.KeywordId) & builderLink.Eq(x => x.Url, linkMain.Url);
                            var colLinkMain = MongoDBHelper.Instance.GetWXLinkMain();
                            var colLinkOther = MongoDBHelper.Instance.GetWXLinkOther();
                            var colLinkContent = MongoDBHelper.Instance.GetWXLinkContent();
                            var queryLink = colLinkMain.Find(filterLink).FirstOrDefault();
                            if (queryLink == null)
                            {
                                colLinkMain.InsertOne(linkMain);
                                colLinkOther.InsertOne(linkOther);
                                colLinkContent.InsertOne(linkContent);
                                Commons.Log("成功保存1条链接！\t" + linkMain.Title);
                            }
                            else
                            {
                                Commons.Log("该链接已保存！\t" + linkMain.Title);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Commons.Log(ex.Message);
                            SetError();
                            Console.ReadLine();
                        }

                    }
                    linkNum += items.Count;          //当前已搜索链接数
                    if (linkNum >= topNum)
                    {
                        Commons.Log("共保存链接数 - " + linkNum);
                        break;
                    }

                }
                else
                {
                    if (linkJson.Property("errcode") != null)
                    {
                        Commons.Log("错误信息 - " + linkJson.Property("errmsg").ToString());
                        Commons.Log("错误代码 - " + linkJson.Property("errmsg").ToString());
                        while (true)
                        {
                            Commons.Log("是否继续？（Y/N）");
                            string code = Console.ReadLine().ToUpper();
                            if (code != "Y"&&code!="N")
                            {
                                continue;
                            }
                            else
                            {
                                if (code == "Y")
                                    break;
                                else
                                    Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            return linkNum;
        }


    }
}
