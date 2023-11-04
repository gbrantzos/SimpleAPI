using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Features.Items;
using SimpleAPI.IntegrationTests.Setup;

namespace SimpleAPI.IntegrationTests.Infrastructure.Endpoints;

public class ItemEndpointTests : IClassFixture<SimpleAPIFactory>
{
    private readonly SimpleAPIFactory _apiFactory;

    public ItemEndpointTests(SimpleAPIFactory apiFactory)
    {
        ArgumentNullException.ThrowIfNull(apiFactory);
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task When_RequestByID_Then_EntityIsRetrievedAndResponseIsOK()
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

        var actual = content.FromJson<ItemViewModel>();
        var expected = json.FromJson<ItemViewModel>();
        expected.ID = expectedID;
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task When_RequestByNonExistingID_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/items/123");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
    }

    [Fact]
    public async Task When_RequestByMalFormed_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/items/abc");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
    }

    [Fact]
    public async Task When_SendingNewItem_EntityIsSavedAndResponseIsCreated()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "code": "156",
                     "description": "Testing 156"
                   }
                   """;

        // Act
        var response = await client.PostStringAsJsonAsync("/items", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.Created);

        var location = response.Headers.Location?.ToString();
        location.Should().NotBeNull()
            .And.MatchRegex("/items/([0-9])*");

        var dbContext = _apiFactory.GetContext();
        var actual = dbContext.Items.SingleOrDefault(i => i.Code == "156");
        actual.Should().NotBeNull();

        // Database assertion 
        var expected = json.FromJson<ItemViewModel>();
        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(i => i.ID));
    }

    [Fact]
    public async Task When_SendingExistingItem_Then_EntityIsSavedAndResponseIsCreated()
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

        var json = """
                   {
                     "code": "412",
                     "description": "Changing item 412"
                   }
                   """;

        // Act
        var response = await client.PutStringAsJsonAsync($"/items/{existingID}", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();

        // Database assertion 
        var dbContext = _apiFactory.GetContext();
        var actual = dbContext.Items.SingleOrDefault(i => i.Code == "412");
        var expected = json.FromJson<ItemViewModel>();
        expected.ID = existingID;
        
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task When_SendingNonExistingItem_Then_ResponseIsNotFound()
    {
        // Arrange
        var client = _apiFactory.CreateClient();
        var json = """
                   {
                     "code": "412",
                     "description": "Changing item 412"
                   }
                   """;
        // Act
        var response = await client.PutStringAsJsonAsync("/items/35", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content);
        // TODO Add assertions on problem details
    }

    [Fact]
    public async Task When_SendingInvalidItem_Then_ResponseIsBadRequest()
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
