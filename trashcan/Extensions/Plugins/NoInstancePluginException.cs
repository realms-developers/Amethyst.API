namespace Amethyst.Extensions.Plugins;

public sealed class NoInstancePluginException(string name) : Exception($"Cannot find AmethystPlugin instance in '{name}'")
{
}
