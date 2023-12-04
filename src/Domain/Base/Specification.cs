using System.Linq.Expressions;

namespace SimpleAPI.Domain.Base;

public class Specification<T>
{
    public static readonly Specification<T> True = new Specification<T>(ex => true);
    public static readonly Specification<T> False = new Specification<T>(ex => false);

    public Expression<Func<T, bool>> Expression { get; }

    public Specification(Expression<Func<T, bool>> expression)
        => Expression = expression;
}
