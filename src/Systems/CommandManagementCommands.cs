using Amethyst.Kernel;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Text;

namespace Amethyst.Systems;

public static class CommandManagementCommands
{
    [Command(["mcmd repos"], "amethyst.desc.commandManagementRepositories")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.repositories")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void CommandRepositories(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection collection = PagesCollection.AsListPage(CommandsOrganizer.Repositories.Select(p => p.Name), 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.noRepositories");
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.commandManagement.repositoriesTitle", null, null, false, page);
    }

    [Command(["mcmd repocmds"], "amethyst.desc.commandManagementRepositoryCommands")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.repositoryCommands")]
    [CommandSyntax("en-US", "<repository name>", "[page]")]
    [CommandSyntax("ru-RU", "<имя репозитория>", "[страница]")]
    public static void CommandRepositoryCommands(IAmethystUser user, CommandInvokeContext ctx, string repositoryName, int page = 0)
    {
        CommandRepository? repo = CommandsOrganizer.GetRepository(repositoryName);
        if (repo is null)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryNotFound", repositoryName);
            return;
        }

        PagesCollection collection = PagesCollection.AsListPage(repo.RegisteredCommands.Select(c => AmethystSession.Profile.CommandPrefix + c.Metadata.Names.First() ?? "<without_name>"), 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.noCommandsInRepository", repositoryName);
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.commandManagement.repositoryCommandsTitle", null, null, false, page);
    }

    [Command(["mcmd info"], "amethyst.desc.commandManagementInfo")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.info")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandInfo(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandInfo", AmethystSession.Profile.CommandPrefix + command.Metadata.Names.First() ?? "<without_name>");
                ctx.Messages.ReplyInfo("amethyst.commandManagement.commandDescription", command.Metadata.Description);
                if (command.Metadata.Syntax != null && command.Metadata.Syntax[ctx.Messages.Language] != null)
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandSyntax", string.Join(' ', command.Metadata.Syntax[ctx.Messages.Language]!));
                }
                else
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandNoSyntax");
                }
                ctx.Messages.ReplyInfo("amethyst.commandManagement.commandRepository", repo.Name);
                if (command.Metadata.Permission != null)
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandPermission", command.Metadata.Permission);
                }
                else
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandNoPermission");
                }
                if (command.Metadata.Rules.HasFlag(CommandRules.Disabled))
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandDisabled");
                }
                if (command.Metadata.Rules.HasFlag(CommandRules.Hidden))
                {
                    ctx.Messages.ReplyInfo("amethyst.commandManagement.commandHidden");
                }
                ctx.Messages.ReplyInfo("amethyst.commandManagement.preferredUser",
                    command.PreferredUser.GetType().Name);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd assignrepo"], "amethyst.desc.commandManagementAssignRepository")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.assignRepository")]
    [CommandSyntax("en-US", "<repository name>")]
    [CommandSyntax("ru-RU", "<имя репозитория>")]
    public static void CommandAssignRepository(IAmethystUser user, CommandInvokeContext ctx, string repositoryName)
    {
        if (repositoryName == "root")
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryRootNotAllowed");
            return;
        }

        CommandRepository? repo = CommandsOrganizer.GetRepository(repositoryName);
        if (repo is null)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryNotFound", repositoryName);
            return;
        }

        if (user.Commands.Repositories.Contains(repo.Name))
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryAlreadyAssigned", repo.Name);
            return;
        }

        user.Commands.Repositories.Add(repo.Name);
        ctx.Messages.ReplySuccess("amethyst.commandManagement.repositoryAssigned", repo.Name);
    }

    [Command(["mcmd unassignrepo"], "amethyst.desc.commandManagementUnassignRepository")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.unassignRepository")]
    [CommandSyntax("en-US", "<repository name>")]
    [CommandSyntax("ru-RU", "<имя репозитория>")]
    public static void CommandUnassignRepository(IAmethystUser user, CommandInvokeContext ctx, string repositoryName)
    {
        if (repositoryName == "root")
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryRootNotAllowed");
            return;
        }

        CommandRepository? repo = CommandsOrganizer.GetRepository(repositoryName);
        if (repo is null)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryNotFound", repositoryName);
            return;
        }

        if (!user.Commands.Repositories.Contains(repo.Name))
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.repositoryNotAssigned", repo.Name);
            return;
        }

        user.Commands.Repositories.Remove(repo.Name);
        ctx.Messages.ReplySuccess("amethyst.commandManagement.repositoryUnassigned", repo.Name);
    }

    [Command(["mcmd myrepos"], "amethyst.desc.commandManagementMyRepositories")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.myRepositories")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void CommandMyRepositories(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        if (user.Commands.Repositories.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.noAssignedRepositories");
            return;
        }

        PagesCollection collection = PagesCollection.AsListPage(user.Commands.Repositories, 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.commandManagement.noAssignedRepositories");
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.commandManagement.myRepositoriesTitle", null, null, false, page);
    }

    [Command(["mcmd disable", "mcmd disable"], "amethyst.desc.commandManagementSetDisabled")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.setDisabled")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandSetDisabled(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (command.Metadata.Rules.HasFlag(CommandRules.Disabled))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandAlreadyDisabled", commandName);
                    return;
                }

                command.Metadata = command.Metadata with { Rules = command.Metadata.Rules | CommandRules.Disabled };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandDisabled", commandName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd enable", "mcmd enable"], "amethyst.desc.commandManagementSetEnabled")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.setEnabled")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandSetEnabled(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (!command.Metadata.Rules.HasFlag(CommandRules.Disabled))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandAlreadyEnabled", commandName);
                    return;
                }

                command.Metadata = command.Metadata with { Rules = command.Metadata.Rules & ~CommandRules.Disabled };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandEnabled", commandName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd hide", "mcmd hide"], "amethyst.desc.commandManagementSetHidden")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.setHidden")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandSetHidden(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (command.Metadata.Rules.HasFlag(CommandRules.Hidden))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandAlreadyHidden", commandName);
                    return;
                }

                command.Metadata = command.Metadata with { Rules = command.Metadata.Rules | CommandRules.Hidden };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandHidden", commandName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd show", "mcmd show"], "amethyst.desc.commandManagementSetShown")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.setShown")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandSetShown(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (!command.Metadata.Rules.HasFlag(CommandRules.Hidden))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandAlreadyShown", commandName);
                    return;
                }

                command.Metadata = command.Metadata with { Rules = command.Metadata.Rules & ~CommandRules.Hidden };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandShown", commandName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd setperm"], "amethyst.desc.commandManagementSetPermission")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.setPermission")]
    [CommandSyntax("en-US", "<command name>", "[permission]")]
    [CommandSyntax("ru-RU", "<имя команды>", "[разрешение]")]
    public static void CommandSetPermission(IAmethystUser user, CommandInvokeContext ctx, string commandName, string? permission = null)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (permission == null)
                {
                    command.Metadata = command.Metadata with { Permission = null };
                    ctx.Messages.ReplySuccess("amethyst.commandManagement.commandPermissionRemoved", commandName);
                }
                else
                {
                    command.Metadata = command.Metadata with { Permission = permission };
                    ctx.Messages.ReplySuccess("amethyst.commandManagement.commandPermissionSet", commandName, permission);
                }
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd addname"], "amethyst.desc.commandManagementAddName")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.addName")]
    [CommandSyntax("en-US", "<command name>", "<new name>")]
    [CommandSyntax("ru-RU", "<имя команды>", "<новое имя>")]
    public static void CommandAddName(IAmethystUser user, CommandInvokeContext ctx, string commandName, string newName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (command.Metadata.Names.Contains(newName))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandNameAlreadyExists", newName);
                    return;
                }

                command.Metadata = command.Metadata with { Names = [.. command.Metadata.Names, newName] };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandNameAdded", commandName, newName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd remname"], "amethyst.desc.commandManagementRemoveName")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.removeName")]
    [CommandSyntax("en-US", "<command name>", "<name to remove>")]
    [CommandSyntax("ru-RU", "<имя команды>", "<имя для удаления>")]
    public static void CommandRemoveName(IAmethystUser user, CommandInvokeContext ctx, string commandName, string nameToRemove)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (!command.Metadata.Names.Contains(nameToRemove))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandNameNotFound", nameToRemove);
                    return;
                }

                command.Metadata = command.Metadata with { Names = [.. command.Metadata.Names.Where(n => n != nameToRemove)] };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandNameRemoved", commandName, nameToRemove);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd rename"], "amethyst.desc.commandManagementRenameCommand")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.renameCommand")]
    [CommandSyntax("en-US", "<command name>", "<new name>")]
    [CommandSyntax("ru-RU", "<имя команды>", "<новое имя>")]
    public static void CommandRenameCommand(IAmethystUser user, CommandInvokeContext ctx, string commandName, string newName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (command.Metadata.Names.Contains(newName))
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandNameAlreadyExists", newName);
                    return;
                }

                command.Metadata = command.Metadata with { Names = [newName] };
                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandRenamed", commandName, newName);
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }

    [Command(["mcmd names"], "amethyst.desc.commandManagementCommandNames")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.commandManagement.commandNames")]
    [CommandSyntax("en-US", "<command name>")]
    [CommandSyntax("ru-RU", "<имя команды>")]
    public static void CommandNames(IAmethystUser user, CommandInvokeContext ctx, string commandName)
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            ICommand? command = repo.GetCommand(commandName);
            if (command != null)
            {
                if (command.Metadata.Names.Length == 0)
                {
                    ctx.Messages.ReplyError("amethyst.commandManagement.commandNoNames", commandName);
                    return;
                }

                ctx.Messages.ReplySuccess("amethyst.commandManagement.commandNames", commandName, string.Join(", ", command.Metadata.Names));
                return;
            }
        }

        ctx.Messages.ReplyError("amethyst.commandManagement.commandNotFound", commandName);
    }
}
