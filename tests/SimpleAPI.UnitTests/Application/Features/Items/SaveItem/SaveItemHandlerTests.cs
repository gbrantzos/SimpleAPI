using FluentAssertions;
using Moq;
using SimpleAPI.Application.Core;
using SimpleAPI.Application.Features.Items.UseCases.SaveItem;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Application.Features.Items.SaveItem;

public class SaveItemHandlerTests
{
    private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

    [Fact]
    public async Task When_RequestIsValidNewItem_ReturnsItemViewModel()
    {
        // Arrange
        var viewModel = new ItemViewModel
        {
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.AddAsync(It.Is<Item>(i => i.Code == "Code"), cancellationToken))
            .Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(m => m.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var handler = new SaveItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeFalse();
        result.Data.Should().BeEquivalentTo(new ItemViewModel()
        {
            Code        = "Code",
            Description = "Valid Item"
        });
        
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task When_RequestIsValidExistingItem_ReturnsItemViewModel()
    {
        // Arrange
        var viewModel = new ItemViewModel
        {
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(4, viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.GetByIDAsync(It.Is<int>(i => i == 4), cancellationToken))
            .ReturnsAsync(new Item
            {
                ID          = 4,
                Code        = "Code",
                Description = "Valid item"
            });
        mockUnitOfWork.Setup(m => m.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var handler = new SaveItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeFalse();
        result.Data.Should().BeEquivalentTo(new ItemViewModel()
        {
            Code        = "Code",
            Description = "Valid Item"
        }, opt => opt.Excluding(i => i.ID));
        
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task When_RequestIsUnknownItem_ReturnsError()
    {
        // Arrange
        var viewModel = new ItemViewModel
        {
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(4, viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.GetByIDAsync(It.Is<int>(i => i == 4), cancellationToken))
            .ReturnsAsync((Item)null!);

        // Act
        var handler = new SaveItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();

        var error = result.Error;
        error.Kind.Should().Be(ErrorKind.NotFound);
        
        _mockRepository.VerifyAll();
    }
}
