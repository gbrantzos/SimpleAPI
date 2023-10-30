using System.Net;
using FluentAssertions;
using SimpleAPI.Domain.Items;
using SimpleAPI.IntegrationTests.Setup;

namespace SimpleAPI.IntegrationTests.Endpoints;

public class ItemEndpointTests : IClassFixture<SimpleAPIFactory>
{
    private readonly SimpleAPIFactory _apiFactory;

    public ItemEndpointTests(SimpleAPIFactory apiFactory)
    {
        ArgumentNullException.ThrowIfNull(apiFactory);
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task When_RetrievingByID_Then_EntityIsRetrievedAndResponseIsOK()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "code": "123",
                     "description": "Testing 123"
                   }
                   """;
        var saveResponse = await client.PostStringAsJsonAsync("/items", json);
        var location = saveResponse.Headers.Location;
        var expectedID = Convert.ToInt32(location!.ToString().Split('/').Last());

        // Act
        var response = await client.GetAsync(location);

        // Assert
        response.ShouldReturn(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        var content = await response.Content.ReadAsStringAsync();

        var actual = content.FromJson<Item>();
        var expected = json.FromJson<Item>();
        expected.ID = expectedID;
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task When_RetrievingByNonExistingID_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/items/123");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
    }

    [Fact]
    public async Task When_RetrievingByMalFormed_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/items/abc");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
    }

    [Fact]
    public async Task When_PostingNewItem_EntityIsSavedAndResponseIsCreated()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "code": "123",
                     "description": "Testing 123"
                   }
                   """;

        // Act
        var response = await client.PostStringAsJsonAsync("/items", json);

        // Assert
        var dbContext = _apiFactory.GetContext();
        var actual = dbContext.Items.SingleOrDefault(i => i.Code == "123");
        response.ShouldReturn(HttpStatusCode.Created);
        actual.Should().NotBeNull();
        response.ShouldHaveLocationHeader($"/items/{actual!.ID}");

        // Database assertion 
        var expected = json.FromJson<Item>();
        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(i => i.ID));
    }

    [Fact]
    public async Task When_PostingExistingItem_Then_EntityIsSavedAndResponseIsCreated()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var existing = new Item
        {
            Code        = "412",
            Description = "Existing item"
        };
        var context = _apiFactory.GetContext();
        context.Add(existing);
        await context.SaveChangesAsync();
        var existingID = existing.ID;

        var json = $$"""
                     {
                       "id": {{existingID}},
                       "code": "412",
                       "description": "Changing item 412"
                     }
                     """;

        // Act
        var response = await client.PostStringAsJsonAsync($"/items", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.ShouldHaveLocationHeader($"/items/{existingID}");

        // Database assertion 
        var dbContext = _apiFactory.GetContext();
        var actual = dbContext.Items.SingleOrDefault(i => i.Code == "412");
        var expected = json.FromJson<Item>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task When_PostingNonExistingItem_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "id": 35,
                     "code": "412",
                     "description": "Changing item 412"
                   }
                   """;
        // Act
        var response = await client.PostStringAsJsonAsync("/items", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task When_PostingInvalidItem_Then_ResponseIsBadRequest()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "description": "Invalid item"
                   }
                   """;

        // Act
        var response = await client.PostStringAsJsonAsync("/items", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.BadRequest, "application/problem+json");
    }

    [Fact]
    public async Task When_DeletingItem_Then_EntityIsDeletedAndResponseIsNoContent()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var item = new Item
        {
            Code        = "TOD",
            Description = "To delete"
        };
        var context = _apiFactory.GetContext();
        context.Add(item);
        await context.SaveChangesAsync();
        var idToDelete = item.ID;

        // Act
        var response = await client.DeleteAsync($"/items/{idToDelete}");

        // Assert
        response.ShouldReturn(HttpStatusCode.NoContent);

        // Database assert
        var actual = context.Items.SingleOrDefault(i => i.ID == idToDelete);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task When_DeletingNonExistentItem_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/items/556");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
    }
}
