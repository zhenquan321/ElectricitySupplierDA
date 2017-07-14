using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoV2;
using IW2S.Models;
using IW2S.Controllers;
using IWSData.Model;
using IW2S.Models.Emarknow;

namespace IW2S.Helpers
{
    public class MongoDBHelper:MDB
    {

        static string conn = AppSettingHelper.GetAppSetting("mongoCon");

        static string dbName = AppSettingHelper.GetAppSetting("mongoDB");


        public MongoDBHelper()
            : base(conn, dbName)
        {

        }

        public static readonly MongoDBHelper Instance = new MongoDBHelper();

        public IMongoCollection<IW2SUser> Get_IW2SUser()
        {
            return base.GetCollection<IW2SUser>("IW2SUser");
        }

        public IMongoCollection<ipfiles> Get_ipfiles()
        {
            return base.GetCollection<ipfiles>("ipfiles");
        }


       ///////
        public IMongoCollection<IW2S_Bing_BaiduKeyword> GetIW2S_Bing_BaiduKeyword()
        {
            return base.GetCollection<IW2S_Bing_BaiduKeyword>("IW2S_Bing_BaiduKeyword");
        }

        public IMongoCollection<IW2S_Bing_BaiduCommend> GetIW2S_Bing_BaiduCommend()
        {
            return base.GetCollection<IW2S_Bing_BaiduCommend>("IW2S_Bing_BaiduCommend");
        }

        public IMongoCollection<IW2S_Bing_KeywordGroup> GetIW2S_Bing_KeywordGroup()
        {
            return base.GetCollection<IW2S_Bing_KeywordGroup>("IW2S_Bing_KeywordGroup");
        }


        public IMongoCollection<IW2S_Bing_level1link> GetIW2S_Bing_level1links()
        {
            return base.GetCollection<IW2S_Bing_level1link>("IW2S_Bing_level1link");
        }


        public IMongoCollection<IW2S_Bing_KeywordCategory> GetIW2S_Bing_KeywordCategory()
        {
            return base.GetCollection<IW2S_Bing_KeywordCategory>("IW2S_Bing_KeywordCategory");
        }
       
        public IMongoCollection<IW2S_Bing_KeywordFilter> GetIW2S_Bing_KeywordFilter()
        {
            return base.GetCollection<IW2S_Bing_KeywordFilter>("IW2S_Bing_KeywordFilter");
        }


     //============================= =  Google

        ///////
        public IMongoCollection<Dnl_Google_BaiduKeyword> GetDnl_Google_BaiduKeyword()
        {
            return base.GetCollection<Dnl_Google_BaiduKeyword>("Dnl_Google_BaiduKeyword");
        }

        public IMongoCollection<Dnl_Google_BaiduCommend> GetDnl_Google_BaiduCommend()
        {
            return base.GetCollection<Dnl_Google_BaiduCommend>("Dnl_Google_BaiduCommend");
        }

        public IMongoCollection<Dnl_Google_KeywordGroup> GetDnl_Google_KeywordGroup()
        {
            return base.GetCollection<Dnl_Google_KeywordGroup>("Dnl_Google_KeywordGroup");
        }


        public IMongoCollection<Dnl_Google_level1link> GetDnl_Google_level1links()
        {
            return base.GetCollection<Dnl_Google_level1link>("Dnl_Google_level1link");
        }


        public IMongoCollection<Dnl_Google_KeywordCategory> GetDnl_Google_KeywordCategory()
        {
            return base.GetCollection<Dnl_Google_KeywordCategory>("Dnl_Google_KeywordCategory");
        }

        public IMongoCollection<Dnl_Google_KeywordFilter> GetDnl_Google_KeywordFilter()
        {
            return base.GetCollection<Dnl_Google_KeywordFilter>("Dnl_Google_KeywordFilter");
        }







//============================================================

        //public IMongoCollection<IW2S_BaiduKeyword> GetIW2S_BaiduKeywords()
        //{
        //    return base.GetCollection<IW2S_BaiduKeyword>("IW2S_BaiduKeyword");
        //}

        //public IMongoCollection<IW2S_BaiduCommend> GetIW2S_BaiduCommends()
        //{
        //    return base.GetCollection<IW2S_BaiduCommend>("IW2S_BaiduCommend");
        //}

        public IMongoCollection<IW2S_SearchKeyword> GetIW2S_SearchKeywords()
        {
            return base.GetCollection<IW2S_SearchKeyword>("IW2S_SearchKeyword");
        }

        //public IMongoCollection<IW2S_level1link> GetIW2S_level1links()
        //{
        //    return base.GetCollection<IW2S_level1link>("IW2S_level1link");
        //}

        //public IMongoCollection<IW2S_KeywordCategory> GetIW2S_KeywordCategorys()
        //{
        //    return base.GetCollection<IW2S_KeywordCategory>("IW2S_KeywordCategory");
        //}
        //public IMongoCollection<IW2S_KeywordGroup> GetIW2S_KeywordGroups()
        //{
        //    return base.GetCollection<IW2S_KeywordGroup>("IW2S_KeywordGroup");
        //}
        public IMongoCollection<IW2S_KeywordFilter> GetIW2S_KeywordFilters()
        {
            return base.GetCollection<IW2S_KeywordFilter>("IW2S_KeywordFilter");
        }

        public IMongoCollection<IW2S_Project> GetIW2S_Projects()
        {
            return base.GetCollection<IW2S_Project>("IW2S_Project");
        }

        public IMongoCollection<Dnl_ProjectGroup> GetDnl_ProjectGroup()
        {
            return base.GetCollection<Dnl_ProjectGroup>("Dnl_ProjectGroup");
        }

        public IMongoCollection<Dnl_ProjectCategory> GetDnl_ProjectCategory()
        {
            return base.GetCollection<Dnl_ProjectCategory>("Dnl_ProjectCategory");
        }

        public IMongoCollection<IW2S_ProLinksCount> GetIW2S_ProLinksCount()
        {
            return base.GetCollection<IW2S_ProLinksCount>("IW2S_ProLinksCount");
        }

        public IMongoCollection<IW2S_BotRegister> GetIW2S_BotRegister()
        {
            return GetCollection<IW2S_BotRegister>("IW2S_BotRegister");
        }

        public IMongoCollection<IW2S_BotData> GetIW2S_BotDataIn6Hours()
        {
            return GetCollection<IW2S_BotData>("IW2S_BotDataIn6Hours");
        }

        public IMongoCollection<IW2S_BotData> GetIW2S_BotDataInDay()
        {
            return GetCollection<IW2S_BotData>("IW2S_BotDataInDay");
        } 

        public IMongoCollection<FreeUser> Get_FreeUser()
        {
            return base.GetCollection<FreeUser>("FreeUser");
        }

        public IMongoCollection<FreeTask> Get_FreeTask()
        {
            return base.GetCollection<FreeTask>("FreeTask");
        }
        public IMongoCollection<FreeTaskRecord> Get_FreeTaskRecord()
        {
            return base.GetCollection<FreeTaskRecord>("FreeTaskRecord");
        }
        public IMongoCollection<FreeBotItem> Get_FreeBotItem()
        {
            return base.GetCollection<FreeBotItem>("FreeBotItem");
        }

        public IMongoCollection<FreeBotShop> Get_FreeBotShop()
        {
            return base.GetCollection<FreeBotShop>("FreeBotShop");
        }
        public IMongoCollection<FreeWebSite> Get_FreeWebSite()
        {
            return base.GetCollection<FreeWebSite>("FreeWebSite");
        }


        public IMongoCollection<FreeKeywordGroup> GetFreeKeywordGroup()
        {
            return base.GetCollection<FreeKeywordGroup>("FreeKeywordGroup");
        }
        public IMongoCollection<FreeKeywordCategory> GetFreeKeywordCategory()
        {
            return base.GetCollection<FreeKeywordCategory>("FreeKeywordCategory");
        }

        public IMongoCollection<FreeShopTimeline> Get_FreeShopTimeline()
        {
            return base.GetCollection<FreeShopTimeline>("FreeShopTimeline");
        }

        public IMongoCollection<links> GetIW2S_links()
        {
            return GetCollection<links>("IW2S_links");
        }

        public IMongoCollection<IW2S_WX_BaiduKeyword> GetIW2S_WX_BaiduKeywords()
        {
            return base.GetCollection<IW2S_WX_BaiduKeyword>("IW2S_WX_BaiduKeyword");
        }

        public IMongoCollection<IW2S_WX_BaiduCommend> GetIW2S_WX_BaiduCommends()
        {
            return base.GetCollection<IW2S_WX_BaiduCommend>("IW2S_WX_BaiduCommend");
        }

        public IMongoCollection<IW2S_WX_KeywordGroup> GetIW2S_WX_KeywordGroups()
        {
            return base.GetCollection<IW2S_WX_KeywordGroup>("IW2S_WX_KeywordGroup");
        }
        public IMongoCollection<IW2S_WX_KeywordCategory> GetIW2S_WX_KeywordCategorys()
        {
            return base.GetCollection<IW2S_WX_KeywordCategory>("IW2S_WX_KeywordCategory");
        }
        public IMongoCollection<IW2S_WX_level1link> GetIW2S_WX_level1links()
        {
            return base.GetCollection<IW2S_WX_level1link>("IW2S_WX_level1link");
        }
        public IMongoCollection<IW2S_WX_KeywordFilter> GetIW2S_WX_KeywordFilters()
        {
            return base.GetCollection<IW2S_WX_KeywordFilter>("IW2S_WX_KeywordFilter");
        }

        public IMongoCollection<WX_links> GetIW2S_WX_links()
        {
            return GetCollection<WX_links>("IW2S_WX_links");
        }

        public IMongoCollection<IW2S_WX_PrjAnalysisItems> GetIW2S_WX_PrjAnalysisItems()
        {
            return base.GetCollection<IW2S_WX_PrjAnalysisItems>("IW2S_WX_PrjAnalysisItem");
        }


        public IMongoCollection<IW2S_AnalysisItem> GetIW2S_AnalysisItems()
        {
            return base.GetCollection<IW2S_AnalysisItem>("IW2S_AnalysisItem");
        }

        public IMongoCollection<IW2S_AnalysisItemValue> GetIW2S_AnalysisItemValues()
        {
            return base.GetCollection<IW2S_AnalysisItemValue>("IW2S_AnalysisItemValue");
        }

        public IMongoCollection<IW2S_PrjAnalysisItem> GetIW2S_PrjAnalysisItems()
        {
            return base.GetCollection<IW2S_PrjAnalysisItem>("IW2S_PrjAnalysisItem");
        }
        public IMongoCollection<IW2S_ImgSearchTask> GetIW2S_ImgSearchTasks()
        {
            return base.GetCollection<IW2S_ImgSearchTask>("IW2S_ImgSearchTask");
        }
        public IMongoCollection<IW2S_ImgSearchLink> GetIW2S_ImgSearchLinks()
        {
            return base.GetCollection<IW2S_ImgSearchLink>("IW2S_ImgSearchLink");
        }
        public IMongoCollection<IW2S_WB_BaiduCommend> GetIW2S_WB_BaiduCommends()
        {
            return base.GetCollection<IW2S_WB_BaiduCommend>("IW2S_WB_BaiduCommend");
        }
        public IMongoCollection<IW2S_WB_level1link> GetIW2S_WB_level1links()
        {
            return base.GetCollection<IW2S_WB_level1link>("IW2S_WB_level1link");
        }
        public IMongoCollection<IW2S_WB_KeywordCategory> GetIW2S_WB_KeywordCategorys()
        {
            return base.GetCollection<IW2S_WB_KeywordCategory>("IW2S_WB_KeywordCategory");
        }
        public IMongoCollection<IW2S_WB_KeywordGroup> GetIW2S_WB_KeywordGroups()
        {
            return base.GetCollection<IW2S_WB_KeywordGroup>("IW2S_WB_KeywordGroup");
        }
        public IMongoCollection<IW2S_WB_KeywordFilter> GetIW2S_WB_KeywordFilters()
        {
            return base.GetCollection<IW2S_WB_KeywordFilter>("IW2S_WB_KeywordFilter");
        }
        public IMongoCollection<WB_links> GetIW2S_WB_links()
        {
            return GetCollection<WB_links>("IW2S_WB_links");
        }

        public IMongoCollection<IW2S_WB_PrjAnalysisItems> GetIW2S_WB_PrjAnalysisItems()
        {
            return base.GetCollection<IW2S_WB_PrjAnalysisItems>("IW2S_WB_PrjAnalysisItem");
        }

        public IMongoCollection<IW2S_SG_BaiduKeyword> GetIW2S_SG_BaiduKeywords()
        {
            return base.GetCollection<IW2S_SG_BaiduKeyword>("IW2S_SG_BaiduKeyword");
        }

        public IMongoCollection<IW2S_SG_BaiduCommend> GetIW2S_SG_BaiduCommends()
        {
            return base.GetCollection<IW2S_SG_BaiduCommend>("IW2S_SG_BaiduCommend");
        }

        public IMongoCollection<IW2S_SG_KeywordGroup> GetIW2S_SG_KeywordGroups()
        {
            return base.GetCollection<IW2S_SG_KeywordGroup>("IW2S_SG_KeywordGroup");
        }
        public IMongoCollection<IW2S_SG_KeywordCategory> GetIW2S_SG_KeywordCategorys()
        {
            return base.GetCollection<IW2S_SG_KeywordCategory>("IW2S_SG_KeywordCategory");
        }
        public IMongoCollection<IW2S_SG_level1link> GetIW2S_SG_level1links()
        {
            return base.GetCollection<IW2S_SG_level1link>("IW2S_SG_level1link");
        }
        public IMongoCollection<IW2S_SG_KeywordFilter> GetIW2S_SG_KeywordFilters()
        {
            return base.GetCollection<IW2S_SG_KeywordFilter>("IW2S_SG_KeywordFilter");
        }

        public IMongoCollection<SG_links> GetIW2S_SG_links()
        {
            return GetCollection<SG_links>("IW2S_SG_links");
        }

        public IMongoCollection<IW2S_SG_PrjAnalysisItem> GetIW2S_SG_PrjAnalysisItems()
        {
            return base.GetCollection<IW2S_SG_PrjAnalysisItem>("IW2S_SG_PrjAnalysisItem");
        }

        public IMongoCollection<IW2S_DomainCategory> GetIW2S_DomainCategorys()
        {
            return base.GetCollection<IW2S_DomainCategory>("IW2S_DomainCategory");
        }
        public IMongoCollection<IW2S_DomainCategoryData> GetIW2S_DomainCategoryDatas()
        {
            return base.GetCollection<IW2S_DomainCategoryData>("IW2S_DomainCategoryData");
        }

        public IMongoCollection<IW2S_ProjectShare> GetIW2S_ProjectShares()
        {
            return base.GetCollection<IW2S_ProjectShare>("IW2S_ProjectShare");
        }

        public IMongoCollection<IW2S_OperateLog> GetIW2S_OperateLogs()
        {
            return base.GetCollection<IW2S_OperateLog>("IW2S_OperateLog");
        }
        public IMongoCollection<IW2S_OperateComment> GetIW2S_OperateComments()
        {
            return base.GetCollection<IW2S_OperateComment>("IW2S_OperateComment");
        }
        public IMongoCollection<IW2S_ShareOutComment> GetIW2S_ShareOutComments()
        {
            return base.GetCollection<IW2S_ShareOutComment>("IW2S_ShareOutComment");
        }
        
        public IMongoCollection<IW2S_UrlQRCode> GetIW2S_UrlQRCodes()
        {
            return base.GetCollection<IW2S_UrlQRCode>("IW2S_UrlQRCode");
        }

        public IMongoCollection<IW2S_ShareToDiscover> GetIW2S_ShareToDiscovers()
        {
            return base.GetCollection<IW2S_ShareToDiscover>("IW2S_ShareToDiscover");
        }

        public IMongoCollection<IW2S_ChartConfig> GetIW2S_ChartConfig()
        {
            return base.GetCollection<IW2S_ChartConfig>("IW2S_ChartConfig");
        }

        #region 数据简报
        public IMongoCollection<Dnl_Report> GetDnl_Report()
        {
            return base.GetCollection<Dnl_Report>("Dnl_Report");
        }

        public IMongoCollection<Dnl_ReportShare> GetDnl_ReportShare()
        {
            return base.GetCollection<Dnl_ReportShare>("Dnl_ReportShare");
        }

        public IMongoCollection<Dnl_Report_Keyword> GetDnl_Report_Keyword()
        {
            return base.GetCollection<Dnl_Report_Keyword>("Dnl_Report_Keyword");
        }

        public IMongoCollection<Dnl_Report_KeywordCategory> GetDnl_Report_KeywordCategory()
        {
            return base.GetCollection<Dnl_Report_KeywordCategory>("Dnl_Report_KeywordCategory");
        }

        public IMongoCollection<Dnl_Report_Statistics> GetDnl_Report_Statistics()
        {
            return base.GetCollection<Dnl_Report_Statistics>("Dnl_Report_Statistics");
        }

        public IMongoCollection<Dnl_Report_keywordChart> GetDnl_Report_keywordChart()
        {
            return base.GetCollection<Dnl_Report_keywordChart>("Dnl_Report_keywordChart");
        }

        public IMongoCollection<Dnl_Report_LinkChart> GetDnl_Report_LinkChart()
        {
            return base.GetCollection<Dnl_Report_LinkChart>("Dnl_Report_LinkChart");
        }

        public IMongoCollection<Dnl_Report_LinkChartCategory> GetDnl_Report_LinkChartCategory()
        {
            return base.GetCollection<Dnl_Report_LinkChartCategory>("Dnl_Report_LinkChartCategory");
        }

        public IMongoCollection<Dnl_Report_Description> GetDnl_Report_Description()
        {
            return base.GetCollection<Dnl_Report_Description>("Dnl_Report_Description");
        }

        public IMongoCollection<Dnl_Report_DomainChart> GetDnl_Report_DomainChart()
        {
            return base.GetCollection<Dnl_Report_DomainChart>("Dnl_Report_DomainChart");
        }

        public IMongoCollection<Dnl_WordTree> GetDnl_WordTree()
        {
            return base.GetCollection<Dnl_WordTree>("Dnl_WordTree");
        }

        public IMongoCollection<Dnl_Report_WordTree> GetDnl_Report_WordTree()
        {
            return base.GetCollection<Dnl_Report_WordTree>("Dnl_Report_WordTree");
        }

        public IMongoCollection<Dnl_Report_TimeLink> GetDnl_Report_TimeLink()
        {
            return base.GetCollection<Dnl_Report_TimeLink>("Dnl_Report_TimeLink");
        }
        #endregion

        public IMongoCollection<Dnl_StopWord> GetDnl_StopWord()
        {
            return base.GetCollection<Dnl_StopWord>("Dnl_StopWord");
        }

        public IMongoCollection<Dnl_EntityTree> GetDnl_EntityTree()
        {
            return base.GetCollection<Dnl_EntityTree>("Dnl_EntityTree");
        }

        #region 搜索引擎关键词与链接库
        public IMongoCollection<Dnl_Keyword> GetDnl_Keyword()
        {
            return base.GetCollection<Dnl_Keyword>("Dnl_Keyword");
        }

        public IMongoCollection<Dnl_KeywordCategory> GetDnl_KeywordCategory()
        {
            return base.GetCollection<Dnl_KeywordCategory>("Dnl_KeywordCategory");
        }

        public IMongoCollection<Dnl_KeywordMapping> GetDnl_KeywordMapping()
        {
            return base.GetCollection<Dnl_KeywordMapping>("Dnl_KeywordMapping");
        }

        public IMongoCollection<Dnl_Link_Baidu> GetDnl_Link_Baidu()
        {
            return base.GetCollection<Dnl_Link_Baidu>("Dnl_Link_Baidu");
        }

        public IMongoCollection<Dnl_LinkMapping_Baidu> GetDnl_LinkMapping_Baidu()
        {
            return base.GetCollection<Dnl_LinkMapping_Baidu>("Dnl_LinkMapping_Baidu");
        }

        public IMongoCollection<Dnl_MappingCoPresent> GetDnl_MappingCoPresent()
        {
            return base.GetCollection<Dnl_MappingCoPresent>("Dnl_MappingCoPresent");
        }
        #endregion

        public IMongoCollection<Dnl_File> GetDnl_File()
        {
            return base.GetCollection<Dnl_File>("Dnl_File");
        }

        public IMongoCollection<Dnl_EntityTreeMapping> GetDnl_EntityTreeMapping()
        {
            return base.GetCollection<Dnl_EntityTreeMapping>("Dnl_EntityTreeMapping");
        }

        public IMongoCollection<PojectChartMongo> GetPojectChart()
        {
            return base.GetCollection<PojectChartMongo>("PojectChart");
        }

        #region 社交媒体关键词与链接库
        public IMongoCollection<MediaKeywordMongo> GetMediaKeyword()
        {
            return base.GetCollection<MediaKeywordMongo>("MediaKeyword");
        }

        public IMongoCollection<MediaKeywordCategoryMongo> GetMediaKeywordCategory()
        {
            return base.GetCollection<MediaKeywordCategoryMongo>("MediaKeywordCategory");
        }

        public IMongoCollection<MediaKeywordMappingMongo> GetMediaKeywordMapping()
        {
            return base.GetCollection<MediaKeywordMappingMongo>("MediaKeywordMapping");
        }
        public IMongoCollection<WXLinkMainMongo> GetWXLinkMain()
        {
            return base.GetCollection<WXLinkMainMongo>("WXLinkMain");
        }

        public IMongoCollection<WXLinkOtherMongo> GetWXLinkOther()
        {
            return base.GetCollection<WXLinkOtherMongo>("WXLinkOther");
        }

        public IMongoCollection<WXLinkContentMongo> GetWXLinkContent()
        {
            return base.GetCollection<WXLinkContentMongo>("WXLinkContent");
        }

        public IMongoCollection<WXNameMongo> GetWXName()
        {
            return base.GetCollection<WXNameMongo>("WXName");
        }
        #endregion
    }

    public static class MongoExtension
    {
        public static ObjectId ToObjectId(this string txt)
        {
            var bs = Encoding.Default.GetBytes(txt);
            byte[] arr = new byte[12];
            foreach (var b in bs)
            {
                Mutl(arr, 31);
                Add(arr, b);
            }
            string hex = GetHex(arr);
            return ObjectId.Parse(hex);
        }


        static void Mutl(byte[] bytes, byte a)
        {
            uint preAdd = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                uint b = ((uint)bytes[i]) * ((uint)a) + preAdd;
                preAdd = b / 256;
                bytes[i] = (byte)(b % 256);
            }
        }

        static void Add(byte[] bytes, byte a)
        {
            uint preAdd = a;
            for (var i = 0; i < bytes.Length; i++)
            {
                uint b = ((uint)bytes[i]) + preAdd;
                bytes[i] = (byte)(b % 256);
                if (b < 256)
                    break;
                preAdd = b / 256;
            }
        }

        static string GetHex(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < bs.Length; i++)
            {
                sb.Append(bs[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
