namespace Amethyst.Extensions.Result;

public sealed class ExtensionHandleResult
{
    public ExtensionHandleResult(ExtensionResult result, string? errorMessage = null)
    {
        State = result;
        ErrorMessage = errorMessage;
    }

    public ExtensionResult State { get; }
    public string? ErrorMessage { get; }
}
