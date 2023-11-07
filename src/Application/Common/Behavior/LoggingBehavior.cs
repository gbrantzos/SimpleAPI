using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SimpleAPI.Application.Base;
using SimpleAPI.Core;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Application.Common.Behavior;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger<LoggingBehavior<TRequest, TResponse>>();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
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
            var result = (Result<TResponse, Error>)response;
            result.Match(
                _ => _logger.LogInformation("[{RequestType}] Executed successfully => {Request} ({ElapsedMs}ms)",
                    requestType,
                    request,
                    sw.ElapsedMilliseconds),
                error => _logger.LogWarning("[{RequestType}] Execution failed => {Request} ({ElapsedMs}ms)\n{ErrorMessage}",
                    requestType,
                    request,
                    sw.ElapsedMilliseconds,
                    error.Message)
            );

            return response;
        }
        catch
        {
            _logger.LogWarning("[{RequestType}] Execution failed, unhandled exception => {Request}\n\n{RequestJson}\n",
                requestType,
                request,
                request.ToJsonForLogging());
            throw;
        }
    }
}
