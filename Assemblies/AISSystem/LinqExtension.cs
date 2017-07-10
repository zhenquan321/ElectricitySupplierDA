using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AISSystem
{
    public static  class LinqExtension
    {
        #region 1 . Get Text
        /// <summary>
        /// Replace   XElement.Value
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns> 
        public static string GetValue(this XElement root)
        {
            return root == null ? null : root.Value;
        }

        public static string GetNextText(this XElement root)
        {
            return root != null && root.NextNode != null ? root.NextNode.ToString() : null;
        }

        public static string GetText(this XElement root, string value)
        {
            var txt = root == null ? null : root.DescendantNodes().FirstOrDefault(x => x is XText && x.ToString().Contains(value));
            return txt == null ? null : txt.ToString();
        }

        public static XText GetXText(this XElement root, string value)
        {
            XText xtext = null;
            if (root != null)
                xtext = root.DescendantNodes().LastOrDefault(x => x is XText && x.ToString().Contains(value)) as XText;
            return xtext;
        }

        public static List<XText> GetFollowSiblingXTexts(this XText xtxt)
        {
            List<XText> xtxts = null;
            if (xtxt != null)
            {
                var texts = xtxt.NodesAfterSelf().Where(x => x is XText).ToList();
                if (texts != null && texts.Count > 0)
                {
                    xtxts = new List<XText>();
                    foreach (var x in texts)
                    {
                        XText y = x as XText;
                        if (y != null)
                            xtxts.Add(y);
                    }
                }

            }
            return xtxts;
        }

        public static List<XText> GetPreceedingSiblingXTexts(this XText xtxt)
        {
            List<XText> xtxts = null;
            if (xtxt != null)
            {
                var texts = xtxt.NodesBeforeSelf().Where(x => x is XText).ToList();
                if (texts != null && texts.Count > 0)
                {
                    xtxts = new List<XText>();
                    foreach (var x in texts)
                    {
                        XText y = x as XText;
                        if (y != null)
                            xtxts.Add(y);
                    }
                }

            }
            return xtxts;
        }

        public static string GetMyText(this XElement root)
        {
            if (root == null)
                return null;
            var txts = root.Nodes().Where(x => x is XText).Select(x => x.ToString()).ToArray();
            if (txts == null || txts.Length == 0)
                return null;
            return string.Join("", txts);
        }


        #endregion

        #region 2 .Descendant Axis
        /// <summary>
        /// First Descendant.Name=tag
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static XElement GetDescendant(this XElement root, string tag)
        {
            return root == null ? null : root.Descendants(tag).FirstOrDefault();
        }
        /// <summary>
        /// Descendant.Value.Contians(value)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement GetDescendant(this XElement root, string tag, string value)
        {
            return root != null ? root.Descendants(tag).LastOrDefault(x => x.Value.Contains(value)) : null;
        }
        /// <summary>
        /// Descendant.Attribute.Value.Contains(value)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XElement GetDescendant(this XElement root, string tag, string attributeName, string attributeValue)
        {
            return root != null ? root.Descendants(tag).FirstOrDefault(x => x.Attribute(attributeName) != null && x.Attribute(attributeName).Value != null && x.Attribute(attributeName).Value.Contains(attributeValue)) : null;
        }

        public static XElement GetDescendantByClass(this XElement root, string tagName, string key)
        {
            return root.GetDescendant(tagName, "class", key);
        }

        public static XElement GetDescendantById(this XElement root, string tagName, string key)
        {
            return root.GetDescendant(tagName, "id", key);
        }
        /// <summary>
        /// Descendant.Value==value
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XElement GetDescendantByEqual(this XElement root, string tag, string attributeName, string attributeValue)
        {
            return root != null ? root.Descendants(tag).FirstOrDefault(x => x.Attribute(attributeName) != null && x.Attribute(attributeName).Value == attributeValue) : null;
        }

        public static List<XElement> GetDescendants(this XElement root, string tag)
        {
            return root != null ? root.Descendants(tag).ToList() : null;
        }

        /// <summary>
        /// Descendants.Value.Contains(value)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<XElement> GetDescendants(this XElement root, string tag, string value)
        {
            return root != null ? root.Descendants(tag).Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.Contains(value)).ToList() : null;
        }
        /// <summary>
        /// Descendants.Attribute.Contains(value)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static List<XElement> GetDescendants(this XElement root, string tag, string attributeName, string attributeValue)
        {
            return root != null ? root.Descendants(tag).Where(x => x.Attribute(attributeName) != null && x.Attribute(attributeName).Value.Contains(attributeValue)).ToList() : null;
        }
        #endregion

        #region 3 . Child axis
        /// <summary>
        /// Child.Name==tag
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static XElement GetChild(this XElement root, string tag)
        {
            return root == null ? null : root.Element(tag);
        }
        /// <summary>
        /// Childern.Name=tag
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xns"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static List<XElement> GetChildern(this XElement root, string tag)
        {
            return root != null && !string.IsNullOrEmpty(tag) ? root.Elements(tag).ToList() : null;
        }
        #endregion

        #region 4 . Sibling axis
        public static List<XElement> GetFollowSiblings(this XElement root, string tag)
        {
            return root != null ? root.ElementsAfterSelf(tag).ToList() : null;
        }

        public static XElement GetFollowSibling(this XElement root, string tag)
        {
            return root != null ? root.ElementsAfterSelf(tag).FirstOrDefault() : null;
        }

        public static List<XElement> GetPreceedingSiblings(this XElement root, string tag)
        {
            return root != null ? root.ElementsBeforeSelf(tag).ToList() : null;
        }

        public static XElement GetPreceedingSibling(this XElement root, string tag)
        {
            return root != null ? root.ElementsBeforeSelf(tag).FirstOrDefault() : null;
        }

        #endregion

        #region 5. Ancentor axis
        public static XElement GetAncetor(this XElement child, string tag)
        {
            return child != null ? child.Ancestors(tag).FirstOrDefault() : null;
        }

        public static List<XElement> GetAncetors(this XElement child, string tag)
        {
            return child != null ? child.Ancestors(tag).ToList() : null;
        }

        #endregion


        #region 7.Attribute
        public static string GetAttribute(this XElement root, string attribute)
        {
            return root != null && root.Attribute(attribute) != null ? root.Attribute(attribute).Value : null;
        }

        public static string GetAttribute(this XElement root, string tag, string attribute, string value)
        {
            return root.GetDescendant(tag, value).GetAttribute(attribute);
        }

        public static string GetAttribute(this XElement root, string tag, string attribute, string attributeName, string attributeValue)
        {
            return GetDescendant(root, tag, attributeName, attributeValue).GetAttribute(attribute);
        }

        public static string GetHref(this XElement root)
        {
            return root.GetAttribute("href");
        }

        public static string GetSrc(this XElement root)
        {
            return root.GetAttribute("src");
        }
        #endregion
    }
}
