using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.SearchItems;

public class SearchItemsHandler : Handler<SearchItemsQuery, IEnumerable<ItemViewModel>>
{
    private readonly IItemRepository _repository;

    public SearchItemsHandler(IItemRepository repository)
    {
        _repository = repository.ThrowIfNull();
    }

    public override async Task<Result<IEnumerable<ItemViewModel>>> Handle(SearchItemsQuery request,
        CancellationToken cancellationToken)
    {
        var criteria = SearchCriteria.Parse<Item>(request.QueryParams);
        var results = await _repository.FindAsync(criteria, cancellationToken);

        return results.Items.Select(i => i.ToViewModel()).ToList();
    }
}
