using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.FindItems;

public class FindItemsHandler : Handler<FindItemsQuery, IReadOnlyList<ItemViewModel>>
{
    private readonly IItemRepository _repository;

    public FindItemsHandler(IItemRepository repository)
    {
        _repository = repository.ThrowIfNull();
    }

    public override async Task<Result<IReadOnlyList<ItemViewModel>>> Handle(FindItemsQuery request,
        CancellationToken cancellationToken)
    {
        var criteria = SearchCriteria.Parse<Item>(request.QueryParams);
        var results = await _repository.FindAsync(criteria, cancellationToken);

        return results
            .Select(i => i.ToViewModel())
            .ToList();
    }
}
