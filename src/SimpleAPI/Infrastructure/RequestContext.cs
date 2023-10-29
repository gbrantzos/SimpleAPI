namespace SimpleAPI.Infrastructure;

public class RequestContext
{
    public string ExecutionID { get; init; } = Guid.NewGuid().ToString("N");
    public DateTime StartedOn { get; init; } = DateTime.UtcNow;
}
