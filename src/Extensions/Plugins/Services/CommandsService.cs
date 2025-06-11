using Amethyst.Systems.Commands;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic;
using Amethyst.Systems.Commands.Dynamic.Utilities;

namespace Amethyst.Extensions.Plugins.Services;

public sealed class CommandsService : IPluginService
{
    internal CommandsService(PluginInstance baseInstance)
    {
        BaseInstance = baseInstance;
    }

    public PluginInstance BaseInstance { get; }

    public void OnPluginLoad()
    {
        ImportUtility.ImportFrom(BaseInstance.Root.Assembly, BaseInstance.Root.LoadIdentifier);
    }

    public void OnPluginUnload()
    {
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            foreach (ICommand command in repo.RegisteredCommands)
            {
                if (command is DynamicCommand dynamicCommand && dynamicCommand.LoadIdentifier == BaseInstance.Root.LoadIdentifier)
                {
                    repo.Remove(dynamicCommand);
                }
            }
        }
    }
}
