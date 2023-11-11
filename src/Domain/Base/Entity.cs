namespace SimpleAPI.Domain.Base;

public abstract class Entity
{
    public int ID { get; init; }
    public int RowVersion { get; set; }
}
