using SimpleAPI.Application.Core;

namespace SimpleAPI.Application.Features.Items.UseCases.DeleteItem;

public record DeleteItemCommand(int ID) : Command<bool> { }
