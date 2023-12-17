using System.Linq.Expressions;
using SimpleAPI.Core.Guards;

namespace SimpleAPI.Domain.Base;

public class Specification<T>
{
    public static readonly Specification<T> True = new Specification<T>(ex => true);
    public static readonly Specification<T> False = new Specification<T>(ex => false);

    public Expression<Func<T, bool>> Expression { get; }

    public Specification(Expression<Func<T, bool>> expression)
        => Expression = expression;

    public override string ToString() => Expression.ToString();
}

public static class Specification
{
    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        var leftExpression = left.Expression;
        var rightExpression = right.Expression;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
        var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
        var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

        return new Specification<T>(finalExpr);
    }

    public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        var leftExpression = left.Expression;
        var rightExpression = right.Expression;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = Expression.Or(leftExpression.Body, rightExpression.Body);
        var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
        var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

        return new Specification<T>(finalExpr);
    }

    public static Specification<T> Not<T>(this Specification<T> left)
    {
        var leftExpression = left.Expression;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = Expression.Not(leftExpression.Body);
        var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
        var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

        return new Specification<T>(finalExpr);
    }

    public static Specification<T> CombineAnd<T>(Specification<T>[] specifications)
    {
        Ensure.NotEmpty(specifications);
        if (specifications.Length == 1)
            return specifications[0];

        var left = specifications[0];
        var leftExpression = left.Expression;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = Expression.AndAlso(leftExpression.Body, specifications[1].Expression.Body);
        foreach (var right in specifications.Skip(2))
            combined = Expression.AndAlso(combined, right.Expression.Body);
        var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
        var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

        return new Specification<T>(finalExpr);
    }

    public static Specification<T> CombineOr<T>(Specification<T>[] specifications)
    {
        Ensure.NotEmpty(specifications);
        if (specifications.Length == 1)
            return specifications[0];

        var left = specifications[0];
        var leftExpression = left.Expression;

        var parameter = Expression.Parameter(typeof(T), "p");
        var combined = Expression.OrElse(leftExpression.Body, specifications[1].Expression.Body);
        foreach (var right in specifications.Skip(2))
            combined = Expression.OrElse(leftExpression.Body, right.Expression.Body);
        var fixedBody = (BinaryExpression)new ParameterReplacer(parameter).Visit(combined);
        var finalExpr = Expression.Lambda<Func<T, bool>>(fixedBody, parameter);

        return new Specification<T>(finalExpr);
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression node)
            => base.VisitParameter(_parameter);

        internal ParameterReplacer(ParameterExpression parameter) => _parameter = parameter;
    }
}
