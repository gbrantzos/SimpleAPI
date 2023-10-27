using Microsoft.AspNetCore.Http.HttpResults;
using SimpleAPI.Domain.Items;

namespace SimpleAPI.Infrastructure.Endpoints.Item;

public static class GetItemEndpoint
{
    public static async Task<Results<Ok<Domain.Items.Item>, NotFound>> Handle(int id,
        IItemRepository itemRepository,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByID(id, cancellationToken);
        if (item is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(item);
    }
}
