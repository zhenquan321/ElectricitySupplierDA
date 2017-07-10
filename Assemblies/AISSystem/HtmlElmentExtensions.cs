using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AISSystem
{
    public static class HtmlElmentExtensions
    {
        #region  Descendants By tag

        public static List<HtmlElement> GetDescendants(this HtmlDocument doc, string tag)
        {
            if (doc == null || string.IsNullOrEmpty(tag))
                return null;
            return doc.GetElementsByTagName(tag).Cast<HtmlElement>().ToList();
        }

        public static List<HtmlElement> GetDescendants(this HtmlElement element, string tag)
        {
            if (element == null || string.IsNullOrEmpty(tag))
                return null;
            return element.GetElementsByTagName(tag).Cast<HtmlElement>().ToList();
        }

        #endregion 

        #region Descendants by tag and att  

        public static  List<HtmlElement> GetDescendants(this HtmlDocument doc, string tag,string att,string key)
        {
            if (doc == null || string.IsNullOrEmpty(tag))
                return null;
            return doc.GetElementsByTagName(tag).Cast<HtmlElement>().Where(x=>x.GetAttribute(att).IsContain(key)).ToList();
        }

        public static List<HtmlElement> GetDescendants(this HtmlElement element, string tag, string att, string key)
        {
            if (element == null || string.IsNullOrEmpty(tag))
                return null;
            return element.GetElementsByTagName(tag).Cast<HtmlElement>().Where(x => x.GetAttribute(att).IsContain(key)).ToList();
        }       

        #endregion 

        #region Descendants by tag and text

        public static List<HtmlElement> GetDescendants(this HtmlDocument doc, string tag, string key)
        {
            if (doc == null || string.IsNullOrEmpty(tag))
                return null;
            return doc.GetElementsByTagName(tag).Cast<HtmlElement>().Where(x => x.GetTxt().IsContain(key)).ToList();
        }

        public static List<HtmlElement> GetDescendants(this HtmlElement element, string tag, string key)
        {
            if (element == null || string.IsNullOrEmpty(tag))
                return null;
            return element.GetElementsByTagName(tag).Cast<HtmlElement>().Where(x => x.GetTxt().IsContain(key)).ToList();
        }
        #endregion 

        #region First/Last Descendant

        public static HtmlElement GetFirstDescendant(this HtmlElement element, string tag)
        {
            var ds = element.GetDescendants(tag);
            return ds == null ? null : ds.FirstOrDefault();
        }

        public static HtmlElement GetLastDescendant(this HtmlElement element, string tag)
        {
            var ds = element.GetDescendants(tag);
            return ds == null ? null : ds.LastOrDefault();
        }

        public static HtmlElement GetFirstDescendant(this HtmlElement element, string tag,string txt)
        {
            var ds = element.GetDescendants(tag);
            return ds == null ? null : ds.FirstOrDefault(x=>x.InnerText.IsContain(txt));
        }

        public static HtmlElement GetLastDescendant(this HtmlElement element, string tag,string txt)
        {
            var ds = element.GetDescendants(tag);
            return ds == null ? null : ds.LastOrDefault(x => x.InnerText.IsContain(txt));
        }

        public static HtmlElement GetFirstDescendant(this HtmlDocument doc, string tag, string txt)
        {
            var ds = doc.GetDescendants(tag);
            return ds == null ? null : ds.FirstOrDefault(x => x.InnerText.IsContain(txt));
        }

        public static HtmlElement GetLastDescendant(this HtmlDocument doc, string tag, string txt)
        {
            var ds = doc.GetDescendants(tag);
            return ds == null ? null : ds.LastOrDefault(x => x.InnerText.IsContain(txt));
        }

        public static HtmlElement GetFirstDescendant(this HtmlElement element, string tag, string att, string key)
        {
            var ds = element.GetDescendants(tag,att ,key );
            return ds == null ? null : ds.FirstOrDefault();
        }

        public static HtmlElement GetLastDescendant(this HtmlElement element, string tag, string att, string key)
        {
            var ds = element.GetDescendants(tag,att ,key);
            return ds == null ? null : ds.LastOrDefault();
        }

        public static HtmlElement GetFirstDescendant(this HtmlDocument doc, string tag, string att, string key)
        {
            var ds = doc.GetDescendants(tag, att, key);
            return ds == null ? null : ds.FirstOrDefault();
        }

        public static HtmlElement GetLastDescendant(this HtmlDocument  doc, string tag, string att, string key)
        {
            var ds = doc.GetDescendants(tag, att, key);
            return ds == null ? null : ds.LastOrDefault();
        }

        #endregion 

        #region GetNext

        public static HtmlElement GetNext(this HtmlElement element)
        {
            if (element == null)
                return null; 
            return  element .NextSibling ;
        }

        public static HtmlElement GetPrev(this HtmlElement element)
        {
            if (element == null || element .Parent==null )
                return null;
            return element.Parent.Children.Cast<HtmlElement>().FirstOrDefault(x=>x.NextSibling ==element );
        }

        public static HtmlElement GetAncentor(this HtmlElement element, string ancentorName)
        {
            HtmlElement currrent =element ;
            while (currrent != null && currrent.Parent != null &&! currrent .Parent .TagName.IsEq(ancentorName ,true ))
            {
                currrent = currrent.Parent;
            }

            return currrent != null && currrent.Parent != null ? currrent.Parent : null;
        }
        #endregion 

        public static string GetTxt(this HtmlElement element)
        {
            if (element == null)
                return null;
            return element.InnerText;
        }

        public static void DoClick(this HtmlElement element)
        {
            if (element == null)
                return;
            element.InvokeMember("click");
        }


        #region Helps

        public static bool IsEq(this string targ, string pat,bool ignorCase)
        {
            if (string.IsNullOrEmpty(targ))
                return string.IsNullOrEmpty(pat);
            if (string.IsNullOrEmpty(pat))
                return false;
            return string.Compare(targ, pat, ignorCase) == 0;
        }

        public static bool IsContain(this string targ, string pat)
        {
            if (string.IsNullOrEmpty(targ) || string.IsNullOrEmpty(pat))
                return false;
            return targ.Contains(pat);
        }

        static string DigitClear(string targ)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(targ))
            {
                foreach (var c in targ)
                {
                    if (Char.IsDigit(c))
                        sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static int ToInt2(this string targ)
        {
            string digits = DigitClear(targ);
            int i;
            if (!int.TryParse(digits, out i))
                return -1; 
            if (targ.Contains("万"))
                i = i * 10000;
            return i;
        }

        public static string ClearSpace(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var c in input)
            {
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static DateTime? ToDateTime2(this string input)
        {
            DateTime dt ;
            if (DateTime.TryParse(input, out dt))
                return dt;
            return null;
        }

        public static string SubAfter(this string input, string pat)
        {
            if (string.IsNullOrEmpty(input)||string.IsNullOrEmpty(pat ))
                return null;
            int index = input.IndexOf(pat);
            if (index == -1||index +pat.Length >=input.Length )
                return null;
            return input.Substring(index + pat.Length);
        }

        public static string SubBefore(this string input, string pat)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pat))
                return null;
            int index = input.IndexOf(pat);
            if (index == -1 )
                return null;
            return input.Substring(0,index );
        }
        #endregion 
    }
}
