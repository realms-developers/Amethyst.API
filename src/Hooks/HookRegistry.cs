using System.Collections.Concurrent;

namespace Amethyst.Hooks;

public static class HookRegistry
{
    private static readonly ConcurrentDictionary<Type, object> _hooks = new();

    public static void RegisterHook<TArgs>(bool canBeCancelled, bool cancelByError = false)
    {
        if (_hooks.ContainsKey(typeof(TArgs)))
            throw new InvalidOperationException($"Hook for {typeof(TArgs).Name} is already registered.");

        var hook = new AmethystHook<TArgs>(GetHookName<TArgs>(), canBeCancelled, cancelByError);
        _hooks[typeof(TArgs)] = hook;
    }

    public static void UnregisterHook<TArgs>()
    {
        if (!_hooks.TryRemove(typeof(TArgs), out _))
            throw new InvalidOperationException($"Hook for {typeof(TArgs).Name} is not registered.");
    }

    public static AmethystHook<TArgs> GetHook<TArgs>()
    {
        if (_hooks.TryGetValue(typeof(TArgs), out var hook))
            return (AmethystHook<TArgs>)hook;

        throw new InvalidOperationException($"Hook for {typeof(TArgs).Name} is not registered.");
    }

    private static string GetHookName<TArgs>()
    {
        var name = typeof(TArgs).Name;
        if (name.EndsWith("Args"))
            name = name[..^4];

        return name;
    }
}
