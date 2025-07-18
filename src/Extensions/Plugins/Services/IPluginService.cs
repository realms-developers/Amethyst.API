namespace Amethyst.Extensions.Plugins.Services;

public interface IPluginService
{
    PluginInstance BaseInstance { get; }

    void OnPluginLoad();
    void OnPluginPostLoad();
    void OnPluginUnload();
    void OnPluginPostUnload();
}
