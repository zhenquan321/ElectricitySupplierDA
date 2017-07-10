using IWSData.Model;
using ProxyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using IWSBot.Utility;
using MongoDB.Driver;
using MongoV2;
using MongoDB.Bson;

namespace IWSBot.Queries
{
    public class BaiduKeywordQuery
    {
        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();
        //HTML.WebHelper web = new HTML.WebHelper();
        string nick_name;
        
        static int count = 0;
        static object baidu_token = new object();
        int pages = 0;


        public BaiduKeywordQuery(string _nick_name)
        {
            lock (baidu_token)
            {
                count++;
                nick_name = _nick_name ?? ("anonymous_"+count );
                
            }
        }

        public void Query(IW2S_BaiduKeyword tsk)
        {

            var link = get_urls(tsk);
            if (string.IsNullOrEmpty(link))
                return;

            GetLinks(link, tsk,0);
        }

        void GetLinks(string link, IW2S_BaiduKeyword tsk,int height)
        {
            
            string searchKeyword = tsk.Keyword.GetLower().RemoveSpace();

            if(!string.IsNullOrEmpty(link))
            {
                log(link);
                var html = get_html(link);
                if (html == null)
                    return;
                var tags = html.SubAfter("相关搜索</div>").SubBefore("id=\"page\"").SplitWith("<a");

                if (tags == null || tags.Length == 0)
                {
                    log("BLOCKED " + tsk.Keyword);
                    return;
                }
                
                foreach (var a in tags)
                {

                    string title = a.GetTxtFromHtml2().RemoveSpace().GetLower();
                    string href = a.GetFirstHref2();
                    var searchKey = tsk.Keyword.ToLower();

                    if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(href) || title == searchKey || !title.Contains(searchKey))
                        continue;

                    IW2S_BaiduCommend baiduCommend = new IW2S_BaiduCommend
                    {
                        _id = "{0}{1}".FormatStr(tsk._id, title).ToObjectId(),
                        CommendKeyword = title,
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        Keyword = tsk.Keyword,
                        KeywordId = tsk._id,
                        UsrId = tsk.UsrId,
                        BotIntervalHours = 7*24,
                        ProjectId = tsk.ProjectId
                    };

                    saveBaiduKeyword(baiduCommend);

                    //if (!string.IsNullOrWhiteSpace(href) && height < 1)
                    //{
                    //    GetLinks("https://www.baidu.com" + href, tsk, height+1);
                    //}
                }
            }
        }

        public void saveBaiduKeyword(IW2S_BaiduCommend baiduCommend)
        {
            if (baiduCommend == null)
            {
                return;
            }


            var col = MongoDBHelper.Instance.GetIW2S_BaiduCommends();
            var builder = Builders<IW2S_BaiduCommend>.Filter;

            var exists_obj = col.Find(builder.Eq(x => x._id, baiduCommend._id)).Project(x => new IDIntDto { Id = x._id, Times = x.Times }).FirstOrDefault();
            if (exists_obj == null || exists_obj.Id == new MongoDB.Bson.ObjectId("000000000000000000000000"))
            {
                col.InsertOne(baiduCommend);
                log("SUCCESS saving keywords {0} for {1}".FormatStr(baiduCommend.CommendKeyword, baiduCommend.Keyword));

            }
            else
            {
                var update = new UpdateDocument { { "$set", new QueryDocument { { "Times", exists_obj.Times + 1 } } } };

                var result = MongoDBHelper.Instance.GetIW2S_BaiduCommends().UpdateOne(new QueryDocument { { "_id", baiduCommend._id } }, update);
            }
        }

         string get_html(string url)
        {
            var html = proxy.GetFastHtmlWithProxyIpAndARE(url, "utf-8").RemoveSpace();
            //if (!html.IsContains("c-container"))
            //    html = web.GetHtml(url, null);
            return html;
        }

         
         string get_urls(IW2S_BaiduKeyword tsk)
         {

             string searchKeywords = tsk.Keyword.RemoveSpace().GetLower();
             if (!string.IsNullOrEmpty(searchKeywords))
             {
                 string baiduUrlFormat = "http://www.baidu.com/s?ie=utf-8&wd={0}";
                 return baiduUrlFormat.FormatStr(searchKeywords.GetUrlEncodedString("utf-8"));
                     
             }
             return string.Empty;
         }


        void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToDateKey2() + "[" + nick_name+"/"+pages + "]:" + msg);
        }

        
        static readonly string[] domain_sufixes = new string[]{  ".cn",".la",".top",".so",".biz",".name",".info",".cc",".tv",".中国",".mobi",".me",".asia",
            ".co",".tel",".公司",".网络",".wang",".net",".org",".edu",".gov",".com"};
        static readonly  string[] domain_sps = new string[] { "/", "?", " ", "." };

        public static string GetDomain(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            string prefix = "";
            foreach (var sufix in domain_sufixes)
            {
                foreach (var sp in domain_sps)
                {
                    prefix = url.SubLastStringBefore(sufix + sp);
                    if (!string.IsNullOrEmpty(prefix))
                        return prefix + sufix;
                }
            }
            return url;
        }

        public static string GetLevel1Domain(string domain)
        { 
            if(string.IsNullOrEmpty(domain) || !domain.IsContain("."))
                return domain;
            if (domain.IsStartWith("www."))
                domain= domain.SubAfter("www.");
            if (domain.IsContain("//"))
                domain = domain.SubAfter("//");
            if (domain.IsContain("?"))
                domain = domain.SubBefore("?");
            if (domain.IsContains("/"))
                domain = domain.SubBefore("/");
            if (string.IsNullOrEmpty(domain) || !domain.IsContain("."))
                return domain;
            //.com.cn, .net.cn
            int level2Index = int.MaxValue ;
            foreach (var sufix in domain_sufixes)
            {
                int index=domain.IndexOf(sufix+".");
                if(index>0&& index<level2Index)
                    level2Index=index; 
            }
            if (level2Index < int.MaxValue && level2Index > 0)
            {
                string tmp = domain.Substring(0, level2Index);
                if (tmp.Contains("."))
                    tmp = tmp.SubLastStringAfter(".");
                domain = tmp + domain.Substring(level2Index);
                return domain;
            }
            else
            {
                string tmp = domain.SubLastStringBefore(".");
                if (tmp.Contains("."))
                    tmp = tmp.SubLastStringAfter(".");
                domain = tmp.GetContact(".").GetContact(domain.SubLastStringAfter("."));
                return domain;
            }            
        }

        static HashSet<char> chinese_commas = new HashSet<char> { '？', '。', '，', '！', '》', '《', '‘', '“', '；' };
        static HashSet<char> split_num_commas = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static HashSet<char> split_bef_commas = new HashSet<char> { '？', '。', '！', '；', '.', '>', '，', ',' };//'，', ','
        static HashSet<char> split_aft_commas = new HashSet<char> { '？', '。', '！', '；', '.', '<', '，', ',' };
        public static string RemoveInivalidChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (chinese_commas.Contains(c))
                    continue;
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c > 255)
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }

    public class IDIntDto
    {
        public ObjectId Id { get; set; }
        public int Times { get; set; }
    }
}
