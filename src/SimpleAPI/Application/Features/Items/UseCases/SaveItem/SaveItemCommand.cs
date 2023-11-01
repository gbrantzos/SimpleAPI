using SimpleAPI.Application.Core;
using SimpleAPI.Application.Features.Items.ViewModels;

namespace SimpleAPI.Application.Features.Items.UseCases.SaveItem;

public record SaveItemCommand(int ItemID, ItemViewModel ViewModel) : Command<ItemViewModel>;
