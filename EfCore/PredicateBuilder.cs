using System;
using System.Linq.Expressions;

namespace EfCore
{
    public static class PredicateBuilder
    {
        // Based on https://stackoverflow.com/a/22569086/3410871
        // More theory on why we must replace parameter of second expression:
        // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/

        // Handy methods, in case we don't have an initial predicate!
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }


        // Simulates AsExpandable() of PredicateBuilder
        private static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
            => new ReplaceVisitor(searchEx, replaceEx).Visit(expression);

        internal class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public ReplaceVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression Visit(Expression node)
                => node == from ? to : base.Visit(node);
        }
    }
}
