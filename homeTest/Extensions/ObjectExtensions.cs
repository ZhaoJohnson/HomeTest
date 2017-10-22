using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HpaUtility.Extensions
{
    public static class ObjectExtensions
    {

        public static PropertyInfo GetPropertyInfo<T>(this T obj, Expression<Func<T, object>> lambda)
        {
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var ue = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)ue.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }
            return (PropertyInfo)(memberExpression).Member;
        }

        /// <summary>
        /// Current Object need sync some property from other object. use this method
        /// </summary>
        /// <typeparam name="TCurrent">Current Object</typeparam>
        /// <typeparam name="TOther">Other object need sync prop into current one</typeparam>
        /// <param name="extraObjAssembeFn"></param>
        public static TCurrent SyncFromOtherObj<TCurrent, TOther>(this TCurrent obj, TOther t2,
            Action<TOther, TCurrent> extraObjAssembeFn = null, IEnumerable<PropertyInfo> ignoreProperties = null)
            where TCurrent : class
            where TOther : class
        {
            FromAnotherObj(t2, obj, extraObjAssembeFn, ignoreProperties);
            return obj;
        }


        /// <summary>
        /// Create new object TTarget base on TSource Object, copy the value of the fields from TSource to TTarget
        /// </summary>
        /// <typeparam name="TSource">Current Object</typeparam>
        /// <typeparam name="TTarget">the Object you want to Create</typeparam>
        /// <param name="obj"></param>
        /// <param name="extraObjAssembeFn"></param>
        /// <returns></returns>
        public static TTarget CreateFromCurrentObj<TSource, TTarget>(this TSource obj,
            Action<TSource, TTarget> extraObjAssembeFn = null, IEnumerable<PropertyInfo> ignoreProperties = null)
            where TTarget : class
        {
            return FromAnotherObj(obj, null, extraObjAssembeFn, ignoreProperties);
        }

        private static TTarget FromAnotherObj<TSource, TTarget>(this TSource obj, TTarget t2 = null,
            Action<TSource, TTarget> extraObjAssembeFn = null, IEnumerable<PropertyInfo> ignoreProperties = null)
            where TTarget : class
        {
            //Step 1. If Obj is null, return null
            if (obj == null)
                return default(TTarget);

            if (t2 == null)
                t2 = Reflection.ConvertType<TTarget>(Reflection.AssembeObj<TSource, TTarget>(obj, null, ignoreProperties));
            else
                Reflection.AssembeObj<TSource, TTarget>(obj, t2, ignoreProperties);

            if (extraObjAssembeFn != null)
                extraObjAssembeFn(obj, t2);
            return t2;
        }

        public static object GetDbObject(this object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }
            if (obj.GetType() == typeof(string))
            {
                if (string.IsNullOrWhiteSpace((string)obj))
                {
                    return DBNull.Value;
                }
                return ((string)obj).Trim();
            }
            return obj;
        }
    }
}
