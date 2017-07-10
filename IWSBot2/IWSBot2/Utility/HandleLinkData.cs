using IWSData.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AISSystem;
using IWSBot.Queries;
using System.Text.RegularExpressions;
using ProxyLib;
using MongoDB.Bson;

namespace IWSBot.Utility
{
    public class HandleLinkData
    {
        HandleLinkData()
        {
           // var it = ProxyLib.IPPool.Instance; 
        }

        public static readonly HandleLinkData Instance = new HandleLinkData();

        private Dictionary<Guid, List<keyword>> dicUseridKeywords = new Dictionary<Guid, List<keyword>>();

        public void Run()
        {
            while (true)
            {
                Random r = new Random();
                var p = get_search_to_qry();
                if (p == null || p.Count == 0)
                {
                    Thread.Sleep(r.Next(30000, 100000));
                    continue;
                }
                try
                {
                    query(p);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Console.WriteLine("baidu_query ERROR.Message:{0},Statck:{1}".FormatStr(ex.Message, ex.StackTrace));
                        ex = ex.InnerException;
                    }
                }
                
            }
        }
        
        private List<level1link> get_search_to_qry()
        {
            List<level1link> qryResult = new List<level1link>();

            var builder = Builders<level1link>.Filter;
            var filter = builder.Eq(x => x.Abstract, BsonString.Empty);            

            var query = Query.Or( Query.NotExists("Abstract"),Query.EQ("Abstract",BsonNull.Value));
            FieldsDocument fd = new FieldsDocument();
            fd.Add("LinkUrl", 1);
            fd.Add("UsrId", 1);
            fd.Add("Keywords", 1);
            fd.Add("Description", 1);

            var col = MongoDBHelper.Instance.Getlevel1links();

            var getresult = col.Find(filter).Project(x => new
            {
                LinkUrl = x.LinkUrl,
                UsrId = x.UsrId,
                Keywords = x.Keywords,
                Description = x.Description
            }).Limit(10).ToList();
            foreach(var nextResult in getresult)
            {
                qryResult.Add(new level1link
                {
                    LinkUrl = nextResult.LinkUrl,
                    UsrId = nextResult.UsrId,
                    Keywords = nextResult.Keywords,
                    Description = nextResult.Description
                });
            }
            
            return qryResult;
        }

        private void query (List<level1link> links)
        {
            try
            {
                List<keyword> businessKeys = null;
                foreach (var link in links)
                {
                    if (dicUseridKeywords.ContainsKey(link.UsrId))
                    {
                        businessKeys = dicUseridKeywords[link.UsrId];
                    }
                    else
                    {
                        var query = new QueryDocument { { "UsrId", link.UsrId } };
                        var businessKeywords = MongoDBHelper.Instance.Getiws_businesskeywords().Find(query).ToList();
                        businessKeys = businessKeywords;
                        if (businessKeywords != null && businessKeywords.Count > 0 && !dicUseridKeywords.ContainsKey(link.UsrId))
                        {
                            dicUseridKeywords.Add(link.UsrId, businessKeywords);
                        }
                    }
                    string realUrl = null, domain = null, detailHtml = null, abstracts = null;
                    int? maxScore = 0;
                    try
                    {
                        BaiduQuery query = new BaiduQuery(null);
                        var tuplehtml = query.get_htmlUrl(link.LinkUrl);
                        
                        if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item1))
                        {
                            realUrl = tuplehtml.Item1;
                        }
                        if (tuplehtml != null && !string.IsNullOrEmpty(tuplehtml.Item2))
                        {
                            detailHtml = tuplehtml.Item2;
                        }
                        if (!string.IsNullOrEmpty(realUrl) && string.IsNullOrEmpty(domain))
                        {
                            domain = BaiduQuery.GetDomain(realUrl);
                        }

                        List<KeywordScore> patterns = businessKeys.Select(x => new KeywordScore { Keyword = x.Txt, Score = x.Score }).ToList();
                        maxScore = patterns.Max(x => x.Score);
                        if (!maxScore.HasValue)
                        {
                            maxScore = 50;
                        }
                        if (!string.IsNullOrEmpty(link.Keywords))
                        {
                            var searchKeys = link.Keywords.SplitWith(" + ");
                            if (searchKeys.Length >= 1)
                            {
                                patterns.Add(new KeywordScore { Keyword = searchKeys[0], Score = 50 });
                            }
                        }

                        if (!string.IsNullOrEmpty(detailHtml))
                        {

                            //detailHtml = detailHtml.SubstringAfter("body>").SubLastStringBefore("</body>");
                            
                            StringBuilder sbabstracts = new StringBuilder();
                            List<string> abstractlist = new List<string>();
                            StringBuilder sbabstractlist = new StringBuilder();
                            foreach (KeywordScore pattern in patterns)
                            {                                
                                string[] splitDetailHtmls = detailHtml.SplitWith(pattern.Keyword);
                                if(splitDetailHtmls.Length > 1)
                                {
                                    maxScore += (pattern.Score.HasValue ? pattern.Score.Value : 50) / 10;
                                }
                                StringBuilder sbpatternStr = new StringBuilder();
                                for (int i = 0; i < splitDetailHtmls.Length - 1; i++)
                                {

                                    string splitDetailHtml1 = splitDetailHtmls[i];
                                    string splitDetailHtml2 = i < splitDetailHtmls.Length - 2 ? splitDetailHtmls[i + 1] : "";
                                    for (int j = splitDetailHtml1.Length - 1; j >= 0; j--)
                                    {
                                        if (split_bef_commas.Contains(splitDetailHtml1[j]) && j - 1 >=0 && !split_num_commas.Contains(splitDetailHtml1[j - 1]))
                                        { break; }
                                        sbpatternStr.Append(splitDetailHtml1[j]);
                                    }
                                    for (int q = sbpatternStr.Length - 1; q >= 0; q--)
                                    {
                                        sbabstracts.Append(sbpatternStr[q]);
                                    }
                                    sbabstracts.Append(pattern);
                                    sbpatternStr.Clear();
                                    for (int j = 0; j < splitDetailHtml2.Length; j++)
                                    {
                                        if (split_aft_commas.Contains(splitDetailHtml2[j]) && j + 1 < splitDetailHtml2.Length && !split_num_commas.Contains(splitDetailHtml2[j + 1]))
                                        { break; }
                                        sbpatternStr.Append(splitDetailHtml2[j]);
                                    }
                                    sbabstracts.Append(sbpatternStr);
                                    sbpatternStr.Clear();

                                    string tmpsbabstracts = sbabstracts.ToString();
                                    tmpsbabstracts = BaiduQuery.RemoveInivalidChar(tmpsbabstracts.GetTxtFromHtml2().RemoveSpace().GetLower());
                                    if (!abstractlist.Contains(tmpsbabstracts))
                                    {
                                        abstractlist.Add(tmpsbabstracts);
                                        sbabstractlist.Append(tmpsbabstracts).Append(" ");
                                    }
                                    sbabstracts.Clear();
                                }
                            }
                            abstracts = sbabstractlist.ToString();
                        }
                        if (string.IsNullOrEmpty(abstracts))
                        {
                            foreach (KeywordScore pattern in patterns)
                            {
                                string[] splitDetailHtmls = link.Description.SplitWith(pattern.Keyword);
                                if (splitDetailHtmls.Length > 1)
                                {
                                    maxScore += (pattern.Score.HasValue ? pattern.Score.Value : 50) / 10;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log(ex.Message);
                    }
                    if (string.IsNullOrEmpty(detailHtml) || string.IsNullOrEmpty(realUrl) || !maxScore.HasValue)
                        continue;
                    if(maxScore.HasValue && maxScore.Value >100)
                    {
                        maxScore = 100;
                    }
                    var updateWebsiteId = new UpdateDocument { { "$set", new QueryDocument { { "Abstract", abstracts },{"TopDomain",BaiduQuery.GetLevel1Domain(domain)},
                    { "LinkUrl", realUrl }, { "Domain", domain }, { "Html", detailHtml } , { "Score", maxScore.Value }} } };
                    var result2 = MongoDBHelper.Instance.Getlevel1links().UpdateOne(new QueryDocument { { "_id", link._id } }, updateWebsiteId);
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }

        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }
        static HashSet<char> split_bef_commas = new HashSet<char> { '？', '。', '！', '；', '.', '>', '，', ',' };//'，', ','
        static HashSet<char> split_aft_commas = new HashSet<char> { '？', '。', '！', '；', '.', '<', '，', ',' };
        static HashSet<char> split_num_commas = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    }

    public class KeywordScore
    {
        public string Keyword { get; set; }
        public int? Score { get; set; }

        public byte BizType { get; set; }

    }
}
