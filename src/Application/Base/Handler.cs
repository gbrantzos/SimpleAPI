using MediatR;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Application.Base;

public abstract class Handler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    public abstract Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}
