namespace SimpleAPI.Core.Base;

// Vale object code based on:
// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects
// https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/ValueObject/ValueObject.cs

public abstract class ValueObject : IComparable<ValueObject>
{
    /// <summary>
    /// Get value object members for equality
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<IComparable> GetEqualityComponents();

    #region Overrides from Object class
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var valueObject = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }
    #endregion

    #region Equality operators
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
    #endregion

    #region Compareable
    public virtual int CompareTo(ValueObject? other)
    {
        if (other is null)
            return 1;

        if (ReferenceEquals(this, other))
            return 0;

        Type thisType = this.GetType();
        Type otherType = other.GetType();
        if (thisType != otherType)
            throw new ArgumentException($"Object is not a {thisType.Name}");

        return GetEqualityComponents()
            .Zip(
                other.GetEqualityComponents(),
                (left, right) => left.CompareTo(right)
            )
            .FirstOrDefault(cmp => cmp != 0);
    }
    
    public static bool operator <(ValueObject left, ValueObject right) => left.CompareTo(right) < 0;
    public static bool operator <=(ValueObject left, ValueObject right) => left.CompareTo(right) <= 0;
    public static bool operator >(ValueObject left, ValueObject right) => left.CompareTo(right) > 0;
    public static bool operator >=(ValueObject left, ValueObject right) => left.CompareTo(right) >= 0;

    #endregion
}
