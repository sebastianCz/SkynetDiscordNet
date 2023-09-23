using System.Linq.Expressions;
using System.Reflection;

public static class Lambdas
{
    public static PropertyInfo GetPropertyInfo<TType, TReturn>(
        this Expression<Func<TType, TReturn>> property
    )
    {
        LambdaExpression lambda = property;
        var memberExpression = lambda.Body is UnaryExpression expression
            ? (MemberExpression)expression.Operand
            : (MemberExpression)lambda.Body;

        return (PropertyInfo)memberExpression.Member;
    }

    public static string GetPropertyName<TType, TReturn>(
        this Expression<Func<TType, TReturn>> property
    ) => property.GetPropertyInfo().Name;
}