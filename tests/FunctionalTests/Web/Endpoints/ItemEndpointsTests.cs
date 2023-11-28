using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.FunctionalTests.Setup;

namespace SimpleAPI.FunctionalTests.Web.Endpoints;

// > Test cases
//
// - POST item should return Created
// - POST invalid item should return Bad Request
// - GET existing item should return OK
// - GET non existing item should return Not Found
// - GET using malformed id should return Bad Request
// - PUT existing item should return OK
// - PUT invalid existing item should return Bad Request
// - PUT existing item with different row version should return Conflict
// - PUT non existing item should return Not Found
// - DELETE existing item should return No Content
// - DELETE non existing item should return Not Found
// - DELETE item with different row version return Conflict

public class ItemEndpointsTests : IClassFixture<SimpleAPIFactory>
{
    private readonly HttpClient _client;
    private const string Endpoint = "/items";

    public ItemEndpointsTests(SimpleAPIFactory apiFactory)
        => _client = apiFactory.CreateClient();

    [Fact]
    private async Task POST_item_should_return_OK()
    {
        // Arrange
        var json = """
                   {
                     "code": "287",
                     "description": "Testing 287",
                     "price": 3.15
                   }
                   """;

        // Act
        var response = await _client.PostStringAsJsonAsync(Endpoint, json);

        // Assert
        response.ShouldReturn(HttpStatusCode.Created);

        var location = response.Headers.Location?.ToString();
        location.Should().NotBeNull()
            .And.MatchRegex($"{Endpoint}/([0-9])*");

        var id = response.Headers.Location.GetID();
        var actual = await _client.GetFromApiAsync<ItemViewModel>(location!);
        var expected = json.FromJson<ItemViewModel>();
        expected.ID = id;

        // New item, so RowVersion should be 1
        expected.RowVersion = 1;
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    private async Task POST_non_valid_item_should_return_Bad_Request()
    {
        // Arrange
        var json = """
                   {
                     "description": "Testing 189"
                   }
                   """;

        // Act
        var response = await _client.PostStringAsJsonAsync(Endpoint, json);

        // Assert
        response.ShouldReturn(HttpStatusCode.BadRequest, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad request");
        problem.Detail.Should().Be("Invalid request");
    }

    [Fact]
    public async Task GET_existing_item_should_return_OK()
    {
        // Arrange
        var json = """
                   {
                     "code": "139",
                     "description": "Testing 139",
                     "price": 4.34
                   }
                   """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, json);

        // Act
        var url = $"{Endpoint}/{newID}";
        var response = await _client.GetAsync(url);

        // Assert
        response.ShouldReturn(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();

        var content = await response.Content.ReadAsStringAsync();
        var actual = content.FromJson<ItemViewModel>();
        var expected = json.FromJson<ItemViewModel>();
        expected.ID = newID;
        actual.Should().BeEquivalentTo(expected, options => options.Excluding(p => p.RowVersion));
    }

    [Fact]
    public async Task GET_non_existing_item_should_return_Not_Found()
    {
        // Act
        var response = await _client.GetAsync($"{Endpoint}/987");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
        problem.Title.Should().Be("Not found");
        problem.Detail.Should().Be("Entity with ID 987 not found");
    }

    [Fact]
    public async Task GET_using_malformed_id_should_return_Bad_Request()
    {
        // Act
        var response = await _client.GetAsync("/items/abc");

        // Assert
        response.ShouldReturn(HttpStatusCode.BadRequest, "application/problem+json");
    }
    
    [Fact]
    public async Task PUT_existing_item_should_return_OK()
    {
        // Arrange
        var existing = """
                       {
                         "code": "321",
                         "description": "Testing 321",
                         "price": 1.32
                       }
                       """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, existing);

        // Act
        var json = """
                   {
                     "rowVersion": 1,
                     "code": "412",
                     "description": "Changing item 412",
                     "price": 3.32
                   }
                   """;
        var response = await _client.PutStringAsJsonAsync($"{Endpoint}/{newID}", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
    }

    [Fact]
    public async Task PUT_invalid_existing_item_should_return_Bad_Request()
    {
        // Arrange
        var existing = """
                       {
                         "code": "329",
                         "description": "Testing 329"
                       }
                       """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, existing);

        var json = """
                   {
                     "description": "Changing item 329"
                   }
                   """;
        // Act
        var response = await _client.PutStringAsJsonAsync($"{Endpoint}/{newID}", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.BadRequest, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad request");
        problem.Detail.Should().Be("Invalid request");
    }

    [Fact]
    public async Task PUT_non_existing_item_should_return_Not_Found()
    {
        // Arrange
        var json = """
                   {
                     "description": "Changing item 329"
                   }
                   """;
        // Act
        var response = await _client.PutStringAsJsonAsync($"{Endpoint}/978", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.BadRequest, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
        problem.Title.Should().Be("Bad request");
        problem.Detail.Should().Be("Invalid request");
    }

    [Fact]
    public async Task PUT_existing_item_with_different_row_version_should_return_Conflict()
    {
        // Arrange
        var existing = """
                       {
                         "code": "412",
                         "description": "Existing item 412"
                       }
                       """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, existing);

        // Act
        var json = """
                   {
                     "rowVersion": 12,
                     "code": "412",
                     "description": "Changing item 412"
                   }
                   """;
        var response = await _client.PutStringAsJsonAsync($"{Endpoint}/{newID}", json);

        // Assert
        response.ShouldReturn(HttpStatusCode.Conflict, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
        problem.Title.Should().Be("Entry modified");
        problem.Detail.Should().Be("Entity already modified");
    }

    [Fact]
    public async Task DELETE_existing_item_should_return_No_Content()
    {
        // Arrange
        var existing = """
                       {
                         "code": "538",
                         "description": "Existing item 538"
                       }
                       """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, existing);

        // Act
        var response = await _client.DeleteAsync($"{Endpoint}/{newID}?version=1");

        // Assert
        response.ShouldReturn(HttpStatusCode.NoContent);

        var getByID = () => _client.GetFromApiAsync<ItemViewModel>($"{Endpoint}/{newID}");
        await getByID.Should()
            .ThrowAsync<HttpRequestException>()
            .Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_non_existing_item_should_return_Not_Found()
    {
        // Act
        var response = await _client.DeleteAsync($"{Endpoint}/556?version=7");

        // Assert
        response.ShouldReturn(HttpStatusCode.NotFound, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
        problem.Title.Should().Be("Not found");
        problem.Detail.Should().Be("Entity with ID 556 not found");
    }

    [Fact]
    public async Task DELETE_item_with_different_row_version_return_Conflict()
    {
        // Arrange
        var existing = """
                       {
                         "code": "598",
                         "description": "Existing item 598"
                       }
                       """;
        var newID = await _client.CreateUsingApiAsync(Endpoint, existing);

        // Act
        var response = await _client.DeleteAsync($"{Endpoint}/{newID}?version=14");

        // Assert
        response.ShouldReturn(HttpStatusCode.Conflict, "application/problem+json");
        response.Content.Should().NotBeNull();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
        problem.Title.Should().Be("Entry modified");
        problem.Detail.Should().Be("Entity already modified");
    }
}
