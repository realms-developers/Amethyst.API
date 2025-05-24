namespace Amethyst.Extensions.Base.Result;

public sealed class ExtensionHandleResult(Guid loadIdentifier, ExtensionResult result, string? errorMessage = null)
{
    public Guid LoadIdentifier { get; } = loadIdentifier;
    public ExtensionResult State { get; } = result;
    public string? ErrorMessage { get; } = errorMessage;
}
