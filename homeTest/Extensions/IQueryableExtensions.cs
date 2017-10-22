using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool ascending) where T : class
        {
            var type = typeof(T);

            var property = type.GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException(propertyName, "Not Exist");

            var param = Expression.Parameter(type, "p");
            var propertyAccessExpression = Expression.MakeMemberAccess(param, property);
            var orderByExpression = Expression.Lambda(propertyAccessExpression, param);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";

            var resultExp = Expression.Call(typeof(Queryable), methodName, 
                new [] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// Split page base on condition
        /// </summary>
        /// <param name="where">condition</param>
        /// <param name="orderBy">order</param>
        /// <param name="ascending">assceding?</param>
        /// <param name="pageIndex">current page No</param>
        /// <param name="pageSize">page size</param>
        /// <param name="totalRecord">total records</param>
        /// <returns>rows</returns>
        public static List<T> SplitPage<T>(this IQueryable<T> query, 
            Expression<Func<T, bool>> where, string orderBy, bool ascending, int pageIndex, int pageSize, out int totalRecord)
            where T : class
        {
            totalRecord = 0;
            var list = query.Where(where);

            totalRecord = list.Count();
            if (totalRecord <= 0) return new List<T>();

            list = list.OrderBy(orderBy, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return list.ToList();
        }

        public static List<TObject> GetMultipleRowByLambda<TObject, TProperty>(this IQueryable<TObject> query,
           Expression<Func<TObject, bool>> where, Expression<Func<TObject, TProperty>> orderByExp, bool ascending=true, int pageIndex=1, int pageSize=0)
           where TObject : class
        {
            var list = query;
            if(where!=null)
                list=query.Where(where);

            int totalRecord = list.Count();
            if (totalRecord <= 0) return new List<TObject>();
            if (pageSize == 0) pageSize = totalRecord;

            if (orderByExp == null)
                return list.ToList();

            string orderBy = orderByExp.GetPropertyName();
            list = list.OrderBy(orderBy, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return list.ToList();
        }

        public static T GetSingleRowByLambda<T>(this IQueryable<T> query,Expression<Func<T, bool>> where)
          where T : class
        {
            return query.FirstOrDefault(where);
        }
    }
}
