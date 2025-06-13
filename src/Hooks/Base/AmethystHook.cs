using Amethyst.Hooks.Context;

namespace Amethyst.Hooks;

public sealed class AmethystHook<TArgs>(string name, bool canBeCancelled, bool canBeModified, bool cancelByError = false)
{
    public string Name { get; } = name;

    public bool CanBeCancelled { get; } = canBeCancelled;

    public bool CanBeModified { get; } = canBeModified;

    public bool CancelByError { get; set; } = cancelByError;

    internal List<HookHandler<TArgs>> _handlers = new();
    internal HookHandler<TArgs>[] _ivkHandlers = [];

    public void Register(HookHandler<TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (_ivkHandlers.Contains(handler))
        {
            throw new InvalidOperationException($"Handler {handler.Method.Name} is already registered for hook {Name}.");
        }

        _handlers.Add(handler);
        _ivkHandlers = _handlers.ToArray();
    }

    public void Unregister(HookHandler<TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.Remove(handler);
        _ivkHandlers = _handlers.ToArray();
    }

    public HookResult<TArgs> Invoke(TArgs args)
    {
        var result = new HookResult<TArgs>(CanBeCancelled, CanBeModified);

        try
        {
            for (int i = 0; i < _ivkHandlers.Length; i++)
            {
                HookHandler<TArgs> handler = _ivkHandlers[i];
                handler.Invoke(in args, result);
            }
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
