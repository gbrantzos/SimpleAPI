using FluentAssertions;
using Moq;
using SimpleAPI.Application.Common;
using SimpleAPI.Application.Features.Items.UseCases.DeleteItem;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Application.Features.Items.DeleteItem;

public class DeleteItemHandlerTests
{
    private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

    [Fact]
    public async Task When_item_exists_entity_is_deleted()
    {
        // Arrange
        var request = new DeleteItemCommand(12, 7);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        var existing = Item.Create("Test.123", "testingItem");
        existing.RowVersion = 7;
        
        mockRepo.Setup(m => m.GetByIDAsync(new ItemID(12), cancellationToken))
            .ReturnsAsync(existing);
        mockRepo.Setup(m => m.Delete(existing));
        mockUnitOfWork.Setup(m => m.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var handler = new DeleteItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeFalse();
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task When_item_does_not_exist_return_error()
    {
        // Arrange
        var request = new DeleteItemCommand(12, 8);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.GetByIDAsync(new ItemID(12), cancellationToken))
            .ReturnsAsync((Item)null!);

        // Act
        var handler = new DeleteItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();

        var error = result.Error;
        error.Kind.Should().Be(ErrorKind.NotFound);

        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task When_request_has_different_RowVersion_return_error()
    {
        // Arrange
        var request = new DeleteItemCommand(12, 8);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        var existingItem = Item.Create("Code", "Description");
        existingItem.RowVersion = 4;
        mockRepo.Setup(m => m.GetByIDAsync(new ItemID(12), cancellationToken))
            .ReturnsAsync(existingItem);

        // Act
        var handler = new DeleteItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();

        var error = result.Error;
        error.Kind.Should().Be(ErrorKind.ModifiedEntry);

        _mockRepository.VerifyAll();
    }
}
