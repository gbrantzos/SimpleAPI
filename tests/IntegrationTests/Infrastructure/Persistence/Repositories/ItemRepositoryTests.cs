using FluentAssertions;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Infrastructure.Persistence.Repositories;
using SimpleAPI.IntegrationTests.Setup;

namespace SimpleAPI.IntegrationTests.Infrastructure.Persistence.Repositories;

// Ideas found on
// https://github.com/skimedic/SoftwareTesting
public class ItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _database;

    public ItemRepositoryTests(DatabaseFixture database) => _database = database;

    [Fact]
    public async Task ShouldAddItem()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        var item = new Item
        {
            Code        = "Test.001",
            Description = "Testing item persistence"
        };

        await _database.Context.ExecuteAndRollbackAsync(async () =>
            {
                // Act
                await repository.AddAsync(item);
                await uow.SaveChangesAsync();

                // Assert
                var actual = _database.Context.Items.Single(i => i.Code == item.Code);
                actual.Should().BeEquivalentTo(item);
            }
        );
    }

    [Fact]
    public async Task ShouldUpdateItem()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        var item = new Item
        {
            Code        = "Test.002",
            Description = "Testing item persistence"
        };
        _database.Context.Add(item);
        await _database.Context.SaveChangesAsync();
        _database.Context.ChangeTracker.Clear();

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            // Act
            var existing = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException("Existing item is null");
            existing.Description = "This a changed description";
            await uow.SaveChangesAsync();

            // Assert
            var actual = _database.Context.Items.Single(i => i.Code == item.Code);
            actual.Should().BeEquivalentTo(existing);
        });
    }

    [Fact]
    public async Task ShouldDeleteItem()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        var item = new Item
        {
            Code        = "Test.003",
            Description = "Testing item persistence"
        };
        _database.Context.Add(item);
        await _database.Context.SaveChangesAsync();
        _database.Context.ChangeTracker.Clear();

        // Act
        repository.Delete(item);
        await uow.SaveChangesAsync();

        // Assert
        var actual = _database.Context.Items.SingleOrDefault(i => i.Code == item.Code);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task ShouldGetItemByID()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var items = new List<Item>
            {
                new Item
                {
                    Code        = "Code.001",
                    Description = "Item 001"
                },
                new Item
                {
                    Code        = "Code.002",
                    Description = "Item 003"
                },
                new Item
                {
                    Code        = "Code.004",
                    Description = "Item 004"
                },
                new Item
                {
                    Code        = "Code.005",
                    Description = "Item 005"
                }
            };
            _database.Context.AddRange(items);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();
            
            var expected = items.Last();
            
            // Act 
            var actual = await repository.GetByIDAsync(expected.ID);
            
            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        });
    }

    // TODO Add the following
    // public void ShouldThrowConcurrencyException()
}
