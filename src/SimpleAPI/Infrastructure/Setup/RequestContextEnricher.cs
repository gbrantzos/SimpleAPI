using Serilog.Core;
using Serilog.Events;

namespace SimpleAPI.Infrastructure.Setup;

public class RequestContextEnricher : ILogEventEnricher
{
    private readonly RequestContextProvider _contextProvider;

    public RequestContextEnricher(RequestContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = _contextProvider.CurrentContext;
        var executionID = context?.ExecutionID;
        
        if (executionID is not null)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ExecutionID", executionID));
    }
}
