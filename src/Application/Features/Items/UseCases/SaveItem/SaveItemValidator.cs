using FluentValidation;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.SaveItem;

public class SaveItemValidator : AbstractValidator<SaveItemCommand>
{
    public SaveItemValidator() 
        => RuleFor(m => m.ViewModel).SetValidator(new ItemViewModelValidator());
}

public class ItemViewModelValidator : AbstractValidator<ItemViewModel>
{
    public ItemViewModelValidator() => RuleFor(m => m.Code)
        .NotEmpty()
        .Length(3, 50);
}
