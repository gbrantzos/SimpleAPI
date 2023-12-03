namespace SimpleAPI.Domain.Base;

public interface IEntityID
{
    public int Value { get; }
    
    public bool IsNew { get; }
}
