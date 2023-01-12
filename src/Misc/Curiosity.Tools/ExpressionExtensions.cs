using System;
using System.Linq.Expressions;

namespace Curiosity.Tools
{
    /// <summary>
    /// Расширения для комбинирования двух <see cref="Expression{TDelegate}"/> различными
    /// логическими способами, безопасным для EF образом.
    /// </summary>
    /// <remarks>
    /// Подсмотренно тут:
    /// https://stackoverflow.com/questions/457316/combining-two-expressions-expressionfunct-bool
    /// </remarks>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Комбинирует два <see cref="Expression{TDelegate}"/> с помощью логического И.
        /// </summary>
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) throw new ArgumentNullException(nameof(expr1));
            if (expr2 == null) throw new ArgumentNullException(nameof(expr2));
            
            var expressions = VisitExpressions(expr1, expr2);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expressions.Item1, expressions.Item2), expressions.Item3);
        }
        
        /// <summary>
        /// Комбинирует два <see cref="Expression{TDelegate}"/> с помощью логического ИЛИ.
        /// </summary>
        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) throw new ArgumentNullException(nameof(expr1));
            if (expr2 == null) throw new ArgumentNullException(nameof(expr2));
            
            var expressions = VisitExpressions(expr1, expr2);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expressions.Item1, expressions.Item2), expressions.Item3);
        }
        
        /// <summary>
        /// Логическое отрицание <see cref="Expression{TDelegate}"/>.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(
            this Expression<Func<T, bool>> expr)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));
            
            return Expression.Lambda<Func<T, bool>>(
                Expression.Not(expr.Body), expr.Parameters[0]);
        }

        private static Tuple<Expression, Expression, ParameterExpression> VisitExpressions<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof (T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            
            return new Tuple<Expression, Expression, ParameterExpression>(left!, right!, parameter);
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

            public override Expression? Visit(Expression? node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
