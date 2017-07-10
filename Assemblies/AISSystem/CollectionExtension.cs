using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AISSystem
{
    public static class CollectionExtension
    {
        public static List<T> DistinctBy<T>(this List<T> collection, Func<T, object> selector)
        {
            if (collection == null)
                return null;
            List<T> list = new List<T>();
            var gs = collection.GroupBy(x => selector(x));
            foreach (var g in gs)
            {
                list.Add(g.First());
            }
            return list;
        }

        public static IEnumerable<T2> ConvertTo<T1, T2>(this IEnumerable<T1> list) where T2:class  
        {
            if (list == null || list.Count() == 0)
                return null;
            List<T2> result = new List<T2>();
            foreach (var t1 in list)
            {
                T2 t2 = t1 as T2;
                if (t2 != null)
                    result.Add(t2);
            }
            return result;
        }

        public static bool HasElement(this ICollection colletion)
        {
            return colletion != null && colletion.Count > 0;
        }

        public static int GetCount(this ICollection colletion)
        {
            return colletion == null ? 0 : colletion.Count;
        }

        public static bool RemoveAt<T1, T2>(this Dictionary<T1, T2> collection, T1 key)
        {
            bool success = false;
            if (collection != null && collection.ContainsKey(key))
            {
                lock (collection)
                {
                    collection.Remove(key);
                }
                success = true;
            }
            return success;
        }


        public static T MaxBy<T>(this IEnumerable<T> collection, Func<T, IComparable> selector) where T : class
        {
            if (collection == null || collection.Count() == 0)
                return default(T);
            T r = null;
            foreach (var a in collection)
            {
                if (r == null)
                {
                    r = a;
                    continue;
                }
                if (selector(a).CompareTo(selector(r)) > 0)
                    r = a;
            }
            return r;
        }
    }

    public class ItemComparer<T> : IEqualityComparer<T>
    {
        Func<T,T, bool> comparer;
        Func<T, object> hashcoder;
        public ItemComparer(Func<T, T, bool> comparer, Func<T, object> hashcoder)
        {
            this.comparer = comparer;
            this.hashcoder = hashcoder;
        }

        public bool Equals(T x, T y)
        {
            if (x == null || y == null)
                return false;
            return comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;
            return hashcoder(obj).ToString().GetHashCode();
        }
    }
}
