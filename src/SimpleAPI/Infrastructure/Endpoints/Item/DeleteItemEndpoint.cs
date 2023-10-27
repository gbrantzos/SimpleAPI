using Microsoft.AspNetCore.Http.HttpResults;
using SimpleAPI.Domain;
using SimpleAPI.Domain.Items;

namespace SimpleAPI.Infrastructure.Endpoints.Item;

public static class DeleteItemEndpoint
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
