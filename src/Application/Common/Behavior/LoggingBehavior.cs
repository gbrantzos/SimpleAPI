using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SimpleAPI.Application.Base;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Application.Common.Behavior;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : Request<TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger<LoggingBehavior<TRequest, TResponse>>();

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        try
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>()
            {
                ["RequestType"] = requestType
            });
            var sw = new Stopwatch();
            sw.Start();
            var response = await next();
            sw.Stop();
            response.Match(
                _ => _logger.LogInformation("[{RequestType}] => {Request} executed successfully ({ElapsedMs}ms)",
                    requestType,
                    request,
                    sw.ElapsedMilliseconds),
                error => _logger.LogWarning("[{RequestType}] => {Request} execution failed ({ElapsedMs}ms)\n{ErrorMessage}",
                    requestType,
                    request,
                    sw.ElapsedMilliseconds,
                    error.Message)
            );

            return response;
        }
        catch (OperationCanceledException tx)
        {
            _logger.LogWarning("[{RequestType}] => execution was cancelled: {Cancellation}\n\n{RequestJson}\n",
                requestType,
                tx.Message,
                request.ToJsonForLogging());
            throw;
        }
        catch (Exception x)
        {
            _logger.LogWarning("[{RequestType}] => execution failed, unhandled exception: {Unhandled}\n\n{RequestJson}\n",
                requestType,
                x.Message,
                request.ToJsonForLogging());
            throw;
        }
    }
}
