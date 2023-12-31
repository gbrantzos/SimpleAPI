using FluentAssertions;
using SimpleAPI.Application.Features.Items.UseCases.SaveItem;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.UnitTests.Application.Features.Items.SaveItem;

public class SaveItemValidatorTests
{
    [Fact]
    public void When_item_is_valid_return_valid()
    {
        // Arrange
        var item = new ItemViewModel()
        {
            Code        = "Code1",
            Description = "Valid object"
        };
        var validator = new SaveItemValidator();

        // Act
        var request = new SaveItemCommand(0, item);
        var validationResult = validator.Validate(request);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void When_code_is_empty_return_ValidationError()
    {
        // Arrange
        var item = new ItemViewModel()
        {
            Code        = "",
            Description = "Invalid object"
        };
        var validator = new SaveItemValidator();

        // Act
        var request = new SaveItemCommand(0, item);
        var validationResult = validator.Validate(request);

        // Arrange
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Any(r => r.PropertyName == "ViewModel.Code").Should().BeTrue();
    }
}
