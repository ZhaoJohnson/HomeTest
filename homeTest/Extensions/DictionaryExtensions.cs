using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HpaUtility.Extensions
{
    public static class DictionaryExtensions
    {
        #region Get Method

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey key, TValue defVal = default(TValue))
        {
            if(dic == null)
                return defVal;

            if (dic.ContainsKey(key))
                return dic[key];

            return defVal;
        }

        public static List<TKey> GetKeysBy<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            Func<KeyValuePair<TKey, TValue>, bool> checkFn = null)
        {
            if (checkFn == null)
                return (from d in dic select d.Key).ToList();
            return (from d in dic where checkFn(d) select d.Key).ToList();
        }

        public static List<TValue> GetValuesBy<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            Func<KeyValuePair<TKey, TValue>, bool> checkFn = null)
        {
            if (checkFn == null)
                return (from d in dic select d.Value).ToList();
            return (from d in dic where checkFn(d) select d.Value).ToList();
        }

        public static TValue GetFirstValueBy<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            Func<TKey, bool> matchFn)
        {
            if (dic == null)
                return default(TValue);
            foreach (var key in dic.Keys) 
            {
                if (matchFn(key))
                    return dic[key];
            }
            return default(TValue);
        }

        #endregion

        #region Merge

        public delegate void DicValueOperator<in TValue>(TValue value);

        #region From Object

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey key, DicValueOperator<TValue> valOperator, Func<TValue> newValObjFn)
        {
            if (dic == null || key == null)
                return dic;
            TValue value;

            if (dic.ContainsKey(key))
                value = dic[key];
            else
            {
                value = newValObjFn();
                dic.Add(key, value);
            }
            valOperator(value);
            return dic;
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey key, TValue value)
        {
            if (dic == null || key == null || value == null)
                return dic;
            if (dic.ContainsKey(key))
                dic.Remove(key);
            dic.Add(key, value);
            return dic;
        }

        public static IDictionary<TKey, List<TValue>> Merge<TKey, TValue>(this IDictionary<TKey, List<TValue>> dic,
            TKey key, params TValue[] values)
        {
            if (dic == null || key == null || values == null || values.Length <= 0)
                return dic;
            List<TValue> valueList;
            if (dic.ContainsKey(key))
                valueList = dic[key];
            else
            {
                valueList = new List<TValue>();
                dic.Add(key, valueList);
            }
            valueList.Merge(values);
            return dic;
        }

        public static IDictionary<TKey, List<TValue>> Merge<TKey, TValue, TSource>(this IDictionary<TKey, List<TValue>> dic,
            IDictionary<TKey, List<TSource>> source, Func<TSource, TValue> getValueFromSourceFn)
        {
            if (dic == null || source == null || getValueFromSourceFn == null)
                return dic;

            foreach (var key in source.Keys)
            {
                List<TValue> valueList;
                if (dic.ContainsKey(key))
                    valueList = dic[key];
                else
                {
                    valueList = new List<TValue>();
                    dic.Add(key, valueList);
                }

                var sourceList = source[key];
                if(sourceList == null)
                    continue;

                valueList.AddRange(sourceList.Select(getValueFromSourceFn));
                
            }
            return dic;
        }

        #endregion

        #region From List

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IEnumerable<TValue> source,
            Func<TValue, TKey> getKeyFromSourceFn)
        {
            return Merge(dic, source, getKeyFromSourceFn, r => r);
        }   
         
        public static IDictionary<TKey, TValue> Merge<TKey, TValue, TSource>(this IDictionary<TKey, TValue> dic,
            IEnumerable<TSource> source,
            Func<TSource, TKey> getKeyFromSourceFn, Func<TSource, TValue> getValueFromSourceFn)
        {
            if (source == null || !source.Any())
                return dic;
            foreach (var item in source)
            {
                dic.Merge(getKeyFromSourceFn(item), getValueFromSourceFn(item));
            }
            return dic;
        }

        public static IDictionary<TKey, List<TValue>> Merge<TKey, TValue>(this IDictionary<TKey, List<TValue>> dic,
            IEnumerable<TValue> source,
            Func<TValue, TKey> getKeyFromSourceFn)
        {
            return Merge(dic, source, getKeyFromSourceFn, r=>r);
        }

        public static IDictionary<TKey, List<TValue>> Merge<TKey, TValue, TSource>(this IDictionary<TKey, List<TValue>> dic,
            IEnumerable<TSource> source,
            Func<TSource, TKey> getKeyFromSourceFn, Func<TSource, TValue> getValueFromSourceFn)
        {
            if (dic == null || source == null || !source.Any() ||
                getKeyFromSourceFn == null || getValueFromSourceFn == null)
                return dic;

            foreach (var item in source)
            {
                TKey key = getKeyFromSourceFn(item);
                List<TValue> valueList;
                if (dic.ContainsKey(key))
                    valueList = dic[key];
                else
                {
                    valueList = new List<TValue>();
                    dic.Add(key, valueList);
                }
                valueList.Merge(getValueFromSourceFn(item));
            }

            return dic;
        }

        #endregion

        #region From Dictionary

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            IDictionary<TKey, TValue> dic2)
        {
            if (dic == null || dic2 == null)
                return dic;
            foreach (var key in dic2.Keys)
            {
                if (dic.ContainsKey(key))
                    dic.Remove(key);
                dic.Add(key, dic2[key]);
            }
            return dic;
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue, TSource>(this IDictionary<TKey, TValue> dic,
            IDictionary<TKey, TSource> dic2, Func<TSource, TValue> getValueFromSourceFn)
        {
            if (dic == null || dic2 == null)
                return dic;
            foreach (var key in dic2.Keys)
            {
                if (dic.ContainsKey(key))
                    dic.Remove(key);
                dic.Add(key, getValueFromSourceFn(dic2[key]));
            }
            return dic;
        }

        #endregion

        #endregion

        #region Remove

        public static bool IsEmptyAfterRemove<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey key, Action<TKey, TValue> beforeRemoveFn = null)
        {
            if (dic == null)
                return true;

            if (!dic.ContainsKey(key)) 
                return dic.Keys.Count <= 0;

            if (beforeRemoveFn != null)
                beforeRemoveFn(key, dic[key]);
            dic.Remove(key);

            return dic.Keys.Count <= 0;
        }

        #endregion
    }
}
