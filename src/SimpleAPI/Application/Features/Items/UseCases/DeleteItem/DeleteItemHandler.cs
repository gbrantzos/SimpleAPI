using SimpleAPI.Application.Core;
using SimpleAPI.Domain.Core;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.Application.Features.Items.UseCases.DeleteItem;

public class DeleteItemHandler : Handler<DeleteItemCommand, bool>
{
    private readonly IItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemHandler(IItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<bool, Error>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIDAsync(request.ID, cancellationToken);
        if (existing is null)
            return Error.Create(ErrorKind.NotFound, $"Entity with ID {request.ID} not found");
        
        _repository.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
