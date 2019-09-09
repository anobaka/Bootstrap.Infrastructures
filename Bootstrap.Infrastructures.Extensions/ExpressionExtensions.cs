using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2,
            Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                func(left, right), parameter);
        }

        private class ReplaceExpressionVisitor
            : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

        public static Expression<Func<TResource, TKey>> BuildKeySelector<TResource, TKey>() =>
            BuildKeyExpression<TResource, TKey>(null, (t, key, value) => Expression.TypeAs(key, typeof(TKey)));

        public static Expression<Func<TResource, object>> BuildKeySelector<TResource>() =>
            BuildKeySelector<TResource, object>();

        public static Expression<Func<TResource, bool>> BuildKeyEqualsExpression<TResource>(object key) =>
            BuildKeyExpression<TResource, bool>(key, (type, i, v) => Expression.Equal(i, v));

        public static Expression<Func<TResource, bool>> BuildKeyContainsExpression<TResource>(
            IEnumerable<object> keys)
        {
            var type = SpecificTypeUtils<TResource>.Type;
            var keyProperty = type.GetKeyProperty();
            if (keyProperty == null)
            {
                throw new InvalidOperationException($"Can not find a key property of type: {type.FullName}");
            }

            var trueTypeKeys = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))
                ?.MakeGenericMethod(keyProperty.PropertyType)
                .Invoke(null, new object[] {keys});
            return BuildKeyExpression<TResource, bool>(trueTypeKeys,
                (t, i, v) => Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {t}, v, i));
        }

        /// <summary>
        /// Temporary method.
        /// todo: optimize
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="v"></param>
        /// <param name="buildBody">key type, key, instance value(s), body</param>
        /// <returns></returns>
        private static Expression<Func<TResource, TOut>> BuildKeyExpression<TResource, TOut>(object v,
            Func<Type, Expression, Expression, Expression> buildBody)
        {
            var type = SpecificTypeUtils<TResource>.Type;
            var keyProperty = type.GetKeyProperty();
            if (keyProperty == null)
            {
                throw new InvalidOperationException($"Can not find a key property of type: {type.FullName}");
            }

            var arg = Expression.Parameter(type);
            var key = Expression.Property(arg, keyProperty);
            var value = v == null ? null : Expression.Constant(v);
            var body = buildBody(keyProperty.PropertyType, key, value);
            var lambda = Expression.Lambda<Func<TResource, TOut>>(body, arg);
            return lambda;
        }
    }
}