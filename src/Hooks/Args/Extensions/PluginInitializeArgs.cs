using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Plugins;
using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Extensions;

[AutoloadHook]
public sealed class PluginInitializeArgs(PluginInstance pluginInstance, ExtensionHandleResult result)
{
    public PluginInstance Instance { get; } = pluginInstance;
    public ExtensionHandleResult Result { get; } = result;
}
