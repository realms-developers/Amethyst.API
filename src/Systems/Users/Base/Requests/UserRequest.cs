namespace Amethyst.Systems.Users.Base.Requests;

public sealed class UserRequest<TContext> where TContext : class
{
    internal UserRequest(string name, int index, TContext ctx, TimeSpan? removeIn, bool autoRemove = true, RequestCallback<TContext>? timeout = null, RequestCallback<TContext>? accepted = null, RequestCallback<TContext>? rejected = null, RequestCallback<TContext>? cancelled = null)
    {
        Name = name;
        Index = index;
        Context = ctx;
        AcceptedCallback = accepted;
        RejectedCallback = rejected;
        CancelledCallback = cancelled;
        AutoRemove = autoRemove;

        if (removeIn != null)
        {
            DisposeTimer = new Timer(state =>
            {
                if (AutoRemove && RemoveCallback != null)
                {
                    RemoveCallback();
                }
            }, null, removeIn.Value, Timeout.InfiniteTimeSpan);

            DisposeCallback = () =>
            {
                if (!IsAccepted && !IsRejected)
                    TryInvoke(TimeoutCallback);

                DisposeTimer.Dispose();
            };
        }
    }

    public string Name { get; }
    public int Index { get; internal set; }

    public TContext Context { get; set; }

    public bool AutoRemove { get; } = true;

    public RequestCallback<TContext>? AcceptedCallback { get; }
    public RequestCallback<TContext>? RejectedCallback { get; }
    public RequestCallback<TContext>? CancelledCallback { get; }
    public RequestCallback<TContext>? TimeoutCallback { get; }

    public bool IsAccepted { get; private set; }
    public bool IsRejected { get; private set; }

    internal Timer? DisposeTimer { get; private set; }
    internal Action? RemoveCallback { get; set; }
    internal Action? DisposeCallback { get; set; }

    public bool Accept(TContext? ctx = null)
    {
        if (IsAccepted)
        {
            return false; // Already accepted
        }

        ctx ??= Context;

        if (TryInvoke(AcceptedCallback))
        {
            IsAccepted = true;

            return true;
        }

        AmethystLog.System.Warning($"UserRequest<{Name}>",
            $"Failed to accept request {Name} at index {Index}. Callback not invoked or failed.");
        return false;
    }

    public bool Reject(TContext? ctx = null)
    {
        if (IsRejected)
        {
            return false; // Cannot reject an accepted request
        }

        ctx ??= Context;

        if (TryInvoke(RejectedCallback))
        {
            AmethystLog.System.Info($"UserRequest<{Name}>", $"Request {Name} at index {Index} rejected.");

            return true;
        }

        AmethystLog.System.Warning($"UserRequest<{Name}>",
            $"Failed to reject request {Name} at index {Index}. Callback not invoked or failed.");
        return false;
    }

    private bool TryInvoke(RequestCallback<TContext>? callback)
    {
        if (callback != null)
        {
            try
            {
                callback(this, Context);

                if (AutoRemove && RemoveCallback != null)
                {
                    RemoveCallback();
                }

                return true;
            }
            catch (Exception ex)
            {
                AmethystLog.System.Critical($"UserRequest<{Name}>",
                    $"Error invoking callback for request {Name} at index {Index}: {ex.Message}");
            }
        }

        return false;
    }
}
