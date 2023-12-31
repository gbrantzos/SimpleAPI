using SimpleAPI.Application.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.ViewModels;

public class ItemViewModel : ViewModel
{
    public string Code { get; init; } = String.Empty;
    public string Description { get; init; } = String.Empty;
    public decimal Price { get; init; }

    public IEnumerable<ItemAlternativeCodeViewModel> AlternativeCodes { get; init; } = new List<ItemAlternativeCodeViewModel>();
}

public static class ItemViewModelExtensions
{
    public static ItemViewModel ToViewModel(this Item item)
        => new()
        {
            ID          = item.ID.Value,
            RowVersion  = item.RowVersion,
            Code        = item.Code,
            Description = item.Description,
            Price       = item.Price.Amount,
            AlternativeCodes = item
                .AlternativeCodes
                .Where(c => c.Kind != ItemAlternativeCode.CodeKind.Base)
                .Select(c => c.ToViewModel())
                .ToList()
        };
}
