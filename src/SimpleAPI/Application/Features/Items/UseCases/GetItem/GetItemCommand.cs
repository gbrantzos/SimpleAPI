using SimpleAPI.Application.Core;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.GetItem;

public record GetItemCommand(int ID) : Command<ItemViewModel>;
