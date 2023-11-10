using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence.Configuration;

public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("item").HasKey(p => p.ID).HasName("item_pk");
        builder.Property(p => p.ID).HasColumnName("id");
        builder.Property(p => p.Code).HasColumnName("code").HasMaxLength(50);
        builder.Property(p => p.Code).HasColumnName("code").IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
    }
}
