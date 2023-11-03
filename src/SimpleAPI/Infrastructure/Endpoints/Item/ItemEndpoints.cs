using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Application.Features.Items.UseCases.SaveItem;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Infrastructure.Endpoints.Item;

// ReSharper disable once ClassNeverInstantiated.Global
public class ItemEndpoints : IEndpointMapper
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("/items")
            .WithTags("Items")
            .WithOpenApi();

        group.MapGet("{id:int}", GetItem.Handle)
            .WithName("GetItem")
            .WithSummary("Get item by ID")
            .WithDescription("Retrieve an item by its ID.");
        group.MapPost("", SaveItem.Handle)
            .WithName("SaveItem")
            .WithSummary("Save new or existing item");
        group.MapDelete("{id:int}", DeleteItem.Handle)
            .WithName("DeleteItem")
            .WithSummary("Delete item by ID");
    }

    private static class GetItem
    {
        public static async Task<Results<Ok<Domain.Features.Items.Item>, NotFound>> Handle(int id,
            IItemRepository itemRepository,
            CancellationToken cancellationToken)
        {
            var item = await itemRepository.GetByID(id, cancellationToken);
            if (item is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(item);
        }
    }

    private static class SaveItem
    {
        public static async Task<IResult> Handle(Domain.Features.Items.Item item,
            IItemRepository itemRepository,
            IUnitOfWork unitOfWork,
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var viewModel = new ItemViewModel()
            {
                Code        = "",
                Description = ""
            };
            var result = await mediator.Send(new SaveItemCommand(0, viewModel), cancellationToken);
            if (String.IsNullOrEmpty(item.Code))
                return Results.BadRequest();

            var isNew = item.ID == 0;
            if (isNew)
            {
                await itemRepository.Add(item, cancellationToken);
            }
            else
            {
                var existing = await itemRepository.GetByID(item.ID, cancellationToken);
                if (existing is null)
                    // return Results.NotFound($"Entity not found, ID: {item.ID}");
                {
                    var problem = new ProblemDetails
                    {
                        Type     = $"https://httpstatuses.io/{HttpStatusCode.NotFound}",
                        Title    = "Item not found",
                        Status   = (int)HttpStatusCode.NotFound,
                        Detail   = $"Entity not found, ID: {item.ID}",
                        Instance = context.Request.Path
                    };
                    return Results.Problem(problem);
                }

                existing.Code                     = item.Code;
                existing.Description              = item.Description;
                context.Response.Headers.Location = $"/items/{item.ID}";
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var location = $"/items/{item.ID}";
            return isNew
                ? Results.Created(location, new { Result = "Item saved successfully" })
                : Results.Ok(new { Result                = "Item saved successfully" });
        }
    }

    private static class DeleteItem
    {
        public static async Task<IResult> Handle(int id,
            IItemRepository itemRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            var item = await itemRepository.GetByID(id, cancellationToken);
            if (item is null)
                return Results.NotFound();

            itemRepository.Delete(item);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        }
    }
}
