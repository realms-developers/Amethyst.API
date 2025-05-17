using System.CommandLine;
using System.Reflection;
using Amethyst.Infrastructure.CLI.Handlers;
using Amethyst.Infrastructure.CLI.Input;
using Amethyst.Infrastructure.CLI.LaunchConfiguration;
using Amethyst.Infrastructure.Core.Profiles;

namespace Amethyst.Infrastructure.Kernel;

internal static class AmethystKernel
{
    internal static ServerProfile Profile = null!;

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "deps", assemblyName);
            return File.Exists(path) ? Assembly.LoadFile(path) : null;
        };

        InitializeConsole();
        RootCommand rootCommand = CommandConfiguration.BuildRootCommand();

        rootCommand.Invoke(args);

        if (Profile != null)
        {
            InitializeServer(Profile);
            return;
        }
    }

    private static void InitializeConsole()
    {
        CancelKeyHandler.Initialize();
        CliInputHandler.Initialize();
    }

    private static void InitializeServer(ServerProfile profile)
    {
        if (!Directory.Exists(profile.SavePath))
        {
            Directory.CreateDirectory(profile.SavePath);
        }

        AppDomain.CurrentDomain.FirstChanceException += (sender, ex) =>
        {
            AmethystLog.Startup.Error(nameof(AmethystKernel), "Caught first-chance exception:");
            AmethystLog.Startup.Error(nameof(AmethystKernel), ex.Exception.ToString() ?? "No data");
        };
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            AmethystLog.Startup.Critical(nameof(AmethystKernel), "Caught unhandled exception:");
            AmethystLog.Startup.Critical(nameof(AmethystKernel), ex.ExceptionObject.ToString() ?? "No data");
            AmethystLog.Startup.Critical(nameof(AmethystKernel), "Server is terminated. Use Ctrl + C to forcefully stop it.");

            Environment.Exit(-1);
        };

        AmethystSession.StartServer();
    }
}
