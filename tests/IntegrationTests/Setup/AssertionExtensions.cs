using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
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

    public static Task<HttpResponseMessage> PostStringAsJsonAsync(this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string? requestUri,
        string json)
        => client.PostAsync(requestUri, new StringContent(
            json,
            Encoding.UTF8,
            "application/json")
        );

    public static Task<HttpResponseMessage> PutStringAsJsonAsync(this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)]
        string? requestUri,
        string json)
        => client.PutAsync(requestUri, new StringContent(
            json,
            Encoding.UTF8,
            "application/json")
        );
}
