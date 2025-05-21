using Amethyst.Systems.Commands;

namespace Amethyst.Extensions.Plugins.Services;

public sealed class CommandsService : IPluginService
{
    internal CommandsService(PluginInstance baseInstance)
    {
        BaseInstance = baseInstance;
    }

    public PluginInstance BaseInstance { get; }

    public void OnPluginLoad() => CommandsManager.ImportCommands(BaseInstance.Root.Assembly, BaseInstance.Root.LoadIdentifier);

    public void OnPluginUnload() => CommandsManager.Commands.RemoveAll(p => p.Data.PluginIdentifier == BaseInstance.Root.LoadIdentifier);

    public bool RegisterCommand(CommandData data)
    {
        if (CommandsManager.FindCommand(data.Name) != null)
        {
            return false;
        }

        CommandData commandData = new(BaseInstance.Root.LoadIdentifier, data.Name, data.Description,
            data.Method, data.Settings, data.Type,
            data.Permission, data.Syntax);


        CommandsManager.Commands.Add(new(commandData));

        return true;
    }

    public bool UnregisterCommand(string name)
    {
        CommandRunner? command = CommandsManager.FindCommand(name);

        return command == null || command.Data.PluginIdentifier != BaseInstance.Root.LoadIdentifier
            ? false
            : CommandsManager.Commands.Remove(command);
    }
}
