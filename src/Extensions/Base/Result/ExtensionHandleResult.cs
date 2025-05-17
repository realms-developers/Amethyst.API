namespace Amethyst.Extensions.Result;

public sealed class ExtensionHandleResult
{
    public ExtensionHandleResult(ExtensionState state, string? errorMessage = null)
    {
        State = state;
        ErrorMessage = errorMessage;
    }

    public ExtensionState State { get; }
    public string? ErrorMessage { get; }

    public bool IsSuccess => State == ExtensionState.Initialized;
}
