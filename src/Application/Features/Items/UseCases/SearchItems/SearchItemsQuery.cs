using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.SearchItems;

public record SearchItemsQuery(string QueryParams) : Query<IEnumerable<ItemViewModel>>;
