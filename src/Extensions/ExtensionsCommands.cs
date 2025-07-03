using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Text;

namespace Amethyst.Extensions;

public static class ExtensionsCommands
{
    [Command(["plugins list"], "amethyst.desc.extensionsPluginsList")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.list")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void PluginsList(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        var collection = PagesCollection.AsListPage(repository.Extensions.Select(e => e.Metadata.Name), 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.extensions.noPluginsLoaded");
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.extensions.pluginsListTitle", null, null, false, page);
    }

    [Command(["plugins allow"], "amethyst.desc.extensionsPluginsAllow")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.allow")]
    [CommandSyntax("en-US", "<plugin name>")]
    [CommandSyntax("ru-RU", "<имя плагина>")]
    public static void PluginsAllow(IAmethystUser user, CommandInvokeContext ctx, string pluginName)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        if (repository.Ruler.IsExtensionAllowed(pluginName))
        {
            ctx.Messages.ReplyError("amethyst.extensions.pluginAlreadyAllowed", pluginName);
            return;
        }
        repository.Ruler.AllowExtension(pluginName);
        ctx.Messages.ReplySuccess("amethyst.extensions.pluginAllowed", pluginName);
    }

    [Command(["plugins disallow"], "amethyst.desc.extensionsPluginsDisallow")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.disallow")]
    [CommandSyntax("en-US", "<plugin name>")]
    [CommandSyntax("ru-RU", "<имя плагина>")]
    public static void PluginsDisallow(IAmethystUser user, CommandInvokeContext ctx, string pluginName)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        if (!repository.Ruler.IsExtensionAllowed(pluginName))
        {
            ctx.Messages.ReplyError("amethyst.extensions.pluginAlreadyDisallowed", pluginName);
            return;
        }
        repository.Ruler.DisallowExtension(pluginName);
        ctx.Messages.ReplySuccess("amethyst.extensions.pluginDisallowed", pluginName);
    }

    [Command(["plugins reload"], "amethyst.desc.extensionsPluginsReload")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.reload")]
    public static void PluginsReload(IAmethystUser user, CommandInvokeContext ctx)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        ExtensionsOrganizer.UnloadPlugins();
        ExtensionsOrganizer.LoadPlugins();

        ctx.Messages.ReplySuccess("amethyst.extensions.pluginsRequestedReload");
    }

    [Command(["plugins load"], "amethyst.desc.extensionsPluginsLoad")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.load")]
    public static void PluginsLoad(IAmethystUser user, CommandInvokeContext ctx)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        ExtensionsOrganizer.LoadPlugins();
        ctx.Messages.ReplySuccess("amethyst.extensions.pluginsRequestedLoad");
    }

    [Command(["plugins unload"], "amethyst.desc.extensionsPluginsUnload")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.plugins.unload")]
    public static void PluginsUnload(IAmethystUser user, CommandInvokeContext ctx)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Plugins.GetRepository("plg.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "plg.main");
            return;
        }

        ExtensionsOrganizer.UnloadPlugins();
        ctx.Messages.ReplySuccess("amethyst.extensions.pluginsRequestedUnload");
    }

    [Command(["modules list"], "amethyst.desc.extensionsModulesList")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.modules.list")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void ModulesList(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Modules.GetRepository("mdl.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "mdl.main");
            return;
        }

        var collection = PagesCollection.AsListPage(repository.Extensions.Select(e => e.Metadata.Name), 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.extensions.noModulesLoaded");
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.extensions.modulesListTitle", null, null, false, page);
    }

    [Command(["modules allow"], "amethyst.desc.extensionsModulesAllow")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.modules.allow")]
    [CommandSyntax("en-US", "<module name>")]
    [CommandSyntax("ru-RU", "<имя модуля>")]
    public static void ModulesAllow(IAmethystUser user, CommandInvokeContext ctx, string moduleName)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Modules.GetRepository("mdl.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "mdl.main");
            return;
        }

        if (repository.Ruler.IsExtensionAllowed(moduleName))
        {
            ctx.Messages.ReplyError("amethyst.extensions.moduleAlreadyAllowed", moduleName);
            return;
        }
        repository.Ruler.AllowExtension(moduleName);
        ctx.Messages.ReplySuccess("amethyst.extensions.moduleAllowed", moduleName);
    }

    [Command(["modules disallow"], "amethyst.desc.extensionsModulesDisallow")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.extensions.modules.disallow")]
    [CommandSyntax("en-US", "<module name>")]
    [CommandSyntax("ru-RU", "<имя модуля>")]
    public static void ModulesDisallow(IAmethystUser user, CommandInvokeContext ctx, string moduleName)
    {
        Base.Repositories.IExtensionRepository? repository = ExtensionsOrganizer.Modules.GetRepository("mdl.main");
        if (repository == null)
        {
            ctx.Messages.ReplyError("amethyst.extensions.repositoryNotFound", "mdl.main");
            return;
        }

        if (!repository.Ruler.IsExtensionAllowed(moduleName))
        {
            ctx.Messages.ReplyError("amethyst.extensions.moduleAlreadyDisallowed", moduleName);
            return;
        }
        repository.Ruler.DisallowExtension(moduleName);
        ctx.Messages.ReplySuccess("amethyst.extensions.moduleDisallowed", moduleName);
    }
}
