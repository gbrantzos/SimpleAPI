using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class ItemAlternativeCodesConfiguration : EntityTypeConfiguration<ItemAlternativeCode, ItemAlternativeCodeID>
{
    public ItemAlternativeCodesConfiguration() : base(typeof(ItemAlternativeCodeID.EFValueConverter)) { }

    public override void Configure(EntityTypeBuilder<ItemAlternativeCode> builder)
    {
        // Trick the EF Core so that it can delete orphan details by setting 
        // a composite key with the real PK (ID) and the parent table FK, which
        // was created as a shadow FK key by calling HasForeignKey.
        // IMPORTANT! Set the HasKey, before base method call, so that the PK
        // is redefined!
        
        // More details https://stackoverflow.com/a/24645345/3410871
        builder.HasKey(new[] { "ID", "item_id" });
        builder.Property("item_id").HasColumnOrder(1);
        
        base.Configure(builder);
        
        builder.Property(p => p.Code)
            .HasColumnName("code")
            .HasColumnOrder(2)
            .HasConversion(v => (string)v, v => ItemCode.FromString(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Kind)
            .HasColumnName("kind")
            .HasColumnOrder(3);
        
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnOrder(4);

        builder.HasIndex(p => p.Code).HasDatabaseName("idx_item_alternative_code__code");
    }
}
