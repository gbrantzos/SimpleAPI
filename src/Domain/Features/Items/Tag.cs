using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Tag : Entity<TagID>
{
    public Tag(string name) => Name = name;

    public string Name { get; init; }

    public override string ToString() => $"{Name} [ID {ID}]";
}
