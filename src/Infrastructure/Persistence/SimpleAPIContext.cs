using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Persistence;

public class SimpleAPIContext : DbContext
{
    public DbSet<Item> Items => Set<Item>();

    public SimpleAPIContext(DbContextOptions<SimpleAPIContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SimpleAPIContext).Assembly);
    }

    public string GetDbSchema() => this.Database.GenerateCreateScript();
}
