using Amethyst.Extensions.Plugins;
using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Extensions;

[AutoloadHook]
public sealed class PluginPreloadArgs(PluginInstance pluginInstance)
{
    public PluginInstance Instance { get; } = pluginInstance;
}
