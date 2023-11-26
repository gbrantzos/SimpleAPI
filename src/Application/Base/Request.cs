using MediatR;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Application.Base;

public abstract record Request<TResponse> : IRequest<Result<TResponse>>;

public record Command<TResponse> : Request<TResponse>;

public record Query<TResponse> : Request<TResponse>;
