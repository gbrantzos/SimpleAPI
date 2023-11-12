using FluentAssertions;
using Moq;
using SimpleAPI.Application.Common;
using SimpleAPI.Application.Features.Items.UseCases.SaveItem;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core.Base;
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
            RowVersion  = 1,
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(m => m.Add(It.Is<Item>(i => i.Code == "Code")));
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
        }, opt => opt.Excluding(i => i.RowVersion));
        
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task When_RequestIsValidExistingItem_ReturnsItemViewModel()
    {
        // Arrange
        var viewModel = new ItemViewModel
        {
            RowVersion  = 3,
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(4, viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        var existingItem = new Item
        {
            ID          = 4,
            RowVersion  = 3,
            Code        = "Code",
            Description = "Valid item"
        };
        mockRepo.Setup(m => m.GetByIDAsync(It.Is<int>(i => i == 4), cancellationToken))
            .ReturnsAsync(existingItem);
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
            ID          = 4,
            Code        = "Code",
            Description = "Valid Item"
        }, opt => opt.Excluding(i => i.RowVersion));
        
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

    [Fact]
    public async Task When_RequestRowVersionIsDifferent_ReturnsError()
    {
        // Arrange
        var viewModel = new ItemViewModel
        {
            RowVersion  = 3,
            Code        = "Code",
            Description = "Valid Item"
        };
        var request = new SaveItemCommand(14, viewModel);
        var mockRepo = _mockRepository.Create<IItemRepository>();
        var mockUnitOfWork = _mockRepository.Create<IUnitOfWork>();
        var cancellationToken = CancellationToken.None;

        var existing = new Item()
        {
            RowVersion = 5
        };
        mockRepo.Setup(m => m.GetByIDAsync(It.Is<int>(i => i == 14), cancellationToken))
            .ReturnsAsync(existing);

        // Act
        var handler = new SaveItemHandler(mockRepo.Object, mockUnitOfWork.Object);
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();

        var error = result.Error;
        error.Kind.Should().Be(ErrorKind.ModifiedEntry);
        
        _mockRepository.VerifyAll();
    }
}
