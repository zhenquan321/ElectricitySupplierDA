using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography; 
namespace AISSystem
{
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
            if(string.IsNullOrEmpty(input ))
                return Guid .Empty ;
            byte[] bytes = ComputeMD5ForString(input);
            return new Guid(bytes);
        }

      

    }
}
