using System;
using System.Linq.Expressions;

namespace HpaUtility.Extensions
{
    public static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> True<T>() { return r => true; }

        public static Expression<Func<T, bool>> False<T>() { return r => false; }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "r");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.And(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool?>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "r");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.And(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "r");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.Or(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool?>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "r");
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.Or(left, right);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }


        public static Expression<Func<TObject, bool>> ConvertToTrueCriterion<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            string featureName = expression.GetPropertyName();

            ParameterExpression paramterExp = Expression.Parameter(typeof(TObject), "r");
            MemberExpression leftExp = Expression.Property(paramterExp, typeof(TObject).GetProperty(featureName));
            ConstantExpression rightExp = Expression.Constant(true, typeof(bool?));
            BinaryExpression e1 = Expression.Equal(leftExp, rightExp);
            return Expression.Lambda<Func<TObject, bool>>(e1, new ParameterExpression[] { paramterExp });
        }

        public static Expression<Func<TObject, bool>> ConvertToNullFalseCriterion<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            string featureName = expression.GetPropertyName();

            ParameterExpression paramterExp = Expression.Parameter(typeof(TObject), "r");

            MemberExpression leftExp = Expression.Property(paramterExp, typeof(TObject).GetProperty(featureName));
            ConstantExpression rightExp = Expression.Constant(null, typeof(bool?));
            BinaryExpression e1 = Expression.Equal(leftExp, rightExp);

            rightExp = Expression.Constant(false, typeof(bool?));
            BinaryExpression e2 = Expression.Equal(leftExp, rightExp);

            Expression predicate = Expression.OrElse(e1, e2);

            return Expression.Lambda<Func<TObject, bool>>(predicate, new ParameterExpression[] { paramterExp });
        }

        public static Expression<Func<TObject, bool>> ConvetToIsNullCriterion<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            string featureName = expression.GetPropertyName();

            ParameterExpression paramterExp = Expression.Parameter(typeof(TObject), "r");

            MemberExpression leftExp = Expression.Property(paramterExp, typeof(TObject).GetProperty(featureName));
            ConstantExpression rightExp = Expression.Constant(null, typeof(TProperty));
            BinaryExpression e1 = Expression.Equal(leftExp, rightExp);

            return Expression.Lambda<Func<TObject, bool>>(e1, new ParameterExpression[] { paramterExp });
        }

        public static Expression<Func<TObject, bool>> ConvertToIsNotNullCriterion<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            string featureName = expression.GetPropertyName();

            ParameterExpression paramterExp = Expression.Parameter(typeof(TObject), "r");

            MemberExpression leftExp = Expression.Property(paramterExp, typeof(TObject).GetProperty(featureName));
            ConstantExpression rightExp = Expression.Constant(null, typeof(TProperty));
            BinaryExpression e1 = Expression.NotEqual(leftExp, rightExp);

            return Expression.Lambda<Func<TObject, bool>>(e1, new ParameterExpression[] { paramterExp });
        }
        public static Expression<Func<TObject, bool>> ConvertToEqualCriterion<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression, TProperty value)
        {
            string featureName = expression.GetPropertyName();

            ParameterExpression paramterExp = Expression.Parameter(typeof(TObject), "r");

            MemberExpression leftExp = Expression.Property(paramterExp, typeof(TObject).GetProperty(featureName));
            ConstantExpression rightExp = Expression.Constant(value, typeof(TProperty));
            BinaryExpression e1 = Expression.Equal(leftExp, rightExp);

            return Expression.Lambda<Func<TObject, bool>>(e1, new ParameterExpression[] { paramterExp });
        }

    }


    /// <summary>
    /// 统一ParameterExpression
    /// </summary>
    internal class ParameterReplacer : ExpressionVisitor
    {
        public ParameterReplacer(ParameterExpression paramExpr)
        {
            this.ParameterExpression = paramExpr;
        }

        public ParameterExpression ParameterExpression { get; private set; }

        public Expression Replace(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.ParameterExpression;
        }
    }
}