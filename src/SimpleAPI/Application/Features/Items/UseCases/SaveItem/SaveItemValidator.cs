using FluentValidation;

namespace SimpleAPI.Application.Features.Items.UseCases.SaveItem;

public class SaveItemValidator : AbstractValidator<SaveItemCommand>
{
    public SaveItemValidator()
    {
        RuleFor(m => m.ViewModel.Code).NotEmpty();
        
        // TODO Remove after testing
        // RuleFor(m => m.Description)
        //     .MaximumLength(4)
        //     .Must(d => !d.StartsWith("Invalid", StringComparison.CurrentCultureIgnoreCase));
    }
}
