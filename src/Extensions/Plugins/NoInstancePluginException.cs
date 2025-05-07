namespace Amethyst.Extensions.Plugins;

public sealed class NoInstancePluginException : Exception
{
    public NoInstancePluginException(string name) : base($"Cannot find AmethystPlugin instance in '{name}'")
    {

    }
}
