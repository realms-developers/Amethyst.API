namespace Amethyst.Extensions.Plugins;

public interface IPluginService
{
    PluginInstance BaseInstance { get; }

    void OnPluginLoad();
    void OnPluginUnload();
}
