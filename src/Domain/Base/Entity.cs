namespace SimpleAPI.Domain.Base;

public abstract class Entity
{
    public int RowVersion { get; set; } // TODO Move to an interface
}

public abstract class Entity<TEntityID> : Entity where TEntityID : EntityID, new()
{
    public TEntityID? ID { get; init; }

    public bool IsNew => ID is null;
}
