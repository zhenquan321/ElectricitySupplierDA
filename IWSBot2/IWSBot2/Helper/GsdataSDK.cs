using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace GSDataAPI
{
    /// <summary>
    /// 清博微信数据SDK
    /// </summary>
    class GSDataSDK
    {
        private string Appid;       //用户唯一标识
        private string Appkey;      //用户签名私钥

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appid">用户唯一标识</param>
        /// <param name="appkey">用户签名私钥</param>
        public GSDataSDK(string appid, string appkey)
        {
            Appid = appid;
            Appkey = appkey;
        }

        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="postData">接口参数，无appid和appkey</param>
        /// <param name="url">接口地址</param>
        /// <returns>查询结果</returns>
        public string Call(IDictionary<string, object> postData, string url)
        {
            //添加appid，按key升序排列并Json化
            postData.Add("appid", Appid);
            postData = postData.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            string jsonStr = GetJson(postData);

            //对字符串小写+appkey值进行MD5加密生成签名signature并放入post数据中
            string str = jsonStr.ToLower() + Appkey;
            string md5 = GetMD5(str);
            postData.Add("signature", md5);

            //再度json化
            jsonStr = GetJson(postData);
            //将字符串的字节数组通过base64加密成字节数组，以post的方式发送
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
            string base64Str = Convert.ToBase64String(bytes);
            string response = HttpPost(url, base64Str);
            return response;
        }

        /// <summary>
        /// 生成Json字符串
        /// </summary>
        /// <param name="postData">双字符串字典</param>
        /// <returns>Json字符串</returns>
        private string GetJson(IDictionary<string, object> data)
        {
            string retString = "{";
            foreach (var item in data)
            {
                //判断值是否含有中文，是则将其中的中文转换为Unicode形式
                string value = null;
                Regex reg = new Regex("[\u4e00-\u9fa5]");
                if (reg.IsMatch(item.Value.ToString()))
                {
                    StringBuilder strBu = new StringBuilder();
                    foreach (char y in item.Value.ToString())
                    {
                        string str = y.ToString();
                        if (reg.IsMatch(str))
                        {
                            str = StringToUnicode(str);
                        }
                        strBu.Append(str);
                    }
                    value = strBu.ToString();
                }
                else
                    value = item.Value.ToString();

                retString += "\"" + item.Key + "\":\"" + value + "\",";
            }
            retString += "}";
            retString = retString.Replace(",}", "}");
            return retString;
        }

        /// <summary>
        /// 生成MD5编码
        /// </summary>
        /// <param name="myString">源字符串</param>
        /// <returns>MD5字符串</returns>
        private string GetMD5(string myString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(myString);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("x2");
            }
            return sTemp.ToLower();
        }

        /// <summary>
        /// 模拟Post请求
        /// </summary>
        /// <param name="Url">接口地址</param>
        /// <param name="postDataStr">post字符串</param>
        /// <returns>返回数据</returns>
        private string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>  
        /// 字符串转为UniCode码字符串
        /// </summary>  
        /// <param name="s">源字符串</param>  
        /// <returns>UniCode码字符串</returns>  
        private string StringToUnicode(string s)
        {
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }  
    }
}
