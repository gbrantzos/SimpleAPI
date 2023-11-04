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
            .WithSummary("Save new item");
        group.MapPut("{id:int}", UpdateItem.Handle)
            .WithName("UpdateItem")
            .WithSummary("Update existing item");
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
            // TODO Replace with MediatR
            var item = await itemRepository.GetByIDAsync(id, cancellationToken);
            if (item is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(item);
        }
    }

    private static class SaveItem
    {
        public static async Task<IResult> Handle(
            ItemViewModel item,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.
                Send(new SaveItemCommand(item), cancellationToken);
            
            // TODO Create mapping error => problem details
            return response.Match(vm =>
            {
                var location = $"/items/{vm.ID}";
                return Results.Created(location, vm);
            }, error => Results.BadRequest());
        }
    }

    private static class UpdateItem
    {
        public static async Task<IResult> Handle(Domain.Features.Items.Item item,
            [FromRoute(Name = "id")] int existingID,
            IItemRepository itemRepository,
            IUnitOfWork unitOfWork,
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            // TODO Replace with MediatR
            // var viewModel = new ItemViewModel()
            // {
            //     Code        = "",
            //     Description = ""
            // };
            // var result = await mediator.Send(new SaveItemCommand(0, viewModel), cancellationToken);
            if (String.IsNullOrEmpty(item.Code))
                return Results.BadRequest();

            var existing = await itemRepository.GetByIDAsync(existingID, cancellationToken);
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

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.Ok(existing.ToViewModel());
        }
    }

    private static class DeleteItem
    {
        public static async Task<IResult> Handle(int id,
            IItemRepository itemRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            // TODO Replace with MediatR
            var item = await itemRepository.GetByIDAsync(id, cancellationToken);
            if (item is null)
                return Results.NotFound();

            itemRepository.Delete(item);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        }
    }
}
