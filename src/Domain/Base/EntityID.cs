using SimpleAPI.Core.Base;

namespace SimpleAPI.Domain.Base;

// public abstract class EntityID : ValueObject
// {
//     public int Value { get; init; }
//
//     protected EntityID(int id) { Value = id; }
//     protected EntityID() : this(0) { }
//
//     protected override IEnumerable<object> GetEqualityComponents()
//     {
//         yield return Value;
//     }
//
//     public override string ToString() => $"{GetType().Name} #{Value}";
// }

public interface IEntityID
{
    public int Value { get; }
}
