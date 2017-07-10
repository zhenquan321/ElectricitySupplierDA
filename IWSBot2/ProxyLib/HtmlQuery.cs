using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;

namespace ProxyLib
{
     public static class HtmlQuery
    {
        /// <summary>
        /// 比较适合 a, p, span, 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tagName"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<string> GetDescendentsByKeys(this string html, string tagName, params string[] keys)
        {
            string[] sps = html.SplitWith("<" + tagName);
            if (sps == null || sps.Length == 0)
                return null;
            List<string> list = new List<string>(sps);
            if (!html.IsStartWith("<" + tagName))
                list.RemoveAt(0);
            list = list.Where(x => x.GetTxtFromHtml().IsContains2(keys)).ToList();
            return list;
        }

        // <a href="xxx/nod?..."> ........<a href="yyyy/nod?...">.... tagName= a , attrName=href, keys={"nod? "} 取出所有 href属性中包含 nod? 的a
        public static List<string> GetDescendents(this string html, string tagName, string attrName, params string[] keys)
        {
            if (!html.IsContains(tagName))
                return null;
            //a,span,p, li ,  <div><div>啊啊啊</div></div>
            string[] sps = html.SplitWith("<" + tagName);
            if (sps == null || sps.Length == 0)
                return null;

            List<string> list = new List<string>(sps);
            if (!html.IsStartWith("<" + tagName))
                list.RemoveAt(0);
            if (keys != null && keys.Length > 0)
            {
                list = list.Where(x => x.GetFirstAttributeValue(attrName).IsContains2(keys)).ToList();
            }
            else
            {
                list = list.Select(x => x.GetFirstAttributeValue(attrName)).ToList();
            }
            return list;
        }

        // <a href="xxx/nod?..."> ........<a href="yyyy/nod?...">.... tagName= a , attrName=href, keys={"nod? "} 取出所有 href属性中包含 nod? 的a
        public static string GetFirstDescendent(this string html, string tagName, string attrName, params string[] keys)
        {

            var list = GetDescendents(html, tagName, attrName, keys);
            return list == null || list.Count == 0 ? null : list.First().SubstringBefore("</" + tagName);
        }

        // <a href="xxx/nod?..."> ........<a href="yyyy/nod?...">.... tagName= a , attrName=href, keys={"nod? "} 取出所有 href属性中包含 nod? 的a
        public static string GetFirstDescendent(this string html, string tagName, string key)
        {

            var list = GetDescendentsByKeys(html, tagName, key);
            return list == null || list.Count == 0 ? null : list.First().SubstringBefore("</" + tagName);
        }

        public static string GetLastDescendent(this string html, string tagName, string attrName, params string[] keys)
        {

            var list = GetDescendents(html, tagName, attrName, keys);
            return list == null || list.Count == 0 ? null : list.Last().SubLastStringBefore("</" + tagName);
        }

        public static List<string> GetAllAttrs(this string html, string att)
        {
            string[] txts = html.SplitWith(att + "=");
            if (txts == null)
                return null;
            return txts.ToList().Select(x => GetValeBetweenQuota(x)).ToList();
        }

        ////<a href="a">abcd <para>xs</para> href="b">abcd <para>xs</para> tagName=1,attrName=href,   处理完得到 a
        public static string GetFirstAttributeValue(this string html, string tagName, string attrName)
        {
            return html.SubstringAfter("<" + tagName).SubstringAfter(attrName + "=").GetFirstAttributeValue().GetTrimed();
        }


        public static string GetFirstHref2(this string html)
        {
            html = html.SubstringAfter("href=");
            return GetFirstAttributeValue(html);
        }

        //href="a">abcd <para>xs</para> href="b">abcd <para>xs</para>  处理完得到 b
        public static string GetLastHref2(this string html)
        {
            html = html.SubLastStringAfter("href=");
            return GetFirstAttributeValue(html);
        }

        //href="a">abcd <para>xs</para> href="b">abcd <para>xs</para>  处理完得到 a
        public static string GetFirstAttributeValue(this string html, string attr)
        {
            return html.SubstringAfter(attr + "=").GetFirstAttributeValue();
        }

        // href="a">abcd <para>xs</para>   处理完得到 a
        public static string GetFirstAttributeValue(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            html = html.GetTrimed();
            int indexOfSpace = html.GetIndex(" ");
            int indexOfGt = html.GetIndex(">");
            int indexOfQuator = html.GetIndex("\"");
            //href="a b c">abcd <para>xs</para> 
            if (indexOfQuator >= 0 && indexOfQuator < indexOfSpace && indexOfQuator < indexOfGt)
                return html.GetValeBetweenQuota();
            if (indexOfGt < 0 && indexOfSpace < 0)
                return html.GetValeBetweenQuota();
            if (indexOfSpace < 0)
                return html.SubstringBefore(">").GetValeBetweenQuota();
            if (indexOfGt < 0)
                return html.SubstringBefore(" ").GetValeBetweenQuota();
            int index = Math.Min(indexOfGt, indexOfSpace);
            html = html.Substring(0, index).GetValeBetweenQuota();
            return html;

        }

        //去掉 <xxxxx>  包括<xxxx>,</xxxx>
        //  href="a">abcd <para>xs</para>   处理完得到 abcdxs
         /// <summary>
         /// 去除网页标签
         /// </summary>
         /// <param name="html">网页源码</param>
         /// <returns></returns>
        public static string GetTxtFromHtml2(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";
            int indexOfLeft = html.GetIndex("<");
            int indexOfRight = html.GetIndex(">");
            //aaaa
            if (indexOfLeft < 0 && indexOfRight < 0)
                return html.ReplaceWith("&nbsp;", "");
            // href='''>aba
            if (indexOfLeft < 0)
            {
                if (indexOfRight >= html.Length - 1)
                    return "";
                return html.Substring(indexOfRight + 1);
            }
            // abc <b
            if (indexOfRight < 0)
            {
                if (indexOfLeft == 0)
                    return "";
                return html.Substring(0, indexOfLeft);
            }
            //abc<a>cd<
            if (indexOfLeft < indexOfRight)
            {
                string a = "";
                if (indexOfLeft > 0)
                    a = html.Substring(0, indexOfLeft);
                if (indexOfRight >= html.Length - 1)
                    return a;
                string b = html.Substring(indexOfRight + 1);
                if (string.IsNullOrEmpty(b))
                    return a;
                html = (a + b);
            }
            if (indexOfLeft > indexOfRight)
            {
                html = html.Substring(indexOfRight + 1);
            }
            return html.GetTxtFromHtml2();

            // href >
            //if (indexOfLeft < 0 && indexOfRight > 0 && indexOfRight < html.Length - 1)
            //    return html.Substring(indexOfRight + 1).GetTrimed();
            //if (indexOfRight >= 0 && indexOfRight < indexOfLeft && indexOfRight < html.Length - 1)
            //    return GetTxtFromHtml(html.Substring(indexOfRight + 1));
            //if (!html.IsContains("<"))
            //    return html.GetTrimed();
            //html = html.SubstringBefore("<").GetContact(html.SubstringAfter(">"));
            //return GetTxtFromHtml(html);
        }

        public static string GetValeBetweenQuota(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            if (html.IsContains("\""))
                return html.SubstringAfter("\"").SubstringBefore("\"");
            return html;
        }

        public static string RemoveHtmlCode(this string html)
        {
            return html.ReplaceWith("&nbsp;", " ");
        }
    }
}
