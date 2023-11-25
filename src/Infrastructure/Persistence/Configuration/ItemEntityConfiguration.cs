using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence.Base;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class ItemEntityConfiguration : EntityTypeConfiguration<Item, ItemID>
{
    public ItemEntityConfiguration() : base(typeof(ItemIDConverter)) { }

    public override void Configure(EntityTypeBuilder<Item> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Code).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();

        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(a => a.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(14, 4);
            priceBuilder.Property(a => a.Currency).HasColumnName("price_currency");
        });

        builder.HasMany(p => p.Tags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey("item_id")
            .HasConstraintName("fk_tag_item_id__item_id")
            .HasRelatedTableIndexName("idx_tag__item_id");
            
        builder.Navigation(p => p.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

// TODO Check if we can skip this with a more generic class (generators)
public class ItemIDConverter : ValueConverter<ItemID, int>
{
    public ItemIDConverter() : base(
        v => v.Value,
        v => new ItemID(v)
    ) { }
}
