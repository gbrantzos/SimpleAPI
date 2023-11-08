using SimpleAPI.Web.IntegrationTests.Setup;

namespace SimpleAPI.Web.IntegrationTests.Endpoints;

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
