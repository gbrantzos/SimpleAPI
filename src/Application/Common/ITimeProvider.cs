namespace SimpleAPI.Application.Common;

public interface ITimeProvider
{
    DateTime GetNow();
    DateTime GetUtcNow();
}

public class SystemTimeProvider : ITimeProvider
{
    public DateTime GetNow() => DateTime.Now;

    public DateTime GetUtcNow() => DateTime.UtcNow;
}

