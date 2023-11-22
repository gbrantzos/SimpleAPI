using Prometheus;

namespace SimpleAPI.Application.Common.Metrics;

public static class SimpleAPIMetrics
{
    public static readonly Counter RequestsCounter = Prometheus.Metrics
        .CreateCounter("simple_requests_counter",
            "Simple API Requests counter",
            new CounterConfiguration
            {
                LabelNames = new[]
                {
                    "RequestType",
                    "RequestResult"
                }
            });

    public static readonly Histogram RequestsDuration = Prometheus.Metrics
        .CreateHistogram("simple_requests_duration",
            "Simple API Requests duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "RequestType" },
                Buckets    = new double[] { 50, 200, 500, 1000, 2000, 5000, 10000 }
            });

    public static readonly Counter QueriesCounter = Prometheus.Metrics
        .CreateCounter("simple_queries_counter", "Simple API SQL Queries counter",
            new CounterConfiguration { LabelNames = new[] { "QueryName" } });

    public static readonly Histogram QueriesDuration = Prometheus.Metrics
        .CreateHistogram("simple_queries_duration",
            "Simple API SQL Queries duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "QueryName" },
                Buckets    = new double[] { 50, 200, 500, 1000, 2000, 5000, 10000 }
            });
}
