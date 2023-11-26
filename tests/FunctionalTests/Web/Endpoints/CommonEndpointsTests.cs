using SimpleAPI.FunctionalTests.Setup;

namespace SimpleAPI.FunctionalTests.Web.Endpoints;

public class CommonEndpointsTests : IClassFixture<SimpleAPIFactory>
{
    private readonly SimpleAPIFactory _factory;

    public CommonEndpointsTests(SimpleAPIFactory factory) 
        => _factory = factory;

    [Theory]
    [InlineData("/")]
    [InlineData("/favicon.ico")]
    [InlineData("/configuration")]
    [InlineData("/metrics")]
    [InlineData("/db-schema")]
    public async Task When_CommonEndpointReached_ReturnsOK(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); 
    }
    
}
