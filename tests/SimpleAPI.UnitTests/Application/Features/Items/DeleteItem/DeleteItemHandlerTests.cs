using FluentAssertions;
using Moq;
using SimpleAPI.Application.Core;
using SimpleAPI.Application.Features.Items.UseCases.DeleteItem;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Application.Features.Items.DeleteItem;

public class DeleteItemHandlerTests
{
    private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

    [Fact]
    public async Task When_RequestedItemExists_EntityIsDeleted()
    {
        // Arrange
        var request = new DeleteItemCommand(12);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        var existing = new Item()
        {
            ID          = 12,
            Code        = "Test.123",
            Description = "Testing Item"
        };
        mockRepo.Setup(m => m.GetByIDAsync(12, cancellationToken))
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
    public async Task When_RequestedItemDoesExists_ReturnsError()
    {
        // Arrange
        var request = new DeleteItemCommand(12);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.GetByIDAsync(12, cancellationToken))
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
}
