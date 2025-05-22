using Amethyst.Hooks.Context;

namespace Amethyst.Hooks;

public sealed class AmethystHook<TArgs>
{
    public AmethystHook(string name, bool canBeCancelled, bool canBeModified, bool cancelByError = false)
    {
        Name = name;
        CanBeCancelled = canBeCancelled;
        CanBeModified = canBeModified;
        CancelByError = cancelByError;
    }

    public string Name { get; }

    public bool CanBeCancelled { get; }

    public bool CanBeModified { get; }

    public bool CancelByError { get; set; }

    internal List<HookHandler<TArgs>> Handlers { get; } = new();

    public void Register(HookHandler<TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (Handlers.Contains(handler))
            throw new InvalidOperationException($"Handler {handler.Method.Name} is already registered for hook {Name}.");

        Handlers.Add(handler);
    }

    public void Unregister(HookHandler<TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        Handlers.Remove(handler);
    }

    public HookResult<TArgs> Invoke(TArgs args)
    {
        var result = new HookResult<TArgs>(CanBeCancelled, CanBeModified);

        try
        {
            Handlers.ForEach(p => p.Invoke(in args, result));
        }
        catch (Exception ex)
        {
            AmethystLog.System.Error("Hooks", $"Error in hook {Name}: {ex.Message}");
            if (CancelByError)
            {
                result.Cancel($"CancelByError = true -> Hook error: {ex.Message}");
            }
        }

        if (result.IsCancelled)
        {
            AmethystLog.System.Debug("Hooks", $"Hook {Name} was cancelled: {string.Join(", ", result.CancellationReasons)}");
        }

        return result;
    }
}
