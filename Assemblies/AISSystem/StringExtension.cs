using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace AISSystem
{
    public static  class StringExtension
    {
        #region 1.For Web

        public static string GetUrlEncodedString(this string term, string charset)
        {
            Encoding e = string.IsNullOrEmpty(charset) ? Encoding.GetEncoding("utf-8") : Encoding.GetEncoding(charset);
            return !string.IsNullOrEmpty(term) ? HttpUtility.UrlEncode(term, e).ReplaceWith("+", "%20") : null;
        }

        public static string GetUrlDecodedString(this string input)
        {
            return !string.IsNullOrEmpty(input) ? HttpUtility.HtmlDecode(input.ReplaceWith("&amp;", "&")) : null;
        }

        public static string GetUrlDecodedString2(this string input)
        {
            Dictionary<string, string> asciiMap = new Dictionary<string, string>();
            Encoding en = Encoding.GetEncoding("utf-8");
            char c = (char)0;
            for (; c < 256; c++)
            {
                var encd = HttpUtility.UrlEncode(c.ToString(), en).ToUpper();
                if (encd.Length == 1)
                    continue;
                asciiMap.Add(encd, c.ToString());
                input = input.Replace(encd, c.ToString());
            }
            input = input.Replace("%20", " ");

            int index = 0;
            StringBuilder sb = new StringBuilder();
            //txt = "%E5%B0%8F%E7%B1%B3"; 
            for (; index < input.Length; )
            {
                StringBuilder temp = new StringBuilder();
                while (index + 2 < input.Length && input[index] == '%')
                {
                    temp.Append(input[index]);
                    temp.Append(input[index + 1]);
                    temp.Append(input[index + 2]);
                    index += 3;
                }
                if (temp.Length > 0)
                {
                    sb.Append(HttpUtility.UrlDecode(temp.ToString()));
                }
                else
                {
                    sb.Append(input[index]);
                    index++;
                }
            }

           input = sb.ToString();
           return input;
        }

        static char GetCh(string txt)
        {
            int i = ToHex(txt[1])
                + 16 * ToHex(txt[2])
                + ToHex(txt[4]) * 16 * 16
                + ToHex(txt[5]) * 16 * 16 * 16
                + ToHex(txt[7]) * 16 * 16 * 16 * 16
                + ToHex(txt[8]) * 16 * 16 * 16 * 16 * 16;
            return (char)i;
        }
        static int ToHex(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            else if (c >= 'A' && c <= 'F')
                return 10 + (c - 'A');
            return -1;
        }

         

        public static string ClearIllegalChar(this string data, string charset)
        {
            if (!string.IsNullOrEmpty(data))
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in data.ToCharArray())
                {
                    if (c < 256)
                        sb.Append(c);
                    else
                        sb.Append(c.ToString().GetUrlEncodedString(charset));
                }
                return sb.ToString();
            }
            return null;
        }
        #endregion

        #region 2.Override
        public static byte[] GetBytes(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return System.Text.Encoding.Default.GetBytes(input);                
        }

        public static string ReplaceWith(this string input, string oldValue, string newValue)
        {
            string str = input.GetCopy();
            int index = string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(input) ? -1 : str.GetIndex(oldValue);
            newValue = !string.IsNullOrEmpty(newValue) ? newValue : "";
            if (index > -1 && oldValue != newValue)
                str = str.Replace(oldValue, newValue);
            return str;
        }

        public static string ReplaceWith(this string input, char oldValue, char newValue)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            string str = input.GetCopy();
            int index =  str.IndexOf(oldValue); 
            if (index > -1 && oldValue != newValue)
                str = str.Replace(oldValue, newValue);
            return str;
        }

        public static string RemoveLowOrderASCIICharacters(this string tmp)
        {
            if (string.IsNullOrEmpty(tmp))
                return null;

            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    continue;
                else
                    info.Append(cc);
            }
            return info.ToString();
        }

        public static string ReplaceNullWithEmpty(this string input)
        {
            return string.IsNullOrEmpty(input) ? "" : input;
        }

        public static string SubstringAfter(this string input, string str)
        {
            int index = input.GetIndex(str);
            if (index > -1 && index + str.Length < input.Length)
                return input.Substring(index + str.Length);
            return null;
        }

        public static string SubstringBefore(this string input, List<string> strs)
        {
            string str = input.GetCopy();
            if (strs != null && strs.Count > 0)
                foreach (string s in strs)
                    str = str.SubstringBefore(s);
            return str;
        }

        public static string SubstringBefore(this string input, string str)
        {
            int index = input.GetIndex(str);
            if (index > -1 && index > 0)
                return input.Substring(0, index);
            return null;
        }

        public static string Truct(this string input, int len)
        {
            if(string.IsNullOrEmpty (input )||input .Length <=len )
                return input ;
            return input.Substring(0, len);
        }

        public static string GetContact(this string input, string str)
        {
            if (string.IsNullOrEmpty(input))
                return str.GetCopy();
            else if (string.IsNullOrEmpty(str))
                return input.GetCopy();
            else
                return input + str;

        }

        public static string GetCopy(this string input)
        {
            return string.IsNullOrEmpty(input) ? null : string.Copy(input);
        }

        public static int GetIndex(this string input, string str)
        {
            return GetIndex(input, 0, str);
        }

        public static int GetIndex(this string str, int start, string model)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(model) || start < 0 || start >= str.Length)
                return -1;
            char c = model[0];
            int index = str.IndexOf(c, start);
            while (index > -1)
            {
                if (index + model.Length > str.Length)
                    return -1;
                string s = str.Substring(index, model.Length);
                if (s == model)
                    break;
                start = index + 1;
                index = str.IndexOf(c, start);
            }
            return index;
        }

        public static bool IsAlphabet(this char c)
        {
            return !((c > 'z' && c > 'Z') || (c < 'a' && c < 'A'));
        }

        static Regex letterRegex = new Regex("[^a-z0-9]");
        public static string ToXName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            //name = name.Replace(':', '_').ToLower();
            if (!IsAlphabet(name[0]))
                name = "X" + name;
            name = name.ToLower();
            name = letterRegex.Replace(name, "");
            if (string.IsNullOrEmpty(name))
                name = "Tag";
            return name;
        }

        public static  string GetTrimed(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input.Trim();
        }

        public static string GetTrimedStart(this string input, char c)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input.TrimStart(c);
        }

        public static string RemoveSpace(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (c == ' ')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string RemoveAscii(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (c > 255)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static bool IsContains(this string input, string value)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(value ))
                return false;
            return input.Contains(value);

        }

        public static bool IsContainsAny(this string input, params string[] value)
        {
            if (string.IsNullOrEmpty(input) || value == null || value.Length == 0)
                return false;
            return value.Any(x => input.IsContains(x));

        }

        public static bool IsContainsAll(this string input, params string[] value)
        {
            if (string.IsNullOrEmpty(input) || value == null || value.Length == 0)
                return false;
            return value.All(x => input.IsContains(x));

        }

        public static int ContainCount(this string input, char c)
        {
            if (string.IsNullOrEmpty(input) )
                return 0;
            string[] strs = input.Split(new char[]{c });
            if (strs != null)
                return strs.Length - 1;
            return 0;
        }

        public static string GetLower(this string input)
        {
            return string.IsNullOrEmpty(input) ? null : input.ToLower();
        }

        public static string GetTruct(this string input, int length)
        {
            if (string.IsNullOrEmpty(input) || input.Length<= length)
                return input;
            return input.Substring(0, length);
        }

        public static bool IsStartWith(this string input, string pat)
        {
            if (string.IsNullOrEmpty(pat) || string.IsNullOrEmpty (input ))
                return false;
            return input.StartsWith(pat);
        }
        #endregion

        #region 3.Conversion
        public static int? ToInt(this string input)
        {
            int x;
            if (int.TryParse(input, out  x))
                return x;
            return null;

        }

        public static int? ExInt(this string input)
        {
            if (!string.IsNullOrEmpty(input) && Regex.Match(input, @"\d+").Success)
                return Convert.ToInt32(Regex.Match(input, @"\d+").Value);
            return null;

        }

        public static long? ExLong(this string input)
        {
            if (!string.IsNullOrEmpty(input) && Regex.Match(input, @"\d+").Success)
                return Convert.ToInt64(Regex.Match(input, @"\d+").Value);
            return null;
        }

        public static double? ExDouble(this string input)
        {
            if (!string.IsNullOrEmpty(input) && Regex.Match(input, @"\d+\.*\d*").Success)
                return Convert.ToDouble(Regex.Match(input, @"\d+\.*\d*").Value);
            return null;
        }

        public static double? ToDouble(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            double x;
            if (double.TryParse(input, out  x))
                return x;
            return null;
        }

        public static Guid? ToGuid(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            Guid id;
            if (Guid.TryParse(input, out id))
                return id;
            return null;
        }

        public static bool? ToBool(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input.IsEq("true", true);
        }

        public static byte? ToByte(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            byte b;
            if (input == null || !byte.TryParse(input ,out b ))
                return null;
            return b;
        }

        public static DateTime? ToDateTime(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            DateTime dt;
            if (DateTime.TryParse(input, out dt))
                return dt;
            return null;
        }

        public static double? ToDateKey(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            input = input.GetDigital();
            if (input.Length > 6)
                input = input.Substring(0, 6) + "." + input.Substring(6);
            return input.ToDouble();
        }

        static readonly Regex ChinaPhoneNoRegex = new Regex(@"\d*\s*\-*\d*\s*\-*\d{6,9}");
        static readonly Regex ChinaMobilePhoneNoRegex = new Regex(@"1\d{10}");

        public static string ExChinaPhoneNo(this string input)
        {
            return ChinaPhoneNoRegex.GetMatchValue(input);
        }

        public static string ExChinaMobilePhoneNo(this string input)
        {
            return ChinaMobilePhoneNoRegex.GetMatchValue(input);
        }

        public static string GetMatchValue(this Regex regex, string input)
        {
            Match match = !string.IsNullOrEmpty(input) && regex != null ? regex.Match(input) : null;
            return match != null && match.Success ? match.Value : null;
        }

        public static string FormatStr(this string format, params object[] objs)
        {
            return string.Format(format, objs);
        }

        public static string[] SplitWith(this string input, params string[] ops)
        {
            if(string.IsNullOrEmpty(input ) || ops ==null || ops.Length ==0)
                return null ;
            return input.Split(ops, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitWithAscii(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length == 0)
                return null;
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();
            int index = -1;
            while (index < input.Length-1)
            {
                index++;
                if (input[index] > 255)
                {
                    sb.Append(input[index]);
                    continue;
                }
                if (sb.Length == 0)
                    continue;
                list.Add(sb.ToString());
                sb.Clear();
            }
            return list.ToArray();
        }

        public static string[] SplitBy(this string input, params string[] ops)
        {
            if (string.IsNullOrEmpty(input) || ops == null || ops.Length == 0)
                return null;
            List<string> list = new List<string>() { input };
            string op = "";
            int index = input.GetFirstIndex(ref op, ops);
            while (index >= 0)
            {
                string r = (index > 0 ? input.Substring(0, index) : "") + op;
                list.Add(r);
                input = input.Substring(index + op.Length);
                index = input.GetFirstIndex(ref op, ops);
            }
            return list.ToArray();
        }

        public static int GetFirstIndex(this string input, ref string op, params string[] ops)
        {
            op = null;
            if (string.IsNullOrEmpty(input) || ops == null || ops.Length == 0)
                return -1;
            int index = -1;
            foreach (var o in ops)
            {
                int a = input.IndexOf(o);
                if (a > -1 && (a < index || index == -1))
                {
                    op = o;
                    index = a;
                }
            }
            return index;
        }

        #endregion

        #region Data Clear

        public static string GetWeiChatNum(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null ;
            string regexExpresion = @"微信\w{0,3}号*码*[：|:]*\s*[a-z|A-Z|0-9\-|_]+\s*";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value.GetAlpaOrNumerString();
        }

        public static string GetQQNum(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"[q|Q][q|Q]\w{0,3}号*码*[：|:]*\s*[0-9]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value.GetNumerString();
        }

        public static string GetTelePhoneNumber(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"电\s*话\w{0,3}\s*号*\s*码*[：|:]*\s*[0-9\-_]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value.GetNumerOrDashString();
        }

        public static string GetMobPhoneNumber(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"手\s*机\w{0,3}\s*号*\s*码*[：|:]*\s*[0-9\-_]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value.GetNumerString();
        }

        public static string GetEmail(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"[0-9\-_a-zA-Z\.]+\@[0-9\-_a-zA-Z\.]+\.[a-zA-Z]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value;
        }

        public static  string GetAlpaOrNumerString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"[a-z|A-Z|0-9\-|_]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value;

        }

        public static string GetNumerString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"[0-9]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value;
        }

        public static string GetNumerOrDashString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string regexExpresion = @"[0-9\-]+";
            Regex regex = new Regex(regexExpresion);
            var m = regex.Match(text);
            if (!m.Success)
                return null;
            return m.Value;
        }

        public static string GetDigital(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (char.IsDigit(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion 

        #region string builder

        public static StringBuilder  AppendAtEnd(this StringBuilder sb, string value)
        {
            if (!string.IsNullOrEmpty(value) && sb!=null )
                sb.Append(value);
            return sb;

        }

        public static StringBuilder AppendAtEnd(this StringBuilder sb, object  value)
        {
            if (value !=null  && sb != null)
                sb.Append(value.ToString());
            return sb;
        }
        #endregion

        public static string SubLastStringAfter(this string input, string str)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(str))
                return null;
            int index = input.LastIndexOf(str);
            if (index > -1 && index + str.Length < input.Length)
                return input.Substring(index + str.Length);
            return null;
        }

        public static string SubLastStringBefore(this string input, string str)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(str))
                return null;
            int index = input.LastIndexOf(str);
            if (index > -1 && index > 0)
                return input.Substring(0, index);
            return null;
        }

        //去掉 <xxxxx>  包括<xxxx>,</xxxx>
        //  href="a">abcd <para>xs</para>   处理完得到 abcdxs
        public static string GetTxtFromHtml(this string html)
        {
            int indexOfLeft = html.GetIndex("<");
            int indexOfRight = html.GetIndex(">");
            if (indexOfRight >= 0 && indexOfRight < indexOfLeft && indexOfRight < html.Length - 1)
                return GetTxtFromHtml(html.Substring(indexOfRight + 1));
            if (!html.IsContain("<"))
                return html;
            html = html.SubBefore("<").GetContact(html.SubAfter(">"));
            return GetTxtFromHtml(html);
        }

    }
}
