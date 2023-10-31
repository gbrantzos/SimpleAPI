namespace SimpleAPI.Infrastructure.Setup;

public class RequestContext
{
    public string ExecutionID { get; private set; }
    public DateTime StartedOn { get; private set; }

    private RequestContext()
    {
        ExecutionID = Guid.NewGuid().ToString("N");
        StartedOn   = DateTime.UtcNow;
    }

    public static RequestContext Create() => new();
}
