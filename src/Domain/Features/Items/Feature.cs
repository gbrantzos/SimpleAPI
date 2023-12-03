using SimpleAPI.Core;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Domain.Features.Items;

[StronglyTypedID]
public class Feature : Entity<FeatureID>
{
    public string Name { get; init; }

    public Feature(string name) => Name = name.ThrowIfEmpty();
}
