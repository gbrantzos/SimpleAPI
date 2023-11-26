using System.Diagnostics;
using MediatR;
using SimpleAPI.Application.Base;
using SimpleAPI.Application.Common.Metrics;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Application.Common.Behavior;

public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : Request<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var sw = new Stopwatch();
        sw.Start();

        var result = await next();
        sw.Stop();

        var requestType = typeof(TRequest).Name;
        SimpleAPIMetrics
            .RequestsCounter
            .WithLabels(requestType, result.HasErrors ? "FAILED" : "SUCCESS")
            .Inc();
        SimpleAPIMetrics
            .RequestsDuration
            .WithLabels(requestType)
            .Observe(sw.Elapsed.TotalMilliseconds);

        return result;
    }
}
