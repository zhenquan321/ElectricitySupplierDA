using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AISSystem
{
    public static class CommonExtensions
    {
        public static double ToDateKey2(this DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day+0.01*dt.Hour +0.0001*dt.Minute+0.000001*dt.Second+0.00000001*dt.Millisecond/10;
        }

        public static int ToDateKey(this DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }

        public static DateTime? ToDateTime(this int dk)
        {
            try
            {
                return new DateTime(dk / 10000, dk / 100 % 100, dk % 100);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime ToExDateTime(this double dk2)
        {
            try
            {
                int dk=(int)dk2 ;
                double dk3= dk2 -dk ;
                int h = (int)(dk3 *100);
                int m = ((int)(dk3 *10000))%100;
                int s = ((int)(dk3 *1000000))%100;
                int ms = ((int)(dk3 *100000000))%100;
                return new DateTime(dk / 10000, dk / 100 % 100, dk % 100,h ,m ,s ,ms );
            }
            catch
            {
                return DateTime .Now  ;
            }
        }

        public static int? Add(this int? a, int? b)
        {
            if (!b.HasValue)
                return a;
            if (!a.HasValue)
                return b;
            return a.Value + b.Value;
        }

        public static bool Eq(this int? a, int? b)
        {
            if (!a.HasValue)
                return !b.HasValue;
            if (!b.HasValue)
                return false;
            return a.Value == b.Value;
        }

        public static bool Changed(this int? a, int? b)
        {
            return !a.Eq(b) && a.HasValue;
        }

        public static double? Add(this double? a, double? b)
        {
            if (!b.HasValue)
                return a;
            if (!a.HasValue)
                return b;
            return a.Value + b.Value;
        }

        public static bool IsNullOrEmpty(this Guid? id)
        {
            return !id.HasValue || id.Value == Guid.Empty; 
        }

        public static bool Changed(this string a, string b)
        {
            return a != b && !string.IsNullOrEmpty(a);
        }

        public static void Replace(this string a, ref string b)
        {
            if (!a.Changed(b))
                return;
            b = a;
        }

        public static void Replace(this int? a, ref  int? b)
        {
            if (!a.Changed(b))
                return;
            b = a;
        }

        public static Nullable<T> GetField<T>(this DataRow row, string name) where T : struct
        {
            Nullable<T> r = null;
            object o = row.Field<object>(name);
            if (o != null)
                r = (T)o;
            return r;
        }

        //public void SetProperties<T>(this object targ,object source)
        //{
        //    T d = default(T);
        //   var ps= targ.GetType().GetProperties().Where(x =>(x.GetValue(targ, null) is T) && x.CanWrite && x.CanRead );
        //   foreach (var p in ps)
        //   {
        //       T s = (T)p.GetValue(source, null);
        //       T t = (T)p.GetValue(source, null); 
        //       if(d.Equals  )
        //   }
        //}
    }
}
