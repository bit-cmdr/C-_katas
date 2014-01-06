using System;
using System.Linq.Expressions;

namespace RepositoryExtensions.Utilities
{
    public static class FuncExtensions
    {
        public static Expression<Func<TSource, TOut>> Compose<TSource, TOut, TDestination>(
            this Expression<Func<TDestination, TOut>> outer,
            Expression<Func<TSource, TDestination>> inner)
        {
            return Expression.Lambda<Func<TSource, TOut>>(
                ParameterReplacer.Replace(outer.Body, outer.Parameters[0], inner.Body),
                inner.Parameters[0]);
        }
    }
}
