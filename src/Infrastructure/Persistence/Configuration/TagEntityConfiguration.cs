using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class TagEntityConfiguration : EntityTypeConfiguration<Tag, TagID>
{
    public TagEntityConfiguration() : base(typeof(TagID.EFValueConverter)) { }

    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(new[] { "ID", "item_id" });
        builder.Property("item_id").HasColumnOrder(1);
        
        base.Configure(builder);
    }
}
