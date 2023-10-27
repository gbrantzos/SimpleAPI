using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Items;

namespace SimpleAPI.Infrastructure.Persistence;

public class SimpleAPIContext : DbContext
{
    public DbSet<Item> Items => Set<Item>();

    public SimpleAPIContext(DbContextOptions<SimpleAPIContext> options) : base(options) { }
}
