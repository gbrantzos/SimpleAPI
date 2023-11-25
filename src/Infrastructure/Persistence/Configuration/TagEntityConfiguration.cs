using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class TagEntityConfiguration : EntityTypeConfiguration<Tag, TagID>
{
    public TagEntityConfiguration() : base(typeof(TagIDConverter)) { }
}

public class TagIDConverter : ValueConverter<TagID, int>
{
    public TagIDConverter() : base(
        v => v.Value,
        v => new TagID(v)
    ) { }
}
