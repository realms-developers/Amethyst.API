using Amethyst.Commands;
using Amethyst.Core;

namespace Amethyst.Extensions.Plugins;

public abstract class PluginInstance
{
    public abstract string Name { get; }
    public abstract Version Version { get; }
    public virtual string? Description { get; }

    public bool IsLoaded { get; private set; }

    public int LoadID => Container.LoadID;

    internal PluginContainer Container = null!;

    protected abstract void Load();
    protected abstract void Unload();

    public bool RegisterCommand(CommandData data)
    {
        if (CommandsManager.FindCommand(data.Name) != null)
        {
            return false;
        }

        CommandData commandData = new(LoadID, data.Name, data.Description,
            data.Method, data.Settings, data.Type,
            data.Permission, data.Syntax);


        CommandsManager.Commands.Add(new(commandData));

        return true;
    }

    public bool UnregisterCommand(string name)
    {
        CommandRunner? command = CommandsManager.FindCommand(name);

        if (command == null || command.Data.PluginID != LoadID)
        {
            return false;
        }

        return CommandsManager.Commands.Remove(command);
    }

    /// <summary>
    /// Called automatically on unload
    /// </summary>
    public int UnregisterCommands() => CommandsManager.Commands.RemoveAll(p => p.Data.PluginID == LoadID);

    internal bool RequestLoad()
    {
        IsLoaded = true;
        PluginLoader.InvokeLoad(Container!);
        return RequestOperation("Load", Load);
    }

    internal bool RequestUnload()
    {
        IsLoaded = false;
        PluginLoader.InvokeUnload(Container!);
        return RequestOperation("Unload", Unload);
    }

    private bool RequestOperation(string logName, Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (FileNotFoundException fnfe)
        {
            string errorMessage = $"Failed to {logName} plugin '{Name}'. Missing dependency: {Path.GetFileNameWithoutExtension(fnfe.FileName)}.dll\n" +
                             $"Please ensure '{fnfe.FileName}' is present in the dependencies directory.";

            if (logName == "Load" && PluginLoader.InFirstLoadStage)
            {
                PluginLoader.LogLoaded.Remove(Container.FileName);
                PluginLoader.LogFailed.Add(Container.FileName, new FileNotFoundException(errorMessage, fnfe));
            }
            else
            {
                AmethystLog.Main.Critical(nameof(PluginInstance), errorMessage);
            }
            return false;
        }
        catch (Exception ex)
        {
            if (logName == "Load" && PluginLoader.InFirstLoadStage)
            {
                PluginLoader.LogLoaded.Remove(Container.FileName);
                PluginLoader.LogFailed.Add(Container.FileName, ex);
            }
            else
            {
                AmethystLog.Main.Critical(nameof(PluginInstance), $"Failed to process '{logName}' operation for '{Name}':");
                AmethystLog.Main.Critical(nameof(PluginInstance), ex.ToString());
            }
        }

        return false;
    }
}
