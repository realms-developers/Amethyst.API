using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Plugins;
using Amethyst.Hooks.Autoloading;

namespace Amethyst.Extensions.Hooks;

[AutoloadHook]
public sealed class PluginDeinitializeArgs
{
    public PluginDeinitializeArgs(PluginInstance pluginInstance, ExtensionHandleResult result)
    {
        Instance = pluginInstance;
        Result = result;
    }

    public PluginInstance Instance { get; }
    public ExtensionHandleResult Result { get; }
}
