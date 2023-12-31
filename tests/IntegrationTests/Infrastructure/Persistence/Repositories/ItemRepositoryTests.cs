using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Domain.Base;
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

        var item = Item.Create("Test.001", "Testing item persistence");
        item.SetPrice(3.2);
        item.AddFeature(new Feature("Testing 1"));
        item.AddFeature(new Feature("Testing 2"));
        item.AddFeature(new Feature("Testing 3"));

        item.AddAlternativeCode("ALTER.0098");

        await _database.Context.ExecuteAndRollbackAsync(async () =>
            {
                // Act
                repository.Add(item);
                await uow.SaveChangesAsync();

                // Assert
                _database.Context.ChangeTracker.Clear();
                // var actual = _database.Context.Items
                //     .Include(i => i.Tags)
                //     .Include(i => i.AlternativeCodes)
                //     .Single(i => i.Code == item.Code);
                var actual = await repository.GetByIDAsync(item.ID);
                actual.Should().BeEquivalentTo(item, options => options
                    .For(i => i.AlternativeCodes).Exclude(c => c.ID));
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
            var item = Item.Create("Test.002", "Testing item persistence");
            _database.Context.Add(item);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            // Act
            var existing = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException("Existing item is null");
            var newCode = "Test.639";
            existing.ChangeCode(newCode);
            existing.SetPrice(Money.InEuro(12432.90m));
            existing.Description = "This a changed description";

            await uow.SaveChangesAsync();

            // Assert
            var actual = _database.Context.Items.Single(i => i.Code == newCode);
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
            var item = Item.Create("Test.003", "Testing item persistence");
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
                Item.Create("Code.001", "Item 001"),
                Item.Create("Code.002", "Item 002"),
                Item.Create("Code.003", "Item 003"),
                Item.Create("Code.004", "Item 004"),
                Item.Create("Code.005", "Item 005")
            };
            items[4].SetPrice(Money.InEuro(23));
            _database.Context.AddRange(items);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var expected = items.Last();

            // Act 
            var actual = await repository.GetByIDAsync(expected.ID);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected, options => options
                .For(i => i.AlternativeCodes).Exclude(c => c.ID));
        });
    }

    [Fact]
    public async Task Should_increase_RowVersion()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = Item.Create("Test.032", "Testing item persistence");
            item.RowVersion.Should().Be(0);

            // Act
            _database.Context.Add(item);
            await _database.Context.SaveChangesAsync();

            // Assert
            item.RowVersion.Should().Be(1);
            _database.Context.ChangeTracker.Clear();
            var existing = await repository.GetByIDAsync(item.ID) ??
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
            var item = Item.Create("Test.032", "Testing item persistence");
            var dt1 = DateTime.Now;
            _database.FakeTimeProvider.SetUtcNow(dt1);

            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            _database.Context.Entry(existingItem).Property(SimpleAPIContext.CreatedAt).CurrentValue.Should().Be(dt1);

            var dt2 = DateTime.Now;
            _database.FakeTimeProvider.SetUtcNow(dt2);

            existingItem.Description = "ChangedDescription";
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var modifiedItem = await repository.GetByIDAsync(item.ID) ??
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
            var item = Item.Create("Test.035", "Testing item persistence");
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.Description = "Modified description";

            using (_database.Context.Database.BeginTransactionAsync())
            {
                var sql =
                    $"update `item` set `description`='New', `row_version`=`row_version` +1 where `id` = {existingItem.ID.Value}";
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
            var item = Item.Create("Test.732", "Testing item with features");
            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.AddFeature(new Feature("Feature 1"));
            existingItem.AddFeature(new Feature("Feature 2"));

            existingItem.AddAlternativeCode("ALT.0098");
            existingItem.AddAlternativeCode("ALT.2098");
            existingItem.AddAlternativeCode("ALT.2098");
            existingItem.AddAlternativeCode("MAK.7098");

            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var actual = await repository.GetByIDAsync(item.ID);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(existingItem, options => options
                .For(i => i.AlternativeCodes).Exclude(c => c.ID));
        });
    }

    [Fact]
    public async Task Should_delete_details()
    {
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = Item.Create("Test.733", "Testing item with features");
            item.AddFeature(new Feature("AFeature 1"));
            item.AddFeature(new Feature("AFeature 2"));
            item.AddFeature(new Feature("AFeature 3"));
            item.AddFeature(new Feature("AFeature 4"));

            item.AddAlternativeCode("ALT.0098");
            item.AddAlternativeCode("ALT.2098");
            item.AddAlternativeCode("ALT.2098");
            item.AddAlternativeCode("MAK.7098");

            repository.Add(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var existingItem = await repository.GetByIDAsync(item.ID) ??
                throw new InvalidOperationException($"Could not find item with ID {item.ID}");
            existingItem.RemoveFeature(new Feature("AFeature 2"));
            existingItem.RemoveFeature(new Feature("AFeature 4"));
            existingItem.RemoveFeature(new Feature("AFeature 6")); // This one should not be found, but causes no issues!


            existingItem.RemoveAlternativeCode("ALT.0098");
            existingItem.RemoveAlternativeCode("ALT.2098");

            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var actual = await repository.GetByIDAsync(item.ID);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(existingItem, options => options
                .For(i => i.AlternativeCodes).Exclude(c => c.ID));
        });
    }

    [Fact]
    public async Task Should_soft_delete_item()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);
        var uow = new UnitOfWork(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var item = Item.Create("Test.003", "Testing item persistence");
            repository.Add(item);
            await uow.SaveChangesAsync();
            var modifiedAt = _database.Context.Entry(item).Property(SimpleAPIContext.ModifiedAt).CurrentValue;
            _database.Context.ChangeTracker.Clear();

            var dt1 = DateTime.Now;
            _database.FakeTimeProvider.SetUtcNow(dt1);

            // Act
            repository.Delete(item);
            await uow.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            // Assert
            var actual = _database
                .Context
                .Items
                .IgnoreQueryFilters()
                .SingleOrDefault(i => i.Code == item.Code);
            actual.Should().NotBeNull();
            _database.Context.Entry(actual!).Property(SimpleAPIContext.DeletedAt).CurrentValue.Should().Be(dt1);
            // Modified at should not be changed
            _database.Context.Entry(actual!).Property(SimpleAPIContext.ModifiedAt).CurrentValue.Should().Be(modifiedAt);
        });
    }

    [Fact]
    public async Task Should_find_using_search_criteria()
    {
        // Arrange
        var repository = new ItemRepository(_database.Context);

        await _database.Context.ExecuteAndRollbackAsync(async () =>
        {
            var items = new List<Item>
            {
                Item.Create("Code.001", "Item 001"),
                Item.Create("Code.002", "Item 002"),
                Item.Create("Code.003", "Item 003"),
                Item.Create("Code.004", "Item 004"),
                Item.Create("Code.005", "Item 005")
            };
            items[4].SetPrice(Money.InEuro(23));
            items[4].AddAlternativeCode("ALTER.987");

            _database.Context.AddRange(items);
            await _database.Context.SaveChangesAsync();
            _database.Context.ChangeTracker.Clear();

            var searchCriteria = new SearchCriteria<Item>(
                specification: new Specification<Item>(i => i.ID >= new ItemID(3) && i.Price.Amount >= 3.14m),
                //specification: new Specification<Item>(i => i.AlternativeCodes.Any(a => a.Code == "12")),
                include: new string[] { "AlternativeCodes" },
                sorting: new[] { new Sorting<Item>(i => i.Code, Sorting.SortDirection.Descending) }
            );

            var pagedSearch = new SearchCriteria<Item>(paging: new OffsetBasedPaging() { Offset = 2, Limit = 3 });

            var nested = new SearchCriteria<Item>(
                specification: new Specification<Item>(p => p.AlternativeCodes.Any(n => n.Code == (ItemCode)"CODE.001"))
            );
            
            // Act 
            var actual = await repository.FindAsync(searchCriteria);
            var pagedResult = await repository.FindAsync(pagedSearch);
            var nestedResults = await repository.FindAsync(nested);
            
            // Assert
            actual.Should().NotBeNull();
            pagedResult.Should().NotBeNull();
            pagedResult.Count.Should().Be(3);
        });
    }
}
