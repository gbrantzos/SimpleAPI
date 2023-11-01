using MediatR;

namespace SimpleAPI.Application.Core;

public abstract record Request<TResponse> : IRequest<Result<TResponse, Error>>;

public record Command<TResponse> : Request<TResponse>;

public record Query<TResponse> : Request<TResponse>;
