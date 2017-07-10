using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AISSystem
{
    public  class AppSettingHelper
    {
        public static string GetAppSetting(string key)
        {
            var keys = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
            if (keys == null || !keys.Contains(key))
                return null;
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value;
        }

        public static string GetConnectionString(string key)
        {
           string conn= System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
           return conn;
        }

        public static string GetADOConnectionString(string key)
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
            return conn.SubAfter("\"").SubBefore("\"");
        }
    }
}
