namespace SimpleAPI.Core.Base;

public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Get value object members for equality
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

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

    #region IEquatable interface
    public virtual bool Equals(ValueObject? other) => Equals((object?) other);
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
}
