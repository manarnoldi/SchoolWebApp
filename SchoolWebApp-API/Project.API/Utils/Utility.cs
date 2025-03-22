using System.Linq.Expressions;

namespace SchoolWebApp.API.Utils
{
    public static class Utility
    {
        public static Expression<Func<T, bool>> CombineExpressions<T>(
    Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(first, parameter),
                    Expression.Invoke(second, parameter)
                ),
                parameter
            );

            return combined;
        }
    }
}
