using System.CommandLine;
using Amethyst.Kernel.CLI.LaunchConfiguration;
using Amethyst.Kernel.Profiles;
using Amethyst.Kernel;
using Amethyst.Kernel.CLI;

namespace Amethyst.Kernel.Kernel;

internal static class AmethystKernel
{
    internal static ServerProfile Profile = null!;

    public static void Main(string[] args)
    {
        SharedDependencyContext.Instance.PreloadAllDependencies();

        ConsoleHooks.AttachHooks();
        RootCommand rootCommand = CommandConfiguration.BuildRootCommand();

        rootCommand.Invoke(args);

        if (Profile != null)
        {
            InitializeServer(Profile);
            return;
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
            if (ex.Exception is OperationCanceledException || ex.Exception is ObjectDisposedException)
            {
                return;
            }

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
