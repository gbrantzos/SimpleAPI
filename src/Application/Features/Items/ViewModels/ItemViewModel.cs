using SimpleAPI.Application.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.ViewModels;

public class ItemViewModel : ViewModel
{
    public string Code { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
}

public static class ItemViewModelExtensions
{
    public static ItemViewModel ToViewModel(this Item item)
        => new()
        {
            ID          = item.ID,
            RowVersion  = item.RowVersion,
            Code        = item.Code,
            Description = item.Description
        };
}
