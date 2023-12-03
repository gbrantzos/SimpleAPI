using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class FeatureEntityConfiguration : EntityTypeConfiguration<Feature, FeatureID>
{
    public FeatureEntityConfiguration() : base(typeof(FeatureID.EFValueConverter)) { }

    public override void Configure(EntityTypeBuilder<Feature> builder)
    {
        base.Configure(builder);
        
        builder.Property(p => p.Name)
            .HasColumnOrder(2)
            .HasColumnName("name");
    }
}
