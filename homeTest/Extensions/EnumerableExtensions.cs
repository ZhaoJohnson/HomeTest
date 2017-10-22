using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace HpaUtility.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether the colletion is null or contains no elements
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            /* If this is a list, use the Count property for efficiency.
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count < 1;
            return !enumerable.Any();
        }

        public static IEnumerable<T2> FromAnotherList<T, T2>(
            this IEnumerable<T> list, Func<T, T2, bool> extraObjAssembeFn = null)
        {
            //Step 1. If List is empty. return null
            if (list.IsNullOrEmpty())
                return new List<T2>();

            //Step 2. Create result list object
            var lstObj = new List<T2>();

            //Step 3. Assemble list Obj
            foreach (var t in list)
            {
                var t2 = Reflection.ConvertType<T2>(Reflection.AssembeObj<T, T2>(t));
                if (extraObjAssembeFn != null)
                    extraObjAssembeFn(t, t2);
                //Add object instance to list
                lstObj.Add(t2);
            }
            return lstObj;
        }

        /// <summary>
        /// Return multiple records
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static IEnumerable<T> FromDataReader<T>(this IEnumerable<T> list, DbDataReader dr)
        {
            //Declare one "instance" object of Object type and an object list
            var lstObj = new List<T>();
            //dataReader loop
            while (dr.Read())
            {
                //Add object instance to list
                lstObj.Add(Reflection.AssembeObj<T>(dr));
            }
            return lstObj;
        }

        /// <summary>
        /// return single records
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T FromDataReaderFirstRow<T>(this object input, DbDataReader dr)
        {
            if (dr.Read() && dr.HasRows)
                return Reflection.AssembeObj<T>(dr, input);
            return default(T);
        }

        public static T FromDataReaderFirstRow<T>(DbDataReader dr)
        {
            if (dr.Read() && dr.HasRows)
                return Reflection.AssembeObj<T>(dr, typeof(T));
            return default(T);
        }

        /// <summary>
        /// Perform distinct on object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        } 
    }
}
