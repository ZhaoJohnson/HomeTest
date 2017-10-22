using System;
using System.Collections.Generic;
using System.Linq;

namespace HpaUtility.Extensions
{
    public static class ListExtensions
    {
        public static List<TObj> Merge<TObj>(this List<TObj> list, params TObj[] objs)
        {
            if (list == null || objs == null || objs.Length <= 0)
                return list;
            foreach (var obj in objs.Where(r => !list.Contains(r)).Where(obj => obj != null))
            {
                list.Add(obj);
            }
            return list;
        }

        public static List<TObj> Merge<TObj>(this List<TObj> list, List<TObj> list2)
        {
            if (list == null || list2 == null || !list2.Any())
                return list;
            foreach (var obj in list2.Where(r => !list.Contains(r)).Where(obj => obj != null))
            {
                list.Add(obj);
            }
            return list;
        }

        public static List<TObj> Merge<TObj, TObj1>(this List<TObj> list, List<TObj1> list2, Func<TObj1, TObj> pickFn)
        {
            if (list == null || list2 == null || list2.Count() <= 0)
                return list;
            foreach (var obj in list2.Where(r => !list.Contains(pickFn(r))).Where(obj => obj != null))
            {
                list.Add(pickFn(obj));
            }
            return list;
        }

        #region

        public static List<TObj> Combine<TObj, TObj1>(this List<TObj> list, List<TObj1> list1, Func<TObj1, TObj> pickFn)
        {
            if (list == null)
                return null;

            if(list1 != null)
                list.AddRange(list1.Select(r=>pickFn(r)));

            return list;
        }

        public static List<TObj> Combine<TObj>(this List<TObj> list, Func<TObj, bool> whereFn = null, params List<TObj>[] lists)
        {
            if (list == null || lists == null || lists.Length <= 0)
                return null;

            foreach (var attach in lists)
            {
                if(whereFn == null)
                    list.AddRange(attach);
                else 
                    list.AddRange(attach.Where(r=> whereFn(r)));
            }

            return list;
        }

        #endregion
    }
}
