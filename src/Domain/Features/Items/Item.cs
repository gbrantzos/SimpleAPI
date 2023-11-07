using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

public class Item : Entity
{
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
}
