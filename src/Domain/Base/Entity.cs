namespace SimpleAPI.Domain.Base;

public abstract class Entity { }

public abstract class Entity<TEntityID> : Entity where TEntityID : struct, IEntityID
{
    public TEntityID ID { get; init; }

    public bool IsNew => ID.IsNew;
}
