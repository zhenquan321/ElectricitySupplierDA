using IWSBot.Utility;
using IWSBot2.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MongoDB.Driver;
using IWSData.Model;
using System.Threading;

namespace IWSBot.Utility
{
    public class WL_IndustryMng
    {
        private const string Secret = "e5cf931143161fd243c7f42fabac0e8a";//卧龙提供Secret
        private const string AppKey = "4314425828";//卧龙提供AppKey

        HTML.WebHelper webHelper = new HTML.WebHelper();

        public static readonly WL_IndustryMng Instance = new WL_IndustryMng();

        public void startWL_Industry()
        {
            while (true)
            {
                try
                {

                    startWL_Industry_ext();
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                }

                Thread.Sleep(30000);
            }

        }

        private void startWL_Industry_ext()
        {
            var col = MongoDBHelper.Instance.GetWL_Industrys();
            var builder = Builders<WL_Industry>.Filter;
            var filter = builder.Eq(x => x.OrgNo, null);

            var data = col.Find(filter).Limit(1).FirstOrDefault();
            if (data == null) return;

            var result = GetIndustryInfo(data.Name);
        }

        private WL_Industry GetIndustryInfo(string company_name)
        {
            WL_Industry result = null;
            var jobj = CallWL_Industry(company_name);
            if(jobj != null)
            {

            }
            return result;
        }
        private JObject CallWL_Industry(string company_name)
        {
            Commons.Log("访问卧龙工商数据接口...");
            string UrlFormat = "http://api.wolongdata.com:8888/icredit/info/get";

            Dictionary<string, object> woLongParameters = new Dictionary<string, object>();
            woLongParameters.Add("company_name", company_name);
            woLongParameters.Add("app_key", AppKey);
            woLongParameters.Add("t", Commons.GetTimestamp());

            string url;
            string sign = Commons.GenerateWolongSign(woLongParameters, Secret, out url);
            string woLongUrl = UrlFormat + url + "&sign=" + sign;

            string woLongJson = webHelper.GetHtmlTimeout(woLongUrl, null, null, 1000 * 60);
            if (woLongJson == null)
            {
                Commons.Log("访问卧龙工商数据接口超时！");
                int tryTimes = 0;
                while (woLongJson == null)
                {
                    tryTimes++;
                    if (tryTimes <= 3)
                    {
                        Commons.Log("开始第 {0} 次重试...".FormatStr(tryTimes));
                        woLongJson = webHelper.GetHtmlTimeout(woLongUrl, null, null, 1000 * 60);
                    }
                    else
                    {
                        break;
                    }
                }
                if (woLongJson == null) return null;
            }
            JObject wl_Industry = (JObject)JsonConvert.DeserializeObject(woLongJson);

            return wl_Industry;
        }
        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }
    }
}
