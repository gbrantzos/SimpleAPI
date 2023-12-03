using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class ItemFeatureEntityConfiguration : EntityTypeConfiguration<ItemFeature>
{
    public override void Configure(EntityTypeBuilder<ItemFeature> builder)
    {
        base.Configure(builder);
        builder.HasKey(p => new { p.ItemID, p.FeatureID })
            .HasName("pk_item_feature");
        
        builder.Property(p => p.FeatureID)
            .HasColumnOrder(1)
            .HasConversion<FeatureID.EFValueConverter>()
            .HasColumnName("feature_id");
        builder.Property(p => p.ItemID)
            .HasColumnOrder(2)
            .HasConversion<ItemID.EFValueConverter>()
            .HasColumnName("item_id");

        builder.HasIndex(p => p.FeatureID)
            .HasDatabaseName("idx_item_feature_feature_id");
    }
}
