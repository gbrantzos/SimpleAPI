using MediatR;

namespace SimpleAPI.Application.Core;

public abstract class Handler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse, Error>>
    where TRequest : IRequest<Result<TResponse, Error>>
{
    public abstract Task<Result<TResponse, Error>> Handle(TRequest request, CancellationToken cancellationToken);
}
