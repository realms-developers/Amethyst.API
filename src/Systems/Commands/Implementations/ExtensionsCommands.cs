using Amethyst.Extensions;
using Amethyst.Systems.Commands.Attributes;
using Amethyst.Text;

namespace Amethyst.Systems.Commands.Implementations;

public static class ExtensionsCommands
{
    #region Plugins
    [ServerCommand(CommandType.Shared, "plugins list", "commands.desc.pluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(ExtensionsOrganizer.Plugins.Repositories
            .SelectMany(r => r.Extensions)
            .Select(e => $"{e.Metadata.Name} ({e.LoadIdentifier})"));

        ctx.Sender.ReplyPage(pages, "commands.text.loadedPlugins", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins allowlist", "commands.desc.allowedPluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsAllowList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(ExtensionsOrganizer.Plugins.Repositories
            .SelectMany(r => r.Ruler.AllowedExtensions));

        ctx.Sender.ReplyPage(pages, "commands.text.allowedPlugins", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins toggle", "commands.desc.setallowPlugin", "amethyst.management.extensions")]
    [CommandsSyntax("<name>")]
    public static void PluginsAllow(CommandInvokeContext ctx, string name)
    {
        bool isNowAllowed = ExtensionsOrganizer.Plugins.Repositories[0].Ruler.ToggleExtension(name);

        ctx.Sender.ReplySuccess(Localization.Get(isNowAllowed ? "commands.text.extensionWasAllowed" : "commands.text.extensionWasDisallowed", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "plugins unload", "commands.desc.unloadPlugin", "amethyst.management.extensions")]
    public static void PluginsUnload(CommandInvokeContext ctx)
    {
        ExtensionsOrganizer.UnloadPlugins();

        ctx.Sender.ReplySuccess("commands.text.pluginsWasUnloaded");
    }

    [ServerCommand(CommandType.Shared, "plugins load", "commands.desc.loadPlugins", "amethyst.management.extensions")]
    public static void PluginsLoad(CommandInvokeContext ctx)
    {
        ExtensionsOrganizer.LoadPlugins();
        ctx.Sender.ReplySuccess("commands.text.pluginsWasLoaded");
    }

    [ServerCommand(CommandType.Shared, "plugins reload", "commands.desc.reloadPlugins", "amethyst.management.extensions")]
    public static void PluginsReload(CommandInvokeContext ctx)
    {
        ExtensionsOrganizer.UnloadPlugins();
        ExtensionsOrganizer.LoadPlugins();

        ctx.Sender.ReplySuccess("commands.text.pluginsWasReloaded");
    }

    #endregion

    #region Modules
    [ServerCommand(CommandType.Shared, "modules list", "commands.desc.moduleList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(ExtensionsOrganizer.Modules.Repositories
            .SelectMany(r => r.Extensions)
            .Select(e => e.Metadata.Name));

        ctx.Sender.ReplyPage(pages, "commands.text.loadedModules", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules allowlist", "commands.desc.allowedModulesList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesAllowList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(ExtensionsOrganizer.Modules.Repositories
            .SelectMany(r => r.Ruler.AllowedExtensions));

        ctx.Sender.ReplyPage(pages, "commands.text.allowedModules", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules toggle", "commands.desc.setallowModule", "amethyst.management.extensions")]
    [CommandsSyntax("<name>")]
    public static void ModulesAllow(CommandInvokeContext ctx, string name)
    {
        bool isNowAllowed = ExtensionsOrganizer.Modules.Repositories[0].Ruler.ToggleExtension(name);

        ctx.Sender.ReplySuccess(Localization.Get(isNowAllowed ? "commands.text.extensionWasAllowed" : "commands.text.extensionWasDisallowed", ctx.Sender.Language));

        ctx.Sender.ReplyWarning("commands.text.pleaseRebootServer");
    }

    #endregion
}
