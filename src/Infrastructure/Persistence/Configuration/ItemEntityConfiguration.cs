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
            .HasColumnName("code")
            .HasConversion(v => (string)v, v => ItemCode.FromString(v))
            .HasMaxLength(50).IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(a => a.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(14, 4);
            priceBuilder.Property(a => a.Currency)
                .HasColumnName("price_currency");
        });

        builder.HasMany(p => p.Tags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey("item_id")
            .HasConstraintName("fk_tag__item_id")
            .HasRelatedTableIndexName("idx_tag__item_id");

        builder.Navigation(p => p.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

