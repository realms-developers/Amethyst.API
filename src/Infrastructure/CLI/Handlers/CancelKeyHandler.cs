namespace Amethyst.Infrastructure.CLI.Handlers;

internal static class CancelKeyHandler
{
    internal static void Initialize()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;

            AmethystSession.StopServer();
        };
    }
}
