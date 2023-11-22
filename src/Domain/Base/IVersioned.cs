namespace SimpleAPI.Domain.Base;

public interface IVersioned
{
    public int RowVersion { get; set; }
}
