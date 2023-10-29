using Serilog.Core;
using Serilog.Events;
using SimpleAPI.Application;

namespace SimpleAPI.Infrastructure.Setup;

public class RequestContextEnricher : ILogEventEnricher
{
    private readonly ContextProvider<RequestContext> _contextProvider;

    public RequestContextEnricher(ContextProvider<RequestContext> contextProvider)
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
