using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Core.Guards;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.GetItem;

public class GetItemHandler : Handler<GetItemCommand, ItemViewModel>
{
    private readonly IItemRepository _repository;

    public GetItemHandler(IItemRepository repository)
    {
        _repository = repository.ThrowIfNull();
    }

    public override async Task<Result<ItemViewModel>> Handle(GetItemCommand request, CancellationToken cancellationToken)
    {
        Ensure.NotNull(request);

        var existing = await _repository.GetByIDAsync(new ItemID(request.ID), cancellationToken);
        if (existing is null)
            return Error.Create(ErrorKind.NotFound, $"Entity with ID {request.ID} not found");

        return existing.ToViewModel();
    }
}
