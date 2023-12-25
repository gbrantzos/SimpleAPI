using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.FindItems;

public class FindItemsHandler : Handler<FindItemsQuery, FindItemsResult>
{
    private readonly IItemRepository _repository;

    public FindItemsHandler(IItemRepository repository)
    {
        _repository = repository.ThrowIfNull();
    }

    public override async Task<Result<FindItemsResult>> Handle(FindItemsQuery request,
        CancellationToken cancellationToken)
    {
        var criteria = SearchCriteria.Parse<Item>(request.QueryParams);
        var results = await _repository.FindAsync(criteria, cancellationToken);

        var items = results
            .Select(i => i.ToViewModel())
            .ToList();
        var result = criteria.IsPaged
            ? new FindItemsResult(items, await _repository.CountAsync(criteria.Specification, cancellationToken))
            : new FindItemsResult(items);
        return result;
    }
}
