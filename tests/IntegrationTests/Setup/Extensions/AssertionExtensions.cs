using System.Net;
using FluentAssertions;

namespace SimpleAPI.IntegrationTests.Setup;

public static class AssertionExtensions
{
    public static void ShouldReturn(this HttpResponseMessage response,
        HttpStatusCode statusCode,
        string contentType = "application/json")
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(statusCode);

        response.Content
            .Headers
            .ContentType?
            .MediaType
            .Should()
            .Be(contentType);
    }

}
