namespace Amethyst.Extensions;

public static class ExtensionStateExtensions
{
    public static string ToFriendlyString(this ExtensionState state)
    {
        return state switch
        {
            ExtensionState.Initialized => "Initialized",
            ExtensionState.Deinitialized => "Deinitialized",
            ExtensionState.NotAllowed => "Not Allowed",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}
