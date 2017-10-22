using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class LambdaExtensions
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static string GetPropertyName<T1, T2>(this Expression<Func<T1, T2>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Set Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="memberLamda"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda, object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.GetValueSetter<T>()(target, value);
                    property.SetValue(target, value, null);
                }
            }
        }

        public static void SetPropertyValue<TObject, TProperty>(this TObject target, Expression<Func<TObject, TProperty>> memberLamda, object value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.GetValueSetter<TObject>()(target, value);
                    property.SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// get Value　
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="memberLamda"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T target, Expression<Func<T, object>> memberLamda)
        {
            object value = null;
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    value = property.GetValueGetter<T>()(target);
                }
            }
            return value;
        }

        public static TProperty GetPropertyValue<TObject, TProperty>(this TObject target, Expression<Func<TObject, TProperty>> memberLamda)
        {
            TProperty value = default(TProperty);
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    value = (TProperty)property.GetValueGetter<TObject>()(target);
                }
            }
            return value;
        }

        #region Expression Get/Set
        /// <summary>
        /// GetSetter( (string example) => example.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Action<T, TProperty> GetLambdaSetter<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            var setMethod = property.GetSetMethod();

            var parameterT = Expression.Parameter(typeof(T), "x");
            var parameterTProperty = Expression.Parameter(typeof(TProperty), "y");
            
            var newExpression =
                Expression.Lambda<Action<T, TProperty>>(
                    Expression.Call(parameterT, setMethod, parameterTProperty),
                    parameterT,
                    parameterTProperty
                );

            return newExpression.Compile();
        }

        /// <summary>
        /// GetGetter( (string example) => example.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Func<T, TProperty> GetLambdaGetter<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;
            var getMethod = property.GetGetMethod();

            var parameterT = Expression.Parameter(typeof(T), "x");

            var newExpression =
                Expression.Lambda<Func<T, TProperty>>(
                    Expression.Call(parameterT, getMethod),
                    parameterT
                );

            return newExpression.Compile();
        }

        #endregion


        #region Property 
        public static Func<T, object> GetValueGetter<T>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var property = Expression.Property(instance, propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
        }

        public static Action<T, object> GetValueSetter<T>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                instance,
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));
            return (Action<T, object>)Expression.Lambda(setterCall, instance, argument)
                                                .Compile();
        }

        #endregion
    }
}
