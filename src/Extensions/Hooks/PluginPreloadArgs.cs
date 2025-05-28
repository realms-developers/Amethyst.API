using Amethyst.Extensions.Plugins;
using Amethyst.Hooks.Autoloading;

namespace Amethyst.Extensions.Hooks;

[AutoloadHook]
public sealed class PluginPreloadArgs
{
    public PluginPreloadArgs(PluginInstance pluginInstance)
    {
        Instance = pluginInstance;
    }

    public PluginInstance Instance { get; }
}
