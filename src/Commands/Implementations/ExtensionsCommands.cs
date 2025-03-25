using Amethyst.Core;
using Amethyst.Core.Server;
using Amethyst.Extensions.Modules;
using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Amethyst.Text;

namespace Amethyst.Commands.Implementations;

public static class ExtensionsCommands
{
    [ServerCommand(CommandType.Shared, "plugins list", "$LOCALIZE commands.desc.pluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(PluginLoader.Containers
            .Where(p => p.PluginInstance != null)
            .Select(p => $"{p.PluginInstance!.Name} ({p.LoadID})"));

        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.loadedPlugins", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins allowlist", "$LOCALIZE commands.desc.allowedPluginsList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void PluginsAllowList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(AmethystSession.ExtensionsConfiguration.AllowedPlugins);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.allowedPlugins", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "plugins setallow", "$LOCALIZE commands.desc.setallowPlugin", "amethyst.management.extensions")]
    [CommandsSyntax("<name with .dll>", "<true | false>")]
    public static void PluginsAllow(CommandInvokeContext ctx, string name, bool value)
    {
        AmethystSession.Profile.Config.Get<ExtensionsConfiguration>().Modify(p =>
        {
            if (p.AllowedPlugins.Contains(name) && value == false)
                p.AllowedPlugins.Remove(name);
            else if (p.AllowedPlugins.Contains(name) == false && value)
                p.AllowedPlugins.Add(name);

            return p;
        }, true);

        if (value) ctx.Sender.ReplySuccess("$LOCALIZE commands.text.extensionWasAllowed");
        else       ctx.Sender.ReplySuccess("$LOCALIZE commands.text.extensionWasDisallowed");
    }

    [ServerCommand(CommandType.Shared, "plugins unload", "$LOCALIZE commands.desc.unload", "amethyst.management.extensions")]
    public static void PluginsUnload(CommandInvokeContext ctx)
    {
        var containers = PluginLoader.Containers;
        containers.ForEach(p => p.Dispose());

        ctx.Sender.ReplySuccess("$LOCALIZE commands.text.pluginsWasUnloaded");
    }

    [ServerCommand(CommandType.Shared, "plugins load", "$LOCALIZE commands.desc.load", "amethyst.management.extensions")]
    public static void PluginsLoad(CommandInvokeContext ctx)
    {
        PluginLoader.LoadPlugins();
        ctx.Sender.ReplySuccess("$LOCALIZE commands.text.pluginsWasLoaded");
    }

    [ServerCommand(CommandType.Shared, "plugins reload", "$LOCALIZE commands.desc.reload", "amethyst.management.extensions")]
    public static void PluginsReload(CommandInvokeContext ctx)
    {
        var containers = PluginLoader.Containers;
        containers.ForEach(p => p.Dispose());
        PluginLoader.LoadPlugins();
        ctx.Sender.ReplySuccess("$LOCALIZE commands.text.pluginsWasReloaded");
    }

    [ServerCommand(CommandType.Shared, "modules list", "$LOCALIZE commands.desc.moduleList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(ModuleLoader.Modules.Select(p => p.Name));

        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.loadedModules", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules allowlist", "$LOCALIZE commands.desc.allowedModulesList", "amethyst.management.extensions")]
    [CommandsSyntax("[page]")]
    public static void ModulesAllowList(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(AmethystSession.ExtensionsConfiguration.AllowedModules);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.allowedModules", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "modules setallow", "$LOCALIZE commands.desc.setallowModule", "amethyst.management.extensions")]
    [CommandsSyntax("<name with .dll>", "<true | false>")]
    public static void ModulesAllow(CommandInvokeContext ctx, string name, bool value)
    {
        AmethystSession.Profile.Config.Get<ExtensionsConfiguration>().Modify(p =>
        {
            if (p.AllowedModules.Contains(name) && value == false)
                p.AllowedModules.Remove(name);
            else if (p.AllowedModules.Contains(name) == false && value)
                p.AllowedModules.Add(name);

            return p;
        }, true);

        if (value) ctx.Sender.ReplySuccess("$LOCALIZE commands.text.extensionWasAllowed");
        else       ctx.Sender.ReplySuccess("$LOCALIZE commands.text.extensionWasDisallowed");

        ctx.Sender.ReplyWarning("$LOCALIZE commands.text.pleaseRebootServer");
    }
}