using System.Reflection;
using Amethyst.Core.Arguments;
using Amethyst.Core.Profiles;
using Amethyst.Logging;

namespace Amethyst.Core;

internal static class AmethystKernel
{
    internal static ServerProfile? Profile;

    internal static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate (object? sender, ResolveEventArgs sargs)
        {
            string resourceName = new AssemblyName(sargs.Name).Name + ".dll";

            string path = Path.Combine("deps", resourceName);

            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };

        ArgumentsHandler.Initialize();
        ExecuteArguments(args);

        if (Profile != null)
        {
            InitializeServer(Profile);
            return;
        }

        ModernConsole.WriteLine($"$!bAmethyst Terraria Server API v{typeof(AmethystKernel).Assembly.GetName().Version}");
        ModernConsole.WriteLine($"$wAvailable commands:");

        foreach (KeyValuePair<string, ArgumentCommandInfo> kvp in ArgumentsHandler.RegisteredCommands)
        {
            ModernConsole.WriteLine($"$!b{kvp.Key} $!r- $!d{kvp.Value.Description}");
        }
    }

    private static void InitializeServer(ServerProfile profile)
    {
        if (!Directory.Exists(profile.SavePath))
        {
            Directory.CreateDirectory(profile.SavePath);
        }

        AppDomain.CurrentDomain.FirstChanceException += (sender, ex) =>
        {
            AmethystLog.Startup.Error("ModuleLoader", $"Caught first-chance exception:");
            AmethystLog.Startup.Error("ModuleLoader", ex.Exception.ToString() ?? "No data");
        };
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            AmethystLog.Startup.Critical("ModuleLoader", $"Caught unhandled exception:");
            AmethystLog.Startup.Critical("ModuleLoader", ex.ExceptionObject.ToString() ?? "No data");
            AmethystLog.Startup.Critical("ModuleLoader", $"Server is terminated.");

            Thread.Sleep(-1);
        };

        AmethystSession.StartServer();
    }

    private static void ExecuteArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args.Length > i + 1)
            {
                string cmd = args[i];
                string arg = args[i + 1];

                ArgumentsHandler.HandleCommand(cmd, arg);
            }
        }
    }
}
