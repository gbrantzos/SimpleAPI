namespace SimpleAPI.Domain.Base;

public abstract class Entity { }

public abstract class Entity<TEntityID> : Entity where TEntityID : IEntityID
{
    public TEntityID? ID { get; init; }

    public bool IsNew => ID is null;
}
