using SimpleAPI.Application.Base;

namespace SimpleAPI.Application.Features.Items.UseCases.DeleteItem;

public record DeleteItemCommand(int ID) : Command<bool> { }
