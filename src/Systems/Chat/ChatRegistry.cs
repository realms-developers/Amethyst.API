namespace Amethyst.Systems.Chat;

public sealed class ChatRegistry<T>
{
    public IReadOnlyList<T> Handlers => _handlers.AsReadOnly();

    internal readonly List<T> _handlers = new();

    public void Add(T handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (_handlers.Contains(handler))
        {
            throw new InvalidOperationException($"Chat message handler '{handler}' is already registered.");
        }

        _handlers.Add(handler);
    }

    public void Remove(T handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (!_handlers.Remove(handler))
        {
            throw new InvalidOperationException($"Chat message handler '{handler}' is not registered.");
        }
    }

    public void Clear()
    {
        _handlers.Clear();
    }
}
