using System.Text.Json.Nodes;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SimpleAPI.Application.Features.Items.UseCases.DeleteItem;
using SimpleAPI.Application.Features.Items.UseCases.GetItem;
using SimpleAPI.Application.Features.Items.UseCases.SaveItem;
using SimpleAPI.Application.Features.Items.UseCases.SearchItems;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Base;
using SimpleAPI.Web.ErrorMapping;

namespace SimpleAPI.Web.Endpoints.Item;

// ReSharper disable once ClassNeverInstantiated.Global
public class ItemEndpoints : IEndpointMapper
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("/items")
            .WithTags("Items")
            .WithOpenApi();

        group.MapGet("/", SearchItem.Handle)
            .WithName("SearchItems")
            .WithSummary("Search items")
            .WithDescription("Retrieve all items using the specified search criteria from query parameters.");
        group.MapGet("{id}", GetItem.Handle)
            .WithName("GetItem")
            .WithSummary("Get item by ID")
            .WithDescription("Retrieve an item by its ID.")
            .Produces<ItemViewModel>(200, "application/json");
        group.MapPost("", SaveItem.Handle)
            .WithName("SaveItem")
            .WithSummary("Save new item")
            .Produces<ItemViewModel>(200, "application/json");
        // .WithOpenApi(op =>
        // {
        //     var example = new OpenApiExample
        //     {
        //         Summary     = "Example",
        //         Description = "This is an example value",
        //         Value       = new OpenApiString("{\"id\": 1}"),
        //     };
        //     op.Responses["200"].Content["application/json"].Examples.Add("base", example);
        //     return op;
        // });
        group.MapPut("{id}", UpdateItem.Handle)
            .WithName("UpdateItem")
            .WithSummary("Update existing item");
        group.MapDelete("{id}", DeleteItem.Handle)
            .WithName("DeleteItem")
            .WithSummary("Delete item by ID");
    }

    private static class GetItem
    {
        public static async Task<Results<Ok<ItemViewModel>, ProblemHttpResult>> Handle(int id,
            IMediator mediator,
            ErrorMapper errorMapper,
            CancellationToken cancellationToken)
        {
            var request = new GetItemCommand(id);
            var response = await mediator.Send(request, cancellationToken);

            return response.Match<Results<Ok<ItemViewModel>, ProblemHttpResult>>(
                vm => TypedResults.Ok(vm),
                error => TypedResults.Problem(errorMapper.MapToProblemDetails(error))
            );
        }
    }

    private static class SaveItem
    {
        public static async Task<IResult> Handle(
            ItemViewModel item,
            IMediator mediator,
            ErrorMapper errorMapper,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new SaveItemCommand(item), cancellationToken);

            return response.Match(
                vm =>
                {
                    var location = $"/items/{vm.ID}";
                    return Results.Created(location, vm);
                },
                error => Results.Problem(errorMapper.MapToProblemDetails(error)));
        }
    }

    private static class UpdateItem
    {
        public static async Task<IResult> Handle(ItemViewModel item,
            [FromRoute(Name = "id")] int existingID,
            IMediator mediator,
            ErrorMapper errorMapper,
            CancellationToken cancellationToken)
        {
            var request = new SaveItemCommand(existingID, item);
            var response = await mediator.Send(request, cancellationToken);

            return response.Match(
                Results.Ok,
                error => Results.Problem(errorMapper.MapToProblemDetails(error)));
        }
    }

    private static class DeleteItem
    {
        public static async Task<IResult> Handle(int id,
            [FromQuery(Name = "version")] int rowVersion,
            IMediator mediator,
            ErrorMapper errorMapper,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new DeleteItemCommand(id, rowVersion), cancellationToken);

            return response.Match(
                _ => Results.NoContent(),
                error => Results.Problem(errorMapper.MapToProblemDetails(error))
            );
        }
    }
    
    private static class SearchItem
    {
        public static async Task<IResult> Handle(HttpRequest request,
            IMediator mediator,
            ErrorMapper errorMapper,
            CancellationToken cancellationToken)
        {
            var queryParams = (request.QueryString.Value ?? "?")[1..];
            var response = await mediator.Send(new SearchItemsQuery(queryParams));

            return response.Match(
                list => Results.Ok(list),
                error => Results.Problem(errorMapper.MapToProblemDetails(error))
            );
        }
    }
}
