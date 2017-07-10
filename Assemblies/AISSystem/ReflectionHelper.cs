using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AISSystem
{
    public class ReflectionHelper
    {
        public static object GetValueFromProperty(object o, string property)
        {
            object value = null;
            if (o != null && !string.IsNullOrEmpty(property))
            {
                PropertyInfo propertyInfo = o.GetType().GetProperty(property);
                if (propertyInfo != null)
                {
                    value = propertyInfo.GetValue(o, null);

                }
            }
            return value;
        }


        public static List<string> GetDescendentFieldPath(Type type, string basePath)
        {
            try
            {
                List<string> paths = new List<string>();
                if (type != null)
                {
                    if (!IsPrimitive(type))
                    {
                        PropertyInfo[] pis = type.GetProperties();
                        if (pis != null && pis.Length > 0)
                        {
                            foreach (var pi in pis)
                            {
                                string subBasePath = string.IsNullOrEmpty(basePath) ? pi.Name : (basePath + "." + pi.Name);
                                List<string> subPaths = GetDescendentFieldPath(pi.PropertyType, subBasePath);
                                if (subPaths != null && subPaths.Count > 0)
                                    paths.AddRange(subPaths);
                                else
                                    paths.Add(subBasePath);
                            }
                        }
                    }
                }
                return paths;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static bool IsPrimitive(Type t)
        {
            return t.IsPrimitive || t.Name == "String" || t.Name.Contains("Nullable");
        }

        public static object MergValue(object from, object to)
        {
            if (IsDefaultValue(from))
                return to;
            if (IsDefaultValue(to))
                return from;

            Type type = to.GetType();
            if (!IsPrimitive(type))
            {
                PropertyInfo[] pis = type.GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var p = pi.GetValue(to, null);
                        var q = pi.GetValue(from, null);
                        if (!IsPrimitive(pi.PropertyType))
                        {
                            p = MergValue(q, p);
                        }
                        else
                        {
                            if (IsDefaultValue(p))
                                p = q;
                        }
                        pi.SetValue(to, p, null);
                    }
                }
            }
            return to;

        }

        private static bool IsDefaultValue(object o)
        {

            if (o == null)
                return true;
            Type t = o.GetType();
            if (t.Name == "String")
            {
                string s = o as string;
                return string.IsNullOrEmpty(s);
            }
            return false;

        }

        public static string ToString(object to, string basePath)
        {
            string str = "";
            Type type = to.GetType();
            if (!IsPrimitive(type))
            {
                PropertyInfo[] pis = type.GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var p = pi.GetValue(to, null);

                        if (!IsPrimitive(pi.PropertyType) && !IsDefaultValue(p))
                        {
                            string s = ToString(p, basePath + pi.Name);
                            str += s + "\r\n";
                        }
                        else
                        {
                            str += basePath + "." + pi.Name + ":" + (IsDefaultValue(p) ? "N/A" : p.ToString()) + "\r\n";
                        }

                    }
                }
            }
            return str;
        }

    }
}
