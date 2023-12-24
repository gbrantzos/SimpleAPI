using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.FindItems;

public record FindItemsQuery(string QueryParams) : Query<FindItemsResult>;

public class FindItemsResult
{
    public IReadOnlyList<ItemViewModel> Items { get; private set; }
    public int Count { get; private set; }

    public FindItemsResult(IReadOnlyList<ItemViewModel> items)
    {
        Items = items;
        Count = items.Count;
    }
}
