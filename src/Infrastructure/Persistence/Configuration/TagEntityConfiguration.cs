using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class TagEntityConfiguration : EntityTypeConfiguration<Tag, TagID>
{
    public TagEntityConfiguration() : base(typeof(TagIDConverter)) { }

    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Trick the EF Core so that it can delete orphan details by setting 
        // a composite key with the real PK (ID) and the parent table FK, which
        // was created as a shadow FK key by calling HasForeignKey.
        // IMPORTANT! Set the HasKey, before base method call, so that the PK
        // is redefined!
        
        // More details https://stackoverflow.com/a/24645345/3410871
        
        builder.HasKey(new[] { "ID", "item_id" });
        base.Configure(builder);
    }
}
