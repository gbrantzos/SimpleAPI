using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common.Behavior;
using SimpleAPI.Core.Base;

namespace SimpleAPI.UnitTests.Application.Common.Behavior;

public class ValidationBehaviorTests
{
    public record SimpleRequest : Request<SimpleResponse> { }

    public class SimpleResponse { }

    [Fact]
    public async Task Should_call_next()
    {
        // Arrange
        var noError = new ValidationResult();
        var validator = new Mock<IValidator<SimpleRequest>>();
        validator.Setup(p => p.ValidateAsync(
                It.IsAny<ValidationContext<SimpleRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(noError);
        var validators = new[] { validator.Object };
        var behavior = new ValidationBehavior<SimpleRequest, SimpleResponse>(validators);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        var next = new Mock<RequestHandlerDelegate<Result<SimpleResponse>>>();
        next.Setup(m => m.Invoke())
            .Returns(Task.FromResult<Result<SimpleResponse>>(new SimpleResponse()))
            .Verifiable();

        // Act
        await behavior.Handle(request, next.Object, cToken);

        // Assert
        next.Verify(m => m.Invoke(), Times.Exactly(1));
    }

    [Fact]
    public async Task Should_return_validation_errors()
    {
        // Arrange
        var error1 = new ValidationResult(new[]
        {
            new ValidationFailure("Property1", "Invalid value for property1")
        });
        var error2 = new ValidationResult(new[]
        {
            new ValidationFailure("Property1", "Property1 cannot be null"),
            new ValidationFailure("Property2", "Invalid value for property2")
        });
        var validator1 = new Mock<IValidator<SimpleRequest>>();
        validator1.Setup(p => p.ValidateAsync(
                It.IsAny<ValidationContext<SimpleRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(error1);
        var validator2 = new Mock<IValidator<SimpleRequest>>();
        validator2.Setup(p => p.ValidateAsync(
                It.IsAny<ValidationContext<SimpleRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(error2);
        var validators = new[] { validator1.Object, validator2.Object };
        var behavior = new ValidationBehavior<SimpleRequest, SimpleResponse>(validators);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        var next = new Mock<RequestHandlerDelegate<Result<SimpleResponse>>>();
        next.Setup(m => m.Invoke())
            .Returns(Task.FromResult<Result<SimpleResponse>>(new SimpleResponse()))
            .Verifiable();

        // Act
        var result = await behavior.Handle(request, next.Object, cToken);

        // Assert
        next.Verify(m => m.Invoke(), Times.Never);
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue();
        result.Error.Kind.Should().Be(ErrorKind.ValidationFailed);

        var expected = new Dictionary<string, object?>()
        {
            { "Property1", new object?[] { "Invalid value for property1", "Property1 cannot be null" } },
            { "Property2", new object?[] { "Invalid value for property2" } },
        };
        result.Error.Details.Should().HaveCount(2);
        result.Error.Details.Should().BeEquivalentTo(expected);
    }
}
