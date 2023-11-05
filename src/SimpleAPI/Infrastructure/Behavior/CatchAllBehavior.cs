using MediatR;

namespace SimpleAPI.Infrastructure.Behavior;

public class CatchAllBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;

    public CatchAllBehavior(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(nameof(CatchAllBehavior<TRequest, TResponse>));
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch
        {
            _logger.LogWarning("[{RequestType}] Unhandled exception. {@Request}", typeof(TRequest).Name, request);
            throw;
        }
    }
}
