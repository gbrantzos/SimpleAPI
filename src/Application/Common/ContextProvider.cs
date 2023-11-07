namespace SimpleAPI.Application.Common;

public class ContextProvider<T> where T : class
{
    public T? CurrentContext
    {
        get { return Current.Value?.Context; }
        set
        {
            var holder = Current.Value;
            if (holder != null)
            {
                // Clear current Context trapped in the AsyncLocals, as its done.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the Context in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                Current.Value = new ContextHolder { Context = value };
            }
        }
    }

    private sealed class ContextHolder
    {
        public T? Context { get; set; }
    }


    private static readonly AsyncLocal<ContextHolder> Current = new();
}
