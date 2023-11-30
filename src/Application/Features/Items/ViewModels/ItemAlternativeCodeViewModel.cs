using SimpleAPI.Application.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.ViewModels;

public class ItemAlternativeCodeViewModel : ViewModel
{
    public string Code { get; init; } = String.Empty;
    public int Kind { get; set; }
    public string? Description { get; init; }
}

public static class ItemAlternativeCodeViewModelExtensions
{
    public static ItemAlternativeCodeViewModel ToViewModel(this ItemAlternativeCode code)
        => new ItemAlternativeCodeViewModel()
        {
            Code        = code.Code,
            Kind        = (int)code.Kind,
            Description = code.Description
        };
}
