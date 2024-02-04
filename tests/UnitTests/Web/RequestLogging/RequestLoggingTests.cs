using Microsoft.AspNetCore.Http;
using Moq;
using SimpleAPI.Web.RequestLogging;

namespace SimpleAPI.UnitTests.Web.RequestLogging;

public class RequestLoggingTests
{
    [Fact]
    public async Task Should_call_handle_callback()
    {
        // Arrange
        var options = new RequestLoggingOptions();
        var handlerMock = new Mock<IRequestHandler>();
        handlerMock.Setup(m => m.Handle()).Verifiable();
        RequestDelegate next = context => Task.CompletedTask;
        
        var sut = new RequestLoggingMiddleware(next, options, handlerMock.Object);
        
        // Act 
        var context = new DefaultHttpContext();
        await sut.InvokeAsync(context);
        
        // Assert
        handlerMock.Verify();
    }
}
