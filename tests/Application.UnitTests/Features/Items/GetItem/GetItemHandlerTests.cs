using Moq;
using SimpleAPI.Application.Features.Items.UseCases.GetItem;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.UnitTests.Features.Items.GetItem;

public class GetItemHandlerTests
{
    private readonly MockRepository _mockRepository = new(MockBehavior.Strict);
    
    [Fact]
    public async Task When_RequestedItemExists_EntityIsReturned()
    {
        // Arrange
        var request = new GetItemCommand(12);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var cancellationToken = CancellationToken.None;

        var existing = new Item()
        {
            ID          = 12,
            Code        = "Test.123",
            Description = "Testing Item"
        };
        mockRepo.Setup(m => m.GetByIDAsync(12, cancellationToken))
            .ReturnsAsync(existing);
        
        // Act
        var handler = new GetItemHandler(mockRepo.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        var expected = existing.ToViewModel();
        result.Should().NotBeNull();
        result.HasErrors.Should().BeFalse();
        result.Data.Should().BeEquivalentTo(expected);
        
        _mockRepository.VerifyAll();
    }
    
    [Fact]
    public async Task When_RequestedItemDoesExists_ReturnsError()
    {
        // Arrange
        var request = new GetItemCommand(12);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.GetByIDAsync(12, cancellationToken))
            .ReturnsAsync((Item)null!);

        // Act
        var handler = new GetItemHandler(mockRepo.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();

        var error = result.Error;
        error.Kind.Should().Be(ErrorKind.NotFound);

        _mockRepository.VerifyAll();
    }

}
