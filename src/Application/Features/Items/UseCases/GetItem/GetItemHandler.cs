using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.GetItem;

public class GetItemHandler : Handler<GetItemCommand, ItemViewModel>
{
    private readonly IItemRepository _repository;

    public GetItemHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public override async Task<Result<ItemViewModel, Error>> Handle(GetItemCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIDAsync(new ItemID(request.ID), cancellationToken);
        if (existing is null)
            return Error.Create(ErrorKind.NotFound, $"Entity with ID {request.ID} not found");

        return existing.ToViewModel();
    }
}
