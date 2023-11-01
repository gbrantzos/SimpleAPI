using SimpleAPI.Application.Core;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.SaveItem;

public class SaveItemHandler : Handler<SaveItemCommand, ItemViewModel>
{
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveItemHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _unitOfWork     = unitOfWork;
    }

    public override async Task<Result<ItemViewModel, Error>> Handle(SaveItemCommand request, CancellationToken cancellationToken)
    {
        var isNew = request.ItemID == 0;
        var item = isNew
            ? new Item()
            : await _itemRepository.GetByID(request.ItemID, cancellationToken);
        if (item is null)
        {
            return Error.Create(ErrorKind.NotFound, $"Entity not found, ID: {request.ItemID}");
        }

        item.Code        = request.ViewModel.Code;
        item.Description = request.ViewModel.Description;

        if (isNew)
        {
            await _itemRepository.Add(item, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ItemViewModel
        {
            Code        = item.Code,
            Description = item.Description
        };
    }
}
