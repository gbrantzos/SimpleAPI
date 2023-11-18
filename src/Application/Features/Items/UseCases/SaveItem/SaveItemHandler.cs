using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Core.Guards;
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

    public override async Task<Result<ItemViewModel, Error>> Handle(SaveItemCommand request, CancellationToken cancellationToken)
    {
        Ensure.NotNull(request);
        
        var isNew = request.ItemID == 0;
        var item = isNew
            ? new Item()
            : await _itemRepository.GetByIDAsync(new ItemID(request.ItemID), cancellationToken);
        if (item is null)
        {
            return Error.Create(ErrorKind.NotFound, $"Entity not found, ID: {request.ItemID}");
        }
        if (!isNew && item.RowVersion != request.ViewModel.RowVersion)
        {
            return Error.Create(ErrorKind.ModifiedEntry, $"Entity already modified");
        }

        item.Code        = request.ViewModel.Code;
        item.Description = request.ViewModel.Description;

        if (isNew)
        {
            _itemRepository.Add(item);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.ToViewModel();
    }
}
