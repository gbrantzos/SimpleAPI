using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;
using SimpleAPI.Core.Guards;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.DeleteItem;

public class DeleteItemHandler : Handler<DeleteItemCommand, bool>
{
    private readonly IItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemHandler(IItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository.ThrowIfNull();
        _unitOfWork = unitOfWork.ThrowIfNull();
    }

    public override async Task<Result<bool, Error>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        Ensure.NotNull(request);
        
        var existing = await _repository.GetByIDAsync(new ItemID(request.ID), cancellationToken);
        if (existing is null)
            return Error.Create(ErrorKind.NotFound, $"Entity with ID {request.ID} not found");
        if (existing.RowVersion != request.RowVersion)
            return Error.Create(ErrorKind.ModifiedEntry, $"Entity already modified");

        _repository.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
