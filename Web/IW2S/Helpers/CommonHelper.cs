using AISSystem;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Helpers
{
    public class CommonHelper
    {
        public static string file_sys_base_url = AppSettingHelper.GetAppSetting("file_api");
        public static string iw2s_site = AppSettingHelper.GetAppSetting("iw2s_site");
        public static string GetLawCodeStr(int value)
        {
            string name = string.Empty;
            switch(value)
            {
                case 1:
                    name = "擅自使用知名商品特有的名称、包装、装潢行为";
                    break;
                case 2:
                    name = "商业贿赂行为";
                    break;
                case 3:
                    name = "虚假宣传行为";
                    break;
                case 4:
                    name = "侵犯商业秘密行为";
                    break;
                case 5:
                    name = "不正当有奖销售行为";
                    break;
                case 6:
                    name = "公用企业或独占经营者强制交易行为";
                    break;
                case 7:
                    name = "滥用行政权力限制竞争行为";
                    break;
                case 8:
                    name = "串通招投标行为";
                    break;
                default:
                    break;
            }

            return name;
        }
        private static char[] constant =   
      {   
        '0','1','2','3','4','5','6','7','8','9',  
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',   
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
      };


        public static string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

        /// <summary>
        /// 拆分为字符串Id列表
        /// </summary>
        /// <param name="idStr">源Id字符串</param>
        /// <returns>字符串Id列表</returns>
        public static List<string> GetIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").ToList();
        }
        public static List<int> GetIdIntListFromStr(string idStr)
        {
            List<int> result = new List<int>();
            if(string.IsNullOrEmpty(idStr ))
            {
                return result;
            }
            var idArray = idStr.Split(';');
            
            int r = 0;
            foreach (var id in idArray)
            {
                int.TryParse(id, out r);
                result.Add(r);
            }
            return result;
        }

        /// <summary>
        /// 拆分为ObjectId列表
        /// </summary>
        /// <param name="idStr">源Id字符串</param>
        /// <returns>ObjectIdId列表</returns>
        public static List<ObjectId> GetObjIdListFromStr(string idStr)
        {
            var idArray = idStr.Split(';');
            return idArray.Where(x => !string.IsNullOrEmpty(x) && x != "undefined").Select(x => new ObjectId(x)).ToList();
        }
    }

 
}