using AISSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuerjiudian
{
    public class EncryptHelper
    {
        const string scret_key = "&*(TIYTYF&%^*(^^";
        public static string GetOrderToken(string order_code)
        {
            return AISSystem.IDHelper.GetGuid(string.Format("{0}{1}", order_code, scret_key)).ToString();
        }

        public static bool VerifyOrderToken(string token, string order_code)
        {
            string _token = GetOrderToken(order_code);
            return _token == token;
        }

        const string pwd_key = "&*^%%*(%%&**^^";

        public static Guid GetEncryPwd(string pwd)
        {
            return IDHelper.GetGuid(pwd + pwd_key);
        }
    }
}