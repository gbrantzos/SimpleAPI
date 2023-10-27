using SimpleAPI.Domain;
using SimpleAPI.Domain.Items;

namespace SimpleAPI.Infrastructure.Endpoints.Item;

public static class SaveItemEndpoint
{
    public static async Task<IResult> Handle(Domain.Items.Item item,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        HttpContext context,
        CancellationToken cancellationToken)
    {
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
                return Results.NotFound($"Entity not found, ID: {item.ID}");

            existing.Code                     = item.Code;
            existing.Description              = item.Description;
            context.Response.Headers.Location = $"/items/{item.ID}";
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var location = $"/items/{item.ID}";
        return isNew
            ? Results.Created(location, new { Result = "Item saved successfully" })
            : Results.Ok(new { Result = "Item saved successfully" });
    }
}
