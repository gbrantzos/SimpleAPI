using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Features.Common;
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
    public async Task Should_add_item()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        var item = new Item
        {
            Code        = "Test.001",
            Description = "Testing item persistence",
            Price       = 3.2
        };
        item.AddTag(new Tag() { Name = "Testing 1" });
        item.AddTag(new Tag() { Name = "Testing 2" });
        item.AddTag(new Tag() { Name = "Testing 3" });

        await _database.Context.ExecuteAndRollbackAsync(async () =>
            {
                // Act
                repository.Add(item);
                await uow.SaveChangesAsync();

                // Assert
                _database.Context.ChangeTracker.Clear();
                var actual = _database.Context.Items.Include(i => i.Tags).Single(i => i.Code == item.Code);
                actual.Should().BeEquivalentTo(item);
            }
        );
    }

    [Fact]
    public async Task Should_update_item()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.002",
                Description = "Testing item persistence",
                Price       = Money.Create(4.6m, Currency.USD)
            };
            _database.Context.Add(item);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            // Act
            var existing = await repository.GetByIDAsync(item.ID!) ??
                throw new InvalidOperationException("Existing item is null");
            existing.Description = "This a changed description";
            existing.Price       = Money.InEuro(12432.90m);
            await uow.SaveChangesAsync();

            // Assert
            var actual = _database.Context.Items.Single(i => i.Code == item.Code);
            actual.Should().BeEquivalentTo(existing);
        });
    }

    [Fact]
    public async Task Should_delete_item()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.003",
                Description = "Testing item persistence"
            };
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            // Act
            repository.Delete(item);
            await uow.SaveChangesAsync();

            // Assert
            var actual = _database.Context.Items.SingleOrDefault(i => i.Code == item.Code);
            actual.Should().BeNull();
        });
    }

    [Fact]
    public async Task Should_get_item_by_ID()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);

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
                    Description = "Item 005",
                    Price       = Money.InEuro(23)
                }
            };
            _database.Context.AddRange(items);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var expected = items.Last();

            // Act 
            var actual = await repository.GetByIDAsync(expected.ID!);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        });
    }

    [Fact]
    public async Task Should_increase_RowVersion()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.032",
                Description = "Testing item persistence"
            };
            item.RowVersion.Should().Be(0);

            // Act
            _database.Context.Add(item);
            await _database.Context.SaveChangesAsync();

            // Assert
            item.RowVersion.Should().Be(1);
            _database.Context.ChangeTracker.Clear();
            var existing = await repository.GetByIDAsync(item.ID!) ??
                throw new InvalidOperationException("Existing item is null");
            existing.RowVersion.Should().Be(item.RowVersion);
        });
    }

    [Fact]
    public async Task Should_update_Audit_properties()
    {
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.032",
                Description = "Testing item persistence"
            };

            var dt1 = DateTime.Now;
            _database.TimeProviderMock.Setup(m => m.GetNow()).Returns(dt1);

            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID!) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            _database.Context.Entry(existingItem).Property(SimpleAPIContext.CreatedAt).CurrentValue.Should().Be(dt1);
            _database.Context.Entry(existingItem).Property(SimpleAPIContext.ModifiedAt).CurrentValue.Should().Be(dt1);

            var dt2 = DateTime.Now;
            _database.TimeProviderMock.Setup(m => m.GetNow()).Returns(dt2);

            existingItem.Description = "ChangedDescription";
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var modifiedItem = await repository.GetByIDAsync(item.ID!) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            _database.Context.Entry(modifiedItem).Property(SimpleAPIContext.CreatedAt).CurrentValue.Should().Be(dt1);
            _database.Context.Entry(modifiedItem).Property(SimpleAPIContext.ModifiedAt).CurrentValue.Should().Be(dt2);
        });
    }

    [Fact]
    public async Task Should_throw_ConcurrencyException()
    {
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.032",
                Description = "Testing item persistence"
            };
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID!) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.Description = "Modified description";

            using (_database.Context.Database.BeginTransactionAsync())
            {
                var sql =
                    $"update `item` set `description`='New', `row_version`=`row_version` +1 where `id` = {existingItem.ID!.Value}";
                await _database.Context.Database.ExecuteSqlRawAsync(sql);
            }

            var updateAction = async () => await uow.SaveChangesAsync();
            await updateAction.Should().ThrowAsync<DbUpdateConcurrencyException>();
        });
    }

    [Fact]
    public async Task Should_add_details()
    {
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.732",
                Description = "Testing item with tags"
            };
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.AddTag(new Tag() { Name = "Tag 1"});
            existingItem.AddTag(new Tag() { Name = "Tag 2"});
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var actual = await repository.GetByIDAsync(item.ID);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(existingItem);
        });
    }
    
    [Fact]
    public async Task Should_delete_details()
    {
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = new Item
            {
                Code        = "Test.733",
                Description = "Testing item with tags"
            };
            item.AddTag(new Tag() { Name = "ATag 1"});
            item.AddTag(new Tag() { Name = "ATag 2"});
            item.AddTag(new Tag() { Name = "ATag 3"});
            item.AddTag(new Tag() { Name = "ATag 4"});
            
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.RemoveTag(new Tag() { Name = "ATag 2"});
            existingItem.RemoveTag(new Tag() { Name = "ATag 4"});
            existingItem.RemoveTag(new Tag() { Name = "ATag 6"}); // This one should not be found, but causes no issues!

            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var actual = await repository.GetByIDAsync(item.ID);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(existingItem);
        });
    }

}
