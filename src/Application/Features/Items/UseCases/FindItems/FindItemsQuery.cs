using SimpleAPI.Application.Base;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.FindItems;

public record FindItemsQuery(string QueryParams) : Query<IReadOnlyList<ItemViewModel>>;
