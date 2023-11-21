using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common.Behavior;
using SimpleAPI.Core.Base;

namespace SimpleAPI.UnitTests.Application.Common.Behavior;

// Details found on https://stackoverflow.com/a/76268601/3410871
public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<SimpleRequest, SimpleResponse>>> _loggerMock;
    private readonly Mock<ILoggerFactory> _loggerFactory;

    public record SimpleRequest : Request<SimpleResponse> { }

    public class SimpleResponse { }

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<SimpleRequest, SimpleResponse>>>();
        _loggerMock.Setup(x => x.Log(
                LogLevel.Information,
                0,
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception?, string>>())
            )
            .Verifiable();

        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>()))
            .Returns(_loggerMock.Object);
    }

    [Fact]
    public async Task Should_log_success()
    {
        // Arrange
        var behavior = new LoggingBehavior<SimpleRequest, SimpleResponse>(_loggerFactory.Object);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        RequestHandlerDelegate<Result<SimpleResponse, Error>> next = () =>
            Task.FromResult<Result<SimpleResponse, Error>>(new SimpleResponse());

        // Act
        await behavior.Handle(request, next, cToken);

        // Arrange
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) =>
                    @o.ToString()!.Contains("executed successfully") && @t.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(1));
    }

    [Fact]
    public async Task Should_log_failure()
    {
        // Arrange
        var behavior = new LoggingBehavior<SimpleRequest, SimpleResponse>(_loggerFactory.Object);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        RequestHandlerDelegate<Result<SimpleResponse, Error>> next = () =>
            Task.FromResult<Result<SimpleResponse, Error>>(Error.Create(ErrorKind.Generic, "Generic error"));

        // Act
        await behavior.Handle(request, next, cToken);

        // Arrange
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                0,
                It.Is<It.IsAnyType>((@o, @t) =>
                    @o.ToString()!.Contains("execution failed") && @t.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(1));
    }

    [Fact]
    public async Task Should_log_cancellation()
    {
        // Arrange
        var behavior = new LoggingBehavior<SimpleRequest, SimpleResponse>(_loggerFactory.Object);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        RequestHandlerDelegate<Result<SimpleResponse, Error>> next = () =>
            throw new OperationCanceledException("Cancel");

        // Act
        var call = async () => await behavior.Handle(request, next, cToken);

        // Arrange
        await call.Should().ThrowAsync<OperationCanceledException>();
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                0,
                It.Is<It.IsAnyType>((@o, @t) =>
                    @o.ToString()!.Contains("execution was cancelled") && @t.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(1));
    }
    
    [Fact]
    public async Task Should_log_unhandled_exception()
    {
        // Arrange
        var behavior = new LoggingBehavior<SimpleRequest, SimpleResponse>(_loggerFactory.Object);
        var request = new SimpleRequest();
        var cToken = CancellationToken.None;
        RequestHandlerDelegate<Result<SimpleResponse, Error>> next = () =>
            throw new Exception("Unhandled exception");

        // Act
        var call = async () => await behavior.Handle(request, next, cToken);

        // Arrange
        await call.Should().ThrowAsync<Exception>().WithMessage("Unhandled exception");
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                0,
                It.Is<It.IsAnyType>((@o, @t) =>
                    @o.ToString()!.Contains("execution failed, unhandled exception") && @t.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(1));
    }

}
