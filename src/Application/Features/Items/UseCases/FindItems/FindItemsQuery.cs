using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.Application.Features.Items.UseCases.FindItems;

public record FindItemsQuery(string QueryParams) : Query<FindItemsResult>;

public class FindItemsResult : QueryResult<ItemViewModel>
{
    public FindItemsResult(IReadOnlyList<ItemViewModel> items) : base(items) { }
    public FindItemsResult(IReadOnlyList<ItemViewModel> items, int totalRows) : base(items, totalRows) { }
}
