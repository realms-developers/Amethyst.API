using Amethyst.Commands.Attributes;
using Amethyst.Core;
using Amethyst.Core.Server;
using Amethyst.Extensions.Modules;
using Amethyst.Extensions.Plugins;
using Amethyst.Text;

namespace Amethyst.Commands.Implementations;

public static class ExtensionsCommands
{
    #region Plugins
    [ServerCommand(CommandType.Shared, "plugins list", "commands.desc.pluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(PluginLoader.Containers
            .Where(p => p.PluginInstance != null)
            .Select(p => $"{p.PluginInstance!.Name} ({p.LoadID})"));

        ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.loadedPlugins", ctx.Sender.Language), null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins allowlist", "commands.desc.allowedPluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsAllowList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(AmethystSession.ExtensionsConfiguration.AllowedPlugins);

        ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.allowedPlugins", ctx.Sender.Language), null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins setallow", "commands.desc.setallowPlugin", "amethyst.management.extensions")]
    [CommandsSyntax("<name with .dll>", "<true | false>")]
    public static void PluginsAllow(CommandInvokeContext ctx, string name, bool value)
    {
        AmethystSession.Profile.Config.Get<ExtensionsConfiguration>().Modify((ref ExtensionsConfiguration p) =>
        {
            if (value)
            {
                p.AllowedPlugins.Add(name);
            }
            else
            {
                p.AllowedPlugins.Remove(name);
            }
        }, true);

        ctx.Sender.ReplySuccess(Localization.Get(value ? "commands.text.extensionWasAllowed" : "commands.text.extensionWasDisallowed", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "plugins unload", "commands.desc.unloadPlugin", "amethyst.management.extensions")]
    public static void PluginsUnload(CommandInvokeContext ctx)
    {
        List<PluginContainer> containers = PluginLoader.Containers;
        containers.ForEach(p => p.Dispose());

        ctx.Sender.ReplySuccess(Localization.Get("commands.text.pluginsWasUnloaded", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "plugins load", "commands.desc.loadPlugins", "amethyst.management.extensions")]
    public static void PluginsLoad(CommandInvokeContext ctx)
    {
        PluginLoader.LoadPlugins();
        ctx.Sender.ReplySuccess(Localization.Get("commands.text.pluginsWasLoaded", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "plugins reload", "commands.desc.reloadPlugins", "amethyst.management.extensions")]
    public static void PluginsReload(CommandInvokeContext ctx)
    {
        List<PluginContainer> containers = PluginLoader.Containers;
        containers.ForEach(p => p.Dispose());
        PluginLoader.LoadPlugins();
        ctx.Sender.ReplySuccess(Localization.Get("commands.text.pluginsWasReloaded", ctx.Sender.Language));
    }

    #endregion

    #region Modules
    [ServerCommand(CommandType.Shared, "modules list", "commands.desc.moduleList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(ModuleLoader.Modules.Select(p => p.Name));

        ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.loadedModules", ctx.Sender.Language), null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules allowlist", "commands.desc.allowedModulesList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesAllowList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(AmethystSession.ExtensionsConfiguration.AllowedModules);

        ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.allowedModules", ctx.Sender.Language), null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules setallow", "commands.desc.setallowModule", "amethyst.management.extensions")]
    [CommandsSyntax("<name with .dll>", "<true | false>")]
    public static void ModulesAllow(CommandInvokeContext ctx, string name, bool value)
    {
        AmethystSession.Profile.Config.Get<ExtensionsConfiguration>().Modify((ref ExtensionsConfiguration p) =>
        {
            if (value)
            {
                p.AllowedModules.Add(name);
            }
            else
            {
                p.AllowedModules.Remove(name);
            }
        }, true);

        ctx.Sender.ReplySuccess(Localization.Get(value ? "commands.text.extensionWasAllowed" : "commands.text.extensionWasDisallowed", ctx.Sender.Language));

        ctx.Sender.ReplyWarning(Localization.Get("commands.text.pleaseRebootServer", ctx.Sender.Language));
    }

    #endregion
}
