using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class ItemEntityConfiguration : EntityTypeConfiguration<Item, ItemID>
{
    public ItemEntityConfiguration() : base(typeof(ItemID.EFValueConverter)) { }

    public override void Configure(EntityTypeBuilder<Item> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Code)
            .HasConversion(v => (string)v, v => ItemCode.FromString(v))
            .HasMaxLength(50)
            .HasColumnOrder(1)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .HasColumnOrder(2)
            .HasColumnName("description")
            .IsRequired();

        builder.ComplexProperty(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(a => a.Amount)
                .HasColumnName("price_amount")
                .HasColumnOrder(3)
                .HasPrecision(14, 4);
            priceBuilder.Property(a => a.Currency)
                .HasColumnOrder(4)
                .HasColumnName("price_currency");
        });

        builder.HasMany(p => p.Features)
            .WithMany()
            .UsingEntity<ItemFeature>(
                r => r.HasOne<Feature>()
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_item_feature__feature_id"),
                l => l.HasOne<Item>()
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_item_feature__item_id")
            );
        builder.Navigation(p => p.Features).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.AlternativeCodes)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey("item_id")
            .HasConstraintName("fk_item_alternative_code__item_id")
            .HasRelatedTableIndexName("idx_item_alternative_code__item_id");
        builder.Navigation(p => p.AlternativeCodes).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(p => p.Code).HasDatabaseName("idx_item__code");
    }
}
