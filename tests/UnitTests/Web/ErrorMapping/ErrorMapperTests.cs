using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleAPI.Core.Base;
using SimpleAPI.Web.ErrorMapping;
using SimpleAPI.Web.HostSetup.Context;

namespace SimpleAPI.UnitTests.Web.ErrorMapping;

public class ErrorMapperTests
{
    private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

    [Fact]
    public void Ensure_AllEnums_Tested()
    {
        // Arrange
        var enumCount = Enum.GetValues(typeof(ErrorKind)).Length;
        var casesCount = new ErrorData().Count();
        
        // Assert
        casesCount.Should().Be(enumCount, $"ErrorKind enum has {enumCount} values");
    }
    
    [Theory]
    [ClassData(typeof(ErrorData))]
    public void When_ErrorReceived_ProblemDetailsReturned(Error error, ProblemDetails expected)
    {
        // Arrange
        var contextProvider = new RequestContextProvider();
        var httpContextAccessorMock = _mockRepository.Create<IHttpContextAccessor>();
        var mapper = new ErrorMapper(contextProvider, httpContextAccessorMock.Object);

        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Path   = "/some-path",
                Method = "GET"
            }
        };
        httpContextAccessorMock.Setup(m => m.HttpContext).Returns(httpContext);
        contextProvider.CurrentContext = RequestContext.Create();

        // Act
        var problemDetails = mapper.MapToProblemDetails(error);

        // Assert
        expected.Instance = "GET /some-path";
        expected.Extensions.Add(ErrorMapper.ExecutionID, contextProvider.CurrentContext.ExecutionID);
        problemDetails.Should().NotBeNull();
        problemDetails.Should().BeEquivalentTo(expected);
    }

    private class ErrorData : TheoryData<Error, ProblemDetails>
    {
        public ErrorData()
        {
            Add(Error.Create(ErrorKind.Generic, "Generic error"), new ProblemDetails
            {
                Type   = "https://httpstatuses.io/500",
                Title  = "Generic error",
                Status = 500,
                Detail = "Generic error"
            });

            Add(Error.Create(ErrorKind.NotFound, "Not found"), new ProblemDetails()
            {
                Type   = "https://httpstatuses.io/404",
                Title  = "Not found",
                Status = 404,
                Detail = "Not found"
            });

            Add(Error.Create(ErrorKind.ValidationFailed, "Validation failed"), new ProblemDetails()
            {
                Type   = "https://httpstatuses.io/400",
                Title  = "Bad request",
                Status = 400,
                Detail = "Validation failed"
            });
        }
    }
}
