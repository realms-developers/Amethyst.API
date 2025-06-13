namespace Amethyst.Hooks.Context;

public sealed class HookResult<TArgs>
{
    internal HookResult(bool canBeCancelled, bool canBeModified)
    {
        CanBeCancelled = canBeCancelled;
        CanBeModified = canBeModified;
    }

    public bool CanBeCancelled { get; }

    public bool CanBeModified { get; }

    public bool IsModified { get; private set; }

    public bool IsCancelled => _cancellationReasons.Count > 0;

    public IReadOnlyList<string> CancellationReasons => _cancellationReasons.AsReadOnly();

    public TArgs? Args { get; private set; }

    private readonly List<string> _cancellationReasons = new();

    public void Cancel(string reason)
    {
        if (!CanBeCancelled)
        {
            throw new InvalidOperationException("This hook result cannot be cancelled.");
        }

        if (_cancellationReasons.Contains(reason))
        {
            return;
        }

        _cancellationReasons.Add(reason);
    }

    public void Modify(TArgs args)
    {
        if (!CanBeModified)
        {
            throw new InvalidOperationException("This hook result cannot be modified.");
        }

        IsModified = true;
        Args = args;
    }
}
