using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Core.Guards;
using SimpleAPI.Domain.Features.Common;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.SaveItem;

public class SaveItemHandler : Handler<SaveItemCommand, ItemViewModel>
{
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveItemHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository.ThrowIfNull();
        _unitOfWork     = unitOfWork.ThrowIfNull();
    }

    public override async Task<Result<ItemViewModel>> Handle(SaveItemCommand request, CancellationToken cancellationToken)
    {
        Ensure.NotNull(request);

        Item item;
        if (request.ItemID == 0)
        {
            // New Item
            item = Item.Create(request.ViewModel.Code, request.ViewModel.Description);
            _itemRepository.Add(item);
        }
        else
        {
            // Existing item
            var existing = await _itemRepository.GetByIDAsync(new ItemID(request.ItemID), cancellationToken);
            if (existing is null)
                return Error.Create(ErrorKind.NotFound, $"Entity not found, ID: {request.ItemID}");
            if (existing.RowVersion != request.ViewModel.RowVersion)
                return Error.Create(ErrorKind.ModifiedEntry, $"Entity already modified");

            item = existing;
        }

        item.SetPrice(Money.InEuro(request.ViewModel.Price));
        HandleAlternativeCodes(item, request.ViewModel.AlternativeCodes);
        
        // Add any other modifications on existing
        item.Description = request.ViewModel.Description;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return item.ToViewModel();
    }

    private static void HandleAlternativeCodes(Item item, IEnumerable<ItemAlternativeCodeViewModel> alternativeCodes)
    {
        var alternatives = alternativeCodes.SmartToList();
        var missingCodes = alternatives
            .Select(a => a.Code)
            .ToList();
        
        var missing = item
            .AlternativeCodes
            .Where(a => !missingCodes.Contains(a.Code))
            .ToList();
        
        foreach (var code in missing)
            item.RemoveCode(code.Code);

        foreach (var code in alternatives)
        {
            var existing = item.GetCode(code.Code);
            if (existing is null)
            {
                item.AddCode((ItemCode)code.Code, code.Description);
                continue;
            }
        
            // Add any other modifications on existing 
            existing.Description = code.Description;
        }
    }
}
